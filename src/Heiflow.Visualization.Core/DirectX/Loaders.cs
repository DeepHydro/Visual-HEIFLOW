using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SharpDX;
using SharpDX.Direct3D9;

namespace Heiflow.Visualization.DirectX
{
    /// <summary>
    /// MDX 的 Microsoft.DirectX.Direct3D.TextureLoader 兼容层。
    /// SlimDX / SharpDX 把这些工厂方法并入 Texture 类型本身。
    /// </summary>
    public static class TextureLoader
    {
        public static Texture FromFile(Device device, string fileName)
        {
            return Texture.FromFile(device, fileName);
        }

        public static Texture FromFile(Device device, string fileName, int width, int height, int mipLevels,
            Usage usage, Format format, Pool pool, Filter filter, Filter mipFilter, int colorKey)
        {
            return Texture.FromFile(device, fileName, width, height, mipLevels, usage, format, pool, filter, mipFilter, colorKey);
        }

        /// <summary>MDX 旧签名把 usage 作为 int 传入，这里做兼容。</summary>
        public static Texture FromFile(Device device, string fileName, int width, int height, int mipLevels,
            int usage, Format format, Pool pool, Filter filter, Filter mipFilter, int colorKey)
        {
            return Texture.FromFile(device, fileName, width, height, mipLevels, (Usage)usage, format, pool, filter, mipFilter, colorKey);
        }

        public static Texture FromStream(Device device, Stream stream)
        {
            return Texture.FromStream(device, stream);
        }

        public static Texture FromStream(Device device, Stream stream, int width, int height, int mipLevels,
            Usage usage, Format format, Pool pool, Filter filter, Filter mipFilter, int colorKey)
        {
            return Texture.FromStream(device, stream, width, height, mipLevels, usage, format, pool, filter, mipFilter, colorKey);
        }

        public static Texture FromStream(Device device, Stream stream, int sizeBytes, int width, int height, int mipLevels,
            Usage usage, Format format, Pool pool, Filter filter, Filter mipFilter, int colorKey)
        {
            return Texture.FromStream(device, stream, sizeBytes, width, height, mipLevels, usage, format, pool, filter, mipFilter, colorKey);
        }

        public static void Save(string fileName, ImageFileFormat format, Texture texture)
        {
            NativeD3DX.SaveTextureToFile(texture, fileName, format);
        }
    }

    /// <summary>
    /// MDX 的 Microsoft.DirectX.Direct3D.ShaderLoader 兼容层。
    /// </summary>
    public static class ShaderLoader
    {
        public static Effect FromStream(Device device, Stream stream, ShaderFlags flags)
        {
            return Effect.FromStream(device, stream, null, null, null, flags);
        }

        /// <summary>
        /// MDX: Effect.FromStream(device, stream, null, null, flags, pool, out errors)。
        /// SharpDX 没有带 out 错误串的重载（编译失败时抛异常），这里把异常消息填进 errors。
        /// </summary>
        public static Effect FromStream(Device device, Stream stream, ShaderFlags flags, EffectPool pool, out string compilationErrors)
        {
            compilationErrors = null;
            try
            {
                return Effect.FromStream(device, stream, null, null, null, flags, pool);
            }
            catch (SharpDXException ex)
            {
                compilationErrors = ex.Message;
                throw;
            }
        }

        /// <summary>MDX: Effect.FromFile(device, path, null, "", flags, pool, out errors)</summary>
        public static Effect FromFile(Device device, string fileName, ShaderFlags flags, EffectPool pool, out string compilationErrors)
        {
            compilationErrors = null;
            try
            {
                return Effect.FromFile(device, fileName, null, null, null, flags, pool);
            }
            catch (SharpDXException ex)
            {
                compilationErrors = ex.Message;
                throw;
            }
        }

        public static Effect FromFile(Device device, string fileName, ShaderFlags flags)
        {
            return Effect.FromFile(device, fileName, null, null, null, flags);
        }

        public static ShaderBytecode CompileFromStream(Stream stream, string entryPoint, string profile, ShaderFlags flags)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ShaderBytecode.Compile(ms.ToArray(), entryPoint, profile, flags);
            }
        }
    }

    /// <summary>与 MDX 的 AdapterInformation 对齐（MDX 用 .Information，SharpDX 用 .Details）。</summary>
    public sealed class AdapterInformation
    {
        public int Adapter { get; internal set; }
        public AdapterDetails Information { get; internal set; }

        internal AdapterInformation(int adapter, AdapterDetails details)
        {
            Adapter = adapter;
            Information = details;
        }
    }

    public sealed class AdapterList : IEnumerable<AdapterInformation>
    {
        readonly List<AdapterInformation> _items = new List<AdapterInformation>();

        internal AdapterList() { }

        internal void Add(AdapterInformation info) { _items.Add(info); }

        public int Count { get { return _items.Count; } }

        public AdapterInformation this[int index] { get { return _items[index]; } }

        public AdapterInformation Default { get { return _items.Count > 0 ? _items[0] : null; } }

        public IEnumerator<AdapterInformation> GetEnumerator() { return _items.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return _items.GetEnumerator(); }
    }

    /// <summary>
    /// MDX 的 Microsoft.DirectX.Direct3D.Manager 兼容层。
    /// SharpDX 把这些能力搬到 Direct3D 实例上，这里保留 MDX 的静态入口。
    /// </summary>
    public static class Manager
    {
        static Direct3D _direct3D;
        static AdapterList _adapters;
        static readonly object Sync = new object();

        public static Direct3D Direct3D
        {
            get
            {
                lock (Sync)
                {
                    if (_direct3D == null || _direct3D.IsDisposed)
                        _direct3D = new Direct3D();
                    return _direct3D;
                }
            }
        }

        public static AdapterList Adapters
        {
            get
            {
                lock (Sync)
                {
                    if (_adapters == null)
                    {
                        _adapters = new AdapterList();
                        var d3d = Direct3D;
                        for (int i = 0; i < d3d.AdapterCount; i++)
                            _adapters.Add(new AdapterInformation(i, d3d.Adapters[i].Details));
                    }
                    return _adapters;
                }
            }
        }

        public static Capabilities GetDeviceCaps(int adapterOrdinal, DeviceType deviceType)
        {
            return Direct3D.GetDeviceCaps(adapterOrdinal, deviceType);
        }
    }

    /// <summary>
    /// Device 构造兼容层：MDX 接受 Control，SharpDX 接受窗口句柄 + Direct3D 实例。
    /// </summary>
    public static class DeviceFactory
    {
        public static Device Create(int adapter, DeviceType deviceType, IntPtr windowHandle,
            CreateFlags createFlags, PresentParameters presentParameters)
        {
            return new Device(Manager.Direct3D, adapter, deviceType, windowHandle, createFlags,
                new[] { presentParameters });
        }

        public static Device Create(int adapter, DeviceType deviceType, System.Windows.Forms.Control control,
            CreateFlags createFlags, PresentParameters presentParameters)
        {
            return Create(adapter, deviceType, control.Handle, createFlags, presentParameters);
        }
    }
}
