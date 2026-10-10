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

        /// <summary>
        /// The class count a render starts out with, before anything has been picked in the panel.
        /// </summary>
        public const int DefaultClassCount = 5;

        public DataColor()
        {
            mColorRamp = new Ramp(22);
            // taken out of the ramp the same way the panel takes them once a count is picked, so a
            // layer that has not been touched is drawn with the classes it shows
            mColorRamp.Colors = Resample(mColorRamp.Colors, DefaultClassCount);
        }

        /// <summary>
        /// Reduces a ramp to count colours. The colours are interpolated between their neighbours
        /// rather than picked by index, picking by index repeats colours and leaves the last class
        /// far away from the one before it whenever the length is not a multiple of the count.
        /// </summary>
        public static System.Drawing.Color[] Resample(System.Drawing.Color[] source, int count)
        {
            if (source == null || source.Length == 0 || count <= 0)
            {
                return source;
            }
            if (count == 1)
            {
                return new[] { source[0] };
            }
            if (count == source.Length)
            {
                return (System.Drawing.Color[])source.Clone();
            }

            int last = source.Length - 1;
            var result = new System.Drawing.Color[count];
            for (int i = 0; i < count; i++)
            {
                double position = (double)i * last / (count - 1);
                int lower = (int)position;
                if (lower >= last)
                {
                    result[i] = source[last];
                    continue;
                }
                result[i] = Interpolate(source[lower], source[lower + 1], position - lower);
            }
            return result;
        }

        private static System.Drawing.Color Interpolate(System.Drawing.Color from, System.Drawing.Color to, double fraction)
        {
            return System.Drawing.Color.FromArgb(
                from.A + (int)((to.A - from.A) * fraction + 0.5),
                from.R + (int)((to.R - from.R) * fraction + 0.5),
                from.G + (int)((to.G - from.G) * fraction + 0.5),
                from.B + (int)((to.B - from.B) * fraction + 0.5));
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
                // The classification hands back one upper edge per break and the classes are the
                // gaps between them, so a break more than the classes asked for is what makes the
                // count of classes come out as the count that was picked.
                var breaks = JenksFisherFloat.CreateJenksFisherBreaksArray(array, numbreaks + 1);
                for (int i = 0; i < array.Length; i++)
                {
                    levels[i] = FindLevel(array[i], breaks);
                }
            }
            else if (clasmethod == ClassificationMethod.Equal_Inteval)
            {
                if (min == max)
                    max = min + 1;
                // The classes are counted the way the other method counts them, from the count the
                // user picked, and only then clamped to what the ramp can show. Dividing by the
                // length of the ramp instead meant the count had no effect on this method.
                var classes = numbreaks > 0 ? numbreaks : mColorRamp.Colors.Length;
                if (classes > mColorRamp.Colors.Length)
                    classes = mColorRamp.Colors.Length;
                var scale = classes / (max - min);
                for (int i = 0; i < array.Length; i++)
                {
                    levels[i] = (int)((array[i] - min) * scale);
                    if (levels[i] < 0)
                        levels[i] = 0;
                    else if (levels[i] >= classes)
                        levels[i] = classes - 1;
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
        /// Index of the interval [breaks[j], breaks[j+1]) the value falls into. The breaks come out of
        /// the classification in ascending order, so the interval is found by bisection instead of
        /// walking the whole list for every value. A value outside the edges ends up in the class at
        /// that end, the lowest below the first edge and the highest at or above the last one.
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
            // Walking off the top means the value is at or above the last edge, so it belongs to the
            // top class that high came to rest on. Falling back to zero here painted the highest
            // values of a layer with the colour of the lowest ones. Walking off the bottom, which is
            // a value under the first edge, is the one case that really is the first class.
            return high < 0 ? 0 : high;
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
