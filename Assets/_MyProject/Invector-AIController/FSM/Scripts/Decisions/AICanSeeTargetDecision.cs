
namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Verify if you can see the target based on the Detection Settings of the AI Controller", UnityEditor.MessageType.Info)]
#endif
    public class AICanSeeTargetDecision : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Can See Target"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            var cansee = CanSeeTarget(fsmBehaviour);
            return cansee;
        }

        protected virtual bool CanSeeTarget(vIFSMBehaviourController fsmBehaviour)
        {
            return fsmBehaviour.aiController.targetInLineOfSight;
        }
    }
}