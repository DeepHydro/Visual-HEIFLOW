//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

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
