using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Check if the current target with a HealthController is Dead", UnityEditor.MessageType.Info)]
#endif
    public class AICheckTargetDead : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Check if Target is Dead"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            return TargetIsDead(fsmBehaviour);
        }

        protected virtual bool TargetIsDead(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour == null) return true;
            Transform target = fsmBehaviour.aiController.currentTarget;
            if (target == null) return true;            
            return fsmBehaviour.aiController.currentTarget.isDead;
        }
    }
}