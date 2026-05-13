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
    public class HighlightTarget
    {
        [SerializeField] public bool enable;
        [SerializeField] public List<Highlight> targets = new();
        [NonSerialized] public bool active;
        [NonSerialized] private float counter;
        [NonSerialized] private float duration;
        [NonSerialized] public bool exitOnMove;
        [NonSerialized] private bool ignoreClamps;
        [NonSerialized] private Vector3 position;
        [NonSerialized] private Vector3 previousFollowTarget;
        [NonSerialized] private Vector3 previousMainTarget;
        [NonSerialized] private float range;
        [NonSerialized] public float speed;
        [NonSerialized] private bool stay;
        [NonSerialized] private Transform transform;

        [NonSerialized] private TargetTypes type;
        [NonSerialized] private float yOffset;

        public void Reset()
        {
            active = false;
            exitOnMove = false;
        }

        public void ExitOnMove()
        {
            exitOnMove = true;
        }

        public Vector3 Velocity(Vector3 target, Vector3 cameraVelocity, Follow follow)
        {
            if (!enable) return cameraVelocity;
            if (!active)
                return follow.rooms.ignoreClamps
                    ? Follow(follow, follow.rooms.RoomTarget(target, follow))
                    : cameraVelocity;
            if (!stay && Clock.Timer(ref counter, duration))
            {
                Reset();
                follow.ForceTargetSmooth();
            }

            var followTarget = type == TargetTypes.Position ? position :
                transform != null ? transform.position : target;
            followTarget += Vector3.up * yOffset;
            if (exitOnMove && Mathf.Abs(target.x - previousMainTarget.x) > 0.01f)
            {
                Reset();
                follow.ForceTargetSmooth();
            }

            previousMainTarget = target;
            if (range > 0)
            {
                var direction = target - followTarget;
                followTarget += direction.normalized * Mathf.Min(range, direction.magnitude);
            }

            return Follow(follow, followTarget);
        }

        private Vector3 Follow(Follow follow, Vector2 followTarget)
        {
            var newCameraPosition = Compute.LerpToTarget(follow.cameraTransform.position, followTarget,
                ref previousFollowTarget, 0.9f, 0.9f, speed);
            previousFollowTarget = followTarget;
            return newCameraPosition - follow.cameraTransform.position;
        }

        public void Trigger(string targetName, Follow follow)
        {
            for (var i = 0; i < targets.Count; i++)
                if (targets[i].name == targetName)
                {
                    Set(targets[i].type, targets[i].transform, targets[i].position, targets[i].duration,
                        targets[i].speed, targets[i].yOffset, targets[i].range, targets[i].stay,
                        targets[i].ignoreClamps, follow);
                    return;
                }
        }

        public void Set(TargetTypes type, Transform transform, Vector3 position, float duration, float speed,
            float yOffset, float range = 0, bool stay = false, bool ignoreClamps = true, Follow follow = null)
        {
            this.ignoreClamps = ignoreClamps;
            this.transform = transform;
            this.position = position;
            this.duration = duration;
            this.yOffset = yOffset;
            this.speed = speed;
            this.range = range;
            this.stay = stay;
            this.type = type;
            counter = 0;
            active = true;
            follow.rooms.ignoreClamps = ignoreClamps;
            previousFollowTarget = type == TargetTypes.Position ? position :
                transform != null ? transform.position : Vector3.zero;
        }

        [Serializable]
        public class Highlight
        {
            [SerializeField] public TargetTypes type;
            [SerializeField] public Transform transform;
            [SerializeField] public Vector2 position;
            [SerializeField] public bool ignoreClamps;
            [SerializeField] public bool stay;
            [SerializeField] public string name;
            [SerializeField] public float yOffset;
            [SerializeField] public float range;
            [SerializeField] public float speed = 10f;
            [SerializeField] public float duration = 1f;
            [SerializeField] public bool open;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool add;
        [SerializeField] [HideInInspector] public bool edit;
        [SerializeField] [HideInInspector] public bool close;
        [SerializeField] [HideInInspector] public bool foldOut;
        public static void CustomInspector(SerializedProperty parent, Color barColor, Color labelColor)
        {
            if (!parent.Bool("edit"))
                return;

            if (TwoBitMachines.Safire2DCamera.Follow.Open(parent, "Highlight Target", barColor, labelColor, true))
            {
                var array = parent.Get("targets");
                if (parent.ReadBool("add")) array.arraySize++;

                if (array.arraySize == 0)
                    Layout.VerticalSpacing(5);

                GUI.enabled = parent.Bool("enable");
                for (var i = 0; i < array.arraySize; i++)
                {
                    var element = array.Element(i);
                    var type = element.Enum("type");

                    FoldOut.BoxSingle(1, FoldOut.boxColor);
                    {
                        Fields.ConstructField();
                        element.ConstructField("name", S.FW - S.B2);
                        if (Fields.ConstructButton("Delete"))
                        {
                            array.DeleteArrayElement(i);
                            break;
                        }

                        if (Fields.ConstructButton("Reopen")) element.Toggle("open");
                    }
                    Layout.VerticalSpacing(2);

                    if (!element.Bool("open"))
                        continue;

                    FoldOut.Box(5, FoldOut.boxColor, offsetY: -2);
                    {
                        element.FieldDouble("type", "transform", "speed", type == 1, true);
                        element.FieldDouble("type", "position", "speed", type == 0, true);
                        Labels.FieldText("Speed");
                        element.FieldAndDisable("Duration", "Stay", "duration", "stay");
                        element.Field("Y Offset", "yOffset");
                        element.Field("Target Range", "range");
                        element.FieldToggle("Ignore Clamps", "ignoreClamps");
                    }
                    Layout.VerticalSpacing(3);
                }

                GUI.enabled = true;
            }

            ;
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    public enum TargetTypes
    {
        Position,
        Transform
    }
}