using System;
using UnityEngine;
using UnityEngine.UI;

namespace TwoBitMachines.FlareEngine
{
    public class DragItem : MonoBehaviour
    {
        [NonSerialized] public Image imageIcon;
        [NonSerialized] public ItemUI itemUI;
        [NonSerialized] public InventorySlot oldSlot;
        [NonSerialized] public RectTransform rectTransform;

        public bool active => gameObject.activeInHierarchy;
        public bool canTransfer => itemUI != null && oldSlot != null;

        private void Awake()
        {
            imageIcon = gameObject.GetComponent<Image>();
            rectTransform = gameObject.GetComponent<RectTransform>();
        }

        public void Enable(ItemUI item, InventorySlot oldSlot, Sprite icon)
        {
            if (item == null || oldSlot == null) return;

            gameObject.SetActive(true);
            itemUI = item;
            this.oldSlot = oldSlot;
            if (imageIcon != null) imageIcon.sprite = icon;
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}