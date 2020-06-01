namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Verify if the AI Controller is currently moving towards a Destination", UnityEditor.MessageType.Info)]
#endif
    public class vAIIsInDestination : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Is In Destination"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            return fsmBehaviour.aiController.isInDestination;

        }
    }
}