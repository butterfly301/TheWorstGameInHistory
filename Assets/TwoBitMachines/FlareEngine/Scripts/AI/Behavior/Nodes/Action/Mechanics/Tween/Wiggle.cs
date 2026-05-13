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
    public class Wiggle : Action
    {
        [SerializeField] public TweenChild tween = new();

        public override NodeState RunNodeLogic(Root root)
        {
            if (nodeSetup == NodeSetup.NeedToInitialize) tween.Reactivate();
            if (tween.active)
            {
                var position = transform.position;
                tween.Run(transform, null, false, false);
                if (Root.deltaTime != 0)
                {
                    root.velocity = (transform.position - position) / Root.deltaTime;
                    transform.position -= (Vector3)root.velocity * Root.deltaTime; // undo change
                }

                return NodeState.Running;
            }

            return NodeState.Success;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414

        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(55, "Wiggle tween." +
                                      "\n \nReturns Running, Success");

            var child = parent.Get("tween");
            var name = ((Act)child.Get("act").enumValueIndex).ToString();
            WiggleUtilEditor.overrideColor = true;
            WiggleUtilEditor.newColor = color;
            WiggleUtilEditor.TweenType(child, name, true);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}