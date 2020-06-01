using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This will overwrite the current Tag, Layer and MaxDistanceToDetect of your controller", UnityEditor.MessageType.Info)]
#endif
    public class vFindSpecificTarget : vStateAction
    {
        public override string categoryName
        {
            get { return "Detection/"; }
        }
        public override string defaultName
        {
            get { return "Find Specific Target"; }
        }

        public LayerMask _detectLayer;
        public vTagMask _detectTags;
        public bool checkForObstacles = true;

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            FindTarget(fsmBehaviour.aiController);
        }

        public virtual void FindTarget(vIControlAI vIControl)
        {
            vIControl.FindSpecificTarget(_detectTags, _detectLayer, checkForObstacles);
        }
    }
}