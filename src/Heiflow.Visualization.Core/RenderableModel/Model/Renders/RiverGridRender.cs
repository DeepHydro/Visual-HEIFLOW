using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Hydrology;
using Heiflow.Models.Subsurface;
using Microsoft.DirectX.Direct3D;
using Heiflow.Spatial;
using Heiflow.Core.MyMath;
using HUST.WREIS.Dot3D;
using Heiflow.Core.Data;
using GeoAPI.Geometries;

namespace Heiflow.Visualization.Renderable.Grid
{
    public class RiverGridRender: MFGridRender
    {
        private RiverNetwork _RiverNetwork;
        private int _RiverColor;
        public RiverGridRender(World world)
            : base(world)
        {         
            _RiverColor = System.Drawing.Color.Blue.ToArgb();
        }

        public override void Initilize()
        {
            VertexUnifiedColor = System.Drawing.Color.Blue.ToArgb();
            _ColourRampCount = 5;
            Opacity = 255;
            NoDataValue = -9999;
            Profiles = new List<IProfile>();

            _Project = LayerObject.Project;
            _MFGrid = _Project.Model.Grid as MFGrid;
            Grid = _MFGrid;

            _RiverNetwork = ((RenderableObject as IDX3DLayer).Select(SFRPackage.PackageName) as SFRPackage).RiverNetwork;
            DataSource = new DataCube<float>(1,1,_RiverNetwork.ReachCount);
            for (int i = 0; i < _RiverNetwork.ReachCount; i++)
                DataSource[0, 0, i] = (float)_RiverNetwork.Reaches[i].TopElevation;
            _Initialized = true;
        }

        public override void CreatMeshes()
        {
            if (_RiverNetwork == null)
                return;
            double layerRadius = EquatorialRadius + this.DistanceAboveSurface * World.Settings.VerticalExaggeration;
            RegularGridTopology topo = _MFGrid.Topology;
            int reachNum = _RiverNetwork.ReachCount;
            int vertexCount = reachNum * 4;
            var vertexList = new CustomVertex.PositionNormalColored[vertexCount];
             int[] vertexIndexList = new int[reachNum * 6];
            CoordinateConvertor.ZoneWide = 6;

            var vertexUnifiedColor = DataColor.GetRandomColor(1);

            var upleft = new Coordinate(_MFGrid.BBox.MinX, _MFGrid.BBox.MaxY);
            var lowerRight = new Coordinate(_MFGrid.BBox.MaxX, _MFGrid.BBox.MinY);
            double deltRow = Math.Abs(upleft.Y - lowerRight.Y) / _MFGrid.RowCount;
            double deltCol = Math.Abs(upleft.X - lowerRight.X) / _MFGrid.ColumnCount;

            GCSConverter.ToLatLon(_MFGrid.BBox.Centre.X, _MFGrid.BBox.Centre.Y);
            ComputeLocalOrigin(GCSConverter.Longitude, GCSConverter.Latitude);
           var vector = _MFGrid.Elevations.GetVector(0,"0", ":");
            MaxCellValue = vector.Max();
            MinCellValue = vector.Min();
            StatisticsInfo = MyStatisticsMath.SimpleStatistics(vector);
            int i_node = 0;
            int i_rch = 0;
            foreach (var riv in _RiverNetwork.Rivers)
            {
                foreach (var rch in riv.Reaches)
                {
                    var row = rch.IRCH;
                    var col = rch.JRCH;
                    double xx = upleft.X + col * deltCol;
                    double yy = upleft.Y - row * deltRow;
                    //NW node
                    int id = _MFGrid.Topology.GetID(row - 1, col - 1);
                    int index = _MFGrid.Topology.CellID2CellIndex[id];

                    double nodeEle = vector[index] * HUST.WREIS.Dot3D.World.Settings.VerticalExaggeration + 100;
                    var curNode = CellNode(GCSConverter, xx, yy, nodeEle, layerRadius);
                    vertexList[i_node] = new CustomVertex.PositionNormalColored()
                    {
                        X = curNode.X,
                        Y = curNode.Y,
                        Z = curNode.Z
                    };
                    vertexList[i_node].Color = _RiverColor;
                    vertexIndexList[i_rch * 6 + 1] = i_node;
                    i_node++;

                    //NE node
                    xx = upleft.X + (col + 1) * deltCol;
                    yy = upleft.Y - row * deltRow;
                    curNode = CellNode(GCSConverter, xx, yy, nodeEle, layerRadius);
                    vertexList[i_node] = new CustomVertex.PositionNormalColored()
                    {
                        X = curNode.X,
                        Y = curNode.Y,
                        Z = curNode.Z
                    };
                    vertexList[i_node].Color = _RiverColor;
                    vertexIndexList[i_rch * 6 + 2] = i_node;
                    vertexIndexList[i_rch * 6 + 5] = i_node;
                    i_node++;

                    //SE node
                    xx = upleft.X + (col + 1) * deltCol;
                    yy = upleft.Y - (row + 1) * deltRow;
                    curNode = CellNode(GCSConverter, xx, yy, nodeEle, layerRadius);
                    vertexList[i_node] = new CustomVertex.PositionNormalColored()
                    {
                        X = curNode.X,
                        Y = curNode.Y,
                        Z = curNode.Z
                    };
                    vertexList[i_node].Color = _RiverColor;
                    vertexIndexList[i_rch * 6 + 3] = i_node;
                    i_node++;

                    //SW node
                    xx = upleft.X + col * deltCol;
                    yy = upleft.Y - (row + 1) * deltRow;
                    curNode = CellNode(GCSConverter, xx, yy, nodeEle, layerRadius);
                    vertexList[i_node] = new CustomVertex.PositionNormalColored()
                    {
                        X = curNode.X,
                        Y = curNode.Y,
                        Z = curNode.Z
                    };
                    vertexList[i_node].Color = _RiverColor;
                    vertexIndexList[i_rch * 6] = i_node;
                    vertexIndexList[i_rch * 6 + 4] = i_node;
                    i_node++;

                    i_rch++;
                }
            }

            GCSConverter.ToLatLon(_MFGrid.BBox.MinX, _MFGrid.BBox.MaxY);
            double west = GCSConverter.Longitude;
            double north = GCSConverter.Latitude;
            GCSConverter.ToLatLon(_MFGrid.BBox.MaxX, _MFGrid.BBox.MinY);
            double east = GCSConverter.Longitude;
            double sourth = GCSConverter.Latitude;
            BBox = new Envelope(sourth, north, west, east);

           CalculateNormals(ref vertexList, vertexIndexList);

            _VertexList = vertexList;
            _VertexIndexList = vertexIndexList;

        }

        public override void CacheColor()
        {
            var timesteps = DataSource.Size[1];
            cachedMaxValues = new float[timesteps];
            cachedMinValues = new float[timesteps];
            cachedStatisticsInfo = new StatisticsInfo[timesteps];
            DataCube<int> color = new DataCube<int>(1,timesteps, DataSource.Size[2]);
            for (int t = 0; t < timesteps; t++)
            {
                var vector = GetGridValues(DataSource, t);
               cachedMaxValues[t] = vector.Max();
               cachedMinValues[t] = vector.Min();
                for (int i = 0; i < vector.Length; i++)
                {
                    color[0, t, i] = _DataColor.GetVertexColor(cachedMaxValues[t], cachedMinValues[t], vector[i], Opacity);
                }
                cachedStatisticsInfo[t] = MyStatisticsMath.SimpleStatistics(vector);
            }
            cachedColor = color;
        }

        public override void UpdateCachedColor()
        {
            if (DataSource == null)
                return;
            if (cachedColor == null)
                CacheColor();
            for (int i = 0; i < DataSource.Size[2]; i++)
            {
                var color = cachedColor[0,CurrentTimeStep,i];
                VertexList[i * 4].Color = color;
                VertexList[i * 4 + 1].Color = color;
                VertexList[i * 4 + 2].Color = color;
                VertexList[i * 4 + 3].Color = color;
            }
            MaxCellValue = cachedMaxValues[CurrentTimeStep];
            MinCellValue = cachedMinValues[CurrentTimeStep];
            StatisticsInfo = cachedStatisticsInfo[CurrentTimeStep];
        }

        public override void UpdateVertexColor()
        {
            if (GridValues == null)
            {
                GridValues = GetGridValues(DataSource, CurrentTimeStep);
            }

            if (GridValues != null)
            {
                MaxCellValue = GridValues.Max();
                MinCellValue = GridValues.Min();
                StatisticsInfo = MyStatisticsMath.SimpleStatistics(GridValues);
                for (int i = 0; i < GridValues.Length; i++)
                {
                    var color = _DataColor.GetVertexColor(MaxCellValue, MinCellValue, GridValues[i], Opacity);
                    VertexList[i * 4].Color = color;
                    VertexList[i * 4 + 1].Color = color;
                    VertexList[i * 4 + 2].Color = color;
                    VertexList[i * 4 + 3].Color = color;
                }
            }
        }
    }
}
