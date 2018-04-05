// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.Hydrology
{

    public class River : HydroLine
    {
        public River(int id)
            : base(id)
        {
            HydroFeatureType = HydroFeatureType.River;
            Reaches = new List<Reach>();
            Upstreams = new List<River>();
        }

        public River(int id, River down, IEnumerable<River> ups)
            : base(id)
        {
            Downstream = down;
            Upstreams = new List<River>();
            Upstreams.AddRange(ups);
            HydroFeatureType = HydroFeatureType.River;
            ICALC = 1;
        }

        public River Downstream { get; set; }

        public List<River> Upstreams { get; set; }

        public List<Reach> Reaches { get; set; }

        //ICALC OUTSEG IClass1.csUPSEG FLOW RUNOFF ETSW PPTSW ROUGHCH

        public int ICALC { get; set; }
        public int OutRiverID { get; set; }
        public int UpRiverID { get; set; }
        public int IPrior { get; set; }
        public double Flow { get; set; }
        public double Runoff { get; set; }

        public Reach LastReach
        {
            get
            {
                return Reaches.Last();
            }
        }

        public Reach FirstReach
        {
            get
            {
                return Reaches.First();
            }
        }

        public IVectorTimeSeries<double> TimeSeries
        {
            get;
            set;
        }

        public void AddReach(Reach reach)
        {
            if (!Reaches.Contains(reach))
            {
                Reaches.Add(reach);
                reach.Parent = this;
                this.Length += reach.Length;
                this.Width = reach.Width;
            }
        }

        public Reach GetReach(int id)
        {
            if (id == 0)
                return null;
            var reach = Reaches.Where(r => r.ID == id);
            if (reach.Count() == 1)
                return reach.First();
            else
                return null;
        }

        public Reach GetReach(string name)
        {
            var reach = Reaches.Where(r => r.Name == name);
            if (reach.Count() == 1)
                return reach.First();
            else
                return null;
        }
    }
}
