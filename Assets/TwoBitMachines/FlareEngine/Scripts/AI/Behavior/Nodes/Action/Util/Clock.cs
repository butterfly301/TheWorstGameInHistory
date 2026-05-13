using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class Clock : Action
    {
        [SerializeField] public ClockRandom type;
        [SerializeField] public float wait = 1f;
        [SerializeField] public float min = 2f;
        [SerializeField] public float max = 5f;
        [SerializeField] public bool dontReset;
        [SerializeField] public string waitAnimation;
        [SerializeField] public UnityEvent onBeginClock;
        [SerializeField] public UnityEvent onFinishClock;
        [NonSerialized] public float actualWait;

        [NonSerialized] public float counter;

        public override NodeState RunNodeLogic(Root root)
        {
            if (!dontReset && nodeSetup == NodeSetup.NeedToInitialize) counter = 0;

            if (counter == 0)
            {
                actualWait = type == ClockRandom.Fixed ? wait : Random.Range(min, max);
                onBeginClock.Invoke();
            }

            root.signals.Set(waitAnimation);
            if (TwoBitMachines.Clock.Timer(ref counter, actualWait, Root.deltaTime))
            {
                onFinishClock.Invoke();
                return NodeState.Success;
            }

            return NodeState.Running;
        }

        #region ▀▄▀▄▀▄ Custom Inspector▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool eventsFoldOut;
        [SerializeField] [HideInInspector] public bool beginFoldOut;
        [SerializeField] [HideInInspector] public bool finishFoldOut;

        public override bool OnInspector(AIBase ai,
            SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(
                    103,
                    "Wait the specified time. If randomized, a new time is chosen each new clock cycle. "
                    + "If Dont Reset Timer is enabled, the timer will not be reset when the state is entered. The clock will continue with its previous value. The signal animation is optional."
                    + "\n \nReturns Running, Success"
                );

            var index = parent.Enum("type");
            FoldOut.Box(4, color, 5, -2);
            {
                parent.Field("Type", "type");
                parent.Field("Wait", "wait", index == 0);
                parent.FieldDouble("Range", "min", "max", index == 1);
                parent.Field("Signal Animation", "waitAnimation");
                parent.FieldToggleAndEnable("Dont Reset Timer", "dontReset");
            }

            if (FoldOut.FoldOutButton(parent.Get("eventsFoldOut")))
            {
                Fields.EventFoldOut(parent.Get("onBeginClock"), parent.Get("beginFoldOut"), "On Begin", color: color);
                Fields.EventFoldOut(parent.Get("onFinishClock"), parent.Get("finishFoldOut"), "On Finish",
                    color: color);
            }

            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    public enum ClockRandom
    {
        Fixed,
        Random
    }
}