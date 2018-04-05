// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
