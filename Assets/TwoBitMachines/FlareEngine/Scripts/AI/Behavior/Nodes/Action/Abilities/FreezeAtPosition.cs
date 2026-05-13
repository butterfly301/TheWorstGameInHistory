#region

using System;
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class FreezeAtPosition : Action
    {
        [NonSerialized] private int direction;
        [NonSerialized] private Player player;
        [NonSerialized] private int playerDirection;
        [NonSerialized] private Vector3 position;

        private void Awake()
        {
            player = gameObject.GetComponent<Player>();
        }

        public override NodeState RunNodeLogic(Root root)
        {
            if (nodeSetup == NodeSetup.NeedToInitialize)
            {
                position = root.transform.position;
                direction = root.direction;
                if (player != null)
                    playerDirection = player.abilities.playerDirection;
            }

            root.velocity = Vector2.zero;
            root.transform.position = position;
            root.direction = direction;

            if (player != null)
            {
                player.abilities.velocity.y = 0;
                player.abilities.playerDirection = playerDirection;
                var velXLeft = playerDirection < 0;
                var velXRight = playerDirection > 0;
                player.signals.Set("velXLeft", velXLeft);
                player.signals.Set("velXRight", velXRight);
            }

            return NodeState.Running;
        }


        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool HasNextState()
        {
            return false;
        }

        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(45, "The Transform will freeze at its current position." +
                                      "\n \nReturns Running");
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}