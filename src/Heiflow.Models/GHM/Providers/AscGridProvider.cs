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
using Heiflow.Models.IO;
using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.GHM
{
    [Export(typeof(IGridFileProvider))]
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
            mfgrid.RowCount = mat.Size[1];
            mfgrid.ColumnCount = mat.Size[2];
            //mfgrid.RowInteval = new MyScalar<float>(asc.CellSize);
            //mfgrid.ColInteval = new MyScalar<float>(asc.CellSize);

            mfgrid.IBound = new DataCube<float>(1, mfgrid.RowCount, mfgrid.ColumnCount);

            mfgrid.ActiveCellCount = 0;

            for (int r = 0; r < mat.Size[0]; r++)
            {
                for (int c = 0; c < mat.Size[1]; c++)
                {
                    if (mat[0,r, c] == asc.NoDataValue)
                        mfgrid.IBound[0, r, c] = 0;
                    else
                    {
                        mfgrid.IBound[0, r, c] = 1;
                        mfgrid.ActiveCellCount++;
                    }
                }
            }
            mfgrid.Elevations = new DataCube<float>(1, 1, mfgrid.ActiveCellCount);

            var vector = asc.LoadSerial(filename, mfgrid);
            mfgrid.Elevations[0]["0",":"] = vector;

            return mfgrid;
        }

        public void Save(string filename, IGrid grid)
        {
            throw new NotImplementedException();
        }
    }
}
