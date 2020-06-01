namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Clear the current target of your controller", UnityEditor.MessageType.Info)]
#endif
    public class vAIClearTarget : vStateAction
    {
        public override string categoryName
        {
            get { return "Detection/"; }
        }
        public override string defaultName
        {
            get { return "Clear Target"; }
        }

        public vAIClearTarget()
        {
            executionType = vFSMComponentExecutionType.OnStateEnter;
        }

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (executionType == vFSMComponentExecutionType.OnStateEnter) fsmBehaviour.aiController.RemoveCurrentTarget();
        }
    }
}