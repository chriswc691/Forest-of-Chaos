using System.Collections.Generic;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a AINoiseListener to detect Noises", UnityEditor.MessageType.Info)]
#endif
    public class vAIHearNoise : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Check for Noise"; }
        }

        [vToggleOption("Noise Type", "Any Noise", "Specific Noise")]
        public bool specific;
        [vHideInInspector("specific")]
        public List<string> noiseTypes;
        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController != null)
            {
                if (fsmBehaviour.aiController.HasComponent<vAINoiseListener>())
                {
                    var noiseListener = fsmBehaviour.aiController.GetAIComponent<vAINoiseListener>();
                    if (specific) return noiseListener.IsListeningSpecificNoises(noiseTypes);
                    else return noiseListener.IsListeningNoise();
                }
            }
            return false;
        }
    }
}
