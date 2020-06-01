using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Check the distance between the AI Controller and the Current Target", UnityEditor.MessageType.Info)]
#endif
    public class vCheckTargetDistance : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Check Target Distance"; }
        }

        protected enum CompareValueMethod
        {
            Greater, Less, Equal
        }
        [SerializeField]
        protected CompareValueMethod compareMethod;
        public float distance;
        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (!fsmBehaviour.aiController.currentTarget.transform) return false;
            var dist = fsmBehaviour.aiController.targetDistance;
            return CompareDistance(dist, distance);
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
