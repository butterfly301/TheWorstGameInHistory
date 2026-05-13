using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.MapSystem
{
    public class GenerateRoomMesh
    {
        public static void ResetMesh(Room room, Zone zone)
        {
            room.roomMesh.Clear();
            room.outlineMesh.Clear();
            PathUtil.previousVertices.Clear();
            PathUtil.triangulateVertices.Clear();
            var pointList = room.pointList;
            var lineList = room.lineList;

            if (pointList.Count > 1) GenerateConnectedPoints(zone, room, pointList, zone.outline, zone.thickness);

            if (lineList.Count > 1) GenerateLines(zone, room, lineList, zone.outline, zone.thickness);

            PathUtil.RemoveDuplicatePoints(PathUtil.triangulateVertices);
            PathUtil.RemoveColinearPoints(PathUtil.triangulateVertices);

            PathUtil.Triangulate(PathUtil.triangulateVertices, zone.background, room.roomMesh.shapeOffset);
            MeshInfo.AddMeshInfoTo(room.roomMesh);
        }

        private static void GenerateConnectedPoints(Zone zone, Room room, List<Point> pointList, Color32 color,
            float thickness)
        {
            PathUtil.firstVertices.Clear();
            for (var i = 0; i < pointList.Count; i++)
            {
                var nextIndex = i >= pointList.Count - 1 ? 0 : i + 1;
                var lastIndex = i - 1 < 0 ? pointList.Count - 1 : i - 1;

                var previous = pointList[lastIndex];
                var point = pointList[i];
                var next = pointList[nextIndex];

                MeshInfo.ClearTempLists();
                PathUtil.CreatePath(pointList, i, nextIndex, thickness, zone.resolution * 0.2f);

                if (PathUtil.currentVertices.Count < 2 || point.invisible) continue;
                if (i == 0) PathUtil.firstVertices.AddRange(PathUtil.currentVertices);

                if (previous.invisible && room.useRoundEnds)
                {
                    var current = new CornerData(PathUtil.currentVertices, false);
                    PathUtil.CreateHalfCircle(room.outlineMesh, current.center, -current.direction, thickness, color,
                        8);
                }
                else if (!previous.invisible && PathUtil.previousVertices.Count > 0)
                {
                    var previousPath = new CornerData(PathUtil.previousVertices, true);
                    PathUtil.CreateElbow(room.outlineMesh, previousPath.center, previousPath.left,
                        PathUtil.currentVertices[1], thickness, 6, color);
                }

                if (i == pointList.Count - 1 && PathUtil.firstVertices.Count > 0)
                {
                    var previousPath = new CornerData(PathUtil.currentVertices, true);
                    PathUtil.CreateElbow(room.outlineMesh, previousPath.center, previousPath.left,
                        PathUtil.firstVertices[1], thickness, 6, color);
                }

                PathUtil.CreateMeshLine(PathUtil.currentVertices, room.outlineMesh, color);

                if (next.invisible && room.useRoundEnds && i < pointList.Count - 1)
                {
                    var currentPath = new CornerData(PathUtil.currentVertices, true);
                    PathUtil.CreateHalfCircle(room.outlineMesh, currentPath.center, -currentPath.direction, thickness,
                        color, 8);
                }

                PathUtil.previousVertices.Clear();
                PathUtil.previousVertices.AddRange(PathUtil.currentVertices);
                MeshInfo.AddMeshInfoTo(room.outlineMesh);
            }
        }

        private static void GenerateLines(Zone zone, Room room, List<Point> lineList, Color32 color, float thickness)
        {
            for (var i = 0; i < lineList.Count; i += 2)
            {
                var nextIndex = i >= lineList.Count - 1 ? 0 : i + 1;

                MeshInfo.ClearTempLists();
                PathUtil.CreatePath(lineList, i, nextIndex, thickness, zone.resolution * 0.5f, addTriangles: false);

                if (PathUtil.currentVertices.Count < 2) continue;
                if (room.useRoundEnds)
                {
                    var current = new CornerData(PathUtil.currentVertices, false);
                    PathUtil.CreateHalfCircle(room.outlineMesh, current.center, -current.direction, thickness, color,
                        8);
                }

                PathUtil.CreateMeshLine(PathUtil.currentVertices, room.outlineMesh, color);

                if (room.useRoundEnds)
                {
                    var current = new CornerData(PathUtil.currentVertices, true);
                    PathUtil.CreateHalfCircle(room.outlineMesh, current.center, -current.direction, thickness, color,
                        8);
                }

                PathUtil.previousVertices.Clear();
                PathUtil.previousVertices.AddRange(PathUtil.currentVertices);
                MeshInfo.AddMeshInfoTo(room.outlineMesh);
            }
        }

        public struct CornerData
        {
            public Vector2 left;
            public Vector2 right;
            public Vector2 center => (left + right) * 0.5f;
            public Vector2 direction => (left - right).normalized;

            public CornerData(List<Vector2> list, bool last)
            {
                left = right = Vector2.zero;
                if (list.Count >= 2)
                {
                    left = list[last ? list.Count - 1 : 0];
                    right = list[last ? list.Count - 2 : 1];
                }
            }
        }
    }
}