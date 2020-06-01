﻿using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Invector.vItemManager
{
    public class vItemWindowDisplay : MonoBehaviour
    {
        public vInventory inventory;
        public vItemWindow itemWindow;
        public vItemOptionWindow optionWindow;
        [HideInInspector]
        public vItemSlot currentSelectedSlot;
        [HideInInspector]
        public int amount;

        public void OnEnable()
        {
            if (inventory == null)
                inventory = GetComponentInParent<vInventory>();
           
            if (inventory && itemWindow)
            {
                inventory.onLeaveItem.RemoveListener(OnDestroyItem);
                inventory.onLeaveItem.AddListener(OnDestroyItem);
                itemWindow.CreateEquipmentWindow(inventory.items, OnSubmit, OnSelectSlot);
            }
        
              
        }

        public void OnDestroyItem(vItem item, int amount)
        {
            var _slot = itemWindow.slots.Find(slot => slot.item.Equals(item));
            if (_slot != null)
            {
                itemWindow.slots.Remove(_slot);
                Destroy(_slot.gameObject);
            }
        }

        public void OnSubmit(vItemSlot slot)
        {
            currentSelectedSlot = slot;
            if (slot.item)
            {
                var rect = slot.GetComponent<RectTransform>();
                if (optionWindow.CanOpenOptions(slot.item))
                {
                    optionWindow.transform.position = rect.position;
                    optionWindow.gameObject.SetActive(true);
                    optionWindow.EnableOptions(slot);
                    // currentSelectedSlot = slot;
                }

            }
        }

        public void OnSelectSlot(vItemSlot slot)
        {
            currentSelectedSlot = slot;
        }

        public void DropItem()
        {
            if (amount > 0)
            {
                inventory.OnDropItem(currentSelectedSlot.item, amount);
                if (currentSelectedSlot != null && (currentSelectedSlot.item == null || currentSelectedSlot.item.amount <= 0))
                {
                    if (itemWindow.slots.Contains(currentSelectedSlot))
                        itemWindow.slots.Remove(currentSelectedSlot);
                    Destroy(currentSelectedSlot.gameObject);
                    if (itemWindow.slots.Count > 0)
                        SetSelectable(itemWindow.slots[0].gameObject);
                }
            }
        }

        public void LeaveItem()
        {
            if (amount > 0)
            {
                inventory.OnLeaveItem(currentSelectedSlot.item, amount);
                if (currentSelectedSlot != null && (currentSelectedSlot.item == null || currentSelectedSlot.item.amount <= 0))
                {
                    if (itemWindow.slots.Contains(currentSelectedSlot))
                        itemWindow.slots.Remove(currentSelectedSlot);
                    Destroy(currentSelectedSlot.gameObject);
                    if (itemWindow.slots.Count > 0)
                        SetSelectable(itemWindow.slots[0].gameObject);
                }
            }
        }

        public void UseItem()
        {
            //currentSelectedSlot.item.amount--;
            inventory.OnUseItemImmediate(currentSelectedSlot.item);
            if (currentSelectedSlot != null && (currentSelectedSlot.item == null || currentSelectedSlot.item.amount <= 0))
            {
                if (itemWindow.slots.Contains(currentSelectedSlot))
                    itemWindow.slots.Remove(currentSelectedSlot);
                Destroy(currentSelectedSlot.gameObject);
                if (itemWindow.slots.Count > 0)
                    SetSelectable(itemWindow.slots[0].gameObject);
            }
        }

        public void SetOldSelectable()
        {
            try
            {
                if (currentSelectedSlot != null)
                    SetSelectable(currentSelectedSlot.gameObject);
                else if (itemWindow.slots.Count > 0 && itemWindow.slots[0] != null)
                {
                    SetSelectable(itemWindow.slots[0].gameObject);
                }
            }
            catch
            {

            }
        }

        void SetSelectable(GameObject target)
        {
            try
            {
                var pointer = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, pointer, ExecuteEvents.pointerExitHandler);
                EventSystem.current.SetSelectedGameObject(target, new BaseEventData(EventSystem.current));
                ExecuteEvents.Execute(target, pointer, ExecuteEvents.selectHandler);
            }
            catch { }

        }

    }
}
