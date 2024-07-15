using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data.Classification
{
    [Serializable]
    public class EditorSettings 
    {
        #region Private Variables

        private Color _endColor;
        private string _excludeExpression;
        private bool _hsl;
        private int _hueShift;
        private IntervalMethod _intervalMethod;
        private int _intervalRoundingDigits;
        private IntervalSnapMethod _intervalSnapMethod;
        private int _maxSampleCount;
        private int _numBreaks;
        private bool _rampColors;
        private Color _startColor;
        private bool _useColor;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of EditorSettings
        /// </summary>
        public EditorSettings()
        {
            _hsl = true;
            _useColor = true;
            _startColor = SymbologyGlobal.ColorFromHsl(5, .7, .7);
            _endColor = SymbologyGlobal.ColorFromHsl(345, .8, .8);
            _maxSampleCount = 10000;
            _intervalMethod = IntervalMethod.EqualInterval;
            _rampColors = true;
            _intervalSnapMethod = IntervalSnapMethod.DataValue;
            _intervalRoundingDigits = 0;
            _numBreaks = 5;
        }

        #endregion

        #region Methods

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the characteristics of the "right" color.
        /// </summary>
        public Color EndColor
        {
            get { return _endColor; }
            set { _endColor = value; }
        }

        /// <summary>
        /// Gets or sets a string that allows the user to use any of the
        /// data fields to eliminate values from being considered as part
        /// of the histogram for statistical interval calculations.
        /// </summary>
        public string ExcludeExpression
        {
            get { return _excludeExpression; }
            set { _excludeExpression = value; }
        }

        /// <summary>
        /// Gets or sets a boolean indicating to display
        /// the hue, saturation, lightness as bounds
        /// instead of start-color, end-color.
        /// </summary>
        public bool HueSatLight
        {
            get { return _hsl; }
            set { _hsl = value; }
        }

        /// <summary>
        /// Gets or sets the hue shift.
        /// </summary>
        public int HueShift
        {
            get { return _hueShift; }
            set { _hueShift = value; }
        }

        /// <summary>
        /// Gets or sets the interval method
        /// </summary>
        public IntervalMethod IntervalMethod
        {
            get { return _intervalMethod; }
            set { _intervalMethod = value; }
        }

        /// <summary>
        /// Gets or sets the maximum sample count.
        /// </summary>
        public int MaxSampleCount
        {
            get { return _maxSampleCount; }
            set { _maxSampleCount = value; }
        }

        /// <summary>
        /// Gets or sets the integer count if equal breaks are used
        /// </summary>
        public int NumBreaks
        {
            get { return _numBreaks; }
            set { _numBreaks = value; }
        }

        /// <summary>
        /// Gets or sets whether this editor should ramp the colors,
        /// or use randomly generated colors.  The default is random.
        /// </summary>
        public bool RampColors
        {
            get { return _rampColors; }
            set { _rampColors = value; }
        }

        /// <summary>
        /// Gets or sets the characteristics of the "left" color.
        /// </summary>
        public Color StartColor
        {
            get { return _startColor; }
            set { _startColor = value; }
        }

        /// <summary>
        /// Gets or sets whether to use the color specifications
        /// </summary>
        public bool UseColorRange
        {
            get { return _useColor; }
            set { _useColor = value; }
        }

        /// <summary>
        /// Gets or sets how intervals like equal breaks choose the
        /// actual values, and whether they are rounded or snapped.
        /// </summary>
        public IntervalSnapMethod IntervalSnapMethod
        {
            get { return _intervalSnapMethod; }
            set { _intervalSnapMethod = value; }
        }

        /// <summary>
        /// Gets or sets the number of digits to preserve when IntervalSnapMethod is set to Rounding
        /// </summary>
        public int IntervalRoundingDigits
        {
            get { return _intervalRoundingDigits; }
            set { _intervalRoundingDigits = value; }
        }

        #endregion
    }
}
