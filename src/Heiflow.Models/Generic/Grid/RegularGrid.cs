//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ILNumerics;
using ILNumerics.Data;
using DotSpatial.Data;
using Heiflow.Models.Subsurface;
using Heiflow.Core.Data;
using System.IO;
using DotSpatial.Controls;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using DotSpatial.Projections.ProjectedCategories;
using DotSpatial.Projections;
using System.ComponentModel;

namespace Heiflow.Models.Generic
{
    public class RegularGrid : IRegularGrid
    {
        protected RegularGridTopology _GridTopo;
        public event EventHandler SizeChanged;
        public event GridUpdate Updated;
        private int _RowCount;
        private int _ColumnCount;
        private int _ActualLayerCount;
        private int _LayerCount;
        private int _UTMZone;

        public RegularGrid()
        {
            _UTMZone = 40;
        }

        public static string ParaValueField
        {
            get
            {
                return "ParaValue";
            }
        }

        public static double NoValue
        {
            get
            {
                return -999;
            }
        }
        public int RowCount
        {
            get
            {
                return _RowCount;
            }
            set
            {
                _RowCount = value;
                OnSizeChanged();
            }
        }

        public int ColumnCount
        {
            get
            {
                return _ColumnCount;
            }
            set
            {
                _ColumnCount = value;
                OnSizeChanged();
            }
        }

        public int ActualLayerCount
        {
            get
            {
                return _ActualLayerCount;
            }
            set
            {
                _ActualLayerCount = value;
                OnSizeChanged();
            }
        }

        public int LayerCount
        {
            get
            {
                _LayerCount = _ActualLayerCount + 1;
                return _LayerCount;
            }
        }

        public int ActiveCellCount
        {
            get;
            set;
        }
        [Browsable(false)]
        public RegularGridTopology Topology
        {
            get
            {
                return _GridTopo;
            }
            set
            {
                _GridTopo = value;
                _GridTopo.Grid = this;
            }
        }
        public Coordinate Origin
        {
            get;
            set;
        }
        public Coordinate BBoxCentroid
        {
            get;
            set;
        }
        public DotSpatial.Projections.ProjectionInfo Projection
        {
            get;
            set;
        }

        /// <summary>
        ///  3dmat[ LayerCount,1,ActiveCellCount]
        /// </summary>
        public DataCube<float> Elevations
        {
            get;
            set;
        }
        /// <summary>
        /// dc[1,1,ActiveCellCount]. The values represent percent slope.
        /// </summary>
        public DataCube<float> Slope
        {
            get;
            set;
        }
        /// <summary>
        ///   dc[1,1,ActiveCellCount]. The aspect value is in compass direction values (0-360 degrees). 
        /// </summary>
        public DataCube<float> Aspect
        {
            get;
            set;
        }
        /// <summary>
        /// mat[ActualLayerCount, Row, Col]
        /// </summary>
        public DataCube<float> IBound
        {
            get;
            set;
        }

        public IBasicModel Owner
        {
            get;
            set;
        }

        public Envelope BBox
        {
            get;
            set;
        }

        public DotSpatial.Data.IFeatureSet FeatureSet
        {
            get;
            set;
        }

        public IFeatureSet CentroidFeature
        {
            get;
            set;
        }
        public IMapFeatureLayer FeatureLayer
        {
            get;
            set;
        }

        public IMapFeatureLayer CentroidFeatureLayer
        {
            get;
            set;
        }
        /// <summary>
        ///  mat[0][0][NCOL]. The cell width along rows. Read one value for each of the NCOL columns.
        /// </summary>
        public DataCube<float> DELR
        {
            get;
            set;
        }
        /// <summary>
        ///  mat[0][0][NROW]. the cell width along columns. Read one value for each of the NROW rows. 
        /// </summary>
        public DataCube<float> DELC
        {
            get;
            set;
        }

        public int UTMZone
        {
            get
            {
                return _UTMZone;
            }
            set
            {
                _UTMZone = value;
            }
        }

        public bool Validate()
        {
            if (FeatureSet != null)
            {
                var dt = FeatureSet.DataTable;
                if (dt.Rows.Count == ActiveCellCount)
                {
                    if (!dt.Columns.Contains(ParaValueField))
                    {
                        dt.Columns.Add(new DataColumn(ParaValueField, Type.GetType("System.Double")));
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dt.Rows[i][ParaValueField] = 0;
                        }
                        FeatureSet.Save();
                    }
                    return true;
                }
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Get X,Y axis expressed by longitude and latitude
        /// </summary>
        /// <returns>mat[2][], mat[0] is longitude, mat[1] is latitude </returns>
        public float[][] GetLonLatAxis()
        {
            var lonlat = new float[2][];
            lonlat[0] = new float[ColumnCount];
            lonlat[1] = new float[RowCount];

            var wgs84 = ProjectionInfo.FromEpsgCode(4326);
            IFeatureSet fs_lon = new FeatureSet(FeatureType.Point);
            fs_lon.Projection = this.Projection;
            for (int c = 0; c < ColumnCount; c++)
            {
                var vertice = LocateCentroid(c + 1, 1);
                Point pt = new Point(vertice);
                fs_lon.AddFeature(pt);
            }
            if (fs_lon.Projection != null)
            {
                fs_lon.Reproject(wgs84);
            }
            for (int c = 0; c < ColumnCount; c++)
            {
                var fea = fs_lon.GetFeature(c).Geometry.Coordinate;
                lonlat[0][c] = (float)fea.X;
            }


            IFeatureSet fs_lat = new FeatureSet(FeatureType.Point);
            fs_lat.Projection = this.Projection;
            for (int r = 0; r < RowCount; r++)
            {
                var vertice = LocateCentroid(1, r + 1);
                Point pt = new Point(vertice);
                fs_lat.AddFeature(pt);
            }
            if (fs_lat.Projection != null)
            {
                fs_lat.Reproject(wgs84);
            }
            for (int r = 0; r < RowCount; r++)
            {
                var fea = fs_lat.GetFeature(r).Geometry.Coordinate;
                lonlat[1][r] = (float)fea.Y;
            }

            return lonlat;
        }

        public float[][] GetPCSAxis()
        {
            var XY = new float[2][];
            XY[0] = new float[ColumnCount];
            XY[1] = new float[RowCount];

            var wgs84 = ProjectionInfo.FromEpsgCode(4326);
            IFeatureSet fs_lon = new FeatureSet(FeatureType.Point);
            fs_lon.Projection = this.Projection;
            for (int c = 0; c < ColumnCount; c++)
            {
                var vertice = LocateCentroid(c + 1, 1);
                XY[0][c] = (float)vertice.X;
            }

            for (int r = 0; r < RowCount; r++)
            {
                var vertice = LocateCentroid(1, r + 1);
                XY[1][r] = (float)vertice.Y;
            }

            return XY;
        }

        public void Build(string filename)
        {
            FeatureSet fs = new FeatureSet(FeatureType.Polygon);
            fs.Name = "Grid";
            fs.Projection = this.Projection;
            fs.DataTable.Columns.Add(new DataColumn("HRU_ID", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("CELL_ID", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("ROW", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("COLUMN", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn(ParaValueField, typeof(double)));
            if (Origin == null)
                Origin = new Coordinate(0, 0);
            int active = 1;
            for (int r = 0; r < RowCount; r++)
            {
                for (int c = 0; c < ColumnCount; c++)
                {
                    if (IBound[0, r, c] != 0)
                    {
                        // create a geometry (square polygon)
                        var vertices = LocateNodes(c, r);
                        ILinearRing ring = new LinearRing(vertices);
                        Polygon geom = new Polygon(ring);

                        // add the geometry to the featureset. 
                        IFeature feature = fs.AddFeature(geom);

                        // now the resulting features knows what columns it has
                        // add values for the columns
                        feature.DataRow.BeginEdit();
                        feature.DataRow["HRU_ID"] = active;
                        feature.DataRow["CELL_ID"] = Topology.GetID(r, c);
                        feature.DataRow["ROW"] = r + 1;
                        feature.DataRow["COLUMN"] = c + 1;
                        feature.DataRow[ParaValueField] = 0;
                        feature.DataRow.EndEdit();
                        active++;
                    }
                }
            }
            fs.SaveAs(filename, true);
            fs.Close();
        }

        public void BuildCentroid(string filename)
        {
            FeatureSet fs = new FeatureSet(FeatureType.Point);
            fs.Projection = this.Projection;
            fs.Name = "Grid Centroid";
            fs.DataTable.Columns.Add(new DataColumn("HRU_ID", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("CELL_ID", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("ROW", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("COLUMN", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn(ParaValueField, typeof(double)));
            if (Origin == null)
                Origin = new Coordinate(0, 0);
            int active = 1;
            for (int r = 0; r < RowCount; r++)
            {
                for (int c = 0; c < ColumnCount; c++)
                {
                    if (IBound[0, r, c] != 0)
                    {
                        // create a geometry (square polygon)
                        var vertice = LocateCentroid(c + 1, r + 1);
                        Point geom = new Point(vertice);

                        // add the geometry to the featureset. 
                        IFeature feature = fs.AddFeature(geom);

                        // now the resulting features knows what columns it has
                        // add values for the columns
                        feature.DataRow.BeginEdit();
                        feature.DataRow["HRU_ID"] = active;
                        feature.DataRow["CELL_ID"] = Topology.GetID(r, c);
                        feature.DataRow["ROW"] = r + 1;
                        feature.DataRow["COLUMN"] = c + 1;
                        feature.DataRow[ParaValueField] = 0;
                        feature.DataRow.EndEdit();
                        active++;
                    }
                }
            }
            fs.SaveAs(filename, true);
            fs.Close();

        }

        /// <summary>
        ///  Convert vector to grid matrix [nrow][ncol]
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="vector">vector storing serial value</param>
        /// <param name="novalue">no data value</param>
        /// <returns></returns>
        public T[][] ToMatrix<T>(T[] vector, T novalue)
        {
            T[][] matrix = new T[RowCount][];
            for (int r = 0; r < RowCount; r++)
            {
                matrix[r] = new T[ColumnCount];
                for (int c = 0; c < ColumnCount; c++)
                {
                    matrix[r][c] = novalue;
                }
            }

            for (int i = 0; i < ActiveCellCount; i++)
            {
                var lc = Topology.ActiveCellLocation[i];
                matrix[lc[0]][lc[1]] = vector[i];
            }
            return matrix;
        }

        public T[, ,] To3DMatrix<T>(T[] vector, T novalue)
        {
            T[, ,] matrix = new T[1, RowCount, ColumnCount];

            for (int i = 0; i < ActiveCellCount; i++)
            {
                int[] loc = Topology.ActiveCellLocation[i];
                matrix[0, loc[0], loc[1]] = vector[i];
            }
            return matrix;
        }
        /// <summary>
        /// Convert vector to grid matrix ILArray[nrow,ncol]
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="vector">vector storing serial value</param>
        /// <param name="novalue">no data value</param>
        /// <returns></returns>
        public ILArray<T> ToILMatrix<T>(ILArray<T> vector, T novalue)
        {
            ILArray<T> matrix = ILMath.zeros<T>(RowCount, ColumnCount);
            //matrix = novalue;
            for (int i = 0; i < ActiveCellCount; i++)
            {
                int[] loc = Topology.ActiveCellLocation[i];
                matrix[RowCount - loc[0] - 1, loc[1]] = vector.GetValue(i);
            }

            return matrix;
        }

        public ILArray<T> ToILMatrix<T>(object[] vector, T novalue)
        {
            ILArray<T> matrix = ILMath.zeros<T>(RowCount, ColumnCount);
            //matrix = novalue;
            for (int i = 0; i < ActiveCellCount; i++)
            {
                int[] loc = Topology.ActiveCellLocation[i];
                matrix[RowCount - loc[0] - 1, loc[1]] = (T)vector[i];
            }

            return matrix;
        }

        private void OnSizeChanged()
        {
            if (SizeChanged != null)
            {
                SizeChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// return row, column index starting from 0
        /// </summary>
        /// <returns></returns>
        public int[] DeterminIndex(Coordinate cod, Coordinate upl, double delx, double dely)
        {
            int[] indexes = new int[2];
            double xx = cod.X - upl.X;
            double yy = upl.Y - cod.Y;
            indexes[0] = (int)Math.Ceiling(yy / dely) - 1;
            indexes[1] = (int)Math.Ceiling(xx / delx) - 1;
            indexes[0] = indexes[0] < 0 ? 0 : indexes[0];
            indexes[1] = indexes[1] < 0 ? 0 : indexes[1];
            return indexes;
        }
        /// <summary>
        /// row, column indexes starting from 1
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public int DeterminID(int rowIndex, int colIndex)
        {
            return (rowIndex - 1) * ColumnCount + colIndex;
        }
        /// <summary>
        /// The returned node is located in SE. All the index starting from 1
        /// </summary>
        /// <param name="col">the col index starting from 1</param>
        /// <param name="row">the row index starting from 1</param>
        /// <returns></returns>
        public Coordinate LocateNode(int col, int row)
        {
            Coordinate c = new Coordinate();

            if (BBox != null)
            {
                double dx = 0;
                double dy = 0;
                dx = DELR[0, 0, col - 1];
                dy = DELC[0, 0, row - 1];
                c.X = Origin.X + col * dx;
                c.Y = Origin.Y - row * dy;
            }
            return c;
        }

        /// <summary>
        /// Return the centroid location of a given cell. All the index starting from 1
        /// </summary>
        /// <param name="col">the col index starting from 1</param>
        /// <param name="row">the row index starting from 1</param>
        /// <returns>centroid location</returns>
        public Coordinate LocateCentroid(int col, int row)
        {
            Coordinate c = new Coordinate();
            double dx = 0;
            double dy = 0;
            dx = DELR[0, 0, col - 1];
            dy = DELC[0, 0, row - 1];
            c.X = Origin.X + col * dx - 0.5 * dx;
            c.Y = Origin.Y - row * dy + 0.5 * dy;
            return c;
        }
        /// <summary>
        /// Return the lower left location of a given cell. All the index starting from 1
        /// </summary>
        /// <param name="col">the col index starting from 1</param>
        /// <param name="row">the row index starting from 1</param>
        /// <returns></returns>
        public Coordinate LocateLowerLeft(int col, int row)
        {
            Coordinate c = new Coordinate();
            double dx = 0;
            double dy = 0;
            dx = DELR[0, 0, col - 1];
            dy = DELC[0, 0, row - 1];
            c.X = Origin.X + (col - 1) * dx;
            c.Y = Origin.Y - row * dy;
            return c;
        }
        /// <summary>
        /// Locate four nodes of a cell. (se,ne, nw, sw)
        /// </summary>
        /// <param name="col">index starting from 0</param>
        /// <param name="row">index starting from 0</param>
        /// <returns></returns>
        public Coordinate[] LocateNodes(int col, int row)
        {
            Coordinate[] coods = new Coordinate[5];
            double dx = 0;
            double dy = 0;
            dx = DELR[0, 0, col];
            dy = DELC[0, 0, row];
            for (int i = 0; i < 4; i++)
            {
                coods[i] = new Coordinate();
            }

            coods[0].X = Origin.X + (col + 1) * dx;
            coods[0].Y = Origin.Y - (row + 1) * dy;


            coods[1].X = coods[0].X;
            coods[1].Y = coods[0].Y + dy;

            coods[2].X = coods[0].X - dx;
            coods[2].Y = coods[0].Y + dy;

            coods[3].X = coods[0].X - dx;
            coods[3].Y = coods[0].Y;

            coods[4] = new Coordinate(coods[0].X, coods[0].Y);
            return coods;
        }
        /// <summary>
        /// hru_index starts from 0
        /// </summary>
        /// <param name="hru_index"></param>
        /// <returns></returns>
        public Coordinate[] LocateNodes(int hru_index)
        {
            var cell = Topology.ActiveCellLocation[hru_index];
            return LocateNodes(cell[1], cell[0]);
        }
        public virtual void BuildTopology()
        {

        }

        /// <summary>
        /// get cell elevation
        /// </summary>
        /// <param name="row">index starts from 0</param>
        /// <param name="col">index starts from 0</param>
        /// <param name="layer">index starts from 0</param>
        /// <returns>elevation</returns>
        public float GetElevationAt(int row, int col, int layer)
        {
            float ele = 0;
            if (Elevations != null)
            {
                var index = Topology.GetSerialIndex(row, col);
                if (index >= 0)
                    ele = Elevations[layer, 0, index];
            }
            return ele;
        }
        /// <summary>
        /// check whether a cell is active. all index start from 0
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool IsActive(int row, int col, int layer)
        {
            if (row >= IBound.Size[1] || col >= IBound.Size[2] || layer >= IBound.Size[0])
                return false;
            else
                return (IBound[layer, row, col] != 0);
        }

        public void Extent(SpatialReference srf)
        {
            this.BBox = new Envelope(srf.X, srf.X + ColumnCount * DELR[0, 0, 0], srf.Y - RowCount * DELR[0, 0, 0], srf.Y);
            this.Origin = new Coordinate(srf.X, srf.Y);
        }

        public void RaiseUpdate()
        {
            if (Updated != null)
                Updated(this);
        }


        /// <summary>
        /// get total area in squired meters
        /// </summary>
        /// <returns></returns>
        public double GetTotalArea()
        {
            var cellarea = DELR.Constants[0] * DELC.Constants[0];
            return cellarea * ActiveCellCount;
        }

        /// <summary>
        /// get cell area in squired meters
        /// </summary>
        /// <returns></returns>
        public double GetCellArea()
        {
            var cellarea = DELR.Constants[0] * DELC.Constants[0];
            return cellarea;
        }
    }
}