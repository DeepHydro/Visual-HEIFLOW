// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heiflow.Core.Data;
using GeoAPI.Geometries;

namespace Heiflow.Core.Hydrology
{
    public class Grid
    {
         public Grid()
        {
        }
         public Grid(int rowN, int colNum, Envelope bbox)
         {
             BBox = bbox;
             RowCount = rowN;
             ColumnCount = colNum;
             float ri = (float) bbox.Height / RowCount;
             float ci = (float)bbox.Width / ColumnCount;
             RowInteval = new MatrixCube<float>(1);
             ColInteval = new MatrixCube<float>(1);
             RowInteval.LayeredValues[0] = new float[1, rowN];
             ColInteval.LayeredValues[0] = new float[1, colNum];

             for (int i = 0; i < rowN; i++)
                 RowInteval.LayeredValues[0][0, i] = ri;
             for (int i = 0; i < colNum; i++)
                 ColInteval.LayeredValues[0][0, i] = ci;
         }

        public Envelope BBox { get; set; }

        public int RowCount { get;  set; }

        public int ColumnCount { get;  set; }

        /// <summary>
        /// equals to: actual layer count + 1
        /// </summary>
        public int LayerCount { get; set; }
        /// <summary>
        /// equals to: actual layer count + 1
        /// </summary>
        public int ActualLayerCount { get; set; }
        /// <summary>
        ///  dimension:1,ColumnCount 
        /// </summary>
        public MatrixCube<float> RowInteval
        {
            get;
            set;
        }

        public int ActiveCellCount { get; set; }
        /// <summary>
        /// ID,ROW,COLUMN start from 1,0,0 respectively
        /// </summary>
        public uint[,] ActiveCellIndex { get; set; }
        /// <summary>
        /// dimension:1,RowCount 
        /// </summary>
        public MatrixCube<float> ColInteval { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// return row, column index starting from 0
        /// </summary>
        /// <returns></returns>
        public int [] DeterminIndex(Coordinate cod, Coordinate upl, double delx,double dely)
        {
            int[] indexes = new int[2];
            double xx = cod.X - upl.X;
            double yy = upl.Y - cod.Y;
            indexes[0] = (int)Math.Ceiling(yy / dely)-1;
            indexes[1] = (int)Math.Ceiling(xx / delx)-1;
            indexes[0] = indexes[0] < 0 ? 0 : indexes[0];
            indexes[1] = indexes[1] < 0 ? 0 : indexes[1];
            return indexes;
        }
        /// <summary>
        /// row, column indexes starting from 1
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public int DeterminID(int rowIndex,int colIndex)
        {
            return (rowIndex - 1) * ColumnCount + colIndex;
        }
        /// <summary>
        /// index starting from 1
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public Coordinate LocateNode(int col,int row)
        {
            Coordinate c = new Coordinate();

            if (BBox != null)
            {
                var tl = new Coordinate(BBox.MinX, BBox.MinY);
                if (RowInteval.IsConstant[0])
                {
                    c.X = tl.X + col * RowInteval.LayeredConstantValues[0];
                }
                else
                {
                    c.X = tl.X + col * RowInteval.LayeredValues[0][0, col - 1];
                }

                if (ColInteval.IsConstant[0])
                {
                    c.Y = tl.Y + (RowCount - row) * ColInteval.LayeredConstantValues[0];
                }
                else
                {
                    c.Y = tl.Y + (RowCount - row) * ColInteval.LayeredValues[0][0, row - 1];
                }

            }
            return c;
        }
    }
}
