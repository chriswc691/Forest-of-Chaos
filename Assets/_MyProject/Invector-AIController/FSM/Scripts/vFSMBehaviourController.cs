using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    [vClassHeader(" FSM BEHAVIOUR CONTROLLER", helpBoxText = "Required a AI Controller Component", useHelpBox = true, iconName = "Textures/Editor/FSMIcon2")]
    public partial class vFSMBehaviourController : vMonoBehaviour, vIFSMBehaviourController
    {
        [vEditorToolbar("FSM")]
        [SerializeField] protected vFSMBehaviour _fsmBehaviour;
    
        [SerializeField] protected bool _stop;
        [SerializeField] protected bool _debugMode;

        Dictionary<string, float> _timers = new Dictionary<string, float>();
        vFSMState _currentState;
        vFSMState _lastState;
        bool inChangeState;
        protected virtual void Start()
        {           
            aiController = GetComponent<vIControlAI>();         
        }
        
        protected virtual void Update()
        {
            if (aiController != null && !aiController.isDead && !isStopped) UpdateStates();
        }

        protected virtual void UpdateStates()
        {
            if (currentState)
            {
                if(!inChangeState)
                {
                    currentState.UpdateState(this);                   
                    UpdateAnyState();
                }                     
            }
            else
            {
                Entry();
            }
        }

        public virtual void ResetFSM()
        {
            if (currentState)                
                currentState.OnStateExit(this);

            currentState = null;
        }

        protected virtual void Entry()
        {
            if (!_fsmBehaviour) return;
            if (_fsmBehaviour.states.Count > 1)
            {
                currentState = _fsmBehaviour.states[0];
                currentState.OnStateEnter(this);              

            }
            else if (currentState != null) currentState = null;
        }

        protected virtual void UpdateAnyState()
        {
            // AnyState
            if (currentState && _fsmBehaviour && _fsmBehaviour.states.Count > 1)
            {
                _fsmBehaviour.states[1].UpdateState(this);
            }
        }

        #region FSM Interface
        public virtual vFSMBehaviour fsmBehaviour { get { return _fsmBehaviour; } set { _fsmBehaviour = value; } }      

        public virtual bool debugMode { get { return _debugMode; } set { _debugMode = value; } }

        public virtual bool isStopped { get { return _stop; } set { _stop = value; } }

        public virtual vIControlAI aiController { get; set; }

        public virtual int indexOffCurrentState
        {
            get { return currentState && _fsmBehaviour ? _fsmBehaviour.states.IndexOf(currentState) : -1; }
        }

        public virtual string nameOffCurrentState
        {
            get { return currentState ? currentState.Name : string.Empty; }
        }

        public virtual void SendDebug(string message, UnityEngine.Object sender = null)
        {
            if (debugList == null) debugList = new List<vFSMDebugObject>();
            if (debugList.Exists(d => d.sender == sender))
            {
                var debug = debugList.Find(d => d.sender == sender);
                debug.message = message;
            }
            else
            {
                debugList.Add(new vFSMDebugObject(message, sender));
            }
        }

        public virtual List<vFSMDebugObject> debugList
        {
            get;protected set;
        }

        public virtual vFSMState anyState
        {
            get { return _fsmBehaviour.states.Count > 1? _fsmBehaviour.states[1]:null; }
        }

        public virtual vFSMState currentState
        {
            get { return _currentState; }
            protected set { _currentState = value; }
        }

        public virtual vFSMState lastState
        {
            get { return _lastState; }
            protected set { _lastState = value; }
        }
        
        public virtual bool HasTimer(string key)
        {
            return _timers.ContainsKey(key);
        }

        public virtual void RemoveTimer(string key)
        {
            if (_timers.ContainsKey(key)) _timers.Remove(key);
        }

        public virtual float GetTimer(string key)
        {
            if (!_timers.ContainsKey(key))
            {
                _timers.Add(key, 0f);
            }
            if (_timers.ContainsKey(key))
            {
                if (debugMode) SendDebug("<color=yellow>Get Timer " + key + " = " + _timers[key].ToString("0.0") + " </color> ", gameObject);
                return _timers[key];
            }
            return 0;
        }

        public virtual void SetTimer(string key,float value)
        {
            if (!_timers.ContainsKey(key))
            {
                _timers.Add(key, value);
            }
            else if (_timers.ContainsKey(key))
            {
                _timers[key] = value;                
            }
            if (debugMode) SendDebug("<color=yellow>Set " + key+ " Timer to " +value.ToString("0.0") +" </color> ", gameObject);
        }

        public virtual void ChangeState(vFSMState state)
        {
            if (state && state != currentState && !inChangeState)
            {
                inChangeState = true;
                _lastState = currentState;
                currentState = null;
                if (_lastState)
                {
                    if (debugMode) SendDebug("<color=red>EXIT:" + _lastState.name + "</color>" + "  "+"<color=yellow> ENTER :" + state.Name + " </color> ", gameObject);
                    _lastState.OnStateExit(this);
                }
                currentState = state;
                state.OnStateEnter(this);                     
                inChangeState = false;
            }
        }

        public virtual void ChangeBehaviour(vFSMBehaviour behaviour)
        {
            if(_fsmBehaviour !=behaviour)
            {
                inChangeState = true;           
                _fsmBehaviour = behaviour;
                currentState = null;
                if(!isStopped) Entry();

                if (debugMode)
                SendDebug("CHANGE BEHAVIOUR TO " + behaviour.name);
                inChangeState = false;
            }
        }

        public virtual void StartFSM()
        {
            isStopped = false;
        }

        public virtual void StopFSM()
        {
            isStopped = true;
        }
        #endregion
    }
}