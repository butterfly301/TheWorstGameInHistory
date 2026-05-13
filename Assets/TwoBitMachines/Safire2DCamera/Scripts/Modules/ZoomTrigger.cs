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
    public class ZoomTrigger
    {
        [SerializeField] public bool enable;
        [SerializeField] public List<ZoomPacket> zooms = new();
        [NonSerialized] private Zoom zoomController;

        public void Initialize(Zoom zoomController)
        {
            this.zoomController = zoomController;
            for (var i = 0; i < zooms.Count; i++)
                zooms[i].bounds.Initialize();
        }

        public void Execute(Follow follow)
        {
            if (!enable)
                return;
            var target = follow.TargetPosition();
            for (var i = 0; i < zooms.Count; i++) zooms[i].Execute(target, zoomController);
        }

        [Serializable]
        public class ZoomPacket
        {
            [SerializeField] public SimpleBounds bounds = new();
            [SerializeField] public OnExit revert;
            [SerializeField] public float scale = 1;
            [SerializeField] public float smooth = 0.5f;
            [SerializeField] public UnityEvent onEnter;
            [SerializeField] public UnityEvent onExit;

            [NonSerialized] private bool active;
            [NonSerialized] private float originalZoomLevel = 1;

            public void Reset()
            {
                active = false;
                originalZoomLevel = 1;
            }

            public void Execute(Vector3 position, Zoom zoom)
            {
                var insideTrigger = bounds.Contains(position);

                if (insideTrigger && !active)
                {
                    active = true;
                    onEnter.Invoke();
                    originalZoomLevel = zoom.scale != 0 ? zoom.scale : 1;
                }

                if (active) zoom.Set(scale, speed: smooth, isTween: false);
                if (active && !insideTrigger)
                {
                    active = false;
                    onExit.Invoke();
                    if (revert == OnExit.RevertOnExit)
                        zoom.Set(originalZoomLevel);
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

            if (Follow.Open(parent, "Zoom Trigger", barColor, labelColor, true, canView: true))
            {
                GUI.enabled = parent.Bool("enable");
                var array = parent.Get("zooms");

                if (parent.ReadBool("add"))
                {
                    array.arraySize++;
                    EditorTools.ClearProperty(array.LastElement());
                    array.LastElement().Get("bounds").Get("position").vector2Value =
                        SceneTools.SceneCenter(Vector2.zero);
                    array.LastElement().Get("bounds").Get("size").vector2Value = new Vector2(5f, 5f);
                    array.LastElement().Get("scale").floatValue = 1f;
                    array.LastElement().Get("smooth").floatValue = 0.5f;
                }

                if (array.arraySize == 0)
                    Layout.VerticalSpacing(5);

                for (var i = 0; i < array.arraySize; i++)
                {
                    var zoom = array.Element(i);

                    var color = FoldOut.boxColor;
                    FoldOut.Box(2, color, 3);
                    {
                        Fields.ConstructField();
                        Fields.ConstructString("", S.LW);
                        zoom.ConstructField("revert", S.CW - S.B2);
                        if (Fields.ConstructButton("Target")) Follow.Select(array, i);
                        if (Fields.ConstructButton("Delete"))
                        {
                            array.DeleteArrayElement(i);
                            break;
                        }

                        zoom.FieldDouble("Settings", "scale", "smooth");
                        Labels.FieldDoubleText("Scale", "Smooth", rightSpacing: 3);
                        zoom.Clamp("smooth");

                        var eventOpen = FoldOut.FoldOutButton(zoom.Get("eventsFoldOut"));
                        Fields.EventFoldOut(zoom.Get("onEnter"), zoom.Get("enterFoldOut"), "On Enter", color: color,
                            execute: eventOpen);
                        Fields.EventFoldOut(zoom.Get("onExit"), zoom.Get("exitFoldOut"), "On Exit", color: color,
                            execute: eventOpen);
                    }
                }

                GUI.enabled = true;
            }
        }

        public static void DrawTrigger(Safire2DCamera main)
        {
            if (!main.zoomTrigger.view || !main.zoomTrigger.enable)
                return;

            for (var i = 0; i < main.zoomTrigger.zooms.Count; i++)
            {
                var previousColor = Handles.color;
                var element = main.zoomTrigger.zooms[i];
                Handles.color = Tint.Purple;
                var bounds = element.bounds;
                SceneTools.DrawAndModifyBounds(ref bounds.position, ref bounds.size,
                    element.select == i ? Tint.PastelGreen : Handles.color, 0.5f);

                if (Mouse.down && bounds.DetectRaw(Mouse.position))
                {
                    for (var j = 0; j < main.zoomTrigger.zooms.Count; j++) main.zoomTrigger.zooms[j].select = -1;
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