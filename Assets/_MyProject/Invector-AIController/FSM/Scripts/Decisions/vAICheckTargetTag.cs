namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Verify the CurrentTarget Tag", UnityEditor.MessageType.Info)]
#endif
    public class vAICheckTargetTag : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Check Target Tag"; }
        }

        public vTagMask targetTags;
        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController.currentTarget.transform != null)
                return targetTags.Contains(fsmBehaviour.aiController.currentTarget.transform.gameObject.tag);
            return true;
        }
    }
}
