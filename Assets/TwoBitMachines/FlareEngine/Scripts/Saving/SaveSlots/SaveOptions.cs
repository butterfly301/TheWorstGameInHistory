using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    [Serializable]
    public class SaveOptions
    {
        public const string folder = "Static";
        public const string key = "Misc";
        [SerializeField] public List<SaveSlot> slot = new();
        [SerializeField] public int currentSlot;
        [SerializeField] public string gameName = "";
        [SerializeField] public bool navigate; // encrypt
        [SerializeField] public bool marked;
        [SerializeField] public bool delete;
        [SerializeField] public int sceneDoor;
        [SerializeField] public int sceneDoorPlayerDirection;

        public void Save()
        {
            Storage.Save(this, folder, key); // general save, while using in editor
        }

        public void Save(int levelNumber, float playTime, bool isSaveMenu)
        {
            if (!isSaveMenu)
                for (var i = 0; i < slot.Count; i++)
                    if (i == currentSlot)
                    {
                        slot[i].UpdateSettings(levelNumber, playTime);
                        break;
                    }

            Storage.Save(this, folder, key);
        }

        public void DeleteSlotData(int slotIndex)
        {
            for (var i = 0; i < slot.Count; i++)
                if (i == slotIndex)
                {
                    slot[i].ClearSettings();
                    Storage.DeleteAll("Slot" + i);
                }

            Storage.Save(this, folder, key); // general save, while using in editor
        }

        public void DeleteAllSlotsData()
        {
            for (var i = 0; i < slot.Count; i++)
            {
                slot[i].ClearSettings();
                Storage.DeleteAll("Slot" + i);
            }

            currentSlot = 0;
            Storage.Save(this, folder, key);
        }

        public string RetrieveSaveFolder()
        {
            if (slot.Count == 0 || currentSlot < 0 || currentSlot >= slot.Count) return gameName;
            for (var i = 0; i < slot.Count; i++)
                if (i == currentSlot)
                    return "Slot" + i; // we make sure folder does indeed exist

            return gameName;
        }

        public static void Load(ref SaveOptions save)
        {
            save = Storage.Load(save, folder, key);
        }
    }

    [Serializable]
    public class SaveSlot
    {
        [SerializeField] public bool initialized;
        [SerializeField] public float totalTime;
        [SerializeField] public float level;

        public void UpdateSettings(int levelNumber, float playTime)
        {
            if (levelNumber > level) level = levelNumber;
            totalTime += playTime;
        }

        public void ClearSettings()
        {
            level = 0;
            totalTime = 0;
            initialized = false;
        }
    }
}