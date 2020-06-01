using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Overwrite the Detection Obstacle Layer of your AI Controller", UnityEditor.MessageType.Info)]
#endif
    public class vAISetObstaclesLayer : vStateAction
    {
        public override string categoryName
        {
            get { return "Detection/"; }
        }
        public override string defaultName
        {
            get { return "Set Obstacles Layer"; }
        }

        public vAISetObstaclesLayer()
        {
            executionType = vFSMComponentExecutionType.OnStateEnter;
        }

        public LayerMask newLayer;

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (executionType == vFSMComponentExecutionType.OnStateEnter) fsmBehaviour.aiController.SetObstaclesLayer(newLayer);
        }
    }
}