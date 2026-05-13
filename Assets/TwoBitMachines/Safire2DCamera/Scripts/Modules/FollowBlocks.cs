using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

namespace TwoBitMachines.Safire2DCamera
{
    [Serializable]
    public class FollowBlocks
    {
        public enum FollowBlockType
        {
            Horizontal,
            Vertical,
            Exit
        }

        [SerializeField] public bool enable;
        [SerializeField] public List<Block> block = new();
        [NonSerialized] private Vector2 entryPoint;

        [NonSerialized] private int index = -1;

        public void Initialize()
        {
            for (var i = 0; i < block.Count; i++)
                block[i].bounds.Initialize();
        }

        public void Reset()
        {
            index = -1;
        }

        public Vector3 Position(Vector3 target, Follow follow, bool isUser)
        {
            if (!enable || isUser)
                return target;

            Vector2 newTarget = target;
            for (var i = 0; i < block.Count; i++)
                if (i == index || block[i].bounds.Contains(target))
                {
                    if (i != index && index >= 0)
                    {
                        if (block[index].vertical)
                            follow.ForceTargetSmooth(y: false);
                        if (block[index].horizontal)
                            follow.ForceTargetSmooth(false);
                    }

                    if (i != index)
                        entryPoint = target;
                    index = block[i].exit ? -1 : i;
                    if (block[i].exit)
                        follow.ForceTargetSmooth();
                    if (index != -1)
                        newTarget = block[i].Execute(target, entryPoint);
                }

            return newTarget;
        }

        [Serializable]
        public class Block
        {
            [SerializeField] public FollowBlockType type;
            [SerializeField] public SimpleBounds bounds = new();
            [SerializeField] [HideInInspector] public int select = -1;
            public bool exit => type == FollowBlockType.Exit;
            public bool vertical => type == FollowBlockType.Vertical;
            public bool horizontal => type == FollowBlockType.Horizontal;

            public Vector2 Execute(Vector3 target, Vector2 entryPoint)
            {
                return type == FollowBlockType.Horizontal
                    ? new Vector2(target.x, entryPoint.y)
                    : new Vector2(entryPoint.x, target.y);
            }
        }

        #region ▀▄▀▄▀▄ Custom Inspector▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool view = true;
        [SerializeField] [HideInInspector] public bool foldOut;
        [SerializeField] [HideInInspector] public bool close;
        [SerializeField] [HideInInspector] public bool edit;
        [SerializeField] [HideInInspector] public bool add;

        public static void CustomInspector(SerializedProperty parent, Color barColor, Color labelColor)
        {
            if (!parent.Bool("edit"))
                return;

            if (Follow.Open(parent, "Follow Blocks", barColor, labelColor, true, canView: true))
            {
                var array = parent.Get("block");

                if (parent.ReadBool("add"))
                {
                    array.arraySize++;
                    array.LastElement().Get("bounds").Get("position").vector2Value =
                        SceneTools.SceneCenter(Vector2.zero);
                    array.LastElement().Get("bounds").Get("size").vector2Value = new Vector2(5f, 5f);
                }

                if (array.arraySize == 0)
                    Layout.VerticalSpacing(5);

                for (var i = 0; i < array.arraySize; i++)
                {
                    GUI.enabled = parent.Bool("enable");
                    var element = array.Element(i);
                    var type = element.Get("type").enumValueIndex;

                    var color = type == 0 ? Tint.Brown : type == 1 ? Tint.Blue : Tint.Delete;
                    FoldOut.BoxSingle(1, color);
                    {
                        Fields.ConstructField();
                        element.ConstructField("type", S.FW - S.B2);
                        if (Fields.ConstructButton("Target")) Follow.Select(array, i);
                        if (Fields.ConstructButton("Delete"))
                        {
                            array.DeleteArrayElement(i);
                            break;
                        }
                    }
                    Layout.VerticalSpacing(2);
                    GUI.enabled = true;
                }
            }
        }

        public static void DrawTrigger(Safire2DCamera main)
        {
            if (!main.follow.followBlocks.view || !main.follow.followBlocks.view)
                return;

            for (var i = 0; i < main.follow.followBlocks.block.Count; i++)
            {
                var previousColor = Handles.color;
                var element = main.follow.followBlocks.block[i];
                Handles.color = element.type == FollowBlockType.Horizontal ? Tint.SoftDark :
                    element.type == FollowBlockType.Vertical ? Tint.Blue : Tint.Delete;
                var bounds = element.bounds;
                SceneTools.DrawAndModifyBounds(ref bounds.position, ref bounds.size,
                    element.select == i ? Tint.PastelGreen : Handles.color, 0.5f);

                if (Mouse.down && bounds.DetectRaw(Mouse.position))
                {
                    for (var j = 0; j < main.follow.followBlocks.block.Count; j++)
                        main.follow.followBlocks.block[j].select = -1;
                    element.select = i;
                }

                Handles.color = previousColor;
            }
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}