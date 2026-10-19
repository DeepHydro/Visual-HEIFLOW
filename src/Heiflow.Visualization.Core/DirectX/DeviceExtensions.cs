using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;
using Color = System.Drawing.Color;

namespace Heiflow.Visualization.DirectX
{
    /// <summary>
    /// Device 兼容扩展。
    /// SharpDX 是纯 P/Invoke 封装（MSIL，支持 Any CPU），因此不存在扩展属性，
    /// 这里补足 MDX 中常用、但 SharpDX 需要不同类型参数的几个方法。
    /// </summary>
    public static class DeviceExtensions
    {
        /// <summary>MDX: device.Clear(flags, System.Drawing.Color, z, stencil)</summary>
        public static void Clear(this Device device, ClearFlags flags, Color color, float z, int stencil)
        {
            device.Clear(flags, ColorUtil.ToRawColorBGRA(color), z, stencil);
        }

        /// <summary>MDX: device.Clear(flags, System.Drawing.Color, z, stencil, rects)</summary>
        public static void Clear(this Device device, ClearFlags flags, Color color, float z, int stencil, RawRectangle[] rects)
        {
            device.Clear(flags, ColorUtil.ToRawColorBGRA(color), z, stencil, rects);
        }

        /// <summary>MDX: device.RenderState.X （读取，枚举型）</summary>
        public static T GetRenderStateValue<T>(this Device device, RenderState state) where T : struct
        {
            int raw = device.GetRenderState(state);
            return (T)Enum.ToObject(typeof(T), raw);
        }

        /// <summary>MDX: device.Reset(pp) —— SharpDX 需要数组</summary>
        public static void Reset(this Device device, PresentParameters presentParameters)
        {
            device.Reset(new[] { presentParameters });
        }

        /// <summary>MDX: device.Clear(flags, int argb, z, stencil)</summary>
        public static void Clear(this Device device, ClearFlags flags, int argb, float z, int stencil)
        {
            device.Clear(flags, ColorUtil.ToRawColorBGRA(argb), z, stencil);
        }

        /// <summary>MDX: device.GetBackBuffer(0, 0, BackBufferType.Mono)</summary>
        public static Surface GetBackBuffer(this Device device, int swapChain, int backBufferIndex)
        {
            return device.GetBackBuffer(swapChain, backBufferIndex);
        }

        /// <summary>MDX: bool b = device.RenderState.X（读取后转成 bool）</summary>
        public static bool GetRenderStateBool(this Device device, RenderState state)
        {
            return device.GetRenderState(state) != 0;
        }

        /// <summary>MDX: Enum e = device.RenderState.X（读取后转成枚举）</summary>
        public static T GetRenderStateEnum<T>(this Device device, RenderState state) where T : struct
        {
            return (T)Enum.ToObject(typeof(T), device.GetRenderState(state));
        }

        /// <summary>MDX: Color c = device.RenderState.X（读取后转成 D3DCOLOR）</summary>
        public static Color GetRenderStateColor(this Device device, RenderState state)
        {
            return Color.FromArgb(device.GetRenderState(state));
        }

        /// <summary>MDX: device.DrawUserPrimitives(PrimitiveType, int, Array)</summary>
        public static void DrawUserPrimitives<T>(this Device device, PrimitiveType primitiveType, int primitiveCount, T[] data)
            where T : struct
        {
            device.DrawUserPrimitives(primitiveType, primitiveCount, data);
        }
    }

    /// <summary>
    /// 颜色与 D3DCOLOR 之间的转换。
    /// D3DCOLOR 布局为 0xAARRGGBB，与 System.Drawing.Color.ToArgb() 一致。
    /// </summary>
    public static class ColorUtil
    {
        public static int ToArgb(Color color)
        {
            return color.ToArgb();
        }

        public static int ToArgb(Color4 color)
        {
            return ToArgb(ToColor(color));
        }

        public static Color ToColor(Color4 c)
        {
            int a = Clamp(c.Alpha), r = Clamp(c.Red), g = Clamp(c.Green), b = Clamp(c.Blue);
            return Color.FromArgb(a, r, g, b);
        }

        public static Color4 ToColor4(Color c)
        {
            return new Color4(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f);
        }

        public static RawColorBGRA ToRawColorBGRA(Color c)
        {
            return new RawColorBGRA { B = c.B, G = c.G, R = c.R, A = c.A };
        }

        public static RawColorBGRA ToRawColorBGRA(int argb)
        {
            return ToRawColorBGRA(Color.FromArgb(argb));
        }

        static int Clamp(float v)
        {
            int i = (int)(v * 255f);
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
    }

    /// <summary>
    /// 设备丢失/重置通知中心。
    /// SharpDX 的 Device 不提供 DeviceLost / DeviceReset / DeviceResizing 事件，
    /// 这里用一个以 Device 为键的静态注册表来填补，行为与 MDX 保持一致：
    /// 由宿主窗口在真正执行 Reset 之后调用 RaiseDeviceReset。
    /// </summary>
    public static class DeviceEvents
    {
        static readonly Dictionary<Device, List<EventHandler>> ResetHandlers =
            new Dictionary<Device, List<EventHandler>>();

        static readonly object Sync = new object();

        public static void AddDeviceResetHandler(Device device, EventHandler handler)
        {
            if (device == null || handler == null) return;
            lock (Sync)
            {
                List<EventHandler> list;
                if (!ResetHandlers.TryGetValue(device, out list))
                {
                    list = new List<EventHandler>();
                    ResetHandlers[device] = list;
                }
                if (!list.Contains(handler))
                    list.Add(handler);
            }
        }

        public static void RemoveDeviceResetHandler(Device device, EventHandler handler)
        {
            if (device == null || handler == null) return;
            lock (Sync)
            {
                List<EventHandler> list;
                if (ResetHandlers.TryGetValue(device, out list))
                    list.Remove(handler);
            }
        }

        /// <summary>由宿主在 device.Reset(...) 成功后调用。</summary>
        public static void RaiseDeviceReset(Device device)
        {
            if (device == null) return;
            List<EventHandler> snapshot;
            lock (Sync)
            {
                List<EventHandler> list;
                if (!ResetHandlers.TryGetValue(device, out list)) return;
                snapshot = new List<EventHandler>(list);
            }
            foreach (var h in snapshot)
            {
                try { h(device, EventArgs.Empty); }
                catch { /* 单个订阅者的失败不应中断其它订阅者 */ }
            }
        }

        public static void UnregisterDevice(Device device)
        {
            if (device == null) return;
            lock (Sync) { ResetHandlers.Remove(device); }
        }
    }
}
