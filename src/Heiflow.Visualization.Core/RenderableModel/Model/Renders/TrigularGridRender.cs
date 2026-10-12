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

            // This grid has no shape to lay the texture out in, the vertices are only numbered, so the
            // texture is a strip as wide as a texture may be and as high as it takes to hold them all,
            // and a vertex is the texel of its own number.
            var vertexCount = _TriangularGrid.VertexCount;
            _DataTextureWidth = Math.Min(vertexCount, MaxStepTextureWidth);
            _DataTextureHeight = (vertexCount + _DataTextureWidth - 1) / _DataTextureWidth;
            _SmoothStepTexture = false;
            _VertexTexel = new int[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                _VertexTexel[i] = i;
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

        public override void CacheColor()
        {
            if (DataSource == null)
            {
                return;
            }
            var timesteps = DataSource.Size[1];
            var vertexCount = _TriangularGrid.VertexCount;

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
                // No array behind the variable the source is set to, which UpdateVertexColor already
                // draws nothing for. It is the variable that decides and not the step, so there is no
                // cache to fill here and the colours that are on screen are kept.
                if (vector == null)
                {
                    return;
                }
                if (!ContinuousColor)
                {
                    cachedMaxValues[t] = vector.Max();
                    cachedMinValues[t] = vector.Min();
                }
                var step = t * vertexCount;
                if (ContinuousColor)
                {
                    var scale = 255f / (_ScaleMax - _ScaleMin);
                    for (int i = 0; i < vertexCount; i++)
                    {
                        level[step + i] = (byte)ShadeOf(vector[i] - _ScaleMin, scale);
                    }
                }
                else
                {
                    var levels = _DataColor.GetLevels(vector, ColourRampCount, ClassificationMethod);
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
            CachedLevelStride = vertexCount;
            // Marked here as well as by the caller that draws a step, the Create Cache button filling
            // it on its own and then handing the playback over.
            MarkCachedColorFresh();
            OnCachingColorFinished(new EventArgs());
        }

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
                    else if (ContinuousColor)
                    {
                        // Without a cache there is no range that covers every step, so the one of this
                        // step is what the values are scaled over. The shades are fine all the same,
                        // only the scale under them still moves from step to step, which is what the
                        // cache is what stops.
                        var palette = LevelPalette;
                        var low = (float)MinCellValue;
                        var high = (float)(MaxCellValue > MinCellValue ? MaxCellValue : MinCellValue + 1);
                        var scale = 255f / (high - low);
                        for (int i = 0; i < _TriangularGrid.VertexCount; i++)
                        {
                            VertexList[i].Color = palette[ShadeOf(vector[i] - low, scale)];
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

                NotifyColorsChanged();
            }
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
                    var vertexCount = _TriangularGrid.VertexCount;
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
                if (!ShadingActive)
                    NotifyColorsChanged();
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
