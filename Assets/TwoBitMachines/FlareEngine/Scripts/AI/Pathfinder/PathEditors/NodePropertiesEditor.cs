#if UNITY_EDITOR
using TwoBitMachines.FlareEngine;
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.Editors
{
    public class NodePropertiesEditor : AIPathfindingEditor
    {
        public static void SetNodeHeadSpace(Pathfinding map)
        {
            for (var x = 0; x < map.linesX; x++)
            for (var y = 0; y < map.linesY; y++)
            {
                var tempNode = map.grid[y * map.linesX + x];
                if (tempNode == null) continue;
                SetNodeHeight(tempNode, map, x, y);
            }
        }

        private static void SetNodeHeight(PathNode tempNode, Pathfinding map, int x, int y)
        {
            var height = 0;
            for (var searchUp = y; searchUp < map.linesY; searchUp++)
            {
                var checkY = map.Node(x, searchUp);
                if (checkY == null) break;
                height++;
            }

            tempNode.height =
                height; //                                                            get head space for each node     
        }

        public static void TurnNullNodesIntoBlocks(Pathfinding map, Vector2 startingPosition, Vector2 cellOffset)
        {
            for (var x = 0; x < map.linesX; x++) // check for edge colliders
            for (var y = 0; y < map.linesY; y++)
            {
                var isnull = map.grid[y * map.linesX + x];
                if (isnull == null)
                {
                    var position = startingPosition + cellOffset + Vector2.right * map.cellSize * x +
                                   Vector2.up * map.cellSize * y;
                    map.grid[y * map.linesX + x] = new PathNode { position = position, x = x, y = y, block = true };
                }
            }
        }
    }
}
#endif