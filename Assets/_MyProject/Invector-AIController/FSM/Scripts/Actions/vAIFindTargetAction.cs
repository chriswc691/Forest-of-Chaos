
namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Call this Action to Find a Target based on your AI Controller Detection Settings, make sure your target has a HealthController", UnityEditor.MessageType.Info)]
#endif
    public class vAIFindTargetAction : vStateAction
    {
        public override string categoryName
        {
            get { return "Detection/"; }
        }
        public override string defaultName
        {
            get { return "Find Target"; }
        }

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            FindTarget(fsmBehaviour);
        }

        public bool checkForObstacles = true;

        protected virtual void FindTarget(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour != null)
            {
                fsmBehaviour.aiController.FindTarget(checkForObstacles);
            }
        }
    }
}
