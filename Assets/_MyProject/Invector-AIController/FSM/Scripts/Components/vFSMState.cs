using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    [SerializeField]
    public class vFSMState : ScriptableObject
    {
        #region Editor

#if UNITY_EDITOR

        #region Editor Variables  
        [Multiline]
        public string description = "FSM State";
        [SerializeField, HideInInspector]
        public int selectedDecisionIndex;

        [SerializeField, HideInInspector]
        public bool canRemove = true;
        [SerializeField, HideInInspector]
        public bool canTranstTo = true;
        [SerializeField, HideInInspector]
        public bool canSetAsDefault = true;
        [SerializeField, HideInInspector]
        public bool canEditName = true;
        [SerializeField, HideInInspector]
        public bool canEditColor = true;
        [SerializeField, HideInInspector]
        public bool isOpen;
        [SerializeField, HideInInspector]
        public bool isSelected;
        [SerializeField]
        public Rect nodeRect;
        [SerializeField, HideInInspector]
        public Vector2 positionRect;
        [SerializeField, HideInInspector]
        public float rectWidth;
        [SerializeField, HideInInspector]
        public bool editingName;
        [SerializeField, HideInInspector]
        public Color nodeColor = Color.green;
        [SerializeField, HideInInspector]
        public bool resizeLeft = false;
        [SerializeField, HideInInspector]
        public bool resizeRight = false;
        [SerializeField, HideInInspector]
        public bool inDrag;
        #endregion

#endif

        #endregion

        #region Public Variables       

        public bool resetCurrentDestination = false;
        public List<vStateTransition> transitions = new List<vStateTransition>();
        public List<vStateAction> actions = new List<vStateAction>();
        public FSMComponent components;
        [SerializeField, HideInInspector]
        public bool useActions = true;
        [SerializeField, HideInInspector]
        public bool useDecisions = true;
        public vFSMBehaviour parentGraph;
        public vFSMState defaultTransition;
       

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public virtual Type requiredType { get { return typeof(vIControlAI); } }
        #endregion

        #region Main Methods

        public virtual void OnStateEnter(vIFSMBehaviourController fsmBehaviour)
        {
            if (resetCurrentDestination)
                fsmBehaviour.aiController.Stop();                      
           
            if (components == null)
                components = new FSMComponent(actions);
            if (useActions && components != null)
                components.DoActions(fsmBehaviour, vFSMComponentExecutionType.OnStateEnter);           
        }

        public virtual void UpdateState(vIFSMBehaviourController fsmBehaviour)
        {
            if (components == null)
                components = new FSMComponent(actions);
            if (useActions && components != null)
                components.DoActions(fsmBehaviour, vFSMComponentExecutionType.OnStateUpdate);  

            fsmBehaviour.ChangeState(TransitTo(fsmBehaviour));          
        }

        public virtual void OnStateExit(vIFSMBehaviourController fsmBehaviour)
        {
            if (components == null)
                components = new FSMComponent(actions);
            if (useActions && components != null)
                components.DoActions(fsmBehaviour, vFSMComponentExecutionType.OnStateExit);           
        }

        public vFSMState TransitTo(vIFSMBehaviourController fsmBehaviour)
        {
            vFSMState node = defaultTransition;
            for (int i = 0; i < transitions.Count; i++)
            {
                node = transitions[i].TransitTo(fsmBehaviour);
                if (node) break;
            }
            return node;
        }
        #endregion

        #region Subclasse

        public class FSMComponent
        {
           
            public List<vStateAction> actionsEnter;
            public List<vStateAction> actionsExit;
            public List<vStateAction> actionsUpdate;

            public FSMComponent(List<vStateAction> actions)
            {
                actionsEnter = actions.FindAll(act => act && (act.executionType & vFSMComponentExecutionType.OnStateEnter) != 0);
                actionsExit = actions.FindAll(act => act && (act.executionType & vFSMComponentExecutionType.OnStateExit) != 0);
                actionsUpdate = actions.FindAll(act => act && (act.executionType & vFSMComponentExecutionType.OnStateUpdate) != 0);
            }

            public void DoActions(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType)
            {

                switch (executionType)
                {
                    case vFSMComponentExecutionType.OnStateEnter:
                        for (int i = 0; i < actionsEnter.Count; i++)
                        {
                            actionsEnter[i].DoAction(fsmBehaviour, vFSMComponentExecutionType.OnStateEnter);
                        }
                        break;
                    case vFSMComponentExecutionType.OnStateExit:
                        for (int i = 0; i < actionsExit.Count; i++)
                        {
                            actionsExit[i].DoAction(fsmBehaviour, vFSMComponentExecutionType.OnStateExit);
                        }
                        break;
                    case vFSMComponentExecutionType.OnStateUpdate:

                        for (int i = 0; i < actionsUpdate.Count; i++)
                        {
                            actionsUpdate[i].DoAction(fsmBehaviour, vFSMComponentExecutionType.OnStateUpdate);
                        }
                        break;
                }
            }

           
        }

        #endregion      
    }
}
