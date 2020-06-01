namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Check if the target is within the CombatRange of the AI Controller", UnityEditor.MessageType.Info)]
#endif
    public class AICheckTargetIsInCombatRange : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Check Target Combat Range"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            return TargetIsInCombatRange(fsmBehaviour.aiController as vIControlAICombat);
        }

        protected virtual bool TargetIsInCombatRange(vIControlAICombat ctrlCombat)
        {
            if (ctrlCombat == null) return false;
            if (ctrlCombat.currentTarget.transform == null) return false;
            if (!ctrlCombat.currentTarget.transform.gameObject.activeSelf) return false;
            return ctrlCombat.targetDistance <= ctrlCombat.combatRange;
        }
    }
}

