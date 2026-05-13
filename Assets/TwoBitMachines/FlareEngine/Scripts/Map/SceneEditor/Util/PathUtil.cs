using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.MapSystem
{
    public class PathUtil
    {
        public static bool cull = true;
        public static float colinear = 0.00002f;
        public static float overlap = 0.001f;
        private static readonly float steps = 10f;
        private static readonly int triangulationLimit = 1000;
        private static bool useSharpCorner;
        private static float cornerWidth = 0.1f;

        public static List<int> index = new();
        public static List<ushort> triangles = new();
        public static List<Color32> colors = new();
        public static List<Vector2> vertices = new();

        public static List<Vector2> triangulateVertices = new();
        public static List<Vector2> currentVertices = new();
        public static List<Vector2> tempVertices = new();
        public static List<Vector2> previousVertices = new();
        public static List<Vector2> firstVertices = new();

        public static void CreatePath(List<Point> pointList, int indexA, int indexB, float thickness, float resolution,
            bool createLine = true, bool addTriangles = true)
        {
            tempVertices.Clear();
            currentVertices.Clear();

            var bezierPointA = pointList[indexA];
            var bezierPointB = pointList[indexB];

            var length = Length(bezierPointA, bezierPointB);
            var deltaTime = resolution == 0 ? 0.01f : 1f / (steps * resolution * length);
            GetPath(bezierPointA, bezierPointB, 0, 1f, deltaTime);

            if (createLine)
            {
                if (addTriangles) triangulateVertices.AddRange(tempVertices);
                PathThickness(thickness);
            }
        }

        private static void GetPath(Point pointA, Point pointB, float start, float end, float inc)
        {
            if (cull && pointA.offsetStart == Vector2.zero &&
                pointB.offsetEnd == Vector2.zero) // if there is no curve, do not calculate bezier, much faster
            {
                AddTempPoint(pointA, pointB, start);
                AddTempPoint(pointA, pointB, end);
                return;
            }

            for (var time = start; time <= end; time += inc)
            {
                AddTempPoint(pointA, pointB, time);
                if (time < end && time + inc >= end)
                {
                    AddTempPoint(pointA, pointB, end, true);
                    return;
                }
            }
        }

        private static void PathThickness(float thickness)
        {
            if (tempVertices.Count == 1) return;

            var neighbor = 1;
            for (var i = 0; i < tempVertices.Count; i++)
            {
                var isLast = i == tempVertices.Count - 1;
                var currentPoint = tempVertices[i];
                var nextPoint = tempVertices[isLast ? i - 1 : i + 1];
                var direction = isLast ? currentPoint - nextPoint : nextPoint - currentPoint;

                for (var j = 1; j <= neighbor; j++)
                {
                    if (i + j < tempVertices.Count) direction += tempVertices[i + j] - currentPoint;
                    if (i - j >= 0) direction += currentPoint - tempVertices[i - j];
                }

                direction = new Vector2(-direction.y, direction.x).normalized;

                SetPoint(-1, currentPoint, direction,
                    thickness); // direction used to create left and right bezier points
                SetPoint(+1, currentPoint, direction, thickness);
            }
        }

        private static void SetPoint(int sign, Vector2 tempPoint, Vector2 tempDirection, float width)
        {
            var direction = tempDirection * width * sign * 0.5f;
            var position = tempPoint + direction;
            currentVertices.Add(position);
        }

        private static void AddTempPoint(Point pointA, Point pointB, float time, bool checkForSamePoint = false)
        {
            var position = Vector2.zero;
            if (pointA.offsetStart == Vector2.zero && pointB.offsetEnd == Vector2.zero)
                position = Vector2.Lerp(pointA.position, pointB.position, Mathf.Clamp(time, 0, 1f)); //
            else
                position = BezierPoint(pointA, pointB, Mathf.Clamp(time, 0, 1f));
            if (cull && checkForSamePoint && tempVertices.Count > 0 &&
                tempVertices[tempVertices.Count - 1] == position) return;
            tempVertices.Add(position);
            if (cull && tempVertices.Count > 2 && ArePointsCollinear(tempVertices))
                tempVertices.RemoveAt(tempVertices.Count - 2);
        }

        public static float Length(Point a, Point b)
        {
            //https://stackoverflow.com/questions/29438398/cheap-way-of-calculating-cubic-bezier-length faster method of estimating bezier arc length
            var line = (b.position - a.position).magnitude;
            var cont_net = (a.position - a.controlStart).magnitude + (b.controlEnd - a.controlStart).magnitude +
                           (b.position - b.controlEnd).magnitude;
            var length = (cont_net + line) / 2f;
            return length;
        }

        public static Vector2 BezierPoint(Point a, Point b, float t)
        {
            var u = 1f - t;
            var tt = t * t;
            var uu = u * u;
            var uuu = uu * u;
            var ttt = tt * t;

            var p = uuu * a.position;
            p += 3f * uu * t * a.controlStart;
            p += 3f * u * tt * b.controlEnd;
            p += ttt * b.position;
            return p;
        }

        public static void CreateMeshLine(List<Vector2> vertices, MeshData meshData, Color color)
        {
            var verticeCount = MeshInfo.vertices.Count;

            for (var i = 0; i < vertices.Count - 1; i += 2)
            {
                MeshInfo.AddPoint(vertices[i + 0], color);
                MeshInfo.AddPoint(vertices[i + 1], color);

                if (i < vertices.Count - 2)
                {
                    MeshInfo.AddTriangle(meshData.shapeOffset + verticeCount, i, i + 1, i + 3);
                    MeshInfo.AddTriangle(meshData.shapeOffset + verticeCount, i, i + 3, i + 2);
                }
            }
        }

        public static void CreateElbow(MeshData meshData, Vector2 center, Vector2 lastPosition, Vector2 nextPosition,
            float width, int divisions, Color32 color)
        {
            var direction1 = lastPosition - center;
            var direction2 = nextPosition - center;
            var rawAngle = Vector2.Angle(direction1, direction2);
            useSharpCorner = false;

            if (rawAngle == 0) return;
            if (rawAngle <= 90.1f) divisions = 2;

            var verticeCount = MeshInfo.vertices.Count;
            var stepAngle = rawAngle / divisions;
            var sign = direction1.CrossSign(direction2) < 0 ? 1 : -1;

            if (rawAngle <= 90.1f)
            {
                var cornerDir1 = direction1.Rotate(-90f * sign);
                var cornerDir2 = direction2.Rotate(90f * sign);
                if (Compute.LineIntersection(nextPosition, nextPosition + cornerDir2 * 2f, lastPosition,
                        lastPosition + cornerDir1 * 2f, out var intersect))
                {
                    cornerWidth = (intersect - center).magnitude;
                    useSharpCorner = true;
                }
            }

            direction1 = sign > 0 ? direction1 : -direction1;
            stepAngle = sign > 0 ? -Mathf.Abs(stepAngle) : stepAngle;
            direction1.Normalize();

            MeshInfo.AddPoint(center, color);

            for (var i = 0; i <= divisions; i++)
            {
                var radius = width * 0.5f;
                if (i == 1 && useSharpCorner) radius = cornerWidth;
                MeshInfo.AddPoint(center + (direction1 * radius).Rotate(stepAngle * i), color);
                MeshInfo.AddTriangle(meshData.shapeOffset + verticeCount, 0, sign < 0 ? i + 1 : i,
                    sign < 0 ? i : i + 1);
            }
        }

        public static void CreateHalfCircle(MeshData meshData, Vector2 center, Vector2 direction, float diameter,
            Color color, int cap)
        {
            var angle = 180f / cap;
            var radius = diameter * 0.5f;
            var verticeCount = MeshInfo.vertices.Count;
            MeshInfo.AddPoint(center, color);

            for (var i = 0; i <= cap; i++)
            {
                MeshInfo.AddPoint(center + direction.Rotate(angle * i) * radius, color);
                MeshInfo.AddTriangle(meshData.shapeOffset + verticeCount, 0, i + 1, i);
            }
        }

        public static void Triangulate(List<Vector2> points, Color color, ushort indexOffset)
        {
            var limit = 0;
            MeshInfo.ClearTempLists();
            var index = MeshInfo.index;
            var triangles = MeshInfo.triangles;
            var colors = MeshInfo.colors;
            var vertices = MeshInfo.vertices;

            if (points == null || points.Count < 3) return;

            if (Compute.IsClockwise(points)) points.Reverse();

            for (var i = 0; i < points.Count; i++)
            {
                vertices.Add(points[i]);
                colors.Add(color);
                index.Add(i);
            }

            while (index.Count > 3 && limit++ <= triangulationLimit) // limit will prevent infinite loops
                for (var i = 0; i < index.Count; i++)
                {
                    var indexA = i - 1 < 0 ? index[index.Count - 1] : index[i - 1];
                    var indexB = index[i];
                    var indexC = i + 1 > index.Count - 1 ? index[0] : index[i + 1];

                    Vector2 pointA = vertices[indexA];
                    Vector2 pointB = vertices[indexB];
                    Vector2 pointC = vertices[indexC];

                    if ((pointA - pointB).CrossSign(pointC - pointB) < 0f) continue;

                    var isTriangle = true;
                    for (var j = 0; j < vertices.Count; j++)
                    {
                        if (j == indexB || j == indexA || j == indexC) continue;
                        if (Compute.IsPointInTriangle(pointA, pointB, pointC, vertices[j]))
                        {
                            isTriangle = false;
                            break;
                        }
                    }

                    if (isTriangle)
                    {
                        triangles.Add((ushort)(indexA + indexOffset));
                        triangles.Add((ushort)(indexB + indexOffset));
                        triangles.Add((ushort)(indexC + indexOffset));
                        index.RemoveAt(i);
                        break;
                    }
                }

            triangles.Add((ushort)(index[0] + indexOffset));
            triangles.Add((ushort)(index[1] + indexOffset));
            triangles.Add((ushort)(index[2] + indexOffset));

            if (limit > triangulationLimit) Debug.Log("Animesh: possible mesh triangulation error");
        }

        public static void RemoveDuplicatePoints(List<Vector2> vertices)
        {
            float startCount = vertices.Count;
            var sqrMagnitude = overlap * overlap;

            for (var i = 1; i < vertices.Count; i++) // start at 1, do not remove first point
            {
                var next = vertices[i];
                var last = vertices[i - 1];
                if ((next - last).sqrMagnitude < sqrMagnitude)
                {
                    vertices.RemoveAt(i);
                    i--;
                }
                else if (i == vertices.Count - 1 && vertices.Count > 1)
                {
                    if ((next - vertices[0]).sqrMagnitude < sqrMagnitude)
                    {
                        vertices.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public static void RemoveColinearPoints(List<Vector2> vertices)
        {
            for (var i = 1; i < vertices.Count - 1; i++)
                if (ArePointsCollinear(vertices[i - 1], vertices[i], vertices[i + 1]))
                {
                    vertices.RemoveAt(i);
                    i--;
                }
        }

        public static bool ArePointsCollinear(List<Vector2> vertices)
        {
            var p1 = vertices[vertices.Count - 3];
            var p2 = vertices[vertices.Count - 2];
            var p3 = vertices[vertices.Count - 1];
            var a = (p2.y - p1.y) * (p3.x - p2.x);
            var b = (p3.y - p2.y) * (p2.x - p1.x);
            return Mathf.Abs(a - b) < colinear;
        }

        public static bool ArePointsCollinear(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            var a = (p2.y - p1.y) * (p3.x - p2.x);
            var b = (p3.y - p2.y) * (p2.x - p1.x);
            return Mathf.Abs(a - b) < colinear;
        }
    }
}