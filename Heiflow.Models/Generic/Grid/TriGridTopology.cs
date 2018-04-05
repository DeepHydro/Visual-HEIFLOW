// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
