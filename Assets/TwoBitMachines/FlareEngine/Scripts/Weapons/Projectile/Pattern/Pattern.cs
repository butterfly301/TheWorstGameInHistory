using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TwoBitMachines.FlareEngine
{
    [Serializable]
    public class Pattern
    {
        [SerializeField] public float variance; // position variance
        [SerializeField] public float angle = 25f;
        [SerializeField] public int projectileRate = 1;
        [SerializeField] public Vector2 separation;
        [SerializeField] public FirePatternType fireDirection;
        private float randomize => Random.Range(-variance, variance);

        public bool Execute(ProjectileBase projectile, Vector3 position, Quaternion rotation)
        {
            var available = (int)projectile.ammunition.available;

            if (projectileRate <= 1) return SingleShot(projectile, position, rotation);

            if (fireDirection == FirePatternType.WeaponDirection)
                return Multiple(projectile, available, position, rotation);

            return Circular(projectile, available, position, rotation);
        }

        public bool SingleShot(ProjectileBase projectile, Vector3 position, Quaternion rotation)
        {
            var newPosition = variance > 0 ? position + rotation * Vector2.up * randomize : position;
            if (projectile.Fire(newPosition, rotation)) return true;
            return false;
        }

        public bool Multiple(ProjectileBase projectile, int available, Vector2 position, Quaternion rotation)
        {
            var success = false;
            var multiplier = (available - 1) / 2f;
            Vector2 up = rotation * Vector2.up;
            Vector2 right = rotation * Vector2.right;
            var startOffset = up * separation.y * multiplier;
            var startAngle = angle * multiplier;
            var offsetX = Mathf.Abs(separation.x);

            for (var i = 0; i < available; i++)
            {
                var rotateAngle = Quaternion.AngleAxis(startAngle - angle * i, Vector3.forward);
                var newPosition = variance > 0 ? position + up * randomize : position;
                newPosition += startOffset - up * separation.y * i;
                var separateX = separation.x < 0
                    ? Mathf.Abs(multiplier - i)
                    : Mathf.Abs(Mathf.Abs(multiplier - i) - multiplier);
                newPosition += right * offsetX * separateX;
                if (projectile.Fire(newPosition, rotation * rotateAngle)) success = true;
            }

            return success;
        }

        public bool Circular(ProjectileBase projectile, int available, Vector3 position, Quaternion rotation)
        {
            var success = false;
            var startAngle = 360f / available;

            for (var i = 0; i < available; i++)
            {
                var angle = startAngle * i;
                var rotateAngle = Quaternion.AngleAxis(angle, Vector3.forward);
                if (projectile.Fire(position, rotation * rotateAngle)) success = true;
            }

            return success;
        }
    }
}