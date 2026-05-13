using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    public class WorldBoolTracker : MonoBehaviour
    {
        public static WorldBoolTracker get;
        public static List<WorldBool> list = new();

        private void Start()
        {
            get = this;
            ClearAll();
        }

        public static void ClearAll()
        {
            if (list != null) list.Clear();
        }

        public void Register(WorldBool worldBool)
        {
            if (!list.Contains(worldBool)) list.Add(worldBool);
        }

        public void SetAllTrueAndClear()
        {
            for (var i = 0; i < list.Count; i++)
                if (list[i] != null)
                    list[i].SetValueAndSave(true);
            list.Clear();
        }

        public void SetAllFalseAndClear()
        {
            for (var i = 0; i < list.Count; i++)
                if (list[i] != null)
                    list[i].SetValueAndSave(false);
            list.Clear();
        }
    }
}