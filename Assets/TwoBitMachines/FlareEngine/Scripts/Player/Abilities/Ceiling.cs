#region

using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.ThePlayer
{
    [AddComponentMenu("")]
    public class Ceiling : Ability
    {
        [SerializeField] public float friction = 1f;
        [SerializeField] public bool autoGrip;
        [SerializeField] public bool edgeJump;
        [SerializeField] public float jumpScale = 1f;
        [SerializeField] public float exitPoint;
        [SerializeField] public bool latched;

        public override bool IsAbilityRequired(AbilityManager player, ref Vector2 velocity)
        {
            if (pause)
                return false;

            return player.world.onCeiling && CanClimbCeiling(player) && (autoGrip || player.jumpButtonHold);
        }

        public override void ExecuteAbility(AbilityManager player, ref Vector2 velocity,
            bool isRunningAsException = false)
        {
            if (player.world.missedAVertical)
            {
                var box = player.world.box;
                var cornerLeft = Physics2D.Raycast(box.cornerTopLeft, box.up, 0.1f, box.world.collisionLayer);
                var dirX = cornerLeft ? 1 : -1f;
                var hit = Physics2D.Raycast(box.topCenter + box.right * dirX * exitPoint, box.up, 0.1f,
                    box.world.collisionLayer);
                if (!hit) return; // exit ceiling climb
            }

            velocity.y = -player.gravityEffect + 1f;
            velocity.x *= friction;
            player.signals.Set("ceilingClimb");

            if (player.jumpButtonPressed)
            {
                if (edgeJump)
                {
                    var collider = player.world.verticalTransform.GetComponent<Collider2D>();
                    if (collider is EdgeCollider2D)
                    {
                        velocity.y = player.maxJumpVel * jumpScale;
                        player.CheckForAirJumps(1);
                        player.signals.Set("ceilingClimb", false);
                        JumpOff(player, ref velocity);
                        return;
                    }
                }

                velocity.y = player.gravityEffect;
                player.signals.Set("ceilingClimb", false);
                JumpOff(player, ref velocity);
            }
        }

        private void JumpOff(AbilityManager player, ref Vector2 velocity)
        {
            if (player.world.mp.velocity.y > 0)
            {
                var tempVelocityB =
                    Vector2.zero; // Input a fake velocity since we don't want the y value to jump up with a moving platform if on ceiling
                player.world.mp.Launch(ref tempVelocityB, ref velocity.y);
            }
            else
            {
                player.world.mp.Launch(ref velocity, ref velocity.y);
            }
        }

        private bool CanClimbCeiling(AbilityManager player)
        {
            return player.world.verticalTransform == null || !player.world.verticalTransform.CompareTag("NoClimb");
        }

        public override void PostCollisionExecute(AbilityManager player, Vector2 velocity)
        {
            if (!pause && player.world.useMovingPlatform && player.world.onCeiling)
                MovingPlatform.LatchToPlatform(player.world, player.world.verticalTransform, LatchMPType.Ceiling);
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(SerializedObject controller, SerializedObject parent, string[] inputList,
            Color barColor, Color labelColor)
        {
            if (Open(parent, "Ceiling", barColor, labelColor))
            {
                FoldOut.Box(4, FoldOut.boxColorLight, offsetY: -2);
                {
                    parent.Slider("Friction", "friction");
                    parent.FieldToggle("Auto Grip", "autoGrip");
                    parent.FieldAndEnable("Edge Jump", "jumpScale", "edgeJump");
                    Labels.FieldText("Jump Scale", 15);
                    parent.Field("Exit Point", "exitPoint");
                }
                Layout.VerticalSpacing(3);
            }

            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}