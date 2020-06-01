using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    [System.Serializable]
    public class vStateDecisionObject
    {
        public bool trueValue = true;
        public vStateDecision decision;
        [SerializeField]
        public bool isValid;
        public bool validated;

        public vStateDecisionObject(vStateDecision decision)
        {
            this.decision = decision;
        }

        public vStateDecisionObject Copy()
        {
            var obj = new vStateDecisionObject(this.decision);
            obj.trueValue = trueValue;
            return obj;
        }

        public bool Validate(vIFSMBehaviourController fsmBehaviour)
        {           
            if (trueValue)
            {
                isValid =  /*if a*/decision ?
                       /*if b*/decision.Decide(fsmBehaviour) :
                       /*else b*/ true;
            }
            else
            {
                isValid = !(/*if a*/decision ?
                      /*if b*/decision.Decide(fsmBehaviour) :
                      /*else b*/ false);
            }
#if UNITY_EDITOR
            if (validationByController == null) validationByController = new Dictionary<vIFSMBehaviourController, bool>();
            if (validationByController.ContainsKey(fsmBehaviour)) validationByController[fsmBehaviour] = isValid;
            else validationByController.Add(fsmBehaviour, isValid);
#endif
            return isValid;
        }

#if UNITY_EDITOR

        private Editor decisionEditor;
        public Dictionary<vIFSMBehaviourController, bool> validationByController;
        public void DrawDecisionEditor()
        {
            if (!decision) return;

            if (!decisionEditor)
                decisionEditor = Editor.CreateEditor(decision);

            if (decisionEditor)            
                decisionEditor.OnInspectorGUI();            
        }
#endif       

    }
}
