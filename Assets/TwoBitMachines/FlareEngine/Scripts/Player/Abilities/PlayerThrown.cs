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
    public class PlayerThrown : Ability
    {
        [SerializeField] public float distanceX = 2;
        [SerializeField] public float jumpForce = 5;
        [SerializeField] public float throwTime = 0.25f;
        [NonSerialized] private float acceleration;

        [NonSerialized] private Vector2 appliedForce;
        [NonSerialized] private bool beginThrow;
        [NonSerialized] private Character character;
        [NonSerialized] private float counter;
        [NonSerialized] private Vector2 direction;
        [NonSerialized] private int throwDirection;

        [NonSerialized] private bool throwing;

        public override void Reset(AbilityManager player)
        {
            ResetPush(player);
        }

        public void ResetPush(AbilityManager player)
        {
            player.character.pushBackActive = false;
            player.pushBackActive = false;
            beginThrow = false;
            throwing = false;
            counter = 0;
        }

        public override bool TurnOffAbility(AbilityManager player)
        {
            Reset(player);
            return true;
        }

        public override bool IsAbilityRequired(AbilityManager player, ref Vector2 velocity)
        {
            if (beginThrow)
            {
                beginThrow = false;
                throwDirection = player.playerDirection;
            }

            character = player.character;
            player.pushBackActive = player.character.pushBackActive = throwing;
            return !pause && throwing;
        }

        public void ActivateThrow()
        {
            if (character == null)
                return;
            counter = 0;
            throwing = true;
            beginThrow = true;
            direction = Vector2.right * character.signals.characterDirection;
            direction.y = 1f;
            acceleration = -2f * distanceX / Mathf.Pow(throwTime, 2f);
            appliedForce = acceleration * direction * throwTime;
        }

        public override void ExecuteAbility(AbilityManager player, ref Vector2 velocity,
            bool isRunningAsException = false)
        {
            if (counter == 0 &&
                Time.deltaTime !=
                0) //                                                   distance x is pushed linearly, vel y is treated as a jump
            {
                velocity.y = jumpForce * direction.y;
                player.hasJumped = velocity.y > 0 ? true : player.hasJumped;
            }

            if (Clock.Timer(ref counter, throwTime))
            {
                if (!Compute.SameSign(throwDirection, -appliedForce.x))
                    velocity.x =
                        0.001f * Mathf.Sign(appliedForce
                            .x); //  when push back is complete, make sure player velocity and pushBackDirection align, or else sprite will flip
                player.signals.characterDirection = (int)Mathf.Sign(velocity.x);
                player.signals.oldCharacterDirection = player.signals.characterDirection;
                player.playerDirection = (int)Mathf.Sign(velocity.x);
                ResetPush(player);
                return;
            }

            appliedForce -= acceleration * direction * Time.deltaTime;
            velocity.x = -appliedForce.x;
            player.signals.Set("thrown");
            player.signals.Set("thrownLeft", throwDirection < 0);
            player.signals.Set("thrownRight", throwDirection > 0);
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(SerializedObject controller, SerializedObject parent, string[] inputList,
            Color barColor, Color labelColor)
        {
            if (Open(parent, "Player Thrown", barColor, labelColor))
            {
                FoldOut.Box(3, FoldOut.boxColorLight, offsetY: -2);
                parent.Field("Distance", "distanceX");
                parent.Field("Jump Force", "jumpForce");
                Labels.FieldDoubleText("Default", "", rightSpacing: 4);
                parent.Field("Throw Time", "throwTime");
                Layout.VerticalSpacing(3);
            }

            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}