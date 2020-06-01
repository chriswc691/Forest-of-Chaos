namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Verify if your CurrentTarget is Crouching", UnityEditor.MessageType.Info)]
#endif
    public class vTargetIsCrouching : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Target Is Crouching?"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController == null || !fsmBehaviour.aiController.currentTarget.isCharacter) return false;
            return fsmBehaviour.aiController.currentTarget.character.isCrouching;
        }
    }
}