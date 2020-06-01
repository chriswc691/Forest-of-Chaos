namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Return true or false if the AI Controller has a CurrentTarget or not", UnityEditor.MessageType.Info)]
#endif
    public class vHasTarget : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Has a CurrentTarget?"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController == null)
                return false;
            return fsmBehaviour.aiController.currentTarget.transform != null;
        }
    }
}