﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Invector.vCharacterController.vActions
{
    using vMelee;
    [vClassHeader("Collectable Standalone", "Use this component when your character doesn't have a ItemManager", openClose = false)]
    public class vCollectableStandalone : vTriggerGenericAction
    {
        public string targetEquipPoint;
        public GameObject weapon;
        public Sprite weaponIcon;
        public string weaponText;
        public UnityEvent OnEquip;
        public UnityEvent OnDrop;

        private vCollectMeleeControl manager;

        public override IEnumerator OnDoActionDelay(GameObject cc)
        {
            yield return StartCoroutine(base.OnDoActionDelay(cc));

            manager = cc.GetComponent<vCollectMeleeControl>();

            if (manager != null)
            {
                manager.HandleCollectableInput(this);
            }
        }
    }
}