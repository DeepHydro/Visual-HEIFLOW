// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
