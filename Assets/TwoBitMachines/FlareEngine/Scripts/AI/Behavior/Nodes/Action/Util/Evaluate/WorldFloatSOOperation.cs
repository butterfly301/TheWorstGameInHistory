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
    public class WorldFloatSOOperation : Action
    {
        [SerializeField] public WorldFloatSO worldFloat;
        [SerializeField] public WorldFloatOperate operation;
        [SerializeField] public OperateWith valueType;
        [SerializeField] public float floatValue;
        [SerializeField] public WorldFloatSO worldFloatValue;

        public override NodeState RunNodeLogic(Root root)
        {
            if (worldFloat == null)
                return NodeState.Failure;

            var value = valueType == OperateWith.FloatValue ? floatValue :
                valueType == OperateWith.WorldFloat && worldFloatValue != null ? worldFloatValue.GetValue() : 0;
            Operate(operation, value);
            return NodeState.Success;
        }

        public void Operate(WorldFloatOperate logic, float value)
        {
            if (logic == WorldFloatOperate.Add) worldFloat.IncrementValue(value);
            if (logic == WorldFloatOperate.Subtract) worldFloat.IncrementValue(-value);
            if (logic == WorldFloatOperate.Multiply) worldFloat.SetValue(worldFloat.GetValue() * value);
            if (logic == WorldFloatOperate.Divide && value != 0) worldFloat.SetValue(worldFloat.GetValue() / value);
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(35, "Perform a math operation on this World Float SO using another World Float.");


            var type = parent.Enum("valueType");
            FoldOut.Box(3, color, offsetY: -2);
            {
                parent.Field("World Float SO", "worldFloat");
                parent.Field("Operation", "operation");
                parent.FieldDouble("Operand", "valueType", "worldFloatValue", type == 0);
                parent.FieldDouble("Operand", "valueType", "floatValue", type == 1);
            }
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}