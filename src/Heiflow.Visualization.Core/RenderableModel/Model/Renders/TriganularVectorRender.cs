using Heiflow.Core.Data;
using Heiflow.Core.MyMath;
using Heiflow.Models.Generic;
using Heiflow.Models.GHM;
using Heiflow.Spatial.Geography;
using HUST.WREIS.Dot3D;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable.Grid
{
    public class TriganularVectorRender: VectorRender
    {
        public TriganularVectorRender(World world):base(world)
        {
            Name = "TriganularVectorRender";
            SupportedLayerObject = "Heiflow.Visualization.Renderable.Grid.DXVectorLayer";
        }

        public TriganularVectorRender()
        {
            Name = "TriganularVectorRender";
            SupportedLayerObject = "Heiflow.Visualization.Renderable.Grid.DXVectorLayer";
        }

        private ITriangularGrid _TriangularGrid;

        public override void Initilize()
        {
            base.Initilize();
            _TriangularGrid = _Project.Model.Grid as ITriangularGrid;
            _VertexIndexList = new int[_TriangularGrid.ActiveCellCount * 6];
            _VertexList = new CustomVertex.PositionNormalColored[_TriangularGrid.ActiveCellCount * 4];
            _VectorStyle.StyleChanged -= VectorStyle_StyleChanged;
            _VectorStyle.HeadSize = 600;
            _VectorStyle.AutoColor = true;
            _VectorStyle.ScaleMethod = ScaleMethod.StandardDeviation;
            CreatMeshes();
            _VectorStyle.StyleChanged += VectorStyle_StyleChanged;
        }
        public override void LoadDataSource(string filename)
        {
            var ghm = Project.Model as GHModel;
            if(File.Exists(filename))
            {
                var provider = ghm.Project.DataCubeFileFactory.Select(filename);
                if(provider != null)
                    this.DataSource = provider.ProvideSingleStep(filename);
            }
        }
        public override void CreatMeshes()
        {
            if (DataSource == null)
                return;

            double maxValue = 0, minValue = 0, avValue = 0;
            int index = 0;
            double radius = EquatorialRadius + RenderableObject.DistanceAboveSurface * World.Settings.VerticalExaggeration;
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
                var newindex = l * _VectorStyle.TileCount;
                headSize = _VectorStyle.ScaleHeadSize(metric_array[newindex], maxValue, minValue, avValue, std);
                if (_VectorStyle.AutoColor)
                {
                    colorIndex = _DataColor.GetVertexColor(levels[newindex],Opacity);
                }

                double angle = anl_array[newindex];// +MathEngine.PIHalf;

                double centralLon = MathEngine.DegreesToRadians(_TriangularGrid.Centroids[newindex].X);
                double centralLat = MathEngine.DegreesToRadians(_TriangularGrid.Centroids[newindex].Y);
                double centralZ = radius + _TriangularGrid.Centroids[newindex].Z * VerticalExaggeration;

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
        }

        public override float GetCellValue(int p1, int p2)
        {
            throw new NotImplementedException();
        }

        public override Models.Generic.ICell SelectCell(double lat, double lon)
        {
            return null;
        }

        public override void UpdateVertexColor()
        {
            if (DataSource != null && RequiredUpdated)
                CreatMeshes();
            NotifyColorsChanged();
        }

        public override void CacheColor()
        {
            //do nothing because we have to receate meshes every time step.
        }

        public override void UpdateCachedColor()
        {
            UpdateVertexColor();
        }

        public override void UpdateCellValues()
        {
         
        }

        protected override void VectorStyle_StyleChanged(object sender, EventArgs e)
        {
            UpdateVertexColor();
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
