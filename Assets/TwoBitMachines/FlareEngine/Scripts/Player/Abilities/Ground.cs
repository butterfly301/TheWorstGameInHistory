#region

using System;
using TwoBitMachines.FlareEngine.Interactables;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.ThePlayer
{
    [AddComponentMenu("")]
    public class Ground : Ability
    {
        [NonSerialized] private Friction groundRef;
        [NonSerialized] private Transform groundTransform;
        [NonSerialized] private bool jumping;
        [NonSerialized] private float sliding;
        [NonSerialized] private float speed = 1f;

        private bool isAuto => groundRef != null && groundRef.type == FrictionType.Auto;
        private bool isSlide => groundRef != null && groundRef.type == FrictionType.Slide;
        private bool isFriction => groundRef != null && groundRef.type == FrictionType.Friction;

        public override void Reset(AbilityManager player)
        {
            jumping = false;
        }

        public override bool TurnOffAbility(AbilityManager player)
        {
            jumping = false;
            return true;
        }

        public override void
            EarlyExecute(AbilityManager player,
                ref Vector2 velocity) // since using EarlyExecute, this will always execute. No need for priority.
        {
            if (jumping)
            {
                KeepMomentum(player, ref velocity);
                if (jumping)
                    return;
            }

            if (pause) return;
            if (!player.ground && (isFriction || velocity.y > 0))
            {
                if (isSlide || isAuto)
                {
                    jumping = true;
                    KeepMomentum(player, ref velocity);
                }

                groundRef = null;
                groundTransform = null;
                return;
            }

            if (player.world.verticalTransform != groundTransform && player.world.verticalTransform != null &&
                player.world.verticalTransform.CompareTag("Friction"))
            {
                if (groundRef == null)
                    sliding = velocity.x == 0 ? player.playerDirection * 2f : velocity.x;
                groundTransform = player.world.verticalTransform;
                groundRef = groundTransform.GetComponent<Friction>();
            }

            if (groundRef == null || groundTransform == null) return;
            if (player.world.verticalTransform != groundTransform && player.world.verticalTransform != null &&
                !player.world.verticalTransform.CompareTag("Friction"))
            {
                groundRef = null;
                groundTransform = null;
                return; // not on same ground anymore, exit out
            }

            //friction
            if (groundRef.type == FrictionType.Friction)
            {
                velocity.x *= groundRef.friction;
                player.signals.Set("friction");
                return;
            }

            //slide
            if (groundRef.type == FrictionType.Slide)
            {
                player.signals.Set("sliding", velocity.x == 0);
                speed = groundRef.slideSpeed;
                if (velocity.x == 0)
                {
                    sliding = Mathf.MoveTowards(sliding, 0, speed * Time.deltaTime);
                    velocity.x = sliding;
                }
                else
                {
                    sliding = Mathf.MoveTowards(sliding, velocity.x,
                        speed * Time.deltaTime * 4f); // changing direction, allow for faster catch up speed.
                    velocity.x = sliding;
                }

                return;
            }

            //auto
            if (groundRef.type == FrictionType.Auto)
            {
                player.signals.Set("autoGround", velocity.x == 0);
                velocity.x += groundRef.autoSpeed;
                sliding = speed = groundRef.autoSpeed;
            }
        }

        private void KeepMomentum(AbilityManager player, ref Vector2 velocity)
        {
            if (player.world.touchingASurface) jumping = false;
            sliding = Mathf.MoveTowards(sliding, 0, speed * Time.deltaTime);
            var sameDirection = Compute.SameSign(velocity.x, sliding) || velocity.x == 0;
            velocity.x = sameDirection ? sliding : velocity.x + sliding;

            if (!sameDirection) sliding = Mathf.MoveTowards(sliding, 0, speed * Time.deltaTime * 4f);
        }

        #region ▀▄▀▄▀▄ Custom Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(SerializedObject controller, SerializedObject parent, string[] inputList,
            Color barColor, Color labelColor)
        {
            if (Open(parent, "Ground", barColor, labelColor)) Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}