using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.McTask; 

public class RobotController : MonoBehaviour
{
    private ROSConnection ros;
    public string topicName = "/walker/cmd/joint"; 
    public GameObject targetCube;

    // Zmienna do zapamiętania ostatniej pozycji sześcianu
    private float lastPositionX = -999f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<JointCmdMsg>(topicName);
    }

    void Update()
    {
        if (targetCube != null)
        {
            // Jeśli sześcian zmienił pozycję na osi X, wysyłamy komendę
            if (targetCube.transform.position.x != lastPositionX)
            {
                SendRobotCommand();
                
                // Zapisujemy nową pozycję, żeby nie wysyłać w kółko tego samego
                lastPositionX = targetCube.transform.position.x;
            }
        }
    }

    void SendRobotCommand()
    {
        JointCmdMsg msg = new JointCmdMsg();
        
        msg.name = "L_shoulder_pitch_joint"; 
        msg.control_mode = 2; 
        msg.position = targetCube.transform.position.x; 
        msg.velocity = 0.5f;
        msg.effort = 20.0f;

        ros.Publish(topicName, msg);
    }
}