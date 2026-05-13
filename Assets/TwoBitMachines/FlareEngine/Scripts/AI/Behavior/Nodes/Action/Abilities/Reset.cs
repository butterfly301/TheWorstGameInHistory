using System;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class Reset : Action
    {
        public enum ResetTo
        {
            ResetPoint,
            InitialPosition
        }

        [SerializeField] public ResetTo resetTo = ResetTo.InitialPosition;
        [SerializeField] public Blackboard resetPoint;
        [SerializeField] public Health healthRef;
        [SerializeField] public float healthValue;
        [SerializeField] public WorldBool worldBool;
        [SerializeField] public TransformTracker tracker;
        [SerializeField] public bool deactivateIfDead = true;
        [NonSerialized] private Vector3 initialPosition;
        [NonSerialized] private Vector3 initialRotation;

        private void Awake()
        {
            initialPosition = transform.position;
            initialRotation = transform.localEulerAngles;
        }

        public override NodeState RunNodeLogic(Root root)
        {
            if (nodeSetup == NodeSetup.NeedToInitialize)
            {
                tracker = tracker == null ? TransformTracker.get : tracker;

                if (worldBool != null && worldBool.GetValue())
                {
                    if (deactivateIfDead) gameObject.SetActive(false);
                    return NodeState.Success;
                }

                if (tracker != null && tracker.Contains(transform))
                {
                    if (deactivateIfDead) gameObject.SetActive(false);
                    return NodeState.Success;
                }

                gameObject.SetActive(true);
                if (root.world.box.collider != null)
                {
                    root.world.box.collider.enabled = true;
                    root.world.box.ColliderBasicReset();
                }
                else
                {
                    var collider = gameObject.GetComponent<Collider2D>();
                    if (collider != null)
                        collider.enabled = true;
                }

                if (resetTo == ResetTo.ResetPoint && resetPoint != null)
                {
                    transform.localEulerAngles = initialRotation;
                    transform.position = resetPoint.GetTarget();
                }

                if (resetTo == ResetTo.InitialPosition)
                {
                    transform.localEulerAngles = initialRotation;
                    transform.position = initialPosition;
                }

                if (healthRef != null) healthRef.SetValue(healthValue);
                var renderer = gameObject.GetComponent<SpriteRenderer>();
                if (renderer != null) renderer.enabled = true;
            }

            return NodeState.Failure;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(80,
                    "This will reset the AI's position and health. If WorldBool or TransformTracker " +
                    "exist and are true, the AI will be considered dead and deactivated. If so, this will return Success." +
                    "\n \nReturns Success, Failure"
                );

            var resetTo = parent.Enum("resetTo") == 0 ? 1 : 0;
            FoldOut.Box(5 + resetTo, color, offsetY: -2);
            parent.Field("Reset To", "resetTo");
            if (resetTo == 1)
                AIBase.SetRef(ai.data, parent.Get("resetPoint"), 0);
            parent.FieldDouble("Health Reference", "healthRef", "healthValue");
            parent.Field("WorldBool", "worldBool");
            parent.Field("Transform Tracker", "tracker");
            parent.Field("Off if Dead", "deactivateIfDead");
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}