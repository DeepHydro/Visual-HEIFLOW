using Heiflow.Core.Data;
using Heiflow.Core.MyMath;
using Heiflow.Models.Generic;
using Heiflow.Models.GHM;
using Heiflow.Spatial.Geography;
using HUST.WREIS.Dot3D;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

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
            double radius = EquatorialRadius + RenderableObject.DistanceAboveSurface * World.Settings.VerticalExaggeration;
            double tanAngle = Math.Tan(_VectorStyle.ArrowAngle);

            double headSize = _VectorStyle.HeadSize;
            int colorIndex = _VectorStyle.ArrowColor;
            var metric_array = DataSource.GetVector(0, CurrentTimeStep.ToString(), ":");
            var anl_array = DataSource.GetVector(1, CurrentTimeStep.ToString(), ":");

            maxValue = metric_array.Max();
            minValue = metric_array.Min();
            avValue = metric_array.Average();
            var std = MyStatisticsMath.StandardDeviation(metric_array);
            var levels = _DataColor.GetLevels(metric_array, this.ColourRampCount, this.ClassificationMethod);

            // An arrow is written for every active cell of the grid, not only for the cells the tile
            // count keeps. The count decides which of them have a length: every tileCount-th cell is
            // drawn, the cells in between get an arrow whose four points sit on their own centre,
            // which is three lines of no length and so draws nothing. Writing every cell is what lets
            // a larger count take hold, a cell the new count leaves out used to keep the arrow the
            // count before it had given it, so only part of the grid answered the setting.
            int tileCount = Math.Max(1, _VectorStyle.TileCount);
            int cellCount = _VertexList.Length / 4;

            for (int c = 0; c < cellCount; c++)
            {
                bool hasData = c < metric_array.Length && c < anl_array.Length;
                if (_VectorStyle.AutoColor && hasData)
                {
                    colorIndex = _DataColor.GetVertexColor(levels[c],Opacity);
                }

                double angle = hasData ? anl_array[c] : 0;

                double centralLon = MathEngine.DegreesToRadians(_TriangularGrid.Centroids[c].X);
                double centralLat = MathEngine.DegreesToRadians(_TriangularGrid.Centroids[c].Y);
                double centralZ = radius + _TriangularGrid.Centroids[c].Z * VerticalExaggeration;
                Point3d center = MathEngine.SphericalRadianToCartesianD(centralLat, centralLon, centralZ);

                double x1 = center.X;
                double y1 = center.Y;
                double z1 = center.Z;
                double x2 = center.X;
                double y2 = center.Y;
                double z2 = center.Z;
                double x3 = center.X;
                double y3 = center.Y;
                double z3 = center.Z;
                double x4 = center.X;
                double y4 = center.Y;
                double z4 = center.Z;

                if (hasData && c % tileCount == 0)
                {
                    headSize = _VectorStyle.ScaleHeadSize(metric_array[c], maxValue, minValue, avValue, std);

                    Heiflow.Spatial.Geography.Point mp1 = Geodesic.GetPointFromHeading(centralLon, centralLat, headSize, angle);
                    Point3d p31 = MathEngine.SphericalRadianToCartesianD(mp1.Y, mp1.X, centralZ);
                    x1 = p31.X;
                    y1 = p31.Y;
                    z1 = p31.Z;

                    double r1 = headSize * tanAngle * _VectorStyle.Dehisce;

                    Heiflow.Spatial.Geography.Point mp2 = Geodesic.GetPointFromHeading(centralLon, centralLat, r1, angle - MathEngine.PIHalf);
                    Point3d p32 = MathEngine.SphericalRadianToCartesianD(mp2.Y, mp2.X, centralZ);
                    x2 = p32.X;
                    y2 = p32.Y;
                    z2 = p32.Z;

                    Heiflow.Spatial.Geography.Point mp3 = Geodesic.GetPointFromHeading(centralLon, centralLat, r1, angle + MathEngine.PIHalf);
                    Point3d p33 = MathEngine.SphericalRadianToCartesianD(mp3.Y, mp3.X, centralZ);
                    x3 = p33.X;
                    y3 = p33.Y;
                    z3 = p33.Z;

                    Heiflow.Spatial.Geography.Point mp4 = Geodesic.GetPointFromHeading(centralLon, centralLat, headSize * _VectorStyle.TailSize, angle + Math.PI);
                    Point3d p34 = MathEngine.SphericalRadianToCartesianD(mp4.Y, mp4.X, centralZ);
                    x4 = p34.X;
                    y4 = p34.Y;
                    z4 = p34.Z;
                }

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

                _VertexList[c * 4 + 0] = p1;
                _VertexList[c * 4 + 1] = p2;
                _VertexList[c * 4 + 2] = p3;
                _VertexList[c * 4 + 3] = p4;
                _VertexIndexList[c * 6 + 0] = c * 4;
                _VertexIndexList[c * 6 + 1] = c * 4 + 1;
                _VertexIndexList[c * 6 + 2] = c * 4;
                _VertexIndexList[c * 6 + 3] = c * 4 + 2;
                _VertexIndexList[c * 6 + 4] = c * 4;
                _VertexIndexList[c * 6 + 5] = c * 4 + 3;
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
