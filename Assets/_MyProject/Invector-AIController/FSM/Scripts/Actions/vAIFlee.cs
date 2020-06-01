using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Call this action to make the AI Flee from his current target, you can set the movement speed and the distance to flee", UnityEditor.MessageType.Info)]
#endif
    public class vAIFlee : vStateAction
    {
        public override string categoryName
        {
            get { return "Movement/"; }
        }
        public override string defaultName
        {
            get { return "Flee"; }
        }

        public vAIMovementSpeed fleeSpeed = vAIMovementSpeed.Running;
        public float fleeDistance = 10f;
        public bool debugMode;
        public bool debugFleeDirection;
        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (executionType == vFSMComponentExecutionType.OnStateUpdate)
            {
                Flee(fsmBehaviour);
            }
            else if (executionType == vFSMComponentExecutionType.OnStateEnter)
            {
                Flee(fsmBehaviour);
                // force update path to be really fast for quick time, so the AI can react quickly and flee
                fsmBehaviour.aiController.ForceUpdatePath();
            }
        }

        protected virtual void Flee(vIFSMBehaviourController fsmBehaviour)
        {
            // FLEEING FROM DAMAGE SENDER
            if (fsmBehaviour != null && fsmBehaviour.aiController.receivedDamage.isValid && fsmBehaviour.aiController.receivedDamage.lastSender != null)
            {
                if (fsmBehaviour.aiController.remainingDistance < 1)
                {
                    for (int i = 1; i < 36; i++)
                    {
                        if (InTimer(fsmBehaviour, 1, "FleeTimer"))
                        {
                            if (Vector3.Distance(fsmBehaviour.aiController.targetDestination, fsmBehaviour.aiController.transform.position) < fleeDistance * 0.25f + fsmBehaviour.aiController.stopingDistance || fsmBehaviour.aiController.isInDestination)
                            {
                                if (debugMode) Debug.Log("Fleeing from damage sender");
                                var threatPoint = fsmBehaviour.aiController.receivedDamage.lastSender.position;
                                var fleeDir = fsmBehaviour.aiController.transform.position - threatPoint;
                                fleeDir = Quaternion.Euler(0, Random.Range(-(5 * i), 5 * i), 0) * fleeDir.normalized;
                                fleeDir.y = 0f;
                                if (debugFleeDirection) Debug.DrawRay(fsmBehaviour.aiController.transform.position, fleeDir * fleeDistance, Color.yellow, 10f);
                                fsmBehaviour.aiController.SetSpeed(fleeSpeed);
                                fsmBehaviour.aiController.MoveTo(fsmBehaviour.aiController.transform.position + fleeDir * fleeDistance);
                                fsmBehaviour.aiController.ForceUpdatePath();
                            }
                        }
                        else i--;
                    }
                }

            }
            // FLEEING FROM A TARGET
            else if (fsmBehaviour != null && fsmBehaviour.aiController.currentTarget.transform != null)
            {
                for (int i = 1; i < 36; i++)
                {
                    if (InTimer(fsmBehaviour, 1, "FleeTimer"))
                    {
                        if (Vector3.Distance(fsmBehaviour.aiController.targetDestination, fsmBehaviour.aiController.transform.position) < fleeDistance * 0.25f + fsmBehaviour.aiController.stopingDistance || fsmBehaviour.aiController.isInDestination)
                        {
                            if (debugMode) Debug.Log("Fleeing from a target");
                            var threatPoint = fsmBehaviour.aiController.currentTarget.transform.position;
                            var fleeDir = fsmBehaviour.aiController.transform.position - threatPoint;
                            fleeDir = Quaternion.Euler(0, Random.Range(-(5 * i), 5 * i), 0) * fleeDir.normalized;
                            if (debugFleeDirection) Debug.DrawRay(fsmBehaviour.aiController.transform.position, fleeDir * fleeDistance, Color.yellow, 10f);
                            fsmBehaviour.aiController.SetSpeed(fleeSpeed);
                            fsmBehaviour.aiController.MoveTo(fsmBehaviour.aiController.transform.position + fleeDir * fleeDistance);
                            fsmBehaviour.aiController.ForceUpdatePath();
                        }
                    }
                    else i--;
                }

            }
            // FLEEING WITHOUT TARGET OR DAMAGE SENDER 
            else if (fsmBehaviour != null)
            {
                for (int i = 1; i < 36; i++)
                {
                    if (InTimer(fsmBehaviour, 1, "FleeTimer"))
                    {
                        if (Vector3.Distance(fsmBehaviour.aiController.targetDestination, fsmBehaviour.aiController.transform.position) < fleeDistance * 0.25f + fsmBehaviour.aiController.stopingDistance || fsmBehaviour.aiController.isInDestination)
                        {
                            if (debugMode) Debug.Log("Fleeing without target or damage sender");
                            var fleeDir = fsmBehaviour.aiController.transform.forward;
                            fleeDir = Quaternion.Euler(0, Random.Range(-(10 * i), 10 * (i)), 0) * fleeDir.normalized;
                            if (debugFleeDirection) Debug.DrawRay(fsmBehaviour.aiController.transform.position, fleeDir * fleeDistance, Color.yellow, 10f);
                            fsmBehaviour.aiController.SetSpeed(fleeSpeed);
                            fsmBehaviour.aiController.MoveTo(fsmBehaviour.aiController.transform.position + fleeDir * fleeDistance);
                            fsmBehaviour.aiController.ForceUpdatePath();
                        }
                    }
                    else i--;
                }
            }
        }
    }
}