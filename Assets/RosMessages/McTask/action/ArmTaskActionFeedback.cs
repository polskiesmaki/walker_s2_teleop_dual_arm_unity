using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;
using RosMessageTypes.Std;
using RosMessageTypes.Actionlib;

namespace RosMessageTypes.McTask
{
    public class ArmTaskActionFeedback : ActionFeedback<ArmTaskFeedback>
    {
        public const string k_RosMessageName = "mc_task_msgs/ArmTaskActionFeedback";
        public override string RosMessageName => k_RosMessageName;


        public ArmTaskActionFeedback() : base()
        {
            this.feedback = new ArmTaskFeedback();
        }

        public ArmTaskActionFeedback(HeaderMsg header, GoalStatusMsg status, ArmTaskFeedback feedback) : base(header, status)
        {
            this.feedback = feedback;
        }
        public static ArmTaskActionFeedback Deserialize(MessageDeserializer deserializer) => new ArmTaskActionFeedback(deserializer);

        ArmTaskActionFeedback(MessageDeserializer deserializer) : base(deserializer)
        {
            this.feedback = ArmTaskFeedback.Deserialize(deserializer);
        }
        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.header);
            serializer.Write(this.status);
            serializer.Write(this.feedback);
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
