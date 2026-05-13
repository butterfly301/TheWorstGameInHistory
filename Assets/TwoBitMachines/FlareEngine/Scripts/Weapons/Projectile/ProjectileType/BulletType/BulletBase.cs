using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.BulletType
{
    [AddComponentMenu("")]
    public class BulletBase : MonoBehaviour
    {
        [SerializeField] public LayerMask layer;
        [SerializeField] public IgnoreEdge ignoreEdges;
        [SerializeField] public float lifeSpan = 5f;
        [SerializeField] public float speed = 10f;
        [SerializeField] public float gravity;
        [SerializeField] public float blastRadius;
        [SerializeField] public string worldEffect = "";
        [SerializeField] public bool invertY = true;

        [SerializeField] public UnityEventEffect onFire;
        [SerializeField] public UnityEventEffect onImpact;
        [SerializeField] public UnityEventEffect onHitTarget;
        [SerializeField] public UnityEventEffect onLifeSpanExpired;
        [NonSerialized] public float counter;
        [NonSerialized] public float damage;

        [NonSerialized] public float damageForce;
        [NonSerialized] public Vector2 direction;
        [NonSerialized] public Vector2 position;
        [NonSerialized] public bool readyToSleep;
        [NonSerialized] public Quaternion rotation;
        [NonSerialized] public bool sleep;
        [NonSerialized] public Vector2 velocity;

        public void Reset(Vector3 newPosition, Quaternion newRotation, float newDamage, float newDamageForce)
        {
            counter = 0;
            sleep = false;
            readyToSleep = false;
            damage = newDamage;
            damageForce = newDamageForce;
            position = newPosition;
            rotation = newRotation;
            direction = rotation * Vector2.right;
            velocity = direction * speed;
        }

        public virtual void Execute()
        {
        }

        public virtual void OnReset(Vector2 characterVelocity)
        {
        }

        public void SetGameObjectTrue()
        {
            gameObject.SetActive(true);
        }

        public void ReadyToSleep()
        {
            readyToSleep = true;
        }

        public bool SetToSleep()
        {
            if (readyToSleep)
            {
                sleep = true;
                gameObject.SetActive(false);
            }

            return sleep;
        }

        public void LifeSpanTimer()
        {
            if (Clock.Timer(ref counter, lifeSpan))
            {
                BlastRadius();
                ReadyToSleep();
                var impact = ImpactPacket.impact.Set(worldEffect, position, direction);
                onLifeSpanExpired.Invoke(impact);
            }
        }

        public void AddMomentum(bool addMomentum, Vector2 characterVelocity)
        {
            if (addMomentum)
            {
                var vX = Compute.SameSign(velocity.x, characterVelocity.x)
                    ? characterVelocity.x
                    : 0; // Only add character velocity if pointing in the same direction
                var vY = Compute.SameSign(velocity.y, characterVelocity.y) ? characterVelocity.y : 0;
                var momentumVel = new Vector2(vX, vY);
                velocity += momentumVel;
                position -= momentumVel *
                            Time.deltaTime; //  subtract momentum velocity so that spawn position stays in the same spot
            }
        }

        public void ApplyGravity(float scale = 1f)
        {
            if (gravity != 0) velocity.y -= gravity * Time.deltaTime * scale;
        }

        public void ApplyRotation(Transform transform)
        {
            if (velocity != Vector2.zero)
            {
                rotation = Quaternion.AngleAxis(Compute.Atan2(velocity),
                    Vector3.forward); //  Make sure sprite is perfectly symmetrical on the x-axis, so when it flips, it doesn't look like it shifted.
                var size = transform.localScale;
                if (invertY)
                    transform.localScale =
                        new Vector3(size.x, Mathf.Abs(size.y) * Mathf.Sign(velocity.x),
                            size.z); // flip y for sprite to face the correct direction in the y axis
            }
        }

        public bool DealDamage(Transform hitTransform, Vector2 hitPoint, Vector2 direction, bool sleep = true)
        {
            if (!Health.IncrementHealth(transform, hitTransform, -damage, direction * damageForce))
                if (Health.IsDamageable(hitTransform))
                    return false;

            var impact = ImpactPacket.impact.Set(worldEffect, hitPoint, direction);
            onHitTarget.Invoke(impact);
            if (sleep) SleepOnImpact(hitPoint, direction);
            return true;
        }

        public void SleepOnImpact(Vector2 position, Vector2 direction)
        {
            var impact = ImpactPacket.impact.Set(worldEffect, position, direction);
            onImpact.Invoke(impact);
            BlastRadius();
            ReadyToSleep();
        }

        public bool IgnoreEdges(Collider2D collider)
        {
            if (ignoreEdges != IgnoreEdge.NeverIgnore && collider is EdgeCollider2D)
                if (ignoreEdges == IgnoreEdge.IgnoreAlways || velocity.y > 0)
                    return true;

            return false;
        }

        public void BlastRadius()
        {
            if (blastRadius != 0)
            {
                var hit = Compute.OverlapCircle(position, blastRadius, layer);
                Health.HitContactResults(transform, Compute.contactResults, hit, -damage, damageForce, position);
            }
        }

        public void CollisionDetection(int bulletRays, Vector2 bulletSize)
        {
            var vel = velocity * Time.deltaTime;
            var velMagnitude = vel.magnitude;
            var velocityNormal = velMagnitude == 0 ? velocity.normalized : vel / velMagnitude;

            if (bulletRays > 1)
            {
                var stepHeight = bulletSize.y / (bulletRays - 1);
                var step = Vector2.up * stepHeight;
                var topCorner = position + (Vector2)(rotation * Vector2.up * bulletSize.y * 0.5f);

                for (var i = 0; i < bulletRays; i++)
                    if (CastRay(topCorner - step * i, velocityNormal, velMagnitude, bulletSize.x))
                        break;
            }
            else
            {
                CastRay(position, velocityNormal, velMagnitude, bulletSize.x);
            }

            position += velocity * Time.deltaTime;
        }

        public virtual bool CastRay(Vector2 origin, Vector2 velocityNormal, float velMagnitude, float size)
        {
            var ray = Physics2D.Raycast(origin, velocityNormal, velMagnitude + size, layer);

            #region Debug

#if UNITY_EDITOR
            if (WorldManager.viewDebugger) Debug.DrawRay(origin, velocityNormal * (velMagnitude + size), Color.red);
#endif

            #endregion

            if (!ray || IgnoreEdges(ray.collider)) return false;
            if (!DealDamage(ray.transform, ray.point, velocityNormal)) return false;
            if (ray.distance != 0)
                velocity = Time.deltaTime <= 0 ? Vector2.zero : velocityNormal * (ray.distance - size) / Time.deltaTime;
            return true;
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public string nameType;
        [SerializeField] [HideInInspector] private bool foldOut = true;
        [SerializeField] [HideInInspector] private bool patternFoldOut = true;
        [SerializeField] [HideInInspector] private bool foldOutEvents;
        [SerializeField] [HideInInspector] private bool foldOutOnFire;
        [SerializeField] [HideInInspector] private bool foldOutOnRelease;
        [SerializeField] [HideInInspector] private List<bool> foldOuts = new();
#pragma warning restore 0414
#endif

        #endregion
    }
}