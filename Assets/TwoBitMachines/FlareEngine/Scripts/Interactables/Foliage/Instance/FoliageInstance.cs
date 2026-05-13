using System;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Interactables
{
    [Serializable]
    public class FoliageInstance
    {
        public Vector3 position;
        public int textureIndex;
    }

    public enum FoliageBrush
    {
        Single,
        Random,
        Eraser,
        None
    }
}