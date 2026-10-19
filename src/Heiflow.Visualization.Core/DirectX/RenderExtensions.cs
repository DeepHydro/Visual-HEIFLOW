using System;
using System.Collections.Generic;
using System.Drawing;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;
using Font = SharpDX.Direct3D9.Font;

namespace Heiflow.Visualization.DirectX
{
    /// <summary>
    /// 顶点/索引缓冲与 D3DX 字体、精灵的 MDX 风格封装。
    /// 这些扩展方法之所以能生效，是因为 SharpDX 的实例方法签名不接受
    /// System.Drawing 类型或数组参数，重载解析会退回到这里的扩展方法。
    /// </summary>
    public static class RenderExtensions
    {
        /// <summary>MDX: vertexBuffer.SetData(array, offset, LockFlags)</summary>
        public static void SetData<T>(this VertexBuffer buffer, T[] data, int offsetToLock, LockFlags flags)
            where T : struct
        {
            if (data == null || data.Length == 0) return;
            using (var stream = buffer.Lock(offsetToLock, 0, flags))
                stream.WriteRange(data);
            buffer.Unlock();
        }

        /// <summary>MDX: indexBuffer.SetData(array, offset, LockFlags)</summary>
        public static void SetData<T>(this IndexBuffer buffer, T[] data, int offsetToLock, LockFlags flags)
            where T : struct
        {
            if (data == null || data.Length == 0) return;
            using (var stream = buffer.Lock(offsetToLock, 0, flags))
                stream.WriteRange(data);
            buffer.Unlock();
        }

        /// <summary>MDX: font.MeasureString(sprite, text, format, 0)</summary>
        public static Rectangle MeasureString(this Font font, Sprite sprite, string text, FontDrawFlags format, int param)
        {
            return MeasureStringCore(font, sprite, text, format);
        }

        public static Rectangle MeasureString(this Font font, Sprite sprite, string text, FontDrawFlags format)
        {
            return MeasureStringCore(font, sprite, text, format);
        }

        /// <summary>MDX: font.MeasureString(sprite, text, format, Color)</summary>
        public static Rectangle MeasureString(this Font font, Sprite sprite, string text, FontDrawFlags format, Color color)
        {
            return MeasureStringCore(font, sprite, text, format);
        }

        /// <summary>MDX: font.MeasureString(sprite, text, int format, Color)</summary>
        public static Rectangle MeasureString(this Font font, Sprite sprite, string text, int format, Color color)
        {
            return MeasureStringCore(font, sprite, text, (FontDrawFlags)format);
        }

        // 与 DrawText 同理：SharpDX 的 MeasureText 对 null / 空串不做防护，这里按空矩形返回。
        static Rectangle MeasureStringCore(Font font, Sprite sprite, string text, FontDrawFlags format)
        {
            if (string.IsNullOrEmpty(text))
                return Rectangle.Empty;

            return ToRectangle(font.MeasureText(sprite, text, format));
        }

        // SharpDX 4.2 的 Font.DrawText 对文本内容不做防护，实测：
        //   text == null  -> native 层抛 NullReferenceException
        //   text == ""    -> D3DX 返回错误码，抛 SharpDXException("Draw failed")
        // MDX 时代这两种都会被静默接受（等于不画），这里还原成跳过绘制。
        static int DrawTextCore(Font font, Sprite sprite, string text, RawRectangle rect, FontDrawFlags format, RawColorBGRA color)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return font.DrawText(sprite, text, rect, format, color);
        }

        static int DrawTextCore(Font font, Sprite sprite, string text, int x, int y, RawColorBGRA color)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            return font.DrawText(sprite, text, x, y, color);
        }

        /// <summary>MDX: font.DrawText(sprite, text, x, y, Color)</summary>
        public static int DrawText(this Font font, Sprite sprite, string text, int x, int y, Color color)
        {
            return DrawTextCore(font, sprite, text, x, y, ColorUtil.ToRawColorBGRA(color));
        }

        /// <summary>MDX: font.DrawText(sprite, text, Rectangle, format, Color)</summary>
        public static int DrawText(this Font font, Sprite sprite, string text, Rectangle rect, FontDrawFlags format, Color color)
        {
            return DrawTextCore(font, sprite, text, ToRaw(rect), format, ColorUtil.ToRawColorBGRA(color));
        }

        /// <summary>MDX: font.DrawText(sprite, text, Rectangle, format, int argb)</summary>
        public static int DrawText(this Font font, Sprite sprite, string text, Rectangle rect, FontDrawFlags format, int argb)
        {
            return DrawTextCore(font, sprite, text, ToRaw(rect), format, ColorUtil.ToRawColorBGRA(argb));
        }

        /// <summary>MDX: 允许把 DrawTextFormat 写成 int 的调用点。</summary>
        public static int DrawText(this Font font, Sprite sprite, string text, Rectangle rect, int format, Color color)
        {
            return DrawTextCore(font, sprite, text, ToRaw(rect), (FontDrawFlags)format, ColorUtil.ToRawColorBGRA(color));
        }

        public static int DrawText(this Font font, Sprite sprite, string text, Rectangle rect, int format, int argb)
        {
            return DrawTextCore(font, sprite, text, ToRaw(rect), (FontDrawFlags)format, ColorUtil.ToRawColorBGRA(argb));
        }

        public static int DrawText(this Font font, Sprite sprite, string text, int x, int y, int argb)
        {
            return DrawTextCore(font, sprite, text, x, y, ColorUtil.ToRawColorBGRA(argb));
        }

        /// <summary>MDX: sprite.Draw(texture, srcRect, center, position, Color)</summary>
        public static void Draw(this Sprite sprite, Texture texture, Rectangle srcRect, Vector3 center, Vector3 position, Color color)
        {
            sprite.Draw(texture, ColorUtil.ToRawColorBGRA(color), ToRaw(srcRect), ToRaw(center), ToRaw(position));
        }

        public static void Draw(this Sprite sprite, Texture texture, Vector3 center, Vector3 position, Color color)
        {
            sprite.Draw(texture, ColorUtil.ToRawColorBGRA(color), null, ToRaw(center), ToRaw(position));
        }

        /// <summary>MDX: sprite.Draw(texture, center, position, int argb)</summary>
        public static void Draw(this Sprite sprite, Texture texture, Vector3 center, Vector3 position, int argb)
        {
            sprite.Draw(texture, ColorUtil.ToRawColorBGRA(argb), null, ToRaw(center), ToRaw(position));
        }

        /// <summary>MDX: sprite.Draw(texture, srcRect, center, position, int argb)</summary>
        public static void Draw(this Sprite sprite, Texture texture, Rectangle srcRect, Vector3 center, Vector3 position, int argb)
        {
            sprite.Draw(texture, ColorUtil.ToRawColorBGRA(argb), ToRaw(srcRect), ToRaw(center), ToRaw(position));
        }

        /// <summary>MDX: line.Draw(Vector2[], Color)</summary>
        public static void Draw(this Line line, Vector2[] points, Color color)
        {
            line.Draw(points, ColorUtil.ToRawColorBGRA(color));
        }

        /// <summary>MDX: line.Draw(points, int argb)</summary>
        public static void Draw(this Line line, Vector2[] points, int argb)
        {
            line.Draw(points, ColorUtil.ToRawColorBGRA(argb));
        }

        public static RawRectangle ToRaw(Rectangle r)
        {
            return new RawRectangle(r.Left, r.Top, r.Right, r.Bottom);
        }

        public static Rectangle ToRectangle(RawRectangle r)
        {
            return new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
        }

        static RawVector3 ToRaw(Vector3 v)
        {
            return new RawVector3(v.X, v.Y, v.Z);
        }
    }

    /// <summary>
    /// MDX 的 device.Lights[index] 兼容层。
    /// SharpDX 没有 Lights 集合，这里用带缓存的槽位对象模拟：
    /// 每次赋值立即提交到设备，语义与 MDX 的 Lights 包装器一致。
    /// </summary>
    public sealed class LightSlot
    {
        readonly Device _device;
        readonly int _index;
        Light _light;
        bool _enabled;

        internal LightSlot(Device device, int index)
        {
            _device = device;
            _index = index;
            try { _light = device.GetLight(index); }
            catch { _light = new Light(); }
        }

        void Commit() { _device.SetLight(_index, ref _light); }

        public LightType Type { get { return _light.Type; } set { _light.Type = value; Commit(); } }

        public Color4 Diffuse { get { return _light.Diffuse; } set { _light.Diffuse = value; Commit(); } }
        public Color4 Specular { get { return _light.Specular; } set { _light.Specular = value; Commit(); } }
        public Color4 Ambient { get { return _light.Ambient; } set { _light.Ambient = value; Commit(); } }

        public Vector3 Position { get { return _light.Position; } set { _light.Position = value; Commit(); } }
        public Vector3 Direction { get { return _light.Direction; } set { _light.Direction = value; Commit(); } }

        public float Range { get { return _light.Range; } set { _light.Range = value; Commit(); } }
        public float Falloff { get { return _light.Falloff; } set { _light.Falloff = value; Commit(); } }
        public float Attenuation0 { get { return _light.Attenuation0; } set { _light.Attenuation0 = value; Commit(); } }
        public float Attenuation1 { get { return _light.Attenuation1; } set { _light.Attenuation1 = value; Commit(); } }
        public float Attenuation2 { get { return _light.Attenuation2; } set { _light.Attenuation2 = value; Commit(); } }
        public float Theta { get { return _light.Theta; } set { _light.Theta = value; Commit(); } }
        public float Phi { get { return _light.Phi; } set { _light.Phi = value; Commit(); } }

        /// <summary>对应 D3D 的 LightEnable，不属于 D3DLIGHT9 结构本身。</summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                _device.SetLight(_index, ref _light);
                _device.EnableLight(_index, value);
            }
        }

        /// <summary>MDX 没有 Update()，保留以兼容可能的调用。</summary>
        public void Update() { Commit(); }

        /// <summary>MDX 没有 Commit()，保留以兼容可能的调用。</summary>
        public void CommitChanges() { Commit(); }
    }

    public static class LightHub
    {
        public static LightSlot Get(Device device, int index)
        {
            return new LightSlot(device, index);
        }
    }
}
