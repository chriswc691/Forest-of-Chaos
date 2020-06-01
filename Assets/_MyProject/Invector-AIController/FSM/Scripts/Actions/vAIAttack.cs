namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a vControlAIMelee", UnityEditor.MessageType.Info)]
#endif
    public class vAIAttack : vStateAction
    {
        public bool overrideAttackDistance;
        [vHideInInspector("overrideAttackDistance")]
        public float attackDistance;
        public bool overrideAttackID;
        [vHideInInspector("overrideAttackID")]
        public int attackID;
        public bool overrideStrongAttack;
        [vHideInInspector("overrideStrongAttack")]
        public bool strongAttack;
        [vHelpBox("Force attack when attack distance reached")]
        public bool forceFirstAttack;
        [vHelpBox("Speed Movement to Attack distance")]
        public vAIMovementSpeed attackSpeed = vAIMovementSpeed.Walking;

        public override string categoryName
        {
            get { return "Combat/"; }
        }
        public override string defaultName
        {
            get { return "Trigger MeleeAttack"; }
        }

        public vAIAttack()
        {
            executionType = vFSMComponentExecutionType.OnStateUpdate | vFSMComponentExecutionType.OnStateEnter | vFSMComponentExecutionType.OnStateExit;
        }

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {

            Attack(fsmBehaviour.aiController as vIControlAICombat, executionType);
        }

        public virtual void Attack(vIControlAICombat aICombat, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (executionType == vFSMComponentExecutionType.OnStateEnter)
            {
                aICombat.InitAttackTime();
                if (forceFirstAttack) aICombat.Attack(overrideStrongAttack ? strongAttack : false, overrideAttackID ? attackID : -1, true);
            }

            if (aICombat != null && aICombat.currentTarget.transform)
            {
                var distance = aICombat.targetDistance;
                if (distance <= (overrideAttackDistance ? attackDistance : aICombat.attackDistance))
                {
                    aICombat.RotateTo(aICombat.currentTarget.transform.position - aICombat.transform.position);
                    if (!aICombat.isAttacking && aICombat.canAttack)
                    {
                        aICombat.Attack(overrideStrongAttack ? strongAttack : false, overrideAttackID ? attackID : -1);
                    }
                }
                else
                {
                    aICombat.SetSpeed(attackSpeed);
                    aICombat.MoveTo(aICombat.currentTarget.transform.position);
                }
            }
            if (executionType == vFSMComponentExecutionType.OnStateExit)
            {
                aICombat.ResetAttackTime();
            }
        }
    }
}