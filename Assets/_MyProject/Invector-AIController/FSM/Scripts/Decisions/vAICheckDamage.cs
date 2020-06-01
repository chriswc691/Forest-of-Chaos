using System.Collections.Generic;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Check if the AI Controller has received any Damage or a specific DamageType", UnityEditor.MessageType.Info)]
#endif
    public class vAICheckDamage : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Check Damage Type"; }
        }

        public List<string> damageTypeToCheck;


        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            return (HasDamage(fsmBehaviour));
        }

        protected virtual bool HasDamage(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController == null) return false;
            var hasDamage = (fsmBehaviour.aiController.receivedDamage.isValid) && (damageTypeToCheck.Count == 0 || damageTypeToCheck.Contains(fsmBehaviour.aiController.receivedDamage.lasType));
           
            if (fsmBehaviour.debugMode)
            {
                fsmBehaviour.SendDebug(Name + " " + (fsmBehaviour.aiController.receivedDamage.isValid) + " " + fsmBehaviour.aiController.receivedDamage.lastSender, this);
            }

            return hasDamage;
        }
    }
}
