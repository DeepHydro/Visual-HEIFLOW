using DotSpatial.Data;
using DotSpatial.Projections;
using Heiflow.Core;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable
{
    public class RenderableLine : RenderableShp
    {
        public RenderableLine(string filename, World parentWorld, System.Drawing.Color lineColor)
            : base("Line vector", parentWorld)
        {
            _Filename = filename;
            m_world = parentWorld;
            _FeatureColor = lineColor;
        }

     //   protected CustomVertex.PositionColored []_MyVertexList;
        private int _nLine;

        public override void UpdateVertices()
        {
            if (isInitialized)
                return;

            string elev_file = _Filename + ".elv";
            FileStream fs_elv = null;
            BinaryReader br_elv = null;
            if (File.Exists(elev_file))
            {
                fs_elv = new FileStream(elev_file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                br_elv = new BinaryReader(fs_elv);
            }

            _IFeatureSet = FeatureSet.Open(_Filename);
            var nshpx = _IFeatureSet.ShapeIndices.Count;
            var vertices = _IFeatureSet.Vertex;
            int linecolor = _FeatureColor.ToArgb();
            double y = 0;
            double x = 0;
            int nshp = 0;
            float terrainHeight = 0;
            double[] xy = new double[2];
            double[] z = new double[1];

            if (br_elv != null)
            {
                nshp = br_elv.ReadInt32();
            }

            int nIndex = 0;
            for (int n = 0; n < nshpx; n++)
            {
                var shpx = _IFeatureSet.ShapeIndices[n];
                int start = shpx.Parts[0].StartIndex;
                int end = shpx.Parts[shpx.Parts.Count - 1].EndIndex;
                int npt = end - start + 1;
                nIndex += (npt - 1) * 2;
            }
            _VertexList = new CustomVertex.PositionNormalColored[vertices.Length / 2];
            _VertexIndexList = new int[nIndex];

            for (int n = 0; n < nshpx; n++)
            {
                var shpx = _IFeatureSet.ShapeIndices[n];
                int start = shpx.Parts[0].StartIndex;
                int end = shpx.Parts[shpx.Parts.Count - 1].EndIndex;
                int npt = end - start + 1;
                if (br_elv != null)
                {
                    nshp = br_elv.ReadInt32();
                }
                for (int i = start; i <= end; i++)
                {
                    x = vertices[2 * i];
                    y = vertices[2 * i + 1];

                    if (!_IFeatureSet.Projection.IsLatLon)
                    {
                        xy[0] = x;
                        xy[1] = y;
                        Reproject.ReprojectPoints(xy, z, _IFeatureSet.Projection, KnownCoordinateSystems.Geographic.World.WGS1984, 0, 1);
                        x = xy[0];
                        y = xy[1];
                    }
                    if (br_elv != null)
                    {
                        terrainHeight = br_elv.ReadSingle();
                    }
                    else
                    {
                        if (AutoTerrainHeight && World.TerrainAccessor != null)
                        {
                            terrainHeight = World.TerrainAccessor.GetElevationAt(y, x);// (100.0 / DrawArgs.Camera.ViewRange.Degrees));
                        }
                    }

                    Vector3 xyzVertex = MathEngine.SphericalToCartesian(y, x,
                        World.Settings.VerticalExaggeration * (DistanceAboveSurface + terrainHeight) + World.EquatorialRadius
                        );

                    _VertexList[i].X = xyzVertex.X;
                    _VertexList[i].Y = xyzVertex.Y;
                    _VertexList[i].Z = xyzVertex.Z;
                    _VertexList[i].Color = linecolor;
                }
            }
            int t=0;
            for (int n = 0; n < nshpx; n++)
            {
                var shpx = _IFeatureSet.ShapeIndices[n];
                int start = shpx.Parts[0].StartIndex;
                int end = shpx.Parts[shpx.Parts.Count - 1].EndIndex;
                int npt = end - start + 1;
                for (int i = start; i < end; i++)
                {
                    _VertexIndexList[2 * t] = i;
                    _VertexIndexList[2 * t + 1] = i + 1;
                    t++;
                }
            }
            if (br_elv != null)
            {
                br_elv.Close();
                fs_elv.Close();
            }

            _nLine = nIndex / 2;
            DataTable = _IFeatureSet.DataTable;
            _IFeatureSet.Close();
            NeedsUpdate = false;
            isInitialized = true;
        }

        public override void Render(DrawArgs drawArgs)
        {
            if (_VertexList == null)
                return;

            if (!isInitialized || drawArgs.WorldCamera.Altitude < MinDisplayAltitude || drawArgs.WorldCamera.Altitude > MaxDisplayAltitude )
            {
                return;
            }
            try
            {
                Device device = DrawArgs.Device;
                if (!World.Settings.EnableHighPerfomance)
                    device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);
                device.RenderState.ZBufferEnable = true;
                device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                device.TextureState[0].ColorOperation = TextureOperation.Disable;

                device.Transform.World = Matrix.Translation(
                (float)(-drawArgs.WorldCamera.ReferenceCenter.X),
                (float)(-drawArgs.WorldCamera.ReferenceCenter.Y),
                (float)(-drawArgs.WorldCamera.ReferenceCenter.Z)
                );

                if (World.Settings.EnableSunShading)
                {
                    Point3d sunPosition = SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);
                    Vector3 sunVector = new Vector3(
                        (float)sunPosition.X,
                        (float)sunPosition.Y,
                        (float)sunPosition.Z);

                    device.RenderState.Lighting = true;
                    Material material = new Material();
                    material.Diffuse = System.Drawing.Color.White;
                    material.Ambient = System.Drawing.Color.White;

                    device.Material = material;
                    device.RenderState.AmbientColor = World.Settings.ShadingAmbientColor.ToArgb();
                    device.RenderState.NormalizeNormals = true;
                    device.RenderState.AlphaBlendEnable = true;

                    device.Lights[0].Enabled = true;
                    device.Lights[0].Type = LightType.Directional;
                    device.Lights[0].Diffuse = System.Drawing.Color.White;
                    device.Lights[0].Direction = sunVector;
                }
                else
                {
                    device.RenderState.Lighting = false;
                    device.RenderState.Ambient = World.Settings.StandardAmbientColor;
                }

                Cull currentCull = drawArgs.device.RenderState.CullMode;
                drawArgs.device.RenderState.CullMode = Cull.Clockwise;
                FillMode fillmode = device.RenderState.FillMode;
                device.RenderState.FillMode =  Microsoft.DirectX.Direct3D.FillMode.Solid;

                drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, _VertexList.Length, _nLine,_VertexIndexList, false, _VertexList);

                device.RenderState.FillMode = fillmode;
                drawArgs.device.RenderState.CullMode = currentCull;
                drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    
    }
}
