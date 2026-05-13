#region

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif

#endregion

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
    [AddComponentMenu("")]
    public class PathfindingRT : Blackboard
    {
        [NonSerialized] public static List<PathfindingRT> maps = new();
        [SerializeField] [HideInInspector] public float cellSize = 1f;
        [SerializeField] public Vector2 maxJump = new(4f, 4f);
        [SerializeField] public AddExtras addExtra;
        [SerializeField] public List<Tilemap> tilemaps = new();

        [SerializeField] [HideInInspector] public bool createPaths;
        [SerializeField] [HideInInspector] public Vector2 cellOffset;
        [SerializeField] [HideInInspector] public LayerMask layerWorld;
        [SerializeField] [HideInInspector] public PathNode air = new();
        [SerializeField] [HideInInspector] public DictionaryMap map = new();
        [SerializeField] [HideInInspector] public List<Vector2Int> wall = new();
        [SerializeField] [HideInInspector] public List<Vector2Int> ladder = new();
        [SerializeField] [HideInInspector] public List<Vector2Int> bridge = new();
        [SerializeField] [HideInInspector] public List<Vector2Int> ceiling = new();
        [SerializeField] [HideInInspector] public DictionaryNeighbor neighbors = new();
        [NonSerialized] public NativeMultiHashMap<Vector2Int, Vector2Int> jobNeighbors;

        [NonSerialized] public NativeHashMap<Vector2Int, PathNodeStruct> jobPath;

        [NonSerialized] private readonly List<Tilemap> queueTempTilemap = new();
        [NonSerialized] private DictionaryMap tempMap = new();
        [NonSerialized] private DictionaryNeighbor tempNeighbors = new();
        [NonSerialized] private readonly List<Tilemap> tempTilemap = new();
        [NonSerialized] public List<TargetPathfindingBase> unit = new();

        public bool unitsCanOccupy { get; private set; }
        public bool isCreatingMap { get; private set; }
        public Vector2 cellYOffset { get; private set; }
        public Vector2 cellXOffset { get; private set; }

        private void Awake()
        {
            air.air = true;
            isCreatingMap = false;
            cellYOffset = cellSize * 0.5f * Vector2.up;
            cellXOffset = cellSize * 0.5f * Vector2.right;

            jobPath = new NativeHashMap<Vector2Int, PathNodeStruct>(map.Count, Allocator.Persistent);
            jobNeighbors = new NativeMultiHashMap<Vector2Int, Vector2Int>(map.Count, Allocator.Persistent);
            CreateJobSystem();
        }

        private void OnEnable()
        {
            if (!maps.Contains(this)) maps.Add(this);
        }

        private void OnDisable()
        {
            if (maps.Contains(this)) maps.Remove(this);
        }

        private void OnDestroy()
        {
            for (var i = 0;
                 i < unit.Count;
                 i++) //                     Must dispose of all jobs before disposing of jobGrid and jobNeighbors
                if (unit[i] != null || !unit[i].Equals(null))
                    unit[i].DisposeFollower();

            if (jobPath.IsCreated) jobPath.Dispose();
            if (jobNeighbors.IsCreated) jobNeighbors.Dispose();
        }

        public void RegisterFollower(TargetPathfindingBase newUnit, bool canBlock)
        {
            if (!unit.Contains(newUnit))
            {
                unit.Add(newUnit);
                if (canBlock) unitsCanOccupy = true;
            }
        }

        public static void OccupiedNodes()
        {
            for (var i = 0; i < maps.Count; i++)
                if (maps[i] != null)
                    maps[i].SetOccupiedPaths();
        }

        public void SetOccupiedPaths() // Occupied by units
        {
            if (!unitsCanOccupy || isCreatingMap) return;
            foreach (var node in map) node.Value.isOccupied = false;
            for (var i = 0; i < unit.Count; i++)
                if (unit[i] != null && unit[i].activeUnit)
                    unit[i].OccupyNode();
        }

        public PathNode PositionToNode(Vector2 position)
        {
            var x = Mathf.FloorToInt(position.x);
            var y = Mathf.FloorToInt(position.y);
            if (map.TryGetValue(new Vector2Int(x, y), out var node)) return node;
            return null;
        }

        public PathNode PositionFindNode(Vector2 position)
        {
            var x = Mathf.FloorToInt(position.x);
            var y = Mathf.FloorToInt(position.y);
            if (map.TryGetValue(new Vector2Int(x, y), out var node)) return node;
            air.x = x;
            air.y = y;
            air.position = new Vector2(x, y) + cellOffset;
            return air;
        }

        public void GridPosition(Vector2 position, out int x, out int y)
        {
            x = Mathf.FloorToInt(position.x);
            y = Mathf.FloorToInt(position.y);
        }

        public bool Node(int x, int y, out PathNode node)
        {
            return map.TryGetValue(new Vector2Int(x, y), out node);
        }

        public bool Contains(int x, int y)
        {
            return map.ContainsKey(new Vector2Int(x, y));
        }

        public void AddTilemaps(Tilemap tilemap)
        {
            tempTilemap.Clear();
            tempTilemap.Add(tilemap);
            AddTilemaps(tempTilemap);
        }

        public void AddTilemaps(List<Tilemap> tilemaps)
        {
            if (isCreatingMap)
            {
                for (var i = 0; i < tilemaps.Count; i++)
                    if (!queueTempTilemap.Contains(tilemaps[i]))
                        queueTempTilemap.Add(tilemaps[i]);

                return;
            }

            layerWorld = 1 << LayerMask.NameToLayer("World");
            cellSize = 1f; // cellSize <= 0 ? 1f : Mathf.Clamp(cellSize, 0.1f, 100f);
            cellOffset = new Vector2(cellSize * 0.5f, cellSize * 0.5f);
            CreateMap(tilemaps);

            if (Application.isPlaying) StartCoroutine(ExpandMap());
        }

        private void CreateMap(List<Tilemap> tilemaps)
        {
            CreateRTNodes.Execute(this, tilemaps);
            AddRTLadder.Execute(this);
            AddRTCeiling.Execute(this);
            AddRTBridge.Execute(this);
            AddRTWall.Execute(this);
            AddRTDrop.Execute(this);
            ConnectJumpNodes.Execute(this);
        }

        private IEnumerator ExpandMap()
        {
            // DebugTimer.Start();
            isCreatingMap = true;
            for (var i = 0; i < unit.Count; i++)
                if (unit[i] != null && !unit[i].JobIsComplete())
                    yield return null;

            CreateJobSystem(true);

            if (queueTempTilemap.Count > 0)
            {
                for (var i = 0; i < queueTempTilemap.Count; i++) tempTilemap.Add(queueTempTilemap[i]);
                queueTempTilemap.Clear();
                AddTilemaps(tempTilemap);
            }
            //  DebugTimer.Stop("Map exapanded: ");
        }

        private void CreateJobSystem(bool checkForDuplicates = false)
        {
            isCreatingMap = true;
            foreach (var element in map)
            {
                var node = element.Value;

                if (checkForDuplicates && jobNeighbors.ContainsKey(node.cell)) continue;

                var nodeStruct = new PathNodeStruct();
                nodeStruct.edgeDrop = node.edgeOfCorner;
                nodeStruct.ceiling = node.ceiling;
                nodeStruct.ground = node.ground;
                nodeStruct.ladder = node.ladder;
                nodeStruct.bridge = node.bridge;
                nodeStruct.wall = node.wall;
                nodeStruct.cell = node.cell;
                nodeStruct.x = node.x;
                nodeStruct.y = node.y;
                jobPath[node.cell] = nodeStruct;

                for (var j = 0; j < node.neighbor.Count; j++) jobNeighbors.Add(node.cell, node.neighbor[j]);
            }

            isCreatingMap = false;
        }

        #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀

#if UNITY_EDITOR
        [SerializeField] [HideInInspector] public List<GridConnections> connections = new();

        [Serializable]
        public struct GridConnections
        {
            public Vector2 position;
            public Vector2 end;
            public Color color;
            public float size;
            public int rays;
            public bool verticalLine;
            public bool horizontalLine;
        }

        public override void OnSceneGUI(Editor editor)
        {
            if (foldOut)
            {
                PathExtras(AddExtras.Ladder, ladder, Color.yellow, cellSize);
                PathExtras(AddExtras.Wall, wall, Color.black, cellSize);
                PathExtras(AddExtras.Ceiling, ceiling, Color.blue, cellSize);
                PathExtras(AddExtras.Bridge, bridge, Tint.DarkOrange, cellSize);

                if (addExtra != AddExtras.None)
                    if (Event.current.type == EventType.Layout)
                        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }

            if (createPaths)
            {
                DebugTimer.Start();
                map.Clear();
                neighbors.Clear();
                connections.Clear();
                createPaths = false;
                isCreatingMap = false;
                addExtra = AddExtras.None;
                AddTilemaps(tilemaps);

                DebugTimer.Stop("Created PathfindingRT Nodes ");
            }

            DrawWhenNotSelected();
        }

        public override void DrawWhenNotSelected()
        {
            if (foldOut)
            {
                Draw.GLStart();
                ShowExtras(ladder, Color.yellow, cellSize);
                ShowExtras(wall, Color.black, cellSize);
                ShowExtras(ceiling, Color.blue, cellSize);
                ShowExtras(bridge, Tint.DarkOrange, cellSize);
                Draw.GLEnd();
            }

            DisplayMapAfterEditing();
        }

        private void PathExtras(AddExtras type, List<Vector2Int> list, Color color, float cellSize, bool isFall = false)
        {
            if (type == addExtra && map.Count > 0)
            {
                var mousePosition = SceneTools.MousePosition();
                var x = Mathf.FloorToInt(mousePosition.x);
                var y = Mathf.FloorToInt(mousePosition.y);
                var position = new Vector2(x, y);
                var cellOffset = new Vector2(cellSize * 0.5f, cellSize * 0.5f);

                Draw.GLCircleInit(position + cellOffset, cellSize / 1.97f, color);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    var index = new Vector2Int(x, y);
                    if (list.Contains(index))
                        list.Remove(index);
                    else
                        list.Add(index);
                }
            }
        }

        private void ShowExtras(List<Vector2Int> list, Color color, float cellSize)
        {
            var cellOffset = new Vector2(cellSize * 0.5f, cellSize * 0.5f);
            for (var i = 0; i < list.Count; i++)
                Draw.GLCircle(new Vector2Int(list[i].x, list[i].y) + cellOffset, cellSize / 1.8f, color);
        }

        private void DisplayMapAfterEditing()
        {
            if (foldOut && !createPaths)
            {
                Draw.GLStart();
                for (var i = 0; i < connections.Count; i++)
                    if (connections[i].verticalLine)
                    {
                        var size = Mathf.Abs(connections[i].position.y - connections[i].end.y);
                        for (var j = 0; j < size; j++)
                            Draw.GLCircle(connections[i].position + Vector2.down * j, connections[i].size,
                                connections[i].color, 1);
                    }
                    else if (connections[i].horizontalLine)
                    {
                        var dir = connections[i].position.x <= connections[i].end.x ? 1f : -1f;
                        var size = Mathf.Abs(connections[i].position.x - connections[i].end.x);
                        for (var j = 0; j < size; j++)
                            Draw.GLCircle(connections[i].position + Vector2.right * j * dir, connections[i].size,
                                connections[i].color, 1);
                    }
                    else
                    {
                        Draw.GLCircle(connections[i].position, connections[i].size, connections[i].color,
                            connections[i].rays);
                    }

                Draw.GLEnd();
            }
        }

        public static void AddNodeDrawing(PathfindingRT map, Vector2 position, float size, Color color, int rays)
        {
            map.connections.Add(new GridConnections { position = position, size = size, color = color, rays = rays });
        }

        public static void AddVerticalPath(PathfindingRT map, Vector2 start, Vector2 end, float size, Color color)
        {
            map.connections.Add(new GridConnections
                { position = start, end = end, color = color, size = size, verticalLine = true });
        }

        public static void AddHorizontalPath(PathfindingRT map, Vector2 start, Vector2 end, float size, Color color)
        {
            map.connections.Add(new GridConnections
                { position = start, end = end, color = color, size = size, horizontalLine = true });
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy)]
        private static void DrawWhenObjectIsNotSelected(Pathfinding pathfinding, GizmoType gizmoType)
        {
            pathfinding.DrawWhenNotSelected();
        }

#endif

        #endregion
    }

    [Serializable]
    public class DictionaryMap : SerializableDictionary<Vector2Int, PathNode>
    {
    }

    [Serializable]
    public class DictionaryNeighbor : SerializableDictionary<PathNode, NeighborList>
    {
    }

    [Serializable]
    public enum AddExtras
    {
        None,
        Ladder,
        Wall,
        Ceiling,
        Bridge
    }
}