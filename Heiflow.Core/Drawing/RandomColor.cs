// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Drawing
{
    public  class RandomColor
    {
        public RandomColor()
        {
        }
        public static System.Drawing.Color DisdillColorFromString(string strcolor)
        {
            string colorStr = strcolor;
            string strSplit = ",";
            char[] chSplit = strSplit.ToCharArray();
            string[] rgbStr = colorStr.Split(chSplit);
            System.Drawing.Color featureColor = System.Drawing.Color.FromArgb(Convert.ToInt32(rgbStr[0]), Convert.ToInt32(rgbStr[1]), Convert.ToInt32(rgbStr[2]));
            return featureColor;
        }
        public static System.Drawing.Color Next()
        {
            Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
            //  对于C#的随机数，没什么好说的
            System.Threading.Thread.Sleep(RandomNum_First.Next(50));
            Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);

            int int_Red = RandomNum_First.Next(256);
            int int_Green = RandomNum_Sencond.Next(256);
            int int_Blue = (int_Red + int_Green > 400) ? 0 : 400 - int_Red - int_Green;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;

            return System.Drawing.Color.FromArgb(int_Red, int_Green, int_Blue);
        }

        public static System.Drawing.Color[] CreateColorSeries(int count)
        {
            System.Drawing.Color[] result = new System.Drawing.Color[count];
            for (int i = 0; i < count; i++)
            {
                Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
                //  对于C#的随机数，没什么好说的
                System.Threading.Thread.Sleep(RandomNum_First.Next(50));
                Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);

                //  为了在白色背景上显示，尽量生成深色
                int int_Red = RandomNum_First.Next(256);
                int int_Green = RandomNum_Sencond.Next(256);
                int int_Blue = (int_Red + int_Green > 400) ? 0 : 400 - int_Red - int_Green;
                int_Blue = (int_Blue > 255) ? 255 : int_Blue;

                result[i] = System.Drawing.Color.FromArgb(int_Red, int_Green, int_Blue);
            }
            return result;
        }
    }
}
