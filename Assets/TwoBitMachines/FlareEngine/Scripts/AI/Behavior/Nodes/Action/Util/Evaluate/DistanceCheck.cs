#region

using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class DistanceCheck : Conditional
    {
        public enum DistanceCheckType
        {
            GreaterThan,
            LessThan,
            Equal
        }

        [SerializeField] public Transform transformA;
        [SerializeField] public Transform transformB;
        [SerializeField] public DistanceCheckType type;
        [SerializeField] public float distance;

        public override NodeState RunNodeLogic(Root root)
        {
            if (transformA == null || transformB == null) return NodeState.Failure;

            var checkDistance = ((Vector2)transformA.position - (Vector2)transformB.position).sqrMagnitude;
            if (type == DistanceCheckType.GreaterThan)
                return checkDistance > distance * distance ? NodeState.Success : NodeState.Failure;

            if (type == DistanceCheckType.LessThan)
                return checkDistance < distance * distance ? NodeState.Success : NodeState.Failure;

            return checkDistance == distance * distance ? NodeState.Success : NodeState.Failure;
        }


        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo")) Labels.InfoBoxTop(35, "Check the distance between two transforms.");

            FoldOut.Box(3, color, offsetY: -2);
            parent.Field("Transform A", "transformA");
            parent.Field("Transform B", "transformB");
            parent.FieldDouble("Type", "type", "distance");
            Labels.FieldText("Distance");
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}