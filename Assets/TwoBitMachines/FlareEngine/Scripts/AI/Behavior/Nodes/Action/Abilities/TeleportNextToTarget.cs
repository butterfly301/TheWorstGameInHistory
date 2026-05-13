#region

using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu("")]
    public class TeleportNextToTarget : Action
    {
        public enum TeleportType
        {
            DistanceAndWallCheck,
            Instant
        }

        [SerializeField] public Blackboard target;
        [SerializeField] public TeleportType type;
        [SerializeField] public Vector2 offset;
        [SerializeField] public float distance;
        [SerializeField] public bool exitOnWall;

        public override NodeState RunNodeLogic(Root root)
        {
            if (target == null) return NodeState.Failure;

            return type == TeleportType.Instant ? Instant(root) : Distance(root);
        }

        public NodeState Distance(Root root)
        {
            var box = root.world.box;
            var tempDistance = target.GetTarget().x - root.position.x;
            if (target.hasNoTargets) return NodeState.Failure;

            var sign = Mathf.Sign(tempDistance);
            var magnitude = distance + Mathf.Abs(tempDistance) + box.skin.x * 2f;
            var corner = sign > 0 ? box.bottomRight - box.skinX : box.bottomLeft + box.skinX;

            for (var i = 0; i < box.rays.x; i++)
            {
                var origin = corner + box.up * box.spacing.y * i;
                var hit = Physics2D.Raycast(origin, box.right * sign, magnitude, WorldManager.collisionMask);
                if (hit)
                {
                    if (exitOnWall)
                        return NodeState.Failure;
                    if (hit.distance > 0)
                        magnitude = hit.distance - box.skin.x * 2f;
                }
            }

            transform.position += Vector3.right * sign * magnitude; // teleport
            return NodeState.Success;
        }


        public NodeState Instant(Root root)
        {
            Vector3 position = target.GetTarget();
            if (target.hasNoTargets) return NodeState.Failure;
            var direction = position.x >= root.position.x ? -1f : 1f;
            position.x += direction * offset.x;
            position.y += offset.y;
            position.z = transform.position.z;
            transform.position = position;
            return NodeState.Success;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        public override bool OnInspector(AIBase ai, SerializedObject so, Color color, bool onEnable)
        {
            if (so.Bool("showInfo"))
                Labels.InfoBoxTop(55,
                    "Teleport next to the target by the distance specified. Exit teleportation if there is a wall in the way. Or teleport instantly." +
                    "\n \nReturns Success, Failure");
            var type = so.Enum("type");
            var height = type == 0 ? 1 : 0;
            FoldOut.Box(3 + height, color, offsetY: -2);
            {
                AIBase.SetRef(ai.data, so.Get("target"), 0);
                so.Field("Type", "type");
                so.Field("Distance", "distance", type == 0);
                so.Field("Exit on Wall", "exitOnWall", type == 0);
                so.Field("Offset", "offset", type == 1);
            }
            Layout.VerticalSpacing(3);
            return true;
        }
#pragma warning restore 0414
#endif

        #endregion
    }
}