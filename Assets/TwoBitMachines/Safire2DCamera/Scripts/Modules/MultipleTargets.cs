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
    public class MultipleTargets
    {
        [SerializeField] public bool enable;
        [SerializeField] public float detect = 2f;
        [SerializeField] public bool zoomToFit;
        [SerializeField] public List<Targets> targets = new();
        [NonSerialized] private Bounds bounds;
        [NonSerialized] private float cameraAspect = 1;

        [NonSerialized] private Zoom zoom;

        public void Initialize(Zoom zoom, Bounds bounds)
        {
            this.zoom = zoom;
            this.bounds = bounds;
            cameraAspect = zoom.originalHeight / zoom.originalWidth;
        }

        public Vector3 FollowMultipleTargets(Vector3 target, bool ignoreClamps)
        {
            if (targets.Count == 0)
                return target;

            Vector3 furthest = Vector2.one * -Mathf.Infinity;
            Vector3 closest = Vector2.one * Mathf.Infinity;
            FindFurthestPoints(target, ref furthest, ref closest);

            for (var i = targets.Count - 1; i >= 0; i--)
                if (targets[i] != null && targets[i].transform != null &&
                    targets[i].transform.gameObject.activeInHierarchy)
                    FindFurthestPoints(targets[i].position, ref furthest, ref closest);

            Vector3 size = new Vector2(Mathf.Abs(furthest.x - closest.x), Mathf.Abs(furthest.y - closest.y)) * 0.5f;
            if (zoomToFit && !ignoreClamps)
                ZoomToFit(size);
            return closest + size;
        }

        private void ZoomToFit(Vector2 size)
        {
            var zoomToTargets = Mathf.Max(size.x / zoom.originalWidth, size.y / zoom.originalHeight);
            var zoomToRoom = Mathf.Min(bounds.width / zoom.originalWidth, bounds.height / zoom.originalHeight);
            var finalZoom = Mathf.Min(zoomToTargets, zoomToRoom);
            zoom.Set(Mathf.Clamp(finalZoom, 1f, 1000f), speed: 0.95f, isTween: false, run: true);
        }

        private void FindFurthestPoints(Vector2 target, ref Vector3 furthest, ref Vector3 closest)
        {
            if (target.x > furthest.x - detect)
                furthest.x = target.x + detect;
            if (target.y > furthest.y - detect)
                furthest.y = target.y + detect;
            if (target.x < closest.x + detect)
                closest.x = target.x - detect;
            if (target.y < closest.y + detect)
                closest.y = target.y - detect;
        }

        public void AddTarget(Transform transform, Vector2 offset)
        {
            targets.Add(new Targets(transform, offset));
        }

        public void RemoveTarget(Transform transform)
        {
            for (var i = targets.Count - 1; i >= 0; i--)
                if (targets[i].transform == transform)
                    targets.RemoveAt(i);
        }

        [Serializable]
        public class Targets
        {
            [SerializeField] public Transform transform;
            [SerializeField] public Vector2 offset;

            public Targets(Transform transform, Vector2 offset)
            {
                this.transform = transform;
                this.offset = offset;
            }

            public Vector3 position => transform.position + (Vector3)offset;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool add;
        [SerializeField] [HideInInspector] public bool foldOut;
        public static void CustomInspector(SerializedProperty parent, Color color)
        {
            if (FoldOut.Bar(parent, color).Label("Multiple Targets", FoldOut.titleColor, false).BRE()
                .BR(execute: parent.Bool("enable")).FoldOut())
            {
                GUI.enabled = parent.Bool("enable");
                FoldOut.Box(2, color, offsetY: -2);
                {
                    parent.Field("Early Detect", "detect");
                    parent.Field("Zoom To Fit", "zoomToFit");
                }
                Layout.VerticalSpacing(3);

                var targets = parent.Get("targets");
                if (parent.ReadBool("add")) targets.arraySize++;
                for (var i = 0; i < targets.arraySize; i++)
                {
                    var element = targets.Element(i);
                    FoldOut.Box(1, color, offsetY: -2);
                    {
                        Fields.ConstructField();
                        element.ConstructField("transform", Layout.labelWidth);
                        element.ConstructField("offset", Layout.contentWidth - Layout.buttonWidth);
                        if (Fields.ConstructButton("Delete"))
                        {
                            targets.DeleteArrayElement(i);
                            break;
                        }
                    }
                    Layout.VerticalSpacing(3);
                }

                GUI.enabled = true;
            }
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}