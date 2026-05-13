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
    public class InputGetKeyList : Action
    {
        [SerializeField] public List<KeyCode> keys = new();
        [SerializeField] public UnityEventInt onPressed;

        public override NodeState RunNodeLogic(Root root)
        {
            if (Input.anyKeyDown)
                for (var i = 0; i < keys.Count; i++)
                    if (Input.GetKeyDown(keys[i]))
                    {
                        onPressed.Invoke(i);
                        return NodeState.Success;
                    }

            return NodeState.Running;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool pressedFoldOut;
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(85,
                    "Will return Success if a key in the list was pressed. An event with the key index pressed is invoked.");

            var array = parent.Get("keys");
            if (array.arraySize == 0)
                array.arraySize++;

            FoldOut.Box(array.arraySize, color, 6, -2);
            {
            }
            Block.BoxArray(array, color, 21, false, 0, "", (height, index) =>
            {
                Block.Header(array.Element(index)).BoxRect(Color.clear, leftSpace: 5, height: height)
                    .Field(array.Element(index))
                    .ArrayButtons()
                    .BuildGet()
                    .ReadArrayButtons(array, index);
            });

            Fields.EventFoldOut(parent.Get("onPressed"), parent.Get("pressedFoldOut"), "On Pressed", color: color);
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}