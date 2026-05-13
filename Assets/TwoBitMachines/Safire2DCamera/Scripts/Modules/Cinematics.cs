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
    public class Cinematics
    {
        [SerializeField] public bool enable;
        [SerializeField] public List<Cinematic> cines = new();
        [NonSerialized] public bool active;
        [NonSerialized] public Follow follow;
        [NonSerialized] public Shakes shakes;

        [NonSerialized] public SlowMotion timeManager;

        public void Initialize(Safire2DCamera main)
        {
            follow = main.follow;
            shakes = main.shake;
            timeManager = main.timeManager;
            for (var i = 0; i < cines.Count; i++) cines[i].bounds.Initialize();
            Reset();
        }

        public void Reset()
        {
            active = false;
            for (var i = 0; i < cines.Count; i++)
            {
                cines[i].activated = false;
                if (cines[i].letterBox != null) cines[i].letterBox.gameObject.SetActive(false);
            }
        }

        public bool Execute()
        {
            if (!enable) return false;

            var target = follow.TargetPosition();
            var cameraPosition = follow.cameraTransform.position;
            for (var i = 0; i < cines.Count; i++)
                if (!cines[i].pause &&
                    cines[i].CompleteCinematic(cameraPosition, target, active, follow, shakes, timeManager))
                    return active = true;

            return active = false;
        }

        public void TriggerCinematic(string cinematicName, Vector2 cameraPosition)
        {
            for (var i = 0; i < cines.Count; i++)
                if (cinematicName == cines[i].name && cines[i].targets.Count > 0)
                {
                    cines[i].TriggerCinematic(shakes, timeManager, cameraPosition);
                    return;
                }
        }

        public void NextCinematicTarget()
        {
            for (var i = 0; i < cines.Count; i++)
                if (cines[i].activated)
                {
                    cines[i].state = CineState.NextTarget;
                    return;
                }
        }

        public void CinematicPause(string cinematicName)
        {
            for (var i = 0; i < cines.Count; i++)
                if (cinematicName == cines[i].name && cines[i].targets.Count > 0)
                {
                    cines[i].pause = true;
                    return;
                }
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

            if (Follow.Open(parent, "Cinematics", barColor, labelColor, true, canView: true))
            {
                GUI.enabled = parent.Bool("enable");
                var array = parent.Get("cines");

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
                    var cine = array.Element(i);
                    var targets = cine.Get("targets");

                    FoldOut.BoxSingle(1, Tint.Orange);
                    {
                        Fields.ConstructField();
                        cine.ConstructField("name", S.FW - S.B4 - 5f, 5f);
                        if (Fields.ConstructButton("Delete"))
                        {
                            array.DeleteArrayElement(i);
                            break;
                        }

                        if (Fields.ConstructButton("Target")) Follow.Select(array, i);
                        if (Fields.ConstructButton("Add")) cine.Toggle("add");
                        if (Fields.ConstructButton("Reopen")) cine.Toggle("open");

                        if (cine.ReadBool("add"))
                        {
                            targets.arraySize++;
                            if (targets.arraySize == 1)
                                targets.LastElement().Get("position").vector2Value =
                                    SceneTools.SceneCenter(Vector2.zero);
                            targets.LastElement().Get("position").vector2Value += Vector2.up * 2f;
                            targets.LastElement().Get("duration").floatValue = 1f;
                        }
                    }
                    Layout.VerticalSpacing(2);

                    if (!cine.Bool("open"))
                        continue;

                    FoldOut.Box(3, Tint.Orange, 5, -2);
                    {
                        cine.Field("Trigger", "type");
                        cine.Field("Follow Pivot", "pivot");
                        cine.Field("Letter Box", "letterBox");
                    }
                    if (FoldOut.FoldOutButton(cine.Get("eventsFoldOut")))
                    {
                        Fields.EventFoldOut(cine.Get("onBegin"), cine.Get("beginFoldOut"), "On Begin",
                            color: Tint.Orange);
                        Fields.EventFoldOut(cine.Get("onComplete"), cine.Get("completeFoldOut"), "On Complete",
                            color: Tint.Orange);
                    }

                    for (var j = 0; j < targets.arraySize; j++)
                    {
                        var target = targets.Element(j);

                        var isLast = j == targets.arraySize - 1;
                        FoldOut.Box(5 + (isLast ? 1 : 0), FoldOut.boxColor, 3);
                        {
                            if (target.FieldAndButton("", "position", "Delete"))
                            {
                                targets.DeleteArrayElement(j);
                                break;
                            }

                            ListReorder.Grip(cine, targets, Layout.GetLastRect(20, 20), j, Tint.WarmWhite);
                            target.FieldDouble("Duration", "tween", "duration");
                            target.FieldDouble("Follow X, Y", "followX", "followY");
                            target.FieldAndDisable("Wait", "Halt", "wait", "halt");
                            target.FieldAndEnable("Zoom", "zoomScale", "zoom");
                            target.FieldToggle("Return To Origin", "returnToOrigin", execute: isLast);
                        }
                        var eventOpen = FoldOut.FoldOutButton(target.Get("eventsFoldOut"));
                        Fields.EventFoldOut(target.Get("onArrival"), target.Get("arrivalFoldOut"), "On Arrival",
                            execute: eventOpen);
                    }
                }

                GUI.enabled = true;
            }
        }

        public static void DrawCinematics(Safire2DCamera main, Editor editor)
        {
            if (!main.cinematics.view || !main.cinematics.enable)
                return;

            for (var i = 0; i < main.cinematics.cines.Count; i++)
            {
                var previousColor = Handles.color;
                var cine = main.cinematics.cines[i];
                Handles.color = Tint.Orange;
                var bounds = cine.bounds;
                SceneTools.DrawAndModifyBounds(ref bounds.position, ref bounds.size,
                    cine.select == i ? Tint.PastelGreen : Tint.Orange, 0.5f);

                var targets = cine.targets;

                for (var j = 0; j < targets.Count; j++)
                    targets[j].position = SceneTools.MovePositionCircleHandle(targets[j].position, editor: editor);
                for (var j = 0; j < targets.Count - 1; j++)
                    SceneTools.Line(targets[j].position, targets[j + 1].position);
                if (targets.Count > 0) SceneTools.Line(bounds.center, targets[0].position);
                if (Mouse.down && bounds.DetectRaw(Mouse.position))
                {
                    for (var j = 0; j < main.cinematics.cines.Count; j++)
                    {
                        main.cinematics.cines[j].select = -1;
                        main.cinematics.cines[j].open = false;
                    }

                    cine.select = i;
                    cine.open = true;
                }

                Handles.color = previousColor;
            }
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    [Serializable]
    public class Cinematic
    {
        [SerializeField] public string name;
        [SerializeField] public TriggerType type;
        [SerializeField] public UnityEvents letterBox;
        [SerializeField] public UnityEvent onComplete;
        [SerializeField] public UnityEvent onBegin;
        [SerializeField] public CinePivot pivot;
        [SerializeField] public List<CinematicTarget> targets = new();
        [SerializeField] [HideInInspector] public SimpleBounds bounds = new();
        [NonSerialized] public bool activated;
        [NonSerialized] private Vector2 bottomClamp;
        [NonSerialized] private Vector2 currentPosition;
        [NonSerialized] private bool dontTrigger;
        [NonSerialized] private Vector2 enterPosition;
        [NonSerialized] private int index;
        [NonSerialized] public bool pause;
        [NonSerialized] private Vector3 previousTarget;
        [NonSerialized] private Vector2 startPosition;

        [NonSerialized] public CineState state;
        [NonSerialized] private float tweenTimer;
        [NonSerialized] private float waitTimer;

        private bool followCenter => pivot == CinePivot.Center || index == targets.Count - 1;

        public void TriggerCinematic(Shakes shakes, SlowMotion timeManager, Vector2 cameraPosition)
        {
            index = -1;
            activated = true;
            onBegin.Invoke();
            tweenTimer = waitTimer = 0;
            state = CineState.NextTarget;
            enterPosition = cameraPosition;
            currentPosition = cameraPosition;
            dontTrigger = type == TriggerType.TriggerOnce;
            shakes.TurnOffAllShakes();

            if (letterBox != null) letterBox.gameObject.SetActive(true);
            if (timeManager != null && timeManager.slowMotion) timeManager.Reset();
        }

        public bool CompleteCinematic(Vector3 cameraPosition, Vector3 target, bool cineFound, Follow follow,
            Shakes shakes, SlowMotion timeManager)
        {
            if (!cineFound && !activated && !dontTrigger && type != TriggerType.ByScript && targets.Count > 0 &&
                bounds.Contains(target)) TriggerCinematic(shakes, timeManager, cameraPosition);
            if (activated == false) return false;
            for (var i = index; i < targets.Count;)
            {
                if (state == CineState.NextTarget || index < 0) NextTarget(cameraPosition, follow);
                if (index < targets.Count && state != CineState.NextTarget)
                    Follow(targets[index], cameraPosition, follow);
                if (index < targets.Count && state == CineState.Wait && Clock.Timer(ref waitTimer, targets[index].wait))
                    NextTarget(cameraPosition, follow);
                previousTarget = target;
                return true;
            }

            if (state == CineState.Hold)
            {
                if ((previousTarget - target).magnitude > 0.01f)
                {
                    follow.ForceTargetSmooth();
                    state = CineState.Complete;
                }
                else
                {
                    follow.SetCameraPosition(currentPosition);
                    return true;
                }
            }

            if (type == TriggerType.ByScript) return false;
            if (bounds.Contains(target)) return true;

            follow.ForceTargetSmooth();
            return activated = false;
        }

        private void NextTarget(Vector3 cameraPosition, Follow follow)
        {
            index++;
            tweenTimer = waitTimer = 0;
            state = CineState.Follow;
            startPosition = cameraPosition;
            if (index >= targets.Count)
                onComplete.Invoke();
            if (index >= targets.Count)
                state = CineState.Hold; //     
            if (index >= targets.Count - 1)
                bottomClamp.y = currentPosition.y - Height(follow);
            if (index == targets.Count - 1 && letterBox != null)
                letterBox.onEvent.Invoke();
            if (index < targets.Count && targets[index].zoom)
                follow.zoom.Set(targets[index].zoomScale, targets[index].duration); //    zoom if target is available
        }

        private void Follow(CinematicTarget target, Vector3 cameraPosition, Follow follow)
        {
            tweenTimer += Time.deltaTime;
            var ease = EasingFunction.Run(target.tween, tweenTimer / target.time);
            var offsetY =
                followCenter && targets.Count > 0
                    ? Vector2.zero
                    : Vector2.up * Height(follow); //follow.zoom.realTimeHeight;
            currentPosition = Vector2.Lerp(startPosition, Position(target) + offsetY, ease);
            if (target.followX != CineFollow.Normal)
            {
                if (target.followX == CineFollow.Ignore)
                    currentPosition.x = cameraPosition.x;
                if (target.followX == CineFollow.Instant)
                    currentPosition.x = Position(target).x;
            }

            if (target.followY != CineFollow.Normal)
            {
                if (target.followY == CineFollow.Ignore)
                    currentPosition.y = cameraPosition.y;
                if (target.followY == CineFollow.Instant)
                    currentPosition.y = Position(target).y + offsetY.y;
            }

            if (target.zoom && targets.Count > 0 && target.returnToOrigin && pivot == CinePivot.BottomCenter &&
                index == targets.Count - 1)
            {
                // when zooming during last target, don't let zoom get below previous clamp
                var direction = Position(target).y >= currentPosition.y ? 1f : -1f;
                var cameraBottom = currentPosition.y - Height(follow);
                if (direction > 0 && cameraBottom <= bottomClamp.y) currentPosition.y = bottomClamp.y + Height(follow);
            }

            follow.SetCameraPosition(currentPosition);

            if (tweenTimer >= target.duration &&
                state == CineState.Follow) target.onArrival.Invoke(); //                               tween complete
            if (tweenTimer >= target.duration && state == CineState.Follow)
                state = target.halt ? CineState.Halt : CineState.Wait;
        }

        private Vector2 Position(CinematicTarget target)
        {
            return index == targets.Count - 1 && target.returnToOrigin ? enterPosition : target.position;
        }

        public float Height(Follow follow)
        {
            return follow.pixelPerfect != null ? follow.pixelPerfect.Height() : follow.zoom.realTimeHeight;
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public int signalIndex;
        [SerializeField] [HideInInspector] public bool eventsFoldOut;
        [SerializeField] [HideInInspector] public bool beginFoldOut;
        [SerializeField] [HideInInspector] public bool completeFoldOut;
        [SerializeField] [HideInInspector] public bool active;
        [SerializeField] [HideInInspector] public bool add;
        [SerializeField] [HideInInspector] public bool open;
        [SerializeField] [HideInInspector] public int select;
#pragma warning restore 0414
#endif

        #endregion
    }

    [Serializable]
    public class CinematicTarget
    {
        [SerializeField] public Tween tween;
        [SerializeField] public Vector2 position;
        [SerializeField] public CineFollow followX;
        [SerializeField] public CineFollow followY;
        [SerializeField] public float wait;
        [SerializeField] public float duration = 1f;
        [SerializeField] public float zoomScale = 1f;
        [SerializeField] public bool zoom;
        [SerializeField] public bool halt;
        [SerializeField] public bool returnToOrigin;
        [SerializeField] public UnityEvent onArrival;
        public float time => duration <= 0 ? 0.0001f : duration;

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool eventsFoldOut;
        [SerializeField] [HideInInspector] public bool arrivalFoldOut;
#pragma warning restore 0414
#endif

        #endregion
    }

    public enum CineState
    {
        NextTarget,
        Follow,
        Wait,
        Halt,
        Hold,
        Complete
    }

    public enum TriggerType
    {
        TriggerOnce,
        Trigger,
        ByScript
    }

    public enum TriggerOnce
    {
        Trigger,
        TriggerOnce
    }

    public enum CinePivot
    {
        Center,
        BottomCenter
    }

    public enum CineFollow
    {
        Normal,
        Instant,
        Ignore
    }
}