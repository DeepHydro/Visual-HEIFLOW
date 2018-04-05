// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace  Heiflow.AI.NeuronDotNet.Core.SOM
{
    public class KohonenMapClassification
    {
        public KohonenMapClassification(string clsname)
        {
            ClassifiedInputPatternIndex = new List<int>();
            mName = clsname;
            ColorIndicator = Color.Orange;
        }

        private string mName;
        public int X { get; set; }
        public int Y { get; set; }
        public List<int> ClassifiedInputPatternIndex { get; set; }
        public Color ColorIndicator { get; set; }

        public int NumberOfInsideInputPatterns { get; set; }

        public string ClassName
        {
            get { return mName; }
        }

    }
}
