using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("Flare Engine/一Saving/TransformTracker")]
    public class TransformTracker : MonoBehaviour
    {
        [NonSerialized] public static List<TransformTracker> tracker = new();

        public static TransformTracker get;
        [SerializeField] public string key = "name";
        [SerializeField] private SaveTransformList saveTransforms = new();

        public static void Reset()
        {
            for (var i = 0; i < tracker.Count; i++)
            {
                if (tracker[i] == null) continue;
                tracker[i].Restore();
            }
        }

        private void Start()
        {
            get = this;
            Restore();
        }

        private void OnEnable()
        {
            if (!tracker.Contains(this)) tracker.Add(this);
        }

        private void OnDisable()
        {
            if (tracker.Contains(this)) tracker.Remove(this);
            get = null;
        }

        public bool Contains(Transform transform)
        {
            return saveTransforms.Contains(transform);
        }

        public void AddToList(Transform transform)
        {
            saveTransforms.AddToList(transform);
        }

        public void AddToList(ImpactPacket packet)
        {
            saveTransforms.AddToList(packet.transform);
        }

        public void Save()
        {
            Storage.Save(saveTransforms, WorldManager.saveFolder, key);
        }

        private void Restore()
        {
            saveTransforms.ClearAll();
            saveTransforms = Storage.Load(saveTransforms, WorldManager.saveFolder, key);
        }
    }

    [Serializable]
    public class SaveTransformList
    {
        public List<Transform> list = new();

        public void ClearAll()
        {
            list.Clear();
        }

        public bool Contains(Transform transform)
        {
            return list.Contains(transform);
        }

        public void AddToList(Transform transform)
        {
            if (!list.Contains(transform)) list.Add(transform);
        }
    }
}