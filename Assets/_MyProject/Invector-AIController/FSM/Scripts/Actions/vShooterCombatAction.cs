using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a ControlAIShooter - Shooter Combat based on this AI Controller Shooter Settings", UnityEditor.MessageType.Info)]
#endif
    public class vShooterCombatAction : vSimpleCombatAction
    {
        public override string categoryName
        {
            get { return "Combat/"; }
        }
        public override string defaultName
        {
            get { return "Shooter Combat"; }
        }
       
        protected override void OnUpdateCombat(vIControlAICombat controller)
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

        protected override void EngageTarget(vIControlAICombat controller)
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

        protected override void CombatMovement(vIControlAICombat controller)
        {
            controller.SetSpeed(combatSpeed);
            if (controller.strafeCombatMovement)
                StrafeCombatMovement(controller);
            else
                SimpleCombatMovement(controller);
        }        

        protected override void SimpleCombatMovement(vIControlAICombat controller)
        {
            bool moveForward = controller.targetDistance > controller.combatRange * 0.8f;
            bool moveBackward = controller.targetDistance < controller.minDistanceOfTheTarget;
            var movepoint = (controller.lastTargetPosition);
            var forwardMovement = (movepoint - controller.transform.position).normalized * (moveForward ? 1 + controller.stopingDistance : (moveBackward ? -(1 + controller.stopingDistance) : 0));
            controller.StrafeMoveTo(controller.transform.position + forwardMovement, (movepoint - controller.transform.position).normalized);

        }       
    }
}