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
    public class AnimationSignal : Action
    {
        [SerializeField] public string signalName;

        [SerializeField] public bool useWorldFloat;
        [SerializeField] public string failedSignalName;
        [SerializeField] public WorldFloat variable;
        [SerializeField] public FloatLogicType logic;
        [SerializeField] public CompareTo compareTo;
        [SerializeField] public float compareFloat;
        [SerializeField] public WorldFloat compareVariable;

        public override NodeState RunNodeLogic(Root root)
        {
            if (!useWorldFloat)
            {
                root.signals.Set(signalName);
                return NodeState.Success;
            }

            if (variable == null)
                return NodeState.Failure;

            var compareValue = compareTo == CompareTo.Value ? compareFloat :
                compareTo == CompareTo.OtherVariable && compareVariable != null ? compareVariable.GetValue() : 0;
            var state = WorldFloatLogic.Compare(logic, variable.GetValue(), compareValue);
            root.signals.Set(state == NodeState.Success ? signalName : failedSignalName);
            return state;
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
                    "Sets an AI animation signal true. You can also set a signal based on a World Float value." +
                    "\n \nReturns Success");
            if (!parent.Bool("useWorldFloat"))
            {
                FoldOut.Box(2, color, offsetY: -2);
                parent.Field("Signal", "signalName");
                parent.FieldAndEnable("Use World Float", "variable", "useWorldFloat");
                Layout.VerticalSpacing(3);
            }
            else
            {
                var logic = parent.Enum("logic");
                var height = logic <= 4 ? 2 : 0;

                var type = parent.Enum("compareTo");
                FoldOut.Box(2 + height, color, offsetY: -2);
                parent.FieldAndEnable("Use World Float", "variable", "useWorldFloat");
                parent.Field("Logic", "logic");
                parent.FieldDouble("Compare Float", "compareTo", "compareFloat", type == 0 && height == 2);
                parent.FieldDouble("Compare Float", "compareTo", "compareVariable", type == 1 && height == 2);
                parent.FieldDouble("Success, Fail Signal", "signalName", "failedSignalName");
                Layout.VerticalSpacing(3);
            }

            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}