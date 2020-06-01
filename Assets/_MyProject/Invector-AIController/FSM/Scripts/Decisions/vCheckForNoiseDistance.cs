using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This is a vCheckForNoiseDistance decision", UnityEditor.MessageType.Info)]
#endif
    public class vCheckForNoiseDistance : vStateDecision
    {
		public override string categoryName
        {
            get { return ""; }
        }

        public override string defaultName
        {
            get { return "Check For Noise Distance"; }
        }

        public bool findNewNoise = false;
        public bool specificType;
        [vHideInInspector("findNewNoise;specificType")]
        public List<string> noiseTypes;

        protected enum CompareValueMethod
        {
            Greater, Less, Equal
        }
        [SerializeField]
        protected CompareValueMethod compareMethod;
        public float distance;

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController != null)
            {
                if (fsmBehaviour.aiController.HasComponent<vAINoiseListener>())
                {
                    var noiseListener = fsmBehaviour.aiController.GetAIComponent<vAINoiseListener>();
                    vNoise noise = null;
                    if (findNewNoise)
                    {
                        if (specificType) noise = noiseListener.GetNearNoiseByTypes(noiseTypes);
                        else noise = noiseListener.GetNearNoise();
                    }
                    else noise = noiseListener.lastListenedNoise;
                    if (noise != null)
                    {
                        return CompareDistance(Vector3.Distance(fsmBehaviour.aiController.transform.position, noise.position), distance);
                    }
                }               
            }
            return true;
        }

        private bool CompareDistance(float distA, float distB)
        {
            switch (compareMethod)
            {
                case CompareValueMethod.Equal:
                    return distA.Equals(distB);
                case CompareValueMethod.Greater:
                    return distA > distB;
                case CompareValueMethod.Less:
                    return distA < distB;
            }
            return false;
        }
    }
}
