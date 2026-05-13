using System;
using UnityEngine;

namespace TwoBitMachines.Safire2DCamera
{
    [Serializable]
    public class Zoom
    {
        [SerializeField] public ZoomType type;
        [NonSerialized] private bool active;
        [NonSerialized] private float counter;
        [NonSerialized] private bool isTween = true;
        [NonSerialized] public bool lockZoom;
        [NonSerialized] public PixelPerfect pixelPerfect;
        [NonSerialized] public float playerDepth;
        [NonSerialized] private float startScale = 1f;
        [NonSerialized] private float tempScale = 1f;
        [NonSerialized] private bool timeScale = true;

        public Camera camera { get; private set; }
        public float widthOrtho { get; private set; }
        public float heightOrtho { get; private set; }
        public float height3D { get; private set; }
        public float width3D { get; private set; }
        public float scale { get; private set; }
        public float speed { get; private set; }
        public float duration { get; private set; }

        // Original values for resets
        public float orthographicSize { get; private set; }
        public float cameraFOV { get; private set; }
        public float cameraDepth { get; private set; }
        public float cameraDistance { get; private set; }

        public float maxHeight => scale * originalHeight;
        public float maxWidth => scale * originalWidth;
        public float originalWidth => isOrtho ? widthOrtho : width3D;
        public float originalHeight => isOrtho ? heightOrtho : height3D;
        public float realTimeHeight => camera.Height();
        public float realTimeWidth => camera.Width();
        public Vector2 cameraSize => new(camera.Width(), camera.Height());

        public Vector3 cameraPosition => camera.transform.position;
        public float currentCameraDepth => cameraPosition.z;

        private bool isOrtho => camera.orthographic;
        private bool isFOV => !isOrtho && type == ZoomType.FOV;
        private bool isDistance => !isOrtho && type == ZoomType.Distance;
        private float distance => Mathf.Abs(playerDepth - currentCameraDepth);
        private float origin => isOrtho ? orthographicSize : isFOV ? cameraFOV : cameraDepth;
        private float smooth => Compute.Lerp(currentCameraSize, TargetCameraSize(scale), speed); //! RETEST

        private float currentCameraSize =>
            isOrtho ? camera.orthographicSize : isFOV ? camera.fieldOfView : currentCameraDepth;

        private float TargetCameraSize(float scale)
        {
            return isOrtho || isDistance
                ? origin * scale
                : 2.0f * Mathf.Atan(height3D * scale / cameraDistance) *
                  Mathf.Rad2Deg; // calculate field of view angle if in FOV mode
        }

        public void Initialize(Safire2DCamera main)
        {
            Reset(false);
            camera = main.cameraRef;
            pixelPerfect = main.pixelPerfect;
            cameraDepth = currentCameraDepth;
            cameraFOV = camera.fieldOfView;
            orthographicSize = camera.orthographicSize;
            heightOrtho = orthographicSize;
            widthOrtho = camera.aspect * heightOrtho;
            cameraDistance =
                distance > 0 ? distance :
                cameraDepth != 0 ? cameraDepth :
                0.1f; // Initial distance between camera and player should always be greater than zero
            height3D = cameraDistance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            width3D = height3D * camera.aspect;
        }

        public void Reset(bool resetZoom = true)
        {
            scale = 1f;
            speed = 1f;
            counter = 0f;
            tempScale = 1f;
            startScale = 1f;
            active = false;
            isTween = false;
            lockZoom = false;

            if (resetZoom)
                ZoomCamera(scale, true);
        }

        public void Set(float scale, float duration = 1, float speed = 1, bool isTween = true, bool timeScale = true,
            bool run = false) // newest zoom overrides previous zoom if zoom not locked
        {
            if (scale <= 0 || lockZoom)
                return;

            counter = 0;
            active = true;
            this.scale = scale;
            this.speed = speed;
            this.isTween = isTween;
            this.timeScale = timeScale;
            startScale = tempScale;
            this.duration = Mathf.Max(duration, 0.00001f);
            if (run)
                Execute(); // keep synced with tween
        }

        public void Execute()
        {
            if (active) ZoomCamera(scale);
            if (pixelPerfect != null) pixelPerfect.zoomScale = tempScale;
        }

        public void ZoomCamera(float scale, bool setImmediately = false)
        {
            if (isOrtho)
                camera.orthographicSize = setImmediately ? TargetCameraSize(scale) : ZoomLerp();
            else if (isFOV)
                camera.fieldOfView = setImmediately ? TargetCameraSize(scale) : ZoomLerp();
            else if (isDistance)
                camera.transform.position =
                    Tooly.SetDepth(cameraPosition, setImmediately ? TargetCameraSize(scale) : ZoomLerp());
        }

        private float ZoomLerp()
        {
            if (isTween)
            {
                counter += timeScale ? Time.deltaTime : Time.unscaledDeltaTime;
                tempScale = Mathf.Lerp(startScale, scale, Mathf.Min(duration, counter) / duration);
                active = !(counter >= duration);
            }
            else
            {
                tempScale = timeScale
                    ? Compute.Lerp(startScale, scale, speed)
                    : Compute.LerpUnscaled(startScale, scale, speed);
            }

            return tempScale * origin;
        }

        public void SetTempScale(float scale)
        {
            tempScale = scale;
        }
    }

    public enum ZoomType
    {
        FOV,
        Distance
    }
}