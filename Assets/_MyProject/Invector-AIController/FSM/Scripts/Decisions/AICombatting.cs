
namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Check if the AI Controller is current in Combat mode", UnityEditor.MessageType.Info)]
#endif
    public class AICombatting : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Is in Combat"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (!(fsmBehaviour.aiController is vIControlAICombat)) return false;
            return (fsmBehaviour.aiController as vIControlAICombat).isInCombat;
        }
    }
}