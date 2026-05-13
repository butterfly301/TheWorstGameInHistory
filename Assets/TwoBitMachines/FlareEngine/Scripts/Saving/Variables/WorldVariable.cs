using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("")]
    public class WorldVariable : MonoBehaviour
    {
        public static List<WorldVariable> variables = new();
        [SerializeField] public bool isHealth;
        [SerializeField] public bool initialized;

        public virtual void Reset()
        {
        }

        private void OnEnable()
        {
            if (!variables.Contains(this)) variables.Add(this);
            Register();
        }

        public static void SaveData()
        {
            for (var i = 0; i < variables.Count; i++) variables[i].Save();
        }

        public static void ResetAndClear()
        {
            for (var i = 0; i < variables.Count; i++)
            {
                variables[i].ClearTempValue();
                variables[i].Reset();
            }
        }

        public static void ClearTempChildren()
        {
            variables.Clear();
            Health.health.Clear();
        }

        public virtual void Initialize()
        {
        }

        public virtual void Save()
        {
        }

        public virtual void ClearTempValue()
        {
        }

        public virtual void Register()
        {
        }

        public virtual bool IncrementValue(Transform aggressor, float floatValue, Vector2 direction)
        {
            return false;
        }

        public virtual void InternalSet(float newValue)
        {
        }

        public virtual void DeleteSavedData()
        {
        }

        public virtual string Name()
        {
            return "";
        }
    }

    [Serializable]
    public class SaveFloat
    {
        public float value;
    }

    [Serializable]
    public class SaveString
    {
        public string value;
    }

    [Serializable]
    public class SaveVector3
    {
        public Vector3 value;
    }

    [Serializable]
    public class SaveBool
    {
        public bool value;
    }

    [Serializable]
    public class SaveStringList
    {
        public List<string> list = new();
    }

    public enum SaveType
    {
        Automatic,
        Manually
    }
}