using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    /// <summary>
    /// Message Object to FSM Debug Window
    /// </summary>
    public class vFSMDebugObject
    {
        public string message = string.Empty;
        public UnityEngine.Object sender;
        public vFSMDebugObject(string message, UnityEngine.Object sender = null)
        {
            if(!string.IsNullOrEmpty(message))
            this.message = message;
            this.sender = sender;
        }
    }

    public partial interface vIFSMBehaviourController 
    {
        Transform transform { get; }

        GameObject gameObject { get; }
        
        /// <summary>
        /// Use this to stop FSM Update
        /// </summary>
        bool isStopped { get; set; }
       
        /// <summary>
        /// Debug mode
        /// </summary>
        bool debugMode { get; set; }

        /// <summary>
        /// Debug Message List <seealso cref="vIFSMBehaviourController.SendDebug(string, Object)"/>
        /// </summary>
        List<vFSMDebugObject> debugList { get; }

        /// <summary>
        /// AI Controller that FSM will to control
        /// </summary>
        vIControlAI aiController { get; set; }

        /// <summary>
        /// <seealso cref="vFSMBehaviour"/> of the FSMBehaviour
        /// </summary>
        vFSMBehaviour fsmBehaviour { get; set; }
        
        /// <summary>
        /// Any State of the FSM (state that makes updating independent of current)
        /// </summary>
        vFSMState anyState { get; }
        
        /// <summary>
        /// Last State of the FSM
        /// </summary>
        vFSMState lastState { get; }
       
        /// <summary>
        /// Current State of the FSM
        /// </summary>
        vFSMState currentState { get; }
        
        /// <summary>
        /// Retur index of current state  <seealso cref="vFSMBehaviour"/>
        /// </summary>
        int indexOffCurrentState { get; }
    
        /// <summary>
        /// Retur name of current state <seealso cref="vFSMBehaviour"/>
        /// </summary>
        string nameOffCurrentState { get; }

        bool HasTimer(string key);

        void RemoveTimer(string key);
        /// <summary>
        /// Check if timer is greater than FSM timer
        /// </summary>
        /// <param name="timer">timer</param>
        /// <returns></returns>
        float GetTimer(string key);

        /// <summary>
        /// Reset FSM timer to zero. Auto called for <seealso cref="vFSMBehaviour"/> when change state
        /// </summary>
        void SetTimer(string key,float timer);

        /// <summary>
        /// Change State
        /// </summary>
        /// <param name="state"> new state</param>
        void ChangeState(vFSMState state);

        ///// <summary>
        ///// Change State using index of state in <seealso cref="vFSMBehaviour"/>. If index dont exit,there will be no changes
        ///// </summary>
        ///// <param name="stateIndex"></param>
        //void ChangeState(int stateIndex);

        ///// <summary>
        ///// Change State using name of state in <seealso cref="vFSMBehaviour"/>. If name dont exit, ther will be no changes.
        ///// </summary>
        ///// <param name="stateName">name of the state in <seealso cref="vFSMBehaviour"/></param>
        //void ChangeState(string stateName);
        void ChangeBehaviour(vFSMBehaviour behaviour);
        /// <summary>
        /// Start FSM Update
        /// </summary>
        void StartFSM();
        
        /// <summary>
        /// Stop the FSM Update
        /// </summary>
        void StopFSM();
        /// <summary>
        /// Send debug Message to FSM Debug Window
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="sender">Object sending message</param>
        void SendDebug(string message, UnityEngine.Object sender = null);
    }
}