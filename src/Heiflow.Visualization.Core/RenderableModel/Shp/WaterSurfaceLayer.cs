using DotSpatial.Data;
using DotSpatial.Projections;
using GeoAPI.Geometries;
using Heiflow.Core;
using Heiflow.Visualization.Renderable.Grid;
using HUST.WREIS.Dot3D;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Heiflow.Visualization.Renderable
{
    /// <summary>
    /// A polygon shapefile drawn as water: every polygon of it is filled with a sheet of triangles
    /// that three waves run across, see Shaders\Water.fx.
    /// </summary>
    /// <remarks>
    /// The polygons of a shapefile hold far too few corners to carry a wave, a lake being some tens
    /// of them, so a grid of squares is laid over the outline and every square the water reaches is
    /// cut into two triangles, see MeshBuilder. The triangles of a grid are all of one size, where
    /// the outline cut up on its own comes out in triangles of every size and shape. The corners the
    /// grid makes are shared through a lookup, or the waves of two triangles next to each other
    /// would not meet and the sheet would come apart along the seams.
    ///
    /// The texture coordinates are metres east and north of the middle of the polygon rather than
    /// the row and the column of a corner, so a wave is the same size on a lake of any shape and
    /// anywhere on earth.
    /// </remarks>
    public class WaterSurfaceLayer : RenderableShp
    {
        /// <summary>
        /// Metres of latitude, and of longitude at the equator. Only the middle of the polygon is
        /// measured with them, and it is close enough over one lake.
        /// </summary>
        private const double MetreOfLatitude = 110540.0;
        private const double MetreOfLongitude = 111320.0;

        /// <summary>
        /// How many corners the sheet of one polygon may hold. A long coastline would ask for more
        /// than the machine has to give, so the squares of the grid grow until the water fits in them
        /// rather than the sheet growing without end.
        /// </summary>
        private const int MaxVertices = 10000;

        private readonly WaterShader _Shader = new WaterShader();
        private readonly List<WaterBody> _Bodies = new List<WaterBody>();
        private readonly DateTime _Started = DateTime.UtcNow;
        public WaterSurfaceLayer(string filename, World parentWorld, Color waterColor)
            : base("Water surface", parentWorld)
        {
            _Filename = filename;
            m_world = parentWorld;
            _FeatureColor = waterColor;

            WaveHeight = 0.6;
            WaveLength = 40.0;
            WaveSpeed = 1.0;
            ShallowColor = Color.FromArgb(97, 168, 194);
            DeepColor = Color.FromArgb(13, 54, 97);
            SkyColor = Color.FromArgb(148, 189, 224);
        }

        /// <summary>
        /// Metres above the datum the water stands at. Nothing set means the height is taken from
        /// the shapefile, and failing that from the ground under the outline.
        /// </summary>
        public double? WaterLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Metres from a trough to a crest, metres between two crests, and how fast a crest runs.
        /// </summary>
        public double WaveHeight
        {
            get;
            set;
        }

        public double WaveLength
        {
            get;
            set;
        }

        public double WaveSpeed
        {
            get;
            set;
        }

        /// <summary>
        /// How long the side of a square of the grid is, in metres, and with it the side of every
        /// triangle of the sheet. A wave needs a few triangles to run over, so it follows from the
        /// length of one, and it is what keeps the sheet small enough. A water too wide for the sheet
        /// the card will take gets squares larger than these rather than no water at all.
        /// </summary>
        public double MeshEdge
        {
            get { return Math.Max(WaveLength / 6.0, 2.0); }
        }

        public Color ShallowColor
        {
            get;
            set;
        }

        public Color DeepColor
        {
            get;
            set;
        }

        public Color SkyColor
        {
            get;
            set;
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            UpdateVertices();
            isInitialized = true;
        }

        public override void UpdateVertices()
        {
            if (_Bodies.Count > 0)
                return;

            try
            {
                _IFeatureSet = FeatureSet.Open(_Filename);
                var shapes = _IFeatureSet.ShapeIndices.Count;
                var vertices = _IFeatureSet.Vertex;
                var wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;
                bool latlon = _IFeatureSet.Projection == null || _IFeatureSet.Projection.IsLatLon;

                for (int n = 0; n < shapes; n++)
                {
                    var shape = _IFeatureSet.ShapeIndices[n];
                    var level = LevelOf(n);

                    // Every ring of the polygon is cut up on its own. Read as one long ring, which is
                    // what the first corner of the first to the last corner of the last gave, the end
                    // of one ring is joined to the start of the next and the ring crosses itself,
                    // which is a shape the cutting cannot finish.
                    for (int part = 0; part < shape.Parts.Count; part++)
                    {
                        int start = shape.Parts[part].StartIndex;
                        int end = shape.Parts[part].EndIndex;
                        int points = end - start + 1;
                        if (points < 3)
                            continue;

                        var ring = new Coordinate[points];
                        var xy = new double[2];
                        var z = new double[1];

                        for (int i = start; i <= end; i++)
                        {
                            xy[0] = vertices[2 * i];
                            xy[1] = vertices[2 * i + 1];

                            // The shapefile is read where it lies, and a projected one is turned into
                            // longitude and latitude, which is what the globe is drawn from.
                            if (!latlon)
                            {
                                Reproject.ReprojectPoints(xy, z, _IFeatureSet.Projection, wgs84, 0, 1);
                            }

                            ring[i - start] = new Coordinate(xy[0], xy[1]);
                        }

                        var body = BuildBody(ring, level);
                        if (body != null)
                            _Bodies.Add(body);
                    }
                }

                DataTable = _IFeatureSet.DataTable;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            finally
            {
                if (_IFeatureSet != null)
                {
                    _IFeatureSet.Close();
                    _IFeatureSet = null;
                }
            }

            NeedsUpdate = false;
            isInitialized = true;
        }

        /// <summary>
        /// How high the water of one polygon stands. The height set on the layer comes first, then a
        /// column of the shapefile that reads as one, and then the ground under the outline, which
        /// puts a lake in the hollow it lies in without anyone saying how deep it is.
        /// </summary>
        private double LevelOf(int shape)
        {
            if (WaterLevel.HasValue)
                return WaterLevel.Value;

            var table = _IFeatureSet.DataTable;
            if (table != null && shape < table.Rows.Count)
            {
                foreach (var column in table.Columns)
                {
                    var name = column.ToString();
                    if (name == null)
                        continue;

                    var upper = name.ToUpper();
                    if (upper.Contains("ELEV") || upper.Contains("LEVEL") || upper.Contains("STAGE")
                        || upper.Contains("HEIGHT") || upper == "Z")
                    {
                        var value = table.Rows[shape][name];
                        double level;
                        if (value != null && double.TryParse(value.ToString(), out level))
                            return level;
                    }
                }
            }

            return 0.0;
        }

        /// <summary>
        /// One polygon as a sheet of triangles, ready to be drawn.
        /// </summary>
        private WaterBody BuildBody(Coordinate[] ring, double level)
        {
            double lon0 = 0.0;
            double lat0 = 0.0;
            for (int i = 0; i < ring.Length; i++)
            {
                lon0 += ring[i].X;
                lat0 += ring[i].Y;
            }
            lon0 /= ring.Length;
            lat0 /= ring.Length;

            // Where the ground is under the outline, for a lake that carries no height of its own.
            if (!WaterLevel.HasValue && level == 0.0 && AutoTerrainHeight && World.TerrainAccessor != null)
            {
                double ground = 0.0;
                for (int i = 0; i < ring.Length; i++)
                {
                    ground += World.TerrainAccessor.GetElevationAt(ring[i].Y, ring[i].X);
                }
                level = ground / ring.Length;
            }

            var mesh = new MeshBuilder(MeshEdge, MaxVertices);
            double eastOfLongitude = MetreOfLongitude * Math.Cos(MathEngine.DegreesToRadians(lat0));

            // The outline in metres east and north of the middle of it, which is the sheet is laid
            // out in. Cutting the outline into triangles of its own gave slivers of every size, and
            // cutting those in four again left them slivers still, which is what the waves were
            // running so badly over.
            var outlineEast = new double[ring.Length];
            var outlineNorth = new double[ring.Length];
            for (int i = 0; i < ring.Length; i++)
            {
                outlineEast[i] = (ring[i].X - lon0) * eastOfLongitude;
                outlineNorth[i] = (ring[i].Y - lat0) * MetreOfLatitude;
            }

            mesh.Fill(outlineEast, outlineNorth);

            if (mesh.VertexCount < 3 || mesh.IndexCount < 3)
                return null;

            float exaggeration = World.Settings.VerticalExaggeration;
            double radius = World.EquatorialRadius
                + exaggeration * (level + DistanceAboveSurface);

            var list = new CustomVertex.PositionNormalTextured[mesh.VertexCount];
            for (int i = 0; i < list.Length; i++)
            {
                double east;
                double north;
                mesh.Corner(i, out east, out north);

                double lon = lon0 + east / eastOfLongitude;
                double lat = lat0 + north / MetreOfLatitude;

                var position = MathEngine.SphericalToCartesian(lat, lon, radius);
                var up = Vector3.Normalize(position);

                list[i].X = position.X;
                list[i].Y = position.Y;
                list[i].Z = position.Z;
                list[i].Nx = up.X;
                list[i].Ny = up.Y;
                list[i].Nz = up.Z;
                list[i].Tu = (float)east;
                list[i].Tv = (float)north;
            }

            var body = new WaterBody();
            body.Vertices = list;
            body.Triangles = mesh.Indices;
            body.VertexCount = list.Length;
            body.IndexCount = mesh.IndexCount;

            // Which way is east and which is north where this water lies. The shader is told, so it
            // knows which way a wave leans from the two metres it is given.
            double step = 0.001;
            var middle = MathEngine.SphericalToCartesian(lat0, lon0, radius);
            var toEast = MathEngine.SphericalToCartesian(lat0, lon0 + step, radius);
            var toNorth = MathEngine.SphericalToCartesian(lat0 + step, lon0, radius);
            body.East = Vector3.Normalize(toEast - middle);
            body.North = Vector3.Normalize(toNorth - middle);

            return body;
        }

        public override void Render(DrawArgs drawArgs)
        {
            if (!isInitialized || !IsOn || _Bodies.Count == 0)
                return;

            double altitude = drawArgs.WorldCamera.Altitude;
            if (altitude < MinDisplayAltitude || altitude > MaxDisplayAltitude)
                return;

            var device = DrawArgs.Device;
            if (device == null)
                return;

            using (ModelRenderScope.Begin(device, drawArgs, FillMode, !World.Settings.EnableHighPerfomance))
            {
                // The water is drawn through, so the blending the fixed pipeline was left with is
                // put back when the layer is done with it.
                bool blending = device.RenderState.AlphaBlendEnable;
                var source = device.RenderState.SourceBlend;
                var destination = device.RenderState.DestinationBlend;
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.SourceBlend = Blend.SourceAlpha;
                device.RenderState.DestinationBlend = Blend.InvSourceAlpha;

                try
                {
                    _Shader.Ensure(device);

                    var parameters = new WaterParameters();
                    parameters.WorldViewProjection = Matrix.Multiply(device.Transform.World,
                        Matrix.Multiply(device.Transform.View, device.Transform.Projection));
                    parameters.EyePosition = drawArgs.WorldCamera.Position;
                    // Where the sun stands is given as a place in the sky, which is a hundred and
                    // fifty million kilometres away, and the shader wants no more than which way to
                    // look for it. It is straightened here, once for the frame, the pixel shader
                    // having no room to do it for every pixel.
                    parameters.SunDirection = Vector3.Normalize(ModelRenderScope.SunVector);

                    // A float runs out of the seconds it can tell apart, so the clock of the water
                    // is taken round once an hour. Nothing of the waves shows the seam.
                    parameters.Time = (float)((DateTime.UtcNow - _Started).TotalSeconds % 3600.0);
                    parameters.WaveHeight = (float)WaveHeight;
                    parameters.WaveLength = (float)Math.Max(WaveLength, 1.0);
                    parameters.WaveSpeed = (float)WaveSpeed;
                    parameters.Opacity = Opacity / 255f;
                    parameters.ShallowColor = FloatsOf(ShallowColor);
                    parameters.DeepColor = FloatsOf(DeepColor);
                    parameters.SkyColor = FloatsOf(SkyColor);

                    foreach (var body in _Bodies)
                    {
                        // The buffers are made here and not where the shapefile is read, or a layer
                        // of a project that is loaded before the card is up would have no water.
                        body.MakeBuffers(device);
                        if (!body.OnCard)
                            continue;

                        parameters.East = body.East;
                        parameters.North = body.North;

                        if (!_Shader.Draw(device, body.VertexBuffer, body.IndexBuffer,
                            body.VertexCount, body.IndexCount, parameters))
                        {
                            DrawFlat(device, body);
                        }
                    }
                }
                catch (DeviceLostException)
                {
                    // The buffers go with the card, and both are made again on the next frame.
                    foreach (var body in _Bodies)
                    {
                        body.Release();
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
                finally
                {
                    device.RenderState.AlphaBlendEnable = blending;
                    device.RenderState.SourceBlend = source;
                    device.RenderState.DestinationBlend = destination;
                }
            }
        }

        /// <summary>
        /// The water as a still sheet of one colour, for a card that will not take the shader.
        /// </summary>
        private void DrawFlat(Device device, WaterBody body)
        {
            bool lighting = device.RenderState.Lighting;
            Color ambient = device.RenderState.Ambient;

            var material = new Material();
            material.Diffuse = ShallowColor;
            material.Ambient = ShallowColor;

            device.RenderState.Lighting = true;
            device.RenderState.Ambient = Color.White;
            device.Material = material;
            device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            device.SetStreamSource(0, body.VertexBuffer, 0, CustomVertex.PositionNormalTextured.StrideSize);
            device.Indices = body.IndexBuffer;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, body.VertexCount, 0,
                body.IndexCount / 3);
            device.Indices = null;

            device.RenderState.Ambient = ambient;
            device.RenderState.Lighting = lighting;
        }

        private static float[] FloatsOf(Color color)
        {
            return new[] { color.R / 255f, color.G / 255f, color.B / 255f, 1f };
        }

        public override void Dispose()
        {
            foreach (var body in _Bodies)
            {
                body.Release();
            }
            _Bodies.Clear();
            _Shader.Dispose();
        }

        /// <summary>
        /// One polygon of water. The corners are worked out before the card is asked for anything,
        /// and the buffers are made the first time the water is drawn and again whenever the card
        /// has been lost and come back, which is why the corners are kept.
        /// </summary>
        private sealed class WaterBody
        {
            public CustomVertex.PositionNormalTextured[] Vertices;
            public int[] Triangles;
            public int VertexCount;
            public int IndexCount;
            public Vector3 East;
            public Vector3 North;

            public VertexBuffer VertexBuffer;
            public IndexBuffer IndexBuffer;

            public bool OnCard
            {
                get { return VertexBuffer != null && IndexBuffer != null; }
            }

            public void MakeBuffers(Device device)
            {
                if (OnCard || device == null || Vertices == null || Triangles == null)
                    return;

                VertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), Vertices.Length,
                    device, Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);
                VertexBuffer.SetData(Vertices, 0, LockFlags.None);

                IndexBuffer = new IndexBuffer(typeof(int), Triangles.Length, device, Usage.WriteOnly, Pool.Default);
                IndexBuffer.SetData(Triangles, 0, LockFlags.None);
            }

            public void Release()
            {
                if (VertexBuffer != null)
                {
                    VertexBuffer.Dispose();
                    VertexBuffer = null;
                }
                if (IndexBuffer != null)
                {
                    IndexBuffer.Dispose();
                    IndexBuffer = null;
                }
            }
        }

        /// <summary>
        /// Corners and triangles of one sheet of water, in metres east and north of the middle of it.
        /// A corner two triangles share is one corner and not two, or the waves of the two would not
        /// meet along the side they have in common.
        /// </summary>
        /// <remarks>
        /// A grid of squares is laid over the outline, and every square the water reaches is cut into
        /// two triangles. The squares are all of one size, so the triangles are too: a wave finds the
        /// same room to run over on every one of them, which is what the outline of a polygon cut up
        /// on its own could never give, its triangles coming out long and thin and of every size.
        ///
        /// Along the shore a square reaches past the outline, and the corners of it that lie outside
        /// are drawn onto the outline itself, onto the nearest point of it, so the edge of the water
        /// follows the shore instead of running in steps.
        /// </remarks>
        private sealed class MeshBuilder
        {
            private readonly Dictionary<long, int> _lookup = new Dictionary<long, int>();
            private readonly List<double> _east = new List<double>();
            private readonly List<double> _north = new List<double>();
            private readonly List<int> _indices = new List<int>();
            private readonly List<double> _crossings = new List<double>();
            private readonly double _edge;
            private readonly int _most;

            public MeshBuilder(double edge, int most)
            {
                _edge = edge;
                _most = most;
            }

            public int VertexCount
            {
                get { return _east.Count; }
            }

            public int IndexCount
            {
                get { return _indices.Count; }
            }

            public int[] Indices
            {
                get { return _indices.ToArray(); }
            }

            public void Corner(int at, out double east, out double north)
            {
                east = _east[at];
                north = _north[at];
            }

            /// <summary>
            /// Lays the grid over the outline, given in metres east and north of the middle of it, and
            /// cuts every square the water reaches into two triangles.
            /// </summary>
            public void Fill(double[] outlineEast, double[] outlineNorth)
            {
                int corners = outlineEast.Length;
                if (corners < 3)
                    return;

                double minEast = outlineEast[0];
                double maxEast = outlineEast[0];
                double minNorth = outlineNorth[0];
                double maxNorth = outlineNorth[0];

                for (int i = 1; i < corners; i++)
                {
                    if (outlineEast[i] < minEast) minEast = outlineEast[i];
                    if (outlineEast[i] > maxEast) maxEast = outlineEast[i];
                    if (outlineNorth[i] < minNorth) minNorth = outlineNorth[i];
                    if (outlineNorth[i] > maxNorth) maxNorth = outlineNorth[i];
                }

                double wide = maxEast - minEast;
                double high = maxNorth - minNorth;
                if (wide <= 0.0 || high <= 0.0)
                    return;

                // A square is as small as a wave needs it to be, and never so small that the sheet
                // outgrows what it may hold: a sea would ask for more corners than the card will
                // take, so the squares grow until the water fits rather than the sheet being cut off
                // half way through.
                double step = Math.Max(_edge, Math.Sqrt((wide * high) / _most));

                int columns = (int)(wide / step) + 1;
                int rows = (int)(high / step) + 1;
                if (columns < 1)
                    columns = 1;
                if (rows < 1)
                    rows = 1;

                // Which corners of the grid lie in the water, one row at a time. Where the line of a
                // row crosses the outline is worked out once for the whole row, and a corner is in
                // the water when an odd number of crossings lie to the west of it.
                var wet = new bool[(columns + 1) * (rows + 1)];
                for (int j = 0; j <= rows; j++)
                {
                    double north = minNorth + j * step;
                    Crossings(outlineEast, outlineNorth, north, _crossings);

                    for (int i = 0; i <= columns; i++)
                    {
                        wet[j * (columns + 1) + i] = Inside(minEast + i * step);
                    }
                }

                for (int j = 0; j < rows; j++)
                {
                    for (int i = 0; i < columns; i++)
                    {
                        if (_east.Count + 4 > _most)
                            return;

                        int at = j * (columns + 1) + i;
                        bool reaches = wet[at] || wet[at + 1] || wet[at + columns + 1]
                            || wet[at + columns + 2];

                        // A square none of whose corners is in the water can still be crossed by a
                        // strip of it too narrow to hold a corner, so the middle of the square is
                        // asked as well.
                        if (!reaches)
                        {
                            Crossings(outlineEast, outlineNorth, minNorth + (j + 0.5) * step, _crossings);
                            if (!Inside(minEast + (i + 0.5) * step))
                                continue;
                        }

                        double west = minEast + i * step;
                        double south = minNorth + j * step;

                        // The two corners of the square that lie outside the water are drawn onto the
                        // outline, and a corner that is in it stays where the grid put it.
                        int corner00 = Place(outlineEast, outlineNorth, west, south, wet[at]);
                        int corner10 = Place(outlineEast, outlineNorth, west + step, south, wet[at + 1]);
                        int corner11 = Place(outlineEast, outlineNorth, west + step, south + step,
                            wet[at + columns + 2]);
                        int corner01 = Place(outlineEast, outlineNorth, west, south + step,
                            wet[at + columns + 1]);

                        // The square is cut along one of its two diagonals, and which one it is
                        // changes from square to square: cut the same way all over, the sheet is
                        // stiffer across the diagonal than along it, and the water is seen to lean.
                        if (((i + j) % 2) == 0)
                        {
                            Add(corner00, corner10, corner11);
                            Add(corner00, corner11, corner01);
                        }
                        else
                        {
                            Add(corner10, corner11, corner01);
                            Add(corner10, corner01, corner00);
                        }
                    }
                }
            }

            /// <summary>
            /// One corner of the sheet: the corner of the square it is, or the nearest point of the
            /// outline when the square reaches past the water there.
            /// </summary>
            private int Place(double[] outlineEast, double[] outlineNorth, double east, double north,
                bool wet)
            {
                if (wet)
                    return Corner(east, north);

                double onEast = east;
                double onNorth = north;
                Nearest(outlineEast, outlineNorth, east, north, ref onEast, ref onNorth);
                return Corner(onEast, onNorth);
            }

            private void Add(int a, int b, int c)
            {
                _indices.Add(a);
                _indices.Add(b);
                _indices.Add(c);
            }

            private int Corner(double east, double north)
            {
                // A hundredth of a metre apart, which is closer than two corners of a lake ever come
                // on purpose.
                long key = unchecked(((long)Math.Round(east * 100.0) << 32)
                    | ((long)Math.Round(north * 100.0) & 0xffffffffL));

                int at;
                if (_lookup.TryGetValue(key, out at))
                    return at;

                at = _east.Count;
                _lookup.Add(key, at);
                _east.Add(east);
                _north.Add(north);
                return at;
            }

            /// <summary>
            /// Where a line running east and west at this north crosses the outline. The crossings of
            /// one row are what tells whether a corner of the grid lies in the water.
            /// </summary>
            private void Crossings(double[] outlineEast, double[] outlineNorth, double north,
                List<double> at)
            {
                at.Clear();

                int corners = outlineEast.Length;
                for (int i = 0; i < corners; i++)
                {
                    int next = (i + 1) % corners;
                    double south = outlineNorth[i];
                    double over = outlineNorth[next];

                    // A corner the line runs through is counted on one side of it only, and a side
                    // lying along the line not at all, or the count on the far side would come out
                    // odd where it should be even.
                    if ((south <= north && over > north) || (over <= north && south > north))
                    {
                        double part = (north - south) / (over - south);
                        at.Add(outlineEast[i] + part * (outlineEast[next] - outlineEast[i]));
                    }
                }

                at.Sort();
            }

            /// <summary>
            /// Whether an odd number of the crossings of the row, which Crossings filled, lie to the
            /// west of this east. Odd means the point is in the water.
            /// </summary>
            private bool Inside(double east)
            {
                int west = 0;
                for (int i = 0; i < _crossings.Count; i++)
                {
                    if (_crossings[i] < east)
                        west++;
                }

                return (west % 2) == 1;
            }

            /// <summary>
            /// The point of the outline nearest to one outside the water, which is where a corner of
            /// the grid is drawn to when the square it belongs to reaches past the shore.
            /// </summary>
            private static void Nearest(double[] outlineEast, double[] outlineNorth, double east,
                double north, ref double onEast, ref double onNorth)
            {
                int corners = outlineEast.Length;
                double closest = double.MaxValue;

                for (int i = 0; i < corners; i++)
                {
                    int next = (i + 1) % corners;

                    double ax = outlineEast[i];
                    double ay = outlineNorth[i];
                    double sideEast = outlineEast[next] - ax;
                    double sideNorth = outlineNorth[next] - ay;

                    double length = sideEast * sideEast + sideNorth * sideNorth;
                    double part = length > 0.0
                        ? ((east - ax) * sideEast + (north - ay) * sideNorth) / length
                        : 0.0;

                    if (part < 0.0)
                        part = 0.0;
                    else if (part > 1.0)
                        part = 1.0;

                    double hereEast = ax + part * sideEast;
                    double hereNorth = ay + part * sideNorth;

                    double away = (hereEast - east) * (hereEast - east)
                        + (hereNorth - north) * (hereNorth - north);

                    if (away < closest)
                    {
                        closest = away;
                        onEast = hereEast;
                        onNorth = hereNorth;
                    }
                }
            }
        }
    }
}
