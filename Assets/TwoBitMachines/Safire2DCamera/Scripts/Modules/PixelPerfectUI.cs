using System;
using UnityEngine;

namespace TwoBitMachines.Safire2DCamera
{
    public class PixelPerfectUI : MonoBehaviour
    {
        [SerializeField] public int PPU = 8;
        [SerializeField] public Vector2Int resolution = new(320, 180);
        [SerializeField] [HideInInspector] private float originSize = 11f;
        [SerializeField] [HideInInspector] private Camera cameraRef;

        [NonSerialized] private int scale = 1;
        public int scaledWidth => resolution.x * scale;
        public int scaledHeight => resolution.y * scale;

        private void Awake()
        {
            if (cameraRef == null)
            {
                cameraRef = gameObject.GetComponent<Camera>();
                originSize = cameraRef.orthographicSize;
            }

            SetPixelCameraView(cameraRef);
        }

        public void Update()
        {
            SetPixelCameraView(cameraRef);
            SnapToPixelGrid();
        }

        private void OnDisable()
        {
            if (cameraRef == null) return;
            cameraRef.rect = new Rect(0, 0, 1f, 1f);
            cameraRef.orthographicSize = originSize;
            cameraRef.ResetAspect();
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
            if (camera == null) return;

            var yScale = Screen.height / resolution.y;
            var xScale = Screen.width / resolution.x;
            scale = Mathf.Max(1, Mathf.Min(yScale, xScale));
            var x = (Screen.width - scaledWidth) * 0.5f;
            var y = (Screen.height - scaledHeight) * 0.5f;
            camera.pixelRect = new Rect(x, y, scaledWidth, scaledHeight);
            camera.orthographicSize = resolution.y / (PPU * 2f);
        }

        public void SnapToPixelGrid()
        {
            var pixelGrid = 1f / (PPU * scale);
            transform.position = Compute.Round(transform.position, pixelGrid);
        }
    }
}