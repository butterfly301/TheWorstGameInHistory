using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TwoBitMachines.MapSystem
{
    [Serializable]
    public class MeshData
    {
        [SerializeField] public ushort shapeOffset;
        [SerializeField] public List<ushort> triangles = new();
        [SerializeField] public List<Color32> colors = new();
        [SerializeField] public List<Vector3> vertices = new();

        public void Clear()
        {
            shapeOffset = 0;
            colors.Clear();
            vertices.Clear();
            triangles.Clear();
        }

        public void Add(MeshData meshData)
        {
            colors.AddRange(meshData.colors);
            vertices.AddRange(meshData.vertices);
            var count = meshData.triangles.Count;
            for (var i = 0; i < count; i++) triangles.Add((ushort)(shapeOffset + meshData.triangles[i]));
            shapeOffset += (ushort)meshData.vertices.Count;
        }

        public void Add(MeshData meshData, Vector3 offset)
        {
            var triangleCount = meshData.triangles.Count;
            var verticeCount = meshData.vertices.Count;

            colors.AddRange(meshData.colors);

            for (var i = 0; i < verticeCount; i++) vertices.Add(meshData.vertices[i] + offset);
            for (var i = 0; i < triangleCount; i++) triangles.Add((ushort)(shapeOffset + meshData.triangles[i]));
            shapeOffset += (ushort)meshData.vertices.Count;
        }

        public void Set(Mesh mesh, MeshFilter meshFilter = null)
        {
            if (mesh == null) mesh = new Mesh();

            mesh.Clear(); // avoid setting error
            mesh.SetVertices(vertices, 0, vertices.Count, MeshUpdateFlags.DontRecalculateBounds);
            mesh.SetTriangles(triangles, 0, false);
            mesh.SetColors(colors);

            if (meshFilter != null)
                // mesh.bounds = boundaryShape.Bounds();
                meshFilter.mesh = mesh;
        }

        public void ChangeMeshColor(Color color)
        {
            for (var i = 0; i < colors.Count; i++) colors[i] = color;
        }

        public MeshData Copy(MeshData copyData)
        {
            Clear();
            vertices.AddRange(copyData.vertices);
            triangles.AddRange(copyData.triangles);
            colors.AddRange(copyData.colors);
            return this;
        }
    }
}