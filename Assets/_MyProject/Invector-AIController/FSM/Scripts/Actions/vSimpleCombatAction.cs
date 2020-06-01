using Invector.vEventSystems;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a ControlAICombat - Simple Combat based on this AI Controller Combat Settings", UnityEditor.MessageType.Info)]
#endif
    public class vSimpleCombatAction : vStateAction
    {
        public bool engageInStrafe = false;
        public vAIMovementSpeed engageSpeed = vAIMovementSpeed.Running;
        public vAIMovementSpeed combatSpeed = vAIMovementSpeed.Walking;
        public override string categoryName
        {
            get { return "Combat/"; }
        }
        public override string defaultName
        {
            get { return "Melee Combat"; }
        }

        public vSimpleCombatAction()
        {
            this.executionType = vFSMComponentExecutionType.OnStateEnter | vFSMComponentExecutionType.OnStateExit | vFSMComponentExecutionType.OnStateUpdate;
        }        

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (fsmBehaviour.aiController is vIControlAICombat)
            {
                var combatController = (fsmBehaviour.aiController as vIControlAICombat);
                switch (executionType)
                {
                    case vFSMComponentExecutionType.OnStateEnter:
                        OnEnterCombat(combatController);
                        break;

                    case vFSMComponentExecutionType.OnStateExit:
                        OnExitCombat(combatController);
                        break;

                    case vFSMComponentExecutionType.OnStateUpdate:
                        OnUpdateCombat(combatController);
                        break;
                }
            }   
        }

        protected virtual void OnEnterCombat(vIControlAICombat controller)
        {
            controller.InitAttackTime();
            controller.isInCombat = true;
        }

        protected virtual void OnExitCombat(vIControlAICombat controller)
        {

            if (controller.currentTarget.transform == null || controller.currentTarget.isDead || !controller.targetInLineOfSight) controller.ResetAttackTime();
            controller.isInCombat = false;
        }

        protected virtual void OnUpdateCombat(vIControlAICombat controller)
        {
            if (controller.currentTarget.transform == null || controller.currentTarget.isLost)
            {
                return;
            }

            if (controller != null)
            {
                if (controller.canAttack)
                    EngageTarget(controller);
                else CombatMovement(controller);
                ControlLookPoint(controller);
            }
        }

        protected virtual void EngageTarget(vIControlAICombat controller)
        {
            if (controller.currentTarget.transform == null) return;
            controller.SetSpeed(engageSpeed);
            if (controller.targetDistance <= controller.attackDistance)
            {
                controller.Stop();
                controller.Attack();
            }
            else if (!controller.animatorStateInfos.HasAnyTag("Attack", "LockMovement", "CustomAction"))
            {
                if (engageInStrafe)
                    controller.StrafeMoveTo(controller.currentTarget.transform.position, (controller.currentTarget.transform.position - controller.transform.position).normalized);
                else controller.MoveTo(controller.currentTarget.transform.position);
            }
            else controller.Stop();
        }

        protected virtual void CombatMovement(vIControlAICombat controller)
        {
            controller.SetSpeed(combatSpeed);
            if (controller.strafeCombatMovement)
                StrafeCombatMovement(controller);
            else
                SimpleCombatMovement(controller);
            if (controller.canBlockInCombat)
            {
                controller.Blocking();
            }
        }

        protected virtual void ControlLookPoint(vIControlAICombat controller)
        {
            if (controller.currentTarget.transform == null || !controller.currentTarget.hasCollider)
                return;

            var movepoint = (controller.lastTargetPosition);
            controller.LookTo(movepoint);
        }

        protected virtual void SimpleCombatMovement(vIControlAICombat controller)
        {
            bool moveForward = controller.targetDistance > controller.combatRange * 0.8f;
            bool moveBackWard = controller.targetDistance < controller.minDistanceOfTheTarget;
            var forwardMovement = (controller.currentTarget.transform.position - controller.transform.position).normalized * (moveForward ? 1 + controller.stopingDistance : (moveBackWard ? -(1 + controller.stopingDistance) : 0)); controller.StrafeMoveTo(controller.transform.position + forwardMovement, (controller.currentTarget.transform.position - controller.transform.position).normalized);

        }

        protected virtual void StrafeCombatMovement(vIControlAICombat controller)
        {
            bool moveForward = controller.targetDistance > controller.combatRange * 0.8f;
            bool moveBackward = controller.targetDistance < controller.minDistanceOfTheTarget;
            var movepoint = (controller.lastTargetPosition);
            var forwardMovement = (movepoint - controller.transform.position).normalized * (moveForward ? 1 + controller.stopingDistance : (moveBackward ? -(1 + controller.stopingDistance) : 0));
            controller.StrafeMoveTo(controller.transform.position + (controller.transform.right * ((controller.stopingDistance + 1f)) * controller.strafeCombatSide) + forwardMovement, (movepoint - controller.transform.position).normalized);
        }        
    }
}