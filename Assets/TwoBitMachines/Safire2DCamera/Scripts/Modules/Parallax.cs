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
    public class Parallax
    {
        [SerializeField] public bool enable;
        [SerializeField] public List<ParallaxLayer> parallax = new();
        [NonSerialized] private Camera mainCamera;
        [NonSerialized] private Vector3 previousCameraPosition;

        [NonSerialized] private Transform transform;

        public void Initialize(Camera camera)
        {
            mainCamera = camera;
            transform = camera.transform;
            for (var i = 0; i < parallax.Count; i++) parallax[i].Create();
            previousCameraPosition = transform.position;
            Reset();
        }

        public void Reset()
        {
            for (var j = 0;
                 j < 5;
                 j++) // get parallax to target quicker on scene start or layers will lag behind for a few frames
            for (var i = 0; i < parallax.Count; i++)
                parallax[i].Execute(mainCamera, 0);
        }

        public void Execute(Camera camera)
        {
            if (!enable)
                return;
            var delta = transform.position - previousCameraPosition;
            previousCameraPosition = transform.position;
            for (var i = 0; i < parallax.Count; i++) parallax[i].Execute(camera, delta.x);
        }

        public void RefreshParallaxImage(int index)
        {
            for (var i = 0; i < parallax.Count; i++)
                if (i == index)
                {
                    parallax[i].RefreshImages();
                    break;
                }
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

            if (Follow.Open(parent, "Infinite Parallax", barColor, labelColor, true))
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

                    FoldOut.Box(1, FoldOut.boxColor, offsetY: -2);
                    {
                        element.Field("Auto Scroll", "autoScroll");
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
    public class ParallaxLayer
    {
        [SerializeField] public Transform transform;
        [SerializeField] public Vector2 parallaxRate;
        [SerializeField] public float autoScroll;
        [SerializeField] [HideInInspector] private bool open;
        [NonSerialized] private GameObject extendLeft;

        [NonSerialized] private GameObject extendRight;
        [NonSerialized] private float length;
        [NonSerialized] private float previousCenter;
        [NonSerialized] private float scroll;
        [NonSerialized] private float startPositionX;
        [NonSerialized] private float startPositionY;

        public void Create()
        {
            if (transform == null)
                return;

            var renderer = transform.GetComponent<SpriteRenderer>();
            if (renderer == null)
                return;

            startPositionX = transform.position.x;
            startPositionY = transform.position.y;

            extendRight = MonoBehaviour.Instantiate(transform.gameObject, Vector3.zero, Quaternion.identity);
            extendLeft = MonoBehaviour.Instantiate(transform.gameObject, Vector3.zero, Quaternion.identity);
            extendRight.transform.parent = transform;
            extendLeft.transform.parent = transform;
            extendRight.transform.localPosition = Vector3.zero;
            extendLeft.transform.localPosition = Vector3.zero;

            length = renderer.bounds.size.x;
            var realLength = length / transform.localScale.x;

            SetLocalPositionX(extendRight.transform, realLength);
            SetLocalPositionX(extendLeft.transform, -realLength);
            scroll = 0;
        }

        //* foreground is negative numbers larger than 1, regular is between 0-1
        public void Execute(Camera camera, float deltaX)
        {
            if (transform == null)
                return;

            if (camera.orthographic)
            {
                if (autoScroll != 0)
                {
                    scroll += Time.deltaTime * autoScroll + (1 - parallaxRate.x) * -deltaX;
                    scroll = scroll > length ? 0 : scroll < -length ? 0 : scroll;
                }

                var cameraX = camera.transform.position.x;
                var distanceX = cameraX * parallaxRate.x;
                SetPositionX(transform, startPositionX + distanceX + scroll);

                var cameraY = camera.transform.position.y;
                var distanceY = cameraY * parallaxRate.y;
                SetPositionY(transform, startPositionY + distanceY);

                var limit = cameraX * (1f - parallaxRate.x);
                if (limit > startPositionX + length + scroll) //                   right
                    startPositionX += length;
                else if (limit < startPositionX - length + scroll) //              left
                    startPositionX -= length;
            }
            else // 3D camera
            {
                scroll = autoScroll != 0 ? Time.deltaTime * autoScroll - deltaX : 0;
                SetPositionX(transform, transform.position.x + scroll);
                Vector2 currentPosition = transform.position;
                var centerX = camera.transform.position.x;
                var velX = centerX - previousCenter;
                var width = FrustumWidth(camera) * 0.5f;
                var right = centerX + width;
                var left = centerX - width;
                previousCenter = centerX;

                if ((velX > 0 || scroll < 0) && right > currentPosition.x + length) //    right
                    SetPositionX(transform, currentPosition.x + length);
                else if ((velX < 0 || scroll > 0) && left < currentPosition.x - length) // left
                    SetPositionX(transform, currentPosition.x - length);
            }
        }

        public void RefreshImages()
        {
            var renderer = transform.GetComponent<SpriteRenderer>();
            extendRight.GetComponent<SpriteRenderer>().sprite = renderer.sprite;
            extendLeft.GetComponent<SpriteRenderer>().sprite = renderer.sprite;
        }

        private void SetPositionX(Transform transform, float positionX)
        {
            transform.position = new Vector3(positionX, transform.position.y, transform.position.z);
        }

        private void SetPositionY(Transform transform, float positionY)
        {
            transform.position = new Vector3(transform.position.x, positionY, transform.position.z);
        }

        private void SetLocalPositionX(Transform transform, float positionX)
        {
            if (transform == null)
                return;
            transform.localPosition = new Vector3(positionX, transform.localPosition.y, transform.localPosition.z);
        }

        private void SetLocalPositionY(Transform transform, float positionY)
        {
            if (transform == null)
                return;
            transform.localPosition = new Vector3(transform.localPosition.x, positionY, transform.localPosition.z);
        }

        private float Distance(Camera cam)
        {
            return Mathf.Abs(transform.position.z - cam.transform.position.z);
        }

        private float FrustumHeight(Camera cam)
        {
            return 2f * Distance(cam) * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        private float FrustumWidth(Camera cam)
        {
            return FrustumHeight(cam) * cam.aspect;
        }
    }
}