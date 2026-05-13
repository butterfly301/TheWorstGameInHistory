using UnityEngine;

namespace TwoBitMachines
{
    public static class QuadMesh
    {
        private static readonly int[] tris = new int[6] { 0, 2, 1, 2, 3, 1 };

        private static readonly Vector2[] uv = new Vector2[4]
        {
            new(0, 0),
            new(1, 0),
            new(0, 1),
            new(1, 1)
        };

        private static readonly Vector3[] vertices = new Vector3[4]
        {
            new(0, 0, 0),
            new(1, 0, 0),
            new(0, 1, 0),
            new(1, 1, 0)
        };

        private static readonly Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward
        };

        public static Mesh Create()
        {
            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = tris;
            mesh.normals = normals;
            mesh.uv = uv;
            return mesh;
        }

        public static void Create(Mesh mesh)
        {
            mesh.vertices = vertices;
            mesh.triangles = tris;
            mesh.normals = normals;
            mesh.uv = uv;
        }

        public static Mesh Create(float sizex, float sizey)
        {
            var mesh = new Mesh();
            var v = new Vector3[4]
            {
                new(0, 0),
                new(sizex, 0),
                new(0, sizey),
                new(sizex, sizey)
            };
            mesh.vertices = v;
            mesh.triangles = tris;
            mesh.normals = normals;
            mesh.uv = uv;
            return mesh;
        }
    }
}