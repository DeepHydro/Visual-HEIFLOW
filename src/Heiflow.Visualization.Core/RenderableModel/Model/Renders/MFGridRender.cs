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

        /// <summary>
        /// Longitude and latitude of every vertex. They follow from the row and the column alone, so they
        /// are worked out once and kept: a new layer or a new vertical exaggeration moves every vertex
        /// along the radius and nowhere else, and none of them has to be projected again for that.
        /// </summary>
        private double[] _VertexLat;
        private double[] _VertexLng;

        /// <summary>
        /// Where every vertex sits in the plane, as the distance from the origin along the rows and along
        /// the columns. MODFLOW hands out a width for every row and for every column, so the two are
        /// summed up rather than being taken as one width for all of them.
        /// </summary>
        private double[] _RowOffset;
        private double[] _ColOffset;

        /// <summary>
        /// The index of the vertex at a row and a column, -1 where there is none. It is built here rather
        /// than read from the topology, whose ids are worked out as row * columns + column + 1 and so give
        /// the last column of a row and the first column of the next one the same id.
        /// </summary>
        private int[,] _VertexIndexAt;

        /// <summary>
        /// The four corners of every cell, in the order north west, north east, south east, south west.
        /// </summary>
        private int[] _CellCorners;

        /// <summary>
        /// The serial index of the cell at a row and a column, -1 where the cell is not active. The
        /// topology looks one up by walking the keys of a dictionary, which is a scan of every active
        /// cell for every lookup, and picking a cell walks it.
        /// </summary>
        private int[,] _SerialIndexAt;

        /// <summary>
        /// Indices of the top faces alone. They are what the normals are worked out from, so that a
        /// vertex of the top is not given the normal of a wall hanging off it.
        /// </summary>
        private int[] _TopIndexList;

        private MFGrid _PlanGrid;
        private MFGrid _IndexGrid;
        private bool _IndexSkirt;
        private int _IndexCellCount;

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

        /// <summary>
        /// Draw the sides of the layer as well as its top, from the top of it down to the top of the one
        /// below, so that a layer seen from the side is a body rather than a sheet. It doubles the
        /// vertices of the mesh, so it is off until it is asked for.
        /// </summary>
        public bool LayerSkirt
        {
            get
            {
                return _LayerSkirt;
            }
            set
            {
                if (_LayerSkirt != value)
                {
                    _LayerSkirt = value;
                    CreatMeshes();
                }
            }
        }
        private bool _LayerSkirt = false;

        public override void CreatMeshes()
        {
            if (_MFGrid == null)
                return;

            _Topology = _MFGrid.Topology;
            RegularGridTopology topo = _Topology;
            if (topo == null || _MFGrid.Elevations == null)
                return;

            int vertexCount = topo.ActiveVertexCount;
            int cellCount = _MFGrid.ActiveCellCount;
            if (vertexCount <= 0 || cellCount <= 0)
                return;

            var vector = _MFGrid.Elevations.GetVector(ModelService.CurrentGridLayer, "0", ":");
            GridValues = vector;

            // The extremes come out of the statistics rather than out of a walk over the values of
            // their own, which is one pass over the grid instead of three, and they are wanted below.
            StatisticsInfo = MyStatisticsMath.SimpleStatistics(vector);
            MaxCellValue = (float)StatisticsInfo.Max;
            MinCellValue = (float)StatisticsInfo.Min;

            GCSConverter.ToLatLon(_MFGrid.BBox.Centre.X, _MFGrid.BBox.Centre.Y);
            ComputeLocalOrigin(GCSConverter.Longitude, GCSConverter.Latitude);
            Grid.BBoxCentroid = new Coordinate(GCSConverter.Longitude, GCSConverter.Latitude);

            // Where every vertex sits in the plane, and the corners and the indices that follow from
            // that. None of them moves when the layer or the exaggeration does, so all of them are
            // worked out once and are only read from then on.
            EnsurePlan(topo, vertexCount);
            EnsureIndices(topo, cellCount);

            double layerRadius = EquatorialRadius + RenderableObject.DistanceAboveSurface * World.Settings.VerticalExaggeration;
            float exaggeration = World.Settings.VerticalExaggeration;
            int total = _LayerSkirt ? vertexCount * 2 : vertexCount;
            var vertexList = new CustomVertex.PositionNormalColored[total];

            if (_VertexValueBuffer == null || _VertexValueBuffer.Length != vertexCount)
            {
                _VertexValueBuffer = new float[vertexCount];
            }
            for (int i = 0; i < vertexCount; i++)
            {
                _VertexValueBuffer[i] = topo.GetVertexValue(vector, i);
            }
            var levels = _DataColor.GetLevels(_VertexValueBuffer, ColourRampCount, ClassificationMethod);

            for (int i = 0; i < vertexCount; i++)
            {
                float verValue = _VertexValueBuffer[i];
                double radius = layerRadius + (verValue + this.DistanceAboveSurface) * exaggeration;
                Vector3 curNode = MathEngine.SphericalToCartesian(_VertexLat[i], _VertexLng[i], radius);
                vertexList[i].X = curNode.X;
                vertexList[i].Y = curNode.Y;
                vertexList[i].Z = curNode.Z;
                vertexList[i].Nx = (float)_VertexLng[i];
                vertexList[i].Ny = (float)_VertexLat[i];
                vertexList[i].Nz = verValue * exaggeration;
                vertexList[i].Color = _DataColor.GetVertexColor(levels[i], Opacity);
            }

            if (_LayerSkirt)
            {
                // The foot of every vertex, at the top of the layer below. It is a copy of the top
                // vertices taken down to their own height, and it takes its colour from the vertex
                // above it, so the walls are of the layer they hold up.
                var bottom = BottomVertexValues(topo, vertexCount, vector,
                    (float)StatisticsInfo.Min, (float)StatisticsInfo.Max);
                for (int i = 0; i < vertexCount; i++)
                {
                    int j = vertexCount + i;
                    float verValue = bottom[i];
                    double radius = layerRadius + (verValue + this.DistanceAboveSurface) * exaggeration;
                    Vector3 curNode = MathEngine.SphericalToCartesian(_VertexLat[i], _VertexLng[i], radius);
                    vertexList[j].X = curNode.X;
                    vertexList[j].Y = curNode.Y;
                    vertexList[j].Z = curNode.Z;
                    vertexList[j].Nx = (float)_VertexLng[i];
                    vertexList[j].Ny = (float)_VertexLat[i];
                    vertexList[j].Nz = verValue * exaggeration;
                    vertexList[j].Color = vertexList[i].Color;
                }
            }

            GCSConverter.ToLatLon(_MFGrid.BBox.MinX, _MFGrid.BBox.MaxY);
            double west = GCSConverter.Longitude;
            double north = GCSConverter.Latitude;
            GCSConverter.ToLatLon(_MFGrid.BBox.MaxX, _MFGrid.BBox.MinY);
            double east = GCSConverter.Longitude;
            double sourth = GCSConverter.Latitude;

            BBox = new Envelope(west, east, sourth, north);
            _MFGrid.CentralPoint = new DataCube<float>(3, 0, cellCount);

            for (int i = 0; i < cellCount; i++)
            {
                int id1 = _CellCorners[i * 4];
                int id2 = _CellCorners[i * 4 + 1];
                int id3 = _CellCorners[i * 4 + 2];
                int id4 = _CellCorners[i * 4 + 3];
                _MFGrid.CentralPoint[0, 0, i] = (vertexList[id1].Nx + vertexList[id2].Nx) * 0.5f;
                _MFGrid.CentralPoint[1, 0, i] = (vertexList[id1].Ny + vertexList[id2].Ny) * 0.5f;
                // Each of the four corners once. The height used to be taken over the third of them
                // twice and over the fourth one not at all.
                _MFGrid.CentralPoint[2, 0, i] = (vertexList[id1].Nz + vertexList[id2].Nz
                    + vertexList[id3].Nz + vertexList[id4].Nz) * 0.25f / exaggeration;
            }

            // The normals come off the top faces alone, or a vertex of the top would be given the
            // normal of a wall hanging off it and the layer would be lit as though it were folded.
            CalculateNormals(ref vertexList, _TopIndexList);

            if (_LayerSkirt)
            {
                // A foot is lit the way the vertex above it is, or a wall would come out of the
                // normals as black, which is what a normal of no length at all is drawn as.
                for (int i = 0; i < vertexCount; i++)
                {
                    int j = vertexCount + i;
                    vertexList[j].Nx = vertexList[i].Nx;
                    vertexList[j].Ny = vertexList[i].Ny;
                    vertexList[j].Nz = vertexList[i].Nz;
                }
            }

            _VertexList = vertexList;

            // Where every vertex goes in the texture the steps are drawn from. The grid is laid out in
            // rows and columns and so is the texture, so a vertex keeps its neighbours around it and
            // the colour between two of them can be read from the texels between them.
            _DataTextureWidth = _MFGrid.ColumnCount + 1;
            _DataTextureHeight = _MFGrid.RowCount + 1;
            _SmoothStepTexture = true;
            _VertexTexel = new int[total];
            for (int i = 0; i < vertexCount; i++)
            {
                var row = (int)topo.VertexAtActiveCells[i, 4];
                var col = (int)topo.VertexAtActiveCells[i, 5];
                var texel = row * _DataTextureWidth + col;
                _VertexTexel[i] = texel;
                if (_LayerSkirt)
                {
                    _VertexTexel[vertexCount + i] = texel;
                }
            }

            NotifyMeshChanged();
        }

        /// <summary>
        /// Works out where every vertex sits in the plane and keeps it. The projection of a vertex is
        /// the one thing that costs anything here and it follows from the row and the column alone, so
        /// it is done the first time and never again for a new layer or a new exaggeration, which only
        /// move the vertices along the radius.
        /// </summary>
        private void EnsurePlan(RegularGridTopology topo, int vertexCount)
        {
            int row = _MFGrid.RowCount;
            int col = _MFGrid.ColumnCount;
            if (_PlanGrid == _MFGrid && _VertexLat != null && _VertexLat.Length == vertexCount)
                return;

            _RowOffset = RowOffsets(row);
            _ColOffset = ColumnOffsets(col);
            _VertexLat = new double[vertexCount];
            _VertexLng = new double[vertexCount];
            _VertexIndexAt = new int[row + 1, col + 1];
            for (int r = 0; r <= row; r++)
            {
                for (int c = 0; c <= col; c++)
                {
                    _VertexIndexAt[r, c] = -1;
                }
            }

            var origin = _MFGrid.Origin;
            double originX = origin != null ? origin.X : _MFGrid.BBox.MinX;
            double originY = origin != null ? origin.Y : _MFGrid.BBox.MaxY;

            for (int i = 0; i < vertexCount; i++)
            {
                int r = topo.VertexAtActiveCells[i, 4];
                int c = topo.VertexAtActiveCells[i, 5];
                _VertexIndexAt[r, c] = i;

                GCSConverter.ToLatLon(originX + _ColOffset[c], originY - _RowOffset[r]);
                _VertexLng[i] = GCSConverter.Longitude;
                _VertexLat[i] = GCSConverter.Latitude;
            }
            _PlanGrid = _MFGrid;
        }

        /// <summary>
        /// Builds the indices of the top faces and, while the walls are asked for, of the walls. They
        /// follow from the rows, the columns and which cells are active, none of which moves when the
        /// layer or the exaggeration does, so the array is built once and handed out again.
        /// </summary>
        private void EnsureIndices(RegularGridTopology topo, int cellCount)
        {
            if (_IndexGrid == _MFGrid && _VertexIndexList != null && _IndexSkirt == _LayerSkirt
                && _IndexCellCount == cellCount)
                return;

            int row = _MFGrid.RowCount;
            int col = _MFGrid.ColumnCount;
            int foot = _LayerSkirt ? topo.ActiveVertexCount : 0;
            var top = new int[cellCount * 6];
            var corners = new int[cellCount * 4];
            var walls = new List<int>();

            _SerialIndexAt = new int[row, col];
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    _SerialIndexAt[r, c] = -1;
                }
            }

            for (int i = 0; i < cellCount; i++)
            {
                var place = topo.ActiveCellLocation != null && i < topo.ActiveCellLocation.Length
                    ? topo.ActiveCellLocation[i] : null;

                if (place == null)
                {
                    // Nothing hands out the row and the column of the cell, the corners the topology
                    // kept are all there is to go on.
                    corners[i * 4] = topo.CellVertex[i, 1];
                    corners[i * 4 + 1] = topo.CellVertex[i, 2];
                    corners[i * 4 + 2] = topo.CellVertex[i, 3];
                    corners[i * 4 + 3] = topo.CellVertex[i, 4];
                    top[i * 6] = corners[i * 4];
                    top[i * 6 + 1] = corners[i * 4 + 1];
                    top[i * 6 + 2] = corners[i * 4 + 3];
                    top[i * 6 + 3] = corners[i * 4 + 3];
                    top[i * 6 + 4] = corners[i * 4 + 1];
                    top[i * 6 + 5] = corners[i * 4 + 2];
                    continue;
                }

                int r = place[0];
                int c = place[1];
                _SerialIndexAt[r, c] = i;

                // A cell is a square of four vertices, and which of them is which follows from the row
                // and the column of the cell and nothing else. The topology looks them up by an id
                // that two different vertices share, and a cell in the last column is then given a
                // corner out of the row below it.
                int id1 = _VertexIndexAt[r, c];
                int id2 = _VertexIndexAt[r, c + 1];
                int id3 = _VertexIndexAt[r + 1, c + 1];
                int id4 = _VertexIndexAt[r + 1, c];
                corners[i * 4] = id1;
                corners[i * 4 + 1] = id2;
                corners[i * 4 + 2] = id3;
                corners[i * 4 + 3] = id4;

                top[i * 6] = id1;
                top[i * 6 + 1] = id2;
                top[i * 6 + 2] = id4;

                top[i * 6 + 3] = id4;
                top[i * 6 + 4] = id2;
                top[i * 6 + 5] = id3;

                if (_LayerSkirt)
                {
                    // A wall is hung on every side of the cell that no other active cell is pressed
                    // against, which is the outside of the grid and the edge of every hole in it. The
                    // two corners of a side are given in the order that turns the wall to face out.
                    if (!IsCellActive(r - 1, c))
                        AddWall(walls, id1, id2, foot);
                    if (!IsCellActive(r + 1, c))
                        AddWall(walls, id3, id4, foot);
                    if (!IsCellActive(r, c - 1))
                        AddWall(walls, id4, id1, foot);
                    if (!IsCellActive(r, c + 1))
                        AddWall(walls, id2, id3, foot);
                }
            }

            _TopIndexList = top;
            _CellCorners = corners;
            _VertexIndexList = walls.Count > 0 ? top.Concat(walls).ToArray() : top;
            _IndexGrid = _MFGrid;
            _IndexSkirt = _LayerSkirt;
            _IndexCellCount = cellCount;
        }

        /// <summary>
        /// Two triangles from the two corners of a side of a cell down to the feet below them.
        /// </summary>
        private static void AddWall(List<int> walls, int p, int q, int foot)
        {
            walls.Add(p);
            walls.Add(q);
            walls.Add(q + foot);

            walls.Add(p);
            walls.Add(q + foot);
            walls.Add(p + foot);
        }

        private bool IsCellActive(int r, int c)
        {
            if (r < 0 || c < 0 || r >= _MFGrid.RowCount || c >= _MFGrid.ColumnCount)
                return false;
            var ibound = _MFGrid.IBound;
            // Without an ibound every cell is taken as active, which leaves the walls on the outside
            // of the grid and off every side that another cell is pressed against.
            return ibound == null || ibound[0, r, c] != 0;
        }

        /// <summary>
        /// How far every vertex is from the origin along the rows. MODFLOW hands out a width for every
        /// row, so they are summed up rather than one width being taken for all of them.
        /// </summary>
        private double[] RowOffsets(int row)
        {
            var offsets = new double[row + 1];
            var delc = _MFGrid.DELC;
            if (delc != null && delc.Size[2] >= row)
            {
                for (int r = 1; r <= row; r++)
                    offsets[r] = offsets[r - 1] + delc[0, 0, r - 1];
                return offsets;
            }
            double height = Math.Abs(_MFGrid.BBox.MaxY - _MFGrid.BBox.MinY) / row;
            for (int r = 1; r <= row; r++)
                offsets[r] = offsets[r - 1] + height;
            return offsets;
        }

        /// <summary>
        /// How far every vertex is from the origin along the columns, summed up out of the widths
        /// MODFLOW hands out for them.
        /// </summary>
        private double[] ColumnOffsets(int col)
        {
            var offsets = new double[col + 1];
            var delr = _MFGrid.DELR;
            if (delr != null && delr.Size[2] >= col)
            {
                for (int c = 1; c <= col; c++)
                    offsets[c] = offsets[c - 1] + delr[0, 0, c - 1];
                return offsets;
            }
            double width = Math.Abs(_MFGrid.BBox.MaxX - _MFGrid.BBox.MinX) / col;
            for (int c = 1; c <= col; c++)
                offsets[c] = offsets[c - 1] + width;
            return offsets;
        }

        /// <summary>
        /// The height of the foot of every vertex, which is the top of the layer below. The last layer
        /// has nothing below it, so it is given the thickness of the one above.
        /// </summary>
        private float[] BottomVertexValues(RegularGridTopology topo, int vertexCount, float[] top,
            float minValue, float maxValue)
        {
            var elevations = _MFGrid.Elevations;
            int layer = ModelService.CurrentGridLayer;
            float[] below = null;
            if (layer + 1 < elevations.Size[0])
                below = elevations.GetVector(layer + 1, "0", ":");

            if (below == null)
            {
                float thickness = 0;
                if (layer > 0)
                {
                    var above = elevations.GetVector(layer - 1, "0", ":");
                    float sum = 0;
                    int n = 0;
                    for (int i = 0; i < top.Length && i < above.Length; i++)
                    {
                        float d = above[i] - top[i];
                        if (d > 0)
                        {
                            sum += d;
                            n++;
                        }
                    }
                    thickness = n > 0 ? sum / n : 0;
                }
                if (thickness <= 0)
                    thickness = Math.Abs(maxValue - minValue) * 0.1f;

                below = new float[top.Length];
                for (int i = 0; i < top.Length; i++)
                    below[i] = top[i] - thickness;
            }

            var values = new float[vertexCount];
            for (int i = 0; i < vertexCount; i++)
                values[i] = topo.GetVertexValue(below, i);
            return values;
        }

        public override float GetCellValue(int row, int col)
        {
            // The serial index of the cell, looked up in the table built when the mesh was. The
            // topology hands it out by walking the keys of a dictionary, which is a scan of every
            // active cell, and this is called for every cell the user picks.
            var index = SerialIndexAt(row, col);
            if (index < 0)
                return 0;

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

        /// <summary>
        /// Puts a colour on a vertex, and on the foot below it while the walls are drawn, so that a wall
        /// keeps the colour of the top it holds up.
        /// </summary>
        private void SetVertexColor(int i, int color)
        {
            if (i < 0 || i >= _VertexList.Length)
                return;

            _VertexList[i].Color = color;

            // The feet are the second half of the array, in the same order as the vertices above them.
            if (_LayerSkirt)
            {
                int foot = _VertexList.Length / 2 + i;
                if (foot < _VertexList.Length)
                    _VertexList[foot].Color = color;
            }
        }

        /// <summary>
        /// The serial index of the cell at a row and a column, -1 where there is no active cell. It is
        /// the index a value is stored under, which is what the row and the column are asked for.
        /// </summary>
        private int SerialIndexAt(int row, int col)
        {
            if (_SerialIndexAt == null || row < 0 || col < 0
                || row > _SerialIndexAt.GetUpperBound(0) || col > _SerialIndexAt.GetUpperBound(1))
            {
                // Nothing was built, the topology is the only thing that knows.
                return _Topology != null ? _Topology.GetSerialIndex(row, col) : -1;
            }
            return _SerialIndexAt[row, col];
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
        /// The classes are handed to the shader as well, so that a step goes to the card as one byte
        /// per vertex rather than as the whole vertex array again.
        /// </summary>
        private float[] _VertexValueBuffer;
        private int[] _LevelBuffer;

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
                        // A value looked up in a table of its own has no class behind it, so there
                        // is nothing for the step textures to carry and the colours stay in the
                        // vertices where they were put.
                        ClearStepLevels();

                        var unqval = vector.Distinct().ToArray();
                        var dic = DataColor.ProduceColorDic<float>(unqval);

                        for (int i = 0; i < _MFGrid.Topology.ActiveVertexCount; i++)
                        {
                            var verValue = _MFGrid.Topology.GetUniqueVertexValue(vector, i);
                            System.Drawing.Color color;
                            if (dic.TryGetValue(verValue, out color))
                                SetVertexColor(i, color.ToArgb());
                            else
                                SetVertexColor(i, System.Drawing.Color.Transparent.ToArgb());
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

                        if (_LevelBuffer == null || _LevelBuffer.Length != vertexCount)
                        {
                            _LevelBuffer = new int[vertexCount];
                        }

                        if (ContinuousColor)
                        {
                            // Without a cache there is no range that covers every step, so the one of
                            // this step is what the values are scaled over. The shades are fine all the
                            // same, only the scale under them still moves from step to step, which is
                            // what the cache is what stops.
                            var low = (float)MinCellValue;
                            var high = (float)(MaxCellValue > MinCellValue ? MaxCellValue : MinCellValue + 1);
                            var scale = 255f / (high - low);
                            for (int i = 0; i < vertexCount; i++)
                            {
                                _LevelBuffer[i] = ShadeOf(_VertexValueBuffer[i] - low, scale);
                            }
                        }
                        else
                        {
                            // Classifying the values costs a sort over all of them and, for natural
                            // breaks, a pass over every pair of them, which is far more than putting
                            // the classes on the vertices afterwards. A run of steps is therefore
                            // classified once and every step of it is then only mapped onto the
                            // breaks that came out of that, which also keeps a class standing for the
                            // same range of values on every step of the run.
                            if (!_DataColor.HasFixedBreaks)
                            {
                                _DataColor.FixBreaks(_VertexValueBuffer, ColourRampCount, ClassificationMethod);
                            }
                            _DataColor.ClassifyInto(_VertexValueBuffer, _LevelBuffer);
                        }

                        // The classes go to the card as one byte for every vertex of the step, which
                        // is what keeps a frame from handing over an array as long as the grid only
                        // to change four bytes of every vertex of it.
                        UpdateStepLevels(_LevelBuffer, vertexCount);

                        // The colours in the vertices are never read while the mesh is drawn from the
                        // step textures, and writing them on every step is the one thing the textures
                        // were brought in to stop.
                        if (!ShadingActive)
                        {
                            var palette = LevelPalette;
                            for (int i = 0; i < vertexCount; i++)
                            {
                                SetVertexColor(i, ContinuousColor
                                    ? palette[_LevelBuffer[i]]
                                    : _DataColor.GetVertexColor(_LevelBuffer[i], Opacity, InvertColor));
                            }
                        }
                    }

                    // The device is only told to take the array again while it still is the array
                    // the mesh is drawn from.
                    if (!ShadingActive)
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
            // One step of the cache takes one byte for every active vertex, which is not the count of
            // vertices in the array while the walls are drawn, that one being twice as long.
            CachedLevelStride = vertexCount;
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
                        SetVertexColor(i, palette[l < palette.Length ? l : last]);
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
                int index = SerialIndexAt(row, col);

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