namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Check if the AI Controller received a specific amount of Damage or Hits", UnityEditor.MessageType.Info)]
#endif
    public class vAICheckMassiveDamage : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Check Damage Amount"; }
        }

        [vToggleOption("Compare Value", "Less", "Greater or Equals")]
        public bool greater = false;        
        [vToggleOption("Compare Type", "Total Hits", "Total Damage")]
        public bool massiveValue = true;
        public int valueToCompare;
       
        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            return (HasDamage(fsmBehaviour));
        }

        protected virtual bool HasDamage(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController.receivedDamage == null) return false;
            var value = massiveValue ? fsmBehaviour.aiController.receivedDamage.massiveValue : fsmBehaviour.aiController.receivedDamage.massiveCount;
            return (greater ? (value >= valueToCompare) : (value < valueToCompare));
        }
    }
}
