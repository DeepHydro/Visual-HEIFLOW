// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Heiflow.Core.Data;

namespace Heiflow.Core.Hydrology
{
    public class HydroFeatureFactory
    {
        public HydroFeatureFactory()
        {
        }

        public static HydroFeatureType GetHydroFeaturetype(string type)
        {
            HydroFeatureType t = HydroFeatureType.Unknown;
            switch (type)
            {
                case "Watershed":
                    t = HydroFeatureType.Watershed;
                    break;
                case "Basin":
                    t = HydroFeatureType.Basin;
                    break;
                case "River":
                    t = HydroFeatureType.River;
                    break;
                case "Site":
                    t = HydroFeatureType.Site;
                    break;
                case "HydroPoint":
                    t = HydroFeatureType.HydroPoint;
                    break;
                case "Irrigation System":
                    t = HydroFeatureType.IrrigationSystem;
                    break;
                case "Lake":
                    t = HydroFeatureType.Lake;
                    break;
                case "Unknown":
                    t = HydroFeatureType.Unknown;
                    break;
            }
            return t;
        }

        public static HydroFeature CreateHydroFeature(int id,string name,string descrip,HydroFeatureType type)
        {
            HydroFeature feature = null;
            switch (type)
            {
                case HydroFeatureType.Basin:
                    feature = new Basin(id);
                    break;
                case HydroFeatureType.Watershed:
                    feature = new  Watershed(id);
                    break;
                case HydroFeatureType.River:
                    feature = new  River(id);
                    break;
                case HydroFeatureType.Site:
                    feature = new HydroPoint(id);
                    break;
                case HydroFeatureType.HydroPoint:
                    feature = new HydroPoint(id);
                    break;
                case HydroFeatureType.IrrigationSystem:
                    feature = new  IrrigationSystem(id);
                    break;
            }
            feature.Name = name;
            feature.Comments = descrip;
            return feature;
        }

        public static HydroFeature GetHydroFeature(int id, IDBase dBase)
        {
            string sql = "select * from " + Configuration.HydroFeaturesTableName + " where FeatureID=" + id;
            DataTable dt = dBase.QueryDataTable(sql);
            if (dt != null)
            {
                DataRow dr = dt.Rows[0];
                string type = dr["FeatureType"].ToString();
                string name = dr["FeatureName"].ToString();
                string des = dr["Descriptions"].ToString();
                HydroFeatureType htype = HydroFeatureFactory.GetHydroFeaturetype(type);
               return CreateHydroFeature(id, name, des, htype);
            }
            else
            {
                return null;
            }
        }
    }
}
