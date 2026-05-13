#region

using System;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class Shake : Action
    {
        [SerializeField] public ShakeType type;
        [SerializeField] public Vector2 displacement = Vector2.one;
        [SerializeField] public float duration = 1f;
        [SerializeField] public float speed = 1f;

        [SerializeField] [HideInInspector] public AnimationCurve damper = new(new Keyframe(0f, 1f),
            new Keyframe(0.9f, .33f, -2f, -2f), new Keyframe(1f, 0f, -5.65f, -5.65f));

        [NonSerialized] private int easeIn;
        [NonSerialized] private bool shakeActive;
        [NonSerialized] private Vector2 shakeTarget;
        [NonSerialized] private Vector2 shakeVelocity;
        [NonSerialized] private float smoothStep;
        [NonSerialized] private Vector2 startPosition;

        public override NodeState RunNodeLogic(Root root) // Belongs to state
        {
            if (nodeSetup == NodeSetup.NeedToInitialize) ResetShake(root);

            if (shakeActive && Root.deltaTime != 0)
            {
                var resetVelocity = (startPosition - root.position) / Root.deltaTime;
                var shakeVelocity = ShakeNow() / Root.deltaTime;
                root.velocity = shakeVelocity + resetVelocity;
                root.isShaking = true;
            }

            return shakeActive ? NodeState.Running : NodeState.Success;
        }

        public void ResetShake(Root root)
        {
            shakeActive = true;
            smoothStep = easeIn = 0;
            startPosition = root.position;
            shakeTarget = shakeVelocity = Vector2.zero;
            root.previousShakeVel = Vector2.zero;
            if (type == ShakeType.Sine)
                shakeTarget = new Vector2(-Random.Range(0.0f, 360f), Random.Range(0.0f, 1000f));
        }

        public Vector2 ShakeNow()
        {
            smoothStep += Root.deltaTime;
            var percent = smoothStep / duration;
            var curve = damper != null ? damper.Evaluate(percent) : 1f;

            if (type == ShakeType.Random)
            {
                if ((shakeVelocity - shakeTarget).sqrMagnitude < 0.01f)
                    shakeTarget =
                        new Vector2(Random.Range(-1, 1) * displacement.x, Random.Range(-1, 1) * displacement.y) * curve;
                shakeVelocity = Vector3.Lerp(shakeVelocity, shakeTarget, Root.deltaTime * speed * 100f);
            }
            else if (type == ShakeType.Perlin)
            {
                shakeVelocity.x = (Mathf.PerlinNoise(smoothStep * speed * 10f, 0f) * 2f - 1f) * curve * displacement.x;
                shakeVelocity.y = (Mathf.PerlinNoise(0f, smoothStep * speed * 10f) * 2f - 1f) * curve * displacement.y;
                if (++easeIn < 5f)
                    shakeVelocity *= easeIn / 4f; // ease in for 4 frames, prevents hard jump.
            }
            else
            {
                var current = speed * 100f * (smoothStep * Vector2.one + shakeTarget); //  Sine Shake
                shakeVelocity.x = Mathf.Sin(current.x) * curve * displacement.x;
                shakeVelocity.y = Mathf.Sin(current.y) * curve * displacement.y;
            }

            if (percent >= 1)
                shakeActive = false; // is shake complete?
            return shakeVelocity;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(55, "Shake the AI. Only shake an object if it's standing still." +
                                      "\n \nReturns Running, Success");

            FoldOut.Box(4, color, offsetY: -2);
            parent.Field("Type", "type");
            parent.Field("Displacement", "displacement");
            parent.Field("Duration", "duration");
            parent.Field("Speed", "speed");
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    public enum ShakeType
    {
        Random,
        Perlin,
        Sine
    }
}