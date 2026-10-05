using Heiflow.Core.Data;
using Heiflow.Core.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Heiflow.Visualization.Renderable.Grid
{
    public class DataColor
    {
        public  Ramp mColorRamp;
        public static int TransparentColor = Color.Transparent.ToArgb();

        public DataColor()
        {
            mColorRamp = new Ramp(22);
            var count = 5;
            var localCR = new Ramp(22);
            int originLen = localCR.Colors.Length;
            int deltaL = (int)Math.Floor((double)originLen / count);
            System.Drawing.Color[] colors = new System.Drawing.Color[count];
            colors[0] = localCR.Colors[0];
            for (int i = 1; i < count - 1; i++)
            {
                colors[i] = localCR.Colors[i * deltaL];
            }
            colors[count - 1] = localCR.Colors[originLen - 1];
            this.ColorRamp.Colors = colors;
        }

        public Ramp ColorRamp
        {
            get
            {
                return mColorRamp;
            }
            set
            {
                mColorRamp = value;
            }
        }

        public static Color[] GetUniqueRandomColor(int count)
        {
            Color[] colors = new Color[count];
            HashSet<Color> hs = new HashSet<Color>();

            Random randomColor = new Random();

            for (int i = 0; i < count; i++)
            {
                Color color;
                while (!hs.Add(color = Color.FromArgb(randomColor.Next(70, 200), randomColor.Next(100, 225), randomColor.Next(100, 230)))) ;
                colors[i] = color;
            }

            return colors;
        }

        public static Dictionary<T, Color> ProduceColorDic<T>(T[] keys)
        {
            Dictionary<T, Color> dic = new Dictionary<T, Color>();
            var colors = GetUniqueRandomColor(keys.Length);
            for (int i = 0; i < keys.Length; i++)
            {
                dic.Add(keys[i], colors[i]);
            }
            return dic;
        }

        /// <summary>
        /// Result of the last classification together with the signature of the values it was made for.
        /// </summary>
        private int[] _CachedLevels;
        private int _CachedNumBreaks;
        private int _CachedColorCount;
        private float _CachedMin;
        private float _CachedMax;
        private double _CachedSum;
        private ClassificationMethod _CachedMethod = ClassificationMethod.Natural_Breaks_Jenks;

        public  int[] GetLevels(float[] array, int numbreaks, ClassificationMethod clasmethod)
        {
            float min;
            float max;
            double sum;
            MinMaxSum(array, out min, out max, out sum);

            // The classification costs far more than the loop that colours the vertices afterwards, and
            // the render asks for the levels again whenever only the ramp, the opacity or the number of
            // classes changed. The values are the same in that case, so the levels are reused.
            if (_CachedLevels != null && _CachedLevels.Length == array.Length && _CachedNumBreaks == numbreaks
                && _CachedColorCount == mColorRamp.Colors.Length && _CachedMethod == clasmethod
                && _CachedMin == min && _CachedMax == max && _CachedSum == sum)
            {
                return _CachedLevels;
            }

            var levels = new int[array.Length];
            if (clasmethod == ClassificationMethod.Natural_Breaks_Jenks)
            {
                var breaks = JenksFisherFloat.CreateJenksFisherBreaksArray(array, numbreaks);
                for (int i = 0; i < array.Length; i++)
                {
                    levels[i] = FindLevel(array[i], breaks);
                }
            }
            else if (clasmethod == ClassificationMethod.Equal_Inteval)
            {
                if (min == max)
                    max = min + 1;
                var colorlen = mColorRamp.Colors.Length;
                for (int i = 0; i < array.Length; i++)
                {
                    levels[i] = (int)((array[i] - min) / (max - min) * colorlen - 1);
                    if (levels[i] < 0)
                        levels[i] = 0;
                    else if (levels[i] >= colorlen)
                        levels[i] = colorlen - 1;
                }
            }

            _CachedLevels = levels;
            _CachedNumBreaks = numbreaks;
            _CachedColorCount = mColorRamp.Colors.Length;
            _CachedMin = min;
            _CachedMax = max;
            _CachedSum = sum;
            _CachedMethod = clasmethod;

            return levels;
        }

        /// <summary>
        /// Smallest value, largest value and sum of an array in one pass.
        /// </summary>
        private static void MinMaxSum(float[] array, out float min, out float max, out double sum)
        {
            float localMin = float.MaxValue;
            float localMax = float.MinValue;
            double localSum = 0.0;
            for (int i = 0; i < array.Length; i++)
            {
                float value = array[i];
                if (value < localMin)
                    localMin = value;
                if (value > localMax)
                    localMax = value;
                localSum += value;
            }
            min = localMin;
            max = localMax;
            sum = localSum;
        }

        /// <summary>
        /// Index of the interval [breaks[j], breaks[j+1]) the value falls into, zero when there is none.
        /// The breaks come out of the classification in ascending order, so the interval is found by
        /// bisection instead of walking the whole list for every value.
        /// </summary>
        private static int FindLevel(float value, List<float> breaks)
        {
            int low = 0;
            int high = breaks.Count - 2;
            while (low <= high)
            {
                int mid = (low + high) / 2;
                if (value >= breaks[mid + 1])
                    low = mid + 1;
                else if (value < breaks[mid])
                    high = mid - 1;
                else
                    return mid;
            }
            return 0;
        }

        public int GetVertexColor(double max, double min,  double averagedValue,int alpha)
        {
                double level = (averagedValue - min) / (max - min) * mColorRamp.Colors.Length - 1;
                //System.Drawing.Color color =Colours.ColourRamps.ColorRamp.GetColour(averaged, min, max);
                System.Drawing.Color color = System.Drawing.Color.White;
                if (level < 0)
                {
                    color = mColorRamp.Colors[0];
                }
                else
                {
                    if (level < mColorRamp.Colors.Length)
                        color = mColorRamp.Colors[(int)level];
                    else
                    {
                        color = mColorRamp.Colors[mColorRamp.Colors.Length - 1];
                    }
                }

                return System.Drawing.Color.FromArgb(alpha, color).ToArgb();
        }

        public int GetVertexColor(int level, int alpha, bool invert=false)
        {
            System.Drawing.Color color = System.Drawing.Color.White;
            if (level < 0)
            {
                color = mColorRamp.Colors[0];
            }
            else
            {
                if (invert)
                {
                    if (level < mColorRamp.Colors.Length)
                        color = mColorRamp.Colors[mColorRamp.Colors.Length - level - 1];
                    else
                    {
                        color = mColorRamp.Colors[0];
                    }
                }
                else
                {
                    if (level < mColorRamp.Colors.Length)
                        color = mColorRamp.Colors[level];
                    else
                    {
                        color = mColorRamp.Colors[mColorRamp.Colors.Length - 1];
                    }
                }
            }

            return System.Drawing.Color.FromArgb(alpha, color).ToArgb();
        }

        public DataCube<int> SetVertexColor(float[][][] data, int opaticy, int varIndex)
        {
            int n2 = data[0].Length;
            int n3 = data[0][0].Length;

            DataCube<int> colors = new DataCube<int>(1,n2, n3);

            for (int j = 0; j < n2; j++)
            {
                var vec = data[varIndex][j];
                float max = vec.Max();
                float min = vec.Min();
                for (int k = 0; k < n3; k++)
                {
                    colors[0,j,k] = GetVertexColor(max, min, data[varIndex][j][k], opaticy);
                }
            }

            return colors;
        }

        public int[][] SetVertexColor(float[][] data, int opaticy)
        {
            int n1 = data.Length;
            int n2 = data[0].Length;
            int[][] colors = new int[n1][];

            for (int j = 0; j < n1; j++)
            {
                colors[j] = new int[n2];
                var vec = data[j];
                float max = vec.Max();
                float min = vec.Min();
                for (int k = 0; k < n2; k++)
                {
                    colors[j][k] = GetVertexColor(max, min, data[j][k], opaticy);
                }
            }
            return colors;
        }

        public static int GetRandomColor(int seed)
        {
            Random r = new Random(seed);
            System.Drawing.Color color = System.Drawing.Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));
            return color.ToArgb();
        }

        public static System.Drawing.Color GetRandomDrawingColor(int seed)
        {
            Random r = new Random(seed);
            System.Drawing.Color color = System.Drawing.Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));
            return color;
        }

        public static int GetColor(double percent)
        {
            double min = 0;
            double max = 1;
            double red = 1.0;
            double green = 1.0;
            double blue = 1.0;
            //TODO: make this a function and abstract to allow multiple gradient mappings

            double dv;
            if (percent < min)
                percent = min;
            if (percent > max)
                percent = max;
            dv = max - min;
            if (percent < (min + 0.25 * dv))
            {
                red = 0;
                green = 4 * (percent - min) / dv;
            }
            else if (percent < (min + 0.5 * dv))
            {
                red = 0;
                blue = 1 + 4 * (min + 0.25 * dv - percent) / dv;
            }
            else if (percent < (min + 0.75 * dv))
            {
                red = 4 * (percent - min - 0.5 * dv) / dv;
                blue = 0;
            }
            else
            {
                green = 1 + 4 * (min + 0.75 * dv - percent) / dv;
                blue = 0;
            }
            return System.Drawing.Color.FromArgb((int)(255 * red), (int)(255 * green), (int)(255 * blue)).ToArgb();
        }
    }
}
