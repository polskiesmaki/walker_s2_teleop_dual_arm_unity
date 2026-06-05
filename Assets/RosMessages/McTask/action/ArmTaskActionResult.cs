using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;
using RosMessageTypes.Actionlib;

namespace RosMessageTypes.McTask
{
    public class ArmTaskActionResult : ActionResult<ArmTaskResult>
    {
        public const string k_RosMessageName = "mc_task_msgs/ArmTaskActionResult";
        public override string RosMessageName => k_RosMessageName;


        public ArmTaskActionResult() : base()
        {
            this.result = new ArmTaskResult();
        }

        public ArmTaskActionResult(HeaderMsg header, GoalStatusMsg status, ArmTaskResult result) : base(header, status)
        {
            this.result = result;
        }
        public static ArmTaskActionResult Deserialize(MessageDeserializer deserializer) => new ArmTaskActionResult(deserializer);

        ArmTaskActionResult(MessageDeserializer deserializer) : base(deserializer)
        {
            this.result = ArmTaskResult.Deserialize(deserializer);
        }
        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.status);
            serializer.Write(this.result);
        }


#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Register()
        {
            MessageRegistry.Register(k_RosMessageName, Deserialize);
        }
    }
}
