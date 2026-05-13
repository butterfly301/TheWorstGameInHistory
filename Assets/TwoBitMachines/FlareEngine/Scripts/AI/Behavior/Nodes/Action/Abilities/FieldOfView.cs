#region

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
    public class FieldOfView : Conditional
    {
        [SerializeField] public float radius = 5;
        [SerializeField] public float angle = 25f;
        [SerializeField] public float offset;
        [SerializeField] public float yOffset = 1f;
        [SerializeField] public Blackboard target;

        [SerializeField] private int xDirection;
        [SerializeField] private BoxCollider2D box;

        public void Start()
        {
            if (target != null && target.GetTransform() != null)
            {
                var character = target.GetTransform().GetComponent<Character>();
                if (character != null) box = character.world.boxCollider;
            }
        }

        public override NodeState RunNodeLogic(Root root)
        {
            if (target == null) return NodeState.Failure;

            xDirection = root.direction;
            var targetPosition = target.GetTarget();
            var fovPosition = root.position + (Vector2)transform.up * yOffset;

            if (target.hasNoTargets || (targetPosition - fovPosition).sqrMagnitude > radius * radius)
                return NodeState.Failure;

            if (Find(targetPosition, fovPosition)) return NodeState.Success;
            return NodeState.Failure;
        }

        public bool Find(Vector2 targetPosition, Vector2 fovPosition)
        {
            var v1 = Compute.RotateVector(transform.right, angle * 0.5f + offset) * radius;
            var v2 = Compute.RotateVector(transform.right, -angle * 0.5f + offset) * radius;

            if (xDirection < 0)
            {
                v1.x *= -1f;
                v2.x *= -1f;
            }

            Vector2 targetHeight =
                box != null ? box.transform.up * (box.size.y * box.transform.localScale.y) : Vector3.up;
            if (Compute.LinesIntersect(fovPosition, fovPosition + v2, targetPosition, targetPosition + targetHeight))
                return true;
            if (Compute.LinesIntersect(fovPosition, fovPosition + v1, targetPosition, targetPosition + targetHeight))
                return true;
            if (Compute.IsPointInTriangle(fovPosition, fovPosition + v1, fovPosition + v2, targetPosition)) return true;
            return false;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override void OnSceneGUI(Editor editor)
        {
            var sign = xDirection < 0 ? -1f : 1f;
            var angleA = angle * 0.5f + offset;
            var angleB = -angle * 0.5f + offset;
            Draw.CircleSector(transform.position + transform.up * yOffset, transform.right, radius, angleA, angleB,
                sign, Color.green, radius * 2f);
        }

        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(55, "Returns Success if the specified target is inside the field of view.");

            FoldOut.Box(4, color, offsetY: -2);
            {
                AIBase.SetRef(ai.data, parent.Get("target"), 0);
                parent.Field("Length", "radius");
                parent.FieldDouble("Angle", "angle", "offset");
                Labels.FieldText("Offset", 3);
                parent.Field("Y Offset", "yOffset");
            }
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}