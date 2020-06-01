﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Invector.vItemManager
{
    public class vEquipAreaControl : MonoBehaviour
    {
        [HideInInspector]
        public List<vEquipArea> equipAreas;

        void Start()
        {
            equipAreas = GetComponentsInChildren<vEquipArea>().vToList();
            foreach (vEquipArea area in equipAreas)
                area.onPickUpItemCallBack = OnPickUpItemCallBack;

            vInventory inventory = GetComponentInParent<vInventory>();
            if (inventory)
                inventory.onOpenCloseInventory.AddListener(OnOpen);
        }

        public void OnOpen(bool value)
        {

        }

        public void OnPickUpItemCallBack(vEquipArea area, vItemSlot slot)
        {
            for (int i = 0; i < equipAreas.Count; i++)
            {
                var sameSlots = equipAreas[i].equipSlots.FindAll(slotInArea =>slotInArea!=slot &&  slotInArea.item != null && slotInArea.item == slot.item);
                for (int a = 0; a < sameSlots.Count; a++)
                {                   
                    equipAreas[i].UnequipItem(sameSlots[a]);
                }
            }
            CheckTwoHandItem(area, slot);
        }

        void CheckTwoHandItem(vEquipArea area, vItemSlot slot)
        {
            if (slot.item == null) return;
            var opposite = equipAreas.Find(_area => _area != null && area.equipPointName.Equals("LeftArm") && _area.currentEquipedItem != null);
            //var RightEquipmentController = changeEquipmentControllers.Find(equipCtrl => equipCtrl.equipArea != null && equipCtrl.equipArea.equipPointName.Equals("RightArm"));
            if (area.equipPointName.Equals("LeftArm"))
                opposite = equipAreas.Find(_area => _area != null && area.equipPointName.Equals("RightArm") && _area.currentEquipedItem != null);
            else if (!area.equipPointName.Equals("RightArm"))
            {
                return;
            }
            if (opposite != null && (slot.item.twoHandWeapon || opposite.currentEquipedItem.twoHandWeapon))
            {
                opposite.onUnequipItem.Invoke(opposite, slot.item);
                opposite.UnequipItem(slot as vEquipSlot);
            }
        }
    }

}
