using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class SpawnProjectile : Action
    {
        public enum PositionType
        {
            Point,
            Target,
            AI_Position
        }

        [SerializeField] public ProjectileBase projectile;
        [SerializeField] public Vector2 direction;
        [SerializeField] public PositionType type;
        [SerializeField] public Blackboard target;
        [SerializeField] public Vector2 position;

        public override NodeState RunNodeLogic(Root root)
        {
            if (projectile == null || (type == PositionType.Target && target == null))
                return NodeState.Failure;

            var fireDirection = direction;

            if (fireDirection == Vector2.zero) fireDirection = new Vector2(root.direction, 0);

            if (type == PositionType.Point)
            {
                projectile.FireProjectileStatic(position, fireDirection);
            }
            else if (type == PositionType.Target)
            {
                var targetPosition = target.GetTarget();
                if (target.hasNoTargets) return NodeState.Failure;
                projectile.FireProjectileStatic(targetPosition, fireDirection);
            }
            else
            {
                projectile.FireProjectileStatic(transform.position, fireDirection);
            }

            return NodeState.Success;
        }

        #region ▀▄▀▄▀▄ Custom Inspector▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool("showInfo"))
                Labels.InfoBoxTop(100,
                    "Spawn a projectile at the fire point. If direction is zero, the direction will be relative to the ai in the x direction." +
                    "\n \nReturns Success, Failure");

            var type = parent.Enum("type");
            var height = type == 2 ? -1 : 0;
            FoldOut.Box(4 + height, color, offsetY: -2);
            {
                parent.Field("Projectile", "projectile");
                parent.Field("Fire Point", "type");
                parent.Field("Point", "position", type == 0);
                if (type == 1)
                    AIBase.SetRef(ai.data, parent.Get("target"), 0);
                parent.Field("Direction", "direction");
            }
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}