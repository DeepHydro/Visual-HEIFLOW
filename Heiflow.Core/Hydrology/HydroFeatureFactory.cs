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
