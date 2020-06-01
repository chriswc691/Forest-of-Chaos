using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Makes the AI Wander around randomly", UnityEditor.MessageType.Info)]
#endif
    public class vWanderAction : vStateAction
    {
        public bool wanderInStrafe = false;

        public override string categoryName
        {
            get { return "Movement/"; }
        }
        public override string defaultName
        {
            get { return "Wander"; }
        }

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            DoWander(fsmBehaviour);
        }

        public vAIMovementSpeed speed = vAIMovementSpeed.Walking;

        protected virtual void DoWander(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour == null) return;
            if (fsmBehaviour.aiController.isDead) return;

            if (fsmBehaviour.aiController.isInDestination || Vector3.Distance(fsmBehaviour.aiController.targetDestination, fsmBehaviour.aiController.transform.position) <= 0.5f + fsmBehaviour.aiController.stopingDistance)
            {
                fsmBehaviour.aiController.SetSpeed(speed);
                var angle = Random.Range(-90f, 90f);
                var dir = Quaternion.AngleAxis(angle, Vector3.up) * fsmBehaviour.aiController.transform.forward;
                var movePoint = fsmBehaviour.aiController.transform.position + dir.normalized * (Random.Range(1f, 4f) + fsmBehaviour.aiController.stopingDistance);
                if (wanderInStrafe)
                    fsmBehaviour.aiController.StrafeMoveTo(movePoint, dir.normalized);
                else
                    fsmBehaviour.aiController.MoveTo(movePoint);
            }
        }
    }
}
