using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector.vCharacterController.AI
{
    [RequireComponent(typeof(BoxCollider))]
    [vClassHeader("AI Cover Point")]
    public class vAICoverPoint : vMonoBehaviour
    {
        private void Awake()
        {
            if (boxCollider) boxCollider.isTrigger = true;
        }
        private void Start()
        {
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(posePosition,out hit,0.2f, UnityEngine.AI.NavMesh.AllAreas))
            {
               
            }
            else gameObject.SetActive(false);
        }
        public float posePositionZ = 0.5f;

        public BoxCollider boxCollider;

     
        public Vector3 posePosition
        {
            get
            {
                return transform.position + transform.forward * posePositionZ;
            }
        }
   
        public bool isOccuped;

      
    }
}