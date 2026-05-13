using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.ThePlayer
{
    public class DashPlay
    {
        [NonSerialized] public bool clearYNextFrame;
        [NonSerialized] private Dash dash;
        [NonSerialized] private float dashRateCounterCounter;
        [NonSerialized] public Vector2 direction;
        [NonSerialized] public int directionX;
        [NonSerialized] private Vector2 distance;
        [NonSerialized] private ContactFilter2D filter2D;
        [NonSerialized] private Health health;
        [NonSerialized] public List<Transform> hit = new();
        [NonSerialized] private float incCounter;

        public void Initialize(Dash dashRef, Transform transform)
        {
            dash = dashRef;
            health = transform.GetComponent<Health>();
        }

        public void Reset()
        {
            clearYNextFrame = false;
            incCounter = 0;
            dashRateCounterCounter = 10000f;
        }

        public void ResetDamage()
        {
            if (dash != null && !dash.canTakeDamage && health != null) health.CanTakeDamage(true);
        }

        public void GetPlayerDirectionX(AbilityManager player, float velX)
        {
            directionX = player.playerDirection;
            if (dash.dashInPlace && velX == 0)
                directionX = 0;
            else if (dash.changeDirectionOnWall && player.world.onWall)
                directionX = player.world.leftWall ? 1 : -1;
            else if (velX != 0) directionX = (int)Mathf.Sign(velX);
        }

        public void SetDashDirection(AbilityManager player)
        {
            if (dash.directionType == DashDirection.HorizontalAxis)
            {
                direction.x = directionX;
                distance.x = dash.dashDistance;
                distance.y = direction.y = 0;
            }
            else if (dash.directionType == DashDirection.MultiDirectional)
            {
                var x = (player.inputs.Holding(dash.left) ? -1 : 0) + (player.inputs.Holding(dash.right) ? 1 : 0);
                var y = (player.inputs.Holding(dash.up) ? 1 : 0) + (player.inputs.Holding(dash.down) ? -1 : 0);
                x = x != 0 && player.world.onWall ? directionX : x;
                x = x == 0 && y == 0 ? directionX : x; // Force x to have a value if none
                var bothActive = x != 0 && y != 0;

                direction.x = player.world.box.right.x * x;
                direction.y = player.world.box.up.y * y;
                distance.x = bothActive ? Mathf.Cos(45f * Mathf.Deg2Rad) * dash.dashDistance :
                    x != 0 ? dash.dashDistance : 0;
                distance.y = bothActive ? Mathf.Cos(45f * Mathf.Deg2Rad) * dash.dashDistance :
                    y != 0 ? dash.dashDistance : 0;
            }
            else
            {
                Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                direction = (worldMousePosition - player.world.box.topCenter).normalized;
                distance.x = dash.dashDistance;
                distance.y = dash.dashDistance;
            }

            if (dash.crouch) player.world.box.ChangeColliderHeight(dash.crouchHeight);
            if (!dash.canTakeDamage && health != null) health.CanTakeDamage(false);
        }

        public void Execute(AbilityManager player, ref Vector2 velocity, ref bool isDashing)
        {
            isDashing = false;
            if (dash.type == DashType.Instant && !dash.dontDash)
            {
                Dash(player, ref velocity, Time.deltaTime, dash.directionType == DashDirection.HorizontalAxis);
            }
            else if (dash.time != 0 && !dash.dontDash && Clock.TimerInverse(ref incCounter, dash.time))
            {
                isDashing = true;
                Dash(player, ref velocity, dash.time, true);
            }

            if (isDashing && player.world.onWall && direction.y == 0)
            {
                if (direction.x < 0 && player.world.leftWall)
                    isDashing = false;
                if (direction.x > 0 && player.world.rightWall)
                    isDashing = false;
            }

            if (isDashing && player.world.onCeiling && direction.x == 0 && direction.y > 0) isDashing = false;
            if (dash.exitOnContact && isDashing)
            {
                if (player.world.onWall)
                {
                    isDashing = false;
                }
                else
                {
                    var box = player.world.boxCollider;
                    var bounds = box.bounds;
                    var hit = Physics2D.BoxCast(bounds.center, bounds.size, box.transform.eulerAngles.z, direction,
                        dash.dashDistance * Time.deltaTime, dash.exitLayer);
                    if (hit)
                        isDashing = false;
                }
            }

            if (!isDashing && dash.crouch && player.world.boxCollider.size.y != player.world.box.boxSize.y)
            {
                if (SafelyStandUp(player.world.box))
                {
                    player.world.box.ColliderReset();
                }
                else
                {
                    dash.dontDash = true;
                    isDashing = true;
                    player.signals.Set("crouch");
                }
            }

            if (!isDashing)
            {
                ResetDamage();
                dash.OnEndEvent(player);
                player.dashBoost = 1f;
            }
        }

        private void Dash(AbilityManager player, ref Vector2 velocity, float time, bool checkNullGravity)
        {
            velocity.x = distance.x != 0 ? distance.x / time * direction.x * player.dashBoost : velocity.x;
            velocity.y = distance.y != 0 ? distance.y / time * direction.y : velocity.y;

            player.signals.Set("dashing");
            player.signals.Set("dashX", distance.x != 0);
            player.signals.Set("dashY", distance.y != 0);
            player.signals.Set("dashDiagonal", distance.x != 0 && distance.y != 0);
            DealDamage(player);

            if (direction.x != 0)
            {
                var dirX = (int)Mathf.Sign(direction.x);
                player.signals.SetDirection(dirX);
                player.playerDirection = dirX;
            }

            if (checkNullGravity && dash.nullifyGravity && distance.y == 0 && !player.world.onGround && velocity.y < 0)
                velocity.y = 0;
            if (distance.y != 0) clearYNextFrame = true;
            if (dash.onDashRate > 0 && Clock.Timer(ref dashRateCounterCounter, dash.onDashRate))
            {
                var impact = ImpactPacket.impact.Set(dash.worldEffect, dash.transform, player.world.boxCollider,
                    dash.transform.position, null, direction, player.playerDirection, 0);
                dash.onDash.Invoke(impact);
            }
        }

        public void
            DealDamage(AbilityManager player) // since using EarlyExecute, this will ALWAYS execute. No need for priority.
        {
            if (!dash.canDealDamage)
                return;
            filter2D.useLayerMask = true;
            filter2D.useTriggers = true;
            filter2D.layerMask = dash.damageLayer;

            var length = Physics2D.OverlapCollider(player.world.boxCollider, filter2D, WorldCollision.colliderResults);

            for (var i = 0; i < length; i++)
            {
                var transform = WorldCollision.colliderResults[i].transform;
                if (hit.Contains(transform))
                    continue;
                Health.IncrementHealth(player.world.transform, transform, -dash.damage, direction);
                hit.Add(transform);
            }
        }

        public bool SafelyStandUp(BoxInfo ray)
        {
            var length = Mathf.Abs(ray.boxSize.y - ray.collider.size.y) * ray.collider.transform.localScale.y;
            for (var i = 0; i < ray.rays.y; i++)
            {
                var origin = ray.cornerTopLeft + ray.right * (ray.spacing.x * i);

                #region Debug

#if UNITY_EDITOR
                if (WorldManager.viewDebugger) Debug.DrawRay(origin, ray.up * length, Color.white);
#endif

                #endregion

                if (Physics2D.Raycast(origin, ray.up, length, WorldManager.collisionMask)) return false;
            }

            return true;
        }
    }
}