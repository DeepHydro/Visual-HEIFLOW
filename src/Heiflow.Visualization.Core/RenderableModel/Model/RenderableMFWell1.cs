using Heiflow.Controls.Project;
using Heiflow.Models.Subsurface;
using DotSpatial.Topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HUST.WREIS.Dot3D;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Heiflow.Visualization.Renderable.Grid
{
    public class RenderableMFWell
    {
        public RenderableMFWell(FHBPackage fhb, MFGrid grid, ProjectManager prjm, World world)
        {
            _FHBPackage = fhb;
            _MFGrid = grid;
            _World = world;
            MaxBarHeight = 5000;
            _ProjectManager = prjm;
        }

        public RenderableMFWell(WELPackage fhb, MFGrid grid, ProjectManager prjm, World world)
        {
            _WELPackage = fhb;
            _MFGrid = grid;
            _World = world;
            MaxBarHeight = 5000;
            _ProjectManager = prjm;
            Scale = 1.0f;
        }

        private WELPackage _WELPackage;
        private FHBPackage _FHBPackage;
        private MFGrid _MFGrid;
        private ProjectManager _ProjectManager;
        private World _World;
        private float _MaxBarHeight;

        private CustomVertex.PositionNormalColored[] _VertexList;
        private int[] _VertexIndexList;

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

        public CustomVertex.PositionNormalColored[] VertexList
        {
            get
            {
                return _VertexList;
            }
        }

        public int[] VertexIndexList
        {
            get
            {
                return _VertexIndexList;
            }
        }

        public void CreatMeshes()
        {
            MFWell[] wells = null;
            int layer = _ProjectManager.CurrentGridLayer;
            if (_FHBPackage != null && _FHBPackage.Wells != null)
            {             
                var twells = _FHBPackage.Group();
                wells = twells[layer];
            }
            //TODO
            if(_WELPackage != null && _WELPackage.Wells != null)
            {
                wells = _WELPackage.Wells[_ProjectManager.CurrentStessPeriod];
                if(wells == null)
                {
                    wells = _WELPackage.Wells[_ProjectManager.CurrentStessPeriod + 1];
                }
            }
            if (wells != null)
            {
                int nwell = wells.Length;
                var upleft = _MFGrid.BBox.TopLeft();
                var lowerRight = _MFGrid.BBox.BottomRight();
                double deltRow = Math.Abs(upleft.Y - lowerRight.Y) / _MFGrid.RowCount;
                double deltCol = Math.Abs(upleft.X - lowerRight.X) / _MFGrid.ColumnCount;

                GeoUTMConverter utm = new GeoUTMConverter()
                {
                    Hemi = GeoUTMConverter.Hemisphere.Northern
                };

                double lat = 0, lng = 0;
                double rlng = 0;
                double rlat = 0;
                double radius;

                var vec = (from w in wells select Math.Abs(w.PumpingRate)).ToArray();
                MaximumValue = vec.Max();
                MinimumValue = vec.Min();
                if (MinimumValue <= 0)
                    MinimumValue = 1;

                _VertexList = new CustomVertex.PositionNormalColored[nwell * 8];
                _VertexIndexList = new int[nwell * 36];
                for (int i = 0; i < nwell; i++)
                {
                    var well = wells[i];
                    var row = well.Row;
                    var col = well.Column;
                    int index = _MFGrid.Topology.GetIndex(row, col);

                    if (index == -1)
                        continue;
                    double[] XX = new double[4];
                    double[] YY = new double[4];

                    XX[0] = upleft.X + col * deltCol * Scale;
                    XX[1] = upleft.X + (col + 1) * deltCol * Scale;
                    XX[2] = upleft.X + (col + 1) * deltCol * Scale;
                    XX[3] = upleft.X + col * deltCol * Scale;

                    YY[0] = upleft.Y - row * deltRow * Scale;
                    YY[1] = upleft.Y - row * deltRow * Scale;
                    YY[2] = upleft.Y - (row + 1) * deltRow * Scale;
                    YY[3] = upleft.Y - (row + 1) * deltRow * Scale;
                    float elev = _MFGrid.Elevations[layer, index, 0];
                    double percent = vec[i] / (MaximumValue - MinimumValue);
                    var height = MaxBarHeight * percent;
                    var color = DataColor.GetColor(percent);

                    for (int j = 0; j < 4; j++)
                    {
                        utm.ToLatLon(XX[j], YY[j], 47, GeoUTMConverter.Hemisphere.Northern);
                        lng = utm.Longitude;
                        lat = utm.Latitude;

                        rlng = MathEngine.DegreesToRadians(lng);
                        rlat = MathEngine.DegreesToRadians(lat);

                        radius = _World.EquatorialRadius + elev * World.Settings.VerticalExaggeration;
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

                        radius = _World.EquatorialRadius + (elev + height) * World.Settings.VerticalExaggeration;
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
            }

        }

        protected void CalculateNormals(ref CustomVertex.PositionNormalColored[] vertices, int[] indices)
        {
            List<Vector3>[] normal_buffer = new List<Vector3>[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                normal_buffer[i] = new List<Vector3>();
            }
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3 p1 = vertices[indices[i + 0]].Position;
                Vector3 p2 = vertices[indices[i + 1]].Position;
                Vector3 p3 = vertices[indices[i + 2]].Position;

                Vector3 v1 = p2 - p1;
                Vector3 v2 = p3 - p1;
                Vector3 normal = Vector3.Cross(v1, v2);

                normal.Normalize();

                // Store the face's normal for each of the vertices that make up the face.
                normal_buffer[indices[i + 0]].Add(normal);
                normal_buffer[indices[i + 1]].Add(normal);
                normal_buffer[indices[i + 2]].Add(normal);
            }

            // Now loop through each vertex vector, and avarage out all the normals stored.
            for (int i = 0; i < vertices.Length; ++i)
            {
                for (int j = 0; j < normal_buffer[i].Count; ++j)
                {
                    Vector3 curNormal = normal_buffer[i][j];

                    if (vertices[i].Normal == Vector3.Empty)
                        vertices[i].Normal = curNormal;
                    else
                        vertices[i].Normal += curNormal;
                }
                vertices[i].Normal.Multiply(1.0f / normal_buffer[i].Count);
            }
        }
    }
}
