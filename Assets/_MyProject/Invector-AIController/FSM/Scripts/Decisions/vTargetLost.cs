
namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Verify if your AI Controller lost the current target", UnityEditor.MessageType.Info)]
#endif
    public class vTargetLost : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Lost the Target?"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour != null && fsmBehaviour.aiController != null && fsmBehaviour.aiController.currentTarget.transform)
            {
                return fsmBehaviour.aiController.currentTarget.isLost;
            }
            return true;
        }
    }
}