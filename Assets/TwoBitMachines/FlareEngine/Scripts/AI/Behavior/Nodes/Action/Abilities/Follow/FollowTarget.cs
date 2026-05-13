#region

using System;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class FollowTarget : Action
    {
        [SerializeField] public Blackboard target;
        [SerializeField] public Axis axis;
        [SerializeField] public float speed = 5f;
        [SerializeField] public float findDistance;
        [SerializeField] public bool hasGravity;
        [SerializeField] public float pauseTimer;
        [SerializeField] public bool pauseOnChange;
        [SerializeField] public int startIndex;
        [SerializeField] private float xVelRef;
        [SerializeField] private bool crossedThreshold;
        [SerializeField] private bool refreshTarget = true;
        [NonSerialized] private float counter;
        [NonSerialized] private int frameExit = -10;
        [NonSerialized] private int index;
        [NonSerialized] private bool pauseX;
        [NonSerialized] private bool reversing;

        [NonSerialized] private Vector3 targetP;

        private bool withinFrame => frameExit + 1 == Time.frameCount || frameExit == Time.frameCount;

        public override NodeState RunNodeLogic(Root root)
        {
            if (target == null) return NodeState.Failure;
            if (nodeSetup == NodeSetup.NeedToInitialize)
            {
                xVelRef = 0;
                counter = 0;
                pauseX = false;
                crossedThreshold = false;
                index = withinFrame ? 0 : startIndex;
                targetP = target.GetTarget(index);
            }

            if (refreshTarget) targetP = target.GetTarget(index);
            if (target.hasNoTargets) return NodeState.Failure;
            if (Root.deltaTime == 0) return NodeState.Running;

            if (axis == Axis.X)
            {
                var newP = Mathf.MoveTowards(root.position.x, targetP.x, speed * Root.deltaTime);
                root.velocity.x += (newP - root.position.x) / Root.deltaTime;
                if (pauseOnChange && !pauseX && !Compute.SameSign(xVelRef, root.velocity.x)) pauseX = true;
                if (pauseX && TwoBitMachines.Clock.TimerInverse(ref counter, pauseTimer))
                {
                    root.velocity.x = 0;
                    root.signals.Set("directionChangedPause");
                }
                else
                {
                    pauseX = false;
                }

                if (hasGravity && !root.onGround)
                {
                    root.velocity.x = xVelRef; // If jumping, retain old direction.
                    if (Mathf.Abs(targetP.x - root.position.x) <= findDistance) crossedThreshold = true;
                }
                else if (Mathf.Abs(targetP.x - root.position.x) <= findDistance || crossedThreshold)
                {
                    root.velocity.x = 0;
                    crossedThreshold = false;
                    return target.NextTarget(ref targetP, ref reversing, ref index, root.transform);
                }

                xVelRef = root.velocity.x;
            }
            else if (axis == Axis.Y)
            {
                var newP = Mathf.MoveTowards(root.position.y, targetP.y, speed * Root.deltaTime);
                root.velocity.y = (newP - root.position.y) / Root.deltaTime;
                if (Mathf.Abs(targetP.y - root.position.y) <= findDistance)
                {
                    root.velocity.y = 0;
                    return target.NextTarget(ref targetP, ref reversing, ref index, root.transform);
                }
            }
            else
            {
                var newPosition = Vector2.MoveTowards(root.position, targetP, speed * Root.deltaTime);
                root.velocity = (newPosition - root.position) / Root.deltaTime;
                if ((targetP - (Vector3)root.position).sqrMagnitude <= findDistance * findDistance)
                {
                    root.velocity = Vector2.zero;
                    frameExit = Time.frameCount;
                    return target.NextTarget(ref targetP, ref reversing, ref index, root.transform);
                }
            }

            return NodeState.Running;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(128,
                    "Follow the target on the chosen axis with the specified speed. The AI will stop near the target if it's within the stop distance. Refresh Target will refresh the target's position every frame. If has gravity is enabled, the AI will retain its direction in the x-axis if it's jumping. The AI will pause if target changes direction." +
                    "\n \nReturns Running, Success, Failure");

            FoldOut.Box(8, color, offsetY: -2);
            AIBase.SetRef(ai.data, parent.Get("target"), 0);
            parent.Field("Axis", "axis");
            parent.Field("Speed", "speed");
            parent.Field("Stop Distance", "findDistance");
            parent.Field("Start Index", "startIndex");
            parent.FieldAndEnable("Pause On Change", "pauseTimer", "pauseOnChange");
            parent.FieldToggleAndEnable("Refresh Target", "refreshTarget");
            parent.FieldToggleAndEnable("Has Gravity", "hasGravity");
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    public enum Axis
    {
        XY,
        X,
        Y
    }
}