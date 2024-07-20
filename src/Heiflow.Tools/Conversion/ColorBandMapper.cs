using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data.Classification
{
    public class ColorBandMapper
    {
        private Bitmap colorBandImage;
        public static string GetColorBandFileFolder()
        {
            return Path.Combine(Applications.VHFAppManager.Instance.ApplicationPath, "colors");
        }
        public static string[] GetColorBandFile()
        {
            string dir = GetColorBandFileFolder();
            DirectoryInfo di = new DirectoryInfo(dir);
            FileInfo[] imgFiles = di.GetFiles("*.png");
            List<string> files = new List<string>();
            for (int i = 0; i < imgFiles.Length; i++)
            {
                files.Add(imgFiles[i].Name);
            }
            return files.ToArray();
        }

        public int ImageHeight
        {
            get
            {
                return colorBandImage.Height;
            }
        }

        public ColorBandMapper(string filePath)
        {
            colorBandImage = new Bitmap(filePath);
        }

        public List<Color> GetCatPalette(int count)
        {
            List<Color> list = new List<Color>();
            for (int i = 0; i < count; i++)
            {
                var color = MapValueToColor(i, 0, count);
                list.Add(color);
            }
            return list;
        }

        public List<Color> GetFullPalette()
        {
            List<Color> list = new List<Color>();
            for (int i = 0; i < colorBandImage.Height; i++)
            {
                Color color = colorBandImage.GetPixel(0, i);
                list.Add(color);
            }
            return list;
        }

        public Color MapValueToColor(double value, double minValue, double maxValue)
        {
            int pixelY = (int)((value - minValue) / (maxValue - minValue) * (colorBandImage.Height - 1));
            Color color = colorBandImage.GetPixel(0, pixelY);
            return color;
        }

        // 释放图像资源  
        public void Dispose()
        {
            colorBandImage.Dispose();
        }
    }
}
