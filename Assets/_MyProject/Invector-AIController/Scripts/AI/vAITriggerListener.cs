using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Invector.vCharacterController.AI
{
    /// <summary>
    /// Is a <seealso cref="vIAIComponent"/>,used to store collider that Controller is triggering
    /// </summary>
    [DisallowMultipleComponent]
    public class vAITriggerListener :MonoBehaviour, vIAIComponent
    {
        public bool test;
        [System.Serializable]
        public class AITriggerEvent : UnityEngine.Events.UnityEvent<Collider> { }
        public virtual Type ComponentType
        {
            get
            {
                return typeof(vAITriggerListener);
            }
        }

        public vTagMask tagsToDetect;

        public LayerMask layersToDetect;
        public AITriggerEvent onTriggerEnter, onTriggerExit;
        /// <summary>
        /// List of colliders that triggered
        /// </summary>
        public virtual List<Collider> colliders { get; protected set; }

        void Start()
        {
            colliders = new List<Collider>();
        }
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (tagsToDetect.Contains(other.gameObject.tag) && layersToDetect.ContainsLayer(other.gameObject.layer) && !colliders.Contains(other))
            {
                onTriggerEnter.Invoke(other);
                colliders.Add(other);
            }
        }
        protected virtual void OnTriggerExit(Collider other)
        {
            if (tagsToDetect.Contains(other.gameObject.tag) && layersToDetect.ContainsLayer(other.gameObject.layer) && colliders.Contains(other))
            {
                onTriggerExit.Invoke(other);
                colliders.Remove(other);
            }
        }
    }
}