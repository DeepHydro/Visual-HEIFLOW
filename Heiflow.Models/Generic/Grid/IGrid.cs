// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Data;
using ILNumerics;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using DotSpatial.Controls;
using GeoAPI.Geometries;

namespace Heiflow.Models.Generic
{

    public interface IGrid
    {
        event EventHandler SizeChanged;
        event GridUpdate Updated;
        Coordinate Origin { get; set; }
        Coordinate BBoxCentroid { get; set; }
        int LayerCount
        {
            get;
        }

        int ActualLayerCount
        {
            set;
            get;
        }
        IBasicModel Owner
        {
            get;
            set;
        }

        IFeatureSet FeatureSet
        {
            get;
            set;
        }

        IMapFeatureLayer FeatureLayer
        {
            get;
            set;
        }

        IFeatureSet CentroidFeature
        {
            get;
            set;
        }

        IMapFeatureLayer CentroidFeatureLayer
        {
            get;
            set;
        }

        Envelope BBox
        {
            get;
            set;
        }

        ProjectionInfo Projection
        {
            get;
            set;
        }

        bool Validate();
        /// <summary>
        /// Build featue set
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        void Build(string filename);
        void BuildCentroid(string filename);
        void BuildTopology();
        void Extent(SpatialReference srf);
        double GetTotalArea();
    
        ILArray<T> ToILMatrix<T>(ILArray<T> vector, T novalue);

        T[][] ToMatrix<T>(T[] vector, T novalue);
    }
}
