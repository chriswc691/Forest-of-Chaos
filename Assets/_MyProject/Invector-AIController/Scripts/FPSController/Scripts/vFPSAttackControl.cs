using UnityEngine;

namespace Invector
{
    [vClassHeader("FPS Attack Control", openClose = false)]
    public class vFPSAttackControl : vMonoBehaviour
    {
        public float enableTime = 0, disableTime = 1;
        public UnityEngine.Events.UnityEvent onAttack, onEnableAttack, onDisableAttack, onActiveWeapon, onDisableWeapon;

        public void Attack()
        {
            onAttack.Invoke();
            Invoke("EnableAttack", enableTime);
            Invoke("DisableAttack", disableTime);
        }

        void EnableAttack()
        {
            onEnableAttack.Invoke();
        }

        void DisableAttack()
        {
            onDisableAttack.Invoke();
        }

        public void SetActiveWeapon(bool value)
        {
            if (value) onActiveWeapon.Invoke();
            else onDisableWeapon.Invoke();
        }
    }
}