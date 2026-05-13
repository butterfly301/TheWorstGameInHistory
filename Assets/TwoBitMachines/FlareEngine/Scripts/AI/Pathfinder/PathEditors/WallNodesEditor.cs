#if UNITY_EDITOR
using TwoBitMachines.FlareEngine;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.Editors
{
    public class WallNodesEditor : AIPathfindingEditor
    {
        public static void Execute(Pathfinding map)
        {
            if (map.wall.Count != 0)
            {
                Register(map);
                Connections(map);
            }
        }

        public static void Register(Pathfinding map)
        {
            for (var x = 0; x < map.linesX; x++)
            for (var y = 0; y < map.linesY; y++)
            {
                var tempNode = map.Node(x, y);
                if (tempNode != null && map.wall.Contains(new Vector2Int(tempNode.x, tempNode.y)))
                {
                    tempNode.wall = true;
                    var topNode = tempNode.ShiftY(map, 1);
                    var right = topNode.ShiftX(map, 1);
                    var left = topNode.ShiftX(map, -1);
                    if (right != null && (right.ground || right.jumpThroughGround))
                        AddNodeToParent(tempNode, right, map, false);
                    if (left != null && (left.ground || left.jumpThroughGround))
                        AddNodeToParent(tempNode, left, map, false);
                    if (Physics2D.OverlapPoint(tempNode.position - Vector2.right, map.layerWorld))
                        tempNode.wallLeft = true;
                }
            }
        }

        public static void Connections(Pathfinding map)
        {
            for (var x = 0; x < map.linesX; x++)
            for (var y = 0; y < map.linesY; y++)
            {
                var wall = map.Node(x, y);
                if (wall != null && wall.wall)
                {
                    ConnectWalls(wall, map, x, y);
                    ConnectWallsToGround(wall, map, x, y);
                }
            }
        }

        private static void ConnectWalls(PathNode wall, Pathfinding map, int x, int y)
        {
            for (var searchRight = 0;
                 searchRight < map.maxJumpDistance;
                 searchRight++) //                              Search for walls from left to right
            {
                var search = map.Node(x + 1 + searchRight, y);
                if (search != null &&
                    search.wall) //                                                               found a wall to jump to.
                    for (var searchRightX = 0; searchRightX < map.maxJumpDistance; searchRightX++)
                    {
                        var wallToJumpTo = map.Node(x + 1 + searchRightX, y);
                        if (wallToJumpTo.wall)
                        {
                            AddNodeToParent(wall, wallToJumpTo, map);
                            return;
                        }

                        AddVisualConnection(map, wallToJumpTo.position, map.cellSize / 4f, Color.cyan, 1);
                    }
            }
        }

        private static void ConnectWallsToGround(PathNode wall, Pathfinding map, int x, int y)
        {
            var jumpCount = 0;
            for (var searchDown = 1;
                 searchDown <= map.maxJumpHeight;
                 searchDown++) //                                Connect air walls to ground for ai to jump to
            {
                var ground = map.Node(x, y - searchDown);
                if (ground == null || ground.wall) return;
                jumpCount++;
                if (ground.ground || ground.jumpThroughGround)
                {
                    for (var searchDownY = 1; searchDownY <= map.maxJumpHeight; searchDownY++)
                    {
                        ground = map.Node(x, y - searchDownY);
                        if (ground.ground || ground.jumpThroughGround)
                        {
                            ground.exact = true;
                            AddNodeToParent(wall, ground, map);
                            break;
                        }

                        AddVisualConnection(map, ground.position, map.cellSize / 4f, Color.cyan, 1);
                    }

                    return;
                }
            }
        }
    }
}
#endif