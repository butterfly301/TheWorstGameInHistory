#region

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
    public class CannonExit : Conditional
    {
        public enum CannonExitType
        {
            Automatic,
            Button
        }

        [SerializeField] public Player player;
        [SerializeField] public CannonExitType exitType;
        [SerializeField] public InputButtonSO input;
        [SerializeField] public float force = 10f;
        [SerializeField] public float airFriction = 2f;
        [SerializeField] public float blastOffset = 1f;

        [SerializeField] private Vector2 direction;
        [SerializeField] private float tempForce;
        [SerializeField] private float healthRef;
        [SerializeField] private bool foundTarget;

        [SerializeField] private Health health;
        [SerializeField] private Character character;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public override NodeState RunNodeLogic(Root root)
        {
            if (player == null) return NodeState.Failure;
            if (spriteRenderer == null) spriteRenderer = player.transform.GetComponent<SpriteRenderer>();
            if (exitType == CannonExitType.Automatic || (input != null && input.Pressed()))
            {
                var health = player.GetComponent<Health>();
                if (health != null) health.CanTakeDamage(true);
                if (spriteRenderer != null) spriteRenderer.enabled = true;
                player?.GetComponent<Cannon>()
                    ?.Activate(transform.up, Mathf.Abs(force), Mathf.Abs(airFriction), blastOffset);
                return NodeState.Success;
            }

            return NodeState.Running;
        }

        public override bool HardReset()
        {
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            return true;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(55, "Exit cannon." +
                                      "\n \nReturns Running, Failure, Success");

            var type = parent.Enum("exitType");

            FoldOut.Box(5, color, offsetY: -2);
            parent.Field("Player", "player");
            parent.Field("Exit", "exitType", type == 0);
            parent.FieldDouble("Exit", "exitType", "input", type == 1);
            parent.Field("Force", "force");
            parent.Field("Air Friction", "airFriction");
            parent.Field("Blast Offset", "blastOffset");
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}