#region

using System;
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    // signals should be the same, but set for both
    [AddComponentMenu("")]
    public class ThrowPlayer : Action
    {
        public enum State
        {
            Throw,
            Bounce
        }

        [SerializeField] public Player player;

        [SerializeField] public float throwTime = 0.5f;
        [SerializeField] public string throwSignal;
        [SerializeField] public Vector2 force = new(25f, 2f);

        [SerializeField] public float timeOut = 4f;
        [SerializeField] public string activeSignal;
        [NonSerialized] private float counterThrow;
        [NonSerialized] private float counterTimeOut;

        [NonSerialized] private int direction;
        [NonSerialized] private bool hitGround;
        [NonSerialized] private bool hitWall;
        [NonSerialized] private Vector2 holdPosition;
        [NonSerialized] private float lerp;
        [NonSerialized] private Vector3 oldPosition;
        [NonSerialized] private bool playerThrown;
        [NonSerialized] private State state;
        [NonSerialized] private Vector2 throwVelocity;

        public void Reset()
        {
            state = 0;
            lerp = 0;
            counterThrow = 0;
            counterTimeOut = 0;
            hitWall = false;
            hitGround = false;
            playerThrown = false;
        }

        public override void OnReset(bool skip = false, bool enteredState = false)
        {
            ResetInput();
        }

        public override NodeState RunNodeLogic(Root root)
        {
            if (player == null) return NodeState.Failure;
            if (nodeSetup == NodeSetup.NeedToInitialize)
            {
                Reset();
                direction = root.direction;
                holdPosition = oldPosition = player.transform.position;
            }

            if (state == State.Throw)
            {
                if (TwoBitMachines.Clock.Timer(ref counterThrow, throwTime)) state = State.Bounce;
                HoldPlayer();
                root.signals.Set(throwSignal);
            }

            if (state == State.Bounce)
            {
                if (!playerThrown)
                {
                    playerThrown = true;
                    Throw();
                }

                player.abilities.gravity.Execute(false, ref throwVelocity);
                var forceX = force.x * direction;
                if (hitGround)
                {
                    lerp += Root.deltaTime;
                    forceX = Mathf.Lerp(forceX, 0, lerp / 0.5f);
                }

                var velocity = new Vector2(forceX, throwVelocity.y);
                player.transform.position = oldPosition;

                var onSurface = false;
                player.world.Move(ref velocity, ref velocity.y, false, Root.deltaTime, ref onSurface);
                oldPosition = player.transform.position;

                // --------------------------------------------//

                root.signals.Set(activeSignal);
                player.signals.Set(activeSignal);
                if (player.world.onWall && !hitWall)
                {
                    hitWall = true;
                    direction = -direction;
                }

                if (player.world.onGround) hitGround = true;
                if (lerp >= 0.5f)
                {
                    ResetInput();
                    return NodeState.Success;
                }
            }

            if (state != State.Throw && TwoBitMachines.Clock.Timer(ref counterTimeOut, timeOut))
            {
                ResetInput();
                return NodeState.Success;
            }

            return NodeState.Running;
        }

        public void HoldPlayer()
        {
            player.transform.position = holdPosition;
            player.BlockInput(true);
            player.signals.Set(throwSignal);
            player.signals.ForceDirection(-direction);
            player.abilities.velocity = Vector2.zero; // clears gravity
        }

        public void ResetInput()
        {
            if (player != null) player.BlockInput(false);
        }

        public void Throw()
        {
            throwVelocity = Compute.ArchObject(holdPosition, holdPosition, force.y, player.abilities.gravity.gravity);
            throwVelocity.y += player.abilities.gravity.gravity * Root.deltaTime * 0.5f;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool beginFoldOut;
        [SerializeField] [HideInInspector] public bool beginTransitionFoldOut;

        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(45, "Throw the player. Player will also bounce off wall." +
                                      "\n \nReturns Running, Success, Failure");

            FoldOut.Box(3, color, offsetY: -2);
            {
                parent.Field("Player", "player");
                parent.FieldDouble("Throw", "throwTime", "throwSignal");
                Labels.FieldDoubleText("Time", "Singal");
                parent.Field("Force", "force");
            }
            Layout.VerticalSpacing(3);
            FoldOut.Box(2, color);
            {
                parent.Field("Bounce Signal", "activeSignal");
                parent.Field("Time Out", "timeOut");
            }
            Layout.VerticalSpacing(5);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}