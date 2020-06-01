using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a Headtrack to Look to the last damage sender", UnityEditor.MessageType.Info)]
#endif
    public class vLookToDamageSender : vStateAction
    {       
       public override string categoryName
        {
            get { return "Controller/"; }
        }
        public override string defaultName
        {
            get { return "Look To Damage Sender (Headtrack)"; }
        }

        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {
            if (fsmBehaviour.aiController.receivedDamage.lastSender)
            {
                fsmBehaviour.aiController.LookToTarget(fsmBehaviour.aiController.receivedDamage.lastSender);
            }
        }
    }
}