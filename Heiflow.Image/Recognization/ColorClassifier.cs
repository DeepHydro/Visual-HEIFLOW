// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Image.ImageSets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Image.Recognization
{
    public class ColorClassifier : IColorClassifier
    {
        private Dictionary<int, double> _KnownRGB;
        private Color[] _KnownColors;
        public static double NoData = -9999;

        public ColorClassifier()
        {
            _KnownRGB = new Dictionary<int, double>();
        }
        public Color[] KnownColors
        {
            get
            {
                return _KnownColors;
            }
            set
            {
                _KnownColors = value;
            }
        }

        public double[] KnownValues { get; set; }

        public void Update()
        {
            _KnownRGB.Clear();
            for (int i = 0; i < KnownColors.Length; i++)
            {
                _KnownRGB.Add(_KnownColors[i].ToArgb(), KnownValues[i]);
            }
        }

        public double Classfiy(Color input)
        {
            double classfied = NoData;
            var rgb = input.ToArgb();
            if (_KnownRGB.Keys.Contains(rgb))
            {
                classfied = _KnownRGB[rgb];
            }
            return classfied;
        }
    }
}
