using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vPlayAnimationAction Action", UnityEditor.MessageType.Info)]
#endif
    public class vPlayAnimationAction : vStateAction
    {       
       public override string categoryName
        {
            get { return "Controller/"; }
        }
        public override string defaultName
        {
            get { return "Play Animation"; }
        }

        public string _animationState;
        public int _layer;

        public vPlayAnimationAction()
        {
            executionType = vFSMComponentExecutionType.OnStateEnter;
        }

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            fsmBehaviour.aiController.animator.Play(_animationState, _layer);
        }
    }
}