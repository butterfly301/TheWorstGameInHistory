#region

using System;
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
    public class InputGetKeyAndSignalList : Action
    {
        [SerializeField] public List<KeySignal> list = new();
        [SerializeField] public Character character;
        private int index = -1;

        public override NodeState RunNodeLogic(Root root)
        {
            if (nodeSetup == NodeSetup.NeedToInitialize) index = -1;
            if (Input.anyKeyDown)
                for (var i = 0; i < list.Count; i++)
                    if (Input.GetKeyDown(list[i].key))
                    {
                        index = i;
                        break;
                    }

            if (index >= 0 && index < list.Count && character != null) character.signals.Set(list[index].signal);
            return NodeState.Running;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414

        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(85, "Will set a character signal true according the key pressed in this list.");

            var array = parent.Get("list");
            if (array.arraySize == 0)
                array.arraySize++;

            FoldOut.Box(1 + array.arraySize, color, 6, -2);
            {
                parent.Field("Character", "character");
            }
            Block.BoxArray(array, color, 21, false, 0, "", (height, index) =>
            {
                Block.Header(array.Element(index)).BoxRect(Color.clear, leftSpace: 5, height: height)
                    .Field("key", 0.5f)
                    .Field("signal", 0.5f)
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

        [Serializable]
        public struct KeySignal
        {
            public KeyCode key;
            public string signal;
        }
    }
}