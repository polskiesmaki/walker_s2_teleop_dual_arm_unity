using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class MoveItDualArmPublisher : MonoBehaviour
{
    private ROSConnection ros;
    public string leftArmTopic = "/unity/left_target_pose";
    public string rightArmTopic = "/unity/right_target_pose";

    public GameObject leftController;
    public GameObject rightController;

    // =========================================================
    // POZYCJA "BOKSERA" - Bezpieczna pozycja startowa
    // =========================================================
    // X (przód): mocno przed robota (40 cm)
    // Y (boki): rozsądnie na boki (30 cm lewo, -30 cm prawo)
    // Z (góra): wysoko! (20 cm od miednicy)
    // Zmień w MoveItDualArmPublisher.cs
    private Vector3 safeLeftRobotPos = new Vector3(0.40f, 0.20f, 0.10f);   
    private Vector3 safeRightRobotPos = new Vector3(0.40f, -0.20f, 0.10f); // Y ustawione na +/- 0.20

    // Ustawiam rotację IDEALNIE na wprost, dłońmi w dół
    private QuaternionMsg relaxedOrientation = new QuaternionMsg 
    { 
        x = 0.0, y = 0.707, z = 0.0, w = 0.707 
    };

    private Vector3 initialLeftPos;
    private Vector3 initialRightPos;

    private float publishRate = 0.03f;
    private float nextPublishTime = 0f;

    private bool isCalibrated = false;
    private float calibrationTimer = 5.0f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseMsg>(leftArmTopic);
        ros.RegisterPublisher<PoseMsg>(rightArmTopic);
    }

    void Update()
    {
        if (!isCalibrated)
        {
            calibrationTimer -= Time.deltaTime;
            
            if (leftController != null) initialLeftPos = leftController.transform.position;
            if (rightController != null) initialRightPos = rightController.transform.position;

            if (calibrationTimer <= 0)
            {
                isCalibrated = true;
                Debug.Log("=== KALIBRACJA ZAKOŃCZONA - WYSYŁAMY DANE ===");
            }
            return; 
        }

        if (Time.time < nextPublishTime) return;
        bool dataSent = false;

        if (leftController != null)
        {
            PublishRelativePose(leftController, initialLeftPos, safeLeftRobotPos, leftArmTopic);
            dataSent = true;
        }

        if (rightController != null)
        {
            PublishRelativePose(rightController, initialRightPos, safeRightRobotPos, rightArmTopic);
            dataSent = true;
        }

        if (dataSent) nextPublishTime = Time.time + publishRate;
    }

    void PublishRelativePose(GameObject controller, Vector3 initialControllerPos, Vector3 safeRobotPos, string topic)
    {
        Vector3 deltaUnity = controller.transform.position - initialControllerPos;
        Vector3 deltaROS = new Vector3(deltaUnity.z, -deltaUnity.x, deltaUnity.y);

        // KLATKA OCHRONNA: 30 cm w każdą stronę maksymalnego wychylenia
        float maxReach = 0.30f;
        deltaROS.x = Mathf.Clamp(deltaROS.x, -maxReach, maxReach); 
        deltaROS.y = Mathf.Clamp(deltaROS.y, -maxReach, maxReach); 
        deltaROS.z = Mathf.Clamp(deltaROS.z, -maxReach, maxReach); 

        Vector3 finalTargetPos = safeRobotPos + deltaROS;

        // Ochrona przed wbiciem w tułów: X NIGDY nie schodzi poniżej 15cm -> 25 cm (change) od osi
        if(finalTargetPos.x < 0.15f) finalTargetPos.x = 0.15f;

        PoseMsg msg = new PoseMsg
        {
            position = new PointMsg { x = finalTargetPos.x, y = finalTargetPos.y, z = finalTargetPos.z },
            orientation = relaxedOrientation 
        };

        ros.Publish(topic, msg);
    }
}
