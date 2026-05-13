using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.MapSystem
{
    [Serializable]
    public class Zone
    {
        [SerializeField] public string name = "New Zone";
        [SerializeField] public string sceneName = "Scene Name";
        [SerializeField] public float thickness = 0.2f;
        [SerializeField] public float resolution = 0.1f;
        [SerializeField] public Color32 outline = new(255, 255, 255, 255);
        [SerializeField] public Color32 background = new(155, 155, 155, 255);
        [SerializeField] public List<Room> roomList = new();

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] public bool foldOut;
        [SerializeField] public bool deleteAsk;
        [SerializeField] public bool delete;
        [SerializeField] public int roomIndex;
        [SerializeField] public List<bool> roomSaveList = new();

        public void SaveOriginalData()
        {
            roomSaveList.Clear();
            for (var i = 0; i < roomList.Count; i++) roomSaveList.Add(roomList[i].visible);
        }

        public void RestoreOriginalData()
        {
            for (var i = 0; i < roomList.Count && i < roomSaveList.Count; i++) roomList[i].visible = roomSaveList[i];
        }
#pragma warning restore 0414
#endif
    }
}