using System;
using UnityEngine;

namespace TwoBitMachines.Safire2DCamera
{
    [ExecuteInEditMode]
    public class ResolutionScalingMonobehaviour : MonoBehaviour
    {
        public enum ResolutionType
        {
            AspectRatio,
            Width,
            Height
        }

        [SerializeField] [HideInInspector] public ResolutionType type;
        [SerializeField] [HideInInspector] public Color color = Color.black;
        [SerializeField] [HideInInspector] public float targetPPU = 8f;
        [SerializeField] [HideInInspector] public float targetFOV = 100f;
        [SerializeField] [HideInInspector] public Vector2 resolution = new(320f, 180f);
        [SerializeField] [HideInInspector] public ResolutionScaling resolutionRef;
        [SerializeField] [HideInInspector] public Camera cam;
        [NonSerialized] private Texture2D border;
        [NonSerialized] private Color colorRef;
        [NonSerialized] private bool isOrtho;

        [NonSerialized] private bool letterbox;
        [NonSerialized] private Vector2 resolutionSize;
        [NonSerialized] private Vector2 screenSize;
        [NonSerialized] private float targetFOVRef;
        [NonSerialized] private float targetPPURef;
        [NonSerialized] private ResolutionType typeRef;

        public void Update()
        {
            Execute();
        }

        private void LateUpdate()
        {
            if (cam == null || type != ResolutionType.AspectRatio) return;

            if (border == null || color != colorRef)
            {
                border = new Texture2D(1, 1, TextureFormat.RGB24, false);
                border.SetPixel(1, 1, color);
                border.Apply();
                colorRef = color;
            }

            GUI.depth = 10;
            if (!letterbox)
            {
                GUI.DrawTexture(new Rect(0, 0, cam.pixelRect.x, cam.pixelRect.height), border);
                GUI.DrawTexture(
                    new Rect(cam.pixelRect.width + cam.pixelRect.x, 0, cam.pixelRect.width, cam.pixelRect.height),
                    border);
            }
            else
            {
                GUI.DrawTexture(new Rect(0, 0, cam.pixelRect.width, cam.pixelRect.y), border);
                GUI.DrawTexture(new Rect(0, Screen.height - cam.pixelRect.y, cam.pixelRect.width, cam.pixelRect.y),
                    border);
            }
        }

        public void Execute(bool enable = true)
        {
            if (cam == null || (resolutionRef != null && !resolutionRef.enable)) return;
            if (screenSize.x == Screen.width && screenSize.y == Screen.height && type == typeRef &&
                isOrtho == cam.orthographic)
                if (resolutionSize.x == resolution.x && resolutionSize.y == resolution.y && targetFOVRef == targetFOV &&
                    targetPPURef == targetPPU)
                    return;

            typeRef = type;
            targetPPURef = targetPPU;
            targetFOVRef = targetFOV;
            isOrtho = cam.orthographic;
            resolutionSize = resolution;
            cam.rect = new Rect(0, 0, 1f, 1f);
            screenSize = new Vector2(Screen.width, Screen.height);

            var targetAspect = resolution.x / resolution.y;
            var currentAspect = screenSize.x / screenSize.y;

            if (type == ResolutionType.AspectRatio)
            {
                if (cam.orthographic)
                {
                    cam.orthographicSize = resolution.y * 0.5f / targetPPU;
                }
                else
                {
                    var distance = Mathf.Abs(cam.transform.position.z);
                    var height = Mathf.Tan(targetFOV * 0.5f * Mathf.Deg2Rad) * distance;
                    cam.fieldOfView = 2.0f * Mathf.Atan(height / distance) * Mathf.Rad2Deg;
                }

                var rect = new Rect(0, 0, 1f, 1f);
                var scaleheight = currentAspect / targetAspect;
                if (scaleheight < 1f)
                {
                    letterbox = true; // letterbox
                    rect.height = scaleheight;
                    rect.y = (1f - scaleheight) * 0.5f;
                }
                else
                {
                    letterbox = false; // pillarbox
                    var scalewidth = 1f / scaleheight;
                    rect.x = (1f - scalewidth) * 0.5f;
                    rect.width = scalewidth;
                }

                cam.rect = rect;
            }
            else if (type == ResolutionType.Width)
            {
                if (cam.orthographic)
                {
                    var desiredHeight = resolution.x / currentAspect;
                    cam.orthographicSize = desiredHeight * 0.5f / targetPPU;
                }
                else
                {
                    var distance = Mathf.Abs(cam.transform.position.z);
                    var height = distance * Mathf.Tan(targetFOV * 0.5f * Mathf.Deg2Rad);
                    cam.fieldOfView = 2.0f * Mathf.Atan(height * (targetAspect / currentAspect) / distance) *
                                      Mathf.Rad2Deg;
                }
            }
            else if (type == ResolutionType.Height)
            {
                if (cam.orthographic)
                {
                    cam.orthographicSize = resolution.y * 0.5f / targetPPU;
                }
                else
                {
                    var distance = Mathf.Abs(cam.transform.position.z);
                    var height = Mathf.Tan(targetFOV * 0.5f * Mathf.Deg2Rad) * distance;
                    cam.fieldOfView = 2.0f * Mathf.Atan(height / distance) * Mathf.Rad2Deg;
                }
            }
        }
    }
}