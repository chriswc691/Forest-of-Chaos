using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI
{
    [vClassHeader("AI Throw Object")]
    public class vAIThrowObject : vMonoBehaviour
    {
        [vEditorToolbar("Settings")]
        public vControlAI controlAI;
        public Transform defaultThrowStartPoint;
        [vEditorToolbar("Throwables")]
        public List<ThrowableObject> throwableObjects;
        [vEditorToolbar("Debug")]
        [vReadOnly(false)] public bool inThrow;
        [vReadOnly(false)] [SerializeField] protected float timeToThrow;
        [System.Serializable]

        public class ThrowableObject
        {
            public string name;
            public Rigidbody prefab;
            public bool customStartingPoint;
            [vHideInInspector("customStartingPoint")]
            public Transform startingPoint;
            public float throwObjectDelayTime = 0.5f;
            public float activeCollisionDelayTime = 0.1f;
            [Tooltip("Time to break Thrown Routine after dead\nIf Time To Throw(see Debug Toolbar) time is greater than this time and Character is dead, the Object will be instantiated but not Launched\nElse Instantiate will be canceled")]
            public float minTimeToThrowAfterDead = 0.25f;
            public float throwAngle = 20f;
            public UnityEngine.Events.UnityEvent onStartThrow, onFinishThrow;
            public ThrowableObject()
            {
                throwObjectDelayTime = 0.5f;
                activeCollisionDelayTime = 0.1f;
                throwAngle = 20f;
            }
        }


        private void Awake()
        {
            controlAI = GetComponent<vControlAI>();
        }
        public virtual Vector3 aimPoint
        {
            get
            {
                return controlAI.currentTarget.transform.position;
            }
        }

        Vector3 StartVelocity(Transform startPTransform, Vector3 targetP, float angle)
        {
            // distance between target and source
            float dist = Vector3.Distance(startPTransform.position, targetP);

            // rotate the object to face the target
            startPTransform.LookAt(targetP);

            // calculate initival velocity required to land the cube on target using the formula (9)
            float Vi = Mathf.Sqrt(dist * -Physics.gravity.y / (Mathf.Sin(Mathf.Deg2Rad * angle * 2)));
            float Vy, Vz;   // y,z components of the initial velocity

            Vy = Vi * Mathf.Sin(Mathf.Deg2Rad * angle);
            Vz = Vi * Mathf.Cos(Mathf.Deg2Rad * angle);

            // create the velocity vector in local space
            Vector3 localVelocity = new Vector3(0f, Vy, Vz);

            // transform it to global vector
            Vector3 globalVelocity = startPTransform.TransformVector(localVelocity);
            return globalVelocity;
        }

        public virtual Vector3 aimDirection
        {
            get
            {
                return aimPoint - defaultThrowStartPoint.position;
            }
        }

        void LaunchObject(Rigidbody projectily, Transform startPTransform, Vector3 targetP, float angle)
        {
            projectily.AddForce(StartVelocity(startPTransform, targetP, angle), ForceMode.VelocityChange);
        }

        public void Throw(string throwableObjectName)
        {
            if (controlAI.ragdolled || controlAI.isDead || controlAI.customAction) return;
            if (!inThrow)
            {
                ThrowableObject throwable = throwableObjects.Find(t => t.name.Equals(throwableObjectName));
                if (throwable != null)
                    StartCoroutine(Launch(throwable));
            }
        }

        Vector3 targetDirection
        {
            get
            {
                return controlAI.currentTarget.transform ? (controlAI.lastTargetPosition - transform.position) : transform.forward;
            }
        }

        Vector3 targetPosition
        {
            get
            {
                return controlAI.lastTargetPosition;
            }
        }

        IEnumerator Launch(ThrowableObject objectToThrow)
        {
            objectToThrow.onStartThrow.Invoke();
            inThrow = true;
            controlAI.RotateTo(targetDirection);
            controlAI.StrafeMoveTo(transform.position, targetDirection);
            timeToThrow = 0;
            Transform startingPoint = objectToThrow.customStartingPoint ? objectToThrow.startingPoint : defaultThrowStartPoint;
            bool canInstantiate = true;
            bool canThrow = true;
            while (timeToThrow < objectToThrow.throwObjectDelayTime)
            {
                timeToThrow += Time.deltaTime;
                if (controlAI == null || controlAI.isDead)
                {
                    canInstantiate = timeToThrow >= objectToThrow.minTimeToThrowAfterDead;
                    canThrow = false;
                    break;
                }
                yield return null;
            }
            timeToThrow = 0;
            if (canInstantiate)
            {
                var obj = Instantiate(objectToThrow.prefab, startingPoint.position, startingPoint.rotation) as Rigidbody;
                obj.isKinematic = false;
                if (canThrow) LaunchObject(obj, startingPoint, targetPosition, objectToThrow.throwAngle);
                objectToThrow.onFinishThrow.Invoke();
                if (canThrow) yield return new WaitForSeconds(2 * objectToThrow.activeCollisionDelayTime);
                else yield return new WaitForSeconds(1f);
                var coll = obj.GetComponent<Collider>();
                if (coll) coll.isTrigger = false;
            }
            else objectToThrow.onFinishThrow.Invoke();
            inThrow = false;
        }
    }
}