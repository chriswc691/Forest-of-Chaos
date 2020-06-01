using System;
using Invector.vEventSystems;
using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("This state has internal actions\nfor simple combat routine", UnityEditor.MessageType.Info)]
#endif
    public class vAISimpleCombatState : vAICombatState, vIStateAttackListener
    {
        public bool engageInStrafe = false;
        public vAIMovementSpeed engageSpeed = vAIMovementSpeed.Running;
        public vAIMovementSpeed combatSpeed = vAIMovementSpeed.Walking;
        public override Type requiredType
        {
            get
            {
                return typeof(vIControlAIMelee);
            }
        }
        public vAISimpleCombatState()
        {
#if UNITY_EDITOR
            description = "Custom Combat State";
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
                if (aiCombat.currentTarget.transform == null || aiCombat.currentTarget.isDead || !aiCombat.targetInLineOfSight) aiCombat.ResetAttackTime();
                aiCombat.isInCombat = false;
            }
        }

        protected override void UpdateCombatState(vIControlAICombat controller)
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
            if(controller.canBlockInCombat)
            {
                controller.Blocking();
            }
        }

        protected virtual void ControlLookPoint(vIControlAICombat controller)
        {
            if (controller.currentTarget.transform == null || !controller.currentTarget.hasCollider || !controller.targetInLineOfSight) return;
            controller.LookTo(controller.currentTarget.transform.position);
        }

        protected virtual void SimpleCombatMovement(vIControlAICombat controller)
        {
            bool moveForward = controller.targetDistance > controller.combatRange * 0.8f;
            bool moveBackWard = controller.targetDistance < controller.minDistanceOfTheTarget;
            var forwardMovement = (controller.currentTarget.transform.position - controller.transform.position).normalized * (moveForward ? 1 + controller.stopingDistance : (moveBackWard ? -(1 + controller.stopingDistance) : 0));            controller.StrafeMoveTo(controller.transform.position + forwardMovement, (controller.currentTarget.transform.position - controller.transform.position).normalized);

        }

        protected virtual void StrafeCombatMovement(vIControlAICombat controller)
        {
            bool moveForward = controller.targetDistance > controller.combatRange * 0.8f;
            bool moveBackWard = controller.targetDistance < controller.minDistanceOfTheTarget;
            var forwardMovement = (controller.currentTarget.transform.position - controller.transform.position).normalized * (moveForward ? 1 + controller.stopingDistance : (moveBackWard ? -(1 + controller.stopingDistance) : 0));
            controller.StrafeMoveTo(controller.transform.position + (controller.transform.right * ((controller.stopingDistance + 1f)) * controller.strafeCombatSide) + forwardMovement, (controller.currentTarget.transform.position - controller.transform.position).normalized);
        }

        public virtual void OnReceiveAttack(vIControlAICombat controller, ref vDamage damage, vIMeleeFighter attacker, ref bool canBlock)
        {
            //HandleDefensiveCombat(controller, true);
            if (damage.damageValue > 0)
                if (attacker != null && attacker.character!=null && !attacker.character.isDead)
                {
                    controller.SetCurrentTarget(attacker.transform);
                }
        }
    }
}