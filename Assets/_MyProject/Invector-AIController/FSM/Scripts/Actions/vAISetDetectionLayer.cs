using UnityEngine;
namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Overwrite the Detection Layer of your AI Controller", UnityEditor.MessageType.Info)]
#endif
    public class vAISetDetectionLayer : vStateAction
    {
        public override string categoryName
        {
            get { return "Detection/"; }
        }
        public override string defaultName
        {
            get { return "Set Detections Layer"; }
        }

        public vAISetDetectionLayer()
        {
            executionType = vFSMComponentExecutionType.OnStateEnter;
        }

        public LayerMask newLayer;

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (executionType == vFSMComponentExecutionType.OnStateEnter) fsmBehaviour.aiController.SetDetectionLayer(newLayer);
        }
    }
}