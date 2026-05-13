using System;
using System.Collections.Generic;

namespace TwoBitMachines.TwoBitSprite
{
    [Serializable]
    public class StateSignals
    {
        public SignalPacket[] all =
        {
            new("alwaysTrue"),
            new("alwaysFalse"),
            new("airGlide"),
            new("airJump"),
            new("autoGround"),
            new("autoCornerJump"),
            new("cannonBlast"),
            new("ceilingClimb"),
            new("changedDirection"),
            new("crouch"),
            new("crouchSlide"),
            new("crouchWalk"),
            new("dashing"),
            new("dashDiagonal"),
            new("dashX"),
            new("dashY"),
            new("flutter"),
            new("floating"),
            new("friction"),
            new("highJump"),
            new("highFallDamage"),
            new("holdingBlock"),
            new("hover"),
            new("inWater"),
            new("jumping"),
            new("jumpOnEnemy"),
            new("ladderClimb"),
            new("meleeCombo"),
            new("meleeLeft"),
            new("meleeRight"),
            new("mouseDirectionLeft"),
            new("mouseDirectionRight"),
            new("onGround"),
            new("onGroundWall"),
            new("onVehicle"),
            new("onRail"),
            new("pickAndThrowBlock"),
            new("pickingUpBlock"),
            new("pushBack"),
            new("pushBackLeft"),
            new("pushBackRight"),
            new("pushBlock"),
            new("pullBlock"),
            new("recoil"),
            new("recoilDown"),
            new("recoilLeft"),
            new("recoilRight"),
            new("recoilUp"),
            new("recoilShake"),
            new("recoilSlide"),
            new("rope"),
            new("ropeClimbing"),
            new("ropeHanging"),
            new("ropeHolding"),
            new("ropeSwinging"),
            new("running"),
            new("sameDirection"),
            new("slamOnEnemy"),
            new("slamRecover"),
            new("sliding"),
            new("slopeSlide"),
            new("slopeSlideAuto"),
            new("staticFlying"),
            new("swimming"),
            new("throwingBlock"),
            new("vehicleMounted"),
            new("wall"),
            new("wallLeft"),
            new("wallRight"),
            new("wallClimb"),
            new("wallHold"),
            new("wallSlide"),
            new("wallHang"),
            new("wallSlideJump"),
            new("wallCornerGrab"),
            new("windJump"),
            new("velX"),
            new("velXLeft"),
            new("velXRight"),
            new("velXZero"),
            new("velY"),
            new("velYUp"),
            new("velYDown"),
            new("velYZero"),
            new("zipline")
        };

        public List<SignalPacket> extra = new();

        public bool foldOutVelocity;
        public bool foldOutWall;
        public bool foldOutWorld;
        public bool foldOutRecoil;
        public bool foldOutAttack;
        public bool foldOutUtility;
        public bool foldOutExtra;
        public bool button;
        public string createSignal;
    }

    [Serializable]
    public class SignalPacket
    {
        public string name;
        public bool use;
        public bool canDelete;

        public SignalPacket(string name)
        {
            this.name = name;
        }
    }
}