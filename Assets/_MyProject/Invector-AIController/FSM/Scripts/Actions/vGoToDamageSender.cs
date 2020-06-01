namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Go to the last damage sender position", UnityEditor.MessageType.Info)]
#endif
    public class vGoToDamageSender : vStateAction
    {
        public override string categoryName
        {
            get { return "Movement/"; }
        }
        public override string defaultName
        {
            get { return "Go To Damage Sender"; }
        }

        public bool goInStrafe = false;
        public vAIMovementSpeed speed = vAIMovementSpeed.Walking;

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (fsmBehaviour.aiController == null || fsmBehaviour.aiController.receivedDamage.lastSender==null) return;
            if (executionType == vFSMComponentExecutionType.OnStateEnter) fsmBehaviour.aiController.ForceUpdatePath(2f);
            fsmBehaviour.aiController.SetSpeed(speed);
            if (goInStrafe)
                fsmBehaviour.aiController.StrafeMoveTo(fsmBehaviour.aiController.receivedDamage.lastSender.position, fsmBehaviour.aiController.receivedDamage.lastSender.position - fsmBehaviour.transform.position);
            else fsmBehaviour.aiController.MoveTo(fsmBehaviour.aiController.receivedDamage.lastSender.position);
        }
    }
}