namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Find a Target and return true or false, can be used in the AnyState to find a target and make a transition to other state if a target was founded", UnityEditor.MessageType.Info)]
#endif
    public class vFindTargetDecision : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "FindTarget Decision"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController != null)
            {
                fsmBehaviour.aiController.FindTarget();
                return fsmBehaviour.aiController.currentTarget.transform != null;
            }
            return true;
        }
    }
}