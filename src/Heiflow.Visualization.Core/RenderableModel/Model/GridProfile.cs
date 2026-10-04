using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using HUST.WREIS.Dot3D;
using Heiflow.Core.MyMath;
using Heiflow.Core.Drawing;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;

namespace Heiflow.Visualization.Renderable.Grid
{
    public enum ProfileType { Row,Column,Custom}

    public class GridProfile : IProfile
    {
        private ProfileRender _ProfileRender;
        private CustomVertex.PositionNormalColored[] mVertexList;
        private int[] mVertexIndexList;

        public GridProfile(ProfileRender render, ProfileType type, int index)
        {
            _ProfileRender = render;
            ProfileType = type;
            if (type == ProfileType.Column)
                ColumnIndex = index;
            else if (type == ProfileType.Row)
                RowIndex = index;
            DistanceAboveSurface = 0;
            MaxProfileLength = 500;
            ScaleFactor = 10;
            if (LayeredColors == null)
            {
                LayeredColors = new int[_ProfileRender.Grid.LayerCount];
                for (int i = 0; i < _ProfileRender.Grid.LayerCount; i++)
                {
                    LayeredColors[i] = RandomColor.Next().ToArgb();
                }
            }          
            BuildProfile();
        }

        public ProfileType ProfileType { get; set; }

        public int RowIndex { get; set; }

        public int ColumnIndex { get; set; }

        public double MaxProfileLength { get; set; }

        public CustomVertex.PositionNormalColored[] VertexList
        {
            get
            {
                return mVertexList;
            }
        }

        public int[] VertexIndexList
        {
            get
            {
                return mVertexIndexList;
            }
        }

        public double DistanceAboveSurface { get; set; }

        public double ScaleFactor { get; set; }

        private static int[] LayeredColors;

        public void BuildProfile()
        {
            var mfgrid = _ProfileRender.Grid as MFGrid;
            Cell[] cells = null;
            if (ProfileType == ProfileType.Column)
                cells = _ProfileRender.RetrieveCellsInColumn(ColumnIndex);
            else if (ProfileType == ProfileType.Row)
                cells = _ProfileRender.RetrieveCellsInRow(RowIndex);

            int ncell = cells.Length * mfgrid.ActualLayerCount;

            double[] values = new double[ncell];

            for (int i = 0; i < cells.Length; i++)
            {
                for (int l = 0; l < mfgrid.ActualLayerCount; l++)
                {
                    values[i * mfgrid.ActualLayerCount + l] = _ProfileRender.GetCellValue(cells[i].I, cells[i].J );
                }
            }

            var eles = from c in cells select c.Elevation;
            var statEles =  MyStatisticsMath.SimpleStatistics(eles.ToArray());

            mVertexList = new CustomVertex.PositionNormalColored[ncell * 4];
            mVertexIndexList = new int[ncell * 6];

            int color = System.Drawing.Color.Red.ToArgb();
            double factor = MaxProfileLength / (statEles.Min - statEles.Max);

            int iv = 0;
            for (int c = 0; c < cells.Length; c++)
            {
                Cell cc = cells[c];
                double baseH = _ProfileRender.EquatorialRadius + DistanceAboveSurface*_ProfileRender.VerticalExaggeration;

                for (int l = 0; l < mfgrid.ActualLayerCount; l++)
                {
                    color = LayeredColors[l];
                    double[] rdus = new double[mfgrid.ActualLayerCount + 1];
                    int index = mfgrid.Topology.GetSerialIndex(cc.I, cc.J);

                    for (int t = 0; t <= mfgrid.ActualLayerCount; t++)
                    {
                        rdus[t] = baseH + mfgrid.Elevations[t,0,index] * _ProfileRender.VerticalExaggeration * 2;
                    }

                    if (ProfileType == ProfileType.Column)
                    {
                        Vector3 curNode = MathEngine.SphericalToCartesian(cc.LatitudeRadianRect[0], cc.LongitudeRadianRect[0], rdus[l]);
                        var pt1 = new CustomVertex.PositionNormalColored()
                        {
                            X = curNode.X,
                            Y = curNode.Y,
                            Z = curNode.Z,
                            Color = color
                        };
                        mVertexList[iv] = pt1;
                        iv++;

                        curNode = MathEngine.SphericalToCartesian(cc.LatitudeRadianRect[3], cc.LongitudeRadianRect[3], rdus[l]);
                        var pt2 = new CustomVertex.PositionNormalColored()
                        {
                            X = curNode.X,
                            Y = curNode.Y,
                            Z = curNode.Z,
                            Color = color
                        };
                        mVertexList[iv] = pt2;
                        iv++;

                        curNode = MathEngine.SphericalToCartesian(cc.LatitudeRadianRect[3], cc.LongitudeRadianRect[3], rdus[l + 1]);
                        var pt3 = new CustomVertex.PositionNormalColored()
                        {
                            X = curNode.X,
                            Y = curNode.Y,
                            Z = curNode.Z,
                            Color = color
                        };
                        mVertexList[iv] = pt3;
                        iv++;

                        curNode = MathEngine.SphericalToCartesian(cc.LatitudeRadianRect[0], cc.LongitudeRadianRect[0], rdus[l + 1]);
                        var pt4 = new CustomVertex.PositionNormalColored()
                        {
                            X = curNode.X,
                            Y = curNode.Y,
                            Z = curNode.Z,
                            Color = color
                        };
                        mVertexList[iv] = pt4;
                        iv++;
                    }
                    else if (ProfileType == ProfileType.Row)
                    {
                        Vector3 curNode = MathEngine.SphericalToCartesian(cc.LatitudeRadianRect[0], cc.LongitudeRadianRect[0], rdus[l]);
                        var pt1 = new CustomVertex.PositionNormalColored()
                        {
                            X = curNode.X,
                            Y = curNode.Y,
                            Z = curNode.Z,
                            Color = color
                        };
                        mVertexList[iv] = pt1;
                        iv++;

                        curNode = MathEngine.SphericalToCartesian(cc.LatitudeRadianRect[1], cc.LongitudeRadianRect[1], rdus[l]);
                        var pt2 = new CustomVertex.PositionNormalColored()
                        {
                            X = curNode.X,
                            Y = curNode.Y,
                            Z = curNode.Z,
                            Color = color
                        };
                        mVertexList[iv] = pt2;
                        iv++;

                        curNode = MathEngine.SphericalToCartesian(cc.LatitudeRadianRect[1], cc.LongitudeRadianRect[1], rdus[l + 1]);
                        var pt3 = new CustomVertex.PositionNormalColored()
                        {
                            X = curNode.X,
                            Y = curNode.Y,
                            Z = curNode.Z,
                            Color = color
                        };
                        mVertexList[iv] = pt3;
                        iv++;

                        curNode = MathEngine.SphericalToCartesian(cc.LatitudeRadianRect[0], cc.LongitudeRadianRect[0], rdus[l + 1]);
                        var pt4 = new CustomVertex.PositionNormalColored()
                        {
                            X = curNode.X,
                            Y = curNode.Y,
                            Z = curNode.Z,
                            Color = color
                        };
                        mVertexList[iv] = pt4;
                        iv++;
                    }
                }
            }
            int ii = 0;
            for (int c = 0; c < cells.Length; c++)
            {
                for (int l = 0; l < mfgrid.ActualLayerCount; l++)
                {
                    mVertexIndexList[ii * 6 + 0] = ii * 4;
                    mVertexIndexList[ii * 6 + 1] = ii * 4 + 2;
                    mVertexIndexList[ii * 6 + 2] = ii * 4 + 1;

                    mVertexIndexList[ii * 6 + 3] = ii * 4;
                    mVertexIndexList[ii * 6 + 4] = ii * 4 + 3;
                    mVertexIndexList[ii * 6 + 5] = ii * 4 + 2;
                    ii++;
                }
            }
            CalculateNormals(ref mVertexList, mVertexIndexList);
        }

        protected void CalculateNormals(ref CustomVertex.PositionNormalColored[] vertices, int[] indices)
        {
            ModelNormals.Calculate(vertices, indices);
        }
    }
}
