using System;
using TwoBitMachines.FlareEngine.Interactables;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.ThePlayer
{
    public class RopeInteraction
    {
        [NonSerialized] private Particle[] particle;
        [NonSerialized] private Rope rope;
        [NonSerialized] private RopeSwing ropeSwing;
        [NonSerialized] private float tetherRadius;

        public void Set(RopeSwing ropeSwing, Rope rope)
        {
            this.rope = rope;
            this.ropeSwing = ropeSwing;
            particle = rope.particle;
            tetherRadius = rope.tetherRadius;
        }

        public Vector2 RopeHoldPoint(WorldCollision world, Vector2 center, Vector2 climbVelocity, out bool climbing)
        {
            climbing = false;
            if (ropeSwing.particle1 < particle.Length && ropeSwing.particle2 < particle.Length)
            {
                // current holdPoint
                var direction = (particle[ropeSwing.particle2].position - particle[ropeSwing.particle1].position)
                    .normalized;
                var holdPoint = particle[ropeSwing.particle1].position + direction * ropeSwing.grabDistance;

                if (climbVelocity.y == 0) return holdPoint;

                // next hold point
                var movePoint = holdPoint + world.box.up * climbVelocity.y;
                if (Rope.HoldPoint(ropeSwing, particle, movePoint, world.box.right, tetherRadius, ref holdPoint))
                    climbing = true;
                return holdPoint;
            }

            return center;
        }

        public Vector2 RopeDirection(Vector2 center)
        {
            if (particle.Length <= 1) return Vector2.up;
            if (particle.Length == 2) return (particle[0].position - particle[1].position).normalized;

            var index = -1;
            var distance = Mathf.Infinity;

            for (var i = 1; i < particle.Length; i++)
            {
                var sqrDist = (center - particle[i].position).sqrMagnitude;
                if (sqrDist < distance)
                {
                    distance = sqrDist;
                    index = i;
                }
            }

            if (index >= 0)
            {
                var firstPoint = particle[index].position;
                var secondPoint = particle[index - 1].position;
                return (secondPoint - firstPoint).normalized;
            }

            return Vector2.up;
        }

        public void RotatePlayerToRope(WorldCollision world, Vector2 center, Vector2 direction, float rate)
        {
            var angle = Vector2.Angle(world.box.up, direction);
            var maxAngle = Mathf.Clamp(angle * Time.deltaTime * rate * 10f, 0, angle);
            maxAngle = angle > 0 && angle < 1.5f ? angle : maxAngle; //                                             
            world.transform.RotateAround(center, Vector3.forward, maxAngle * world.box.up.CrossSign(direction));
            world.box.Update();
        }
    }
}