using System;
using SharpDX.Direct3D9;
using Font = SharpDX.Direct3D9.Font;

namespace Heiflow.Visualization.DirectX
{
    /// <summary>
    /// MDX 支持 new Font(device, System.Drawing.Font)，SharpDX 只接受 FontDescription。
    /// 这里补上从 GDI+ 字体构造 D3DX 字体的入口。
    /// </summary>
    public static class FontFactory
    {
        public static Font Create(Device device, System.Drawing.Font font)
        {
            var desc = new FontDescription
            {
                FaceName = font.Name,
                Height = (int)Math.Ceiling(font.GetHeight()),
                Width = 0,
                Weight = font.Bold ? FontWeight.Bold : FontWeight.Normal,
                MipLevels = 0,
                Italic = font.Italic,
                CharacterSet = FontCharacterSet.Default,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default,
                PitchAndFamily = FontPitchAndFamily.Default
            };
            return new Font(device, desc);
        }

        public static Font Create(Device device, System.Drawing.Font font, FontWeight weight)
        {
            var desc = new FontDescription
            {
                FaceName = font.Name,
                Height = (int)Math.Ceiling(font.GetHeight()),
                Width = 0,
                Weight = weight,
                MipLevels = 0,
                Italic = font.Italic,
                CharacterSet = FontCharacterSet.Default,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default,
                PitchAndFamily = FontPitchAndFamily.Default
            };
            return new Font(device, desc);
        }
    }
}
