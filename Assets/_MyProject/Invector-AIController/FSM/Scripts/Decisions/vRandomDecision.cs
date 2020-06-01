using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Creates a Random Chance to go for the next state", UnityEditor.MessageType.Info)]
#endif
    public class vRandomDecision : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Random Decision"; }
        }

        [Range(0, 100)]
        [Tooltip("Percentage Chance between true and false")]
        public float randomTrueFalse;
        public float frequency;
        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (frequency > 0)
            {
                if (InTimer(fsmBehaviour, frequency))
                    return Random.Range(0, 100) < randomTrueFalse;
                else return false;
            }

            return Random.Range(0, 100) < randomTrueFalse;
        }
    }
}
