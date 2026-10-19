using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D9;

namespace Heiflow.Visualization.DirectX
{
    /// <summary>
    /// 顶点结构兼容层。
    /// SlimDX / SharpDX 均未提供 MDX 的 Microsoft.DirectX.Direct3D.CustomVertex，
    /// 这里按 MDX 的字段名与内存布局原样重建（MDX 使用扁平的 X/Y/Z/Nx/Ny/Nz/Rhw/Tu/Tv/Color 字段，
    /// 而非嵌套的 Position/Normal 向量），使既有代码无需改动。
    /// </summary>
    public static class CustomVertex
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PositionColored
        {
            public float X;
            public float Y;
            public float Z;
            public int Color;

            public Vector3 Position
            {
                get { return new Vector3(X, Y, Z); }
                set { X = value.X; Y = value.Y; Z = value.Z; }
            }

            public PositionColored(Vector3 pos, int c) { X = pos.X; Y = pos.Y; Z = pos.Z; Color = c; }
            public PositionColored(float x, float y, float z, int c) { X = x; Y = y; Z = z; Color = c; }

            public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Diffuse;
            public static readonly int StrideSize = 16;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionNormalColored
        {
            public float X;
            public float Y;
            public float Z;
            public float Nx;
            public float Ny;
            public float Nz;
            public int Color;

            public Vector3 Position
            {
                get { return new Vector3(X, Y, Z); }
                set { X = value.X; Y = value.Y; Z = value.Z; }
            }

            public Vector3 Normal
            {
                get { return new Vector3(Nx, Ny, Nz); }
                set { Nx = value.X; Ny = value.Y; Nz = value.Z; }
            }

            public PositionNormalColored(Vector3 pos, Vector3 n, int c)
            { X = pos.X; Y = pos.Y; Z = pos.Z; Nx = n.X; Ny = n.Y; Nz = n.Z; Color = c; }

            public PositionNormalColored(float x, float y, float z, float nx, float ny, float nz, int c)
            { X = x; Y = y; Z = z; Nx = nx; Ny = ny; Nz = nz; Color = c; }

            public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Diffuse;
            public static readonly int StrideSize = 28;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionNormalTextured
        {
            public float X;
            public float Y;
            public float Z;
            public float Nx;
            public float Ny;
            public float Nz;
            public float Tu;
            public float Tv;

            public Vector3 Position
            {
                get { return new Vector3(X, Y, Z); }
                set { X = value.X; Y = value.Y; Z = value.Z; }
            }

            public Vector3 Normal
            {
                get { return new Vector3(Nx, Ny, Nz); }
                set { Nx = value.X; Ny = value.Y; Nz = value.Z; }
            }

            public PositionNormalTextured(Vector3 pos, Vector3 n, float tu, float tv)
            { X = pos.X; Y = pos.Y; Z = pos.Z; Nx = n.X; Ny = n.Y; Nz = n.Z; Tu = tu; Tv = tv; }

            public PositionNormalTextured(float x, float y, float z, float nx, float ny, float nz, float tu, float tv)
            { X = x; Y = y; Z = z; Nx = nx; Ny = ny; Nz = nz; Tu = tu; Tv = tv; }

            public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture1;
            public static readonly int StrideSize = 32;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionTextured
        {
            public float X;
            public float Y;
            public float Z;
            public float Tu;
            public float Tv;

            public Vector3 Position
            {
                get { return new Vector3(X, Y, Z); }
                set { X = value.X; Y = value.Y; Z = value.Z; }
            }

            public PositionTextured(Vector3 pos, float tu, float tv)
            { X = pos.X; Y = pos.Y; Z = pos.Z; Tu = tu; Tv = tv; }

            public PositionTextured(float x, float y, float z, float tu, float tv)
            { X = x; Y = y; Z = z; Tu = tu; Tv = tv; }

            public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Texture1;
            public static readonly int StrideSize = 20;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionColoredTextured
        {
            public float X;
            public float Y;
            public float Z;
            public int Color;
            public float Tu;
            public float Tv;

            public Vector3 Position
            {
                get { return new Vector3(X, Y, Z); }
                set { X = value.X; Y = value.Y; Z = value.Z; }
            }

            public PositionColoredTextured(Vector3 pos, int c, float tu, float tv)
            { X = pos.X; Y = pos.Y; Z = pos.Z; Color = c; Tu = tu; Tv = tv; }

            public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Diffuse | VertexFormat.Texture1;
            public static readonly int StrideSize = 24;
        }

        /// <summary>D3DFVF_XYZRHW | D3DFVF_DIFFUSE。Z 为屏幕深度，Rhw 为齐次除法倒数。</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct TransformedColored
        {
            public float X;
            public float Y;
            public float Z;
            public float Rhw;
            public int Color;

            public Vector3 Position
            {
                get { return new Vector3(X, Y, Z); }
                set { X = value.X; Y = value.Y; Z = value.Z; }
            }

            public TransformedColored(float x, float y, float z, float rhw, int c)
            { X = x; Y = y; Z = z; Rhw = rhw; Color = c; }

            public static readonly VertexFormat Format = VertexFormat.PositionRhw | VertexFormat.Diffuse;
            public static readonly int StrideSize = 20;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TransformedColoredTextured
        {
            public float X;
            public float Y;
            public float Z;
            public float Rhw;
            public int Color;
            public float Tu;
            public float Tv;

            public Vector3 Position
            {
                get { return new Vector3(X, Y, Z); }
                set { X = value.X; Y = value.Y; Z = value.Z; }
            }

            public TransformedColoredTextured(float x, float y, float z, float rhw, int c, float tu, float tv)
            { X = x; Y = y; Z = z; Rhw = rhw; Color = c; Tu = tu; Tv = tv; }

            public static readonly VertexFormat Format = VertexFormat.PositionRhw | VertexFormat.Diffuse | VertexFormat.Texture1;
            public static readonly int StrideSize = 28;
        }
    }
}
