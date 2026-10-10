using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Hydrology;
using Heiflow.Models.Subsurface;
using Microsoft.DirectX;
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
        /// <summary>
        /// Metres the surface of a reach is lifted above the cell it flows through. Without it the river
        /// network sinks into the landscape and is hidden by it as soon as the terrain is exaggerated.
        /// </summary>
        private const double LiftAboveGround = 10;

        private RiverNetwork _RiverNetwork;
        private int _RiverColor;
        public RiverGridRender(World world)
            : base(world)
        {         
            _RiverColor = System.Drawing.Color.Blue.ToArgb();
        }

        public override void Initilize()
        {
            _Project = LayerObject.Project;
            _MFGrid = _Project.Model.Grid as MFGrid;
            Grid = _MFGrid;
            _Topology = _MFGrid.Topology;

            // Set them before the meshes are built. GetVertexColor multiplies the opacity into the colour
            // of every vertex, and a reach would stay invisible if the opacity was still the default zero.
            VertexUnifiedColor = System.Drawing.Color.Blue.ToArgb();
            _ColourRampCount = 5;
            Opacity = 255;
            NoDataValue = -9999;
            Profiles = new List<IProfile>();

            var package = (RenderableObject as IDX3DLayer).Select(SFRPackage.PackageName) as SFRPackage;
            _RiverNetwork = package != null ? package.RiverNetwork : null;

            if (_RiverNetwork == null || _MFGrid == null)
            {
                _Initialized = true;
                return;
            }

            // The reaches are enumerated river by river, in that order they are stored in the data source
            // and in that order CreatMeshes writes them into the vertex list.
            int reachNum = _RiverNetwork.Reaches.Count;
            DataSource = new DataCube<float>(1, 1, reachNum);
            int n = 0;
            foreach (var river in _RiverNetwork.Rivers)
            {
                foreach (var reach in river.Reaches)
                {
                    if (n >= reachNum)
                        break;
                    DataSource[0, 0, n] = (float)reach.TopElevation;
                    n++;
                }
            }

            // Every other renderer builds its meshes here. This one relied on the layer noticing the
            // difference between its vertical exaggeration and the one of the world, nothing was ever
            // drawn when they happened to agree.
            CreatMeshes();
            _Initialized = true;
        }

        public override void CreatMeshes()
        {
            if (_RiverNetwork == null || _MFGrid == null)
                return;

            // ReachCount is a field that is carried around with the network and does not always agree with
            // the number of reaches it holds, the list is what is iterated.
            int reachNum = _RiverNetwork.Reaches.Count;
            if (reachNum == 0)
                return;

            _Topology = _MFGrid.Topology;
            double exaggeration = World.Settings.VerticalExaggeration;

            // Same datum as the terrain of MFGridRender, otherwise the river floats above the landscape or
            // disappears into it.
            double layerRadius = EquatorialRadius + RenderableObject.DistanceAboveSurface * exaggeration;
            int vertexCount = reachNum * 4;
            var vertexList = new CustomVertex.PositionNormalColored[vertexCount];
            var vertexIndexList = new int[reachNum * 6];

            var upleft = new Coordinate(_MFGrid.BBox.MinX, _MFGrid.BBox.MaxY);
            var lowerRight = new Coordinate(_MFGrid.BBox.MaxX, _MFGrid.BBox.MinY);
            double deltRow = Math.Abs(upleft.Y - lowerRight.Y) / _MFGrid.RowCount;
            double deltCol = Math.Abs(upleft.X - lowerRight.X) / _MFGrid.ColumnCount;

            GCSConverter.ToLatLon(_MFGrid.BBox.Centre.X, _MFGrid.BBox.Centre.Y);
            ComputeLocalOrigin(GCSConverter.Longitude, GCSConverter.Latitude);

            var ground = _MFGrid.Elevations.GetVector(CurrentLayerIndex, "0", ":");
            MaxCellValue = (float)_RiverNetwork.Reaches.Max(rch => rch.TopElevation);
            MinCellValue = (float)_RiverNetwork.Reaches.Min(rch => rch.TopElevation);
            StatisticsInfo = MyStatisticsMath.SimpleStatistics(
                _RiverNetwork.Reaches.Select(rch => (float)rch.TopElevation).ToArray());

            int i_node = 0;
            int i_rch = 0;
            foreach (var riv in _RiverNetwork.Rivers)
            {
                foreach (var rch in riv.Reaches)
                {
                    if (i_rch >= reachNum)
                        break;

                    // IRCH and JRCH start counting at one, the topology at zero.
                    int row = rch.IRCH - 1;
                    int col = rch.JRCH - 1;

                    double cellElevation = 0;
                    int index = _Topology.GetSerialIndex(row, col);
                    if (index >= 0)
                        cellElevation = ground[index];

                    // The bed of a reach can lie below the ground, keep it on top of the hill so that the
                    // water stays visible whatever the exaggeration is.
                    double elevation = Math.Max(cellElevation, rch.TopElevation);
                    double nodeEle = (elevation + DistanceAboveSurface + LiftAboveGround) * exaggeration;

                    // Corners of the cell the reach belongs to.
                    double west = upleft.X + col * deltCol;
                    double east = upleft.X + (col + 1) * deltCol;
                    double north = upleft.Y - row * deltRow;
                    double sourth = upleft.Y - (row + 1) * deltRow;

                    // The two triangles are wound the way the terrain mesh of MFGridRender winds them.
                    //NW
                    AddNode(vertexList, i_node, west, north, nodeEle, layerRadius);
                    vertexIndexList[i_rch * 6 + 1] = i_node;
                    i_node++;

                    //NE
                    AddNode(vertexList, i_node, east, north, nodeEle, layerRadius);
                    vertexIndexList[i_rch * 6 + 2] = i_node;
                    vertexIndexList[i_rch * 6 + 5] = i_node;
                    i_node++;

                    //SE
                    AddNode(vertexList, i_node, east, sourth, nodeEle, layerRadius);
                    vertexIndexList[i_rch * 6 + 3] = i_node;
                    i_node++;

                    //SW
                    AddNode(vertexList, i_node, west, sourth, nodeEle, layerRadius);
                    vertexIndexList[i_rch * 6 + 0] = i_node;
                    vertexIndexList[i_rch * 6 + 4] = i_node;
                    i_node++;

                    i_rch++;
                }
            }

            GCSConverter.ToLatLon(_MFGrid.BBox.MinX, _MFGrid.BBox.MaxY);
            double bboxWest = GCSConverter.Longitude;
            double bboxNorth = GCSConverter.Latitude;
            GCSConverter.ToLatLon(_MFGrid.BBox.MaxX, _MFGrid.BBox.MinY);
            double bboxEast = GCSConverter.Longitude;
            double bboxSouth = GCSConverter.Latitude;
            // An envelope is built from (minX, maxX, minY, maxY), which is west, east, south, north.
            BBox = new Envelope(bboxWest, bboxEast, bboxSouth, bboxNorth);

            CalculateNormals(ref vertexList, vertexIndexList);

            _VertexList = vertexList;
            _VertexIndexList = vertexIndexList;

            NotifyMeshChanged();
        }

        /// <summary>
        /// Writes one corner of the rectangle of a reach into the vertex list.
        /// </summary>
        private void AddNode(CustomVertex.PositionNormalColored[] vertexList, int index, double x, double y,
            double nodeEle, double layerRadius)
        {
            var curNode = CellNode(GCSConverter, x, y, nodeEle, layerRadius);
            vertexList[index].X = curNode.X;
            vertexList[index].Y = curNode.Y;
            vertexList[index].Z = curNode.Z;
            vertexList[index].Color = _RiverColor;
        }

        public override void CacheColor()
        {
            if (DataSource == null)
            {
                return;
            }
            var timesteps = DataSource.Size[1];
            cachedMaxValues = new float[timesteps];
            cachedMinValues = new float[timesteps];
            cachedStatisticsInfo = new StatisticsInfo[timesteps];
            DataCube<int> color = new DataCube<int>(1,timesteps, DataSource.Size[2]);
            for (int t = 0; t < timesteps; t++)
            {
                var vector = GetGridValues(DataSource, t);
                // No array behind the variable the source is set to, which UpdateVertexColor already
                // draws nothing for. It is the variable that decides and not the step, so there is no
                // cache to fill here and the colours that are on screen are kept.
                if (vector == null)
                {
                    return;
                }
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
            // the cache was filled under the ramp, class count, method and opacity of that moment,
            // once any of them moved on it has to be recomputed or the layer keeps the old colours
            if (IsCachedColorStale)
            {
                CacheColor();
                if (cachedColor != null)
                    MarkCachedColorFresh();
            }
            // CacheColor leaves without one while the source has no array for the variable, and
            // there is nothing to put on the vertices until it has.
            if (cachedColor != null)
            {
                for (int i = 0; i < DataSource.Size[2]; i++)
                {
                    var color = cachedColor[0, CurrentTimeStep, i];
                    VertexList[i * 4].Color = color;
                    VertexList[i * 4 + 1].Color = color;
                    VertexList[i * 4 + 2].Color = color;
                    VertexList[i * 4 + 3].Color = color;
                }
                MaxCellValue = cachedMaxValues[CurrentTimeStep];
                MinCellValue = cachedMinValues[CurrentTimeStep];
                StatisticsInfo = cachedStatisticsInfo[CurrentTimeStep];
                NotifyColorsChanged();
            }
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

                NotifyColorsChanged();
            }
        }
    }
}
