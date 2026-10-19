using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using Rectangle = System.Drawing.Rectangle;

namespace Heiflow.Visualization.DirectX
{
    /// <summary>
    /// MDX 支持 new Texture(device, Bitmap, Usage, Pool)，SharpDX 只能按尺寸/内存创建。
    /// 这里补上从 GDI+ 位图创建纹理的入口（内部走 D3DX 的内存加载），语义与 MDX 构造一致。
    /// </summary>
    public static class TextureFactory
    {
        public static Texture FromBitmap(Device device, Bitmap bitmap, Usage usage, Pool pool)
        {
            return FromBitmap(device, bitmap, usage, pool, 0);
        }

        public static Texture FromBitmap(Device device, Bitmap bitmap, Usage usage, Pool pool, int mipLevels)
        {
            byte[] png;
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                png = ms.ToArray();
            }
            return Texture.FromMemory(device, png, bitmap.Width, bitmap.Height, mipLevels, usage,
                Format.Unknown, pool, Filter.Point, Filter.Point, 0);
        }

        public static Texture FromBitmap(Device device, Image image, Usage usage, Pool pool)
        {
            using (var bmp = new Bitmap(image))
                return FromBitmap(device, bmp, usage, pool);
        }
    }

    /// <summary>纹理/表面的 Lock 重载：MDX 允许传 System.Drawing.Rectangle，SharpDX 只接受 RawRectangle。</summary>
    public static class TextureExtensions
    {
        public static DataStream LockRectangle(this Texture texture, int level, Rectangle rect, LockFlags flags)
        {
            DataStream stream;
            texture.LockRectangle(level, ToRaw(rect), flags, out stream);
            return stream;
        }

        public static DataStream LockRectangle(this Texture texture, int level, Rectangle rect, LockFlags flags, out DataStream stream)
        {
            texture.LockRectangle(level, ToRaw(rect), flags, out stream);
            return stream;
        }

        public static DataRectangle LockRectangle(this Texture texture, int level, LockFlags flags, out DataStream stream)
        {
            return texture.LockRectangle(level, flags, out stream);
        }

        static RawRectangle ToRaw(Rectangle r)
        {
            return new RawRectangle(r.Left, r.Top, r.Right, r.Bottom);
        }
    }
}
