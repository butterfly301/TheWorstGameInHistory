using System;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

namespace TwoBitMachines.Safire2DCamera
{
    [ExecuteInEditMode]
    public class PixelPerfect : MonoBehaviour
    {
        [SerializeField] public int PPU = 8;
        [SerializeField] public Color color = Color.black;
        [SerializeField] public Vector2Int resolution = new(320, 180);

        [SerializeField] [HideInInspector] private float originSize = 11f;
        [SerializeField] [HideInInspector] private Camera cameraRef;
        [NonSerialized] private int scale = 1;

        [NonSerialized] public float zoomScale = 1f; // set externally
        public int scaledWidth => resolution.x * scale;
        public int scaledHeight => resolution.y * scale;

        private void Awake()
        {
            zoomScale = 1f;
            if (cameraRef == null)
            {
                cameraRef = gameObject.GetComponent<Camera>();
                originSize = cameraRef.orthographicSize;
            }

            SetPixelCameraView(cameraRef);
        }

        private void OnDisable()
        {
            if (cameraRef == null)
                return;
            cameraRef.rect = new Rect(0, 0, 1f, 1f);
            cameraRef.orthographicSize = originSize;
            cameraRef.ResetAspect();
        }

        private void OnPreRender()
        {
            GL.Clear(false, true, color);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            var tempTexture = RenderTexture.GetTemporary(scaledWidth, scaledHeight);
            tempTexture.filterMode = FilterMode.Point;
            source.filterMode = FilterMode.Point;
            Graphics.Blit(source, tempTexture);
            Graphics.Blit(tempTexture, destination);
            RenderTexture.ReleaseTemporary(tempTexture);
        }

        private void SetPixelCameraView(Camera camera)
        {
            if (camera == null)
                return;

            var yScale = Screen.height / resolution.y;
            var xScale = Screen.width / resolution.x;
            scale = Mathf.Max(1, Mathf.Min(yScale, xScale));
            var x = (Screen.width - scaledWidth) * 0.5f;
            var y = (Screen.height - scaledHeight) * 0.5f;
            camera.pixelRect = new Rect(x, y, scaledWidth, scaledHeight);
            camera.orthographicSize = Height(); //resolution.y * zoomScale) / (PPU * 2f);
        }

        public void SnapToPixelGrid()
        {
            var pixelGrid = 1f / (PPU * scale);
            transform.position = Compute.Round(transform.position, pixelGrid);
        }

        public float SnapToPixelGrid(float value)
        {
            var pixelGrid = 1f / (PPU * scale);
            return Compute.Round(value, pixelGrid);
        }

        public float Height()
        {
            return SnapToPixelGrid(resolution.y * zoomScale / (PPU * 2f));
        }

        public void Execute() // make sure this updates as the last script
        {
            SetPixelCameraView(cameraRef);
            SnapToPixelGrid();
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public static void CustomInspector(SerializedObject parent, Color barColor, Color labelColor)
        {
            parent.Update();
            {
                FoldOut.Box(2, Tint.Box);
                Fields.Start(parent, "PPU").F("PPU", S.H).F("color", S.Q).B("Wrench", S.Q, out var pressed);
                if (pressed)
                    RoundAllObjects(parent.Int("PPU"));
                parent.Field("Resolution", "resolution");
                Layout.VerticalSpacing(5);
            }
            parent.ApplyModifiedProperties();
        }

        private void Update()
        {
            if (!EditorApplication.isPlaying)
            {
                Execute();
                hideFlags = HideFlags.HideInInspector;
            }
        }

        public static void RoundAllObjects(float PPU)
        {
            var list = Resources.FindObjectsOfTypeAll(typeof(GameObject));
            var pixelGrid = 1f / PPU;
            for (var i = 0; i < list.Length; i++)
            {
                var o = (GameObject)list[i];
                var newPosition = Round(o.transform.position, pixelGrid);
                o.transform.position = Tooly.SetPosition(newPosition.x, newPosition.y, o.transform.position);
            }

            Debug.Log("Total objects snapped: " + list.Length);
        }

        private static Vector3 Round(Vector3 position, float pixelGrid)
        {
            position.x = Mathf.RoundToInt(position.x / pixelGrid) * pixelGrid;
            position.y = Mathf.RoundToInt(position.y / pixelGrid) * pixelGrid;
            position.z = Mathf.RoundToInt(position.z / pixelGrid) * pixelGrid;
            return position;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}