
using System.Collections.Generic;
using UnityEngine;
namespace Invector.vCharacterController.AI.FSMBehaviour
{
    [System.Serializable]
    public class vStateTransition
    {       
        public List<vStateDecisionObject> decisions = new List<vStateDecisionObject>();
        public vFSMState trueState, falseState;
        public bool muteTrue, muteFalse;
        public vTransitionOutputType transitionType = vTransitionOutputType.Default;
        //[vEnumFlag]
        //public vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate;
      //  public bool validate;
        public float transitionDelay;
        
        public vStateTransition(vStateDecision decision)
        {
            if(decision)
                decisions.Add(new vStateDecisionObject(decision));
        }

        public vFSMState parentState;

        Dictionary<vIFSMBehaviourController, float> transitionTimers;
        public vFSMState TransitTo(vIFSMBehaviourController fsmBehaviour)
        { 
            var val = true;
            vFSMState returState = null;
            for(int i=0;i<decisions.Count;i++)
            {
                bool value = decisions[i].Validate(fsmBehaviour);
                
                if (!value)
                {                               
                    val = false;                  
                }              
            }
            if (val && trueState) returState= useTruState && !muteTrue ? trueState : null;
            else if (!val && falseState) returState=  useFalseState && !muteFalse ? falseState : null;

            if (transitionTimers == null) transitionTimers = new Dictionary<vIFSMBehaviourController, float>();
            if (!transitionTimers.ContainsKey(fsmBehaviour)) transitionTimers.Add(fsmBehaviour,0f);

            if (transitionTimers[fsmBehaviour] < transitionDelay && returState)
            {
                transitionTimers[fsmBehaviour] += Time.deltaTime;
                if (fsmBehaviour.debugMode) fsmBehaviour.SendDebug("<color=green>" + parentState.name + " Delay " + (transitionDelay - transitionTimers[fsmBehaviour]).ToString("00") + " To Enter in " + returState.Name + "</color>", parentState);
                return null;
            }
            else 
            {
                transitionTimers[fsmBehaviour] = 0;
                if (fsmBehaviour.debugMode && returState) fsmBehaviour.SendDebug("<color=yellow>" + parentState.name + " Transited to " + returState.name +"</color>", parentState);
            }
            return returState;
        }

        public bool useTruState
        {
           get { return (transitionType == vTransitionOutputType.TrueFalse || transitionType == vTransitionOutputType.Default); }
        }

        public bool useFalseState
        {
            get { return (transitionType == vTransitionOutputType.TrueFalse); }
        }


        #region Editor
#if UNITY_EDITOR
        public Rect trueRect, falseRect;
        public bool selectedTrue, selectedFalse;
        public bool trueSideRight;
        public bool falseSideRight;
        public UnityEditor.Editor decisionEditor;
        public bool isOpen;
        public Vector3 scroolView;
        public int sameTargetCount;
        public void SetTrueState(vFSMState node)
        {
            if(node.canTranstTo)
                trueState = node;
        }

        public void SetFalseState(vFSMState node)
        {
            if (node.canTranstTo)
                falseState = node;
        }

       

#endif
        #endregion
    }
}
