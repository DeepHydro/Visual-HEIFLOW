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

            // A line through the flow instead of an arrow on every cell, see Streamline.fx. The
            // tile count is what says how far apart the lines start, so it is shared with the
            // arrows rather than kept twice.
            ShowStreamlines = false;
            StreamlineSteps = 16;
            StreamlineSmooth = 4;
            StreamlineSpacing = 1;
            StreamlineWidth = 2f;
            FlowSpeed = 2f;
            FadeTime = 2.5f;
            DashCells = 6f;
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
        private bool _ShowStreamlines;
        private int _StreamlineSteps;
        private int _StreamlineSmooth;
        private int _StreamlineSpacing;
        private float _StreamlineWidth;
        private float _FlowSpeed;
        private float _FadeTime;

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

        /// <summary>
        /// Draw a line traced through the flow instead of an arrow on a cell. The mesh then holds
        /// lines and is drawn through Streamline.fx, which runs dashes along them.
        /// </summary>
        [Browsable(false), Category("Behavior")]
        public bool ShowStreamlines
        {
            get
            {
                return _ShowStreamlines;
            }
            set
            {
                _ShowStreamlines = value;
                OnStyleChanged();
            }
        }

        /// <summary>
        /// How many cells one line is traced through, which is how long it gets.
        /// </summary>
        [Category("Behavior")]
        public int StreamlineSteps
        {
            get
            {
                return _StreamlineSteps;
            }
            set
            {
                _StreamlineSteps = Math.Max(1, value);
                OnStyleChanged();
            }
        }

        /// <summary>
        /// How many points a line is cut into for every cell it crosses. The trace itself walks from
        /// cell to cell, which is what makes it a chain of straight pieces, and these are the points
        /// a curve is put through to take the corners off it. One point per cell draws the chain as
        /// it is, four draws a curve that no longer shows where a cell ended.
        /// </summary>
        [Category("Behavior")]
        public int StreamlineSmooth
        {
            get
            {
                return _StreamlineSmooth;
            }
            set
            {
                _StreamlineSmooth = Math.Max(1, Math.Min(12, value));
                OnStyleChanged();
            }
        }

        /// <summary>
        /// How many cells a line keeps clear around itself. A line stops where another one already
        /// runs within this many cells, and none is started where one is that close, so that two
        /// lines neither lie over each other nor cross. Zero asks for no room at all and lets the
        /// lines run where they will, which is how they were drawn before.
        /// </summary>
        [Category("Behavior")]
        public int StreamlineSpacing
        {
            get
            {
                return _StreamlineSpacing;
            }
            set
            {
                _StreamlineSpacing = Math.Max(0, Math.Min(4, value));
                OnStyleChanged();
            }
        }

        /// <summary>
        /// How wide a line is drawn, in pixels. A line is a ribbon of two triangles a vertex wide
        /// rather than a line list, because a card is free to ignore the width of a line, and most
        /// of them do.
        /// </summary>
        [Category("Behavior")]
        public float StreamlineWidth
        {
            get
            {
                return _StreamlineWidth;
            }
            set
            {
                _StreamlineWidth = Math.Max(1f, Math.Min(12f, value));
                OnStyleChanged();
            }
        }

        /// <summary>
        /// How fast a dash runs along a line, in cells of the grid per second.
        /// </summary>
        [Category("Behavior")]
        public float FlowSpeed
        {
            get
            {
                return _FlowSpeed;
            }
            set
            {
                _FlowSpeed = Math.Max(0f, value);
                OnStyleChanged();
            }
        }

        /// <summary>
        /// How long a dash stays lit behind its head, in seconds. A longer time leaves a longer
        /// trail, and a dash is gone where it has run its course.
        /// </summary>
        [Category("Behavior")]
        public float FadeTime
        {
            get
            {
                return _FadeTime;
            }
            set
            {
                _FadeTime = Math.Max(0.1f, value);
                OnStyleChanged();
            }
        }

        /// <summary>
        /// How many cells of a line are left dark between the trail of one dash and the head of the
        /// next, turned into metres by the grid it is drawn on.
        /// </summary>
        [Category("Behavior")]
        public float DashCells { get; set; }

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
