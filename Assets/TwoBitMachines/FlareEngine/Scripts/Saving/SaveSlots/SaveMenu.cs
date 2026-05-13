using System;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    public class SaveMenu : MonoBehaviour
    {
        [SerializeField]
        public RectTransform highlight; // identifies the current save slot selected by user in the ui menu

        [SerializeField]
        public RectTransform selected; // identifies the current save slot being used for saving game state

        [NonSerialized] private SaveSlotUI[] list = new SaveSlotUI[1];
        [NonSerialized] private SaveOptions save;

        [NonSerialized] private SaveSlotUI saveSlotUI;

        private void Awake()
        {
            list = gameObject.GetComponentsInChildren<SaveSlotUI>();
        }

        private void Start()
        {
            save = WorldManager.get.save;

            for (var i = 0; i < save.slot.Count; i++)
                if (i < list.Length)
                {
                    list[i].Initialize(this, save, save.slot[i], i);
                    if (i == save.currentSlot) OnSelect(list[i], true);
                }
        }

        public void DeleteSlotData()
        {
            saveSlotUI?.DeleteSlotData();
            ForceSelect();
        }

        public void CopySlotData()
        {
            SaveOptions.Load(ref save);

            for (var i = 0; i < save.slot.Count; i++)
                if (save.currentSlot != i && !save.slot[i].initialized)
                {
                    save.slot[i].initialized = true;
                    save.slot[i].level = save.slot[save.currentSlot].level;
                    save.slot[i].totalTime = save.slot[save.currentSlot].totalTime;
                    if (i < list.Length)
                    {
                        list[i].SetText(save.slot[i]);
                        list[i].slotHasBeenInitialized.Invoke();
                    }

                    save.Save();
                    WorldManager.get.save = save;
                    return;
                }
        }

        public void
            OnSelect(SaveSlotUI saveSlotUI,
                bool forceSelect = false) // called automatically when inventory slot is selected
        {
            if (this.saveSlotUI == saveSlotUI || forceSelect) SelectSlot(saveSlotUI.slotIndex);

            this.saveSlotUI = saveSlotUI;
            if (highlight != null) SetPosition(highlight, saveSlotUI.transform, saveSlotUI.highlightOffset);
        }

        private void SelectSlot(int slotIndex)
        {
            for (var i = 0; i < list.Length; i++)
                if (i == slotIndex)
                {
                    list[i].SelectSlot();
                    SetPosition(selected, list[i].transform, list[i].selectedOffset);
                    break;
                }
        }

        private void ForceSelect()
        {
            // at least one must be active
            SaveOptions.Load(ref save);

            var allFalse = true;
            for (var i = 0; i < save.slot.Count; i++)
                if (save.slot[i].initialized)
                    allFalse = false;

            if (allFalse && list.Length > 0) // none are selected, so force one
                OnSelect(list[0], true);
            if (!allFalse)
            {
                // current slot is set to one that is not initialized
                var move = false;
                for (var i = 0; i < save.slot.Count; i++)
                    if (i == save.currentSlot && !save.slot[i].initialized)
                    {
                        move = true;
                        break;
                    }

                if (move)
                    for (var i = 0; i < save.slot.Count; i++)
                        if (save.slot[i].initialized)
                        {
                            save.currentSlot = i;
                            save.Save();
                            WorldManager.get.save = save;
                            if (i < list.Length)
                            {
                                saveSlotUI = list[i];
                                SetPosition(selected, list[i].transform, list[i].selectedOffset);
                                SetPosition(highlight, list[i].transform, list[i].highlightOffset);
                            }

                            return;
                        }

                // refresh select and highlight rects
                for (var i = 0; i < save.slot.Count; i++)
                    if (i == save.currentSlot)
                    {
                        saveSlotUI = list[i];
                        SetPosition(selected, list[i].transform, list[i].selectedOffset);
                        SetPosition(highlight, list[i].transform, list[i].highlightOffset);
                        break;
                    }
            }
        }

        private void SetPosition(RectTransform targetRect, Transform destination, Vector2 offset)
        {
            var rectTransform = destination.GetComponent<RectTransform>();

            if (rectTransform == null) return;

            var element2Position = rectTransform.anchoredPosition;

            var finalPosition = element2Position + offset;

            // Set the anchored position of element1 to match element2's position with the offset
            targetRect.anchoredPosition = finalPosition;
        }
    }
}