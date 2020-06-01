namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a AICompanion attached to your AI Controller to make him follow you", UnityEditor.MessageType.Info)]
#endif
    public class vCompanionIsForceToFollow : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Force Companion To Follow"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController.HasComponent<vAICompanion>())
            {
                return fsmBehaviour.aiController.GetAIComponent<vAICompanion>().forceFollow;
            }
            return true;
        }
    }
}
