using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D9;

namespace Heiflow.Visualization.DirectX
{
    /// <summary>与 MDX 的 Microsoft.DirectX.Direct3D.ExtendedMaterial 对齐。</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ExtendedMaterial
    {
        public Material Material3D;
        public string TextureFilename;
    }

    /// <summary>
    /// SharpDX 的 Direct3D9 绑定没有包裹 D3DX 的网格相关入口
    /// （D3DXCreateMeshFVF / D3DXLoadMeshFromX / D3DXComputeNormals / D3DXSaveTextureToFile），
    /// 这里用少量 P/Invoke 补齐，避免改动既有业务逻辑。
    /// 依赖 DirectX 9.0c 运行时的 d3dx9_43.dll（与 SharpDX 自身的 Effect/Font 要求一致）。
    /// </summary>
    public static class NativeD3DX
    {
        const string D3dxLib = "d3dx9_43.dll";

        // Any CPU 进程不一定会探测 x86\/x64 子目录下的 native DLL（SQLite.Interop.dll 能那样
        // 部署，是因为 System.Data.SQLite 自己实现了探测逻辑，这里没有）。DllImport 只按系统
        // 搜索路径找 d3dx9_43.dll，所以显式按进程位数 LoadLibrary 一次；之后同名 DllImport
        // 会直接复用这个已加载模块。
        static NativeD3DX()
        {
            EnsureNativeLibrary();
        }

        /// <summary>
        /// 优先用系统已装的 DirectX 9.0c 运行时；找不到时回退到输出目录的 x64\/x86 子目录。
        /// 全部失败也不抛 —— 由首次 P/Invoke 抛出 DllNotFoundException，错误信息更直白。
        /// </summary>
        static void EnsureNativeLibrary()
        {
            try
            {
                if (LoadLibrary(D3dxLib) != IntPtr.Zero)
                    return;

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                if (string.IsNullOrEmpty(baseDir))
                    return;

                string candidate = Path.Combine(baseDir, IntPtr.Size == 8 ? "x64" : "x86", D3dxLib);
                if (File.Exists(candidate))
                    LoadLibrary(candidate);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Preload " + D3dxLib + " failed: " + ex.Message);
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport(D3dxLib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        static extern int D3DXCreateMeshFVF(
            int numFaces, int numVertices, int options, int fvf,
            IntPtr device, out IntPtr mesh);

        [DllImport(D3dxLib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        static extern int D3DXLoadMeshFromXW(
            string fileName, int options, IntPtr device,
            out IntPtr adjacency, out IntPtr materials, out IntPtr effectInstances,
            out int numMaterials, out IntPtr mesh);

        [DllImport(D3dxLib, CallingConvention = CallingConvention.StdCall)]
        static extern int D3DXComputeNormals(IntPtr mesh, IntPtr adjacency);

        [DllImport(D3dxLib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        static extern int D3DXSaveTextureToFileW(
            string destFile, int destFormat, IntPtr srcTexture, IntPtr srcPalette);

        [StructLayout(LayoutKind.Sequential)]
        struct NativeExtendedMaterial
        {
            public Material Material3D;
            public IntPtr TextureFilename;
        }

        static void Check(int hr)
        {
            if (hr < 0)
                Marshal.ThrowExceptionForHR(hr);
        }

        /// <summary>MDX: new Mesh(numFaces, numVertices, meshFlags, vertexFormat, device)</summary>
        public static Mesh CreateMesh(int numFaces, int numVertices, MeshFlags options, VertexFormat fvf, Device device)
        {
            IntPtr meshPtr;
            Check(D3DXCreateMeshFVF(numFaces, numVertices, (int)options, (int)fvf, device.NativePointer, out meshPtr));
            return new Mesh(meshPtr);
        }

        /// <summary>MDX: Mesh.FromFile(fileName, meshFlags, device, out materials)</summary>
        public static Mesh LoadMeshFromX(Device device, string fileName, MeshFlags options, out ExtendedMaterial[] materials)
        {
            IntPtr adjacency, materialBuffer, effectInstances, meshPtr;
            int numMaterials;
            Check(D3DXLoadMeshFromXW(fileName, (int)options, device.NativePointer,
                out adjacency, out materialBuffer, out effectInstances, out numMaterials, out meshPtr));

            materials = ReadMaterials(materialBuffer, numMaterials);

            if (adjacency != IntPtr.Zero) Marshal.Release(adjacency);
            if (materialBuffer != IntPtr.Zero) Marshal.Release(materialBuffer);
            if (effectInstances != IntPtr.Zero) Marshal.Release(effectInstances);

            return new Mesh(meshPtr);
        }

        static ExtendedMaterial[] ReadMaterials(IntPtr buffer, int count)
        {
            var result = new ExtendedMaterial[count];
            if (buffer == IntPtr.Zero || count <= 0) return result;

            IntPtr dataPtr = GetBufferPointer(buffer);
            if (dataPtr == IntPtr.Zero) return result;

            int stride = Marshal.SizeOf(typeof(NativeExtendedMaterial));
            for (int i = 0; i < count; i++)
            {
                IntPtr p = IntPtr.Add(dataPtr, i * stride);
                var native = (NativeExtendedMaterial)Marshal.PtrToStructure(p, typeof(NativeExtendedMaterial));
                result[i].Material3D = native.Material3D;
                result[i].TextureFilename = native.TextureFilename != IntPtr.Zero
                    ? Marshal.PtrToStringAnsi(native.TextureFilename)
                    : null;
            }
            return result;
        }

        [ComImport, Guid("8BA5FB08-5192-40fe-9492-8B1F1F4CF0F4")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface ID3DXBuffer
        {
            [PreserveSig] IntPtr GetBufferPointer();
            int GetBufferSize();
        }

        static IntPtr GetBufferPointer(IntPtr buffer)
        {
            var com = (ID3DXBuffer)Marshal.GetObjectForIUnknown(buffer);
            try { return com.GetBufferPointer(); }
            finally { Marshal.ReleaseComObject(com); }
        }

        /// <summary>MDX: mesh.ComputeNormals()</summary>
        public static void ComputeNormals(Mesh mesh)
        {
            Check(D3DXComputeNormals(mesh.NativePointer, IntPtr.Zero));
        }

        /// <summary>MDX: TextureLoader.Save(fileName, imageFileFormat, texture)</summary>
        public static void SaveTextureToFile(Texture texture, string fileName, ImageFileFormat format)
        {
            Check(D3DXSaveTextureToFileW(fileName, (int)format, texture.NativePointer, IntPtr.Zero));
        }

        /// <summary>MDX: SurfaceLoader.Save(fileName, imageFileFormat, surface)</summary>
        public static void SaveSurfaceToFile(Surface surface, string fileName, ImageFileFormat format)
        {
            Check(D3DXSaveSurfaceToFileW(fileName, (int)format, surface.NativePointer, IntPtr.Zero));
        }

        [DllImport(D3dxLib, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        static extern int D3DXSaveSurfaceToFileW(
            string destFile, int destFormat, IntPtr srcSurface, IntPtr srcPalette);

        /// <summary>顶点数（MDX 的 mesh.NumberVertices）。</summary>
        public static int GetVertexCount(Mesh mesh)
        {
            int bytesPerVertex = mesh.NumBytesPerVertex;
            if (bytesPerVertex <= 0) return 0;
            return mesh.VertexBuffer.Description.SizeInBytes / bytesPerVertex;
        }

        /// <summary>面数（MDX 的 mesh.NumberFaces）。</summary>
        public static int GetFaceCount(Mesh mesh)
        {
            return GetIndexCount(mesh) / 3;
        }

        /// <summary>索引数。</summary>
        public static int GetIndexCount(Mesh mesh)
        {
            var desc = mesh.IndexBuffer.Description;
            bool is32 = desc.Format == Format.Index32;
            return desc.Size / (is32 ? 4 : 2);
        }

        /// <summary>MDX: Mesh.Cylinder(device, radius1, radius2, length, slices, stacks)</summary>
        public static Mesh CreateCylinder(Device device, float radius1, float radius2, float length, int slices, int stacks)
        {
            IntPtr meshPtr;
            Check(D3DXCreateCylinder(device.NativePointer, radius1, radius2, length, slices, stacks, out meshPtr, IntPtr.Zero));
            return new Mesh(meshPtr);
        }

        [DllImport(D3dxLib, CallingConvention = CallingConvention.StdCall)]
        static extern int D3DXCreateCylinder(IntPtr device, float radius1, float radius2, float length,
            int slices, int stacks, out IntPtr mesh, IntPtr adjacency);
    }
}
