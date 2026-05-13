#region

using System;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class BoxCollider2DSize : Action
    {
        [SerializeField] public Vector2 offset;
        [SerializeField] public Vector2 size;
        [NonSerialized] private BoxCollider2D box;

        private void Awake()
        {
            box = gameObject.GetComponent<BoxCollider2D>();
        }

        public override NodeState RunNodeLogic(Root root)
        {
            if (box == null)
                return NodeState.Failure;
            if (nodeSetup == NodeSetup.NeedToInitialize)
            {
                box.offset = offset;
                box.size = size;
                transform.position += transform.up * 0.001f;
            }

            return NodeState.Success;
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
                Labels.InfoBoxTop(55, "Change the size and offset of a BoxCollider2D." +
                                      "\n \nReturns Failure, Success");

            FoldOut.Box(2, color, offsetY: -2);
            parent.Field("Size", "size");
            parent.Field("Offset", "offset");
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}