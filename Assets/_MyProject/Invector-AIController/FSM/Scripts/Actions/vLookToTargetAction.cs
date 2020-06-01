namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a HeadTrack attached to your AI Controller", UnityEditor.MessageType.Info)]
#endif
    public class vLookToTargetAction : vStateAction
    {
        public override string categoryName
        {
            get { return "Controller/"; }
        }
        public override string defaultName
        {
            get { return "Look To Target (Headtrack)"; }
        }

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (fsmBehaviour != null && fsmBehaviour.aiController.currentTarget.transform && fsmBehaviour.aiController.targetInLineOfSight)
            {
                fsmBehaviour.aiController.LookTo(fsmBehaviour.aiController.lastTargetPosition, 3f);
            }
        }
    }

}
