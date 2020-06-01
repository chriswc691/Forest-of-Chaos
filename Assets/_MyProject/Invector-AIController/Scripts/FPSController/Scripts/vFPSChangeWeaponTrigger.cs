using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Invector
{   
    public class vFPSChangeWeaponTrigger : MonoBehaviour
    {
        public bool pressButtonToChange;
        //public vCharacterController.GenericInput button = new vCharacterController.GenericInput("E", "A", "A");
        public string targetWeaponName;
        public UnityEngine.Events.UnityEvent onChangeWeapon;
        private void OnTriggerStay(Collider other)
        {
            if (/*button.GetButtonDown() ||*/ !pressButtonToChange)
            {
                var weaponManager = other.GetComponentInParent<vFPSWeaponManager>();
                if (weaponManager)
                {
                    weaponManager.EquipWeapon(targetWeaponName);
                    onChangeWeapon.Invoke();
                }
            }
        }       
    }
}