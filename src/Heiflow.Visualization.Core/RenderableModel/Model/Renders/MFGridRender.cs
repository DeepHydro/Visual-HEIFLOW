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

            // Where every vertex goes in the texture the steps are drawn from. The grid is laid out in
            // rows and columns and so is the texture, so a vertex keeps its neighbours around it and
            // the colour between two of them can be read from the texels between them.
            _DataTextureWidth = _MFGrid.ColumnCount + 1;
            _DataTextureHeight = _MFGrid.RowCount + 1;
            _SmoothStepTexture = true;
            _VertexTexel = new int[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                var row = (int)topo.VertexAtActiveCells[i, 4];
                var col = (int)topo.VertexAtActiveCells[i, 5];
                _VertexTexel[i] = row * _DataTextureWidth + col;
            }

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

        /// <summary>
        /// The most the classes of all the steps are allowed to take. One byte is kept for every
        /// vertex of every step, which over a long run on a fine grid comes to more memory than the
        /// machine has.
        /// </summary>
        private const long MaxCachedColorBytes = 512L * 1024 * 1024;

        /// <summary>
        /// Set when the cache of this grid was found to be over that, so the layer stops trying to fill
        /// it on every frame and the colours are made for the step on screen instead.
        /// </summary>
        private bool _CacheTooLarge;

        public override void UpdateVertexColor()
        {
            if (GridValues == null)
            {
                GridValues = GetGridValues(DataSource, CurrentTimeStep);
            }

            if (GridValues != null)
            {
                if (UseCache && !_CacheTooLarge)
                {
                    // The cache carries the statistics of every step as well, so the values of the
                    // step do not have to be scanned here to know the range it covers. Scanning them
                    // walked the whole grid on every frame for numbers that were already stored.
                    UpdateCachedColor();
                }
                else
                {
                    var vector = GridValues;
                    // the statistics deliver the extremes as well, they are not scanned a second time
                    StatisticsInfo = MyStatisticsMath.SimpleStatistics(vector);
                    MaxCellValue = (float)StatisticsInfo.Max;
                    MinCellValue = (float)StatisticsInfo.Min;
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
                        if (ContinuousColor)
                        {
                            // Without a cache there is no range that covers every step, so the one of
                            // this step is what the values are scaled over. The shades are fine all the
                            // same, only the scale under them still moves from step to step, which is
                            // what the cache is what stops.
                            var palette = LevelPalette;
                            var low = (float)MinCellValue;
                            var high = (float)(MaxCellValue > MinCellValue ? MaxCellValue : MinCellValue + 1);
                            var scale = 255f / (high - low);
                            for (int i = 0; i < vertexCount; i++)
                            {
                                VertexList[i].Color = palette[ShadeOf(_VertexValueBuffer[i] - low, scale)];
                            }
                        }
                        else
                        {
                            var levels = _DataColor.GetLevels(_VertexValueBuffer, ColourRampCount, ClassificationMethod);

                            for (int i = 0; i < vertexCount; i++)
                            {
                                VertexList[i].Color = _DataColor.GetVertexColor(levels[i], Opacity, InvertColor);
                            }
                        }
                    }
                    NotifyColorsChanged();
                }
            }
        }

        public override void CacheColor()
        {
            if (DataSource == null)
            {
                return;
            }
            var timesteps = DataSource.Size[1];
            var vertexCount = _MFGrid.Topology.ActiveVertexCount;

            // The class a vertex falls into is kept for every vertex of every step, one byte each, and
            // the colour comes from the table of classes rather than being stored, which is a quarter
            // of what an integer per vertex would have taken. Over a long run on a fine grid even that
            // is more than the machine has, and then nothing is cached and the colours are made for
            // the step that is being drawn. A byte holds no class beyond two hundred and fifty five,
            // so a ramp with more classes than that is drawn without the cache as well. A shade is
            // never above that either, so a ramp of any size can be spread over them.
            _CacheTooLarge = (long)timesteps * vertexCount > MaxCachedColorBytes
                || (!ContinuousColor && _DataColor.ColorRamp.Colors.Length > 256);
            if (_CacheTooLarge)
            {
                cachedLevel = null;
                return;
            }

            var prg = 0;
            var level = new byte[(int)((long)timesteps * vertexCount)];
            cachedMaxValues = new float[timesteps];
            cachedMinValues = new float[timesteps];
            cachedStatisticsInfo = new StatisticsInfo[timesteps];
            this.VarIndex = DataSource.SelectedVariableIndex;

            // A shade means the same value on every step only when all of them are scaled over one
            // range, and that range is not known before the last step has been read. While shades are
            // asked for the steps are therefore walked twice, once over the extremes to find the range
            // and once to scale the values over it, which is what doubles the cost of filling it.
            if (ContinuousColor)
            {
                _ScaleMin = float.MaxValue;
                _ScaleMax = float.MinValue;
                for (int t = 0; t < timesteps; t++)
                {
                    var vector = GetGridValues(DataSource, t);
                    if (vector == null)
                    {
                        return;
                    }
                    var min = vector.Min();
                    var max = vector.Max();
                    cachedMinValues[t] = min;
                    cachedMaxValues[t] = max;
                    if (min < _ScaleMin)
                    {
                        _ScaleMin = min;
                    }
                    if (max > _ScaleMax)
                    {
                        _ScaleMax = max;
                    }
                    prg = (t + 1) * 50 / timesteps;
                    OnCachingColorPrgChanged(prg);
                }
                if (_ScaleMax <= _ScaleMin)
                {
                    _ScaleMax = _ScaleMin + 1;
                }
            }

            for (int t = 0; t < timesteps; t++)
            {
                var vector = GetGridValues(DataSource, t);
                // The source can be set to a variable that has no array behind it, which is the case
                // UpdateVertexColor already handles by drawing nothing. Whether there is one depends
                // on the variable alone and not on the step, so if the first step has none no step
                // will. The cache is left as it is, whatever is on screen keeps its colours.
                if (vector == null)
                {
                    return;
                }
                if (!ContinuousColor)
                {
                    cachedMaxValues[t] = vector.Max();
                    cachedMinValues[t] = vector.Min();
                }
                // One array is enough for the values of every step, it is refilled rather than
                // allocated again for each of them.
                if (_VertexValueBuffer == null || _VertexValueBuffer.Length != vertexCount)
                {
                    _VertexValueBuffer = new float[vertexCount];
                }
                for (int i = 0; i < vertexCount; i++)
                {
                    _VertexValueBuffer[i] = _MFGrid.Topology.GetVertexValue(vector, i);
                }
                var step = t * vertexCount;
                if (ContinuousColor)
                {
                    var scale = 255f / (_ScaleMax - _ScaleMin);
                    for (int i = 0; i < vertexCount; i++)
                    {
                        level[step + i] = (byte)ShadeOf(_VertexValueBuffer[i] - _ScaleMin, scale);
                    }
                }
                else
                {
                    var levels = _DataColor.GetLevels(_VertexValueBuffer, ColourRampCount, ClassificationMethod);
                    for (int i = 0; i < vertexCount; i++)
                    {
                        // A value below the lowest break comes back as a class below the first one,
                        // and that one is coloured as the first class is.
                        var l = levels[i];
                        level[step + i] = (byte)(l < 0 ? 0 : l);
                    }
                }
                cachedStatisticsInfo[t] = MyStatisticsMath.SimpleStatistics(vector);
                prg = ContinuousColor ? 50 + (t + 1) * 50 / timesteps : (t + 1) * 100 / timesteps;
                OnCachingColorPrgChanged(prg);
            }
            MaxCellValue = cachedMaxValues[0];
            MinCellValue = cachedMinValues[0];
            StatisticsInfo = cachedStatisticsInfo[0];
            cachedLevel = level;
            // The cache is marked here and not only by the caller that draws a step, because the
            // Create Cache button fills it on its own and then hands the playback over. Without this
            // it was still stale as far as the next frame was concerned, so the step that was drawn
            // first filled the whole cache a second time.
            MarkCachedColorFresh();
            OnCachingColorFinished(new EventArgs());
        }

        public override void UpdateCachedColor()
        {
            // the cache was filled under the ramp, class count, method and opacity of that moment,
            // once any of them moved on it has to be recomputed or the layer keeps the old colours
            if (IsCachedColorStale)
            {
                CacheColor();
                // CacheColor leaves without one while the source has no array for the variable,
                // and calling it fresh now would stop it from being rebuilt once the array is there.
                if (cachedLevel != null)
                    MarkCachedColorFresh();
            }
            if (cachedLevel != null)
            {
                // The colours in the vertices are never read while the mesh is drawn from the step
                // textures, and writing them on every step would hand the card the whole array again,
                // which is the one thing the textures were brought in to stop.
                if (!ShadingActive)
                {
                    // What is cached is the class of every vertex and not its colour, so the class is
                    // turned into one through the table of the scheme that is current now.
                    var palette = LevelPalette;
                    var vertexCount = _MFGrid.Topology.ActiveVertexCount;
                    var step = CurrentTimeStep * vertexCount;
                    var last = palette.Length - 1;
                    for (int i = 0; i < vertexCount; i++)
                    {
                        // A cache that could not be filled again still holds the classes of a ramp that
                        // had more of them, and a class past the end of the table is drawn as the last
                        // one of it.
                        var l = cachedLevel[step + i];
                        VertexList[i].Color = palette[l < palette.Length ? l : last];
                    }
                }
                // What is drawn is scaled over the range of every step taken together while shades are
                // on, so that is the range the legend has to name, or it would hand out the numbers of
                // this step for colours that were spread over all of them.
                if (ContinuousColor)
                {
                    MaxCellValue = _ScaleMax;
                    MinCellValue = _ScaleMin;
                }
                else
                {
                    MaxCellValue = cachedMaxValues[CurrentTimeStep];
                    MinCellValue = cachedMinValues[CurrentTimeStep];
                }
                // The colours are in the vertex array now, which the device has to be told to take
                // again. UpdateVertexColor notifies for the paths that run through it, but the
                // animation asks for the cached colours straight away so that the values of the step
                // are not read a second time, and those frames need it just the same.
                if (!ShadingActive)
                    NotifyColorsChanged();
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