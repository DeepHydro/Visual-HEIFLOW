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

        public static string[] GetColorBandFile()
        {
            string dir = Path.Combine( Applications.VHFAppManager.Instance.ApplicationPath, "colors");
            DirectoryInfo di = new DirectoryInfo(dir);
            FileInfo[] imgFiles = di.GetFiles("*.png");
            List<string> files = new List<string>();
            for (int i = 0; i < imgFiles.Length; i++)	
            {
                files.Add( imgFiles[i].FullName);
            }
            return files.ToArray();
        }

        public ColorBandMapper(string filePath)
        {
            colorBandImage = new Bitmap(filePath);
        }

        public List<Color> GetPalette(int count)
        {
            List<Color> list = new List<Color>();
            for (int i = 0; i < count; i++)
            {
                var color = MapValueToColor(i, 0, count);
                list.Add(color);
            }
            return list;
        }

        public Color MapValueToColor(double value, double minValue, double maxValue)
        {
            // 计算色带中对应的像素位置  
            // 假设色带宽度为bandWidth（在这个案例中，由于色带是PNG图像，bandWidth应该是图像的宽度）  
            // 但这里我们直接根据图像宽度计算  
            int pixelY = (int)((value - minValue) /(maxValue - minValue) * (colorBandImage.Height - 1));

            // 由于PNG图像的高度可能远大于需要，我们只取顶部的一行  
            int pixelX = 0;

            // 获取该位置的颜色  
            Color color = colorBandImage.GetPixel(pixelX, pixelY);

            return color;
        }

        // 释放图像资源  
        public void Dispose()
        {
            colorBandImage.Dispose();
        }
    }
}
