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
    public class WorldFloatLogic : Conditional
    {
        [SerializeField] public WorldFloat variable;
        [SerializeField] public FloatLogicType logic;
        [SerializeField] public CompareTo compareTo;
        [SerializeField] public float compareFloat;
        [SerializeField] public WorldFloat compareVariable;

        public override NodeState RunNodeLogic(Root root)
        {
            if (variable == null)
                return NodeState.Failure;

            var compareValue = compareTo == CompareTo.Value ? compareFloat :
                compareTo == CompareTo.OtherVariable && compareVariable != null ? compareVariable.GetValue() : 0;
            return Compare(logic, variable.GetValue(), compareValue);
        }

        public static NodeState Compare(FloatLogicType logic, float a, float b)
        {
            if (logic == FloatLogicType.Greater) return a > b ? NodeState.Success : NodeState.Failure;
            if (logic == FloatLogicType.Less) return a < b ? NodeState.Success : NodeState.Failure;
            if (logic == FloatLogicType.Equal) return a == b ? NodeState.Success : NodeState.Failure;
            if (logic == FloatLogicType.GreaterOrEqualTo) return a >= b ? NodeState.Success : NodeState.Failure;
            if (logic == FloatLogicType.LessOrEqualTo) return a <= b ? NodeState.Success : NodeState.Failure;
            if (logic == FloatLogicType.IsTrue) return a > 0 ? NodeState.Success : NodeState.Failure;
            if (logic == FloatLogicType.IsFalse) return a <= 0 ? NodeState.Success : NodeState.Failure;
            if (logic == FloatLogicType.IsEven) return a % 2 == 0 ? NodeState.Success : NodeState.Failure;
            if (logic == FloatLogicType.IsOdd) return a % 2 != 0 ? NodeState.Success : NodeState.Failure;
            return NodeState.Failure;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo")) Labels.InfoBoxTop(35, "Compare a world float to a float value.");

            var logic = parent.Enum("logic");
            var height = logic <= 4 ? 1 : 0;

            var type = parent.Enum("compareTo");
            FoldOut.Box(2 + height, color, offsetY: -2);
            parent.Field("World Float", "variable");
            parent.Field("Logic", "logic");
            parent.FieldDouble("Compare To", "compareTo", "compareFloat", type == 0 && height == 1);
            parent.FieldDouble("Compare To", "compareTo", "compareVariable", type == 1 && height == 1);
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}