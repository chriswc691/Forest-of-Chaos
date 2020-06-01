using System;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public abstract class vStateAction : ScriptableObject
    {
        public abstract string categoryName { get; }
        public abstract string defaultName { get; }
        public virtual Type requiredType { get { return typeof(vIControlAI); } }
        public vFSMBehaviour parentFSM;
        [vEnumFlag]
        public vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate;
        public abstract void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate);

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

        protected virtual bool InRandomTimer(vIFSMBehaviourController fsmBehaviour,float minTimer,float maxTimer,string timerTag = "")
        {
            var tag = string.IsNullOrEmpty(timerTag) ? name : timerTag;
            if (!fsmBehaviour.HasTimer(tag))
            {
                fsmBehaviour.SetTimer(tag,UnityEngine. Random.Range(minTimer,maxTimer) + Time.time);               
            }
            float timer = fsmBehaviour.GetTimer(tag);
            if(timer <Time.time)
            {
                fsmBehaviour.SetTimer(tag, UnityEngine.Random.Range(minTimer, maxTimer) + Time.time);
                return true;
            }
            return false;
        }

#if UNITY_EDITOR
        public event UnityEngine.Events.UnityAction<vStateAction> onDestroy;

        public bool editingName;

        public void DestroyImmediate()
        {
            DestroyImmediate(this, true);
            if (onDestroy != null)
                onDestroy.Invoke(this);
        }
#endif
    }
}