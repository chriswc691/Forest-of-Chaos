namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a vAICover attached to your AI Controller", UnityEditor.MessageType.Info)]
#endif
    public class vAIGetCoverAction : vStateAction
    {
        public override string categoryName
        {
            get { return "Movement/"; }
        }
        public override string defaultName
        {
            get { return "Get Cover"; }
        }

        public vAIGetCoverAction()
        {
            executionType = vFSMComponentExecutionType.OnStateUpdate | vFSMComponentExecutionType.OnStateExit;
        }
        public vAIMovementSpeed speed = vAIMovementSpeed.Running;

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            vIControlAICombat combatController = fsmBehaviour.aiController as vIControlAICombat;
            if (combatController == null) return;
            combatController.SetSpeed(speed);
            if (executionType == vFSMComponentExecutionType.OnStateUpdate && fsmBehaviour.aiController.HasComponent<vAICover>())
            {
                var cover = fsmBehaviour.aiController.GetAIComponent<vAICover>();
                if (fsmBehaviour.aiController.currentTarget.transform)
                    cover.GetCoverFromTargetThreat();
                else
                    cover.GetCoverFromRandomThreat();
            }
            if (executionType == vFSMComponentExecutionType.OnStateExit)
            {
                if (fsmBehaviour.aiController.HasComponent<vAICover>())
                {
                    var cover = fsmBehaviour.aiController.GetAIComponent<vAICover>();
                    cover.OnExitCover();
                }
                   (combatController).isInCombat = false;
                combatController.isCrouching = false;
            }
            if (executionType == vFSMComponentExecutionType.OnStateEnter)
            {
                (combatController).isInCombat = true;
            }
        }
    }
}