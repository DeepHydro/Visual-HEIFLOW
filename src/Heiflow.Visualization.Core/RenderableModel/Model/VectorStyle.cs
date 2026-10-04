using HUST.WREIS.Dot3D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable.Grid
{
    [Serializable]
    public class VectorStyle
    {
        public VectorStyle()
        {
            _HeadSize = 800;
            MaximumSize = 800;
            MinimumSize = 10;
            ArrowAngle = (float)Math.PI / 12;
            AutoColor = false;
            ScaleMethod = ScaleMethod.StandardDeviation;
            ArrowStyle = ArrowStyle.HeadOnly;
            StandardDevisionNumber = 2;
            HeadSizeFilter = 0.02;
            mArrowColor = Color.White;
            _TailSize = 1.5;
            _Dehisce = 0.6;
            _TileCount = 3;
        }
        private Color mArrowColor;
        private bool _AutoColor;
        private ScaleMethod _ScaleMethod;
        private int _ArrowColor;
        private double _HeadSize;
        private double _TailSize;
        private double _Dehisce;
        public event EventHandler StyleChanged;
        private int _TileCount;

        [Browsable(true), Category("Head")]
        public double HeadSize
        {
            get
            {
                return _HeadSize;
            }
            set
            {
                _HeadSize = value;
                OnStyleChanged();
            }
        }

        [Browsable(true), Category("Arrow")]
        public double TailSize
        {
            get
            {
                return _TailSize;
            }
            set
            {
                _TailSize = value;
                OnStyleChanged();
            }
        }

        [Browsable(true), Category("Head")]
        public double Dehisce
        {
            get
            {
                return _Dehisce;
            }
            set
            {
                _Dehisce = value;
                OnStyleChanged();
            }
        }

        [Browsable(true), Category("Arrow")]
        public double ArrowAngle { get; set; }

        [Browsable(true), Category("Arrow")]
        public double MaximumSize { get; set; }

        [Browsable(true), Category("Arrow")]
        public double MinimumSize
        { get; set; }

        [Category("Arrow")]
        public Color Color
        {
            get
            {
                return mArrowColor;
            }
            set
            {
                mArrowColor = value;
                ArrowColor = value.ToArgb();
                OnStyleChanged();
            }
        }

        [Browsable(false), Category("Arrow")]
        public int ArrowColor
        {
            get
            {
                return _ArrowColor;
            }
            set
            {
                _ArrowColor = value;
                OnStyleChanged();
            }
        }

        [Category("Arrow")]
        public ArrowStyle ArrowStyle { get; set; }

        [Browsable(false), Category("Behavior")]
        public bool AutoColor 
        { 
            get
            {
                return _AutoColor;
            }
            set
            {
                _AutoColor = value;
                OnStyleChanged();
            }
        }

        [Browsable(false), Category("Behavior")]
        public ScaleMethod ScaleMethod
        {
            get
            {
                return _ScaleMethod;
            }
            set
            {
                _ScaleMethod = value;
                OnStyleChanged();
            }
        }
            [Category("Behavior")]
        public int TileCount
        {
            get
            {
                return _TileCount;
            }
            set
            {
                _TileCount = value;
                OnStyleChanged();
            }
        }

        [Category("Behavior")]
        public int StandardDevisionNumber { get; set; }

        [Category("Behavior")]
        public double HeadSizeFilter { get; set; }

        private void OnStyleChanged()
        {
            if(StyleChanged != null)
            {
                StyleChanged(this, EventArgs.Empty);
            }
        }

        public double ScaleHeadSize(double value, double max, double min, double mean, double sd)
        {
            double size = 0;

            if (max == min)
                return HeadSize;

            if (ScaleMethod == ScaleMethod.None)
            {
                size = HeadSize;
            }
            else
            {
                double newMax = max;
                double newMin = min;
                double maxSize = MaximumSize;
                double minSize = MinimumSize;

                if (ScaleMethod == ScaleMethod.StandardDeviation)
                {
                    newMax = mean + StandardDevisionNumber * sd;
                    newMin = mean - StandardDevisionNumber * sd;
                    if (ArrowStyle == ArrowStyle.HeadOnly)
                    {
                        maxSize = HeadSize * (1 - HeadSizeFilter);
                        minSize = HeadSize * HeadSizeFilter;
                    }
                }
                else
                {
                    if (ArrowStyle == ArrowStyle.HeadOnly)
                    {
                        maxSize = MaximumSize;
                        minSize = MinimumSize;
                    }
                }

                if (value < newMin)
                {
                    size = minSize;
                }
                else if (value > newMax)
                {
                    size = maxSize;
                }
                else
                {
                    size = (value - newMin) / (newMax - newMin) * HeadSize;
                }
            }
            return size;
        }
    }
}
