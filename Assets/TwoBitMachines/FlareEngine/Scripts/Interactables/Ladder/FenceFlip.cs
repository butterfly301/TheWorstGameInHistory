using System;
using TwoBitMachines.FlareEngine.ThePlayer;
using TwoBitMachines.TwoBitSprite;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Interactables
{
    [Serializable]
    public class FenceFlip // animation signals: fenceFlip, fenceFlipReverse
    {
        [SerializeField] public bool canFlip;
        [SerializeField] public float flipTime = 2f;
        [SerializeField] public SpriteEngine spriteEngine;

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] private bool fenceFoldOut;
#pragma warning restore 0414
#endif

        #endregion

        [NonSerialized] private float direction;
        [NonSerialized] private bool flipReverse;
        [NonSerialized] private float halfCounter;
        [NonSerialized] private bool isFlipping;
        [NonSerialized] private float radius;
        [NonSerialized] private Vector2 startPoint;

        [NonSerialized] private State state;

        public void Reset()
        {
            halfCounter = 0;
            isFlipping = false;
            state = State.FirstHalf;
        }

        public bool Flip(LadderInstance ladder, LadderClimb ladderClimb, AbilityManager player, string flipButton,
            ref Vector2 velocity)
        {
            if (!canFlip || spriteEngine == null) return false;

            if (!isFlipping && player.inputs.Pressed(flipButton))
            {
                flipReverse = ladderClimb.hasFlipped;
                startPoint = player.world.transform.position;
                var distance = ladder.CenterX() - startPoint.x;
                direction = Mathf.Sign(distance);
                radius = Mathf.Abs(distance);
                state = State.FirstHalf;
                isFlipping = true;
            }

            if (isFlipping)
            {
                velocity = Vector2.zero;
                SetSignals(player);

                if (state == State.FirstHalf && Switch(ladder, player, 0))
                {
                    state = State.SecondHalf;
                    ladderClimb.hasFlipped = !ladderClimb.hasFlipped;
                    startPoint = player.world.transform.position;
                    ladderClimb.fenceReverse.SetLayerOrder(player, ladder, ladderClimb.hasFlipped);
                }

                if (state == State.SecondHalf && Switch(ladder, player, direction * radius)) isFlipping = false;
            }

            spriteEngine.SetSignal("idle");
            return isFlipping;
        }

        private bool Switch(LadderInstance ladder, AbilityManager player, float offset)
        {
            var position = player.world.transform.position;
            var complete = Clock.Timer(ref halfCounter, flipTime * 0.5f);
            var target = ladder.CenterX() + offset;
            var percent = halfCounter / (flipTime * 0.5f);
            position.x = complete ? position.x : Mathf.Lerp(startPoint.x, target, percent);
            player.world.transform.position = position;
            return complete;
        }

        private void SetSignals(AbilityManager player)
        {
            player.signals.Set("fenceFlipping");
            spriteEngine.SetSignal("fenceFlipping");

            player.signals.Set("fenceFlip", !flipReverse);
            player.signals.Set("fenceFlipReverse", flipReverse);
            if (flipReverse)
                spriteEngine.SetSignal("fenceFlipReverse");
            else
                spriteEngine.SetSignal("fenceFlip");
        }

        private enum State
        {
            FirstHalf,
            SecondHalf
        }
    }
}