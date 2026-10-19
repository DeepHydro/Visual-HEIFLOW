using GeoAPI.Geometries;
using Heiflow.Core.Data;
using Heiflow.Core.MyMath;
using Heiflow.Models.Generic;
using Heiflow.Models.GHM;
using Heiflow.Spatial.Geography;
using HUST.WREIS.Dot3D;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

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

        /// <summary>
        /// The values the mesh is coloured with, one for every triangle of every step of the file.
        /// The name of that file is handed over by the importer, see ModelLayerImporter, and without
        /// reading it here the render has no source at all: the cache stayed empty however often it
        /// was asked for, and the mesh kept the colours of the heights while the animation played.
        /// Every step of the file is taken, which is what the cache and the playback need, and not
        /// only the one step a vector layer draws, see TriganularVectorRender.
        /// </summary>
        public override void LoadDataSource(string filename)
        {
            var ghm = Project != null ? Project.Model as GHModel : null;
            if (ghm == null || ghm.Project == null || string.IsNullOrEmpty(filename) || !File.Exists(filename))
            {
                return;
            }

            var provider = ghm.Project.DataCubeFileFactory.Select(filename);
            if (provider != null)
            {
                DataSource = provider.Provide(filename);
            }
        }

        public override void Initilize()
        {
            // The ramp of colours and the opacity the mesh is coloured with are the ones the base
            // class sets, so it is called before the mesh is built and not after it, or the first
            // colours would be made with a ramp of no classes and an opacity of nothing at all.
            base.Initilize();
            _TriangularGrid = _Project.Model.Grid as TriangularGrid;
            CreatMeshes();
        }

        /// <summary>
        /// How many triangles the grid holds. A variable of this grid is kept for its vertices and
        /// not for its triangles, but a source that keeps one for every triangle is still spread over
        /// the vertices round it, so the count of them is kept as well, see NodeValues.
        /// </summary>
        private int _CellCount;

        /// <summary>
        /// How many vertices the mesh is drawn with, which is every vertex of the grid once and not
        /// three for every triangle. A value is kept for a vertex and not for a triangle, and it is
        /// the vertices that are coloured: the card spreads the colour of the three of a triangle
        /// over the face between them, which is how a field of values is drawn on this kind of grid.
        /// </summary>
        private int _NodeCount;

        /// <summary>
        /// The texel of a step is read at the vertex and not at the pixel. The vertices of this grid
        /// are only numbered, so a texture coordinate found between two corners of a triangle runs
        /// over thousands of texels that have nothing to do with it, and a face would come out filled
        /// with stripes of colours belonging all over the grid. The shader for such a grid reads the
        /// texel of the vertex itself, see ModelColorNodes.fx, and hands the pixel shader the colour
        /// of the ramp, which is spread over the triangle the way the fixed pipeline spread the
        /// colour of a vertex. A card that cannot run it stays on the vertices, see
        /// ModelColorShader.SupportsShaders.
        /// </summary>
        protected override bool StepTexturePerVertex
        {
            get { return true; }
        }

        public override void CreatMeshes()
        {
            Grid = _TriangularGrid;
            double radius = 0;
            _elev = _TriangularGrid.Elevations.GetVector(0, "0", ":");

            var vertexCount = _TriangularGrid.VertexCount;
            _NodeCount = vertexCount;

            var topology = _TriangularGrid.Topology;
            _CellCount = topology != null && topology.VertexIndices != null
                ? Math.Min(_TriangularGrid.ActiveCellCount, topology.VertexIndices.Length)
                : 0;

            // The heights the mesh is coloured by before the values of a variable are read. They are
            // kept for the vertices, and where that array does not cover the grid the height of the
            // vertex itself is taken, which is what the mesh file holds for it.
            var elev = _elev != null && _elev.Length >= vertexCount
                ? _elev
                : (from v in _TriangularGrid.Vertex select (float)v.Z).ToArray();
            GridValues = elev;
            float maxValue = elev.Length > 0 ? elev.Max() : 0;
            float minValue = elev.Length > 0 ? elev.Min() : 0;

            _VertexList = new CustomVertex.PositionNormalColored[vertexCount];
            var levels = _DataColor.GetLevels(elev, ColourRampCount, ClassificationMethod);

            for (int i = 0; i < vertexCount; i++)
            {
                var verValue = (float)_TriangularGrid.Vertex[i].Z;
                radius = this.EquatorialRadius + (verValue + RenderableObject.DistanceAboveSurface) * World.Settings.VerticalExaggeration;
                Vector3 curNode = MathEngine.SphericalToCartesian(_TriangularGrid.Vertex[i].Y, _TriangularGrid.Vertex[i].X, radius);
                VertexList[i] = new CustomVertex.PositionNormalColored()
                {
                    X = curNode.X,
                    Y = curNode.Y,
                    Z = curNode.Z
                };
                _VertexList[i].Color = _DataColor.GetVertexColor(levels[i], Opacity);
            }

            var latList = from v in _TriangularGrid.Vertex select v.Y;
            var lngList = from v in _TriangularGrid.Vertex select v.X;

            // An envelope is asked for as west, east, south and north, longitude first and latitude
            // second. The vertices of the grid hold them the other way about, and an envelope built
            // that way round holds nothing, so the layer would never be flown to.
            BBox = new Envelope(lngList.Min(), lngList.Max(), latList.Min(), latList.Max());

            _VertexIndexList = new int[_CellCount * 3];
            _LineIndexList = new int[_CellCount * 6];
            var line = 0;
            for (int i = 0; i < _CellCount; i++)
            {
                int id0 = (int)topology.VertexIndices[i][0];
                int id1 = (int)topology.VertexIndices[i][1];
                int id2 = (int)topology.VertexIndices[i][2];

                // One triangle that names a vertex the grid does not have takes the whole draw call
                // with it, the card throws away everything it was handed rather than reading past
                // the end of the buffer, and not one triangle of the grid would come up. Such a one
                // is made into a triangle of no area instead, which is drawn as nothing at all.
                if (id0 < 0 || id0 >= vertexCount || id1 < 0 || id1 >= vertexCount
                    || id2 < 0 || id2 >= vertexCount)
                {
                    _VertexIndexList[i * 3] = 0;
                    _VertexIndexList[i * 3 + 1] = 0;
                    _VertexIndexList[i * 3 + 2] = 0;
                    continue;
                }

                _VertexIndexList[i * 3] = id0;
                _VertexIndexList[i * 3 + 1] = id1;
                _VertexIndexList[i * 3 + 2] = id2;

                // The three edges of the triangle, which is what a wire frame is made of. An edge
                // between two triangles is drawn twice, once by each of them, which is nothing next
                // to the lines a wire frame used to be drawn with.
                _LineIndexList[line++] = id0;
                _LineIndexList[line++] = id1;
                _LineIndexList[line++] = id1;
                _LineIndexList[line++] = id2;
                _LineIndexList[line++] = id2;
                _LineIndexList[line++] = id0;
            }

            // A triangle that had to be left out leaves its edge behind as two lines of no length,
            // and a buffer shorter than the one the card was told about draws nothing at all, so the
            // rest of it is cut off.
            if (line != _LineIndexList.Length)
            {
                var edges = new int[line];
                Array.Copy(_LineIndexList, edges, line);
                _LineIndexList = edges;
            }

            // The normals come off the triangles, and the vertices of one are shared with the ones
            // next to it, so what a vertex is given is the mean of the triangles round it. That is
            // what draws a field that runs smoothly over the grid, which is what a value kept for a
            // vertex asks for.
            CalculateNormals(ref _VertexList, _VertexIndexList);

            // This grid has no shape to lay the texture out in, the vertices are only numbered, so
            // the texture is a strip as wide as a texture may be and as high as it takes to hold
            // them all, and a vertex is the texel of its own number.
            _DataTextureWidth = Math.Min(vertexCount, MaxStepTextureWidth);
            _DataTextureHeight = _DataTextureWidth > 0
                ? (vertexCount + _DataTextureWidth - 1) / _DataTextureWidth
                : 0;
            _SmoothStepTexture = false;
            _VertexTexel = new int[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                _VertexTexel[i] = i;
            }

            MaxCellValue = maxValue;
            MinCellValue = minValue;

            // The geometry is new, so the layer has to hand it to the card again, see
            // ModelRenderDX.MeshVersion.
            NotifyMeshChanged();
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

        /// <summary>
        /// The values of one step as one for every vertex of the grid, which is what the mesh is
        /// coloured by. A variable of this kind of grid is kept for the vertices and not for the
        /// triangles of it, so an array that holds one for every vertex is taken as it comes.
        ///
        /// A source that keeps one for every triangle instead is spread over the vertices round it,
        /// the way the vertices of a regular grid are given the mean of the cells that touch them,
        /// see MFGridRender.
        ///
        /// Nothing comes back from an array that is neither the one nor the other, a source that was
        /// read for another grid, and there is then nothing to colour the mesh from.
        /// </summary>
        private float[] NodeValues(float[] vector)
        {
            if (vector == null || _TriangularGrid == null || _NodeCount <= 0)
            {
                return null;
            }

            var vertexCount = _TriangularGrid.VertexCount;
            if (vector.Length == vertexCount)
            {
                return vector;
            }

            var topology = _TriangularGrid.Topology;
            if (topology == null || topology.NodeConnectedCells == null || vector.Length < _CellCount)
            {
                // Neither one for every vertex nor one for every triangle, a source that was read
                // for another grid. There is nothing here to colour the mesh from.
                return null;
            }

            var values = new float[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                var cells = i < topology.NodeConnectedCells.Length ? topology.NodeConnectedCells[i] : null;
                if (cells == null || cells.Length == 0)
                {
                    continue;
                }

                var sum = 0.0;
                for (int c = 0; c < cells.Length; c++)
                {
                    var cell = (int)cells[c];
                    if (cell >= 0 && cell < vector.Length)
                    {
                        sum += vector[cell];
                    }
                }
                values[i] = (float)(sum / cells.Length);
            }
            return values;
        }

        public override void CacheColor()
        {
            if (DataSource == null)
            {
                return;
            }
            var timesteps = DataSource.Size[1];
            var vertexCount = _NodeCount;
            if (vertexCount <= 0)
            {
                return;
            }

            // The variable the cache is filled for is the one the source is set to, which is the one
            // the animation draws, and not whichever one the render happens to hold, see MFGridRender.
            this.VarIndex = DataSource.SelectedVariableIndex;

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
                    var vector = NodeValues(GetGridValues(DataSource, t));
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
                var vector = NodeValues(GetGridValues(DataSource, t));
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
                GridValues = NodeValues(GetGridValues(DataSource, CurrentTimeStep));
            }

            // One value for every vertex, whichever way the source keeps them, see NodeValues. The
            // cache is filled out of the same numbers, so the two agree.
            var vector = NodeValues(GridValues);

            if (vector != null)
            {
                if (UseCache && !_CacheTooLarge)
                {
                    UpdateCachedColor();
                }
                else
                {
                    MaxCellValue = vector.Max();
                    MinCellValue = vector.Min();
                    StatisticsInfo = MyStatisticsMath.SimpleStatistics(vector);
                    if (UniqueColor)
                    {
                        var unqval = vector.Distinct().ToArray();
                        var dic = DataColor.ProduceColorDic<float>(unqval);
                        for (int i = 0; i < _NodeCount; i++)
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
                        for (int i = 0; i < _NodeCount; i++)
                        {
                            VertexList[i].Color = palette[ShadeOf(vector[i] - low, scale)];
                        }
                    }
                    else
                    {
                        MaxCellValue = vector.Max();
                        MinCellValue = vector.Min();
                        var levels = _DataColor.GetLevels(vector, ColourRampCount, ClassificationMethod);
                        for (int i = 0; i < _NodeCount; i++)
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
                    var vertexCount = _NodeCount;
                    var step = CurrentTimeStep * vertexCount;
                    var last = palette.Length - 1;

                    // A cache that was filled before the mesh was rebuilt holds fewer classes than
                    // there are vertices now, and one that runs out is left with the colours that
                    // are on the mesh rather than walking past the end of it.
                    if (step + vertexCount > cachedLevel.Length)
                    {
                        vertexCount = Math.Max(0, cachedLevel.Length - step);
                    }

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
