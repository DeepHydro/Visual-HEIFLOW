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
                Source.IBound = new DataCube<float>(this.LayerCount, RowCount, ColumnCount);
                Source.DELC = new DataCube<float>(1, 1,RowCount);
                Source.DELR = new DataCube<float>(1, 1,ColumnCount);
                Source.DELC.Flags[0,0] = TimeVarientFlag.Constant;
                Source.DELR.Flags[0,0] = TimeVarientFlag.Constant;
                Source.DELC.Constants[0,0] = this.XSize;
                Source.DELR.Constants[0,0] = this.YSize;
                Source.DELC.ILArrays[0]["0", ":"] = this.XSize;
                Source.DELR.ILArrays[0]["0", ":"] = this.YSize;
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
                                Source.IBound[l,r,c] = 1;
                            active++;
                            centroids.Add(cor);
                        }
                    }
                }
                Source.ActiveCellCount = active;
                Source.Elevations = new DataCube<float>(Source.LayerCount, 1, active);
                Source.Elevations.Variables[0] = "Top Elevation";

                for (int i = 0; i < active; i++)
                {
                    //var cell = DEM.ProjToCell(centroids[i]);
                    //if (cell != null && cell.Row > 0)
                    //    Source.Elevations.Value[0][0][i] = (float)DEM.Value[cell.Row, cell.Column];
                    //else
                    //    Source.Elevations.Value[0][0][i] = 0;
                    var pt=new Coordinate(centroids[i].X-0.5*XSize,centroids[i].Y-0.5*YSize);
                    var buf = ZonalStatastics.GetCellAverage(DEM, pt, XSize, AveragingMethod);
                    if (buf == -9999)
                        buf = 0;
                    Source.Elevations[0, 0, i] = buf;
                }

                for (int l = 1; l < Source.LayerCount; l++)
                {
                    Source.Elevations.Variables[l] = string.Format("Layer {0} Bottom Elevation", l);
                    for (int i = 0; i < active; i++)
                    {
                        Source.Elevations[l,0,i] = (float)(Source.Elevations[l - 1,0,i] - LayerGroups[l - 1].LayerHeight);
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
