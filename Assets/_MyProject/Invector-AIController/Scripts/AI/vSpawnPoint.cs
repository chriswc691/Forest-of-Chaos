using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI
{
    [vClassHeader("SpawnPoint", helpBoxText = "This component needs a Collider set as IsTrigger to check if this SpawnPoint area is in use", useHelpBox = true, openClose = false)]
    public class vSpawnPoint : vMonoBehaviour
    {
        public bool isValid { get { return colliders.Count == 0; } }
        public List<Collider> colliders = new List<Collider>();
        private void OnTriggerStay(Collider other)
        {
            if (colliders.Contains(other))
            {
                var health = other.gameObject.GetComponentInParent<vIHealthController>();
                if (health != null && health.isDead)
                {
                    colliders.Remove(other);
                }
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Untagged") && !colliders.Contains(other)) colliders.Add(other);
        }
        void OnTriggerExit(Collider other)
        {
            if (colliders.Contains(other)) colliders.Remove(other);
        }
    }
}
