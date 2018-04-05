// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using GeoAPI.Geometries;
using Heiflow.Core.Data;
using Heiflow.Core.MyMath;
using Heiflow.Models.Subsurface;
using Heiflow.Spatial.SpatialAnalyst;
using Heiflow.Spatial.SpatialRelation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic.Grid
{
    public class RegularGridGenerator : IGridGenerator
    {
        public enum GenMethod { ByCellSize, ByCellNumber };
        public RegularGridGenerator()
        {
            RowCount = 50;
            ColumnCount = 50;
            XSize = 100;
            YSize = 100;
            Method = GenMethod.ByCellNumber;
            AveragingMethod = Core.MyMath.AveragingMethod.Median;
        }

        public IFeatureSet Domain
        {
            get;
            set;
        }

        public IRaster DEM
        {
            get;
            set;
        }

        public int RowCount
        {
            get;
            set;
        }

        public int ColumnCount
        {
            get;
            set;
        }

        public int LayerCount
        {
            get;
            set;
        }

        public float XSize
        {
            get;
            set;
        }
        public float YSize
        {
            get;
            set;
        }

        public string Error
        {
            get;
            private set;
        }

        public GenMethod Method
        {
            get;
            set;
        }

        public Coordinate Origin
        {
            get;
            set;
        }

        public RegularGrid Source
        {
            get;
            set;
        }

        public ObservableCollection<LayerGroup> LayerGroups
        {
            get;
            set;
        }

        public AveragingMethod AveragingMethod
        {
            get;
            set;
        }
        public IGrid Generate()
        {
            if (Source == null)
                Source = new RegularGrid();
            if (Domain != null)
            {
                Source.Origin = this.Origin;
                Source.ActualLayerCount = this.LayerCount;
                Source.RowCount = RowCount;
                Source.ColumnCount = ColumnCount;
                Source.IBound = new MyVarient3DMat<float>(this.LayerCount, RowCount, ColumnCount);
                Source.DELC = new MyVarient3DMat<float>(1, 1);
                Source.DELR = new MyVarient3DMat<float>(1, 1);
                Source.DELC.Flags[0,0] = TimeVarientFlag.Constant;
                Source.DELR.Flags[0,0] = TimeVarientFlag.Constant;
                Source.DELC.Constants[0,0] = this.XSize;
                Source.DELR.Constants[0,0] = this.YSize;
                Source.Projection = Domain.Projection;
                Source.BBox = new Envelope(Domain.Extent.MinX, Domain.Extent.MaxX, Domain.Extent.MinY, Domain.Extent.MaxY);

                int active = 0;
                var geo = Domain.Features[0].Geometry.Coordinates;
                List<Coordinate> centroids = new List<Coordinate>();
                for (int r = 0; r < RowCount; r++)
                {
                    for (int c = 0; c < ColumnCount; c++)
                    {
                        var cor = Source.LocateCentroid(c + 1, r + 1);
                        
                        if (SpatialRelationship.PointInPolygon(geo, cor))
                        {
                            for (int l = 0; l < Source.ActualLayerCount; l++)
                                Source.IBound.Value[l][r][c] = 1;
                            active++;
                            centroids.Add(cor);
                        }
                    }
                }
                Source.ActiveCellCount = active;
                Source.Elevations = new MyVarient3DMat<float>(Source.LayerCount, 1, active);
                Source.Elevations.Variables[0] = "Top Elevation";

                for (int i = 0; i < active; i++)
                {
                    //var cell = DEM.ProjToCell(centroids[i]);
                    //if (cell != null && cell.Row > 0)
                    //    Source.Elevations.Value[0][0][i] = (float)DEM.Value[cell.Row, cell.Column];
                    //else
                    //    Source.Elevations.Value[0][0][i] = 0;
                    var pt=new Coordinate(centroids[i].X-0.5*XSize,centroids[i].Y-0.5*YSize);
                    Source.Elevations.Value[0][0][i] = ZonalStatastics.GetCellAverage(DEM, pt, XSize, AveragingMethod);
                }

                for (int l = 1; l < Source.LayerCount; l++)
                {
                    Source.Elevations.Variables[l] = string.Format("Layer {0} Bottom Elevation", l);
                    for (int i = 0; i < active; i++)
                    {
                        Source.Elevations.Value[l][0][i] = (float)(Source.Elevations.Value[l - 1][0][i] - LayerGroups[l - 1].LayerHeight);
                    }
                }
                Source.BuildTopology();
                Source.Elevations.Topology = Source.Topology;
            }
            else
            {
                Error = "The domain featureset is null";
            }
            return Source;
        }
    }
}
