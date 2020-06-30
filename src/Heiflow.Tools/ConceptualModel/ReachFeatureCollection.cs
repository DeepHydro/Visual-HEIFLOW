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
           // Reaches = new SortedList<double, ReachFeature>();
        }

        public int SegmentID
        {
            get;
            set;
        }
        public int OutSegmentID
        {
            get;
             set;
        }
        public SortedList<double, ReachFeature> Reaches
        {
            get;
            set;
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
        /// <summary>
        /// starting from 1
        /// </summary>
        public int Column { get; set; }
        /// <summary>
        /// starting from 1
        /// </summary>
        public int Row { get; set; }
        public int CellID { get; set; }
        public DataRow DataRow { get; set; }
        public double Elevation { get; set; }
        public double Slope { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double OrderKey { get; set; }

        public double MinSegElevation { get; set; }
        public double MaxSegElevation { get; set; }
        public double MeanSlope { get; set; }
        public int UpRiverID { get; set; }
        public int IPrior { get; set; }
        public double Flow { get; set; }
        public double Offset { get; set; }

        public double STRCH1 { get; set; }

        public double Runoff { get; set; }

        public double Rainfall { get; set; }

        public double ET { get; set; }

        public double Roughness { get; set; }

        public double BedThickness { get; set; }

        public double THTI { get; set; }

        public double THTS { get; set; }

        public double EPS { get; set; }
    }
}
