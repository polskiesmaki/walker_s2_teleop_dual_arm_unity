using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector.MessageGeneration;


namespace RosMessageTypes.McTask
{
    public class ArmTaskAction : Action<ArmTaskActionGoal, ArmTaskActionResult, ArmTaskActionFeedback, ArmTaskGoal, ArmTaskResult, ArmTaskFeedback>
    {
        public const string k_RosMessageName = "mc_task_msgs/ArmTaskAction";
        public override string RosMessageName => k_RosMessageName;


        public ArmTaskAction() : base()
        {
            this.action_goal = new ArmTaskActionGoal();
            this.action_result = new ArmTaskActionResult();
            this.action_feedback = new ArmTaskActionFeedback();
        }

        public static ArmTaskAction Deserialize(MessageDeserializer deserializer) => new ArmTaskAction(deserializer);

        ArmTaskAction(MessageDeserializer deserializer)
        {
            this.action_goal = ArmTaskActionGoal.Deserialize(deserializer);
            this.action_result = ArmTaskActionResult.Deserialize(deserializer);
            this.action_feedback = ArmTaskActionFeedback.Deserialize(deserializer);
        }

        public override void SerializeTo(MessageSerializer serializer)
        {
            serializer.Write(this.action_goal);
            serializer.Write(this.action_result);
            serializer.Write(this.action_feedback);
        }

    }
}
