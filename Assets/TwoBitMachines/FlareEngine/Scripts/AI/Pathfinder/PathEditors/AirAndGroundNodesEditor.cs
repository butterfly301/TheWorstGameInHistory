#if UNITY_EDITOR
using TwoBitMachines.FlareEngine;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.Editors
{
    public class AirAndGroundNodesEditor : AIPathfindingEditor
    {
        public static void Execute(Vector2 startingPosition, PathNode[] grid, Pathfinding map, Vector2 cellOffset)
        {
            for (var x = 0; x < map.linesX; x++)
            {
                var nodeBasePosition = startingPosition + cellOffset + Vector2.right * map.cellSize * x;
                for (var y = 0; y < map.linesY; y++)
                {
                    var nodePosition = nodeBasePosition + Vector2.up * map.cellSize * y;
                    if (NodeIsAir(map, nodePosition))
                    {
                        var nodeBelowPosition = nodePosition + Vector2.down * map.cellSize;
                        if (NodeIsGround(map, nodeBelowPosition))
                        {
                            grid[y * map.linesX + x] = new PathNode
                                { position = nodePosition, x = x, y = y, ground = true };
                            AddVisualConnection(map, nodePosition, map.cellSize / 4f, Color.green, 1);
                        }
                        else
                        {
                            grid[y * map.linesX + x] = new PathNode
                                { position = nodePosition, x = x, y = y, air = true };
                        }
                    }
                }
            }
        }

        private static bool NodeIsAir(Pathfinding map, Vector2 nodePosition)
        {
            return !Physics2D.OverlapPoint(nodePosition, map.layerWorld);
        }

        private static bool NodeIsGround(Pathfinding map, Vector2 nodePosition)
        {
            return Physics2D.OverlapPoint(nodePosition, map.layerWorld);
        }
    }
}
#endif