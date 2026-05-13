using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    public class AddRTWall
    {
        public static void Execute(PathfindingRT map)
        {
            if (map.wall.Count != 0) ConnectWalls(map);
        }

        public static void ConnectWalls(PathfindingRT map)
        {
            for (var i = 0; i < map.wall.Count; i++)
                if (map.Node(map.wall[i].x, map.wall[i].y, out var wall) && wall.wall)
                {
                    if (map.Node(wall.x + 1, wall.y + 1, out var right) && right.ground) wall.AddNeighbor(right, false);
                    if (map.Node(wall.x - 1, wall.y + 1, out var left) && left.ground) wall.AddNeighbor(left, false);

                    ConnectWalls(map, wall);
                    ConnectWallsToGround(map, wall);
                }
        }

        private static void ConnectWalls(PathfindingRT map, PathNode node)
        {
            for (var x = 1; x <= map.maxJump.x; x++) //      search for ladders from left to right
                if (map.Node(node.x + x, node.y, out var otherNode))
                {
                    if (otherNode.ground) return;
                    if (otherNode.wall)
                    {
                        node.AddNeighbor(otherNode);
#if UNITY_EDITOR
                        PathfindingRT.AddHorizontalPath(map, node.position + Vector2.right * map.cellSize,
                            otherNode.position, map.cellSize / 4f, Color.cyan);
#endif
                        return;
                    }
                }
        }

        private static void ConnectWallsToGround(PathfindingRT map, PathNode node)
        {
            if (node.ground || map.wall.Contains(new Vector2Int(node.x, node.y - 1))) return;
            for (var y = node.y - 1; y >= node.y - 20; y--)
                if (map.Node(node.x, node.y - y, out var ground) && ground.ground)
                {
                    node.AddNeighbor(ground);
#if UNITY_EDITOR
                    PathfindingRT.AddVerticalPath(map, node.position + Vector2.down * map.cellSize, ground.position,
                        map.cellSize / 4f, Color.cyan);
#endif
                    return;
                }
        }
    }
}