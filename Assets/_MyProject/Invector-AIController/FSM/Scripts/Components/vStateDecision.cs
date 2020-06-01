using System;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public abstract class vStateDecision : ScriptableObject
    {
        public abstract string categoryName { get; }
        public abstract string defaultName { get; }
        public vFSMBehaviour parentFSM;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public virtual Type requiredType { get { return typeof(vIControlAI); } }

        public abstract bool Decide(vIFSMBehaviourController fsmBehaviour);//{ return true; }

        protected virtual bool InTimer(vIFSMBehaviourController fsmBehaviour, float compareTimer = 1f, string timerTag = "")
        {
            var tag = string.IsNullOrEmpty(timerTag) ? name : timerTag;
            float timer = fsmBehaviour.GetTimer(tag);
            fsmBehaviour.SetTimer(tag, timer + Time.deltaTime);
            if (timer > compareTimer)
            {
                fsmBehaviour.SetTimer(tag, 0);
                return true;
            }
            return false;
        }

        protected virtual bool InRandomTimer(vIFSMBehaviourController fsmBehaviour, float minTimer, float maxTimer, string timerTag = "")
        {
            var tag = string.IsNullOrEmpty(timerTag) ? name : timerTag;
            if (!fsmBehaviour.HasTimer(tag))
            {
                fsmBehaviour.SetTimer(tag, UnityEngine.Random.Range(minTimer, maxTimer) + Time.time);
            }
            float timer = fsmBehaviour.GetTimer(tag);
            if (timer < Time.time)
            {
                fsmBehaviour.SetTimer(tag, UnityEngine.Random.Range(minTimer, maxTimer) + Time.time);
                return true;
            }
            return false;
        }
        #region Editor
#if UNITY_EDITOR       
        public bool editingName;
        public Rect trueRect = new Rect(0, 0, 10, 10);
        public Rect falseRect = new Rect(0, 0, 10, 10);
        public bool selectedTrue, selectedFalse;


#endif
        #endregion
    }
}
