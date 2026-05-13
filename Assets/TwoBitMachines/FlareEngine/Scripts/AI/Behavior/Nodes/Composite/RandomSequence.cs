#region

using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class RandomSequence : Composite
    {
        private readonly Random range = new();

        public override NodeState RunNodeLogic(Root root)
        {
            if (useSignal) root.signals.Set(defaultSignal);

            if (nodeSetup == NodeSetup.NeedToInitialize) Shuffle(children);

            for (var i = currentChildIndex; i < children.Count; i++)
            {
                var childState = children[i].RunChild(root);
                if (childState == NodeState.Success)
                {
                    currentChildIndex++; // only execute the current node unless composite can interrupt itself
                    continue;
                }

                if (childState == NodeState.Failure) return NodeState.Failure;
                if (childState == NodeState.Running) return NodeState.Running;
            }

            return NodeState.Success;
        }

        public void Shuffle(IList<Node> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = range.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(45,
                    "This will shuffle the list of nodes so that the execution order is always randomized.");

            FoldOut.Box(3, color, offsetY: -2);
            parent.Field("Can Interrupt", "canInterrupt");
            parent.Field("On Interrupt", "onInterrupt");
            parent.FieldAndEnable("Default Signal", "defaultSignal", "useSignal");
            Layout.VerticalSpacing(3);

            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}