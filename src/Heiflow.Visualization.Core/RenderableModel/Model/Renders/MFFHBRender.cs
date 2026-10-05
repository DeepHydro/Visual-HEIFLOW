using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
using System.Linq;
using HUST.WREIS.Dot3D;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Heiflow.Models.Generic;
using Heiflow.Presentation.Controls;
using GeoAPI.Geometries;

namespace Heiflow.Visualization.Renderable.Grid
{
    public class MFFHBRender:ModelRenderDX
    {
        private MFGrid _MFGrid;
        private float _MaxBarHeight = 10;
        private FHBPackage _FHBPackage;
        public MFFHBRender(World world)
            : base(world)
        {
            Scale = 0.01f;
            SupportedLayerObject = "Heiflow.Visualization.Renderable.Grid.RenderableMFWellLayer";
        }

        public float Scale
        {
            get;
            set;
        }

        public float MaximumValue
        {
            get;
            private set;
        }

        public float MinimumValue
        {
            get;
            private set;
        }

        public float MaxBarHeight
        {
            get
            {
                return _MaxBarHeight;
            }
            set
            {
                _MaxBarHeight = value;
            }
        }

        public MFGrid MFGrid
        {
            get
            {
                return _MFGrid;
            }
            set
            {
                _MFGrid = value;
            }
        }
        public override void Initilize()
        {
            base.Initilize();
            _Project = LayerObject.Project;
            var buf = from pck in LayerObject.Packages where pck.Name == FHBPackage.PackageName select pck;
            if (buf.Any())
            {
                _FHBPackage = buf.First() as FHBPackage;
                _MFGrid = _FHBPackage.MFGridInstance;
                CreatMeshes();
            }
            else
            {
                _Initialized = false;
            }
        }
        public override void CreatMeshes()
        {
            if (_FHBPackage != null)
            {
                int nwell = _FHBPackage.NFLW;

                var upleft = new Coordinate(_MFGrid.BBox.MinX, _MFGrid.BBox.MaxY);
                var lowerRight = new Coordinate(_MFGrid.BBox.MaxX, _MFGrid.BBox.MinY);
                double deltRow = Math.Abs(upleft.Y - lowerRight.Y) / _MFGrid.RowCount;
                double deltCol = Math.Abs(upleft.X - lowerRight.X) / _MFGrid.ColumnCount;

                double lat = 0, lng = 0;
                double rlng = 0;
                double rlat = 0;
                double radius;

                var mat = _FHBPackage.FlowRate;
                var vec = new float[mat.Size[2]];
                for (int i = 0; i < mat.Size[2]; i++)
                {
                    vec[i] = mat[4, 0, i];
                }
                MaximumValue = vec.Max();
                MinimumValue = vec.Min();
                if (MinimumValue <= 0)
                    MinimumValue = 1;

                _VertexList = new CustomVertex.PositionNormalColored[nwell * 8];
                _VertexIndexList = new int[nwell * 36];
                for (int i = 0; i < mat.Size[2]; i++)
                {
                    int layer = (int)mat[0, 0, i];
                    int row = (int)mat[1, 0, i];
                    int col = (int)mat[2, 0, i];

                    int index = _MFGrid.Topology.GetSerialIndex(row - 1, col - 1);
                    double[] XX = new double[4];
                    double[] YY = new double[4];

                    XX[0] = upleft.X + col * deltCol;
                    XX[1] = upleft.X + (col +Scale) * deltCol;
                    XX[2] = upleft.X + (col + Scale) * deltCol;
                    XX[3] = upleft.X + col * deltCol;

                    YY[0] = upleft.Y - row * deltRow ;
                    YY[1] = upleft.Y - row * deltRow ;
                    YY[2] = upleft.Y - (row + Scale) * deltRow ;
                    YY[3] = upleft.Y - (row + Scale) * deltRow;

                    float elev = 0;
                    if (index != -1)
                        elev = _MFGrid.Elevations[layer,0,index];
                    double percent = 1;
                    if (MaximumValue != MinimumValue)
                       percent = vec[i] / (MaximumValue - MinimumValue);
                    var height = MaxBarHeight * percent;
                    var color = DataColor.GetColor(percent);

                    for (int j = 0; j < 4; j++)
                    {
                        GCSConverter.ToLatLon(XX[j], YY[j]);
                        lng = GCSConverter.Longitude;
                        lat = GCSConverter.Latitude;

                        rlng = MathEngine.DegreesToRadians(lng);
                        rlat = MathEngine.DegreesToRadians(lat);

                        radius = EquatorialRadius + (elev + this.DistanceAboveSurface) * World.Settings.VerticalExaggeration;
                        Vector3 curNode = MathEngine.SphericalToCartesian(lat, lng, radius);
                        _VertexList[i * 8 + j] = new CustomVertex.PositionNormalColored()
                        {
                            X = curNode.X,
                            Y = curNode.Y,
                            Z = curNode.Z,
                            Nx = (float)lng,
                            Ny = (float)lat,
                            Nz = elev * World.Settings.VerticalExaggeration
                        };
                        _VertexList[i * 8 + j].Color = color;

                        radius = EquatorialRadius + (elev + height + RenderableObject.DistanceAboveSurface) * World.Settings.VerticalExaggeration;
                        curNode = MathEngine.SphericalToCartesian(lat, lng, radius);
                        _VertexList[i * 8 + j + 4] = new CustomVertex.PositionNormalColored()
                        {
                            X = curNode.X,
                            Y = curNode.Y,
                            Z = curNode.Z,
                            Nx = (float)lng,
                            Ny = (float)lat,
                            Nz = (float)(elev + height) * World.Settings.VerticalExaggeration
                        };
                        _VertexList[i * 8 + j + 4].Color = color;
                    }

                    //top
                    _VertexIndexList[i * 36 + 0] = i * 8;
                    _VertexIndexList[i * 36 + 1] = i * 8 + 1;
                    _VertexIndexList[i * 36 + 2] = i * 8 + 2;
                    _VertexIndexList[i * 36 + 3] = i * 8;
                    _VertexIndexList[i * 36 + 4] = i * 8 + 2;
                    _VertexIndexList[i * 36 + 5] = i * 8 + 3;
                    //front
                    _VertexIndexList[i * 36 + 6] = i * 8;
                    _VertexIndexList[i * 36 + 7] = i * 8 + 1;
                    _VertexIndexList[i * 36 + 8] = i * 8 + 4;
                    _VertexIndexList[i * 36 + 9] = i * 8 + 1;
                    _VertexIndexList[i * 36 + 10] = i * 8 + 5;
                    _VertexIndexList[i * 36 + 11] = i * 8 + 4;
                    //bottom
                    _VertexIndexList[i * 36 + 12] = i * 8 + 4;
                    _VertexIndexList[i * 36 + 13] = i * 8 + 7;
                    _VertexIndexList[i * 36 + 14] = i * 8 + 6;
                    _VertexIndexList[i * 36 + 15] = i * 8 + 4;
                    _VertexIndexList[i * 36 + 16] = i * 8 + 6;
                    _VertexIndexList[i * 36 + 17] = i * 8 + 5;
                    //back
                    _VertexIndexList[i * 36 + 18] = i * 8 + 3;
                    _VertexIndexList[i * 36 + 19] = i * 8 + 2;
                    _VertexIndexList[i * 36 + 20] = i * 8 + 7;
                    _VertexIndexList[i * 36 + 21] = i * 8 + 7;
                    _VertexIndexList[i * 36 + 22] = i * 8 + 2;
                    _VertexIndexList[i * 36 + 23] = i * 8 + 6;
                    //left
                    _VertexIndexList[i * 36 + 24] = i * 8 + 0;
                    _VertexIndexList[i * 36 + 25] = i * 8 + 3;
                    _VertexIndexList[i * 36 + 26] = i * 8 + 7;
                    _VertexIndexList[i * 36 + 27] = i * 8 + 0;
                    _VertexIndexList[i * 36 + 28] = i * 8 + 7;
                    _VertexIndexList[i * 36 + 29] = i * 8 + 4;
                    //right
                    _VertexIndexList[i * 36 + 30] = i * 8 + 1;
                    _VertexIndexList[i * 36 + 31] = i * 8 + 2;
                    _VertexIndexList[i * 36 + 32] = i * 8 + 6;
                    _VertexIndexList[i * 36 + 33] = i * 8 + 1;
                    _VertexIndexList[i * 36 + 34] = i * 8 + 6;
                    _VertexIndexList[i * 36 + 35] = i * 8 + 5;
                }
                CalculateNormals(ref _VertexList, _VertexIndexList);
                NotifyMeshChanged();
            }
        }

        public override float GetCellValue(int p1, int p2)
        {
            throw new NotImplementedException();
        }

        public override Models.Generic.ICell SelectCell(double lat, double lon)
        {
            throw new NotImplementedException();
        }

        public override void UpdateVertexColor()
        {
                CreatMeshes();
        }

        public override void CacheColor()
        {
            throw new NotImplementedException();
        }

        public override void UpdateCachedColor()
        {
            throw new NotImplementedException();
        }

        public override void UpdateCellValues()
        {
            throw new NotImplementedException();
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
