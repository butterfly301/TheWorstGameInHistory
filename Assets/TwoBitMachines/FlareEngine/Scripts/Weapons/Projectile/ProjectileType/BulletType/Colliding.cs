using UnityEngine;

namespace TwoBitMachines.FlareEngine.BulletType
{
    [AddComponentMenu("")]
    public class Colliding : BulletBase
    {
        [SerializeField] public bool addMomentum;
        [SerializeField] public bool expireOnImpact = true;

        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (IgnoreEdges(collider)) return;
            if (collider.gameObject.layer == gameObject.layer) return;
            var direction = velocity.normalized;
            if (Health.IncrementHealth(transform, collider.transform, -damage, direction * damageForce)) // deal damage
            {
                var impact = ImpactPacket.impact.Set(worldEffect, position, direction);
                onHitTarget.Invoke(impact);
                if (expireOnImpact) SleepOnImpact(position, direction);
            }
        }

        public override void OnReset(Vector2 characterVelocity)
        {
            AddMomentum(addMomentum, characterVelocity);
        }

        public override void Execute()
        {
            if (SetToSleep()) return;
            LifeSpanTimer();
            ApplyGravity();

            if (gravity != 0) ApplyRotation(transform);
            position += velocity * Time.deltaTime;
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}