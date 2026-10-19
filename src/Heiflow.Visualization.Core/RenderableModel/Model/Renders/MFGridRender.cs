using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using HUST.WREIS.Dot3D;
using Heiflow.Core.Drawing;
using Heiflow.Models.Subsurface;
using Heiflow.Spatial;
using Heiflow.Models.Generic;
using Heiflow.Core.MyMath;
using Heiflow.Core.Data;
using GeoAPI.Geometries;


using SharpDX;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

namespace Heiflow.Visualization.Renderable.Grid
{
    /// <summary>
    /// How much of the third dimension a grid is drawn with, which is what makes it a sheet or a
    /// body. The top of a layer is drawn either way, the difference is how far the walls that hang
    /// off it reach down.
    /// </summary>
    public enum GridSkirt
    {
        /// <summary>
        /// The top of the layer on its own, which is a sheet.
        /// </summary>
        Flat = 0,

        /// <summary>
        /// Down to the top of the layer below, which is a body one layer thick.
        /// </summary>
        Layer = 1,

        /// <summary>
        /// Down to the floor of the model, which is a body as thick as the whole of it.
        /// </summary>
        Model = 2,

        /// <summary>
        /// Every layer of the model, one face on the top of each, so that the grid is a stack of
        /// them rather than one sheet.
        /// </summary>
        Layers = 3,

        /// <summary>
        /// The top of the layer as it is drawn on its own, and the walls around the outside of the
        /// model under it: down the sides of the outermost cells to the floor of the model, which
        /// is the shape of the whole of it. Nothing of the inside is given a wall of its own, and
        /// only the vertices on the outside are given the two a wall stands on, so it is a body of
        /// the same shape as the model for a small part of what a body costs.
        /// </summary>
        Boundary = 4,
    }

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
        private GridSkirt _IndexSkirt;
        private int _IndexFaces;
        private int _IndexCellCount;

        /// <summary>
        /// How many faces the grid was last drawn with, and how many vertices one of them takes. A
        /// colour is put on every one of the faces a vertex has, see SetVertexColor.
        /// </summary>
        private int _FaceCount = 1;
        private int _FaceVertices;

        /// <summary>
        /// The vertices of the grid the walls around the outside stand on, and the sides of it as
        /// pairs of them. They are filled for the boundary alone and hold far less than the grid
        /// does: the outside of it and nothing of the inside.
        /// </summary>
        private int[] _BoundaryVertices;
        private int[] _BoundaryEdges;

        /// <summary>
        /// Where every vertex of the grid is on the outside, -1 for the ones no wall stands on. It
        /// is what a colour is put on the walls with when the step moves on.
        /// </summary>
        private int[] _BoundarySlots;

        /// <summary>
        /// Where the vertices of the walls start in the mesh, and how many stand on the top of one,
        /// see CreatMeshes. Both are nothing but the outside of the grid, far less than it is.
        /// </summary>
        private int _WallVertexOffset;
        private int _WallVertexCount;

        /// <summary>
        /// The most vertices a grid of stacked layers is drawn with. A face is a copy of every
        /// vertex of the grid, so a model of many layers on a fine grid would ask for more of them
        /// than a card should be handed, and every stride-th interface is drawn instead, see
        /// LayerStride.
        /// </summary>
        private const int MaxFaceVertices = 400000;

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
        /// How much of the third dimension the grid is drawn with: the top of the layer on its own,
        /// the walls down to the layer below, or the walls down to the floor of the model. The walls
        /// double the vertices of the mesh, so a sheet is what is drawn until a body is asked for.
        /// </summary>
        public GridSkirt Skirt
        {
            get
            {
                return _Skirt;
            }
            set
            {
                if (_Skirt == value)
                    return;

                _Skirt = value;
                CreatMeshes();
            }
        }
        private GridSkirt _Skirt = GridSkirt.Flat;

        /// <summary>
        /// Whether walls are hung off the top at all, which is what every kind but a sheet has in
        /// common.
        /// </summary>
        public bool LayerSkirt
        {
            get
            {
                return _Skirt != GridSkirt.Flat;
            }
            set
            {
                Skirt = value ? GridSkirt.Layer : GridSkirt.Flat;
            }
        }

        /// <summary>
        /// The step textures are filled by walking the vertices of the grid and asking the mesh
        /// where each of them goes in the texture, one texel for one vertex of the grid and in that
        /// order. A stack of layers holds no such thing: a vertex of it is on every face of the
        /// stack and the textures carry one shade for all of them. A stack is therefore drawn from
        /// the colours on its vertices instead.
        /// </summary>
        protected override bool ShadedColorsAllowed
        {
            get { return _Skirt != GridSkirt.Layers; }
        }



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

            if (_VertexValueBuffer == null || _VertexValueBuffer.Length != vertexCount)
            {
                _VertexValueBuffer = new float[vertexCount];
            }
            for (int i = 0; i < vertexCount; i++)
            {
                _VertexValueBuffer[i] = topo.GetVertexValue(vector, i);
            }

            // What the grid is drawn as: one face on its own, or a face with a foot to hang walls
            // off, or one on the top of every layer. Each of them is a copy of every vertex of the
            // grid at the height of its own interface.
            var faces = SkirtFaces(topo, vertexCount, vector,
                (float)StatisticsInfo.Min, (float)StatisticsInfo.Max);
            int faceCount = faces.Length;

            // Where every vertex sits in the plane, and the corners and the indices that follow from
            // that. None of them moves when the layer or the exaggeration does, so all of them are
            // worked out once and are only read from then on.
            EnsurePlan(topo, vertexCount);
            EnsureIndices(topo, cellCount, faceCount);

            double layerRadius = EquatorialRadius + RenderableObject.DistanceAboveSurface * World.Settings.VerticalExaggeration;
            float exaggeration = World.Settings.VerticalExaggeration;
            var levels = _DataColor.GetLevels(_VertexValueBuffer, ColourRampCount, ClassificationMethod);

            // Every face is the grid as it is on its own interface, taken down or up along the
            // radius and nowhere else. It carries the colour of the values the layer is drawn for,
            // and a face further down is that colour taken a shade darker, so that the layers of a
            // stack can be told apart where they meet.
            // The walls of the boundary are the only thing besides the sheet, and only the vertices
            // they stand on are given a foot: two copies of those, one on the top and one on the
            // floor, where a body would give a foot to every vertex of the grid.
            bool boundary = _Skirt == GridSkirt.Boundary;
            int walls = boundary ? _BoundaryVertices.Length : 0;
            int total = vertexCount * faceCount + walls * 2;
            var vertexList = new CustomVertex.PositionNormalColored[total];

            for (int face = 0; face < faceCount; face++)
            {
                var height = faces[face];
                int at = face * vertexCount;

                for (int i = 0; i < vertexCount; i++)
                {
                    PlaceVertex(vertexList, at + i, i, height[i],
                        FaceShade(_DataColor.GetVertexColor(levels[i], Opacity), face),
                        layerRadius, exaggeration);
                }
            }

            _WallVertexOffset = vertexCount * faceCount;
            _WallVertexCount = walls;

            if (boundary)
            {
                var topFace = faces[0];
                var floor = BoundaryFloor(topo, topFace);
                int feet = _WallVertexOffset;

                for (int i = 0; i < walls; i++)
                {
                    int vertex = _BoundaryVertices[i];
                    var color = _DataColor.GetVertexColor(levels[vertex], Opacity);

                    PlaceVertex(vertexList, feet + i, vertex, topFace[vertex], color, layerRadius, exaggeration);
                    PlaceVertex(vertexList, feet + walls + i, vertex, floor[i],
                        FaceShade(color, 1), layerRadius, exaggeration);
                }
            }

            _FaceCount = faceCount;
            _FaceVertices = vertexCount;

            GCSConverter.ToLatLon(_MFGrid.BBox.MinX, _MFGrid.BBox.MaxY);
            double west = GCSConverter.Longitude;
            double north = GCSConverter.Latitude;
            GCSConverter.ToLatLon(_MFGrid.BBox.MaxX, _MFGrid.BBox.MinY);
            double east = GCSConverter.Longitude;
            double sourth = GCSConverter.Latitude;

            BBox = new Envelope(west, east, sourth, north);
            _MFGrid.CentralPoint = new DataCube<float>(3, 0, cellCount);

            // Out of the corners of the cell and out of the heights of the top, which is where the
            // centre of a cell is whichever shape the grid is drawn as.
            var surface = faces[0];
            for (int i = 0; i < cellCount; i++)
            {
                int id1 = _CellCorners[i * 4];
                int id2 = _CellCorners[i * 4 + 1];
                int id3 = _CellCorners[i * 4 + 2];
                int id4 = _CellCorners[i * 4 + 3];
                _MFGrid.CentralPoint[0, 0, i] = (float)(_VertexLng[id1] + _VertexLng[id2]) * 0.5f;
                _MFGrid.CentralPoint[1, 0, i] = (float)(_VertexLat[id1] + _VertexLat[id2]) * 0.5f;
                // Each of the four corners once. The height used to be taken over the third of them
                // twice and over the fourth one not at all.
                _MFGrid.CentralPoint[2, 0, i] = (surface[id1] + surface[id2]
                    + surface[id3] + surface[id4]) * 0.25f;
            }

            // The normals come off the faces alone, or a vertex would be given the normal of a wall
            // hanging off it and the grid would be lit as though it were folded. Every face is in
            // the list, so a foot is lit the way the face it holds up is rather than coming out of
            // the normals as black, which is what a normal of no length at all is drawn as. The
            // walls of the boundary hold vertices of their own and none of the sheet, so they are
            // taken in as well and are lit off themselves.
            CalculateNormals(ref vertexList, boundary ? _VertexIndexList : _TopIndexList);

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
                var texel = TexelOf(topo, i);

                // The same place in the texture for the vertex on every face, which is what puts the
                // values of the step on all of them.
                for (int face = 0; face < faceCount; face++)
                {
                    _VertexTexel[face * vertexCount + i] = texel;
                }
            }

            // The two ends of a wall of the boundary are the vertex of the grid it stands on, so
            // they read the same place in the texture as it does.
            if (boundary)
            {
                int feet = vertexCount * faceCount;

                for (int i = 0; i < walls; i++)
                {
                    var texel = TexelOf(topo, _BoundaryVertices[i]);
                    _VertexTexel[feet + i] = texel;
                    _VertexTexel[feet + walls + i] = texel;
                }
            }

            NotifyMeshChanged();
        }

        /// <summary>
        /// Puts a vertex of the grid into the mesh at the place given, at the height given. The
        /// radius is the only thing that moves it, the row and the column it stands on were worked
        /// out once, see EnsurePlan.
        /// </summary>
        private void PlaceVertex(CustomVertex.PositionNormalColored[] vertexList, int at, int vertex,
            float height, int color, double layerRadius, float exaggeration)
        {
            double radius = layerRadius + (height + this.DistanceAboveSurface) * exaggeration;
            Vector3 curNode = MathEngine.SphericalToCartesian(_VertexLat[vertex], _VertexLng[vertex], radius);
            vertexList[at].X = curNode.X;
            vertexList[at].Y = curNode.Y;
            vertexList[at].Z = curNode.Z;
            vertexList[at].Nx = (float)_VertexLng[vertex];
            vertexList[at].Ny = (float)_VertexLat[vertex];
            vertexList[at].Nz = height * exaggeration;
            vertexList[at].Color = color;
        }

        /// <summary>
        /// Builds the line drawn around the outside of the model while the boundary is asked for:
        /// along the top of the outermost cells, down the sides of them and along the floor of the
        /// model, which is what shows how thick the whole of it is. Only the vertices on the outside
        /// come into it, so the line costs next to nothing beside the sheet it is drawn over.
        /// </summary>
        /// <summary>
        /// How high the floor of the model is under every vertex the walls stand on. It is the last
        /// interface the model holds a height for, and where there is none the wall is left as deep
        /// as the top it hangs off.
        /// </summary>
        private float[] BoundaryFloor(RegularGridTopology topo, float[] surface)
        {
            int count = _BoundaryVertices.Length;
            var floor = new float[count];
            var elevations = _MFGrid.Elevations;

            if (elevations != null && elevations.Size[0] > 1)
            {
                var cells = elevations.GetVector(elevations.Size[0] - 1, "0", ":");
                if (cells != null)
                {
                    for (int i = 0; i < count; i++)
                    {
                        floor[i] = topo.GetVertexValue(cells, _BoundaryVertices[i]);
                    }
                    return floor;
                }
            }

            for (int i = 0; i < count; i++)
            {
                floor[i] = surface[_BoundaryVertices[i]];
            }
            return floor;
        }

        /// <summary>
        /// Where a vertex goes in the texture the steps are drawn from, out of the row and the
        /// column it stands on.
        /// </summary>
        private int TexelOf(RegularGridTopology topo, int vertex)
        {
            var row = (int)topo.VertexAtActiveCells[vertex, 4];
            var col = (int)topo.VertexAtActiveCells[vertex, 5];
            return row * _DataTextureWidth + col;
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
        /// Builds the indices of the faces and of the walls that hang between them. They follow from
        /// the rows, the columns, which cells are active and how many faces there are, none of which
        /// moves when the layer or the exaggeration does, so the array is built once and handed out
        /// again.
        /// </summary>
        private void EnsureIndices(RegularGridTopology topo, int cellCount, int faceCount)
        {
            // The same grid in two shapes asks for two different arrays even where the number of
            // faces is the one thing they have in common, a stack of layers and the walls around it.
            if (_IndexGrid == _MFGrid && _VertexIndexList != null && _IndexFaces == faceCount
                && _IndexCellCount == cellCount && _IndexSkirt == _Skirt)
                return;

            int row = _MFGrid.RowCount;
            int col = _MFGrid.ColumnCount;
            int stride = topo.ActiveVertexCount;
            bool boundary = _Skirt == GridSkirt.Boundary;
            // The sheet is drawn whichever shape is asked for, the boundary is one with a line around
            // the outside of the model over it.
            var top = new int[cellCount * 6 * faceCount];
            var corners = new int[cellCount * 4];
            var walls = new List<int>();

            // The walls around the outside are all the mesh holds, so the vertices are counted out
            // as the walls are, and only the ones a wall stands on are given a place in it.
            var edges = boundary ? new List<int>() : null;
            var used = boundary ? new List<int>() : null;
            var remap = boundary ? new int[stride] : null;
            if (remap != null)
            {
                for (int i = 0; i < stride; i++)
                {
                    remap[i] = -1;
                }
            }

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
                    AddFace(top, cellCount, i, faceCount, stride,
                        corners[i * 4], corners[i * 4 + 1], corners[i * 4 + 2], corners[i * 4 + 3]);
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

                AddFace(top, cellCount, i, faceCount, stride, id1, id2, id3, id4);

                // A wall is hung on every side of the cell that no other active cell is pressed
                // against, which is the outside of the grid and the edge of every hole in it, and
                // between two faces there is one: the side of the layer between them. The two
                // corners of a side are given in the order that turns the wall to face out.
                // While the boundary is asked for, the sides the line runs along are kept as they
                // are found, and the mesh itself is left as the sheet is.
                if (boundary)
                {
                    if (!IsCellActive(r - 1, c))
                        AddEdge(edges, used, remap, id1, id2);
                    if (!IsCellActive(r + 1, c))
                        AddEdge(edges, used, remap, id3, id4);
                    if (!IsCellActive(r, c - 1))
                        AddEdge(edges, used, remap, id4, id1);
                    if (!IsCellActive(r, c + 1))
                        AddEdge(edges, used, remap, id2, id3);
                }
                else
                {
                    for (int face = 0; face + 1 < faceCount; face++)
                    {
                        int at = face * stride;
                        if (!IsCellActive(r - 1, c))
                            AddWall(walls, id1 + at, id2 + at, stride);
                        if (!IsCellActive(r + 1, c))
                            AddWall(walls, id3 + at, id4 + at, stride);
                        if (!IsCellActive(r, c - 1))
                            AddWall(walls, id4 + at, id1 + at, stride);
                        if (!IsCellActive(r, c + 1))
                            AddWall(walls, id2 + at, id3 + at, stride);
                    }
                }
            }

            _CellCorners = corners;

            if (boundary)
            {
                int count = used.Count;

                // The walls stand on copies of the vertices of the outside rather than on the sheet
                // itself, so they can be lit off themselves and the sheet is left alone. One copy on
                // the top and one on the floor, both behind the sheet in the mesh, and the foot of a
                // wall is as far on again as there are vertices on the top, see CreatMeshes.
                int at = stride * faceCount;
                for (int i = 0; i < edges.Count; i += 2)
                {
                    AddWall(walls, at + edges[i], at + edges[i + 1], count);
                }

                _BoundaryVertices = used.ToArray();
                _BoundaryEdges = edges.ToArray();
                _BoundarySlots = remap;
                _TopIndexList = top;
                _VertexIndexList = walls.Count > 0 ? top.Concat(walls).ToArray() : top;
            }
            else
            {
                _BoundaryVertices = null;
                _BoundaryEdges = null;
                _BoundarySlots = null;
                _TopIndexList = top;
                _VertexIndexList = walls.Count > 0 ? top.Concat(walls).ToArray() : top;
            }

            _IndexGrid = _MFGrid;
            _IndexSkirt = _Skirt;
            _IndexFaces = faceCount;
            _IndexCellCount = cellCount;
        }

        /// <summary>
        /// One side of a wall of the boundary, as the two vertices of the grid it stands on. A
        /// vertex is given its place in the mesh the first time a side asks for it. A corner the
        /// topology has no vertex for leaves the side out: there is nothing to hang a wall on.
        /// </summary>
        private static void AddEdge(List<int> edges, List<int> used, int[] remap, int p, int q)
        {
            int at1 = VertexSlot(used, remap, p);
            int at2 = VertexSlot(used, remap, q);
            if (at1 < 0 || at2 < 0)
                return;

            edges.Add(at1);
            edges.Add(at2);
        }

        private static int VertexSlot(List<int> used, int[] remap, int vertex)
        {
            // A corner that is not one of the vertices of the grid, and one the table does not hold
            // a place for at all, are both left out rather than asked for.
            if (vertex < 0 || vertex >= remap.Length)
                return -1;

            int at = remap[vertex];
            if (at < 0)
            {
                at = used.Count;
                remap[vertex] = at;
                used.Add(vertex);
            }
            return at;
        }

        /// <summary>
        /// Two triangles of a cell, written for every face of the grid. The four corners are the
        /// ones of the first face, the others are the same corners a whole face further on.
        /// </summary>
        private static void AddFace(int[] top, int cellCount, int cell, int faceCount, int stride,
            int id1, int id2, int id3, int id4)
        {
            for (int face = 0; face < faceCount; face++)
            {
                int at = (face * cellCount + cell) * 6;
                int off = face * stride;
                top[at] = id1 + off;
                top[at + 1] = id2 + off;
                top[at + 2] = id4 + off;

                top[at + 3] = id4 + off;
                top[at + 4] = id2 + off;
                top[at + 5] = id3 + off;
            }
        }

        /// <summary>
        /// Two triangles from the two corners of a side of a cell down to the corners below them.
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
        /// The height of every vertex of every face the grid is drawn with, one array of them to a
        /// face and the first of them the one the walls hang off.
        ///
        /// Elevations holds the interfaces and not the layers, one more of them than there are
        /// layers: the first is the top of the model, the one after a layer is the floor of it, and
        /// the last is the floor of the model, which is what SFRPackage walks when it lays the
        /// layers out, `Elevations[l + 1] = Elevations[l] - height[l]`. So a foot is the floor of the
        /// layer that is drawn, or the floor of the whole model where the whole of it is asked for,
        /// and every layer at once is a face on every one of them. Where no floor is held, the
        /// thickness of the layer above is taken instead.
        /// </summary>
        private float[][] SkirtFaces(RegularGridTopology topo, int vertexCount, float[] top,
            float minValue, float maxValue)
        {
            var elevations = _MFGrid.Elevations;
            int layer = ModelService.CurrentGridLayer;

            if (_Skirt == GridSkirt.Layers)
            {
                // A face on every interface, or on as many of them as the vertices allow.
                int interfaces = elevations.Size[0];
                int stride = LayerStride(interfaces, vertexCount);
                var stack = new List<float[]>();
                for (int i = 0; i < interfaces; i += stride)
                {
                    var face = FaceVertexValues(topo, vertexCount, elevations, i);
                    if (face != null)
                        stack.Add(face);
                }
                if ((interfaces - 1) % stride != 0)
                {
                    var floor = FaceVertexValues(topo, vertexCount, elevations, interfaces - 1);
                    if (floor != null)
                        stack.Add(floor);
                }
                return stack.Count > 0 ? stack.ToArray()
                    : new[] { InterfaceVertexValues(topo, vertexCount, top) };
            }

            var values = new List<float[]> { InterfaceVertexValues(topo, vertexCount, top) };

            // A sheet is the one face and nothing else, and so is the boundary: it is the sheet with
            // a line around the outside of the model over it, and the line is built on its own, see
            // BuildBoundaryWire. The other two take a second face, which is where the walls stop:
            // the floor of the layer that is drawn, or the floor of the whole model.
            if (_Skirt == GridSkirt.Flat || _Skirt == GridSkirt.Boundary)
                return values.ToArray();

            int bottom = _Skirt == GridSkirt.Model ? elevations.Size[0] - 1 : layer + 1;
            var foot = bottom > layer && bottom < elevations.Size[0]
                ? FaceVertexValues(topo, vertexCount, elevations, bottom) : null;

            values.Add(foot != null ? foot
                : EstimatedFootValues(topo, vertexCount, elevations, top, minValue, maxValue, layer));
            return values.ToArray();
        }

        /// <summary>
        /// The height of every vertex of one interface of the model, turned out of the heights of
        /// the cells the way the mesh wants them. Nothing comes back where the interface is not
        /// held.
        /// </summary>
        private static float[] FaceVertexValues(RegularGridTopology topo, int vertexCount,
            DataCube<float> elevations, int face)
        {
            var vector = elevations.GetVector(face, "0", ":");
            return vector != null ? InterfaceVertexValues(topo, vertexCount, vector) : null;
        }

        /// <summary>
        /// The height of every vertex out of the heights of the cells it stands between.
        /// </summary>
        private static float[] InterfaceVertexValues(RegularGridTopology topo, int vertexCount, float[] cells)
        {
            var values = new float[vertexCount];
            for (int i = 0; i < vertexCount; i++)
            {
                values[i] = topo.GetVertexValue(cells, i);
            }
            return values;
        }

        /// <summary>
        /// Where a layer is the last one and no floor is held under it, the floor is put a layer's
        /// thickness down: the thickness of the one above where that is known, and a tenth of the
        /// span of the values where it is not.
        /// </summary>
        private static float[] EstimatedFootValues(RegularGridTopology topo, int vertexCount,
            DataCube<float> elevations, float[] top, float minValue, float maxValue, int layer)
        {
            float thickness = 0;
            if (layer > 0)
            {
                var above = elevations.GetVector(layer - 1, "0", ":");
                float sum = 0;
                int n = 0;
                for (int i = 0; i < top.Length && above != null && i < above.Length; i++)
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

            var values = InterfaceVertexValues(topo, vertexCount, top);
            for (int i = 0; i < values.Length; i++)
            {
                values[i] -= thickness;
            }
            return values;
        }

        /// <summary>
        /// How many interfaces of the model are skipped between two faces of a stack. A face is a
        /// copy of every vertex of the grid, so a model of many layers on a fine grid would ask for
        /// more of them than a card should be handed, and one face is then put on every stride-th
        /// interface instead.
        /// </summary>
        private static int LayerStride(int interfaces, int vertexCount)
        {
            if (vertexCount <= 0)
                return 1;

            int stride = 1;
            while (stride < interfaces
                && (long)((interfaces + stride - 1) / stride) * vertexCount > MaxFaceVertices)
            {
                stride++;
            }
            return stride;
        }

        /// <summary>
        /// The colour of a vertex on a face, taken a shade darker the further down the face lies, so
        /// that the layers of a stack can be told apart where they meet.
        /// </summary>
        private static int FaceShade(int color, int face)
        {
            if (face <= 0)
                return color;

            float dim = Math.Max(0.45f, 1f - 0.07f * face);
            int alpha = (color >> 24) & 0xFF;
            int red = (int)(((color >> 16) & 0xFF) * dim);
            int green = (int)(((color >> 8) & 0xFF) * dim);
            int blue = (int)((color & 0xFF) * dim);
            return (alpha << 24) | (red << 16) | (green << 8) | blue;
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
        /// Puts a colour on a vertex, on every face of it, so that a wall keeps the colour of the
        /// face it holds up and a stack is one colour down through its layers.
        /// </summary>
        private void SetVertexColor(int i, int color)
        {
            if (i < 0 || i >= _VertexList.Length)
                return;

            // Every face is a whole copy of the grid, one behind the other, so the vertex is at the
            // same place in each of them. A sheet has one face and is the vertex itself.
            int faces = _FaceCount > 0 ? _FaceCount : 1;
            int stride = _FaceVertices > 0 ? _FaceVertices : _VertexList.Length;

            for (int face = 0; face < faces; face++)
            {
                int at = face * stride + i;
                if (at < _VertexList.Length)
                    _VertexList[at].Color = FaceShade(color, face);
            }

            // A wall of the boundary keeps the colour of the vertex of the grid it stands on, on
            // the copy on the top and a shade darker on the one on the floor, which is the way a
            // body is drawn as well.
            if (_BoundarySlots == null || _WallVertexCount == 0 || i >= _BoundarySlots.Length)
                return;

            int slot = _BoundarySlots[i];
            if (slot < 0)
                return;

            int onTop = _WallVertexOffset + slot;
            if (onTop < _VertexList.Length)
                _VertexList[onTop].Color = color;

            int foot = onTop + _WallVertexCount;
            if (foot < _VertexList.Length)
                _VertexList[foot].Color = FaceShade(color, 1);
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