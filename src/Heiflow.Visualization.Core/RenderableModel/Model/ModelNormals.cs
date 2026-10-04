using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Heiflow.Visualization.Renderable.Grid
{
    /// <summary>
    /// Normal computation of the 3D model meshes.
    /// </summary>
    public static class ModelNormals
    {
        /// <summary>
        /// Triangles shorter than this are degenerate and do not contribute a normal.
        /// </summary>
        private const float MinLength = 1e-8f;

        /// <summary>
        /// Averages the normals of the faces that share a vertex and stores a unit length normal on it.
        ///
        /// The version this replaces kept one <see cref="System.Collections.Generic.List{T}"/> per vertex,
        /// so a grid with 100 000 vertices allocated 100 000 list objects plus their internal arrays before
        /// a single normal was computed, and it added the face normals to whatever the vertex carried
        /// already. The vertex buffers of the grid renderers store the longitude, the latitude and the
        /// value in the normal fields while the mesh is built, so the result was a vector that was neither
        /// a proper normal nor normalised, which made the sun shading wrong. Two flat arrays are used here
        /// and the normals are rebuilt from zero.
        /// </summary>
        public static void Calculate(CustomVertex.PositionNormalColored[] vertices, int[] indices)
        {
            if (vertices == null || indices == null || vertices.Length == 0 || indices.Length < 3)
                return;

            int vertexCount = vertices.Length;
            Vector3[] sums = new Vector3[vertexCount];
            int[] hits = new int[vertexCount];
            int triangleCount = indices.Length / 3;

            for (int t = 0; t < triangleCount; t++)
            {
                int i0 = indices[t * 3];
                int i1 = indices[t * 3 + 1];
                int i2 = indices[t * 3 + 2];

                if (i0 < 0 || i0 >= vertexCount || i1 < 0 || i1 >= vertexCount || i2 < 0 || i2 >= vertexCount)
                    continue;

                Vector3 p0 = vertices[i0].Position;
                Vector3 p1 = vertices[i1].Position;
                Vector3 p2 = vertices[i2].Position;

                Vector3 face = Vector3.Cross(p1 - p0, p2 - p0);
                float length = Length(ref face);
                if (length < MinLength)
                    continue;

                face.X /= length;
                face.Y /= length;
                face.Z /= length;

                sums[i0] += face;
                sums[i1] += face;
                sums[i2] += face;

                hits[i0]++;
                hits[i1]++;
                hits[i2]++;
            }

            for (int i = 0; i < vertexCount; i++)
            {
                if (hits[i] == 0)
                    continue;

                Vector3 normal = sums[i];
                float length = Length(ref normal);
                if (length < MinLength)
                    continue;

                normal.X /= length;
                normal.Y /= length;
                normal.Z /= length;
                vertices[i].Normal = normal;
            }
        }

        private static float Length(ref Vector3 v)
        {
            return (float)System.Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        }
    }
}
