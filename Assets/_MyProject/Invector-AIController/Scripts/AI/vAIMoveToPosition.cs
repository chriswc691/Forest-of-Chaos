using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController.AI
{
    [vClassHeader("AI Move To Position", "It's recommended to call the StopFSM function in the OnStartMove Event, to avoid conflicts with the FSM behaviour.")]
    public class vAIMoveToPosition : vMonoBehaviour
    {
        [System.Serializable]
        public class vAIPosition
        {
            public string Name;
            public Transform target;
            public vAIMovementSpeed speed = vAIMovementSpeed.Walking;
            public bool rotateToTargetForward;
            public UnityEngine.Events.UnityEvent onStartMove;
            public UnityEngine.Events.UnityEvent onFinishMove;
            public UnityEngine.Events.UnityEvent onCancelMove;
            internal bool canMove;
            Vector3 lastTargetDirection;

            public IEnumerator MoveToPosition(vIControlAI controlAI)
            {
                lastTargetDirection = controlAI.transform.forward;
                onStartMove.Invoke();
                controlAI.SetSpeed(speed);
                controlAI.MoveTo(target.position);
                controlAI.ForceUpdatePath(2f);
                yield return new WaitForSeconds(1);

                while (!controlAI.isInDestination && canMove)
                {
                    if (controlAI.remainingDistance > 2)
                    {
                        controlAI.MoveTo(target.position);
                        lastTargetDirection = target.position - controlAI.transform.position;
                        lastTargetDirection.y = 0;
                    }
                    else controlAI.StrafeMoveTo(target.position, rotateToTargetForward ? target.forward : lastTargetDirection);
                    yield return null;
                }
                if (canMove)
                {
                    onFinishMove.Invoke();
                    
                }
                    
                else
                    onCancelMove.Invoke();
                canMove = false;
            }
        }

        protected vIControlAI controlAI;
        public List<vAIPosition> positions;
        private vAIPosition currentPosition;

        public bool moveToOnStart;
        [vHideInInspector("moveToOnStart")]
        public string positionTarget;

        private IEnumerator Start()
        {
            controlAI = GetComponent<vIControlAI>();
            yield return new WaitForEndOfFrame();
            if (moveToOnStart) MoveTo(positionTarget);
        }

        public void MoveTo(string positionName)
        {
            if (controlAI == null || controlAI.isDead) return;
            if (currentPosition == null || currentPosition.Name != positionName)
            {
                var newPosition = positions.Find(p => p.Name.Equals(positionName));
                if (newPosition != null)
                {
                    if (currentPosition != null && currentPosition.canMove) currentPosition.canMove = false;
                    currentPosition = newPosition;
                    currentPosition.canMove = true;
                    StartCoroutine(currentPosition.MoveToPosition(controlAI));                    
                }
            }
        }
    }
}