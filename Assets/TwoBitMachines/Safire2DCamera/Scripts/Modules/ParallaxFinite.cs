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
    public class ParallaxFinite
    {
        public static ParallaxFinite get;
        [SerializeField] public bool enable;
        [SerializeField] public List<ParallaxFiniteLayer> parallax = new();

        public void Initialize()
        {
            get = this;
            for (var i = 0; i < parallax.Count; i++) parallax[i].Initialize();
        }

        public void Reset()
        {
            for (var i = 0; i < parallax.Count; i++) parallax[i].Reset();
        }

        public void Execute(Camera camera, Follow follow)
        {
            if (!enable)
                return;

            for (var i = 0; i < parallax.Count; i++) parallax[i].Execute(camera, follow);
        }

        public static void
            AddParallax(Transform transform, Vector2 rate, Vector2 offset,
                bool mustBeVisible) // There should only be one camera in the scene if using this method
        {
            if (get == null)
                return;
            var newLayer = new ParallaxFiniteLayer();
            newLayer.Initialize(transform, rate, offset, mustBeVisible);
            get.parallax.Add(newLayer);
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public int signalIndex;
        [SerializeField] [HideInInspector] public bool foldOut;
        [SerializeField] [HideInInspector] public bool active;
        [SerializeField] [HideInInspector] public bool close;
        [SerializeField] [HideInInspector] public bool edit;
        [SerializeField] [HideInInspector] public bool add;

        public static void CustomInspector(SerializedProperty parent, Color barColor, Color labelColor)
        {
            if (!parent.Bool("edit"))
                return;

            if (Follow.Open(parent, "Finite Parallax", barColor, labelColor, true))
            {
                GUI.enabled = parent.Bool("enable");

                var array = parent.Get("parallax");

                if (parent.ReadBool("add")) array.arraySize++;

                if (array.arraySize == 0)
                    Layout.VerticalSpacing(5);

                for (var i = 0; i < array.arraySize; i++)
                {
                    var element = array.Element(i);
                    FoldOut.BoxSingle(1, Tint.Blue);
                    {
                        Fields.ConstructField();
                        Fields.ConstructSpace(15);
                        element.ConstructField("transform", S.FW * 0.32f);
                        element.ConstructField("parallaxRate", S.FW * 0.68f - S.B2 - 20f, 5f);
                        if (Fields.ConstructButton("Delete"))
                        {
                            array.DeleteArrayElement(i);
                            break;
                        }

                        if (Fields.ConstructButton("Reopen")) element.Toggle("open");
                        ListReorder.Grip(parent, array, Layout.GetLastRect(20, 20), i, Tint.WarmWhite, yOffset: 2);
                    }
                    Layout.VerticalSpacing(2);

                    if (!element.Bool("open")) continue;

                    FoldOut.Box(2, FoldOut.boxColor, offsetY: -2);
                    {
                        element.Field("Starting Offset", "startingOffset");
                        //element.FieldAndEnable ("Max Distance", "maxDistance", "clampDistance");
                        element.FieldToggleAndEnable("Must Be Visible", "mustBeVisible");
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

    [Serializable]
    public class ParallaxFiniteLayer
    {
        [SerializeField] public Transform transform;
        [SerializeField] public Vector2 parallaxRate;
        [SerializeField] public Vector2 startingOffset;
        [SerializeField] public bool clampDistance;
        [SerializeField] public bool mustBeVisible;
        [SerializeField] public Vector2 maxDistance = new(50f, 0f);
        [SerializeField] [HideInInspector] private bool open;
        [NonSerialized] private SpriteRenderer renderer;
        [NonSerialized] private bool searched;

        [NonSerialized] private float startPositionX;
        [NonSerialized] private float startPositionY;

        public void Initialize(Transform transformRef, Vector2 rate, Vector2 offset, bool mustBeVisibleRef)
        {
            if (transformRef == null)
                return;
            transform = transformRef;
            parallaxRate = rate;
            startingOffset = offset;
            mustBeVisible = mustBeVisibleRef;
            startPositionX = transform.position.x;
            startPositionY = transform.position.y;
        }

        public void Initialize()
        {
            if (transform == null)
                return;
            startPositionX = transform.position.x;
            startPositionY = transform.position.y;
        }

        public void Reset()
        {
            if (transform == null)
                return;
            SetPositionX(transform, startPositionX);
            SetPositionY(transform, startPositionY);
        }

        //* foreground is negative numbers larger than 1, regular is between 0-1
        public void Execute(Camera camera, Follow follow)
        {
            if (transform == null || !camera.orthographic)
                return;

            if (mustBeVisible)
            {
                if (!searched)
                {
                    searched = true;
                    renderer = transform.GetComponent<SpriteRenderer>();
                }

                if (renderer != null)
                {
                    Vector2 target = follow.TargetPosition();
                    var targetLeft = target.x - camera.Width() - 2f;
                    var targetRight = target.x + camera.Width() + 2f;

                    var parallaxLeft = transform.position.x - renderer.bounds.extents.x;
                    var parallaxRight = transform.position.x + renderer.bounds.extents.x;
                    // float y = transform.position.y + 5f;
                    // Debug.DrawLine (new Vector2 (parallaxLeft, y), new Vector2 (parallaxRight, y), Color.red);
                    var leftVisible = targetLeft >= parallaxLeft && targetLeft <= parallaxRight;
                    var rightVisible = targetRight >= parallaxLeft && targetRight <= parallaxRight;
                    var visible = leftVisible || rightVisible;

                    if (!visible) return;
                }
            }

            var distanceX = (camera.transform.position.x - startPositionX) * parallaxRate.x;
            // if (clampDistance && maxDistance.x != 0 && Mathf.Abs (distanceX) > (maxDistance.x * parallaxRate.x))
            // {
            //         distanceX = maxDistance.x * parallaxRate.x * Mathf.Sign (distanceX);
            // }
            SetPositionX(transform, startPositionX + distanceX);

            var distanceY = (camera.transform.position.y - startPositionY) * parallaxRate.y;
            // if (clampDistance && maxDistance.y != 0 && Mathf.Abs (distanceY) > (maxDistance.y * parallaxRate.y))
            // {
            //         distanceY = maxDistance.y * parallaxRate.y * Mathf.Sign (distanceY);
            // }
            SetPositionY(transform, startPositionY + distanceY);
        }

        private void SetPositionX(Transform transform, float positionX)
        {
            transform.position = new Vector3(positionX + startingOffset.x, transform.position.y, transform.position.z);
        }

        private void SetPositionY(Transform transform, float positionY)
        {
            transform.position = new Vector3(transform.position.x, positionY + startingOffset.y, transform.position.z);
        }
    }
}