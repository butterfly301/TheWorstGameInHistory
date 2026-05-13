using System;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class ProjectileRotatePattern : Action
    {
        [SerializeField] public ProjectileBase projectile;
        [SerializeField] public RotatePattern pattern;
        [SerializeField] public Tween tween = Tween.Linear;
        [SerializeField] public float rotateBy = 180f;
        [SerializeField] public float fireRate = 0.1f;
        [SerializeField] public float rotateSpeed = 10f;
        [SerializeField] public float duration = 1f;
        [SerializeField] public bool timeLimit;

        [NonSerialized] private float completeCounter;
        [NonSerialized] private float endAngle;
        [NonSerialized] private float lerpTime;
        [NonSerialized] private float shootCounter;
        [NonSerialized] private float startAngle;

        public override NodeState RunNodeLogic(Root root)
        {
            if (projectile == null) return NodeState.Failure;

            if (nodeSetup == NodeSetup.NeedToInitialize)
            {
                completeCounter = 0;
                shootCounter = 1000f; // shoot immediately
                lerpTime = 0;
                if (pattern == RotatePattern.RotateByAngle)
                {
                    startAngle = transform.eulerAngles.z;
                    endAngle = startAngle + rotateBy;
                }
            }

            if (TwoBitMachines.Clock.Timer(ref shootCounter, fireRate))
                projectile.FireProjectileStatic(transform, transform.rotation);

            if (pattern == RotatePattern.RotateByAngle)
            {
                lerpTime += Time.deltaTime;
                var percent = lerpTime / duration;
                var currentAngle = Mathf.Lerp(startAngle, endAngle, EasingFunction.Run(tween, percent));
                transform.localEulerAngles = new Vector3(0, 0, currentAngle);
                if (percent >= 1f) return NodeState.Success;
            }
            else
            {
                transform.Rotate(Vector3.forward * rotateSpeed * 10f * Time.deltaTime, Space.Self);
                if (timeLimit && TwoBitMachines.Clock.Timer(ref completeCounter, duration)) return NodeState.Success;
            }

            return NodeState.Running;
        }

        #region ▀▄▀▄▀▄ Custom Inspector▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(45, "Rotate and shoot a projectile." +
                                      "\n \nReturns Success, Failure");

            var type = parent.Enum("pattern");
            FoldOut.Box(5, color, offsetY: -2);
            {
                parent.Field("Projectile", "projectile");
                parent.Field("Type", "pattern", type == 0);
                parent.FieldDouble("Type", "pattern", "rotateBy", type == 1);
                parent.Field("Rotate Speed", "rotateSpeed", type == 0);
                parent.Field("Easing", "tween", type == 1);
                parent.Field("Fire Rate", "fireRate");
                parent.Field("Duration", "duration", type == 1);
                parent.FieldAndEnable("Duration", "duration", "timeLimit", type == 0);
            }
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    public enum RotatePattern
    {
        RotateFreely,
        RotateByAngle
    }
}