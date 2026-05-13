using System.Collections.Generic;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    public class ConnectJumpNodes
    {
        public static List<Vector2> airNode = new();

        public static void
            Execute(PathfindingRT map) //                       Connect edges so that ai can jump from platform to platform
        {
            foreach (var node in map.map.Values)
                if (node != null && (node.leftCorner || node.rightCorner))
                    SearchForCorners(map.map, map, node);
        }

        private static void SearchForCorners(DictionaryMap grid, PathfindingRT map, PathNode cornerA)
        {
            foreach (var cornerB in map.map.Values)
            {
                if (cornerB == null || (!cornerB.leftCorner && !cornerB.rightCorner) ||
                    cornerA.Same(cornerB)) // cornerB is not valid
                    continue;
                if (CornersNotRelativeToEachOther(map, cornerA, cornerB)) continue;
                if (!CornersWithinJumpingDistance(map, cornerA, cornerB)) continue;
                if (cornerB.y >= cornerA.y && VerticalBlock(grid, cornerA, cornerB.y)) continue;
                if (HorizontalBlock(grid, map, cornerA, cornerB)) continue;
                if (CornerNextToWall(map, cornerA, cornerB)) continue;
                PathIsValidConnectNodes(map, cornerA, cornerB);
            }
        }

        private static bool CornersNotRelativeToEachOther(PathfindingRT map, PathNode cornerA, PathNode cornerB)
        {
            // left edge is to the right and higher than right edge. Vice versa. If both false, exit out.
            if (cornerA.leftCorner && cornerB.rightCorner && cornerA.x > cornerB.x && cornerB.y >= cornerA.y)
                return false; // can jump from left bottom to top right
            if (cornerA.rightCorner && cornerB.leftCorner && cornerA.x < cornerB.x && cornerB.y >= cornerA.y)
                return false; // can jump from right bottom to top left
            return true;
        }

        private static bool CornersWithinJumpingDistance(PathfindingRT map, PathNode cornerA, PathNode cornerB)
        {
            var withinDistanceX = Mathf.Abs(cornerA.x - cornerB.x) <= map.maxJump.x;
            var withinDistanceY = Mathf.Abs(cornerA.y - cornerB.y) <= map.maxJump.y;
            return withinDistanceX && withinDistanceY;
        }

        private static bool CornerNextToWall(PathfindingRT map, PathNode cornerA, PathNode cornerB)
        {
            if (cornerA.leftCorner && cornerB.rightCorner && cornerA.nextToWall && cornerB.x < cornerA.x - 1)
                return true; // wall is between corners. Corner might already have a connection on said wall. Exit out.
            if (cornerA.rightCorner && cornerB.leftCorner && cornerA.nextToWall && cornerB.x > cornerA.x + 1)
                return true;
            return false;
        }

        private static bool HorizontalBlock(DictionaryMap grid, PathfindingRT map, PathNode cornerA, PathNode cornerB)
        {
            if (cornerB.x >= cornerA.x)
                for (var searchX = cornerA.x; searchX <= cornerB.x; searchX++)
                {
                    grid.TryGetValue(new Vector2Int(searchX, cornerB.y), out var connectNode);
                    if (connectNode != null && connectNode.block) return true;
                }
            else
                for (var searchX = cornerB.x; searchX <= cornerA.x; searchX++)
                {
                    grid.TryGetValue(new Vector2Int(searchX, cornerB.y), out var connectNode);
                    if (connectNode != null && connectNode.block) return true;
                }

            return false;
        }

        private static bool VerticalBlock(DictionaryMap grid, PathNode cornerA, int y)
        {
            for (var searchY = cornerA.y + 1; searchY <= y; searchY++)
            {
                grid.TryGetValue(new Vector2Int(cornerA.x, searchY), out var connectNode);
                if (connectNode != null && (connectNode.block || !connectNode.air || connectNode.height < 1))
                    return true;
            }

            return false;
        }

        private static void PathIsValidConnectNodes(PathfindingRT map, PathNode cornerA, PathNode cornerB)
        {
            cornerA.AddNeighbor(cornerB);

#if UNITY_EDITOR
            airNode.Clear();
            var offset = new Vector2(map.cellSize * 0.5f, map.cellSize * 0.5f);
            var start = cornerA.y > cornerB.y ? cornerB : cornerA;
            var end = cornerA.y > cornerB.y ? cornerA : cornerB;

            for (var searchY = start.y + 1; searchY <= end.y; searchY++)
            {
                airNode.Add(new Vector2(start.x, searchY) + offset);
                if (searchY == end.y)
                {
                    if (end.x >= start.x)
                        for (var searchX = start.x + 1; searchX < end.x; searchX++)
                            airNode.Add(new Vector2(searchX, end.y) + offset);
                    else if (end.x <= start.x)
                        for (var searchX = end.x + 1; searchX < start.x; searchX++)
                            airNode.Add(new Vector2(searchX, end.y) + offset);
                }
            }

            for (var i = 0; i < airNode.Count; i++)
                PathfindingRT.AddNodeDrawing(map, airNode[i], map.cellSize / 4f, Color.cyan, 1);
#endif
        }
    }
}