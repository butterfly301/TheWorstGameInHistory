#if UNITY_EDITOR
using TwoBitMachines.FlareEngine;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.Editors
{
    public class LadderNodesEditor : AIPathfindingEditor
    {
        public static void Execute(Pathfinding map)
        {
            if (map.ladder.Count != 0)
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
                if (tempNode != null && map.ladder.Contains(new Vector2Int(tempNode.x, tempNode.y)))
                    tempNode.ladder = true;
            }
        }

        public static void Connections(Pathfinding map)
        {
            for (var x = 0; x < map.linesX; x++)
            for (var y = 0; y < map.linesY; y++)
            {
                var node = map.Node(x, y);
                if (node != null && node.ladder)
                {
                    ConnectLadders(node, map, x, y);
                    ConnectLaddersToGround(node, map, x, y);
                }
            }
        }

        private static void ConnectLadders(PathNode ladder, Pathfinding map, int x, int y)
        {
            for (var searchRight = 0;
                 searchRight < map.maxJumpDistance;
                 searchRight++) //      search for ladders from left to right
            {
                var search = map.Node(x + 1 + searchRight, y);
                if (search != null && search.ladder &&
                    !search.ground) //                                                       found a ladder to jump to.
                    for (var searchRightX = 0; searchRightX < map.maxJumpDistance; searchRightX++)
                    {
                        var ladderToJumpTo = map.Node(x + 1 + searchRightX, y);
                        if (ladderToJumpTo == null)
                            continue;
                        if (ladderToJumpTo.ladder)
                        {
                            AddNodeToParent(ladder, ladderToJumpTo,
                                map); //   connect ladder points, and draw air nodes between them
                            return;
                        }

                        AddVisualConnection(map, ladderToJumpTo.position, map.cellSize / 4f, Color.cyan, 1);
                    }
            }
        }

        private static void ConnectLaddersToGround(PathNode ladder, Pathfinding map, int x, int y)
        {
            var jumpCount = 0; // connect air ladders to ground for ai to jump to
            for (var searchDown = 1; searchDown <= map.maxJumpHeight; searchDown++)
            {
                var ground = map.Node(x, y - searchDown);
                if (ground == null || ground.ladder) return;
                jumpCount++;
                if (ground.ground || ground.jumpThroughGround)
                {
                    for (var searchDownY = 1; searchDownY <= map.maxJumpHeight; searchDownY++)
                    {
                        ground = map.Node(x, y - searchDownY);
                        if (ground == null)
                            continue;
                        if (ground.ground || ground.jumpThroughGround)
                        {
                            ground.exact = true;
                            AddNodeToParent(ladder, ground, map);
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