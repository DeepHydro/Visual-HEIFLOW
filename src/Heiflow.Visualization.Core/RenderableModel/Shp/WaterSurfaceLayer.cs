using DotSpatial.Data;
using DotSpatial.Projections;
using GeoAPI.Geometries;
using Heiflow.Core;
using Heiflow.Visualization.Renderable.Grid;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Core;
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
    /// of them, so every triangle the outline is cut into is split into four again and again until
    /// its sides are short enough. The corners the splitting makes are shared through a lookup, or
    /// the waves of two triangles next to each other would not meet and the sheet would come apart
    /// along the seams.
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
        /// How often a triangle may be split in two along its sides, and how many corners the whole
        /// sheet may hold. Both are here so that a long coastline cannot ask for the whole memory of
        /// the machine.
        /// </summary>
        private const int MaxDepth = 9;
        private const int MaxVertices = 90000;

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
        /// How long the side of a triangle may be, in metres. A wave needs a few triangles to run
        /// over, so it follows from the length of one, and it is what keeps the sheet small enough.
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
                    int start = shape.Parts[0].StartIndex;
                    int end = shape.Parts[shape.Parts.Count - 1].EndIndex;
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

                    var body = BuildBody(ring, LevelOf(n));
                    if (body != null)
                        _Bodies.Add(body);
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

            // The outline is cut into triangles where it lies, and each of those is cut into four
            // again, in metres east and north of the middle of the polygon.
            var triangles = new TriPolygon(ring).Triangulate();
            double eastOfLongitude = MetreOfLongitude * Math.Cos(MathEngine.DegreesToRadians(lat0));

            foreach (var triangle in triangles)
            {
                var corners = triangle.Points;
                mesh.Split(
                    (corners[0].X - lon0) * eastOfLongitude, (corners[0].Y - lat0) * MetreOfLatitude,
                    (corners[1].X - lon0) * eastOfLongitude, (corners[1].Y - lat0) * MetreOfLatitude,
                    (corners[2].X - lon0) * eastOfLongitude, (corners[2].Y - lat0) * MetreOfLatitude,
                    0);
            }

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
                    parameters.SunDirection = ModelRenderScope.SunVector;

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
        private sealed class MeshBuilder
        {
            private readonly Dictionary<long, int> _lookup = new Dictionary<long, int>();
            private readonly List<double> _east = new List<double>();
            private readonly List<double> _north = new List<double>();
            private readonly List<int> _indices = new List<int>();
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
            /// Cuts a triangle into four while its sides are longer than a wave needs, and keeps it
            /// as it is when they are not, or when the sheet has grown as big as it may.
            /// </summary>
            public void Split(double ax, double ay, double bx, double by, double cx, double cy, int depth)
            {
                if (depth >= MaxDepth || _east.Count + 3 > _most || LongestSide(ax, ay, bx, by, cx, cy) <= _edge)
                {
                    Add(ax, ay, bx, by, cx, cy);
                    return;
                }

                double abx = (ax + bx) * 0.5;
                double aby = (ay + by) * 0.5;
                double bcx = (bx + cx) * 0.5;
                double bcy = (by + cy) * 0.5;
                double cax = (cx + ax) * 0.5;
                double cay = (cy + ay) * 0.5;

                // The four triangles keep the way the one they came from turns, so the sheet is lit
                // from the same side all over.
                Split(ax, ay, abx, aby, cax, cay, depth + 1);
                Split(abx, aby, bx, by, bcx, bcy, depth + 1);
                Split(cax, cay, bcx, bcy, cx, cy, depth + 1);
                Split(abx, aby, bcx, bcy, cax, cay, depth + 1);
            }

            private void Add(double ax, double ay, double bx, double by, double cx, double cy)
            {
                _indices.Add(Corner(ax, ay));
                _indices.Add(Corner(bx, by));
                _indices.Add(Corner(cx, cy));
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

            private static double LongestSide(double ax, double ay, double bx, double by, double cx, double cy)
            {
                double ab = (bx - ax) * (bx - ax) + (by - ay) * (by - ay);
                double bc = (cx - bx) * (cx - bx) + (cy - by) * (cy - by);
                double ca = (ax - cx) * (ax - cx) + (ay - cy) * (ay - cy);
                return Math.Sqrt(Math.Max(ab, Math.Max(bc, ca)));
            }
        }
    }
}
