using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.Drawing;

namespace HUST.WREIS.Dot3D.Display
{
    public class cPoint
    {
        public float X { get; set; }
        public float Y { get; set; }

        public cPoint(float x,float y)
        {
            X = x;
            Y = y;
        }
    }

    public class DataSource
    {
        public delegate String OnDrawXAxisLabelEvent(DataSource src, int idx);
        public delegate String OnDrawYAxisLabelEvent(DataSource src, float value);

        public OnDrawXAxisLabelEvent OnRenderXAxisLabel = null;
        public OnDrawYAxisLabelEvent OnRenderYAxisLabel = null;

        private cPoint[] samples = null;

        private int length = 0;
        private String name = String.Empty;
        private int downSample = 1;
        private Color color = Color.Black;

        public float VisibleDataRange_X = 0;
        public float DY = 0;
        public float YD0 = -200;
        public float YD1 = 200;
        public float Cur_YD0 = -200;
        public float Cur_YD1 = 200;

        public float grid_distance_y = 200;       // grid distance in units ( draw a horizontal line every 200 units )       
        public float off_Y = 0;
        public bool yFlip = true;
        public float grid_off_y = 0;
        public bool Active = true;
        private bool YAutoScaleGraph = false;
        private bool XAutoScaleGraph = false;
        public float XAutoScaleOffset = 100;
        public float CurGraphHeight = 1.0f;
        public float CurGraphWidth = 1.0f;

        public bool AutoScaleY
        {
            get
            {
                return YAutoScaleGraph;
            }
            set
            {
                YAutoScaleGraph = value;
            }
        }

        public bool AutoScaleX
        {
            get
            {
                return XAutoScaleGraph;
            }
            set
            {
                XAutoScaleGraph = value;
            }
        }

        public cPoint[] Samples
        {
            get
            {
                return samples;
            }
            set
            {
                samples = value;
                length = samples.Length;
            }
        }

        public float XMin
        {
            get
            {
                float x_min = float.MaxValue;
                if (samples.Length > 0)
                {
                    foreach (cPoint p in samples)
                    {
                        if (p.X < x_min) x_min = p.X;
                    }
                }
                return x_min;
            }
        }

        public float XMax
        {
            get
            {
                float x_max = float.MinValue;
                if (samples.Length > 0)
                {
                    foreach (cPoint p in samples)
                    {
                        if (p.X > x_max) x_max = p.X;
                    }
                }
                return x_max;
            }
        }

        public float YMin
        {
            get
            {
                float y_min = float.MaxValue;
                if (samples.Length > 0)
                {
                    foreach (cPoint p in samples)
                    {
                        if (p.Y < y_min) y_min = p.Y;
                    }
                }
                return y_min;
            }
        }

        public float YMax
        {
            get
            {
                float y_max = float.MinValue;
                if (samples.Length > 0)
                {
                    foreach (cPoint p in samples)
                    {
                        if (p.Y > y_max) y_max = p.Y;
                    }
                }
                return y_max;
            }
        }

        public void SetDisplayRangeY(float y_start, float y_end)
        {
            YD0 = y_start;
            YD1 = y_end;
        }

        public void SetGridDistanceY(float grid_dist_y_units)
        {
            grid_distance_y = grid_dist_y_units;
        }

        public void SetGridOriginY(float off_y)
        {
            grid_off_y = off_y;
        }

        [Category("Properties")] // Take this out, and you will soon have problems with serialization;
        [DefaultValue(typeof(string), "")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        [Category("Properties")] // Take this out, and you will soon have problems with serialization;
        [DefaultValue(typeof(Color), "")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color GraphColor
        {
            get { return color; }
            set { color = value; }
        }

        [Category("Properties")] // Take this out, and you will soon have problems with serialization;
        [DefaultValue(typeof(int), "0")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Length
        {
            get { return length; }
            set
            {
                length = value;
                if (length != 0)
                {
                    samples = new cPoint[length];
                }
                else
                {
                    // length is 0
                    if (samples != null)
                    {
                        samples = null;
                    }
                }
            }
        }


        [Category("Properties")] // Take this out, and you will soon have problems with serialization;
        [DefaultValue(typeof(int), "1")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Downsampling
        {
            get { return downSample; }
            set { downSample = value; }
        }

    } 
}
