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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using Heiflow.Core.Data;
using GeoAPI.Geometries;

namespace Heiflow.Core.Hydrology
{
  
    public class HydroPoint : HydroFeature, IObservationsSite
    {
        public HydroPoint(int id)
            : base(id)
        {
            HydroFeatureType = HydroFeatureType.HydroPoint;
            MeasuredVariables = new List<IVariable>();
            Coordinate = new Coordinate();
        }

        protected string[] mObservationItems;
        protected River mRiver;

        public int Row { get; set; }
        public int Column { get; set; }
        public int ActualID { get; set; }
        public int CellID { get; set; }
        public int Layer { get; set; }
        public Coordinate Coordinate { get; set; }

        [CategoryAttribute("Topology"), DescriptionAttribute("The river at which the hydro point locates")]
        /// <summary>
        /// /River
        /// </summary>
        public River RiverObject
        {
            get
            {
                return mRiver;
            }
            set
            {
                mRiver = value;
            }
        }

        [CategoryAttribute("Topology"), DescriptionAttribute("The reach at which the hydro point locates")]
        /// <summary>
        /// /River
        /// </summary>
        public Reach ReachObject
        {
            get;
            set;
        }

        [CategoryAttribute("Topology"), DescriptionAttribute("Names of measured variables")]
        /// <summary>
        /// Names of  variables measured at the hydropoint
        /// </summary>
        public string[] ObservationItems
        {
            set
            {
                mObservationItems = value;
            }
            get
            {
                return mObservationItems;
            }
        }


        public List<IVectorTimeSeries<double>> TimeSeriesCollection { get; set; }

        public int SpatialIndex { get; set; }

        public double Elevation { get; set; }
        public double Cell_Elevation { get; set; }
        public static HydroPoint [] FromShpfile(string shpfile)
        {
            var shp = DotSpatial.Data.FeatureSet.Open(shpfile);
            var dt = shp.DataTable;
            var hobs = from dr in dt.AsEnumerable()
                       select new HydroPoint(int.Parse(dr["SiteID"].ToString()))
                       {
                           Column = int.Parse(dr["COLUMN"].ToString()),
                           Row = int.Parse(dr["ROW"].ToString()),
                           CellID = int.Parse(dr["Cell_ID"].ToString()),
                           Name = dr["SiteName"].ToString(),
                           ActualID = int.Parse(dr["ACT_ID"].ToString()),
                       };
            return hobs.ToArray();
        }

        #region IObservationsSite members
        public double Longitude
        {
            get;
            set;
        }

        public double Latitude
        {
            get;
            set;
        }

        public double LocalX
        {
            get;
            set;
        }

        public double LocalY
        {
            get;
            set;
        }

        public double Distance { get; set; }

        public List<IVariable> MeasuredVariables
        {
            get;
            set;
        }

        public ITimeSeries<double> TimeSeries
        {
            get;
            set;
        }

        public string SiteType { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public int MonitorType { get; set; }

        #endregion


        
    }
}
