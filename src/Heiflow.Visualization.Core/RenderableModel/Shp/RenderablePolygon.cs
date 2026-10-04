using DotSpatial.Data;
using DotSpatial.Projections;
using GeoAPI.Geometries;
using Heiflow.Core;
using Heiflow.Visualization.Geometry.Triangulation;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Core;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using ProjNet.CoordinateSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable
{
    public class RenderablePolygon : RenderableShp
    {
        public RenderablePolygon(string filename, World parentWorld, System.Drawing.Color polygonColor)
            : base("Polygon vector", parentWorld)
        {
            _Filename = filename;
            m_world = parentWorld;
            _FeatureColor = polygonColor;
        }
        protected CustomVertex.PositionColored[][] _Vertices = null;
        protected int[][] _VertexIndexLists = null;

        public override void UpdateVertices()
        {
            if (isInitialized)
                return;

            _IFeatureSet = FeatureSet.Open(_Filename);
            var nshpx = _IFeatureSet.ShapeIndices.Count;
            _Vertices = new CustomVertex.PositionColored[nshpx][];
            _VertexIndexLists = new int[nshpx][];
            var vertices = _IFeatureSet.Vertex;
            int linecolor = _FeatureColor.ToArgb();
            int color = _FeatureColor.ToArgb();

            if (_IFeatureSet.Projection.IsLatLon)
            {
                for (int n = 0; n < nshpx; n++)
                {
                    var shpx = _IFeatureSet.ShapeIndices[n];
                    int start = shpx.Parts[0].StartIndex;
                    int end = shpx.Parts[shpx.Parts.Count - 1].EndIndex;
                    int npt = end - start + 1;

                    Coordinate[] points = new Coordinate[npt];
                    for (int i = start; i <= end; i++)
                    {
                        points[i - start] = new Coordinate(vertices[2 * i], vertices[2 * i + 1]);
                    }

                    TriPolygon poly = new TriPolygon(points);
                    var trigs = poly.Triangulate();
                    int ntrg = trigs.Count;
                    _Vertices[n] = new CustomVertex.PositionColored[ntrg * 3];
                    _VertexIndexLists[n] = new int[ntrg * 3];
                    int index = 0;
                    for (int i = 0; i < ntrg; i++)
                    {
                        var pt_trg = trigs[i].Points;
                        for (int t = 0; t < 3; t++)
                        {
                            var pt = pt_trg[t];
                            double terrainHeight = 0;
                            var lat = pt.Y;
                            var lng = pt.X;
                            if (AutoTerrainHeight && World.TerrainAccessor != null)
                            {
                                terrainHeight = World.TerrainAccessor.GetElevationAt(lat, lng);
                            }
                            Vector3 xyzVertex = MathEngine.SphericalToCartesian(lat, lng,
               World.Settings.VerticalExaggeration * (DistanceAboveSurface + terrainHeight) + World.EquatorialRadius
                       );

                            _Vertices[n][i * 3 + t] = new CustomVertex.PositionColored()
                            {
                                X = xyzVertex.X,
                                Y = xyzVertex.Y,
                                Z = xyzVertex.Z,
                                Color = color
                            };
                            _VertexIndexLists[n][i * 3 + t] = index;
                            index++;
                        }
                    }
                }
            }
            else
            {
                double[] xy = new double[2];
                double[] z = new double[1];
                var wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;

                for (int n = 0; n < nshpx; n++)
                {
                    var shpx = _IFeatureSet.ShapeIndices[n];
                    int start = shpx.Parts[0].StartIndex;
                    int end = shpx.Parts[shpx.Parts.Count - 1].EndIndex;
                    int npt = end - start + 1;

                    Coordinate[] points = new Coordinate[npt];
                    for (int i = start; i <= end; i++)
                    {
                        xy[0] = vertices[2 * i];
                        xy[1] = vertices[2 * i + 1];
                        Reproject.ReprojectPoints(xy, z, _IFeatureSet.Projection, wgs84, 0, 1);
                        points[i - start] = new Coordinate(xy[0], xy[1]);
                    }

                    TriPolygon poly = new TriPolygon(points);
                    var trigs = poly.Triangulate();
                    int ntrg = trigs.Count;
                    _Vertices[n] = new CustomVertex.PositionColored[ntrg * 3];
                    _VertexIndexLists[n] = new int[ntrg * 3];
                    int index = 0;
                    for (int i = 0; i < ntrg; i++)
                    {
                        var pt_trg = trigs[i].Points;
                        for (int t = 0; t < 3; t++)
                        {
                            var pt = pt_trg[t];
                            double terrainHeight = 0;
                            var lat = pt.Y;
                            var lng = pt.X;
                            if (AutoTerrainHeight && World.TerrainAccessor != null)
                            {
                                terrainHeight = World.TerrainAccessor.GetElevationAt(lat, lng);
                            }
                            Vector3 xyzVertex = MathEngine.SphericalToCartesian(lat, lng,
               World.Settings.VerticalExaggeration * (DistanceAboveSurface + terrainHeight) + World.EquatorialRadius
                       );

                            _Vertices[n][i * 3 + t] = new CustomVertex.PositionColored()
                            {
                                X = xyzVertex.X,
                                Y = xyzVertex.Y,
                                Z = xyzVertex.Z,
                                Color = color
                            };
                            _VertexIndexLists[n][i * 3 + t] = index;
                            index++;
                        }
                    }
                }
            }

            DataTable = _IFeatureSet.DataTable;
            _IFeatureSet.Close();
            NeedsUpdate = false;
            isInitialized = true;
        }

        public override void Render(DrawArgs drawArgs)
        {
            if (!isInitialized || drawArgs.WorldCamera.Altitude < MinDisplayAltitude || drawArgs.WorldCamera.Altitude > MaxDisplayAltitude)
            {
                return;
            }
            try
            {
                Device device = DrawArgs.Device;
                if (!World.Settings.EnableHighPerfomance)
                    device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);
                device.RenderState.ZBufferEnable = true;

                Cull currentCull = drawArgs.device.RenderState.CullMode;
                drawArgs.device.RenderState.CullMode = Cull.None;

                drawArgs.device.Transform.World = Matrix.Translation(
                    (float)-drawArgs.WorldCamera.ReferenceCenter.X,
                    (float)-drawArgs.WorldCamera.ReferenceCenter.Y,
                    (float)-drawArgs.WorldCamera.ReferenceCenter.Z
                    );


                drawArgs.device.TextureState[0].ColorOperation = TextureOperation.Disable;
                drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;

                for (int n = 0; n < _Vertices.Length; n++)
                {
                    drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, _Vertices[n].Length, _VertexIndexLists[n].Length / 3,
               _VertexIndexLists[n], false, _Vertices[n]);
                }

                drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
                drawArgs.device.RenderState.CullMode = currentCull;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
