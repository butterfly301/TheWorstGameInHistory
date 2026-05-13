using System;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    public class SpriteTrailUnit : MonoBehaviour
    {
        [NonSerialized] public SpriteTrailEffect effect;

        private void LateUpdate()
        {
            if (effect != null) effect.RunTrail();
        }
    }
}