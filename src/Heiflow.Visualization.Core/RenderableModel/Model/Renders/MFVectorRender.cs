using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using System.ComponentModel;
using Heiflow.Models.Subsurface;
using Heiflow.Core.MyMath;
using Heiflow.Spatial.Geography;
using HUST.WREIS.Dot3D;
using Heiflow.Core.Data;
using Heiflow.Visualization.RenderableModel.Model.Renders;


namespace Heiflow.Visualization.Renderable.Grid
{
    public class MFVectorRender : MFGridRender, IVectorRender
    {
        protected VectorStyle _VectorStyle;
        public MFVectorRender(World world):base(world)
        {
            RequiredUpdated = true;
            _VectorStyle = new VectorStyle();
        }

        public VectorStyle Style
        {
            get
            {
                return _VectorStyle;
            }
            set
            {
                _VectorStyle = value;
            }
        }
        public bool RequiredUpdated
        {
            get;
            set;
        }

        public override void Initilize()
        {
            base.Initilize();
            _Project = LayerObject.Project;
            _MFGrid = _Project.Model.Grid as MFGrid;
            _VertexIndexList = new int[_MFGrid.ActiveCellCount * 6];
            _VertexList = new CustomVertex.PositionNormalColored[_MFGrid.ActiveCellCount * 4];
            _VectorStyle.StyleChanged -= VectorStyle_StyleChanged;
            _VectorStyle.HeadSize = 600;
            _VectorStyle.TileCount = 1;
            _VectorStyle.AutoColor = true;
            _VectorStyle.ScaleMethod = ScaleMethod.StandardDeviation;
            CreatMeshes();
            _VectorStyle.StyleChanged += VectorStyle_StyleChanged;
        }
        public override void CacheColor()
        {
            //do nothing because we have to receate meshes every time step.
        }

        public override void CreatMeshes()
        {
            if (DataSource == null)
                return;

            double maxValue = 0, minValue = 0, avValue = 0;
            int index = 0;
            double radius = EquatorialRadius + this.DistanceAboveSurface *World.Settings.VerticalExaggeration;
            double tanAngle = Math.Tan(_VectorStyle.ArrowAngle);

            double headSize = _VectorStyle.HeadSize;
            int colorIndex = _VectorStyle.ArrowColor;
            var metric_array = DataSource.GetVector(0, CurrentTimeStep.ToString(), ":");
            var anl_array = DataSource.GetVector(1, CurrentTimeStep.ToString(), ":");
            var rawlen = metric_array.Length;
            var newlen = (int)System.Math.Floor((double)rawlen / _VectorStyle.TileCount);

            maxValue = metric_array.Max();
            minValue = metric_array.Min();
            avValue = metric_array.Average();
            var std = MyStatisticsMath.StandardDeviation(metric_array);
            var levels = _DataColor.GetLevels(metric_array, this.ColourRampCount, this.ClassificationMethod);

            for (int l = 0; l < newlen; l++)
            {
                var newindex = l * Style.TileCount;
                headSize = Style.ScaleHeadSize(metric_array[newindex], maxValue, minValue, avValue, std);
                if (_VectorStyle.AutoColor)
                {
                    //colorIndex = GetVertexColor(maxValue, minValue, metric_array[l], Opacity);
                    colorIndex = _DataColor.GetVertexColor(levels[newindex], Opacity);
                }

                double angle = anl_array[newindex];// +MathEngine.PIHalf;

                double centralLon = MathEngine.DegreesToRadians(_MFGrid.CentralPoint[0, 0, newindex]);
                double centralLat = MathEngine.DegreesToRadians(_MFGrid.CentralPoint[1, 0, newindex]);
                double centralZ = radius + _MFGrid.CentralPoint[2, 0, newindex] * World.Settings.VerticalExaggeration; 

                Heiflow.Spatial.Geography.Point mp1 = Geodesic.GetPointFromHeading(centralLon, centralLat, headSize, angle);
                Point3d p31 = MathEngine.SphericalRadianToCartesianD(mp1.Y, mp1.X, centralZ);
                double x1 = p31.X;
                double y1 = p31.Y;
                double z1 = p31.Z;

                double r1 = headSize * tanAngle * _VectorStyle.Dehisce;

                Heiflow.Spatial.Geography.Point mp2 = Geodesic.GetPointFromHeading(centralLon, centralLat, r1, angle - MathEngine.PIHalf);
                Point3d p32 = MathEngine.SphericalRadianToCartesianD(mp2.Y, mp2.X, centralZ);
                double x2 = p32.X;
                double y2 = p32.Y;
                double z2 = p32.Z;

                Heiflow.Spatial.Geography.Point mp3 = Geodesic.GetPointFromHeading(centralLon, centralLat, r1, angle + MathEngine.PIHalf);
                Point3d p33 = MathEngine.SphericalRadianToCartesianD(mp3.Y, mp3.X, centralZ);
                double x3 = p33.X;
                double y3 = p33.Y;
                double z3 = p33.Z;

                Heiflow.Spatial.Geography.Point mp4 = Geodesic.GetPointFromHeading(centralLon, centralLat, headSize * _VectorStyle.TailSize, angle + Math.PI);
                Point3d p34 = MathEngine.SphericalRadianToCartesianD(mp4.Y, mp4.X, centralZ);
                double x4 = p34.X;
                double y4 = p34.Y;
                double z4 = p34.Z;

                CustomVertex.PositionNormalColored p1 = new CustomVertex.PositionNormalColored()
                {
                    X = (float)x1,
                    Y = (float)y1,
                    Z = (float)z1
                };

                CustomVertex.PositionNormalColored p2 = new CustomVertex.PositionNormalColored()
                {
                    X = (float)x2,
                    Y = (float)y2,
                    Z = (float)z2
                };
                CustomVertex.PositionNormalColored p3 = new CustomVertex.PositionNormalColored()
                {
                    X = (float)x3,
                    Y = (float)y3,
                    Z = (float)z3
                };
                CustomVertex.PositionNormalColored p4 = new CustomVertex.PositionNormalColored()
                {
                    X = (float)x4,
                    Y = (float)y4,
                    Z = (float)z4
                };

                p1.Color = colorIndex;
                p2.Color = colorIndex;
                p3.Color = colorIndex;
                p4.Color = colorIndex;

                _VertexList[index * 4 + 0] = p1;
                _VertexList[index * 4 + 1] = p2;
                _VertexList[index * 4 + 2] = p3;
                _VertexList[index * 4 + 3] = p4;
                _VertexIndexList[index * 6 + 0] = index * 4;
                _VertexIndexList[index * 6 + 1] = index * 4 + 1;
                _VertexIndexList[index * 6 + 2] = index * 4;
                _VertexIndexList[index * 6 + 3] = index * 4 + 2;
                _VertexIndexList[index * 6 + 4] = index * 4;
                _VertexIndexList[index * 6 + 5] = index * 4 + 3;
                index++;
            }

            CalculateNormals(ref _VertexList, _VertexIndexList);
            NotifyMeshChanged();
        }

        public override void UpdateVertexColor()
        {
            DataSource = (Owner.GetPackage(VelocityPackage.PackageName) as VelocityPackage).DataCube;
            if (DataSource != null && RequiredUpdated)
                CreatMeshes();
            NotifyColorsChanged();
        }

        public override void UpdateCachedColor()
        {
            UpdateVertexColor();
        }

        protected void VectorStyle_StyleChanged(object sender, EventArgs e)
        {
            UpdateVertexColor();
        }

    }
}
