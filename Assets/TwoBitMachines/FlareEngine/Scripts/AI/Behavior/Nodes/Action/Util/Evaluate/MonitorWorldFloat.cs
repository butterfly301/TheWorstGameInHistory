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
    public class MonitorWorldFloat : Action
    {
        public enum MonitorType
        {
            OnDecrease,
            OnIncrease,
            OnAny
        }

        [SerializeField] public WorldFloat variable;
        [SerializeField] public MonitorType returnSuccess;
        private float previousFloat;

        public override NodeState RunNodeLogic(Root root)
        {
            if (variable == null)
                return NodeState.Failure;

            if (nodeSetup == NodeSetup.NeedToInitialize) previousFloat = variable.GetValue();

            var newValue = variable.GetValue();
            if (previousFloat != newValue)
            {
                var oldValue = previousFloat;
                previousFloat = newValue;

                if (returnSuccess == MonitorType.OnDecrease)
                {
                    if (newValue < oldValue) return NodeState.Success;
                }
                else if (returnSuccess == MonitorType.OnIncrease)
                {
                    if (newValue > oldValue) return NodeState.Success;
                }
                else
                {
                    return NodeState.Success;
                }
            }

            return NodeState.Running;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(55, "Check if a world variable has changed value. This will work with Health." +
                                      "\n \nReturns Running, Success, Failure");

            FoldOut.Box(2, color, offsetY: -2);
            parent.Field("World Float", "variable");
            parent.Field("Return Success", "returnSuccess");
            Layout.VerticalSpacing(3);

            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}