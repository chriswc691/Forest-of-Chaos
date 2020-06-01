using System.Collections;
using UnityEngine;

namespace Invector
{
    [vClassHeader("AI SIMPLE HOLDER")]
    public class vSimpleHolder : vMonoBehaviour
    {
        [vHelpBox("These GameObjects are just the weapon mesh to display in the character \n Use events to manipulate the Original Weapon Prefab", vHelpBoxAttribute.MessageType.Info)]
        [Tooltip("The Holder object is just the holder mesh without any colliders or components")]
        public GameObject holderObject;
        [Tooltip("The Weapon object is just the weapon mesh without any colliders or components")]
        public GameObject weaponObject;
        [vHelpBox("Trigger a Equip animation - Check the UpperBody Layer, EquipWeapon State too see how it works.", vHelpBoxAttribute.MessageType.Info)]
        public Animator animator;
        public string equipAnim = "LowBack";
        public string unequipAnim = "LowBack";
        public float equipDelayTime = 0.5f;
        public float unequipDelayTime = 0.5f;
        [vReadOnly]
        [SerializeField]
        protected bool isEquiped;
        private bool inEquip;
        [vHelpBox("Use to enable or disable the Original Weapon Prefab", vHelpBoxAttribute.MessageType.Info)]
        [vHelpBox("Is called when fake weapon (Weapon Mesh) is enabled")]
        public UnityEngine.Events.UnityEvent onEnableWeapon;
        [vHelpBox("Is called when fake weapon (Weapon Mesh) is disabled")]
        public UnityEngine.Events.UnityEvent onDisableWeapon;
        [vHelpBox("Is called when fake holder (Holder Mesh) is enabled")]
        public UnityEngine.Events.UnityEvent onEnableHolder;
        [vHelpBox("Is called when fake holder (Holder Mesh) is disabled")]
        public UnityEngine.Events.UnityEvent onDisableHolder;

        void SetActiveHolder(bool active)
        {
            if (holderObject)
            {
                holderObject.SetActive(active);
            }

            if (active) onEnableHolder.Invoke(); else onDisableHolder.Invoke();
        }

        void SetActiveWeapon(bool active)
        {
            if (weaponObject)
            {
                weaponObject.SetActive(active);
            }

            if (active) onEnableWeapon.Invoke(); else onDisableWeapon.Invoke();
        }

        public void EquipWeapon()
        {
            if (isEquiped) return;
            if (animator)
            {
                animator.CrossFadeInFixedTime(equipAnim, 0.25f);
            }

            StartCoroutine(EquipRoutine());
        }

        public void UnequipWeapon()
        {
            if (!isEquiped) return;
            if (animator)
            {
                animator.CrossFadeInFixedTime(unequipAnim, 0.25f);
            }
            StartCoroutine(UnequipRoutine());
        }

        IEnumerator UnequipRoutine()
        {
            SetActiveHolder(true);
            if (!inEquip) // ignore time if inEquip or immediate unequip
            {
                var equipTime = unequipDelayTime;
                while (equipTime > 0 && !inEquip)
                {
                    yield return null;
                    equipTime -= Time.deltaTime;
                }
            }
            SetActiveWeapon(true);
            isEquiped = false;
        }

        IEnumerator EquipRoutine()
        {
            inEquip = true;
            SetActiveHolder(true);
            var equipTime = equipDelayTime;
            while (equipTime > 0) // ignore time if  immediate equip
            {
                yield return null;
                equipTime -= Time.deltaTime;
            }

            SetActiveWeapon(false);
            inEquip = false;
            isEquiped = true;
        }
    }
}

