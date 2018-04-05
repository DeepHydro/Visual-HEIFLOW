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

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using DotSpatial.Data;
using NetTopologySuite.Geometries;

namespace Heiflow.Core.Hydrology
{
 
    public interface IHydroFeature 
    {
        int ID { get; set; }
        string Name { get; set; }
        string Comments { get; set; }
        Geometry Shapes { get; set; }
        BoundingBox BBox { get; set; }
        Point Position { get; set; }
        HydroFeatureType HydroFeatureType { get; set; }
        List<IHydroFeature> ParentFeatures { get; set; }
        List<IHydroFeature> SubFeatures { get; set; }
    }
    [Serializable]
    public class HydroFeature : IHydroFeature
    {
        public HydroFeature()
        {
            Modified = false;
        }

        public HydroFeature(int id)
        {
            mID = id;
            ParentFeatures = new List<IHydroFeature>();
            SubFeatures = new List<IHydroFeature>();
            HydroFeatureType = HydroFeatureType.Unknown;
            Modified = false;
        }

        protected int mID;
        protected Shape mShape;

        public Shape Shape
        {
            get
            {
                return mShape;
            }
        }

        #region IHydroFeature 成员

        /// <summary>
        /// ID starts from 1
        /// </summary>
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }
        public int OldID
        {
            get;
            set;
        }

        public string Code
        {
            set;
            get;
        }

        public string Name
        {
            get;
            set;
        }

        public HydroFeatureType HydroFeatureType
        {
            get;
            set;
        }

        public List<IHydroFeature> ParentFeatures
        {
            get;
            set;
        }

        public List<IHydroFeature> SubFeatures
        {
            get;
            set;
        }

        public Geometry Shapes { get; set; }

       public BoundingBox BBox { get; set; }

        public Point Position { get; set; }

        public string Comments { get; set; }

        #endregion
        public bool Modified { get; set; }
        /// <summary>
        /// starting from zero
        /// </summary>
        public int SubIndex { get; set; }

        public int SubID { get; set; }

        public object Tag { get; set; }
    }
}
