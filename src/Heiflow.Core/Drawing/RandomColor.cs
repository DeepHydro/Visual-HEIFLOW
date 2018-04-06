//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

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
