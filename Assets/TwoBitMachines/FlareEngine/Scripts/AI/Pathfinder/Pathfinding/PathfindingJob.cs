using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
    [BurstCompile]
    public struct PathfindingJob : IJob
    {
        [ReadOnly] public NativeMultiHashMap<int, int> jobNeighbors;
        [ReadOnly] public NativeArray<PathNodeStruct> jobGrid;
        [ReadOnly] public PathNodeStruct start;
        [ReadOnly] public float characterSizeY;
        [ReadOnly] public int linesX;
        [ReadOnly] public int linesY;

        public NativeList<int> result;
        public PathNodeStruct target;

        public void Execute()
        {
            CheckTarget();
            var openSet = new NativeList<int>(Allocator.Temp);
            var closedSet = new NativeList<int>(Allocator.Temp);
            var neighbors = new NativeList<PathNodeStruct>(Allocator.Temp);
            var nodeCost = new NativeHashMap<int, NodeCost>(jobGrid.Length, Allocator.Temp);
            openSet.Add(start.index);
            nodeCost.Add(start.index, new NodeCost(0, 0, 0));

            var foundPath = false;
            var canSearch = false;
            var nearestTarget = 0;
            var nearestDistance = Mathf.Infinity;

            while (openSet.Length > 0)
            {
                var node = jobGrid[openSet[0]];

                for (var i = 1; i < openSet.Length; i++)
                {
                    var openSetNode = jobGrid[openSet[i]];
                    if (nodeCost[openSetNode.index].fCost() <= nodeCost[node.index].fCost())
                        if (nodeCost[openSetNode.index].hCost < nodeCost[node.index].hCost)
                            node = openSetNode;
                }

                for (var i = 0; i < openSet.Length; i++)
                    if (openSet[i] == node.index)
                    {
                        openSet.RemoveAtSwapBack(i);
                        break;
                    }

                closedSet.Add(node.index);

                if (node.index == target.index)
                {
                    foundPath = true;
                    RetracePath(start.index, target.index, nodeCost);
                    break;
                }

                neighbors.Clear();
                if (jobNeighbors.ContainsKey(node.index))
                {
                    var values = jobNeighbors.GetValuesForKey(node.index);
                    while (values.MoveNext())
                    {
                        var neighborIndex = values.Current;
                        if (neighborIndex >= 0 && neighborIndex < jobGrid.Length)
                        {
                            var connectNode = jobGrid[neighborIndex];
                            if (connectNode.height > characterSizeY || connectNode.ceiling)
                            {
                                neighbors.Add(connectNode);
                                if (!nodeCost.ContainsKey(neighborIndex))
                                    nodeCost.Add(neighborIndex, new NodeCost(0, 0, 0));
                            }
                        }
                    }
                }

                for (var x = -1; x <= 1; x++)
                for (var y = -1; y <= 1; y++)
                {
                    var block = x + y;
                    if (block != 1 && block != -1) // don't test corners or middle nodes
                        continue;
                    var checkX = node.x + x;
                    var checkY = node.y + y;

                    if (checkX >= 0 && checkX < linesX && checkY >= 0 && checkY < linesY)
                    {
                        var nextNode = jobGrid[checkY * linesX + checkX];
                        if (nextNode.height < characterSizeY && !nextNode.ceiling)
                            continue; //tile.ground &&

                        // if (node.bridge && nextNode.bridge && y != 0)
                        //      continue;
                        if (node.Path() && nextNode.Path())
                        {
                            neighbors.Add(nextNode);
                            if (!nodeCost.ContainsKey(nextNode.index))
                                nodeCost.Add(nextNode.index, new NodeCost(0, 0, 0));
                        }
                    }
                }

                for (var i = 0; i < neighbors.Length; i++)
                {
                    var neighbour = jobGrid[neighbors[i].index];

                    var moveCostToNeighbour = nodeCost[node.index].gCost +
                                              GetDistance(node.x, node.y, neighbour.x,
                                                  neighbour.y); // nodeCost[node.index].penalty;

                    if (closedSet.Contains(neighbour.index) &&
                        moveCostToNeighbour >= nodeCost[neighbour.index].gCost) continue;

                    if (moveCostToNeighbour < nodeCost[neighbour.index].gCost || !openSet.Contains(neighbour.index))
                    {
                        var gCost = moveCostToNeighbour;
                        var hCost = GetDistance(neighbour.x, neighbour.y, target.x, target.y);
                        nodeCost[neighbour.index] = new NodeCost(gCost, hCost, node.index);

                        if (hCost < nearestDistance && neighbour.ground)
                        {
                            nearestDistance = hCost;
                            nearestTarget = neighbour.index;
                            canSearch = true;
                        }

                        if (!openSet.Contains(neighbour.index)) openSet.Add(neighbour.index);
                    }
                }
            }

            if (canSearch && !foundPath)
            {
                if (start.ground &&
                    GetDistance(start.x, start.y, target.x, target.y) <
                    nearestDistance) // star path is actually closer, stay here
                    nearestTarget = start.index;
                RetracePath(start.index, nearestTarget, nodeCost);
            }

            openSet.Dispose();
            closedSet.Dispose();
            neighbors.Dispose();
            nodeCost.Dispose();
        }

        public int
            GetDistance(int grid_a_X, int grid_a_Y, int grid_b_X,
                int grid_b_Y) // don't pass struct since they are being copied
        {
            var dX = math.abs(grid_a_X - grid_b_X);
            var dY = math.abs(grid_a_Y - grid_b_Y);
            return dX + dY; //manhattan heuristic
        }

        public void RetracePath(int start, int end, NativeHashMap<int, NodeCost> nodeCost)
        {
            result.Clear();
            var currentNode = end;
            while (currentNode != start)
            {
                result.Add(currentNode);
                currentNode = nodeCost[currentNode].parent;
            }

            result.Add(start);

            if (result.Length > 0 &&
                jobGrid[result[0]].edgeDrop)
                result.RemoveAt(
                    0); // if final target point is an edgeDrop, remove, since an edgeDrop is just air, ai should't move into air, edge drop useful for detecting ambiguous states and drops
        }

        public void CheckTarget()
        {
            if (target.air && !target.moving && !target.bridge && !target.edgeDrop)
                for (var i = 0; i <= 5; i++)
                    if (target.y - i >= 0 && target.y - i < linesY)
                    {
                        var ground = jobGrid[(target.y - i) * linesX + target.x];
                        if (ground.Path())
                        {
                            target = ground;
                            return;
                        }
                    }
        }
    }

    public struct NodeCost
    {
        public int gCost;
        public int hCost;
        public int parent;

        public Vector2Int parentV2;

        //  public int penalty;
        public int fCost()
        {
            return gCost + hCost;
        }

        public NodeCost(int newGCost, int newHCost, int newParent)
        {
            gCost = newGCost;
            hCost = newHCost;
            parent = newParent;
            parentV2 = Vector2Int.zero;
        }

        public NodeCost(int newGCost, int newHCost, Vector2Int newParent)
        {
            gCost = newGCost;
            hCost = newHCost;
            parentV2 = newParent;
            parent = 0;
        }
    }
}