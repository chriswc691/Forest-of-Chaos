using System;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public class vAIShooterCombatState : vAICombatState
    {
        public vAIMovementSpeed engageSpeed = vAIMovementSpeed.Running;
        public override Type requiredType
        {
            get
            {
                return typeof(vIControlAIShooter);
            }
        }
        public vAIShooterCombatState()
        {
#if UNITY_EDITOR
            description = "Custom Shooter Combat State";
#endif
        }

        public override void OnStateEnter(vIFSMBehaviourController fsmBehaviour)
        {
            base.OnStateEnter(fsmBehaviour);

            if (fsmBehaviour.aiController is vIControlAICombat)
            {
                var aiCombat = (fsmBehaviour.aiController as vIControlAICombat);
                aiCombat.InitAttackTime();
                aiCombat.isInCombat = true;
            }
        }

        public override void OnStateExit(vIFSMBehaviourController fsmBehaviour)
        {
            base.OnStateExit(fsmBehaviour);
            if (fsmBehaviour.aiController is vIControlAICombat)
            {
                var aiCombat = (fsmBehaviour.aiController as vIControlAICombat);
                aiCombat.ResetAttackTime();
                aiCombat.isInCombat = false;
            }
        }

        protected override void UpdateCombatState(vIControlAICombat controller)
        {
            if (controller.currentTarget.transform == null) return;

            if (controller != null)
            {
                if (controller.targetDistance > controller.attackDistance)
                    EngageTarget(controller);
                else
                    CombatMovement(controller);

                ControlLookPoint(controller);
                HandleShotAttack(controller);
            }
        }

        protected virtual void HandleShotAttack(vIControlAICombat controller)
        {
            controller.AimToTarget();

            if (controller.canAttack)
                controller.Attack();
        }

        protected virtual void EngageTarget(vIControlAICombat controller)
        {
            if (controller.currentTarget.transform == null)
                return;

            controller.SetSpeed(engageSpeed);

            if (!controller.animatorStateInfos.HasAnyTag("Attack", "LockMovement", "CustomAction"))
            {
                var movepoint = (controller.lastTargetPosition);
                controller.StrafeMoveTo(movepoint, movepoint - controller.transform.position);
            }
        }

        protected virtual void CombatMovement(vIControlAICombat controller)
        {
            controller.SetSpeed(engageSpeed);
            if (controller.strafeCombatMovement)
                StrafeCombafeMovement(controller);
            else
                SimpleCombatMovement(controller);
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
            bool moveBackward = controller.targetDistance < controller.minDistanceOfTheTarget;
            var movepoint = (controller.lastTargetPosition);
            var forwardMovement = (movepoint - controller.transform.position).normalized * (moveForward ? 1 + controller.stopingDistance : (moveBackward ? -(1 + controller.stopingDistance) : 0));
            controller.StrafeMoveTo(controller.transform.position + forwardMovement, (movepoint - controller.transform.position).normalized);

        }

        protected virtual void StrafeCombafeMovement(vIControlAICombat controller)
        {
            bool moveForward = controller.targetDistance > controller.combatRange * 0.8f;
            bool moveBackward = controller.targetDistance < controller.minDistanceOfTheTarget;
            var movepoint = (controller.lastTargetPosition);
            var forwardMovement = (movepoint - controller.transform.position).normalized * (moveForward ? 1 + controller.stopingDistance : (moveBackward ? -(1 + controller.stopingDistance) : 0));           
            controller.StrafeMoveTo(controller.transform.position + (controller.transform.right * ((controller.stopingDistance + 1f)) * controller.strafeCombatSide) + forwardMovement, (movepoint - controller.transform.position).normalized);
        }
    }
}