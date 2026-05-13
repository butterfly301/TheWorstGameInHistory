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
    public class FlipChildrenObjects : Action
    {
        [SerializeField] public FlipOn flipOn;
        [SerializeField] public bool flipScaleX = true;
        [SerializeField] public bool flipPositionX;
        [SerializeField] public bool mirrorAngle;

        [SerializeField] public float positionXDirection = 1f;
        [SerializeField] public float scaleXDirection = 1f;
        [SerializeField] [HideInInspector] public SpriteRenderer spriteRenderer;
        [NonSerialized] public float oldSign;

        public override NodeState RunNodeLogic(Root root)
        {
            var sign = Mathf.Sign(root.direction);
            if (root.velocity.x == 0)
                sign = oldSign;

            if (flipOn == FlipOn.SpriteDirection)
            {
                if (spriteRenderer == null)
                    spriteRenderer = transform.gameObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                    sign = spriteRenderer.flipX ? -1f : 1f;
            }

            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (flipPositionX)
                {
                    var p = child.localPosition;
                    child.localPosition = new Vector3(sign * positionXDirection > 0 ? Mathf.Abs(p.x) : -Mathf.Abs(p.x),
                        p.y, p.z);
                }

                if (flipScaleX)
                {
                    var scaleX = Mathf.Abs(child.localScale.x) * sign * scaleXDirection;
                    child.localScale = new Vector3(scaleX, child.localScale.y, child.localScale.z);
                }

                if (mirrorAngle)
                {
                    var r = child.localEulerAngles;
                    child.localRotation = Quaternion.Euler(r.x, sign < 0 ? 180f : 0f, r.z);
                }
            }

            oldSign = sign;
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
                Labels.InfoBoxTop(65,
                    "Flip the children objects of this AI in the x direction. In an AIFSM, run this inside an Always state." +
                    "\n \nReturns Running");

            FoldOut.Box(4, color, offsetY: -2);
            {
                parent.Field("Flip On", "flipOn");
                parent.FieldAndEnable("Flip Scale X", "scaleXDirection", "flipScaleX");
                Labels.FieldText("Direction", 18);
                parent.FieldAndEnable("Flip Position X", "positionXDirection", "flipPositionX");
                Labels.FieldText("Direction", 18);
                parent.FieldToggleAndEnable("Mirror Angle", "mirrorAngle");
            }
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    public enum FlipOn
    {
        VelocityDirection,
        SpriteDirection
    }
}