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

using Heiflow.Core;
using Heiflow.Core.Data;
 

namespace Heiflow.Core.Hydrology
{
    public  class HydroNetwork
    {
        public HydroNetwork(IDBase dbase)
        {
            mDBase = dbase;
            mODMDataAdaptor = new ODMDataAdaptor(dbase);
        }

        protected IDBase mDBase;
        private ODMDataAdaptor mODMDataAdaptor;

        public Basin[] GetTopLevelBasins(int topFeatureID)
        {
             Basin[] results = null;
             string sql = "select * from " + Configuration.HydroFeaturesTableName + " where ParentFeatureID=" + topFeatureID.ToString() + " and FeatureType='Basin'";
            DataTable dt = Configuration.DataBase.QueryDataTable(sql);
            if (dt != null)
            {
                var basins = from row in dt.AsEnumerable()
                             select new Basin(row.Field<int>("FeatureID"))
                                 {
                                     Name = row.Field<string>("FeatureName"),
                                     Comments = row.Field<string>("Descriptions")
                                 };
                results = basins.ToArray();
            }
            return results;
        }

        /// <summary>
        ///  Add all sub-features to the parent feature recursively.
        /// </summary>
        /// <param name="parent"></param>
        public void CreateTopDownNetwork(HydroFeature parent)
        {
            string sql = "select * from " + Configuration.HydroFeaturesTableName + " where ParentFeatureID=" + parent.ID;
            DataTable dt = mDBase.QueryDataTable(sql);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int id = Convert.ToInt32(dr["FeatureID"].ToString());
                    string type = dr["FeatureType"].ToString();
                    string name = dr["FeatureName"].ToString();
                    string des = dr["Descriptions"].ToString();
                    HydroFeatureType htype = HydroFeatureFactory.GetHydroFeaturetype(type);
                    HydroFeature feature = HydroFeatureFactory.CreateHydroFeature(id, name, des, htype);
                    if (feature.HydroFeatureType == HydroFeatureType.HydroPoint)
                        feature = mODMDataAdaptor.GetSiteInfo(id);
                    parent.SubFeatures.Add(feature);
                    feature.ParentFeatures.Add(parent);
                    if (feature.HydroFeatureType != HydroFeatureType.HydroPoint)
                        CreateTopDownNetwork(feature);
                }
            }
        }

        //public void CreateBasinNetwork( Basin basin)
        //{
        //    string sql = "select * from " + Configuration.HydroFeaturesTableName + " where ParentFeatureID=" + basin.ID;
        //    DataTable dt = mDBase.QueryDataTable(sql);
        //    if (dt != null)
        //    {
        //        var watersheds = from row in dt.AsEnumerable()
        //                            select new Watershed(row.Field<int>("FeatureID"))
        //                            {
        //                                Name = row.Field<string>("FeatureName"),
        //                                Descriptions = row.Field<string>("Descriptions")
        //                            };
        //        foreach (Watershed w in watersheds)
        //        {
        //            CreateWatershedNetwork(w);
        //            basin.SubFeatures.Add(w);
        //        }                         
        //    }
        //}

        //public void CreateWatershedNetwork(Watershed w)
        //{
        //    string sql = "select * from " + Configuration.HydroFeaturesTableName + " where ParentFeatureID=" + w.ID;
        //    DataTable dt = mDBase.QueryDataTable(sql);
        //    if (dt != null)
        //    {
        //        var rivers = from row in dt.AsEnumerable()
        //                     select new River(row.Field<int>("FeatureID"))
        //                     {
        //                         Name = row.Field<string>("FeatureName"),
        //                         Descriptions = row.Field<string>("Descriptions")
        //                     };
        //        foreach (River r in rivers)
        //        {
        //            CreateRiverNetwork(r);
        //            w.SubFeatures.Add(r);
        //        }
        //    }
        //}

        //public void CreateRiverNetwork(River r)
        //{
        //    string sql = "select * from " + Configuration.HydroFeaturesTableName + " where ParentFeatureID=" + r.ID;
        //    DataTable dt = mDBase.QueryDataTable(sql);
        //    if (dt != null)
        //    {
        //        var sites = from row in dt.AsEnumerable() select row.Field<int>("FeatureID");
        //        r.SubFeatures.AddRange(mODMDataAdaptor.GetSites(sites.ToArray())); 
        //    }
        //}

    }
}

