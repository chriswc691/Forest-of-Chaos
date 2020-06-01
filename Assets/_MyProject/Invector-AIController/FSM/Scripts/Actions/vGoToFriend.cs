namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a AICompanion attached to your AI Controller - Use it to make the AI go to your Friend position", UnityEditor.MessageType.Info)]
#endif
    public class vGoToFriend : vStateAction
    {
        public override string categoryName
        {
            get { return "Movement/"; }
        }
        public override string defaultName
        {
            get { return "Go To Friend(Companion AI)"; }
        }
        public vAIMovementSpeed speed = vAIMovementSpeed.Running;
        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (fsmBehaviour.aiController.HasComponent<vAICompanion>())
            {
                fsmBehaviour.aiController.SetSpeed(speed);
                MoveToFriendPosition(fsmBehaviour.aiController.GetAIComponent<vAICompanion>());
            }
        }
        public virtual void MoveToFriendPosition(vAICompanion aICompanion)
        {
            if (aICompanion)
            {

                aICompanion.GoToFriend();
            }
        }
    }
}