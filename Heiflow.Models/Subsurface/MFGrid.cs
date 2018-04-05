// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using ILNumerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Data;
using DotSpatial.Data;
using System.Data;

namespace Heiflow.Models.Subsurface
{
    public class MFGrid : RegularGrid
    {
        public MFGrid()
        {

        }
        public My2DMat<float> CentralPoint
        {
            get;
            set;
        }
        private int[] _ExtractedCellIndex;
     
        /// <summary>
        /// Extract a sub grid from exsiting grid. All the index starts from 0
        /// </summary>
        /// <param name="lurow"></param>
        /// <param name="lucol"></param>
        /// <param name="rlrow"></param>
        /// <param name="rlcol"></param>
        /// <returns></returns>
        public IRegularGrid Extract(IFeatureSet fs,  int lurow, int lucol, int rlrow, int rlcol)
        {
            MFGrid newgrid = new MFGrid();
            int nrow = rlrow - lurow + 1;
            int ncol = rlcol - lucol + 1;
            newgrid.ActualLayerCount = this.ActualLayerCount;
            newgrid.IBound = new MyVarient3DMat<float>(this.ActualLayerCount, nrow, ncol);
            newgrid.RowCount = nrow;
            newgrid.ColumnCount = ncol;
            newgrid.BBox = this.BBox;
            newgrid.DELR = this.DELR;
            newgrid.DELC = this.DELC;

            var dt = fs.DataTable.AsEnumerable();
            var nact= (from dr in dt where dr.Field<Int16>("Extraction") != 0 select dr).Count();
            newgrid.ActiveCellCount = nact;
            _ExtractedCellIndex = new int[newgrid.ActiveCellCount];


            int index=0;
            for (int l = 0; l < this.ActualLayerCount; l++)
            {
                foreach (var dr in dt)
                {
                    int r = (int)dr.Field<Int64>("ROW") - 1;
                    int c = (int)dr.Field<Int64>("COLUMN") - 1;
                    var isactive = dr.Field<Int16>("Extraction");
                   
                    if(isactive != 0)
                    {
                        newgrid.IBound[l, r - lurow, c - lucol] = isactive;
                    }
                }
            }

            index = 0;
            for (int r = 0; r < nrow;r++ )
            {
                for (int c = 0; c < ncol; c++)
                {
                    if(newgrid.IBound[0, r, c ] != 0)
                    {
                        _ExtractedCellIndex[index] = this.Topology.GetIndex(r + lurow, c + lucol);
                        index++;
                    }
                }
            }

          // newgrid.Elevations = MyMath.zeros<float>(this.LayerCount, newgrid.ActiveCellCount);
            newgrid.Elevations = new MyVarient3DMat<float>(this.LayerCount, 1,newgrid.ActiveCellCount);
   
            for (int l = 0; l < this.LayerCount; l++)
            {
                //var mat = GetSubSerialArray<float>(this.Elevations, newgrid, l);
                //newgrid.Elevations[l, 0] = mat.Value;
            }

            return newgrid;
        }

        public override void BuildTopology()
        {
            this.Topology = new RegularGridTopology();
            Topology.Build();
        }
    }
}
