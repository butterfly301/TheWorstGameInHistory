#region

using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class SequenceController : Action
    {
        [SerializeField] public SequenceState sequence = new();
        [NonSerialized] private float counter;

        public override NodeState RunNodeLogic(Root root)
        {
            if (nodeSetup == NodeSetup.NeedToInitialize) counter = 0;
            if (TwoBitMachines.Clock.Timer(ref counter, sequence.duration))
            {
                sequence.Execute();
                return NodeState.Success;
            }

            return NodeState.Running;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(45,
                    "Execute a sequence of AIFSM or GameObjects in order after the specified duration." +
                    "\n \nReturns Running, Success");

            var sequence = parent.Get("sequence");

            var tint = 1f;


            var itemArray = sequence.Get("itemList");
            itemArray.IncIfZero();

            FoldOut.BarOffsetY(sequence, color * tint, -2, 5, 20).Label("Duration").SL(3)
                .LF("duration", 60, yOffset: 0);

            if (Bar.ButtonRight("Delete", Tint.White, toolTip: "Delete Sequence")) itemArray.arraySize--;
            if (Bar.ButtonRight("Add", Tint.White, toolTip: "Add Sequence Item")) itemArray.arraySize++;

            Layout.Update(0.25f);
            FoldOut.Box(itemArray.arraySize, color * tint, offsetY: -4, extraHeight: 2);
            {
                for (var j = 0; j < itemArray.arraySize; j++)
                {
                    var itemElement = itemArray.Element(j);
                    var type = itemElement.Enum("type");
                    itemElement.FieldDouble("type", "aifsm", "state", titleIsField: true, execute: type == 0);
                    itemElement.FieldAndToggle("type", "gameObject", "enable", true, type == 1);
                }
            }
            Layout.VerticalSpacing(3);
            Layout.Update();
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    [Serializable]
    public class SequenceState
    {
        [SerializeField] public float duration = 1f;
        [SerializeField] public int signalIndex;
        [SerializeField] public bool active;
        [SerializeField] public List<SequenceItem> itemList = new();

        public void Execute()
        {
            for (var i = 0; i < itemList.Count; i++)
            {
                var item = itemList[i];
                if (item.type == SequenceItemType.AIFSM)
                    if (item.aifsm != null)
                        item.aifsm.ChangeState(item.state);

                if (item.type == SequenceItemType.GameObject)
                    if (item.gameObject != null)
                        item.gameObject.SetActive(item.enable);
            }
        }
    }

    [Serializable]
    public class SequenceItem
    {
        [SerializeField] public AIFSM aifsm;
        [SerializeField] public GameObject gameObject;
        [SerializeField] public SequenceItemType type;
        [SerializeField] public string state;
        [SerializeField] public bool enable;
    }

    public enum SequenceItemType
    {
        AIFSM,
        GameObject
    }
}