using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using HUST.WREIS.Dot3D;
using Heiflow.Core.Drawing;
using Heiflow.Models.Subsurface;
using Heiflow.Spatial;
using Heiflow.Models.Generic;
using Heiflow.Core.MyMath;
using Heiflow.Core.Data;
using GeoAPI.Geometries;


namespace Heiflow.Visualization.Renderable.Grid
{
    public class MFGridRender : ModelRenderDX
    {
        protected CellVertexIndices[] _CellVertexIndicesList;
        protected MFGrid _MFGrid;

        public MFGridRender(World world)
            : base(world)
        {
            Name = "MFGridRender";
          //  SupportedLayerObject = "Heiflow.Visualization.Renderable.Grid.RenderableModelLayer";
            SupportedLayerObject = "Heiflow.Models.GHM.RenderableModelLayer";
        }
        public struct CellVertexIndices
        {
            public int VertextI;
            public int VertextJ;
            /// <summary>
            /// the I index of the cell  to which this vertex belongs
            /// </summary>
            public int CellI;
            /// <summary>
            /// / the J index of the cell  to which this vertex belongs
            /// </summary>
            public int CellJ;
            /// <summary>
            /// use this index to retrieve lat and lon from the cell to which this vertex belongs
            /// </summary>
            public int LatLngIndex;
        }
        public Cell this[int i]
        {
            get
            {
                return _Cells[i];
            }
        }
        public MFVectorRender VelocityVectors
        {
            get;
            protected set;
        }

        public MFGrid MFGrid
        {
            get
            {
                return _MFGrid;
            }
        }

        public override void Initilize()
        {
            _Project = LayerObject.Project;
            _MFGrid = _Project.Model.Grid as MFGrid;
            Grid = _MFGrid;
            CreatMeshes();
            base.Initilize();
        }

        public override void CreatMeshes()
        {
            _Topology = _MFGrid.Topology;
            double layerRadius = EquatorialRadius + RenderableObject.DistanceAboveSurface * World.Settings.VerticalExaggeration;
            RegularGridTopology topo = this._MFGrid.Topology;
            int vertexCount = topo.ActiveVertexCount;
            var vertexList = new CustomVertex.PositionNormalColored[vertexCount];
            int[] vertexIndexList = new int[_MFGrid.ActiveCellCount * 6];
            double lat = 0, lng = 0;
            double radius = 0;

            var vertexUnifiedColor = DataColor.GetRandomColor(1);
            var upleft = new Coordinate(_MFGrid.BBox.MinX, _MFGrid.BBox.MaxY);
            var lowerRight = new Coordinate(_MFGrid.BBox.MaxX, _MFGrid.BBox.MinY);
            double deltRow = Math.Abs(upleft.Y - lowerRight.Y) / _MFGrid.RowCount;
            double deltCol = Math.Abs(upleft.X - lowerRight.X) / _MFGrid.ColumnCount;

            var vector = _MFGrid.Elevations.GetVector(ModelService.CurrentGridLayer, "0", ":");
            GridValues = vector;
            float maxValue = vector.Max();
            float minValue = vector.Min();
            float verValue = 0;
            GCSConverter.ToLatLon(_MFGrid.BBox.Centre.X, _MFGrid.BBox.Centre.Y);
            ComputeLocalOrigin(GCSConverter.Longitude, GCSConverter.Latitude);
            Grid.BBoxCentroid = new Coordinate(GCSConverter.Longitude, GCSConverter.Latitude);
          
            var vertext_vec = new float[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                vertext_vec[i] = topo.GetVertexValue(vector, i);
            }
            var levels = _DataColor.GetLevels(vertext_vec, ColourRampCount, ClassificationMethod);

            for (int i = 0; i < vertexCount; i++)
            {
                var row = topo.VertexAtActiveCells[i, 4];
                var col = topo.VertexAtActiveCells[i, 5];

                double xx = upleft.X + col * deltCol;
                double yy = upleft.Y - row * deltRow;

                GCSConverter.ToLatLon(xx, yy);
                lng = GCSConverter.Longitude;
                lat = GCSConverter.Latitude;

                //verValue = topo.GetVertexValue(vector, i);
                verValue = vertext_vec[i];
                radius = layerRadius + (verValue + this.DistanceAboveSurface) * World.Settings.VerticalExaggeration;
                Vector3 curNode = MathEngine.SphericalToCartesian(lat, lng, radius);
                vertexList[i] = new CustomVertex.PositionNormalColored()
                {
                    X = curNode.X,
                    Y = curNode.Y,
                    Z = curNode.Z,
                    Nx = (float)lng,
                    Ny = (float)lat,
                    Nz = verValue * World.Settings.VerticalExaggeration
                };
                //vertexList[i].Color = _DataColor.GetVertexColor(maxValue, minValue, verValue, Opacity);
                vertexList[i].Color = _DataColor.GetVertexColor(levels[i], Opacity);
            }

            GCSConverter.ToLatLon(_MFGrid.BBox.MinX, _MFGrid.BBox.MaxY);
            double west = GCSConverter.Longitude;
            double north = GCSConverter.Latitude;
            GCSConverter.ToLatLon(_MFGrid.BBox.MaxX, _MFGrid.BBox.MinY);
            double east = GCSConverter.Longitude;
            double sourth = GCSConverter.Latitude;

            BBox = new Envelope(west, east, sourth, north);
            _MFGrid.CentralPoint = new DataCube<float>(3, 0, _MFGrid.ActiveCellCount);

            for (int i = 0; i < _MFGrid.ActiveCellCount; i++)
            {
                int id1 = topo.CellVertex[i, 1];
                int id2 = topo.CellVertex[i, 2];
                int id3 = topo.CellVertex[i, 3];
                int id4 = topo.CellVertex[i, 4];
                _MFGrid.CentralPoint[0, 0, i] = (vertexList[id1].Nx + vertexList[id2].Nx) * 0.5f;
                _MFGrid.CentralPoint[1, 0, i] = (vertexList[id1].Ny + vertexList[id2].Ny) * 0.5f;
                _MFGrid.CentralPoint[2, 0, i] = (vertexList[id1].Nz + vertexList[id2].Nz + vertexList[id2].Nz + vertexList[id4].Nz) * 0.25f / World.Settings.VerticalExaggeration;

                vertexIndexList[i * 6] = id1;
                vertexIndexList[i * 6 + 1] = id2;
                vertexIndexList[i * 6 + 2] = id4;

                vertexIndexList[i * 6 + 3] = id4;
                vertexIndexList[i * 6 + 4] = id2;
                vertexIndexList[i * 6 + 5] = id3;
            }

            CalculateNormals(ref vertexList, vertexIndexList);

            _VertexList = vertexList;
            _VertexIndexList = vertexIndexList;
            MaxCellValue = maxValue;
            MinCellValue = minValue;

            StatisticsInfo = MyStatisticsMath.SimpleStatistics(vector);
            NotifyMeshChanged();
        }

        public override float GetCellValue(int row, int col)
        {
            var id = _Topology.GetID(row, col);
            if (_Topology.CellID2CellIndex.Keys.Contains(id))
            {
                var index = _Topology.CellID2CellIndex[id];
                if (GridValues != null)
                {
                    return GridValues[index];
                }
                else if (DataSource != null)
                {
                    var vec = GetGridValues(DataSource, CurrentTimeStep);

                    return vec[index];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        protected Vector3 CellNode(GCSConverter utm, double xx, double yy, double nodeEle, double layerRadius)
        {
            double lat = 0, lng = 0;
            CoordinateConvertor.ZoneWide = 6;
            double radius = 0;
            double rlng = 0;
            double rlat = 0;
            utm.ToLatLon(xx, yy);
            lng = utm.Longitude;
            lat = utm.Latitude;

            rlng = MathEngine.DegreesToRadians(lng);
            rlat = MathEngine.DegreesToRadians(lat);
            radius = layerRadius + nodeEle;
            Vector3 curNode = MathEngine.SphericalToCartesian(lat, lng, radius);
            return curNode;

        }

        public override ICell SelectCell(double lat, double lon)
        {
            MFCell mfcell = null;
            var pt = new Coordinate(lon, lat);
            if (BBox.Contains(lon, lat))
            {
                GCSConverter.ToUTM(lat, lon);
                //var inteval = _MFGrid.RowInteval[0, 0, 0];
                var inteval = _MFGrid.DELR[0, 0, 0];
                //var ind = _MFGrid.DeterminIndex(new Coordinate(utm.X, utm.Y), _MFGrid.BBox.TopLeft(), inteval, inteval);
                var ind = _MFGrid.DeterminIndex(new Coordinate(GCSConverter.X, GCSConverter.Y), new Coordinate(_MFGrid.BBox.MinX, _MFGrid.BBox.MaxY), inteval, inteval);
                mfcell = new MFCell(ind[0], ind[1]);
                mfcell.CalcCoordinate(this);
                mfcell.CurrentValue = GetCellValue(mfcell.I, mfcell.J);
                mfcell.SerialIndex = _Topology.GetSerialIndex(ind[0], ind[1]);
                var center = _MFGrid.LocateCentroid(ind[1], ind[0]);
                GCSConverter.ToLatLon(center.X, center.Y);
                mfcell.CentralLatitude = GCSConverter.Latitude;
                mfcell.CentralLongitude = GCSConverter.Longitude;
            }
            return mfcell;
        }

        public override void UpdateCellValues()
        {
            foreach (var cell in SelectedCells)
            {
                cell.CurrentValue = GetCellValue(cell.I, cell.J);
            }
        }


        #region Color Method
        /// <summary>
        /// Buffers of the vertex values and of the classes they fall into. They are refilled on every
        /// update instead of being allocated again, the arrays hold one entry per vertex of the grid.
        /// </summary>
        private float[] _VertexValueBuffer;

        public override void UpdateVertexColor()
        {
            if (GridValues == null)
            {
                GridValues = GetGridValues(DataSource, CurrentTimeStep);
            }

            if (GridValues != null)
            {
                var vector = GridValues;
                // the statistics deliver the extremes as well, they are not scanned a second time
                StatisticsInfo = MyStatisticsMath.SimpleStatistics(vector);
                MaxCellValue = (float)StatisticsInfo.Max;
                MinCellValue = (float)StatisticsInfo.Min;
                if (UseCache)
                {
                    UpdateCachedColor();
                }
                else
                {
                    if (UniqueColor)
                    {
                        var unqval = vector.Distinct().ToArray();
                        var dic = DataColor.ProduceColorDic<float>(unqval);

                        for (int i = 0; i < _MFGrid.Topology.ActiveVertexCount; i++)
                        {
                            var verValue = _MFGrid.Topology.GetUniqueVertexValue(vector, i);
                            System.Drawing.Color color;
                            if (dic.TryGetValue(verValue, out color))
                                _VertexList[i].Color = color.ToArgb();
                            else
                                _VertexList[i].Color = System.Drawing.Color.Transparent.ToArgb();
                        }
                    }
                    else
                    {
                        var vertexCount = _MFGrid.Topology.ActiveVertexCount;
                        if (_VertexValueBuffer == null || _VertexValueBuffer.Length != vertexCount)
                        {
                            _VertexValueBuffer = new float[vertexCount];
                        }
                        for (int i = 0; i < vertexCount; i++)
                        {
                            _VertexValueBuffer[i] = _MFGrid.Topology.GetVertexValue(vector, i);
                        }
                        var levels = _DataColor.GetLevels(_VertexValueBuffer, ColourRampCount, ClassificationMethod);

                        for (int i = 0; i < vertexCount; i++)
                        {
                           // var verValue = _MFGrid.Topology.GetVertexValue(vector, i);
                            VertexList[i].Color = _DataColor.GetVertexColor(levels[i], Opacity);
                        }
                    }
                }

                NotifyColorsChanged();
            }
        }

        public override void CacheColor()
        {
            var timesteps = DataSource.Size[1];
            var prg = 0;
            DataCube<int> color = new DataCube<int>(1, timesteps, _MFGrid.Topology.ActiveVertexCount);
            cachedMaxValues = new float[timesteps];
            cachedMinValues = new float[timesteps];
            cachedStatisticsInfo = new StatisticsInfo[timesteps];
            for (int t = 0; t < timesteps; t++)
            {
                var vector = GetGridValues(DataSource, t);
                cachedMaxValues[t] = vector.Max();
                cachedMinValues[t] = vector.Min();
                var vertext_vec = new float[_MFGrid.Topology.ActiveVertexCount];
                for (int i = 0; i < _MFGrid.Topology.ActiveVertexCount; i++)
                {
                    vertext_vec[i] = _MFGrid.Topology.GetVertexValue(vector, i);
                }
                var levels = _DataColor.GetLevels(vertext_vec, ColourRampCount, ClassificationMethod);
                for (int i = 0; i < _MFGrid.Topology.ActiveVertexCount; i++)
                {
                    //var verValue = _MFGrid.Topology.GetVertexValue(vector, i);
                    color[0, t, i] = _DataColor.GetVertexColor(levels[i], Opacity);
                }
                cachedStatisticsInfo[t] = MyStatisticsMath.SimpleStatistics(vector);
                prg = (t + 1) * 100 / timesteps;
            }
            MaxCellValue = cachedMaxValues[0];
            MinCellValue = cachedMinValues[0];
            StatisticsInfo = cachedStatisticsInfo[0];
            cachedColor = color;
        }

        public override void UpdateCachedColor()
        {
            if (cachedColor != null)
            {
                for (int i = 0; i < _MFGrid.Topology.ActiveVertexCount; i++)
                {
                    VertexList[i].Color = cachedColor[0, CurrentTimeStep, i];
                }
                MaxCellValue = cachedMaxValues[CurrentTimeStep];
                MinCellValue = cachedMinValues[CurrentTimeStep];
            }
        }

        #endregion

        public override float[] GetCellTimeSeries(int row, int col)
        {
            float[] vec = null;
            if (DataSource != null)
            {
                int index = _Topology.GetSerialIndex(row, col);

                if (DataSource is DataCube<float>)
                {
                    vec = new float[DataSource.Size[1]];
                    for (int t = 0; t < DataSource.Size[1]; t++)
                    {
                        vec[t] = DataSource[VarIndex, t, index];
                    }
                }
            }
            return vec;
        }
    }
}