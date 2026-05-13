using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

namespace TwoBitMachines.Safire2DCamera
{
    [Serializable]
    public class BasicTrigger
    {
        [SerializeField] public bool enable;
        [SerializeField] public List<Basic> basics = new();

        public void Initialize()
        {
            for (var i = 0; i < basics.Count; i++)
                basics[i].bounds.Initialize();
        }

        public void Execute(Follow follow)
        {
            if (!enable)
                return;

            var target = follow.TargetPosition();
            for (var i = 0; i < basics.Count; i++) basics[i].Execute(target);
        }

        [Serializable]
        public class Basic
        {
            [SerializeField] public SimpleBounds bounds = new();
            [SerializeField] public TriggerOnce triggerOnce;
            [SerializeField] public UnityEvent onEnter;
            [SerializeField] public UnityEvent onExit;

            [NonSerialized] private bool active;
            [NonSerialized] private bool hasBeenTriggered;

            public void Reset()
            {
                hasBeenTriggered = active = false;
            }

            public void Execute(Vector3 target)
            {
                if (hasBeenTriggered)
                    return;

                if (!active && bounds.Contains(target))
                {
                    active = true;
                    onEnter.Invoke();
                }

                if (active && !bounds.Contains(target))
                {
                    if (triggerOnce == TriggerOnce.TriggerOnce)
                        hasBeenTriggered = true;
                    active = false;
                    onExit.Invoke();
                }
            }

            #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
            [SerializeField] [HideInInspector] public int select = -1;
            [SerializeField] [HideInInspector] public bool eventsFoldOut;
            [SerializeField] [HideInInspector] public bool enterFoldOut;
            [SerializeField] [HideInInspector] public bool exitFoldOut;
#pragma warning restore 0414
#endif

            #endregion
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool add;
        [SerializeField] [HideInInspector] public bool edit;
        [SerializeField] [HideInInspector] public bool close;
        [SerializeField] [HideInInspector] public bool foldOut;
        [SerializeField] [HideInInspector] public bool view = true;

        public static void CustomInspector(SerializedProperty parent, Color barColor, Color labelColor)
        {
            if (!parent.Bool("edit"))
                return;

            if (Follow.Open(parent, "Basic Trigger", barColor, labelColor, true, canView: true))
            {
                GUI.enabled = parent.Bool("enable");
                var array = parent.Get("basics");

                if (parent.ReadBool("add"))
                {
                    array.arraySize++;
                    EditorTools.ClearProperty(array.LastElement());
                    array.LastElement().Get("bounds").Get("position").vector2Value =
                        SceneTools.SceneCenter(Vector2.zero);
                    array.LastElement().Get("bounds").Get("size").vector2Value = new Vector2(5f, 5f);
                }

                if (array.arraySize == 0)
                    Layout.VerticalSpacing(5);

                for (var i = 0; i < array.arraySize; i++)
                {
                    var basic = array.Element(i);

                    var color = FoldOut.boxColor;
                    FoldOut.BoxSingle(1, color, 3);
                    {
                        Fields.ConstructField();
                        Fields.ConstructString("", S.LW);
                        basic.ConstructField("triggerOnce", S.CW - S.B2);
                        if (Fields.ConstructButton("Target")) Follow.Select(array, i);
                        if (Fields.ConstructButton("Delete"))
                        {
                            array.DeleteArrayElement(i);
                            break;
                        }

                        var eventOpen = FoldOut.FoldOutButton(basic.Get("eventsFoldOut"), -3);
                        Fields.EventFoldOut(basic.Get("onEnter"), basic.Get("enterFoldOut"), "On Enter", color: color,
                            execute: eventOpen);
                        Fields.EventFoldOut(basic.Get("onExit"), basic.Get("exitFoldOut"), "On Exit", color: color,
                            execute: eventOpen);
                    }
                }

                GUI.enabled = true;
            }
        }

        public static void DrawTrigger(Safire2DCamera main)
        {
            if (!main.basicTrigger.view || !main.basicTrigger.enable)
                return;

            for (var i = 0; i < main.basicTrigger.basics.Count; i++)
            {
                var previousColor = Handles.color;
                var element = main.basicTrigger.basics[i];
                Handles.color = Tint.WarmWhite;
                var bounds = element.bounds;
                SceneTools.DrawAndModifyBounds(ref bounds.position, ref bounds.size,
                    element.select == i ? Tint.PastelGreen : Handles.color, 0.5f);

                if (Mouse.down && bounds.DetectRaw(Mouse.position))
                {
                    for (var j = 0; j < main.basicTrigger.basics.Count; j++) main.basicTrigger.basics[j].select = -1;
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