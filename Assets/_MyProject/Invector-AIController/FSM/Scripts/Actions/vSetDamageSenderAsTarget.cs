using UnityEngine;

namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Set The Damage sender as AI Target", UnityEditor.MessageType.Info)]
#endif
    public class vSetDamageSenderAsTarget : vStateAction
    {       
       public override string categoryName
        {
            get { return "Detection/"; }
        }
        public override string defaultName
        {
            get { return "Set DamageSender as Target"; }
        }
        public vSetDamageSenderAsTarget()
        {
            executionType = vFSMComponentExecutionType.OnStateEnter;
        }
        public override void DoAction(vIFSMBehaviourController fsmBehaviour, vFSMComponentExecutionType executionType = vFSMComponentExecutionType.OnStateUpdate)
        {

            if(fsmBehaviour.aiController.receivedDamage.lastSender)
            {
                fsmBehaviour.aiController.SetCurrentTarget(fsmBehaviour.aiController.receivedDamage.lastSender);
            }          
        }
    }
}