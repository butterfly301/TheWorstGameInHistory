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
    public class SetAsChild : Action
    {
        [SerializeField] public SetAsChildType type;
        [SerializeField] public Blackboard child;
        [SerializeField] public Vector3 localPosition;
        [SerializeField] public bool setLocalPosition;

        public override NodeState RunNodeLogic(Root root)
        {
            if (child == null)
                return NodeState.Failure;

            var childTransform = child.GetTransform();

            if (childTransform == null)
                return NodeState.Failure;

            if (type == SetAsChildType.SetAsChild)
            {
                childTransform.parent = transform;
                if (setLocalPosition)
                    childTransform.localPosition = localPosition;
            }
            else
            {
                childTransform.parent = null;
            }

            return NodeState.Success;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(65,
                    "Set the specified transform as child of the AI or remove it as a child of the AI." +
                    "\n \nReturns Success, Failure");

            var index = parent.Enum("type");
            var height = index == 0 ? 1 : 0;
            FoldOut.Box(2 + height, color, offsetY: -2);
            parent.Field("Type", "type");
            AIBase.SetRef(ai.data, parent.Get("child"), 0);
            parent.FieldAndEnable("Local Position", "localPosition", "setLocalPosition", index == 0);
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    public enum SetAsChildType
    {
        SetAsChild,
        RemoveAsChild
    }
}