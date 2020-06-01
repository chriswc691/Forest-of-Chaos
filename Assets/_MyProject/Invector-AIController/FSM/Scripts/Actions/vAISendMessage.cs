namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a AIMessageReceiver attached to your AI Controller - This will send a message to your Controller, so you can triggers custom Events", UnityEditor.MessageType.Info)]
#endif
    public class vAISendMessage : vStateAction
    {
        public override string categoryName
        {
            get { return "Controller/"; }
        }
        public override string defaultName
        {
            get { return "SendMessage"; }
        }

        public vAISendMessage()
        {
            executionType = vFSMComponentExecutionType.OnStateEnter;
        }
        public string listenerName;
        public string message;
        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (fsmBehaviour.messageReceiver) fsmBehaviour.messageReceiver.Send(listenerName, message);
        }
    }
}