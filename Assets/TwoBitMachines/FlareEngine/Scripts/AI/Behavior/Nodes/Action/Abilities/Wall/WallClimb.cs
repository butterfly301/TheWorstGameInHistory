#region

using System;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class WallClimb : Action
    {
        [SerializeField] public float climbSpeed = 3f;
        [SerializeField] public float walkSpeed = 10f;
        [SerializeField] public float climbTime = 3f;
        [SerializeField] public string signal;
        [NonSerialized] public bool climbing;

        [NonSerialized] public float counter;
        [NonSerialized] public float direction;

        public void Reset()
        {
            counter = 0;
            climbing = false;
        }

        public override NodeState RunNodeLogic(Root root)
        {
            if (nodeSetup == NodeSetup.NeedToInitialize) Reset();
            if (!climbing)
            {
                root.velocity.x = walkSpeed * root.direction;
                if (root.world.onWall)
                {
                    direction = root.world.rightWall ? 1f : -1f;
                    climbing = true;
                }
            }

            if (climbing)
            {
                root.velocity.x = direction;
                root.velocity.y = climbSpeed;
                root.signals.Set(signal);
                if (TwoBitMachines.Clock.Timer(ref counter, climbTime) || !root.world.onWall) return NodeState.Success;
            }

            return NodeState.Running;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(55, "Climb a wall. If not on a wall, walk towards one first." +
                                      "\n \nReturns Running, Success");
            FoldOut.Box(3, color, offsetY: -2);
            {
                parent.Field("Climb Time", "climbTime");
                parent.FieldDouble("Climb Speed", "climbSpeed", "signal");
                Labels.FieldText("Signal");
                parent.Field("Walk Speed", "walkSpeed");
            }
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}