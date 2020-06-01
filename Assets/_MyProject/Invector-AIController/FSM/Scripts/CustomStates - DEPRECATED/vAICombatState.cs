
using System;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public abstract class vAICombatState : vFSMState
    {
        public override Type requiredType
        {
            get
            {
                return typeof(vIControlAICombat);
            }
        }

        public override void UpdateState(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController is vIControlAICombat)
                UpdateCombatState(fsmBehaviour.aiController as vIControlAICombat);

            base.UpdateState(fsmBehaviour);
        }

        /// <summary>
        /// Called from UpdateState if vIControlAI is a vIControlAICombat
        /// </summary>
        /// <param name="ctrlAICombat"></param>
        protected abstract void UpdateCombatState(vIControlAICombat ctrlAICombat);

    }
}