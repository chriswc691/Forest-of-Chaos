using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a ControlAIShooter", UnityEditor.MessageType.Info)]
#endif
    public class vAIShooterAttack : vStateAction
    {
        public override string categoryName
        {
            get { return "Combat/"; }
        }
        public override string defaultName
        {
            get { return "Trigger ShooterAttack"; }
        }

        public vAIShooterAttack()
        {
            executionType = vFSMComponentExecutionType.OnStateEnter | vFSMComponentExecutionType.OnStateExit | vFSMComponentExecutionType.OnStateUpdate;
        }
        public bool useRandomAttackType;
        [vHideInInspector("useRandomAttackType")]
        [Tooltip("Use values between 0 and 100")]
        public int chanceToStrongAttack;
        [vHideInInspector("useRandomAttackType")]
        public Vector2 minMaxTimeToTryStrongAttack = new Vector2(5, 10);
        [vHideInInspector("useRandomAttackType", invertValue = true)]
        public bool useStrongAttack = false;
        public bool overrideAttackID = false;
        [vHideInInspector("overrideAttackID")]
        public int attackID = 0;
        [vHelpBox("Use this to ignore attack time")]
        public bool forceCanAttack;

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (fsmBehaviour.aiController is vIControlAIShooter)
            {
                ControlAttack(fsmBehaviour, fsmBehaviour.aiController as vIControlAIShooter);
            }
        }

        protected virtual void ControlAttack(vIFSMBehaviourController fsmBehaviour, vIControlAIShooter combat, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            switch (executionType)
            {
                case vFSMComponentExecutionType.OnStateEnter:
                    InitAttack(combat);
                    break;
                case vFSMComponentExecutionType.OnStateUpdate:
                    HandleAttack(fsmBehaviour, combat);
                    break;
                case vFSMComponentExecutionType.OnStateExit:
                    FinishAttack(combat);
                    break;
            }
        }

        protected virtual void InitAttack(vIControlAIShooter combat)
        {
            combat.isInCombat = true;
            combat.InitAttackTime();
        }

        protected virtual void HandleAttack(vIFSMBehaviourController fsmBehaviour, vIControlAIShooter combat)
        {
            combat.AimToTarget();
            if (!combat.isAiming) return;
            if (useRandomAttackType)
            {
                if (InRandomTimer(fsmBehaviour, minMaxTimeToTryStrongAttack.x, minMaxTimeToTryStrongAttack.y))
                {
                    DoAttack(combat, Random.Range(0f, 100f) <= chanceToStrongAttack, overrideAttackID ? attackID : -1, forceCanAttack);
                }
                else DoAttack(combat, false, overrideAttackID ? attackID : -1, forceCanAttack);
            }
            else
            {
                DoAttack(combat, useStrongAttack, overrideAttackID ? attackID : -1, forceCanAttack);
            }
        }

        protected virtual void DoAttack(vIControlAIShooter combat, bool isStrongAttack = false, int attackId = -1, bool forceCanAttack = false)
        {
            combat.Attack(isStrongAttack, attackId, forceCanAttack);
        }

        protected virtual void FinishAttack(vIControlAIShooter combat)
        {
            combat.isInCombat = false;
            combat.ResetAttackTime();
        }
    }
}