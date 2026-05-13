#if UNITY_EDITOR
using TwoBitMachines.FlareEngine;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.Editors
{
    public class EdgesNodesEditor : AIPathfindingEditor
    {
        private static readonly int searchLeft = -1;
        private static readonly int searchRight = 1;

        public static void Execute(Pathfinding map)
        {
            for (var x = 0; x < map.linesX; x++)
            for (var y = 0; y < map.linesY; y++)
            {
                var cornerNode = map.grid[y * map.linesX + x];
                if (cornerNode == null) continue;
                if (cornerNode.leftCorner && FindCornerEdge(cornerNode, map, searchLeft, out var cornerEdgeNodeA) &&
                    NoLadders(map, x, y, searchLeft))
                    FindEdgeDrops(cornerNode, cornerEdgeNodeA, map, EdgeDropLength(map, x, y, searchLeft), searchLeft);
                if (cornerNode.rightCorner && FindCornerEdge(cornerNode, map, searchRight, out var cornerEdgeNodeB) &&
                    NoLadders(map, x, y, searchRight))
                    FindEdgeDrops(cornerNode, cornerEdgeNodeB, map, EdgeDropLength(map, x, y, searchRight),
                        searchRight);
            }
        }

        private static bool FindCornerEdge(PathNode cornerNode, Pathfinding map, int sign, out PathNode cornerEdgeNode)
        {
            cornerEdgeNode = map.Node(cornerNode.x + sign, cornerNode.y);
            if (cornerEdgeNode != null && !cornerEdgeNode.ladder && !cornerEdgeNode.bridge && cornerEdgeNode.air) //  
            {
                cornerEdgeNode.edgeOfCorner = true;
                AddNodeToParent(cornerNode, cornerEdgeNode, map);
                return true;
            }

            return false;
        }

        private static void FindEdgeDrops(PathNode cornerNode, PathNode cornerEdgeNode, Pathfinding map, int dropLength,
            int sign)
        {
            if (dropLength >
                map.maxJumpHeight) // This is a big drop, player can't jump to platform but it can fall from it.
                for (var searchDown = cornerNode.y; searchDown >= 0; searchDown--)
                {
                    var dropNode = map.Node(cornerNode.x + sign, searchDown);
                    if (dropNode != null && dropNode.height > 1 && dropNode.ground)
                    {
                        AddNodeToParent(cornerNode, dropNode, map, false);
                        for (var searchDownA = cornerNode.y; searchDownA >= 0; searchDownA--)
                        {
                            var dropNodeA = map.Node(cornerNode.x + sign, searchDownA);
                            if (dropNodeA == null || dropNodeA.ground)
                                break;
                            AddVisualConnection(map, dropNodeA.position, map.cellSize / 8f, Color.red, 1);
                        }

                        return;
                    }
                }
            else
                for (var searchDown = cornerNode.y; searchDown >= 0; searchDown--)
                {
                    var dropNode = map.Node(cornerNode.x + sign, searchDown);
                    if (dropNode != null && dropNode.height > 1 && dropNode.ground)
                    {
                        dropNode.exact = !cornerNode.jumpThroughGround &&
                                         UseExactPoint(map, cornerNode, dropNode, sign);
                        CreateSecondDropPoint(map, dropNode, cornerNode, sign);
                        AddNodeToParent(cornerNode, dropNode, map);
                        AddVisualConnection(map, dropNode.position, map.cellSize / 8f,
                            dropNode.exact ? Color.magenta : Color.green, 1);
                        for (var searchDownA = cornerNode.y; searchDownA >= 0; searchDownA--)
                        {
                            var dropNodeA = map.Node(cornerNode.x + sign, searchDownA);
                            if (dropNodeA == null || dropNodeA.ground)
                                break;
                            AddVisualConnection(map, dropNodeA.position, map.cellSize / 4f, Color.cyan, 1);
                        }

                        return;
                    }
                }
        }

        private static int EdgeDropLength(Pathfinding map, int x, int y, int sign)
        {
            var length = 0;
            for (var searchDown = y; searchDown >= 0; searchDown--)
            {
                var dropNode = map.Node(x + sign, searchDown);
                if (dropNode == null || !dropNode.air)
                    break;
                length++;
            }

            return length;
        }

        private static bool NoLadders(Pathfinding map, int x, int y, int sign)
        {
            for (var searchDown = y; searchDown >= 0; searchDown--)
            {
                var dropNode = map.Node(x + sign, searchDown);
                if (dropNode == null)
                    return true;
                if (dropNode.ladder)
                    return false;
            }

            return true;
        }

        private static bool UseExactPoint(Pathfinding map, PathNode cornerNode, PathNode dropNode, int direction)
        {
            var distanceY =
                (int)cornerNode
                    .DistanceY(dropNode); // we look for any air gaps below platform, if any are found, we set to exact point
            for (var searchDown = 1; searchDown <= distanceY; searchDown++)
            {
                var airNode = cornerNode.Shift(map, 0, -searchDown);
                if (airNode != null && (airNode.air || airNode.ground))
                    return true;
            }

            // If no air gaps are found, then we check for tunnels, i.e - another platform that is exactly one unit away from this corner.
            return cornerNode.ShiftX(map, direction * 2) == null || cornerNode.Shift(map, direction * 2, -1) == null;
        }
    }
}
#endif