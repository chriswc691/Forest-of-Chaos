using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vHasWayPointArea decision", UnityEditor.MessageType.Info)]
#endif
    public class vHasWayPointArea : vStateDecision
    {
		public override string categoryName
        {
            get { return ""; }
        }

        public override string defaultName
        {
            get { return "Has WayPointArea?"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            //TO DO
            return fsmBehaviour.aiController.waypointArea!=null;
        }
    }
}
