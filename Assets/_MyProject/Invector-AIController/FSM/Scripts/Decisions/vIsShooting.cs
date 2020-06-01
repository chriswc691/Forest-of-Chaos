namespace Invector.vCharacterController.AI.FSMBehaviour
{
#if UNITY_EDITOR
    [vFSMHelpbox("Requires a AIControlShooter - Verify if it's currently Shooting", UnityEditor.MessageType.Info)]
#endif
    public class vIsShooting : vStateDecision
    {
        public override string categoryName
        {
            get { return ""; }
        }
        public override string defaultName
        {
            get { return "Is Shooting?"; }
        }

        public override bool Decide(vIFSMBehaviourController fsmBehaviour)
        {
            if (fsmBehaviour.aiController is vIControlAIShooter)
            {
                vIControlAIShooter shooter = (fsmBehaviour.aiController as vIControlAIShooter);
                return shooter.shooterManager ? shooter.shooterManager.isShooting || shooter.isAttacking : false;
            }
            return true;
        }
    }
}