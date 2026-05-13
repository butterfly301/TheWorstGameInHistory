using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

namespace TwoBitMachines.Safire2DCamera
{
    [Serializable]
    public class Shakes
    {
        [SerializeField] public bool enable;
        [SerializeField] public ShakeStateSaved shake;
        [NonSerialized] public Transform camera;
        [NonSerialized] public Shake constantShake = new();

        [NonSerialized] private AnimationCurve damper = new(new Keyframe(0f, 1f), new Keyframe(0.9f, .33f, -2f, -2f),
            new Keyframe(1f, 0f, -5.65f, -5.65f));

        [NonSerialized] public Vector3 previousAngle;
        [NonSerialized] public Vector3 previousPosition;

        [NonSerialized] private AnimationCurve recoil = new(new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(0.05f, 1f, -7f, -7f), new Keyframe(0.5f, 0f, 0.0f, 0.0f));

        [NonSerialized] public List<Shake> shakes = new();
        [NonSerialized] public Shake singleShake = new();

        public void Initialize(Transform camera)
        {
            this.camera = camera;
            shakes.Clear();
            Reset();
        }

        public void Reset()
        {
            previousPosition = camera.position;
            previousAngle = camera.eulerAngles;
            if (enable)
                TurnOffAllShakes(true);
        }

        public void RestoreCameraValues()
        {
            camera.position = previousPosition; // isolate camera movement, so that shake does not displace camera
            camera.eulerAngles = previousAngle;
        }

        public void Set(ShakeType type, float speed, float strength, float duration, bool constant, TimeScale timeScale,
            Vector3 offset)
        {
            if (!enable || Time.timeScale == 0)
                return;

            if (type == ShakeType.SingleShake)
            {
                singleShake.Set(type, speed, strength, duration, false, timeScale, offset);
                return;
            }

            if (constant && type != ShakeType.OneShot)
            {
                constantShake.Set(type, speed, strength, duration, true, timeScale, offset);
                return;
            }

            for (var i = 0; i < shakes.Count; i++)
                if (!shakes[i].active)
                {
                    shakes[i].Set(type, speed, strength, duration, false, timeScale, offset);
                    return;
                }

            if (shakes.Count < 20) //shake list full, create new, pool limit is 20 
            {
                var newShake = new Shake();
                newShake.Set(type, speed, strength, duration, false, timeScale, offset);
                shakes.Add(newShake);
                return;
            }

            for (var i = 0;
                 i < shakes.Count;
                 i++) //if shake limit reached, override existing shake that has a smaller offset
                if (offset.sqrMagnitude > shakes[i].position.sqrMagnitude)
                {
                    shakes[i].Set(type, speed, strength, duration, false, timeScale, offset);
                    return;
                }

            shakes[0].Set(type, speed, strength, duration, false, timeScale,
                offset); // if nothing found, override first
        }

        public void Shake(string name)
        {
            if (shake == null)
                return;
            for (var i = 0; i < shake.shakes.Count; i++)
                if (shake.shakes[i].name.Equals(name))
                {
                    var s = shake.shakes[i];
                    Set(s.shakeType, s.speed, s.strength, s.duration, s.constant, s.timeScale, s.amplitude);
                    return;
                }
        }

        public void Execute()
        {
            previousPosition = camera.position;
            previousAngle = camera.eulerAngles;

            if (!enable || Time.timeScale == 0)
                return;

            var position = Vector3.zero;
            for (var i = 0; i < shakes.Count; i++)
                if (shakes[i].active)
                    position += shakes[i].ShakeNow(damper, recoil);

            if (constantShake.active) position += constantShake.ShakeNow(damper, recoil);
            if (singleShake.active) position += singleShake.ShakeNow(damper, recoil);
            camera.position += Tooly.SetPosition(position.x, position.y, Vector3.zero);
            camera.eulerAngles += Tooly.SetDepth(Vector3.zero, position.z);
        }

        public void TurnOffAllShakes(bool immediate = false)
        {
            for (var i = 0; i < shakes.Count; i++)
                shakes[i].active = false;
            constantShake.TurnOff(immediate);
            singleShake.TurnOff(immediate);
        }

        public void TurnOffConstantShake()
        {
            constantShake.TurnOff();
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool add;
        [SerializeField] [HideInInspector] public bool edit;
        [SerializeField] [HideInInspector] public bool close;
        [SerializeField] [HideInInspector] public bool foldOut;

        public static void CustomInspector(SerializedProperty parent, Color barColor, Color labelColor, Shakes shakeRef,
            Safire2DCamera main)
        {
            if (!parent.Bool("edit"))
                return;

            if (Follow.Open(parent, "Shakes", barColor, labelColor, true))
            {
                GUI.enabled = parent.Bool("enable");

                FoldOut.BoxSingle(1, Tint.Orange);
                parent.Field("Shake SO", "shake");
                Layout.VerticalSpacing(2);

                var shake = parent.Get("shake");
                if (shake == null || shake.objectReferenceValue == null)
                    return;

                var shakeObj = new SerializedObject(shake.objectReferenceValue);
                shakeObj.Update();
                {
                    var shakes = shakeObj.Get("shakes");

                    if (parent.ReadBool("add")) shakes.arraySize++;

                    for (var i = 0; i < shakes.arraySize; i++)
                    {
                        var element = shakes.Element(i);

                        FoldOut.BoxSingle(1, Tint.Orange);
                        {
                            Fields.ConstructField();
                            Fields.ConstructSpace(15);
                            element.ConstructField("name", Layout.labelWidth - 20f, 5f);

                            if (Fields.ConstructButton("Play", Layout.quarter))
                            {
                                if (main.cameraRef == null)
                                    main.cameraRef = main.gameObject.GetComponent<Camera>();
                                if (main.cameraRef != null)
                                {
                                    if (Application.isPlaying)
                                    {
                                        shakeRef.ShakeTestMode(shakeRef.shake.shakes, element.Get("name").stringValue);
                                    }
                                    else
                                    {
                                        if (shakeRef.shakes.Count > 0)
                                            shakeRef.StopTestMode();

                                        shakeRef.Initialize(main.cameraRef.transform);
                                        shakeRef.singleShake.TurnOff();
                                        shakeRef.constantShake.TurnOff();
                                        shakeRef.ShakeTestMode(shakeRef.shake.shakes, element.Get("name").stringValue);
                                        EditorApplication.update -= shakeRef.TestMode;
                                        EditorApplication.update += shakeRef.TestMode;
                                        Clock.Initialize();
                                    }
                                }
                            }

                            if (Fields.ConstructButton("Red", Layout.quarter) && shakeRef != null)
                                shakeRef.StopTestMode();
                            if (Fields.ConstructButton("Delete", Layout.quarter))
                            {
                                shakes.DeleteArrayElement(i);
                                break;
                            }

                            if (Fields.ConstructButton("Reopen", Layout.quarter)) element.Toggle("open");
                            ListReorder.Grip(shakeObj, shakes, Layout.GetLastRect(20, 20), i, Tint.WarmWhite,
                                yOffset: 2);
                        }
                        Layout.VerticalSpacing(2);
                        if (!element.Bool("open")) continue;

                        FoldOut.Box(5, FoldOut.boxColor, offsetY: -2);
                        {
                            element.FieldDouble("Type", "shakeType", "timeScale");
                            element.Field("Amplitude", "amplitude"); // z-axis if for angle!
                            element.Slider("Speed", "speed");
                            element.Slider("Strength", "strength");
                            element.FieldAndDisable("Duration", "Constant", "duration", "constant");
                        }
                        Layout.VerticalSpacing(3);
                    }

                    GUI.enabled = true;
                }
                shakeObj.ApplyModifiedProperties();
            }
        }

        public void TestMode()
        {
            Clock.SimulateTimeEditor();
            RestoreCameraValues();
            Execute();
            if (AllShakesCompleteTestMode(shakes) && singleShake.trauma <= 0)
                StopTestMode();
        }

        public void StopTestMode()
        {
            shakes.Clear();
            singleShake.TurnOff();
            constantShake.TurnOff();
            constantShake.active = false;
            if (camera != null)
                RestoreCameraValues();
            EditorApplication.update -= TestMode;
        }

        public void ShakeTestMode(List<ShakeInfo> shakeList, string name)
        {
            for (var i = 0; i < shakeList.Count; i++)
                if (name == shakeList[i].name)
                {
                    var s = shakeList[i];
                    if (shakes != null)
                        Set(s.shakeType, s.speed, s.strength, s.duration, s.constant, TimeScale.TimeUnscaled,
                            s.amplitude);
                    return;
                }
        }

        public bool AllShakesCompleteTestMode(List<Shake> shakeThis)
        {
            if (constantShake.active)
                return false;
            for (var i = shakeThis.Count - 1; i >= 0; i--)
                if (shakeThis[i].active)
                    return false;
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    [Serializable]
    public class ShakeInfo
    {
        [SerializeField] public TimeScale timeScale;
        [SerializeField] public ShakeType shakeType;
        [SerializeField] public string name = "New";
        [SerializeField] public float speed = 0.10f;
        [SerializeField] public float strength = 1f;
        [SerializeField] public float duration = 1f;
        [SerializeField] public Vector3 amplitude;
        [SerializeField] public bool constant;
        [SerializeField] [HideInInspector] public bool open;
    }

    public class Shake
    {
        public bool active;
        private Vector3 amplitude;
        private bool constant;
        private float duration;
        private int easeIn;

        public Vector3 position;

        private ShakeType shakeType;

        private float smoothStep;
        private float speed;
        private float strength;
        private Vector3 target;
        private TimeScale timeScale;
        public float trauma;
        private float timeDelta => timeScale == TimeScale.TimeScale ? Time.deltaTime : Time.unscaledDeltaTime;
        private bool shakeComplete => !constant && percentComplete >= 1;
        private float percentComplete => smoothStep / duration;
        private float random => Random.Range(-1, 1);

        public void TurnOff(bool immediate = false)
        {
            if (!active)
                return;
            if (immediate)
                active = false;
            duration = 1f;
            constant = false;
            trauma = smoothStep = 0;
            amplitude = target = position = Vector3.zero;
        }

        public virtual void Set(ShakeType shakeType, float speed, float strength, float duration, bool constant,
            TimeScale timeScale, Vector3 amplitude)
        {
            active = true;
            easeIn = 0;
            smoothStep = 0;
            this.speed = speed;
            this.amplitude = amplitude;
            this.strength = strength;
            this.duration = duration;
            this.constant = constant;
            this.shakeType = shakeType;
            this.timeScale = timeScale;
            target = Vector3.zero;
            position = Vector3.zero; // position z is angle
            trauma = Mathf.Min(trauma + strength * 0.5f, 1f);
            trauma = trauma < 0.5f
                ? 0.5f
                : trauma; // keep trauma at a base level since it'll be too small to notice otherwise
            if (shakeType == ShakeType.OneShot)
                this.amplitude = ((Vector2)amplitude).normalized * -1f;
            if (shakeType == ShakeType.Sine)
                target = new Vector3(0, -Random.Range(0, 360f), Random.Range(0, 1000f));
        }

        public Vector3 ShakeNow(AnimationCurve damper, AnimationCurve recoil)
        {
            var deltaTime = timeDelta;
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                deltaTime = Clock.unscaledDeltaTime;
#endif
            smoothStep += deltaTime;
            var curve = !constant ? damper.Evaluate(percentComplete) * strength : 1 * strength;

            if (shakeType == ShakeType.SingleShake)
            {
                trauma = Mathf.Max(trauma - deltaTime * duration, 0); // duration is now decay
                if ((position - target).sqrMagnitude < 0.01f)
                    target = new Vector3(random * amplitude.x, random * amplitude.y, random * amplitude.z) *
                             Mathf.Pow(trauma, 2f);
                position = Vector3.Lerp(position, target, deltaTime * speed * 100f);
                active = trauma > 0;
                return position;
            }

            if (shakeType == ShakeType.Perlin)
            {
                position.x = (Mathf.PerlinNoise(smoothStep * speed * 10f, 0f) * 2f - 1f) * curve * amplitude.x;
                position.y = (Mathf.PerlinNoise(0, smoothStep * speed * 10f) * 2f - 1f) * curve * amplitude.y;
                position.z = (Mathf.PerlinNoise(0, smoothStep * speed * 10f) * 2f - 1f) * curve * amplitude.z;
                if (++easeIn < 5f)
                    position *= easeIn / 4f; // ease in for 4 frames, prevents hard jump.
            }
            else if (shakeType == ShakeType.Random)
            {
                if ((position - target).sqrMagnitude < 0.01f)
                    target = new Vector3(random * amplitude.x, random * amplitude.y, random * amplitude.z) * curve;
                position = Vector3.Lerp(position, target, deltaTime * speed * 100f);
            }
            else if (shakeType == ShakeType.Sine)
            {
                var current = speed * 100f * (smoothStep * Vector3.one + target);
                position.x = Mathf.Sin(current.x) * curve * amplitude.x;
                position.y = Mathf.Sin(current.y) * curve * amplitude.y;
                position.y = Mathf.Sin(current.z) * curve * amplitude.z;
            }
            else //     one shot
            {
                position = amplitude * recoil.Evaluate(smoothStep / duration) * strength;
            }

            active = !shakeComplete;
            return position;
        }
    }

    public enum ShakeType
    {
        Random,
        Perlin,
        Sine,
        OneShot,
        SingleShake
    }
}