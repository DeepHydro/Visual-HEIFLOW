// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace  Heiflow
{
    public class ColorMap
    {
        private Color mLow;
        private Color mHigh;
        private int mColorStep;
        private double[] mRgbStep;
        public ColorMap(Color low, Color high)
        {
            mLow = low;
            mHigh = high;
            int[] step = new int[3];
            step[0] = Math.Abs(high.R - low.R);
            step[1] = Math.Abs(high.G - low.G);
            step[2] = Math.Abs(high.B - low.B);
            mColorStep = step[0];

            foreach (int temp in step)
            {
                if (mColorStep <= temp)
                    mColorStep = temp;
            }
            //if (mColorStep <= 0)
            //    throw new Exception("颜色上下限输入有误!");

            mRgbStep = new double[3];
            mRgbStep[0] = ComputeRgbStep(mLow.R, mHigh.R);
            mRgbStep[1] = ComputeRgbStep(mLow.G, mHigh.G);
            mRgbStep[2] = ComputeRgbStep(mLow.B, mHigh.B);


        }

        public Color[] GenerateUniqueColors(int count)
        {
            double[] data = new double[count];
            for (int i = 0; i < count; i++)
            {
                data[i] = i + 1;
            }        
            return MapDataToColor(data);
        }

        public Color[] MapDataToColor(double[] data)
        {
            Color[] result = null;
            if (data != null)
            {
                result = new Color[data.Length];
                double min = data[0];
                double max = data[0];
                foreach (double d in data)
                {
                    if (d >= max)
                    {
                        max = d;
                    }
                    if (d <= min)
                    {
                        min = d;
                    }
                }
                double step = (max - min) / mColorStep;
                int i = 0;
                foreach (double d in data)
                {
                    int level = (int)Math.Round((d - min) / step);
                    result[i] = MapDataToColor(level);
                    i++;
                }

            }
            return result;
        }

        public Color[,] MapDataToColor(float[,] data)
        {
            Color[,] result = null;
            if (data != null)
            {
                int column = (int)data.Length / (int)data.GetLongLength(0);
                int row = (int)data.GetLongLength(0);
                result = new Color[row, column];

                double min = data[0, 0];
                double max = data[0, 0];
                foreach (double d in data)
                {

                    if (d >= max)
                    {
                        max = d;
                    }
                    if (d <= min)
                    {
                        min = d;
                    }
                }
                double step = (max - min) / mColorStep;
                for (int r = 0; r < row; r++)
                {
                    for (int c = 0; c < column; c++)
                    {
                        int level = (int)Math.Round((data[r, c] - min) / step);
                        if (data[r, c] == 0)
                        {
                            result[r, c] =  Color.FromArgb(System.Drawing.Color.Transparent.R,System.Drawing.Color.Transparent.G,System.Drawing.Color.Transparent.B);
                        }
                        else
                            result[r, c] = MapDataToColor(level);
                    }
                }
            }
            return result;
        }

        public Color MapDataToColor(double max, double min, double data)
        {
            Color result = new Color();
            double step = (max - min) / mColorStep;


            int level = (int)Math.Round((data - min) / step);
            result = MapDataToColor(level);
            return result;
        }

        public Color MapDataToColor(int levle)
        {
            Color result =  Color.FromArgb(ComputeRGB(levle, 0),ComputeRGB(levle, 1),ComputeRGB(levle, 2));
            return result;
        }

        private double ComputeRgbStep(double low, double high)
        {
            double currentStep = (high - low) / mColorStep;
            return currentStep;
        }
        private int ComputeRGB(int level, int rgbIndex)
        {
            int result;
            if (rgbIndex == 0)
            {
                result = (int)Math.Round((double)(mLow.R + level * mRgbStep[0]));
            }
            else if (rgbIndex == 1)
            {
                result = (int)Math.Round((double)(mLow.G + level * mRgbStep[1]));
            }
            else
            {
                result = (int)Math.Round((double)(mLow.B + level * mRgbStep[2]));
            }

            return result;
        }

    }
}
