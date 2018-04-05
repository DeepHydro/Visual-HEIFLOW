using DotSpatial.Data;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.ConceptualModel
{
    public class ReachFeatureCollection
    {
        public ReachFeatureCollection(int segid)
        {
            SegmentID = segid;
            Reaches = new SortedList<double, ReachFeature>();
        }

        public int SegmentID
        {
            get;
            private  set;
        }
        public int OutSegmentID
        {
            get;
             set;
        }
        public SortedList<double, ReachFeature> Reaches
        {
            get;
            private set;
        }
    
        public int NReach
        {
            get
            {
                return Reaches.Keys.Count;
            }
        }
    }

    public class ReachFeature
    {
        public DataRow Row { get; set; }
        public double Elevation { get; set; }
        public double Slope { get; set; }
    }
}
