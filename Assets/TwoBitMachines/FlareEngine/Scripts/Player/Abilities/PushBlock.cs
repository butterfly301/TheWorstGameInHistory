#region

using System;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.ThePlayer
{
    [AddComponentMenu("")]
    public class PushBlock : Ability
    {
        public enum PushType
        {
            Automatic,
            Button
        }

        [SerializeField] public PushType type;
        [SerializeField] public string pushButton = "";
        [SerializeField] public float pushSpeed = 0.5f;
        [SerializeField] public float pullSpeed = 0.5f;
        [SerializeField] public bool exitOnJump;

        [NonSerialized] private bool active;
        [NonSerialized] private Character block;

        private bool automatic => type == PushType.Automatic;

        public override void Reset(AbilityManager player)
        {
            active = false;
        }

        public override bool TurnOffAbility(AbilityManager player)
        {
            Reset(player);
            return true;
        }

        public override bool IsAbilityRequired(AbilityManager player, ref Vector2 velocity)
        {
            if (pause)
                return false;

            if (active && (block == null || (exitOnJump && player.tryingToJump))) active = false;
            if (active && !automatic &&
                !player.inputs.Holding(pushButton)) active = false; // user no longer holding button, stop pushing
            if (active && !player.world.onWall &&
                (automatic || !player.inputs.Holding(pushButton))) active = false; // player not on wall anymore
            if (active && player.world.onWall && !player.world.wasOnSlopeUp &&
                player.world.wallTransform !=
                block.transform) active = false; // colliding transform doesn't match block transform
            if (active && !player.world.onWall && velocity.x != 0 && OppositeWall(velocity, player.world.box))
                active = false; // when pulling, don't get sandwiched between two surfaces
            if (active) return true;
            if (player.world.onGround && player.world.onWall && !(exitOnJump && player.tryingToJump) &&
                player.world.wallTransform.CompareTag("Block"))
                if (automatic || player.inputs.Holding(pushButton))
                {
                    block = player.world.wallTransform.GetComponent<Character>();
                    return block == null ? false : active = true;
                }

            return false;
        }

        public override void ExecuteAbility(AbilityManager player, ref Vector2 velocity,
            bool isRunningAsException = false)
        {
            if (block == null)
            {
                active = false;
                return;
            }

            if (!player.world.onWall)
            {
                velocity.x *= pullSpeed;
                block.externalVelocity.x = velocity.x;
                block.Execute();
                player.signals.Set("pullBlock", velocity.x != 0);
            }
            else
            {
                block.externalVelocity.x = velocity.x * pushSpeed;
                block.Execute(); //             push AI transform, the player gets executed before the AI, so we need to syncTransforms
                Physics2D.SyncTransforms(); //  Sync AI collider and transform for proper collision. Monitor performance
                player.signals.Set("pushBlock", velocity.x != 0);
            }
        }

        private bool
            OppositeWall(Vector2 velocity,
                BoxInfo box) // if pulling, must check for walls in pull direction, or player might get stuck
        {
            var signX = Mathf.Sign(velocity.x);
            var magnitude = Mathf.Abs(velocity.x * Time.deltaTime) * 1.75f + box.skin.x * 2f;
            var corner = signX > 0 ? box.bottomRight - box.skinX : box.bottomLeft + box.skinX;

            for (var i = 0; i < box.rays.x; i++)
            {
                var origin = corner + box.up * box.spacing.y * i;
                var hit = Physics2D.Raycast(origin, box.right * signX, magnitude, box.world.collisionLayer);
                if (hit)
                {
                    if (i == 0 && box.world.climbSlopes && hit.distance > 0 &&
                        Compute.Between(Vector2.Angle(hit.normal, box.up), 0, box.world.maxSlopeAngle)) continue;
                    return true;
                }
            }

            return false;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(SerializedObject controller, SerializedObject parent, string[] inputList,
            Color barColor, Color labelColor)
        {
            if (Open(parent, "Push Block", barColor, labelColor))
            {
                var type = parent.Enum("type");
                FoldOut.Box(4, FoldOut.boxColorLight, offsetY: -2);
                {
                    parent.DropDownListAndField(inputList, "Button", "pushButton", "type", type == 1);
                    parent.Field("Button", "type", type == 0);
                    parent.Field("Push Speed", "pushSpeed");
                    parent.Field("Pull Speed", "pullSpeed");
                    parent.FieldToggle("Exit On Jump", "exitOnJump");
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