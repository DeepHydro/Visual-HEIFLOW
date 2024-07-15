using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data.Classification
{
    public class Scheme  
    {
        #region Private Variables

        private List<Break> _breaks; // A temporary list for helping construction of schemes.
        private EditorSettings _editorSettings;
        private Statistics _statistics;
        private List<double> _values;

        #endregion

        #region Nested type: Break

        /// <summary>
        /// Breaks for value ranges
        /// </summary>
        protected class Break
        {
            /// <summary>
            /// A double value for the maximum value for the break
            /// </summary>
            public double? Maximum { get; set; }

            /// <summary>
            /// The string name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            ///  Creates a new instance of a break
            /// </summary>
            public Break()
            {
                Name = string.Empty;
                Maximum = 0;
            }

            /// <summary>
            /// Creates a new instance of a break with a given name
            /// </summary>
            /// <param name="name">The string name for the break</param>
            public Break(string name)
            {
                Name = name;
                Maximum = 0;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of DrawingScheme
        /// </summary>
        public Scheme()
        {
            _statistics = new Statistics();
            _editorSettings = new EditorSettings();
        }

        #endregion

        #region Methods

  

        /// <summary>
        /// Generates the break categories for this scheme
        /// </summary>
        public void CreateBreakCategories()
        {
            int count = EditorSettings.NumBreaks;
            switch (EditorSettings.IntervalMethod)
            {
                case IntervalMethod.EqualFrequency:
                    Breaks = GetQuantileBreaks(count);
                    break;
                case IntervalMethod.NaturalBreaks:
                    Breaks = GetNaturalBreaks(count);
                    break;
                default:
                    Breaks = GetEqualBreaks(count);
                    break;
            }
            ApplyBreakSnapping();
            SetBreakNames(Breaks);
            //List<Color> colorRamp = GetColorSet(count);
            //List<double> sizeRamp = GetSizeSet(count);
           
            //int colorIndex = 0;
            //Break prevBreak = null;
            //foreach (Break brk in Breaks)
            //{
            //    //get the color for the category
            //    Color randomColor = colorRamp[colorIndex];
            //    double randomSize = sizeRamp[colorIndex];
            //    prevBreak = brk;
            //    colorIndex++;
            //}
        }

        public int[] GetBreakIndex()
        {
            var list = new int[Values.Count];
            var nbr = Breaks.Count;
            var len2 = nbr - 2;
            for (int i = 0; i < Values.Count; i++)
            {
                list[i] = 0;
                for (int j = len2; j >= 0; j--)
                {
                    if(Values[i] > Breaks[j].Maximum)
                    {
                        list[i] = j+1;
                        break;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// THe defaul
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        protected virtual List<double> GetSizeSet(int count)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < count; i++)
            {
                result.Add(20);
            }
            return result;
        }

        /// <summary>
        /// Creates a list of generated colors according to the convention
        /// specified in the EditorSettings.
        /// </summary>
        /// <param name="count">The integer count of the number of colors to create.</param>
        /// <returns>The list of colors created.</returns>
        protected List<Color> GetColorSet(int count)
        {
            List<Color> colorRamp = null;
            if (EditorSettings.UseColorRange)
            {
                if (!EditorSettings.RampColors)
                {
                    colorRamp = CreateRandomColors(count);
                }
                else if (!EditorSettings.HueSatLight)
                {
                    colorRamp = CreateRampColors(count, EditorSettings.StartColor, EditorSettings.EndColor);
                }
                else
                {
                    Color cStart = EditorSettings.StartColor;
                    Color cEnd = EditorSettings.EndColor;
                    colorRamp = CreateRampColors(count, cStart.GetSaturation(), cStart.GetBrightness(),
                                                 (int)cStart.GetHue(),
                                                 cEnd.GetSaturation(), cEnd.GetBrightness(), (int)cEnd.GetHue(),
                                                 EditorSettings.HueShift, cStart.A, cEnd.A);
                }
            }
            else
            {
                colorRamp = GetDefaultColors(count);
            }
            return colorRamp;
        }

    

        /// <summary>
        /// Creates the colors in the case where the color range controls are not being used.
        /// This can be overriddend for handling special cases like ponit and line symbolizers
        /// that should be using the template colors.
        /// </summary>
        /// <param name="count">The integer count to use</param>
        /// <returns></returns>
        protected virtual List<Color> GetDefaultColors(int count)
        {
            return EditorSettings.RampColors ? CreateUnboundedRampColors(count) : CreateUnboundedRandomColors(count);
        }

        /// <summary>
        /// The default behavior for creating ramp colors is to create colors in the mid-range for
        /// both lightness and saturation, but to have the full range of hue
        /// </summary>
        /// <param name="numColors"></param>
        /// <returns></returns>
        private static List<Color> CreateUnboundedRampColors(int numColors)
        {
            return CreateRampColors(numColors, .25f, .25f, 0, .75f, .75f, 360, 0, 255, 255);
        }

        private static List<Color> CreateUnboundedRandomColors(int numColors)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            List<Color> result = new List<Color>(numColors);
            for (int i = 0; i < numColors; i++)
            {
                result.Add(rnd.NextColor());
            }
            return result;
        }

        private List<Color> CreateRandomColors(int numColors)
        {
            List<Color> result = new List<Color>(numColors);
            Random rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < numColors; i++)
            {
                result.Add(CreateRandomColor(rnd));
            }
            return result;
        }

        /// <summary>
        /// Creates a random color, but accepts a given random class instead of creating a new one.
        /// </summary>
        /// <param name="rnd"></param>
        /// <returns></returns>
        protected Color CreateRandomColor(Random rnd)
        {
            Color startColor = EditorSettings.StartColor;
            Color endColor = EditorSettings.EndColor;
            if (EditorSettings.HueSatLight)
            {
                double hLow = startColor.GetHue();
                double dH = endColor.GetHue() - hLow;
                double sLow = startColor.GetSaturation();
                double ds = endColor.GetSaturation() - sLow;
                double lLow = startColor.GetBrightness();
                double dl = endColor.GetBrightness() - lLow;
                double aLow = (startColor.A) / 255.0;
                double da = (endColor.A - aLow) / 255.0;
                return SymbologyGlobal.ColorFromHsl(rnd.NextDouble() * dH + hLow, rnd.NextDouble() * ds + sLow,
                                                    rnd.NextDouble() * dl + lLow).ToTransparent((float)(rnd.NextDouble() * da + aLow));
            }
            int rLow = Math.Min(startColor.R, endColor.R);
            int rHigh = Math.Max(startColor.R, endColor.R);
            int gLow = Math.Min(startColor.G, endColor.G);
            int gHigh = Math.Max(startColor.G, endColor.G);
            int bLow = Math.Min(startColor.B, endColor.B);
            int bHigh = Math.Max(startColor.B, endColor.B);
            int iaLow = Math.Min(startColor.A, endColor.A);
            int aHigh = Math.Max(startColor.A, endColor.A);
            return Color.FromArgb(rnd.Next(iaLow, aHigh), rnd.Next(rLow, rHigh), rnd.Next(gLow, gHigh), rnd.Next(bLow, bHigh));
        }

        private static List<Color> CreateRampColors(int numColors, Color startColor, Color endColor)
        {
            List<Color> result = new List<Color>(numColors);
            double dR = (endColor.R - (double)startColor.R) / numColors;
            double dG = (endColor.G - (double)startColor.G) / numColors;
            double dB = (endColor.B - (double)startColor.B) / numColors;
            double dA = (endColor.A - (double)startColor.A) / numColors;
            for (int i = 0; i < numColors; i++)
            {
                result.Add(Color.FromArgb((int)(startColor.A + dA * i), (int)(startColor.R + dR * i), (int)(startColor.G + dG * i), (int)(startColor.B + dB * i)));
            }
            return result;
        }

 

        /// <summary>
        /// Uses the currently calculated Values in order to calculate a list of breaks
        /// that have equal separations.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        protected List<Break> GetEqualBreaks(int count)
        {
            List<Break> result = new List<Break>();
            double min = Values[0];
            double dx = (Values[Values.Count - 1] - min) / count;

            for (int i = 0; i < count; i++)
            {
                Break brk = new Break();
                // max
                if (i == count - 1)
                {
                    brk.Maximum = null;
                }
                else
                {
                    brk.Maximum = min + (i + 1) * dx;
                }
                result.Add(brk);
            }
            return result;
        }

        /// <summary>
        /// Applies the snapping type to the given breaks
        /// </summary>
        protected void ApplyBreakSnapping()
        {
            if (Values == null || Values.Count == 0) return;
            switch (EditorSettings.IntervalSnapMethod)
            {
                case IntervalSnapMethod.None:
                    break;
                case IntervalSnapMethod.SignificantFigures:
                    foreach (Break item in Breaks)
                    {
                        if (item.Maximum == null) continue;
                        double val = (double)item.Maximum;
                        item.Maximum = Utils.SigFig(val, EditorSettings.IntervalRoundingDigits);
                    }
                    break;
                case IntervalSnapMethod.Rounding:
                    foreach (Break item in Breaks)
                    {
                        if (item.Maximum == null) continue;
                        item.Maximum = Math.Round((double)item.Maximum, EditorSettings.IntervalRoundingDigits);
                    }
                    break;
                case IntervalSnapMethod.DataValue:
                    foreach (Break item in Breaks)
                    {
                        if (item.Maximum == null) continue;
                        item.Maximum = Utils.GetNearestValue((double)item.Maximum, Values);
                    }
                    break;
            }
        }

        /// <summary>
        /// Attempts to create the specified number of breaks with equal numbers of members in each.
        /// </summary>
        /// <param name="count">The integer count.</param>
        /// <returns>A list of breaks.</returns>
        protected List<Break> GetQuantileBreaks(int count)
        {
            List<Break> result = new List<Break>();
            int binSize = (int)Math.Ceiling(Values.Count / (double)count);
            for (int iBreak = 1; iBreak <= count; iBreak++)
            {
                if (binSize * iBreak < Values.Count)
                {
                    Break brk = new Break();
                    brk.Maximum = Values[binSize * iBreak];
                    result.Add(brk);
                }
                else
                {
                    // if num breaks is larger than number of members, this can happen
                    Break brk = new Break();
                    brk.Maximum = null;
                    result.Add(brk);
                    break;
                }
            }
            return result;
        }

        protected List<Break> GetNaturalBreaks(int count)
        {
            var breaks = new JenksBreaksCalcuation(Values, count);
            breaks.Optimize();
            var results = breaks.GetResults();

            var output = new List<Break>(count);
            output.AddRange(results.Select(result => new Break
            {
                Maximum = Values[result]
            }));

            // Set latest Maximum to null
            output.Last().Maximum = null;

            return output;
        }

        /// <summary>
        /// Sets the names for the break categories
        /// </summary>
        /// <param name="breaks"></param>
        protected static void SetBreakNames(IList<Break> breaks)
        {
            for (int i = 0; i < breaks.Count; i++)
            {
                Break brk = breaks[i];
                if (breaks.Count == 1)
                {
                    brk.Name = "All Values";
                }
                else if (i == 0)
                {
                    brk.Name = "<= " + brk.Maximum;
                }
                else if (i == breaks.Count - 1)
                {
                    brk.Name = "> " + breaks[i - 1].Maximum;
                }
                else
                {
                    brk.Name = breaks[i - 1].Maximum + " - " + brk.Maximum;
                }
            }
        }

        private static List<Color> CreateRampColors(int numColors, float minSat, float minLight, int minHue, float maxSat, float maxLight, int maxHue, int hueShift, int minAlpha, int maxAlpha)
        {
            List<Color> result = new List<Color>(numColors);
            double ds = (maxSat - (double)minSat) / numColors;
            double dh = (maxHue - (double)minHue) / numColors;
            double dl = (maxLight - (double)minLight) / numColors;
            double dA = (maxAlpha - (double)minAlpha) / numColors;
            for (int i = 0; i < numColors; i++)
            {
                double h = (minHue + dh * i) + hueShift % 360;
                double s = minSat + ds * i;
                double l = minLight + dl * i;
                float a = (float)(minAlpha + dA * i) / 255f;
                result.Add(SymbologyGlobal.ColorFromHsl(h, s, l).ToTransparent(a));
            }
            return result;
        }

        #endregion

        #region Properties

        public EditorSettings EditorSettings
        {
            get { return _editorSettings; }
            set { _editorSettings = value; }
        }
 
        public Statistics Statistics
        {
            get
            {
                return _statistics;
            }
            protected set
            {
                _statistics = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of breaks for this scheme
        /// </summary>
        protected List<Break> Breaks
        {
            get { return _breaks; }
            set { _breaks = value; }
        }

 
        public List<double> Values
        {
            get { return _values; }
            set { _values = value; }
        }

        #endregion
    }
}
