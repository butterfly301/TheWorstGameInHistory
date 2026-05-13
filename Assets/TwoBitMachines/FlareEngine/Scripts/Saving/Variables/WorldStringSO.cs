using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    public class WorldStringSO : ScriptableObject
    {
        public string value;
        private WorldString reference;

        public void SetWorldValue(string newValue)
        {
            value = newValue;
        }

        public void Register(WorldString worldString)
        {
            reference = worldString;
        }

        public void SetValue(string newValue)
        {
            if (reference != null) reference.SetValue(newValue);
        }
    }
}