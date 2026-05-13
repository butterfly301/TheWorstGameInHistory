#region

using System;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;
#if UNITY_EDITOR
using System;
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class RunAndAttack : Action
    {
        [SerializeField] public Blackboard target;
        [SerializeField] public Collider2D collider2DRef;
        [SerializeField] public float speed = 12f;
        [SerializeField] public float smooth = 0.85f;
        [SerializeField] public float radius = 4f;
        [SerializeField] public float attempts = 5f;
        [SerializeField] public string signal;
        [SerializeField] public float holdTime = 0.5f;
        [NonSerialized] private float attempted;
        [NonSerialized] private float counter;
        [NonSerialized] private bool foundTarget;
        [NonSerialized] private bool hold;
        [NonSerialized] private float oldDirectionX;

        [NonSerialized] private float velocityX;

        public void Reset()
        {
            velocityX = 0;
            counter = 0;
            foundTarget = false;
            hold = false;
            if (collider2DRef != null)
                collider2DRef.gameObject.SetActive(false);
        }

        public override NodeState RunNodeLogic(Root root)
        {
            if (target == null) return NodeState.Failure;
            if (nodeSetup == NodeSetup.NeedToInitialize)
            {
                Reset();
                attempted = 0;
                oldDirectionX = -1;
            }

            var t = target.GetTarget();

            if (target.hasNoTargets) return NodeState.Failure;

            if (hold)
            {
                root.signals.Set(signal);
                if (TwoBitMachines.Clock.Timer(ref counter, holdTime))
                {
                    Reset();
                    return NodeState.Success;
                }
            }
            else if (foundTarget)
            {
                root.signals.Set(signal);
                root.velocity.x = velocityX = Compute.Lerp(velocityX, 0, smooth);
                if (Mathf.Abs(velocityX) < 0.25f)
                {
                    Reset();
                    hold = true;
                }
            }
            else
            {
                var directionX = t.x < root.position.x ? -1f : 1f;
                root.velocity.x = velocityX = Compute.Lerp(velocityX, directionX * speed, smooth);

                if ((t - root.position).sqrMagnitude <= radius * radius)
                {
                    if (collider2DRef != null)
                        collider2DRef.gameObject.SetActive(true);
                    foundTarget = true;
                }
                else if (oldDirectionX != Mathf.Sign(velocityX))
                {
                    oldDirectionX = Mathf.Sign(velocityX);
                    attempted++;
                    if (attempted >= attempts) return NodeState.Success;
                }
            }

            if (collider2DRef != null)
            {
                var v = collider2DRef.transform.localPosition;
                collider2DRef.transform.localPosition =
                    new Vector3(Mathf.Sign(velocityX) > 0 ? Mathf.Abs(v.x) : -Mathf.Abs(v.x), v.y, v.z);
            }

            return NodeState.Running;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool HasNextState()
        {
            return false;
        }

        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(55, "Follow a target in the x direction, and attack if within radius." +
                                      "\n \nReturns Success, Failure");

            FoldOut.Box(6, color, offsetY: -2);
            {
                AIBase.SetRef(ai.data, parent.Get("target"), 0);
                parent.FieldDouble("Speed", "speed", "smooth");
                Labels.FieldText("Smooth");
                parent.FieldDouble("Attack Radius", "radius", "collider2DRef");
                Labels.FieldDoubleText("Radius", "");
                parent.Field("Attack Signal", "signal");
                parent.Field("Hold Time", "holdTime");
                parent.Field("Attempts", "attempts");
            }
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}