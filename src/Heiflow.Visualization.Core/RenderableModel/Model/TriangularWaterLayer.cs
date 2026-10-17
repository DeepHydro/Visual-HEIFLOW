using Heiflow.Core;
using Heiflow.Models.Generic;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Heiflow.Visualization.Renderable.Grid
{
    /// <summary>
    /// The triangular grid of a model drawn once again as a sheet of water, see Shaders\Water.fx.
    /// </summary>
    /// <remarks>
    /// It is a copy of the grid and not the grid itself: the geometry is read out of the model once
    /// and kept here as corners of its own, in the form the water shader wants them, so that the
    /// mesh the model is drawn with is left alone. The two are then two layers where there was one,
    /// the grid as it always was and the water over it, and neither gets in the way of the other.
    ///
    /// The water stands a set depth above the bed, the bed being the height the grid holds for every
    /// vertex of it, so the sheet follows the ground under it rather than lying flat.
    ///
    /// The grid has no shape the texture could be laid out in, its vertices being only numbered, so
    /// metres east and north of the middle of it are worked out for every one of them, the same way
    /// WaterSurfaceLayer does for a polygon, and a wave is then the same size all over the grid.
    /// </remarks>
    public class TriangularWaterLayer : RenderableObject
    {
        /// <summary>
        /// Metres of latitude, and of longitude at the equator. Only the middle of the grid is
        /// measured with them, and it is close enough over one model.
        /// </summary>
        private const double MetreOfLatitude = 110540.0;
        private const double MetreOfLongitude = 111320.0;

        private readonly WaterShader _Shader = new WaterShader();
        private readonly TriangularGrid _Grid;
        private readonly DateTime _Started = DateTime.UtcNow;

        private CustomVertex.PositionNormalTextured[] _Vertices;
        private int[] _Indices;
        private VertexBuffer _VertexBuffer;
        private IndexBuffer _IndexBuffer;
        private Vector3 _East;
        private Vector3 _North;

        /// <summary>
        /// The depth the corners now in hand were made for. The height of the water is in the corners
        /// themselves, so a depth that has been changed is seen only after they are made again.
        /// </summary>
        private double _BuiltDepth;
        private bool _DepthBuilt;

        /// <summary>
        /// Set when the grid gave nothing to draw, so that the layer does not ask again every frame
        /// and say the same thing in the log a hundred times over.
        /// </summary>
        private bool _Failed;

        public TriangularWaterLayer(string name, World parentWorld, TriangularGrid grid)
            : base(name, parentWorld)
        {
            _Grid = grid;

            Depth = 1.0;
            WaterOpacity = 100.0;
            WaveHeight = 0.6;
            WaveLength = 40.0;
            WaveSpeed = 1.0;
            ShallowColor = Color.FromArgb(97, 168, 194);
            DeepColor = Color.FromArgb(13, 54, 97);
            SkyColor = Color.FromArgb(148, 189, 224);
        }

        /// <summary>
        /// Metres of water over the bed, the bed being the height the grid holds for a vertex. The
        /// water is level with the grid at nothing, and then the two lie one over the other.
        /// </summary>
        public double Depth
        {
            get;
            set;
        }

        /// <summary>
        /// How solid the water is drawn, in per cent: a hundred hides the bed under it, less lets the
        /// bed show through, and nothing at all is no water to be seen.
        /// </summary>
        /// <remarks>
        /// The layer inherits an Opacity of its own, but it is kept out of the property panel and it
        /// is a number of two hundred and fifty five, so this one is put beside the other settings of
        /// the water instead.
        /// </remarks>
        [Category("Water")]
        [Description("How solid the water is drawn. 100 hides the bed, 0 is invisible.")]
        public double WaterOpacity
        {
            get;
            set;
        }

        /// <summary>
        /// Metres from a trough to a crest.
        /// </summary>
        [Category("Water")]
        public double WaveHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Metres between two crests.
        /// </summary>
        [Category("Water")]
        public double WaveLength
        {
            get;
            set;
        }

        /// <summary>
        /// How fast a crest runs.
        /// </summary>
        [Category("Water")]
        public double WaveSpeed
        {
            get;
            set;
        }

        /// <summary>
        /// The colour of a crest, and of the water where it is shallow.
        /// </summary>
        [Category("Water")]
        public Color ShallowColor
        {
            get;
            set;
        }

        /// <summary>
        /// The colour of a trough, and of the water where it is deep.
        /// </summary>
        [Category("Water")]
        public Color DeepColor
        {
            get;
            set;
        }

        /// <summary>
        /// The colour of the sky, which the water takes where it lies flat to the eye.
        /// </summary>
        [Category("Water")]
        public Color SkyColor
        {
            get;
            set;
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            Build();
            isInitialized = true;
        }

        public override void Update(DrawArgs drawArgs)
        {
            // A layer put into a tree that has already been gone over is never initialized again:
            // RenderableObjectList initializes its children once and Render and Update of a child
            // that came in afterwards are called without it. The grid is therefore read out here as
            // well, the first time it is asked for, and not only in Initialize.
            //
            // It is read out again whenever the depth has been changed, the height of the water being
            // in the corners themselves, and not at all once the grid has been found wanting, which
            // would otherwise be tried and said in the log once for every frame.
            if (_Failed)
                return;

            if (!_DepthBuilt || _BuiltDepth != Depth)
            {
                _Vertices = null;
                _Indices = null;
                Release();
                Build();
            }
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }

        /// <summary>
        /// Reads the grid out into corners and triangles of this layer's own. It is done once, and
        /// again after the card has been lost, see MakeBuffers.
        /// </summary>
        private void Build()
        {
            if (_Vertices != null)
                return;

            // A layer that draws nothing has always been the hardest thing to find out about, so
            // every way of leaving this method without a mesh in it says which one it was.
            if (_Grid == null)
            {
                Log.Write(Log.Levels.Warning, "WATR", "No grid was given to draw as water.");
                _Failed = true;
                return;
            }

            try
            {
                var vertex = _Grid.Vertex;
                int vertexCount = _Grid.VertexCount;
                if (vertex == null || vertexCount <= 0 || vertex.Length < vertexCount)
                {
                    Log.Write(Log.Levels.Warning, "WATR", "The grid holds no vertices to draw as"
                        + " water: it says " + vertexCount + ", and Vertex is "
                        + (vertex == null ? "null" : vertex.Length + " long") + ".");
                    _Failed = true;
                    return;
                }

                var topology = _Grid.Topology;
                if (topology == null || topology.VertexIndices == null)
                {
                    Log.Write(Log.Levels.Warning, "WATR", "The grid holds " + vertexCount
                        + " vertices but no topology, so there is no triangle to draw as water.");
                    _Failed = true;
                    return;
                }

                double lon0 = 0.0;
                double lat0 = 0.0;
                double bed0 = 0.0;
                for (int i = 0; i < vertexCount; i++)
                {
                    lon0 += vertex[i].X;
                    lat0 += vertex[i].Y;
                    bed0 += vertex[i].Z;
                }
                lon0 /= vertexCount;
                lat0 /= vertexCount;
                bed0 /= vertexCount;

                double eastOfLongitude = MetreOfLongitude * Math.Cos(MathEngine.DegreesToRadians(lat0));
                float exaggeration = World.Settings.VerticalExaggeration;

                var list = new CustomVertex.PositionNormalTextured[vertexCount];
                for (int i = 0; i < vertexCount; i++)
                {
                    double lat = vertex[i].Y;
                    double lon = vertex[i].X;
                    double level = vertex[i].Z + Depth;
                    double radius = World.EquatorialRadius + exaggeration * (level + DistanceAboveSurface);

                    var position = MathEngine.SphericalToCartesian(lat, lon, radius);
                    var up = Vector3.Normalize(position);

                    list[i].X = position.X;
                    list[i].Y = position.Y;
                    list[i].Z = position.Z;
                    list[i].Nx = up.X;
                    list[i].Ny = up.Y;
                    list[i].Nz = up.Z;
                    list[i].Tu = (float)((lon - lon0) * eastOfLongitude);
                    list[i].Tv = (float)((lat - lat0) * MetreOfLatitude);
                }

                int cells = Math.Min(_Grid.ActiveCellCount, topology.VertexIndices.Length);

                var indices = new int[cells * 3];
                for (int i = 0; i < cells; i++)
                {
                    int id0 = (int)topology.VertexIndices[i][0];
                    int id1 = (int)topology.VertexIndices[i][1];
                    int id2 = (int)topology.VertexIndices[i][2];

                    // One triangle that names a vertex the grid does not have takes the whole draw
                    // call with it, the card throwing away everything it was handed rather than
                    // reading past the end of the buffer. Such a one is made into a triangle of no
                    // area, which is drawn as nothing at all.
                    if (id0 < 0 || id0 >= vertexCount || id1 < 0 || id1 >= vertexCount
                        || id2 < 0 || id2 >= vertexCount)
                    {
                        continue;
                    }

                    indices[i * 3] = id0;
                    indices[i * 3 + 1] = id1;
                    indices[i * 3 + 2] = id2;
                }

                // Which way is east and which is north where this water lies. The shader is told, so
                // it knows which way a wave leans from the two metres it is given.
                double step = 0.001;
                double radius0 = World.EquatorialRadius + exaggeration * (bed0 + Depth + DistanceAboveSurface);
                var middle = MathEngine.SphericalToCartesian(lat0, lon0, radius0);
                var toEast = MathEngine.SphericalToCartesian(lat0, lon0 + step, radius0);
                var toNorth = MathEngine.SphericalToCartesian(lat0 + step, lon0, radius0);
                _East = Vector3.Normalize(toEast - middle);
                _North = Vector3.Normalize(toNorth - middle);

                _Vertices = list;
                _Indices = indices;
                _BuiltDepth = Depth;
                _DepthBuilt = true;
                _Failed = false;

                Log.Write(Log.Levels.Warning, "WATR", "Water over the grid: " + vertexCount
                    + " vertices and " + cells + " triangles, the water standing "
                    + Depth.ToString("0.##") + " m over the bed.");
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                _Failed = true;
            }
        }

        /// <summary>
        /// The buffers are made here and not where the grid is read, or a layer of a project that is
        /// loaded before the card is up would have no water in it.
        /// </summary>
        private void MakeBuffers(Device device)
        {
            if (device == null || _Vertices == null || _Indices == null)
                return;

            if (_VertexBuffer != null && _IndexBuffer != null)
                return;

            try
            {
                _VertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), _Vertices.Length,
                    device, Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);
                _VertexBuffer.SetData(_Vertices, 0, LockFlags.None);

                _IndexBuffer = new IndexBuffer(typeof(int), _Indices.Length, device, Usage.WriteOnly, Pool.Default);
                _IndexBuffer.SetData(_Indices, 0, LockFlags.None);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                Release();
            }
        }

        private void Release()
        {
            if (_VertexBuffer != null)
            {
                _VertexBuffer.Dispose();
                _VertexBuffer = null;
            }
            if (_IndexBuffer != null)
            {
                _IndexBuffer.Dispose();
                _IndexBuffer = null;
            }
        }

        public override void Render(DrawArgs drawArgs)
        {
            // See Update: the grid is read out here too, for a layer that the tree never gave an
            // Initialize to, and for one whose Update is not called before its Render.
            if (!_Failed && !_DepthBuilt)
                Build();

            if (!IsOn || _Vertices == null || _Indices == null || _Indices.Length < 3)
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
                    MakeBuffers(device);
                    if (_VertexBuffer == null || _IndexBuffer == null)
                        return;

                    _Shader.Ensure(device);

                    var parameters = new WaterParameters();
                    parameters.WorldViewProjection = Matrix.Multiply(device.Transform.World,
                        Matrix.Multiply(device.Transform.View, device.Transform.Projection));
                    parameters.EyePosition = drawArgs.WorldCamera.Position;
                    // Where the sun stands is given as a place in the sky, and the shader wants no
                    // more than which way to look for it. It is straightened here, once for the
                    // frame, the pixel shader having no room to do it for every pixel.
                    parameters.SunDirection = Vector3.Normalize(ModelRenderScope.SunVector);

                    // A float runs out of the seconds it can tell apart, so the clock of the water
                    // is taken round once an hour. Nothing of the waves shows the seam.
                    parameters.Time = (float)((DateTime.UtcNow - _Started).TotalSeconds % 3600.0);
                    parameters.WaveHeight = (float)WaveHeight;
                    parameters.WaveLength = (float)Math.Max(WaveLength, 1.0);
                    parameters.WaveSpeed = (float)WaveSpeed;
                    // The shader takes nothing to one, and it is told how solid the water is meant to
                    // be whatever the number has been set to.
                    parameters.Opacity = (float)(Solidity() / 100.0);
                    parameters.ShallowColor = FloatsOf(ShallowColor);
                    parameters.DeepColor = FloatsOf(DeepColor);
                    parameters.SkyColor = FloatsOf(SkyColor);
                    parameters.East = _East;
                    parameters.North = _North;

                    if (!_Shader.Draw(device, _VertexBuffer, _IndexBuffer,
                        _Vertices.Length, _Indices.Length, parameters))
                    {
                        DrawFlat(device);
                    }
                }
                catch (DeviceLostException)
                {
                    // The buffers go with the card, and both are made again on the next frame.
                    Release();
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
        private void DrawFlat(Device device)
        {
            bool lighting = device.RenderState.Lighting;
            Color ambient = device.RenderState.Ambient;

            var material = new Material();
            material.Diffuse = Faded(ShallowColor);
            material.Ambient = Faded(ShallowColor);

            device.RenderState.Lighting = true;
            device.RenderState.Ambient = Color.White;
            device.Material = material;
            device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            device.SetStreamSource(0, _VertexBuffer, 0, CustomVertex.PositionNormalTextured.StrideSize);
            device.Indices = _IndexBuffer;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _Vertices.Length, 0,
                _Indices.Length / 3);
            device.Indices = null;

            device.RenderState.Ambient = ambient;
            device.RenderState.Lighting = lighting;
        }

        private static float[] FloatsOf(Color color)
        {
            return new[] { color.R / 255f, color.G / 255f, color.B / 255f, 1f };
        }

        /// <summary>
        /// How solid the water is to be drawn, in per cent, the number held between nothing and a
        /// hundred whatever it has been set to.
        /// </summary>
        private double Solidity()
        {
            return Math.Max(0.0, Math.Min(100.0, WaterOpacity));
        }

        /// <summary>
        /// The colour of the water thinned by how solid it is to be drawn, for a card that draws it
        /// without the shader and so has no mind for an opacity of its own.
        /// </summary>
        private Color Faded(Color color)
        {
            return Color.FromArgb((int)(Solidity() * 2.55), color);
        }

        public override void Dispose()
        {
            Release();
            _Shader.Dispose();
        }
    }
}
