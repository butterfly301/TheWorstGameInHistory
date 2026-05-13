#region

using System;
using System.Collections.Generic;
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class ControlPlayerTransform : Action
    {
        [SerializeField] public List<Vector2> position = new();
        [SerializeField] public Player player;
        [SerializeField] public float rate = 5f;
        [SerializeField] public float hold;
        [SerializeField] public UnityEvent onComplete;
        [SerializeField] public string animationSignal;

        [NonSerialized] private float counter;
        [NonSerialized] private bool holdPlayer;
        [NonSerialized] private int index;
        [NonSerialized] private Vector2 start;

        public override NodeState RunNodeLogic(Root root)
        {
            if (player == null || position.Count == 0)
                return NodeState.Failure;

            player.BlockInput(true);
            if (nodeSetup == NodeSetup.NeedToInitialize)
            {
                index = 0;
                counter = 0;
                holdPlayer = false;
                player.transform.position = Position(root);
                start = player.transform.position;
                index++;
            }

            if (holdPlayer)
            {
                index = position.Count - 1; // hold last position
                MovePlayer(root);
                if (TwoBitMachines.Clock.Timer(ref counter, hold))
                {
                    Invoke(root);
                    player.BlockInput(false);
                    return NodeState.Success;
                }

                player.signals.Set(animationSignal);
            }
            else
            {
                MovePlayer(root);

                if ((start - Position(root)).magnitude < 0.1f) index++;
                if (index >= position.Count)
                {
                    holdPlayer = true;
                    if (hold <= 0)
                    {
                        Invoke(root);
                        player.BlockInput(false);
                        return NodeState.Success;
                    }
                }

                player.signals.Set(animationSignal);
            }

            return NodeState.Running;
        }

        public override void OnReset(bool skip = false, bool enteredState = false)
        {
            if (player != null)
                player.BlockInput(false);
        }

        private Vector2 Position(Root root)
        {
            if (index >= position.Count)
                return Vector2.zero;
            var localPosition = position[index];
            localPosition.x = Mathf.Sign(root.direction) * Mathf.Abs(localPosition.x);
            localPosition.x /= transform.localScale.x; // undo scale affect
            localPosition.y /= transform.localScale.y;
            Vector2 returnPosition = transform.TransformPoint(localPosition);
            return returnPosition;
        }

        private void MovePlayer(Root root)
        {
            var destination = Position(root);
            var z = player.transform.position.z;
            player.transform.position = Vector2.MoveTowards(start, destination, rate * Root.deltaTime);
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, z);
            start = player.transform.position;
            player.abilities.ClearYVelocity();
            player.signals.SetDirection(-root.direction);
        }

        private void Invoke(Root root)
        {
            player.signals.SetDirection(root.direction);
            onComplete.Invoke();
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool eventFoldout;
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(105,
                    "Move the player's transform along a set of points. The last point keeps the player in place for the hold time. These points are local to the AI. This will block player input and set a player animation signal." +
                    "\n \nReturns Running, Success, Failure");

            var array = parent.Get("position");
            if (array.arraySize == 0) array.arraySize++;

            FoldOut.Box(4 + array.arraySize, color, offsetY: -2);
            {
                parent.Field("Player", "player");
                parent.Field("Animation Signal", "animationSignal");
                parent.Field("Velocity", "rate");
                parent.Field("Hold Time", "hold");
                for (var i = 0; i < array.arraySize; i++)
                    Fields.ArrayProperty(parent.Get("position"), array.Element(i), i, "Point");
            }
            Layout.VerticalSpacing(3);
            Fields.EventFoldOut(parent.Get("onComplete"), parent.Get("eventFoldout"), "On Complete", color: color);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}