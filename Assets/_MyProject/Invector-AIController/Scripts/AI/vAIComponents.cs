using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector.vCharacterController.AI
{   
    /// <summary>
    /// Is used to store in  <seealso cref="vIControlAI"/> All components that inherit this interface
    /// </summary>    
    public partial interface vIAIComponent
    {
        /// <summary>
        /// Type of Component. Used like a key to dicitionary        
        /// </summary>
        System.Type  ComponentType { get; }
    }

    /// <summary>
    /// Is a <seealso cref="vIAIComponent"/> that receive the Start,Update and Pause events of the  <seealso cref="vIControlAI"/>
    /// </summary>   
    public partial interface vIAIUpdateListener : vIAIComponent
    {
       
        /// <summary>
        /// Is called automatically by <seealso cref="vIControlAI"/> when is started
        /// </summary>
        /// <param name="controller"></param>
        void OnStart(vIControlAI controller);

        /// <summary>
        /// Is called automatically by <seealso cref="vIControlAI"/> when is updated;
        /// </summary>
        /// <param name="controller"></param>
        void OnUpdate(vIControlAI controller);

        /// <summary>
        /// Is called automatically by <seealso cref="vIControlAI"/> when is paused;
        /// </summary>
        /// <param name="controller"></param>
        void OnPause(vIControlAI controller);
    }  
}