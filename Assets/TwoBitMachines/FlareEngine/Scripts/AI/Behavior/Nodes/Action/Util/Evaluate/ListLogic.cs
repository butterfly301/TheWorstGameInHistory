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
    public class ListLogic : Conditional
    {
        [SerializeField] public Blackboard list;
        [SerializeField] public FloatLogicType logic;
        [SerializeField] public CompareTo compareTo;
        [SerializeField] public float compareFloat;
        [SerializeField] public Blackboard compareVariable;

        public override NodeState RunNodeLogic(Root root)
        {
            if (list == null)
                return NodeState.Failure;

            var compareValue = compareTo == CompareTo.Value ? compareFloat :
                compareTo == CompareTo.OtherVariable && compareVariable != null ? compareVariable.GetValue() : 0;
            return WorldFloatLogic.Compare(logic, list.ListCount(), compareValue);
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(35, "Compare the size of the specified list to a float value.");

            var logic = parent.Enum("logic");
            var height = logic <= 4 ? 2 : 0;

            var type = parent.Enum("compareTo");
            FoldOut.Box(2 + height, color, offsetY: -2);
            AIBase.SetRef(ai.data, parent.Get("list"), 0);
            parent.Field("Logic", "logic");
            parent.Field("Compare To", "compareTo", height == 2);
            parent.Field("Compare Float", "compareFloat", type == 0 && height == 2);
            if (type == 1 && height == 2)
                AIBase.SetRef(ai.data, parent.Get("compareVariable"), 1);
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}