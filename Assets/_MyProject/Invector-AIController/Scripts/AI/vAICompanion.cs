using Invector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector.vCharacterController.AI
{
    [vClassHeader("AI COMPANION")]
    public class vAICompanion :vMonoBehaviour, vIAIComponent
    {

        public vHealthController friend;
        public string friendTag = "Player";
        public float maxFriendDistance;
        public float minFriendDistance;
        public Type ComponentType
        {
            get
            {
                return this.GetType();
            }
        }

        public bool forceFollow;

        internal vControlAI controlAI;
        protected vAICompanionControl controller;

        protected void Start()
        {
            controlAI = GetComponent<vControlAI>();
            controlAI.onDead.AddListener(RemoveCompanion);
            if (!friend) FindFriend();
        }

        private void RemoveCompanion(GameObject arg0)
        {
            if(controller != null && controller.aICompanions.Contains(this))
            {
                controller.aICompanions.Remove(this);
            }
        }

        public bool friendIsFar
        {
           get
            {
                return friendDistance > maxFriendDistance;
            }
        }

        public bool friendIsDead
        {
            get
            {
                return friend && friend.isDead;
            }
        }

        public void FindFriend()
        {
            var fGO = FindObjectsOfType<vHealthController>().vToList().Find(p => p.gameObject.CompareTag(friendTag));
            if(fGO)
            {
                friend = fGO;
                controller = friend.GetComponent<vAICompanionControl>();
                if (controller && !controller.aICompanions.Contains(this)) controller.aICompanions.Add(this);
            }           
        }

        public float friendDistance
        {
            get
            {
                return friend ? (friend.transform.position - transform.position).magnitude : 0;
            }            
        }
     
        public void GoToFriend()
        {
            if (!friend||!controlAI) return;
            if (friendDistance > minFriendDistance)
            {
                controlAI.SetSpeed(friendDistance > minFriendDistance * 2 ? vAIMovementSpeed.Running : vAIMovementSpeed.Walking);
                controlAI.MoveTo(friend.transform.position);
            }
            else
            {
                controlAI.LookTo(friend.transform.position);
                controlAI.Stop();
            }
        }
    }
}