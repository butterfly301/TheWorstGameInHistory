using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TwoBitMachines
{
    public static class Compute
    {
        public static List<int> listRange = new();
        public static List<Collider2D> contactResults = new();
        public static ContactFilter2D contactFilter;

        public static float Atan2(Vector3 direction)
        {
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        public static Quaternion Atan2Quaternion(Vector3 direction)
        {
            return Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
        }

        public static float Angle(Vector2 a, Vector2 b, out float angle)
        {
            return angle = Vector2.Angle(a, b);
        }

        public static float AngleDirection(Vector2 a, Vector2 b)
        {
            return Vector2.Angle(a, b) * a.CrossSign(b);
        }

        public static Vector2 HalfWayNormal(Vector2 a, Vector2 b)
        {
            return (a + b).normalized;
        }

        public static void RemoveItem<T>(this List<T> t, T u)
        {
            if (t.Contains(u)) t.Remove(u);
        }

        public static void AddItem<T>(this List<T> t, T u)
        {
            if (!t.Contains(u)) t.Add(u);
        }

        public static Vector2 ArchObject(Vector2 objectPosition, Vector2 targetPoint, float archHeight, float gravity)
        {
            if (gravity == 0)
                return Vector2.zero;
            var displacement = targetPoint - objectPosition;
            var newHeight = displacement.y > 0 ? Mathf.Abs(displacement.y) + archHeight : archHeight;
            var velocityY = Vector2.up * Mathf.Sqrt(-2f * gravity * newHeight);
            var calculation = Mathf.Sqrt(-2f * newHeight / gravity) +
                              Mathf.Sqrt(2f * (displacement.y - newHeight) / gravity);
            if (calculation == 0)
                return Vector2.zero;
            var velocityX = new Vector2(displacement.x, 0) / calculation;
            return velocityX + velocityY;
        }

        public static Vector2 Abs(Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        public static bool Between(float value, float min, float max)
        {
            return value > min && value <= max;
        }

        public static bool BetweenInclusive(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        public static Vector2 CameraSize(this Camera camera)
        {
            var height = camera.orthographicSize * 2f;
            var width = height * camera.aspect;
            return new Vector2(width, height);
        }

        public static bool ContainsLayer(LayerMask mask, int layer)
        {
            return (mask & (1 << layer)) != 0;
        }

        public static int OverlapCollider2D(Collider2D collider2D, LayerMask layer)
        {
            contactFilter.useLayerMask = true;
            contactFilter.useTriggers = true;
            contactFilter.layerMask = layer;
            return Physics2D.OverlapCollider(collider2D, contactFilter, contactResults);
        }

        public static int OverlapCircle(Vector2 position, float radius, LayerMask layer)
        {
            contactFilter.useLayerMask = true;
            contactFilter.useTriggers = true;
            contactFilter.layerMask = layer;
            return Physics2D.OverlapCircle(position, radius, contactFilter, contactResults);
        }

        public static Collider2D HitContactNearestResult(int hits, Vector3 position)
        {
            var distance = Mathf.Infinity;
            Collider2D target = null;
            for (var i = 0; i < hits; i++)
                if (contactResults[i] != null)
                {
                    var newDistance = (position - contactResults[i].transform.position).sqrMagnitude;
                    if (newDistance < distance)
                    {
                        distance = newDistance;
                        target = contactResults[i];
                    }
                }

            return target;
        }

        public static Collider2D HitContactRandomResult(int hits, Vector3 position)
        {
            Collider2D target = null;
            var i = Random.Range(0, hits);
            if (i >= 0 && i < hits) target = contactResults[i];
            return target;
        }

        public static bool InRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        public static float Abs(this float a)
        {
            return Mathf.Abs(a);
        }

        public static float Sign(this float a)
        {
            return Mathf.Sign(a);
        }

        public static Vector2 Clamp(Vector2 value, float min)
        {
            value.x = Mathf.Clamp(value.x, min, value.x); // this will use its own value as the max value
            value.y = Mathf.Clamp(value.y, min, value.y);
            return value;
        }

        public static float CrossSign(this Vector2 a, Vector2 b)
        {
            return Mathf.Sign(a.x * b.y - a.y * b.x);
        }

        public static float CrossProduct(this Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static Vector3 ScaleVector(this Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector2 Rotate(this Vector2 v, float angle)
        {
            var cos = Mathf.Cos(Mathf.Deg2Rad * angle);
            var sin = Mathf.Sin(Mathf.Deg2Rad * angle);
            return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
        }

        public static bool Valid(this RaycastHit2D hit)
        {
            return hit && hit.distance > 0 && hit.collider != null;
        }

        public static Vector2 RotateVector(Vector2 v, float cos, float sin)
        {
            return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
        }

        public static Vector2 RotateVector(Vector2 v, float angle)
        {
            var cos = Mathf.Cos(Mathf.Deg2Rad * angle);
            var sin = Mathf.Sin(Mathf.Deg2Rad * angle);
            return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
        }

        public static float SineWave(float phase, float speed, float amplitude)
        {
            return Mathf.Sin(Time.time * speed + phase) * amplitude;
        }

        public static float GetAngleFromVector(Vector3 dir)
        {
            dir = dir.normalized;
            var n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0)
                n += 360;
            return n;
        }

        public static Vector3 GetVectorFromAngle(float angle)
        {
            angle *= Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public static float Lerp(float from, float to, float timeDuration, ref float timeCounter)
        {
            timeDuration = timeDuration <= 0 ? 1f : timeDuration;
            timeCounter = Mathf.Clamp(timeCounter + Time.deltaTime, 0, timeDuration);
            return Mathf.Lerp(from, to, timeCounter / timeDuration);
        }

        public static float Lerp(float from, float to, float timeDuration, ref float timeCounter, out bool complete)
        {
            complete = false;
            timeDuration = timeDuration <= 0 ? 1f : timeDuration;
            timeCounter = Mathf.Clamp(timeCounter + Time.deltaTime, 0, timeDuration);
            var value = Mathf.Lerp(from, to, timeCounter / timeDuration);
            if (timeCounter >= timeDuration)
            {
                timeCounter = 0;
                complete = true;
                return to;
            }

            return value;
        }

        public static float LerpUnscaled(float from, float to, float timeDuration, ref float timeCounter,
            out bool complete)
        {
            complete = false;
            timeDuration = timeDuration <= 0 ? 0.001f : timeDuration;
            timeCounter = Mathf.Clamp(timeCounter + Time.unscaledDeltaTime, 0, timeDuration);
            var value = Mathf.Lerp(from, to, timeCounter / timeDuration);
            if (timeCounter >= timeDuration)
            {
                timeCounter = 0;
                complete = true;
                return to;
            }

            return value;
        }

        public static float LerpRunning(float from, float to, float timeDuration, ref float timeCounter,
            ref bool isRunning)
        {
            timeDuration = timeDuration <= 0 ? 0.001f : timeDuration;
            timeCounter = Mathf.Clamp(timeCounter + Time.deltaTime, 0, timeDuration);
            var value = Mathf.Lerp(from, to, timeCounter / timeDuration);
            if (timeCounter >= timeDuration)
            {
                timeCounter = 0;
                isRunning = false;
                return to;
            }

            return value;
        }

        public static float Lerp(float from, float to, float speed)
        {
            var t = 1f - Mathf.Pow(1f - speed, Time.deltaTime);
            if (float.IsNaN(t))
                t = 0.999f;
            return Mathf.Lerp(from, to, t); // speed must be between 0-1. 
        }

        public static float LerpUnscaled(float from, float to, float speed)
        {
            var t = 1f - Mathf.Pow(1f - speed, Time.unscaledDeltaTime);
            if (float.IsNaN(t))
                t = 0.999f;
            return Mathf.Lerp(from, to, t); // speed must be between 0-1. 
        }

        public static Vector2 Lerp(Vector2 from, Vector2 to, float speed)
        {
            var x = Lerp(from.x, to.x, speed);
            var y = Lerp(from.y, to.y, speed);
            return new Vector2(x, y);
        }

        public static Vector2 LerpNormal(Vector2 from, Vector2 to, float t)
        {
            var x = Mathf.Lerp(from.x, to.x, t);
            var y = Mathf.Lerp(from.y, to.y, t);
            return new Vector2(x, y);
        }

        public static Vector2 Lerp(Vector2 from, Vector2 to, Vector2 smooth)
        {
            var x = Lerp(from.x, to.x, smooth.x);
            var y = Lerp(from.y, to.y, smooth.y);
            return new Vector2(x, y);
        }

        public static Vector2 DistanceLerp(Vector2 from, Vector2 to, float speed, float startTime, Tween tween,
            out float percent)
        {
            var journeyLength = Vector2.Distance(from, to);
            var distanceCovered = (Time.time - startTime) * speed;
            percent = distanceCovered / journeyLength;
            return Vector2.Lerp(from, to, EasingFunction.Run(tween, percent));
        }

        public static Vector2 Lerp(Vector3 origin, Vector3 previousPosition, Vector3 position, float speed)
        {
            origin.x = SmootherLerp(origin.x, previousPosition.x, position.x, speed);
            origin.y = SmootherLerp(origin.y, previousPosition.y, position.y, speed);
            if (float.IsNaN(origin.y))
                origin.y = position.y;
            if (float.IsNaN(origin.x))
                origin.x = position.x;
            return origin;
        }

        public static Vector3 LerpToTarget(Vector3 origin, Vector3 target, ref Vector3 previousTarget, float Vx,
            float Vy, float speed)
        {
            var newSpeedX = Vx * speed;
            var newSpeedY = Vy * speed;
            origin.x = Vx >= 1f ? target.x :
                Vx <= 0 ? origin.x : SmootherLerp(origin.x, previousTarget.x, target.x, newSpeedX);
            origin.y = Vy >= 1f ? target.y :
                Vy <= 0 ? origin.y : SmootherLerp(origin.y, previousTarget.y, target.y, newSpeedY);
            if (float.IsNaN(origin.y))
                origin.y = target.y;
            if (float.IsNaN(origin.x))
                origin.x = target.x;
            previousTarget = target;
            return origin;
        }

        public static Vector3 LerpConditional(Vector3 from, Vector3 to, ref bool lerping, float smooth = 0.999999f)
        {
            if (lerping)
            {
                if ((from - to).sqrMagnitude < 0.1f) lerping = false;
                return Lerp(from, to, smooth);
            }

            return to;
        }

        public static bool IsPointInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
        {
            var ab = b - a;
            var ac = c - a;
            var ap = p - a;

            var u = (ap.x * ac.y - ap.y * ac.x) / (ab.x * ac.y - ab.y * ac.x);
            var v = (ap.y * ab.x - ap.x * ab.y) / (ab.x * ac.y - ab.y * ac.x);
            var w = 1f - u - v;

            return u >= 0f && u <= 1f && v >= 0f && v <= 1f && w >= 0f && w <= 1f;
        }

        public static bool IsClockwise(List<Vector2> points)
        {
            var sum = 0f;
            for (var i = 0; i < points.Count; i++)
            {
                var current = points[i];
                var next = points[(i + 1) % points.Count];
                sum += (next.x - current.x) * (next.y + current.y);
            }

            return sum < 0f;
        }

        //http://csharphelper.com/blog/2014/09/determine-where-a-line-intersects-a-circle-in-c/
        public static bool LineCircleIntersection(Vector2 center, float radius, Vector2 lineOrigin,
            Vector2 lineDirection, out Vector2 intersection)
        {
            intersection = Vector2.zero;
            var point1 =
                lineOrigin +
                lineDirection * radius *
                3f; // we create a line that intersects the circle at two points for the math to work, but we are only interested in one point of intersection
            var point2 =
                lineOrigin -
                lineDirection * radius *
                3f; // these two points will create a line segment that crosses the circle. For our purposes, LineOrigin must be withint the circle to get correct intersection
            var dx = point2.x - point1.x;
            var dy = point2.y - point1.y;
            var pcx = point1.x - center.x;
            var pcy = point1.y - center.y;
            var A = dx * dx + dy * dy;
            var B = 2f * (dx * pcx + dy * pcy);
            var C = pcx * pcx + pcy * pcy - radius * radius;
            var det = B * B - 4f * A * C;

            if (A <= 0.0000001f || det < 0) // No real solutions.
                return false;

            var t = (-B - Mathf.Sqrt(det)) / (2f * A);
            intersection = new Vector2(point1.x + t * dx, point1.y + t * dy);

            if (PointInsideCircle(center, intersection,
                    radius + 0.02f)) // add a bit too radius for floating point imprecision
                return true;
            return false;
        }

        public static Vector2 LineCircleIntersect(Vector2 center, float radius, Vector2 lineOrigin,
            Vector2 lineDirection)
        {
            var point1 =
                lineOrigin +
                lineDirection * radius *
                3f; // we create a line that intersects the circle at two points for the math to work, but we are only interested in one point of intersection
            var point2 =
                lineOrigin -
                lineDirection * radius *
                3f; // these two points will create a line segment that crosses the circle. For our purposes, LineOrigin must be withint the circle to get correct intersection
            var dx = point2.x - point1.x;
            var dy = point2.y - point1.y;
            var pcx = point1.x - center.x;
            var pcy = point1.y - center.y;
            var A = dx * dx + dy * dy;
            var B = 2f * (dx * pcx + dy * pcy);
            var C = pcx * pcx + pcy * pcy - radius * radius;
            var det = B * B - 4f * A * C;

            if (A <= 0.0000001f || det < 0) // no real solutions.
                return Vector2.zero;

            var t = (-B - Mathf.Sqrt(det)) / (2f * A);
            return new Vector2(point1.x + t * dx, point1.y + t * dy);
        }

        public static bool LineIntersection(Vector2 a, Vector2 b, Vector2 q, Vector2 r, out Vector2 intersectionPoint)
        {
            var deltaAB = b - a;
            var deltaQR = r - q;
            intersectionPoint = Vector2.zero;

            var denominator = deltaAB.y * deltaQR.x - deltaAB.x * deltaQR.y;

            var t1 = ((a.x - q.x) * deltaQR.y + (q.y - a.y) * deltaQR.x) / denominator;
            if (float.IsInfinity(t1))
                return false; // the lines are parallel (or close enough to it).

            var t2 = ((q.x - a.x) * deltaAB.y + (a.y - q.y) * deltaAB.x) / -denominator;

            // the segments intersect if t1 and t2 are between 0 and 1.
            if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
            {
                intersectionPoint =
                    new Vector2(a.x + deltaAB.x * t1, a.y + deltaAB.y * t1); // find the point of intersection.
                return true;
            }

            return false;
        }

        public static bool LineIntersectionRay(Vector2 a, Vector2 rayA, Vector2 b, Vector2 rayB,
            out Vector2 intersectionPoint)
        {
            var deltaAB = a + rayA * 1000f - a;
            var deltaQR = b + rayB * 1000f - b;
            intersectionPoint = Vector2.zero;

            // solve for t1 and t2
            var denominator = deltaAB.y * deltaQR.x - deltaAB.x * deltaQR.y;

            var t1 = ((a.x - b.x) * deltaQR.y + (b.y - a.y) * deltaQR.x) / denominator;
            if (float.IsInfinity(t1))
                return false; // the lines are parallel (or close enough to it).

            var t2 = ((b.x - a.x) * deltaAB.y + (a.y - b.y) * deltaAB.x) / -denominator;

            // the segments intersect if t1 and t2 are between 0 and 1.
            if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
            {
                intersectionPoint =
                    new Vector2(a.x + deltaAB.x * t1, a.y + deltaAB.y * t1); // find the point of intersection.
                return true;
            }

            return false;
        }

        public static bool LinesIntersect(Vector2 a, Vector2 b, Vector2 q, Vector2 r)
        {
            var deltaAB = b - a;
            var deltaQR = r - q;
            // solve for t1 and t2
            var denominator = deltaAB.y * deltaQR.x - deltaAB.x * deltaQR.y;
            var t1 = ((a.x - q.x) * deltaQR.y + (q.y - a.y) * deltaQR.x) / denominator;
            if (float.IsInfinity(t1))
                return false; // the lines are parallel (or close enough to it).
            var t2 = ((q.x - a.x) * deltaAB.y + (a.y - q.y) * deltaAB.x) / -denominator;
            // the segments intersect if t1 and t2 are between 0 and 1.
            return t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1;
        }

        public static float LineIntersectY(Vector2 p1, Vector2 p2, Vector2 testPoint)
        {
            var denom = p2.x - p1.x;
            if (denom == 0) return testPoint.y;

            var slope = (p2.y - p1.y) / (p2.x - p1.x); // Calculate the slope and y-intercept of the line
            var yIntercept = p1.y - slope * p1.x;
            return slope * testPoint.x + yIntercept; // Calculate the y-coordinate of the third point on the line
        }

        public static Quaternion LookAtDirection(Vector2 direction)
        {
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }

        public static Vector2 PointOnLine(Vector2 lineStart, Vector2 lineEnd, float x, Vector2 fallBack)
        {
            var diffX = lineEnd.x - lineStart.x;
            if (diffX == 0)
                return fallBack; // dont divide by zero
            var y = lineStart.y + (lineEnd.y - lineStart.y) / diffX * (x - lineStart.x);
            return new Vector2(x, y);
        }


        public static float PointOnLine(Vector2 lineStart, Vector2 lineEnd, float x, float fallBack)
        {
            var diffX = lineEnd.x - lineStart.x;
            if (diffX == 0)
                return fallBack; // dont divide by zero
            return lineStart.y + (lineEnd.y - lineStart.y) / diffX * (x - lineStart.x);
        }

        public static bool PointInsideCircle(Vector2 center, Vector2 point, float radius)
        {
            var x = point.x - center.x;
            var y = point.y - center.y;
            return x * x + y * y <= radius * radius;
        }

        public static Rect PositionToRect(Vector2 position, Vector2 size)
        {
            var b = Vector2.zero; // positions
            var c = Vector2.zero;
            if (size.y > 0)
            {
                b = position + Vector2.right * size.x; // find top left corner
                c = position + Vector2.down * size.y;
                var left = position.x < b.x ? position : b;
                var top = position.y > c.y ? position : c;
                return new Rect(left.x, top.y, Mathf.Abs(size.x), Mathf.Abs(size.y));
            }
            else
            {
                b = position + Vector2.right * size.x;
                c = position - Vector2.down * size.y;
                var left = position.x < b.x ? position : b;
                var top = c.y > position.y ? position : c;
                return new Rect(left.x, top.y, Mathf.Abs(size.x), Mathf.Abs(size.y));
            }
        }

        public static Vector2 RotatePoint(Vector2 centerOfRotation, Vector2 point, float angle)
        {
            angle = angle * Mathf.Deg2Rad; //Convert to radians
            var rotatedX = Mathf.Cos(angle) * (point.x - centerOfRotation.x) -
                Mathf.Sin(angle) * (point.y - centerOfRotation.y) + centerOfRotation.x;
            var rotatedY = Mathf.Sin(angle) * (point.x - centerOfRotation.x) +
                           Mathf.Cos(angle) * (point.y - centerOfRotation.y) + centerOfRotation.y;
            return new Vector2(rotatedX, rotatedY);
        }

        public static float Round(float value, float snapSize)
        {
            return (float)Math.Round(value / snapSize) * snapSize;
        }

        public static Rect RoundRect(Rect rect, float snapSize)
        {
            rect.x = (float)Math.Round(rect.x / snapSize) * snapSize;
            rect.y = (float)Math.Round(rect.y / snapSize) * snapSize;
            return rect;
        }

        public static Rect OffsetRect(Rect rect, Vector2 offset)
        {
            rect.x += offset.x;
            rect.y += offset.y;
            return rect;
        }

        public static Vector2 Round(Vector2 value, float snapSize)
        {
            snapSize = snapSize <= 0 ? 1 : snapSize;
            value.x = Round(value.x, snapSize);
            value.y = Round(value.y, snapSize);
            return value;
        }

        public static Vector2Int Floor(Vector2 value)
        {
            return new Vector2Int((int)Mathf.Floor(value.x), (int)Mathf.Floor(value.y));
        }

        public static Vector3 Round(Vector3 value, float snapSize)
        {
            snapSize = snapSize <= 0 ? 1 : snapSize;
            value.x = Round(value.x, snapSize);
            value.y = Round(value.y, snapSize);
            value.z = Round(value.z, snapSize);
            return value;
        }

        public static Vector3 Floor(Vector3 value, float snapSize)
        {
            snapSize = snapSize <= 0 ? 1 : snapSize;
            value.x = Mathf.Floor(value.x / snapSize) * snapSize;
            value.y = Mathf.Floor(value.y / snapSize) * snapSize;
            value.z = Mathf.Floor(value.z / snapSize) * snapSize;
            return value;
        }

        public static Vector3 Ceil(Vector3 value, float snapSize)
        {
            snapSize = snapSize <= 0 ? 1 : snapSize;
            value.x = Mathf.Ceil(value.x / snapSize) * snapSize;
            value.y = Mathf.Ceil(value.y / snapSize) * snapSize;
            value.z = Mathf.Ceil(value.z / snapSize) * snapSize;
            return value;
        }

        public static bool SameSign(float a, float b)
        {
            return
                (a > 0 && b > 0) ||
                (a < 0 && b <
                    0); // do a zero check for the first parameter, second parameter assumed to be either 1 or -1
        }

        public static void SwapInts(ref int a, ref int b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        public static void Swap<T>(List<T> list, int index1, int index2)
        {
            if (index1 < 0 || index1 >= list.Count || index2 < 0 || index2 >= list.Count) return;

            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        public static float SmootherLerp(float previousFollowPosition, float previousTargetPosition,
            float targetPosition, float speed)
        {
            var t = Time.deltaTime * speed;
            if (t == 0)
                return previousFollowPosition;
            var v = (targetPosition - previousTargetPosition) / t;
            var f = previousFollowPosition - previousTargetPosition + v;
            return targetPosition - v + f * Mathf.Exp(-t);
        }

        public static Vector2 SmootherLerp(Vector2 previousFollowPosition, Vector2 previousTargetPosition,
            Vector2 targetPosition, Vector2 speed)
        {
            Vector2 value;
            value.x = SmootherLerp(previousFollowPosition.x, previousTargetPosition.x, targetPosition.x, speed.x);
            value.y = SmootherLerp(previousFollowPosition.y, previousTargetPosition.y, targetPosition.y, speed.y);
            return value;
        }

        public static Vector2 Velocity(float angle, float magnitude)
        {
            return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * magnitude;
        }

        public static Vector2 Velocity(float angle, float radius, float signX)
        {
            return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * signX, Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
        }

        public static Vector2 VelocityReverse(float angle, float radius, float signX)
        {
            return new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad) * signX, Mathf.Cos(angle * Mathf.Deg2Rad)) * radius;
        }

        public static float WrapAngle(float angle)
        {
            return angle % 360 + (angle < 0 ? 360 : 0);
        }

        public static int WrapArrayIndex(int newIndex, int arraySize)
        {
            return newIndex >= arraySize ? 0 : newIndex < 0 ? arraySize - 1 : newIndex;
        }

        public static void ListRange(int index, ref int rangeEnd, int size, int range)
        {
            listRange.Clear();
            var count = 0;

            rangeEnd = Mathf.Clamp(rangeEnd, range - 1, int.MaxValue);
            var rangeStart = Mathf.Clamp(rangeEnd - range + 1, 0, int.MaxValue);

            rangeEnd = index > rangeEnd ? index : index < rangeStart ? index + range - 1 : rangeEnd;
            rangeStart = Mathf.Clamp(rangeEnd - range + 1, 0, int.MaxValue);

            for (var i = rangeStart; i < size; i++)
            {
                count++;
                listRange.Add(i);
                if (count == range) return;
            }
        }

        public static bool IndexInListRange(int index)
        {
            for (var i = 0; i < listRange.Count; i++)
                if (index == listRange[i])
                    return true;

            return false;
        }


        public static void FlipLocalPositionX(Transform position, float direction)
        {
            var p = position.localPosition;
            position.localPosition = new Vector3(direction > 0 ? Mathf.Abs(p.x) : -Mathf.Abs(p.x), p.y, p.z);
        }
    }
}