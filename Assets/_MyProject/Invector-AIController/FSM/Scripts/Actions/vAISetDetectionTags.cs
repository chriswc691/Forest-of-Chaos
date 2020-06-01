namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Overwrite the Detection Tags of your AI Controller", UnityEditor.MessageType.Info)]
#endif
    public class vAISetDetectionTags : vStateAction
    {
        public override string categoryName
        {
            get { return "Detection/"; }
        }
        public override string defaultName
        {
            get { return "Set Detections Tags"; }
        }

        public vAISetDetectionTags()
        {
            executionType = vFSMComponentExecutionType.OnStateEnter;
        }

        public vTagMask tags;

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (executionType == vFSMComponentExecutionType.OnStateEnter) fsmBehaviour.aiController.SetDetectionTags(tags);
        }
    }
}