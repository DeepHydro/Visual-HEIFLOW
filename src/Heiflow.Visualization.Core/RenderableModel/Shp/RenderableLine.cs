using DotSpatial.Data;
using DotSpatial.Projections;
using Heiflow.Core;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

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
                device.SetRenderState(RenderState.ZEnable, true);
                device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Disable);

                device.SetTransform(TransformState.World, Matrix.Translation(
                (float)(-drawArgs.WorldCamera.ReferenceCenter.X),
                (float)(-drawArgs.WorldCamera.ReferenceCenter.Y),
                (float)(-drawArgs.WorldCamera.ReferenceCenter.Z)
                ));

                if (World.Settings.EnableSunShading)
                {
                    Point3d sunPosition = SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);
                    Vector3 sunVector = new Vector3(
                        (float)sunPosition.X,
                        (float)sunPosition.Y,
                        (float)sunPosition.Z);

                    device.SetRenderState(RenderState.Lighting, true);
                    Material material = new Material();
                    material.Diffuse = ColorUtil.ToColor4(System.Drawing.Color.White);
                    material.Ambient = ColorUtil.ToColor4(System.Drawing.Color.White);

                    device.Material = material;
                    device.SetRenderState(RenderState.Ambient, World.Settings.ShadingAmbientColor.ToArgb());
                    device.SetRenderState(RenderState.NormalizeNormals, true);
                    device.SetRenderState(RenderState.AlphaBlendEnable, true);

                    LightHub.Get(device, 0).Enabled = true;
                    LightHub.Get(device, 0).Type = LightType.Directional;
                    LightHub.Get(device, 0).Diffuse = ColorUtil.ToColor4(System.Drawing.Color.White);
                    LightHub.Get(device, 0).Direction = sunVector;
                }
                else
                {
                    device.SetRenderState(RenderState.Lighting, false);
                    device.SetRenderState(RenderState.Ambient, World.Settings.StandardAmbientColor.ToArgb());
                }

                Cull currentCull = drawArgs.device.GetRenderStateEnum<Cull>(RenderState.CullMode);
                drawArgs.device.SetRenderState(RenderState.CullMode, (int)(Cull.Clockwise));
                FillMode fillmode = device.GetRenderStateEnum<FillMode>(RenderState.FillMode);
                device.SetRenderState(RenderState.FillMode, FillMode.Solid);

                drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, _VertexList.Length, _nLine,_VertexIndexList, Format.Index32, _VertexList);

                device.SetRenderState(RenderState.FillMode, fillmode);
                drawArgs.device.SetRenderState(RenderState.CullMode, currentCull);
                drawArgs.device.SetTransform(TransformState.World, drawArgs.WorldCamera.WorldMatrix);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    
    }
}
