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

namespace Heiflow.Models.Generic
{
    public class TriGridTopology : ITriGridTopology
    {
        public TriGridTopology()
        {
        }

        private ITriangularGrid _Grid;

        private uint[][] mNodeConnectedCells;

        /// <summary>
        /// Index three vertexes that compose a triangular
        /// </summary>
        public uint[][] VertexIndices { get; set; }
        /// <summary>
        /// Store indexes of cells that connects to a same node
        /// </summary>
        public uint[][] NodeConnectedCells
        {
            get
            {
                return mNodeConnectedCells;
            }
            set
            {
                mNodeConnectedCells = value;
            }
        }

        public ITriangularGrid Grid
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

        /// <summary>
        /// Store  indexes of cells that surround a same cell, the number of the surrouding cells is up to three
        /// </summary>
        public uint[][] NeigbouringCells { get; set; }

       // public Coordinate[] CellCentroids { get; set; }

        /// <summary>
        /// used by directx to render the mesh
        /// </summary>
        public uint[] SuccessiveVertexIndices
        {
            get
            {
                int vc = VertexIndices[0].Length;
                int cellCount= VertexIndices.Length / vc;
             
                uint[] Indices = new uint[cellCount];

                for(int i=0; i<cellCount;i++)
                {
                    for (int j = 0; j < vc; j++)
                    {
                        Indices[i*vc + j] = VertexIndices[i][j];
                    }
                }

                return Indices;
            }
        }


        public float GetVertexValue(float[] vector, int i)
        {
            if (vector != null)
                return vector[i];
            else
                return 0;
        }
    }
}
