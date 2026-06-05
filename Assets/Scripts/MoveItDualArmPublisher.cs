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

    // Bezpieczna pozycja początkowa robota
    private Vector3 safeLeftRobotPos = new Vector3(0.4f, 0.3f, 0.2f);   
    private Vector3 safeRightRobotPos = new Vector3(0.4f, -0.3f, 0.2f); 

    private Vector3 initialLeftPos;
    private Vector3 initialRightPos;

    private float publishRate = 0.1f; // 10 pakietów na sekundę
    private float nextPublishTime = 0f;

    // KALIBRACJA
    private bool isCalibrated = false;
    private float calibrationTimer = 5.0f; // 5 sekund na przygotowanie po starcie!

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<PoseMsg>(leftArmTopic);
        ros.RegisterPublisher<PoseMsg>(rightArmTopic);
    }

    void Update()
    {
        // 1. FAZA KALIBRACJI: Czekamy 5 sekund, zanim zaczniemy liczyć ruch
        if (!isCalibrated)
        {
            calibrationTimer -= Time.deltaTime;
            
            // W trakcie odliczania ciągle aktualizujemy pozycję bazową
            if (leftController != null) initialLeftPos = leftController.transform.position;
            if (rightController != null) initialRightPos = rightController.transform.position;

            if (calibrationTimer <= 0)
            {
                isCalibrated = true;
                Debug.Log("=== KALIBRACJA ZAKOŃCZONA - WYSYŁAMY DANE ===");
            }
            return; // Przerywamy Update - nie wysyłamy danych, dopóki timer nie minie
        }

        // 2. FAZA WYSYŁANIA (Z OGRANICZNIKIEM 0.1s)
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
        
        // Translacja współrzędnych z Unity do ROS 2
        Vector3 deltaROS = new Vector3(deltaUnity.z, -deltaUnity.x, deltaUnity.y);
        Vector3 finalTargetPos = safeRobotPos + deltaROS;

        PoseMsg msg = new PoseMsg
        {
            position = new PointMsg { x = finalTargetPos.x, y = finalTargetPos.y, z = finalTargetPos.z },
            orientation = new QuaternionMsg { x = 0.0, y = 0.0, z = 0.0, w = 1.0 } // Zamrożona rotacja (łatwiejsze dla IK)
        };

        ros.Publish(topic, msg);
    }
}