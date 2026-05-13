#region

using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class InputGetSequence : Conditional
    {
        public List<KeyCode> keys = new();
        public bool exitOnFailure;
        private int index;

        public override NodeState RunNodeLogic(Root root)
        {
            if (nodeSetup == NodeSetup.NeedToInitialize) index = 0;

            if (index < keys.Count && Input.anyKeyDown)
            {
                if (Input.GetKeyDown(keys[index]))
                    index++;
                else if (exitOnFailure) return NodeState.Failure;
            }

            return index >= keys.Count ? NodeState.Success : NodeState.Running;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(85, "User must press correct sequence of keys to return Success.");

            var array = parent.Get("keys");
            if (array.arraySize == 0)
                array.arraySize++;

            FoldOut.Box(1 + array.arraySize, color, 6, -2);
            {
                parent.FieldToggleAndEnable("Exit On Fail", "exitOnFailure");
            }
            Block.BoxArray(array, color, 21, false, 0, "", (height, index) =>
            {
                Block.Header(array.Element(index)).BoxRect(Color.clear, leftSpace: 5, height: height)
                    .Field(array.Element(index))
                    .ArrayButtons()
                    .BuildGet()
                    .ReadArrayButtons(array, index);
            });
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}