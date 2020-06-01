using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Check what FSM State is running", UnityEditor.MessageType.Info)]
#endif
    public class AICheckState : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }        
        
        public override string defaultName
        {
            get { return "Check FSM State"; }
        }

        [SerializeField, HideInInspector] protected int stateIndex;

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            return fsmBehaviour.indexOffCurrentState == stateIndex + 2;
        }
    }
}