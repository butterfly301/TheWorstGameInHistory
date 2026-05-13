/*
 * Copyright (c) 2023 MiniGames
 *
 * Check out how to use it here.
 * https://www.youtube.com/channel/UCrLZAN_rgpW7i84gDAHHH1g
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tiny
{
    public class Trail : MonoBehaviour
    {
        [SerializeField] [Tooltip("The material to apply to the trail.")]
        private Material material;

        [SerializeField] [Tooltip("Define the lifetime of a point in the trail, in seconds.")]
        private float duration = 0.1f;

        [SerializeField] [Tooltip("Increase this value to make the trail corners appear rounder.")]
        private int corner = 1;

        [SerializeField]
        [Tooltip("Enable this to connect the first and last positions of the line, and form a closed loop.")]
        private bool loop;

        [SerializeField] [Tooltip("The array of Vector3 points to connect.")]
        private Vector3[] points = { new(0f, 0f, -1f), new(0f, 0f, 1f) };

        [NonSerialized] private Transform cacheTM;
        [NonSerialized] private int lastCorner = -1;

        [NonSerialized] private int lastSegmentCount = -1;
        [NonSerialized] private Mesh mesh;
        [NonSerialized] private int pointCount = -1;
        [NonSerialized] private float toCornerT;

        [NonSerialized] private GameObject trailGo;

        private Coroutine update;

        [NonSerialized] private Vector3[] vertices;

        /// <summary>
        ///     The array of Vector3 points to connect.
        /// </summary>
        public Vector3[] Points
        {
            get => points;
            set => points = value;
        }

        /// <summary>
        ///     Enable this to connect the first and last positions of the line, and form a closed loop.
        /// </summary>
        public bool Loop => loop && points.Length >= 3;

        private void Start()
        {
            cacheTM = transform;

            trailGo = new GameObject(name + "Trail", typeof(MeshFilter), typeof(MeshRenderer));
            DontDestroyOnLoad(trailGo);

            mesh = new Mesh { name = "Trail Effect" };
            mesh.MarkDynamic();
            trailGo.GetComponent<MeshFilter>().sharedMesh = mesh;
            trailGo.layer = gameObject.layer;

            var meshRenderer = trailGo.GetComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;

            Initialize((int)(duration / Time.fixedDeltaTime));
        }

        private void LateUpdate()
        {
            if (cacheTM.hasChanged)
                TransformVertices();

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }

        private void OnEnable()
        {
            if (trailGo == null)
                return;

            trailGo.SetActive(true);
            Initialize((int)(duration / Time.fixedDeltaTime));
        }

        private void OnDisable()
        {
            if (trailGo)
                trailGo.SetActive(false);

            if (update != null)
                StopCoroutine(update);
            update = null;
        }

        private void OnDestroy()
        {
            if (mesh != null)
                DestroyImmediate(mesh);
            mesh = null;

            if (trailGo != null)
                DestroyImmediate(trailGo);
            trailGo = null;
        }

        /// <summary>
        ///     Removes all points from the TrailRenderer. Useful for restarting a trail from a new position.
        /// </summary>
        public void Clear()
        {
            if (!enabled || pointCount <= 1 || !trailGo)
                return;

            if (update != null)
                StopCoroutine(update);

            ClearVertices();

            update = StartCoroutine(PhysicsUpdate());
        }

        private void SetVerticesAndCorner()
        {
            var nextSegmentPoint = pointCount + pointCount * corner;

            Array.Copy(vertices, 0, vertices, nextSegmentPoint, vertices.Length - nextSegmentPoint);

            TransformVertices();

            var next2 = nextSegmentPoint * 2;
            var next3 = nextSegmentPoint * 3;

            for (var x = -1; ++x < pointCount;)
            {
                var a = vertices[x];
                var b = vertices[x + nextSegmentPoint];
                var c = vertices[x + next2];
                var d = vertices[x + next3];

                for (int n = -1, index = pointCount + x; ++n < corner; index += pointCount)
                {
                    var t = (n + 1) * toCornerT;
                    vertices[index] = CatmullRomSpline(a, a, b, c, t);
                    vertices[index + nextSegmentPoint] = CatmullRomSpline(a, b, c, d, t);
                }
            }
        }

        private void SetVertices()
        {
            Array.Copy(vertices, 0, vertices, pointCount, vertices.Length - pointCount);
            TransformVertices();
        }

        private IEnumerator PhysicsUpdate()
        {
            YieldInstruction wait = new WaitForFixedUpdate();

            Action action = corner > 0 ? SetVerticesAndCorner : SetVertices;

            while (true)
            {
                yield return wait;
                action();
                cacheTM.hasChanged = false;
            }
        }

        private void TransformVertices()
        {
            var localToWorldMatrix = cacheTM.localToWorldMatrix;
            for (var i = -1; ++i < pointCount;)
                vertices[i] = localToWorldMatrix.MultiplyPoint3x4(points[i]);
        }

        private void ClearVertices()
        {
            TransformVertices();

            for (var i = pointCount; i < vertices.Length; i += pointCount)
                Array.Copy(vertices, 0, vertices, i, pointCount);
        }

        private void Initialize(int segment)
        {
            var corner = segment >= 3 ? this.corner : 0;

            if (lastSegmentCount == segment && pointCount == points.Length && lastCorner == corner)
            {
                ClearVertices();

                update = StartCoroutine(PhysicsUpdate());
                return;
            }

            pointCount = points.Length;
            lastCorner = corner;
            lastSegmentCount = segment;

            if (pointCount <= 1)
            {
                mesh.Clear();
                return;
            }

            var segmentAndCorner = segment + segment * corner;

            var uvs = new Vector2[pointCount * (segmentAndCorner + 1)];

            var isLoop = Loop;

            var indexs = new int[(isLoop ? pointCount : pointCount - 1) * 6 * segmentAndCorner];

            var uv = new Vector2();

            var endPoint = pointCount - 1;

            var invSegment = 1f / segment;
            var invEnd = 1f / endPoint;
            toCornerT = 1f / (corner + 1);

            for (int y = -1, i = -1; ++y <= segment;)
            {
                uv.y = y * invSegment;
                for (var x = -1; ++x < pointCount;)
                {
                    uv.x = x * invEnd;
                    uvs[++i] = uv;
                }

                if (y == segment)
                    continue;

                for (var n = -1; ++n < corner;)
                {
                    uv.y = Mathf.Lerp(y * invSegment, (y + 1) * invSegment, (n + 1) * toCornerT);

                    for (var x = -1; ++x < pointCount;)
                    {
                        uv.x = x * invEnd;
                        uvs[++i] = uv;
                    }
                }
            }

            var index = 0;
            var lineCount = isLoop ? endPoint + 1 : endPoint;

            for (var y = -1; ++y < segmentAndCorner;)
            {
                var beginIndex = y * pointCount;
                var nextIndex = y * pointCount;
                if (isLoop)
                    beginIndex += endPoint;
                else
                    nextIndex += 1;

                for (var x = -1; ++x < lineCount; index += 6, beginIndex = nextIndex++)
                {
                    indexs[index + 0] = beginIndex;
                    indexs[index + 1] = beginIndex + pointCount;
                    indexs[index + 2] = nextIndex;
                    indexs[index + 3] = nextIndex;
                    indexs[index + 4] = beginIndex + pointCount;
                    indexs[index + 5] = nextIndex + pointCount;
                }
            }

            vertices = new Vector3[uvs.Length];
            ClearVertices();

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.SetIndices(indexs, MeshTopology.Triangles, 0);

            update = StartCoroutine(PhysicsUpdate());
        }

        /// <summary>
        ///     p1 과 p2 사이에 곡선을 생성한다.
        ///     t == 0 일 때 p1을, t == 1 일 때 p2를 리턴한다.
        /// </summary>
        private static Vector3 CatmullRomSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            var t2 = t * t;
            var t3 = t2 * t;
            return 0.5f * (2 * p1 + (-p0 + p2) * t + (2 * p0 - 5 * p1 + 4 * p2 - p3) * t2 +
                           (-p0 + 3 * p1 - 3 * p2 + p3) * t3);
        }

        public void SetMaterial(Material value)
        {
            material = value;
            if (trailGo != null) trailGo.GetComponent<MeshRenderer>().material = material;
        }
    }
}