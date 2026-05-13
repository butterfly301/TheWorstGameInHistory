using System;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    [AddComponentMenu("")]
    public class SlowMotion : ReactionBehaviour
    {
        [NonSerialized] private const float intensityLimit = 0.001f;

        [NonSerialized] public static bool inSlowMotion;
        [NonSerialized] public static SlowMotion slowMotion;

        [SerializeField] [Range(0.001f, 0.999f)]
        public float intensity = 1f;

        [SerializeField] public float duration = 1f;
        [SerializeField] public SlowMotionType type;

        [NonSerialized] private bool active;
        [NonSerialized] private float counter;
        [NonSerialized] private float currentTimeScale = 1f;
        [NonSerialized] private float startValue = 1f;
        [NonSerialized] private bool timeScaleWasPaused;

        public static void Reset()
        {
            inSlowMotion = false;
            slowMotion = null;
        }

        public static void Run() // only one slow motion can run at a time
        {
            if (inSlowMotion && slowMotion != null && slowMotion.Complete()) Reset();
        }

        public override void Activate(ImpactPacket impact)
        {
            slowMotion = this;
            inSlowMotion = true;
            Time.timeScale = Mathf.Clamp(intensity, intensityLimit, 0.99f);
            duration = Mathf.Clamp(duration, 0.001f, Mathf.Abs(duration));

            counter = 0;
            active = true;
            startValue = Time.timeScale;
            currentTimeScale = Time.timeScale;
        }

        public bool Complete()
        {
            if (Time.timeScale == 0)
            {
                timeScaleWasPaused = true;
                return false; // user has paused game, this tool will never set TimeScale = 0
            }

            if (timeScaleWasPaused && active) Time.timeScale = currentTimeScale;

            timeScaleWasPaused = false;
            currentTimeScale = Time.timeScale;

            if (Time.timeScale == 1 || !active) return true;

            counter += Time.unscaledDeltaTime;
            if (type == SlowMotionType.Lerp) Time.timeScale = Mathf.Lerp(startValue, 1f, counter / duration);
            if (counter >= duration)
            {
                Time.timeScale = 1f;
                active = false;
                return true;
            }

            return false;
        }
    }

    public enum SlowMotionType
    {
        Constant,
        Lerp
    }
}