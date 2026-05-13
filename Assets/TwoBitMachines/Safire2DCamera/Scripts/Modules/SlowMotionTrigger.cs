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
    public class SlowMotionTrigger
    {
        [SerializeField] public bool enable;
        [SerializeField] public List<SlowMotionChild> slowMotions = new();

        [NonSerialized] private SlowMotion timeManager;

        public void Initialize(SlowMotion timeManager)
        {
            this.timeManager = timeManager;
            for (var i = 0; i < slowMotions.Count; i++)
                slowMotions[i].bounds.Initialize();
        }

        public void Execute(Follow follow)
        {
            if (!enable)
                return;

            var target = follow.TargetPosition();
            for (var i = 0; i < slowMotions.Count; i++) slowMotions[i].Execute(target, timeManager);
        }

        [Serializable]
        public class SlowMotionChild
        {
            [SerializeField] public SimpleBounds bounds = new();
            [SerializeField] public OnExit revert;
            [SerializeField] public float intensity = 1;
            [SerializeField] public UnityEvent onEnter;
            [SerializeField] public UnityEvent onExit;
            [NonSerialized] private bool active;

            public void Reset()
            {
                active = false;
            }

            public void Execute(Vector3 position, SlowMotion timeManager)
            {
                if (!active && bounds.Contains(position))
                {
                    active = true;
                    onEnter.Invoke();
                    timeManager.Set(Mathf.Clamp(intensity, 0, 1f), 5f, true);
                }

                if (active && !bounds.Contains(position))
                {
                    active = false;
                    onExit.Invoke();
                    if (revert == OnExit.RevertOnExit)
                        timeManager.Set(Time.timeScale, 1f); // ease out when exiting
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

            if (Follow.Open(parent, "Slow Motion Trigger", barColor, labelColor, true, canView: true))
            {
                GUI.enabled = parent.Bool("enable");
                var array = parent.Get("slowMotions");

                if (parent.ReadBool("add"))
                {
                    array.arraySize++;
                    EditorTools.ClearProperty(array.LastElement());
                    array.LastElement().Get("bounds").Get("position").vector2Value =
                        SceneTools.SceneCenter(Vector2.zero);
                    array.LastElement().Get("bounds").Get("size").vector2Value = new Vector2(5f, 5f);
                    array.LastElement().Get("intensity").floatValue = 1f;
                }

                if (array.arraySize == 0)
                    Layout.VerticalSpacing(5);

                for (var i = 0; i < array.arraySize; i++)
                {
                    var slowMotion = array.Element(i);

                    var color = FoldOut.boxColor;
                    FoldOut.BoxSingle(1, color, 6);
                    {
                        Fields.ConstructField();
                        slowMotion.ConstructField("revert", S.LW);
                        slowMotion.ConstructField("intensity", S.CW - S.B2);
                        if (Fields.ConstructButton("Target")) Follow.Select(array, i);
                        if (Fields.ConstructButton("Delete"))
                        {
                            array.DeleteArrayElement(i);
                            break;
                        }

                        Labels.FieldText("Intensity", S.B2 + 2);

                        if (FoldOut.FoldOutButton(slowMotion.Get("eventsFoldOut")))
                        {
                            Fields.EventFoldOut(slowMotion.Get("onEnter"), slowMotion.Get("enterFoldOut"), "On Enter",
                                color: color);
                            Fields.EventFoldOut(slowMotion.Get("onExit"), slowMotion.Get("exitFoldOut"), "On Exit",
                                color: color);
                        }
                    }
                }

                GUI.enabled = true;
            }
        }

        public static void DrawTrigger(Safire2DCamera main)
        {
            if (!main.slowMotionTrigger.view || !main.slowMotionTrigger.enable)
                return;

            for (var i = 0; i < main.slowMotionTrigger.slowMotions.Count; i++)
            {
                var previousColor = Handles.color;
                var element = main.slowMotionTrigger.slowMotions[i];
                Handles.color = Color.blue;
                var bounds = element.bounds;
                SceneTools.DrawAndModifyBounds(ref bounds.position, ref bounds.size,
                    element.select == i ? Tint.PastelGreen : Handles.color, 0.5f);

                if (Mouse.down && bounds.DetectRaw(Mouse.position))
                {
                    for (var j = 0; j < main.slowMotionTrigger.slowMotions.Count; j++)
                        main.slowMotionTrigger.slowMotions[j].select = -1;
                    element.select = i;
                }

                Handles.color = previousColor;
            }
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    public enum OnExit
    {
        RevertOnExit,
        Maintain
    }
}