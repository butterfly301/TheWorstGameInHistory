using System;
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
    [AddComponentMenu("")]
    public class TargetPathfindingBase : Blackboard
    {
        [SerializeField] public float followSpeed = 5f;
        [SerializeField] public float ceilingSpeed = 5f;
        [SerializeField] public float ladderSpeed = 5f;
        [SerializeField] public float wallSpeed = 5f;
        [SerializeField] public bool ignoreUnits = true;
        [SerializeField] public float pauseAfterJump;
        [NonSerialized] public bool activeUnit = true;
        [NonSerialized] public BezierJump bezierJump = new();
        [NonSerialized] public Vector2 bottomCenter;
        [NonSerialized] public float counter;
        [NonSerialized] public PathNode currentNode; //  node ai currently inhabits
        [NonSerialized] public bool followToCenter;
        [NonSerialized] public PathNode futureNode; //   node that comes after nextNode
        [NonSerialized] public float gravity;
        [NonSerialized] public PathState jumpToState;
        [NonSerialized] public JumpType jumpType; // for jump state
        [NonSerialized] public PathNode nextNode; //     node ai is following
        [NonSerialized] public Stack<PathNode> path = new();
        [NonSerialized] public bool pauseAfterJumpActive;
        [NonSerialized] public float pauseCounter;
        [NonSerialized] public Vector2 position;
        [NonSerialized] public PathState previousState;
        [NonSerialized] public bool recalculate;
        [NonSerialized] public AnimationSignals signals;

        [NonSerialized] public Vector2 size; //           character Size

        [NonSerialized] public PathState state;

        [NonSerialized] public Blackboard targetRef;
        [NonSerialized] public int targetX;
        [NonSerialized] public int targetY;

        [NonSerialized] public float timeStamp;

        [NonSerialized] public float variance;
        //[SerializeField] public float cornerYOffset;
        //[SerializeField] public bool cornerGrab;

        [NonSerialized] public float velRef;
        [NonSerialized] public bool wait;
        [NonSerialized] public bool waitForPath;

        [NonSerialized] public WorldCollision world;

        public bool pathSafeToChagne => followingSafeNode && currentNode != null && currentNode.path;
        public bool followingSafeNodeX => state == PathState.Follow || state == PathState.Ceiling;
        public bool followingSafeNodeY => state == PathState.Ladder || state == PathState.Wall;
        public bool followingUnsafeNode => !followingSafeNodeX && !followingSafeNodeY;
        public bool followingSafeNode => followingSafeNodeX || followingSafeNodeY;
        public bool stateChanged => previousState != state;
        public bool blockUnits => !ignoreUnits;
        public bool notJumpingState => state != PathState.Jump;

        public virtual float cellSize => 1f;
        public virtual Vector2 cellYOffset => Vector2.up;

        public virtual void CalculatePath(Blackboard target)
        {
        }

        public virtual void DisposeFollower()
        {
        }

        public virtual void OccupyNode()
        {
        }

        public virtual bool JobIsComplete()
        {
            return false;
        }
    }


    public enum PathState
    {
        Follow = 0,
        Jump = 1,
        Ceiling = 2,
        CornerGrab = 3,
        Ladder = 4,
        Wall = 5,
        Moving = 6
    }
}