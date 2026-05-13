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
    public class Root : Composite
    {
        public static float deltaTime;
        [SerializeField] public bool pause;
        [NonSerialized] public AIState aiState;
        [NonSerialized] public float counter;
        [NonSerialized] public int direction;
        [NonSerialized] public int foundInterrupt = -1;
        [NonSerialized] public Gravity gravity;
        [NonSerialized] public bool hasJumped;
        [NonSerialized] public int interruptIndex;
        [NonSerialized] public bool isShaking;
        [NonSerialized] public bool moveWithTranslate = false;
        [NonSerialized] public MovingPlatform movingPlatform;
        [NonSerialized] public bool onSurface;
        [NonSerialized] public bool pauseCollision = false;
        [NonSerialized] public Vector2 position;
        [NonSerialized] public Vector2 previousPosition;
        [NonSerialized] public Vector2 previousShakeVel;
        [NonSerialized] public string previousStateName;
        [NonSerialized] public bool selectInterrupt = false;
        [NonSerialized] public AnimationSignals signals;
        [NonSerialized] public string stateName;
        [NonSerialized] public StateNode stateNode;

        [NonSerialized] public CharacterType type;
        [NonSerialized] public Vector2 velocity;
        [NonSerialized] public WorldCollision world;
        public bool onGround => onSurface || world.onGround;

        public void RunTree(ref Vector2 velocity, ref int direction, Vector2 position)
        {
            onSurface = false;
            hasJumped = false;
            this.velocity = velocity;
            this.direction = direction;
            this.position = position;
            for (var i = 0; i < children.Count; i++)
            {
                interruptIndex = i;
                nodeState = children[i].RunChild(this);
            }

            HandleInterrupt();
            velocity = this.velocity;
            direction = this.direction;
        }

        private void HandleInterrupt()
        {
            if (foundInterrupt > -1 && children.Count > 0 && foundInterrupt < children.Count)
                children[foundInterrupt].Reset(); // each branch from root can be treated separately
            foundInterrupt = -1; // -1 is off/false;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo")) Labels.InfoBoxTop(35, "The entry point into the tree.");

            FoldOut.Box(3, color, offsetY: -2);
            parent.Field("Can Interrupt", "canInterrupt");
            parent.Field("On Interrupt", "onInterrupt");
            parent.Field("Pause", "pause");
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion

        #region 2D Results

        public static ContactFilter2D filter2D;
        public static List<RaycastHit2D> rayResults = new();
        public static List<Collider2D> colliderResults = new();
        public static RaycastHit2D raycastHit2D;
        public static Collider2D collider2DRef;
        public static bool isAlways;

        public void SetLayerMask(LayerMask layer)
        {
            filter2D.useLayerMask = true;
            filter2D.layerMask = layer;
            filter2D.useTriggers = true;
        }

        #endregion

        #region FSM

        public bool RunFSM(AIState state, Vector2 position, ref Vector2 velocity, ref int direction,
            out string nextState, bool reset)
        {
            if (state.useSignal)
                signals.Set(state.defaultSignal);
            onSurface = reset ? false : onSurface; // should only be set off once per frame
            hasJumped = reset ? false : hasJumped;
#if UNITY_EDITOR
            isAlways = !reset;
            if (!isAlways) stateName = state.stateName;
#endif
            aiState = state;
            this.velocity = velocity;
            this.direction = direction;
            this.position = position;
            if (state.type == AIStateType.Parallel)
                nextState = RunFSMParallel(state.action);
            else
                nextState = RunFSMSequence(state, state.action);
            velocity = this.velocity;
            direction = this.direction;
            return nextState != "";
        }

        public string RunFSMSequence(AIState state, List<StateNode> action)
        {
            for (var i = state.childIndex; i < action.Count; i++)
            {
                stateNode = action[i];
                var childState = stateNode.node.RunChild(this);
#if UNITY_EDITOR
                if (!isAlways)
                {
                    fsmChild = stateNode.node.nameType;
                    fsmIndex = state.childIndex;
                }
#endif

                if (childState == NodeState.Success)
                {
                    if (stateNode.goToSuccess) return stateNode.onSuccess;
                    state.childIndex = state.childIndex + 1 >= action.Count ? 0 : state.childIndex + 1;
                    continue;
                }

                if (childState == NodeState.Failure)
                {
                    if (stateNode.goToFailure) return stateNode.onFailure;
                    if (state.type != AIStateType.SequenceSucceed)
                    {
                        state.childIndex = state.childIndex + 1 >= action.Count ? 0 : state.childIndex + 1;
                        continue;
                    }
                }

                return "";
            }

            return "";
        }

        public string RunFSMParallel(List<StateNode> action)
        {
#if UNITY_EDITOR
            if (!isAlways) fsmChild = "Is Parallel";
#endif
            for (var i = 0; i < action.Count; i++)
            {
                stateNode = action[i];
                var childState = stateNode.node.RunChild(this);
                if (childState == NodeState.Success && stateNode.goToSuccess)
                    return stateNode.onSuccess;
                if (childState == NodeState.Failure && stateNode.goToFailure)
                    return stateNode.onFailure;
            }

            return "";
        }

        #endregion
    }
}