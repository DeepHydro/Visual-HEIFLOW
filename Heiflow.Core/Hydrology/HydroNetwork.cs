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

