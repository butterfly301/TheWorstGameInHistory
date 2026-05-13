#region

using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
    [AddComponentMenu("")]
    public class Pathfinding : Blackboard
    {
        [NonSerialized] public static List<Pathfinding> maps = new();
        [SerializeField] public LayerMask layerWorld;
        [SerializeField] public float maxJumpHeight = 4f;
        [SerializeField] public float maxJumpDistance = 4f;
        [SerializeField] public float cellSize = 1f;

        [SerializeField] [HideInInspector] public List<Vector2Int> ladder = new();
        [SerializeField] [HideInInspector] public List<Vector2Int> ceiling = new();
        [SerializeField] [HideInInspector] public List<Vector2Int> moving = new();
        [SerializeField] [HideInInspector] public List<Vector2Int> bridge = new();
        [SerializeField] [HideInInspector] public List<Vector2Int> wall = new();
        [SerializeField] [HideInInspector] public List<Vector2Int> fall = new();
        [SerializeField] [HideInInspector] public SimpleBounds bounds = new();
        [SerializeField] [HideInInspector] public int linesX;
        [SerializeField] [HideInInspector] public int linesY;
        [SerializeField] [HideInInspector] public PathNode[] grid;

        [SerializeField] [HideInInspector]
        public List<NeighborList>
            neighbor; // this list should actually exist inside PathNode, but the inspector crashed every time this AIAction get's instantiated

        [SerializeField] [HideInInspector] private List<TargetPathfindingBase> unit = new();

        public NativeArray<PathNodeStruct> jobGrid;
        [SerializeField] [HideInInspector] public NativeMultiHashMap<int, int> jobNeighbors;

        public Vector2 cellYOffset { get; private set; }
        public Vector2 cellXOffset { get; private set; }

        public PathNode PositionToNode(Vector2 position)
        {
            var gridPosition = (position - bounds.position) / cellSize; // cell size cannot be zero!
            var x = Mathf.FloorToInt(Mathf.Clamp(Mathf.Abs(gridPosition.x), 0, linesX - 1));
            var y = Mathf.FloorToInt(Mathf.Clamp(Mathf.Abs(gridPosition.y + 0.1f), 0, linesY - 1));
            return grid[y * linesX + x];
        }

        public PathNode Node(int x, int y)
        {
            return grid[Mathf.Clamp(y, 0, linesY - 1) * linesX + Mathf.Clamp(x, 0, linesX - 1)];
        }

        public void SetOccupiedPaths() // Occupied by units
        {
            for (var i = 0; i < grid.Length; i++) grid[i].isOccupied = false; // clear old values
            for (var i = 0; i < unit.Count; i++)
                if (unit[i] != null && unit[i].activeUnit)
                    unit[i].OccupyNode();
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField] public bool showPaths = true;
        [SerializeField] public bool addLadders;
        [SerializeField] public bool addWalls;

        [SerializeField] public bool addCeilings;

        //[SerializeField] public bool addMoving = false; // moving platforms
        // [SerializeField] public bool enableFall = false;
        [SerializeField] public bool addBridge;
        [SerializeField] [HideInInspector] public bool createPaths;
        [SerializeField] [HideInInspector] public List<GridConnections> connections = new();

        [Serializable]
        public struct GridConnections
        {
            public Vector2 position;
            public Color color;
            public float size;
            public int rays;
        }
#pragma warning restore 0414
#endif

        #endregion

        #region Setup

        public void Awake()
        {
            cellYOffset = cellSize * 0.5f * Vector2.up;
            cellXOffset = cellSize * 0.5f * Vector2.right;
            bounds.Initialize();
            InitializeJobSystem();
        }

        public void OnEnable()
        {
            if (!maps.Contains(this)) maps.Add(this);
        }

        public void OnDisable()
        {
            if (maps.Contains(this)) maps.Remove(this);
        }

        public void RegisterFollower(TargetPathfindingBase newUnit)
        {
            if (!unit.Contains(newUnit)) unit.Add(newUnit);
        }

        public override bool Contains(Vector2 position)
        {
            return bounds.Contains(position);
        }

        private void OnDestroy()
        {
            for (var i = 0;
                 i < unit.Count;
                 i++) //                     Must dispose of all jobs before disposing of jobGrid and jobNeighbors
                if (unit[i] != null || !unit[i].Equals(null))
                    unit[i].DisposeFollower();
            if (jobGrid.IsCreated)
                jobGrid.Dispose();
            if (jobNeighbors.IsCreated)
                jobNeighbors.Dispose();
        }

        public static void OccupiedNodes()
        {
            for (var i = maps.Count - 1; i >= 0; i--)
                if (maps[i] != null)
                    maps[i].SetOccupiedPaths();
        }

        public void InitializeJobSystem()
        {
            jobGrid = new NativeArray<PathNodeStruct>(grid.Length, Allocator.Persistent);
            var listN = new List<PathNode>();
            // this is a costly operation
            for (var i = 0; i < grid.Length; i++)
            {
                var node = new PathNodeStruct();
                var n = grid[i];
                node.moving = n.moving;
                node.jumpThroughGround = n.jumpThroughGround;
                node.rightCorner = n.rightCorner;
                node.leftCorner = n.leftCorner;
                node.wall = n.wall;
                node.edgeDrop = n.edgeOfCorner;
                node.ceiling = n.ceiling;
                node.ground = n.ground;
                node.height = n.height;
                node.ladder = n.ladder;
                node.bridge = n.bridge;
                node.block = n.block;
                node.exact = n.exact;
                node.air = n.air;
                node.x = n.x;
                node.y = n.y;
                node.index = i;
                jobGrid[i] = node;
            }

            jobNeighbors = new NativeMultiHashMap<int, int>(listN.Count, Allocator.Persistent);

            for (var i = 0; i < neighbor.Count; i++)
            for (var j = 0; j < neighbor[i].neighbor.Count; j++)
            {
                var index = neighbor[i].gridX + neighbor[i].gridY * linesX;
                jobNeighbors.Add(index, neighbor[i].neighbor[j].x + neighbor[i].neighbor[j].y * linesX);
            }
        }

        #endregion

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
        public override void OnSceneGUI(Editor editor)
        {
            if (bounds.position == Vector2.zero) bounds.position = SceneTools.SceneCenter(transform.position);
            SceneTools.DrawAndModifyBounds(ref bounds.position, ref bounds.size, Color.green);

            var snapSize = cellSize > 1 ? cellSize * 0.25f : cellSize;
            bounds.position = Compute.Round(bounds.position, snapSize);
            bounds.size = Compute.Round(bounds.size, cellSize);

            if (showPaths)
            {
                SceneTools.TwoDGrid(bounds.position, bounds.size, new Vector2(cellSize, cellSize),
                    new Color32(4, 184, 236, 50));
                PathExtras(addLadders, ladder, Color.yellow, cellSize);
                PathExtras(addWalls, wall, Color.black, cellSize);
                PathExtras(addCeilings, ceiling, Color.blue, cellSize);
                //PathExtras (addMoving, moving, Color.red, cellSize);
                PathExtras(addBridge, bridge, Tint.DarkOrange, cellSize);
                // PathExtras (enableFall, fall, Tint.Orange, cellSize, true);
                if (addLadders || addWalls || addCeilings || addBridge)
                    if (Event.current.type == EventType.Layout)
                        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }

            AIPathfindingEditor.CreateGrid(this, showPaths);
        }

        public override void DrawWhenNotSelected()
        {
            if (showPaths)
            {
                SceneTools.TwoDGrid(bounds.position, bounds.size, new Vector2(cellSize, cellSize),
                    new Color32(4, 184, 236, 50));
                ShowExtras(ladder, Color.yellow, cellSize);
                ShowExtras(wall, Color.black, cellSize);
                ShowExtras(ceiling, Color.blue, cellSize);
                ShowExtras(bridge, Tint.DarkOrange, cellSize);
                ShowExtras(fall, Tint.Orange, cellSize);
            }

            AIPathfindingEditor.DisplayMapAfterEditing(this, showPaths);
        }

        private void PathExtras(bool edit, List<Vector2Int> list, Color color, float cellSize, bool isFall = false)
        {
            if (edit && grid.Length > 0 && linesX != 0 && linesY != 0)
            {
                var node = PositionToNode(SceneTools.MousePosition());
                if (node != null)
                {
                    Draw.GLCircleInit(node.position, cellSize / 1.97f, color);
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        var index = new Vector2Int(node.x, node.y);
                        if (list.Contains(index))
                            list.Remove(index);
                        // if (isFall) node.isFall = false;
                        else
                            list.Add(index);
                        // if (isFall) node.isFall = true;
                    }
                }
            }
        }

        private void ShowExtras(List<Vector2Int> list, Color color, float cellSize)
        {
            Draw.GLStart();
            for (var i = list.Count - 1; i >= 0; i--)
            {
                var index = list[i].y * linesX + list[i].x;
                if (index < grid.Length && index >= 0)
                    Draw.GLCircle(grid[index].position, cellSize / 1.8f, color);
                else
                    list.RemoveAt(i);
            }

            Draw.GLEnd();
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.InSelectionHierarchy)]
        private static void DrawWhenObjectIsNotSelected(Pathfinding pathfinding, GizmoType gizmoType)
        {
            pathfinding.DrawWhenNotSelected();
        }

#endif

        #endregion
    }

    [Serializable]
    public class NeighborList
    {
        public int gridX = -1;
        public int gridY = -1;
        [SerializeField] public List<PathNode> neighbor = new();
        public Vector2Int gridID => new(gridX, gridY);
    }
}