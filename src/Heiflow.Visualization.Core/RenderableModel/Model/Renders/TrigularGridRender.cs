using GeoAPI.Geometries;
using Heiflow.Core.Data;
using Heiflow.Core.MyMath;
using Heiflow.Models.Generic;
using Heiflow.Models.GHM;
using Heiflow.Spatial.Geography;
using HUST.WREIS.Dot3D;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable.Grid
{
    public class TrigularGridRender : ModelRenderDX
    {
        private TriangularGrid _TriangularGrid;
        private float[] _elev;
        public TrigularGridRender(World world):base(world)
        {
            Name = "TrigularGridRender";
            SupportedLayerObject = "Heiflow.Visualization.Renderable.Grid.RegularGridLayer";
            MinusElevation = false;
        }
        public TrigularGridRender()
        {
            Name = "TrigularGridRender";
            SupportedLayerObject = "Heiflow.Visualization.Renderable.Grid.RegularGridLayer";
            MinusElevation = false;
        }

        public bool MinusElevation
        {
            get;
            set;
        }

        public override void Initilize()
        {
            _TriangularGrid = _Project.Model.Grid as TriangularGrid;
            CreatMeshes();
            base.Initilize();
        }

        public override void CreatMeshes()
        {
            Grid = _TriangularGrid;
            double radius = 0;
             _elev = _TriangularGrid.Elevations.GetVector(0, "0", ":");
            GridValues = _elev;
            float maxValue = _elev.Max();
            float minValue = _elev.Min();
            float verValue = 0;
            _VertexList = new CustomVertex.PositionNormalColored[_TriangularGrid.VertexCount];
            var levels = _DataColor.GetLevels(_elev, ColourRampCount, ClassificationMethod);

            for (int i = 0; i < _TriangularGrid.VertexCount; i++)
            {
                verValue = (float)_TriangularGrid.Vertex[i].Z;
                radius = this.EquatorialRadius + (verValue +RenderableObject.DistanceAboveSurface) * World.Settings.VerticalExaggeration ;
              Vector3 curNode = MathEngine.SphericalToCartesian(_TriangularGrid.Vertex[i].Y, _TriangularGrid.Vertex[i].X, radius);
                VertexList[i] = new CustomVertex.PositionNormalColored()
                {
                    X = curNode.X,
                    Y = curNode.Y,
                    Z = curNode.Z,
                    Nx = (float)_TriangularGrid.Vertex[i].Y,
                    Ny = (float)_TriangularGrid.Vertex[i].X,
                    Nz = verValue * World.Settings.VerticalExaggeration
                };
                _VertexList[i].Color = _DataColor.GetVertexColor(levels[i], Opacity);
            }

            var latList = from  v in _TriangularGrid.Vertex select v.Y;
            var lngList = from v in _TriangularGrid.Vertex select v.X;
            BBox = new Envelope(latList.Min(), latList.Max(), lngList.Min(), lngList.Max());

            _VertexIndexList = new int[_TriangularGrid.ActiveCellCount * 3];
            for (int i = 0; i < _TriangularGrid.ActiveCellCount; i++)
            {
                int id0 = (int)_TriangularGrid.Topology.VertexIndices[i][0];
                int id1 = (int)_TriangularGrid.Topology.VertexIndices[i][1];
                int id2 = (int)_TriangularGrid.Topology.VertexIndices[i][2];

                _VertexIndexList[i * 3] = id0;
                _VertexIndexList[i * 3 + 1] = id1;
                _VertexIndexList[i * 3 + 2] = id2;
            }
        }

        public override float GetCellValue(int p1, int p2)
        {
            throw new NotImplementedException();
        }

        public override ICell SelectCell(double lat, double lon)
        {
            Point3d point = new Point3d(lon,lat, 0);
            TriangularCell cell = null;
            Point3d[] poly = new Point3d[3];

            for (uint i = 0; i < _TriangularGrid.ActiveCellCount; i++)
            {
                uint id0 = _TriangularGrid.Topology.VertexIndices[i][0];
                uint id1 = _TriangularGrid.Topology.VertexIndices[i][1];
                uint id2 = _TriangularGrid.Topology.VertexIndices[i][2];
                poly[0] = new Point3d(_TriangularGrid.Vertex[id0].X, _TriangularGrid.Vertex[id0].Y, _TriangularGrid.Vertex[id0].Z);
                poly[1] = new Point3d(_TriangularGrid.Vertex[id1].X, _TriangularGrid.Vertex[id1].Y, _TriangularGrid.Vertex[id1].Z);
                poly[2] = new Point3d(_TriangularGrid.Vertex[id2].X, _TriangularGrid.Vertex[id2].Y, _TriangularGrid.Vertex[id2].Z);
                if (PointInPolygon(poly, point))
                {
                    cell = new TriangularCell((int)i);
                    cell.VertexID = new uint[] { id0, id1, id2 };
                    cell.Points = poly;
                    cell.Render = this;
                    break;
                }
            }
            return cell;
        }
        public override void CacheColor()
        {
            var timesteps = DataSource.Size[1];
            var prg = 0;
            DataCube<int> color = new DataCube<int>(1,timesteps, _TriangularGrid.VertexCount);
            cachedMaxValues = new float[timesteps];
            cachedMinValues = new float[timesteps];
            cachedStatisticsInfo = new StatisticsInfo[timesteps];

            for (int t = 0; t < timesteps; t++)
            {
                var vector = GetGridValues(DataSource, t);
                var levels = _DataColor.GetLevels(vector, ColourRampCount, ClassificationMethod);
                cachedMaxValues[t] = vector.Max();
                cachedMinValues[t] = vector.Min();
                for (int i = 0; i < _TriangularGrid.VertexCount; i++)
                {
                    var verValue = vector[i];
                    color[0, t, i] = _DataColor.GetVertexColor(levels[i], Opacity);
                }
                cachedStatisticsInfo[t] = MyStatisticsMath.SimpleStatistics(vector);
                prg = (t + 1) * 100 / timesteps;
                OnCachingColorPrgChanged(prg);
            }
            OnCachingColorFinished(new EventArgs()); 
            MaxCellValue = cachedMaxValues[0];
            MinCellValue = cachedMinValues[0];
            StatisticsInfo = cachedStatisticsInfo[0];
            cachedColor = color;
        }

        public override void UpdateVertexColor()
        {
            if (GridValues == null)
            {
                GridValues = GetGridValues(DataSource, CurrentTimeStep);
            }

            if (GridValues != null)
            {
                if (UseCache)
                {
                    UpdateCachedColor();
                }
                else
                {
                    var vector = GridValues;
                    MaxCellValue = vector.Max();
                    MinCellValue = vector.Min();
                    StatisticsInfo = MyStatisticsMath.SimpleStatistics(vector);
                    if (UniqueColor)
                    {
                        var unqval = vector.Distinct().ToArray();
                        var dic = DataColor.ProduceColorDic<float>(unqval);
                        for (int i = 0; i < _TriangularGrid.VertexCount; i++)
                        {
                            VertexList[i].Color = dic[vector[i]].ToArgb();
                        }
                    }
                    else
                    {
                        MaxCellValue = vector.Max();
                        MinCellValue = vector.Min();
                        var levels = _DataColor.GetLevels(vector, ColourRampCount, ClassificationMethod);
                        for (int i = 0; i < _TriangularGrid.VertexCount; i++)
                        {
                            VertexList[i].Color = _DataColor.GetVertexColor(levels[i], Opacity, InvertColor);
                        }
                    }
                }
            }
        }

        public override void UpdateCachedColor()
        {
            if (cachedColor != null)
            {
                for (int i = 0; i < _TriangularGrid.VertexCount; i++)
                {
                    VertexList[i].Color = cachedColor[0,CurrentTimeStep,i];
                }
                MaxCellValue = cachedMaxValues[CurrentTimeStep];
                MinCellValue = cachedMinValues[CurrentTimeStep];
            }
        }

        public override void UpdateCellValues()
        {
            
        }

        public bool PointInPolygon(Point3d[] _vertices, Point3d point)
        {
            var j = _vertices.Length - 1;
            var oddNodes = false;

            for (var i = 0; i < _vertices.Length; i++)
            {
                if (_vertices[i].Y < point.Y && _vertices[j].Y >= point.Y ||
                    _vertices[j].Y < point.Y && _vertices[i].Y >= point.Y)
                {
                    if (_vertices[i].X +
                        (point.Y - _vertices[i].Y) / (_vertices[j].Y - _vertices[i].Y) * (_vertices[j].X - _vertices[i].X) < point.X)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            return oddNodes;
        }

        public override Cell[] RetrieveCellsInColumn(int ColumnIndex)
        {
            throw new NotImplementedException();
        }

        public override Cell[] RetrieveCellsInRow(int RowIndex)
        {
            throw new NotImplementedException();
        }

        public override float[] GetCellTimeSeries(int p1, int p2)
        {
            throw new NotImplementedException();
        }
    }
}
