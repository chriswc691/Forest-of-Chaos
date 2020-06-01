namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Overwrite the Line of Sight of your AI Controller", UnityEditor.MessageType.Info)]
#endif
    public class vAISetLineOfSight : vStateAction
    {
        public override string categoryName
        {
            get { return "Detection/"; }
        }
        public override string defaultName
        {
            get { return "Set Line Of Sight"; }
        }

        public vAISetLineOfSight()
        {
            executionType = vFSMComponentExecutionType.OnStateEnter;
            fieldOfView = -1;
            minDistanceToDetect = -1;
            maxDistanceToDetect = -1f;
            lostTargetDistance = -1f;
        }

        [vHelpBox("If you don't want to overwrite a value leave it to -1")]
        public float fieldOfView;
        public float minDistanceToDetect;
        public float maxDistanceToDetect;
        public float lostTargetDistance;

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (executionType == vFSMComponentExecutionType.OnStateEnter) fsmBehaviour.aiController.SetLineOfSight(fieldOfView, minDistanceToDetect, maxDistanceToDetect, lostTargetDistance);
        }
    }
}