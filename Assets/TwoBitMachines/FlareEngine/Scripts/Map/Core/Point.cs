using System;
using UnityEngine;

namespace TwoBitMachines.MapSystem
{
    [Serializable]
    public class Point
    {
        [SerializeField] public Vector2 position;
        [SerializeField] public Vector2 offsetEnd;
        [SerializeField] public Vector2 offsetStart;
        [SerializeField] public bool invisible;

        public Point(Vector2 position)
        {
            this.position = position;
        }

        public Vector2 controlEnd => position + offsetEnd;
        public Vector2 controlStart => position + offsetStart;
    }
}