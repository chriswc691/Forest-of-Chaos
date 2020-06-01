namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Use it to change the current FSM Behavior of your Controller", UnityEditor.MessageType.Info)]
#endif
    public class vFSMChangeBehaviour : vStateAction
    {
        public override string categoryName
        {
            get { return "Controller/"; }
        }
        public override string defaultName
        {
            get { return "Change FSM Behaviour"; }
        }

        public vFSMBehaviour newBehaviour;
        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            fsmBehaviour.ChangeBehaviour(newBehaviour);
        }
    }
}