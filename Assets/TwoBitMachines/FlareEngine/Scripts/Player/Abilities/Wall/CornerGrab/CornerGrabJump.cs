using System;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.ThePlayer
{
    [Serializable]
    public class CornerGrabJump
    {
        [NonSerialized] private Vector2 jumpToVelocity;

        [NonSerialized] private int wallDirRef;
        public bool isJumping { get; private set; }

        public void Reset()
        {
            isJumping = false;
        }

        public void StartJump(Wall wall, AbilityManager player, int wallDirection, float topCornerY,
            ref Vector2 velocity)
        {
            var offsetX = player.world.box.sizeX * wallDirection;
            var jumpTo = new Vector2(player.world.position.x + offsetX, topCornerY);
            velocity = Compute.ArchObject(player.world.box.bottomCenter, jumpTo, 0.25f, player.gravity.gravity);
            wallDirRef = wallDirection;
            jumpToVelocity = velocity;
            isJumping = true;
            player.world.mp.Follow();
        }

        public void AutoJumpToCorner(AbilityManager player, ref Vector2 velocity)
        {
            if (!isJumping) return;

            if (player.ground || Wall.CheckForGround(player.world.box, jumpToVelocity.x, velocity.y) ||
                (player.inputX != 0 && !Compute.SameSign(player.inputX, wallDirRef))) isJumping = false;
            velocity.x = isJumping ? jumpToVelocity.x : velocity.x;
        }
    }
}