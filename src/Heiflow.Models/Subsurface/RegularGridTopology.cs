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

using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using ILNumerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Subsurface
{
    /// <summary>
    /// note that row or column starting from 0
    /// </summary>
    public class RegularGridTopology : IGridTopology
    {
        /// <summary>
        /// ID starts from 1, index starts from 0
        /// </summary>
        /// <param name="mg"></param>
        public RegularGridTopology()
        {
 
        }

        private RegularGrid _Grid;

        public RegularGrid Grid
        {
            get
            {
                return _Grid;
            }
            set
            {
                _Grid = value;
            }
        }
        public int RowCount
        {
            get;
            set;
        }
        public int ColumnCount
        {
            get;
            set;
        }
        public int ActiveCellCount
        {
            get;
            set;
        }
        public int ActiveVertexCount { get; private set; }
        /// <summary>
        /// [CellID, nw vertex Index,ne vertex Index, se vertex Index,sw vertex Index].
        /// </summary>
        public int[,] CellVertex { get; private set; }
        /// <summary>
        /// [nw cell ID,ne ID, se ID,sw ID, row, column].  存储所有网格节点的连接信息, 数组索引为节点的序列(逐行); cell ID索引从1开始 
        /// </summary>
        public int[,] VertexAtActiveCells { get; private set; }

        /// <summary>
        ///(Cell ID, Vertex Index) 
        /// </summary>
        public Dictionary<int, int> ActiveVertexIndex { get; set; }
        ///// <summary>
        ///// Cell ID, cell index in a array starting from 0
        ///// </summary>
        //public Dictionary<int, int> ActiveCellIndex { get; set; }
        ///// <summary>
        ///// cell index in a array starting from 0, Cell ID
        ///// </summary>
        //public Dictionary<int, int> ActiveCellID { get; set; }
        /// <summary>
        /// [Cell index][row,col] cell index starts from 0.  存储活动网格位置(行列索引，从0开始)
        /// </summary>
        //public Dictionary<int, int[]> ActiveCellLocation { get; private set; }
        public int[][] ActiveCell { get;  set; }
        public int[] ActiveCellIDs { get;  set; }
        /// <summary>
        /// mapping between Cell ID  (starts from 1) and Cell Serial Index (starts from 0)
        /// </summary>
        public Dictionary<int, int> CellID2CellIndex { get; private set; }

        public void Build()
        {
            RowCount = Grid.RowCount;
            ColumnCount = Grid.ColumnCount;
            ActiveCellCount = Grid.ActiveCellCount;
            int row = Grid.RowCount;
            int col = Grid.ColumnCount;
            bool[,] isActVer = new bool[row + 1, col + 1];
            var ibound = Grid.IBound;
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    if (ibound[0, r, c] != 0)
                    {
                        isActVer[r, c] = true;
                        isActVer[r, c + 1] = true;
                        isActVer[r + 1, c] = true;
                        isActVer[r + 1, c + 1] = true;
                    }
                }
            }
            int i = 0;
            var acc = isActVer.Cast<bool>().Select(ac => ac == true).Count();
            ActiveVertexCount = acc;
            VertexAtActiveCells = new int[acc, 6];
            ActiveVertexIndex = new Dictionary<int, int>();
            //ActiveCellIndex = new Dictionary<int, int>();
            //ActiveCellID = new Dictionary<int, int>();

            for (int r = 0; r < row + 1; r++)
            {
                for (int c = 0; c < col + 1; c++)
                {
                    if (isActVer[r, c])
                    {
                        VertexAtActiveCells[i, 4] = r;
                        VertexAtActiveCells[i, 5] = c;
                        var cid = GetID(r, c);
                        if (!ActiveVertexIndex.ContainsKey(cid))
                            ActiveVertexIndex.Add(cid, i);
                        bool isside = false;
                        float[] isactive = null;
                        int [] cids = null;

                        if (r == 0 || r == row || c == 0 || c == col)
                        {
                            isside = true;
                            var safer = r;
                            var safec = c;
                            if (r == row)
                                safer = r - 1;
                            if (c == col)
                                safec = c - 1;

                            var xx = new int[] { r - 1, c - 1, r - 1, c, r, c, r, c - 1 };

                            for (int n = 0; n < 4; n++ )
                            {
                                if (xx[2 * n] >= 0 && xx[2 * n] <= row - 1 && xx[2 * n + 1] >= 0 && xx[2 * n + 1] <= col - 1 && ibound[0, xx[2 * n], xx[2 * n+1]] > 0)
                                {
                                    isactive = new float[] { 1, 1, 1, 1 };
                                    var temp = GetID(xx[2 * n], xx[2 * n + 1]);
                                    cids = new int[] { temp, temp, temp, temp };                 
                                    break;
                                }
                            }
                        }
                        else
                        {
                            isactive = new float[] { ibound[0, r - 1, c - 1], ibound[0, r - 1, c], ibound[0, r, c], ibound[0, r, c - 1] };
                            cids = new int[4] { GetID(r - 1, c - 1), GetID(r - 1, c), GetID(r, c), GetID(r, c - 1) };
                            foreach(var act in isactive)
                            {
                                if(act == 0)
                                {
                                    isside = true;
                                    break;
                                }
                            }              
                        }

                        if (isside)
                        {
                            for (int n = 0; n < 4; n++)
                            {
                                if (isactive[n] != 0)
                                {
                                    VertexAtActiveCells[i, 0] = cids[n];
                                    VertexAtActiveCells[i, 1] = cids[n];
                                    VertexAtActiveCells[i, 2] = cids[n];
                                    VertexAtActiveCells[i, 3] = cids[n];
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int n = 0; n < 4; n++)
                            {
                                VertexAtActiveCells[i, n] = cids[n];
                            }
                        }
                        i++;
                    }       
                }
            }

            i = 0;
            CellVertex = new int[Grid.ActiveCellCount, 5];
            //ActiveCellLocation = new Dictionary<int, int[]>();
            ActiveCell=new int[Grid.ActiveCellCount][];
            ActiveCellIDs = new int[Grid.ActiveCellCount];
            CellID2CellIndex = new Dictionary<int, int>();

                for (int r = 0; r < row; r++)
                {
                    for (int c = 0; c < col; c++)
                    {
                        if (ibound[0, r, c] != 0)
                        {
                            CellVertex[i, 0] = GetID(r, c);
                            CellVertex[i, 1] = ActiveVertexIndex[GetID(r, c)];
                            CellVertex[i, 2] = ActiveVertexIndex[GetID(r, c + 1)];
                            CellVertex[i, 3] = ActiveVertexIndex[GetID(r + 1, c + 1)];
                            CellVertex[i, 4] = ActiveVertexIndex[GetID(r + 1, c)];
                            //ActiveCellLocation.Add(CellVertex[i, 0], new int[] { r, c });
                            //ActiveCellIndex.Add(CellVertex[i, 0], i);
                            //ActiveCellID.Add(i, CellVertex[i, 0]);
                            ActiveCell[i] = new int[] { r, c };
                            ActiveCellIDs[i] = GetID(r, c);
                            CellID2CellIndex.Add(ActiveCellIDs[i], i);
                            i++;
                        }
                    }
                }
        }
        /// <summary>
        /// The generated ID starts from 1 rather than 0
        /// </summary>
        /// <param name="row">row index starting from 0</param>
        /// <param name="col">col index starting from 0</param>
        /// <returns></returns>
        public int GetID(int row, int col)
        {
            return row * Grid.ColumnCount + (col + 1);
        }

        /// <summary>
        /// get serial index which starts from 0
        /// </summary>
        /// <param name="row">row index starting from 0</param>
        /// <param name="col">col index starting from 0</param>
        /// <returns></returns>
        public int GetIndex(int row, int col)
        {
            var id = GetID(row, col);
            if (CellID2CellIndex.Keys.Contains(id))
                return CellID2CellIndex[id];
            else
                return -1;
        }

        /// <summary>
        /// get serial index which starts from 0
        /// </summary>
        /// <param name="row">row index starting from 0</param>
        /// <param name="col">col index starting from 0</param>
        /// <returns></returns>
        public int GetSerialIndex(int row, int col)
        {
            var id = GetID(row, col);
            if (CellID2CellIndex.Keys.Contains(id))
                return CellID2CellIndex[id];
            else
                return -1;
        }

        public float GetVertexValue(float[,] matrix, int index)
        {
            int[] lc = null;
            float sum = 0;
            int count = 0;
            for (int i = 0; i < 4; i++)
            {
                int id = VertexAtActiveCells[index, i];
                if (id != 0)
                {
                    var loc = CellID2CellIndex[id];
                    lc = ActiveCell[loc];
                    sum += matrix[lc[0], lc[1]];
                    count++;
                }
            }
            return sum / count;
        }

        public float GetUniqueVertexValue(float[] vecotr, int index)
        {
            int id = VertexAtActiveCells[index, 0];
            if (id != 0)
            {
                var lc = CellID2CellIndex[id];  
                return vecotr[lc];
            }
            else
            {
                return 0;
            }
        }

        public float GetVertexValue(int[] matrix, int index)
        {
            int ci = 0;
            float sum = 0;
            int count = 0;
            for (int i = 0; i < 4; i++)
            {
                int id = VertexAtActiveCells[index, i];
                if (id != 0)
                {
                    ci = CellID2CellIndex[id];
                    sum += matrix[ci];
                    count++;
                }
            }
            return (sum / count);
        }

        public float GetVertexValue(float[] matrix, int index)
        {
            int ci = 0;
            float sum = 0;
            int count = 0;
            for (int i = 0; i < 4; i++)
            {
                int id = VertexAtActiveCells[index, i];
                if (id != 0)
                {
                    ci = CellID2CellIndex[id];
                    sum += matrix[ci];
                    count++;
                }
            }
            if (count == 0)
            {
                return 0;
            }
            else
            {
                return (sum / count);
            }
        }

        public int GetVertexValue(int[,] matrix, int index, float scalor)
        {
            int[] lc = null;
            int sum = 0;
            int count = 0;
            for (int i = 0; i < 4; i++)
            {
                int id = VertexAtActiveCells[index, i];
                if (id != 0)
                {
                    var loc = CellID2CellIndex[id];
                    lc = ActiveCell[loc];
                    sum += matrix[lc[0], lc[1]];
                    count++;
                }
            }
            if (count == 0)
            {
                count = count == 0 ? 1 : count;
            }

            return (int)(sum / count / scalor);
        }

        /// <summary>
        /// return Max and Min values
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public float[] GetActiveCellValueRange(float[,] matrix)
        {
            float[] range = new float[2];
            int row = Grid.RowCount;
            int col = Grid.ColumnCount;
            //var ibound = Grid.IBound[ILMath.full, ILMath.full, 0];
            var ibound = Grid.IBound;
            var list = new List<float>();
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    if (ibound[0, r, c] != 0)
                    {
                        list.Add(matrix[r, c]);
                    }
                }
            }
            range[0] = list.Max();
            range[1] = list.Min();
            return range;
        }
        public float[] GetUniqueValues(float[,] matrix)
        {
            int row = Grid.RowCount;
            int col = Grid.ColumnCount;
            var list = new List<float>();
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    if (!list.Contains(matrix[r, c]))
                    {
                        list.Add(matrix[r, c]);
                    }
                }
            }
            return list.ToArray();
        }

        public int[] GetActiveCellValueRange(int[,] matrix)
        {
            int[] range = new int[2];
            int row = Grid.RowCount;
            int col = Grid.ColumnCount;
            var ibound = Grid.IBound;
            var list = new List<int>();
            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    if (ibound[0, r, c] != 0)
                    {
                        list.Add(matrix[r, c]);
                    }
                }
            }
            range[0] = list.Max();
            range[1] = list.Min();
            return range;
        }
        public int[] GetActiveCellValueRange(int[] matrix)
        {
            int[] range = new int[2];
            range[0] = matrix.Max();
            range[1] = matrix.Min();
            return range;
        }

        public float[] GetActiveCellValueRange(float[] matrix)
        {
            float[] range = new float[2];
            range[0] = matrix.Max();
            range[1] = matrix.Min();
            return range;
        }
    }

}