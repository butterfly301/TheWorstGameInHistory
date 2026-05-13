using System.Collections.Generic;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TwoBitMachines.FlareEngine
{
    public class CreateRTNodes
    {
        public static void Execute(PathfindingRT map, List<Tilemap> tilemaps)
        {
            for (var i = 0; i < tilemaps.Count; i++)
                if (tilemaps[i] != null && Compute.ContainsLayer(map.layerWorld, tilemaps[i].gameObject.layer))
                    CreateNodes(map, tilemaps[i]);
        }

        public static void CreateNodes(PathfindingRT map, Tilemap tilemap)
        {
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                var cellPosition = new Vector3Int(pos.x, pos.y, pos.z);
                var tile = tilemap.GetTile(cellPosition);

                if (tile == null) // in air
                {
                    if (map.wall.Count > 0)
                    {
                        var wallID = new Vector2Int(pos.x, pos.y);
                        if (map.wall.Contains(wallID))
                        {
                            PathNode wall;
                            if (map.Node(pos.x, pos.y, out wall))
                            {
                                wall.wall = true;
                            }
                            else
                            {
                                wall = new PathNode
                                    { position = wallID + map.cellOffset, x = pos.x, y = pos.y, wall = true };
                                map.map[wallID] = wall;
                            }

                            wall.wallLeft = tilemap.GetTile(cellPosition - Vector3Int.right);
                        }
                    }


                    continue;
                }

                var cell = new Vector2Int(cellPosition.x, cellPosition.y);
                var node = new PathNode { position = cell, x = cell.x, y = cell.y, block = true };
                map.map[cell] = node;

                // possible ground node
                cellPosition += Vector3Int.up;
                tile = tilemap.GetTile(cellPosition);


                if (tile != null) continue;
                // is air again, which means we are on ground
                cell = new Vector2Int(cellPosition.x, cellPosition.y);
                var position = cell + map.cellOffset;
                node = new PathNode { position = position, x = cell.x, y = cell.y, ground = true };
                map.map[cell] = node;
#if UNITY_EDITOR
                PathfindingRT.AddNodeDrawing(map, position, map.cellSize / 4f, Color.green, 1);
#endif

                if (IsCorner(map, tilemap, cellPosition.x, cellPosition.y, -1, out var nextToWallA))
                {
                    node.nextToWall = nextToWallA;
                    node.leftCorner = true;
#if UNITY_EDITOR
                    PathfindingRT.AddNodeDrawing(map, position, map.cellSize / 6f, Color.red, 1);
#endif
                }

                if (IsCorner(map, tilemap, cellPosition.x, cellPosition.y, 1, out var nextToWallB))
                {
                    node.nextToWall = nextToWallB;
                    node.rightCorner = true;
#if UNITY_EDITOR
                    PathfindingRT.AddNodeDrawing(map, position, map.cellSize / 6f, Color.red, 1);
#endif
                }
            }
        }

        private static bool IsCorner(PathfindingRT map, Tilemap tilemap, int x, int y, int sign, out bool nextToWall)
        {
            nextToWall = false;
            var cell = new Vector3Int(x + sign, y, 0);
            if (tilemap.GetTile(cell) != null) return nextToWall = true; // next to wall

            if (tilemap.GetTile(new Vector3Int(x + sign, y - 1, 0)) == null)
            {
                if (!map.Contains(x + sign, y))
                {
                    Vector3 position = new Vector2(cell.x, cell.y) + map.cellOffset;
                    var node = new PathNode { position = position, x = cell.x, y = cell.y, edgeOfCorner = true };
                    map.map[new Vector2Int(cell.x, cell.y)] = node;
#if UNITY_EDITOR
                    PathfindingRT.AddNodeDrawing(map, position, 0.62f, Color.grey, 2);
#endif
                }

                return true;
            }

            return false;
        }
    }
}