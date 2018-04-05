// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.IO;
using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.GHM
{
    public class AscGridProvider :IGridFileProvider
    {
        public AscGridProvider()
        {

        }


        public string FileTypeDescription
        {
            get
            {
                return ".asc grid file";
            }
        }

        public string Extension
        {
            get
            {
                return ".asc";
            }
        }

        public string FileName
        {
            get;
            set;
        }

        public IGrid Provide(string filename)
        {
            AscReader asc = new AscReader();
            var mat = asc.Load(filename);
            var mfgrid = new MFGrid();
            mfgrid.RowCount = mat.Size[0];
            mfgrid.ColumnCount = mat.Size[1];
            //mfgrid.RowInteval = new MyScalar<float>(asc.CellSize);
            //mfgrid.ColInteval = new MyScalar<float>(asc.CellSize);

            mfgrid.IBound = new MyVarient3DMat<float>(1, mfgrid.RowCount, mfgrid.ColumnCount);

            mfgrid.ActiveCellCount = 0;

            for (int r = 0; r < mat.Size[0]; r++)
            {
                for (int c = 0; c < mat.Size[1]; c++)
                {
                    if (mat[r, c,MyMath.none] == asc.NoDataValue)
                        mfgrid.IBound[0, r, c] = 0;
                    else
                    {
                        mfgrid.IBound[0, r, c] = 1;
                        mfgrid.ActiveCellCount++;
                    }
                }
            }
            mfgrid.Elevations = new MyVarient3DMat<float>(1, 1, mfgrid.ActiveCellCount);

            var vector = asc.LoadSerial(filename, mfgrid);
            mfgrid.Elevations.Value[0][0] = vector;

            return mfgrid;
        }

        public void Save(string filename, IGrid grid)
        {
            throw new NotImplementedException();
        }
    }
}
