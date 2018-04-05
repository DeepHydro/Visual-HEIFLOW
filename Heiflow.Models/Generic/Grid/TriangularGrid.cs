// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using GeoAPI.Geometries;
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic
{
    public class TriangularGrid : ITriangularGrid
    {
        private Coordinate[] _Centroids;
        private Coordinate[] _Vertex;
        private ITriGridTopology _Topology;
        public event EventHandler SizeChanged;
        public event GridUpdate Updated;

        public Coordinate Origin
        {
            get;
            set;
        }
        public Coordinate BBoxCentroid
        {
            get;
            set;
        }
        public ITriGridTopology Topology
        {
            get
            {
                return _Topology;
            }
            set
            {
                _Topology = value;
                _Topology.Grid = this;
            }
        }

        public int VertexCount
        {
            get;
            set;
        }

        public int CellCount
        {
            get;
            set;
        }

        public int LayerCount
        {
            get;
            private set;
        }

        public int ActualLayerCount
        {
            get;
            set;
        }

        public IBasicModel Owner
        {
            get;
            set;
        }

        public DotSpatial.Data.IFeatureSet FeatureSet
        {
            get;
            set;
        }

        public DotSpatial.Data.IFeatureSet CentroidFeature
        {
            get;
            set;
        }

        public IMapFeatureLayer FeatureLayer
        {
            get;
            set;
        }

        public IMapFeatureLayer CentroidFeatureLayer
        {
            get;
            set;
        }

        public Envelope BBox
        {
            get;
            set;
        }

        public DotSpatial.Projections.ProjectionInfo Projection
        {
            get;
            set;
        }
        public Coordinate[] Centroids
        {
            get
            {
                return _Centroids;
            }
            set
            {
                _Centroids = value;
            }
        }

        public Coordinate[] Vertex
        {
            get
            {
                return _Vertex;
            }
            set
            {
                _Vertex = value;
            }
        }

        /// <summary>
        ///  2dmat[ LayerCount,ActiveCellCount]
        /// </summary>
        public My3DMat<float> Elevations
        {
            get;
            set;
        }

        public bool Validate()
        {
            return true;
        }

        public void Build(string filename)
        {
         
        }

        public void BuildCentroid(string filename)
        {  

        }

        public void BuildTopology()
        {
          
        }

        public void Extent(SpatialReference srf)
        {
         
        }

        public void OnSizeChanged()
        {
            if (SizeChanged != null)
                SizeChanged(this, new EventArgs());
        }



        public ILNumerics.ILArray<T> ToILMatrix<T>(ILNumerics.ILArray<T> vector, T novalue)
        {
            return vector;
        }


        public T[][] ToMatrix<T>(T[] vector, T novalue)
        {
            T [][] mat=new T[1][];
            mat[0] = vector;
            return mat;
        }

        public void OnUpdated()
        {
            if (Updated != null)
                Updated(this);
        }





        public double GetTotalArea()
        {
            return 0;
        }
    }
}
