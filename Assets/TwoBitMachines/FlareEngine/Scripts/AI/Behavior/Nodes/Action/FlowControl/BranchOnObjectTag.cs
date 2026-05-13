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
    public class BranchOnObjectTag : Action
    {
        [SerializeField] public List<BranchTo> branch = new();
        [SerializeField] public UnityEventEffect onBranch;
        [NonSerialized] private Transform id;

        [NonSerialized] private AIFSM parent;

        public void Start()
        {
            parent = GetComponent<AIFSM>();
        }

        public void ActivateBranch(ImpactPacket packet)
        {
            if (packet.attacker == null || parent == null)
                return;

            var tag = packet.attacker.tag;
            for (var i = 0; i < branch.Count; i++)
                if (branch[i].tag == tag)
                {
                    parent.ChangeState(branch[i].state);
                    onBranch.Invoke(packet);
                    return;
                }
        }

        public override NodeState RunNodeLogic(Root root)
        {
            return NodeState.Success;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] [HideInInspector] public bool eventFoldout;

        public override bool HasNextState()
        {
            return false;
        }

        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(95,
                    "This will read an object's tag and jump to the specified state. Create as many options as necessary. To activate, call its Activate Branch method. This is typically used by the Health component. Only works with AIFSM." +
                    "\n \nReturns  Success");

            var array = parent.Get("branch");
            if (array.arraySize == 0)
                array.arraySize++;

            FoldOut.Box(array.arraySize, color, offsetY: -2);
            for (var i = 0; i < array.arraySize; i++)
                Fields.ArrayPropertyFieldDouble(array, i, "Tag, State", "tag", "state");
            Layout.VerticalSpacing(3);
            Fields.EventFoldOut(parent.Get("onBranch"), parent.Get("eventFoldout"), "On Branch", color: color,
                offsetY: -2);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }

    [Serializable]
    public class BranchTo
    {
        public string tag = "";
        public string state = "";
    }
}