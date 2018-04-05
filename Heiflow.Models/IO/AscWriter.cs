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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using Heiflow.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DotNetDBF;
using Heiflow.Core.Data;

namespace Heiflow.Models.IO
{
    public class AscWriter : FileProvider
    {
        public AscWriter()
        {
            Extension = ".asc";
            FileTypeDescription = "ASC file";
        }

        public override void Initialize()
        {
           
        }

        public void Save<T>(string ascfile, T[][] mat, int nrow, int ncol, int cellsize, float xcorner, float ycorner, float nodatavalue)
        {
            StreamWriter sw = new StreamWriter(ascfile);
            string line = string.Format("ncols {0}\nnrows {1}\nxllcorner {2}\nyllcorner {3}\ncellsize {4}\nNODATA_value {5}", ncol, nrow, xcorner, ycorner, cellsize, nodatavalue);
            sw.WriteLine(line);
            for (int i = 0; i < mat.Length; i++)
            {
                line = string.Join("\t", mat[i]);
                sw.WriteLine(line);
            }

            sw.Close();
        }

        public void Export(string dbfile, string ascfile, int nrow, int ncol, int cellsize, float xcorner, float ycorner, float nodatavalue = -999)
        {
            DBFReader dbf = new DBFReader(dbfile);
            int act_index = dbf.GetFiledNameIndex("active");
            int row_index = dbf.GetFiledNameIndex("row");
            int col_index = dbf.GetFiledNameIndex("col");
            int elev_index = dbf.GetFiledNameIndex("elev");
            My2DMat<float> mat = new My2DMat<float>(nrow, ncol);
            int nact = 0;
            for (int n = 0; n < dbf.RecordCount; n++)
            {
                var obj = dbf.NextRecord();
                int row = int.Parse(obj[row_index].ToString());
                int col = int.Parse(obj[col_index].ToString());
                int act = int.Parse(obj[act_index].ToString());
                if (act > 0)
                {
                    mat.Value[row - 1][col - 1] = float.Parse(obj[elev_index].ToString());
                    nact++;
                }
                else
                    mat.Value[row - 1][col - 1] = nodatavalue;           
            }

            dbf.Close();
            this.Save<float>(ascfile, mat.Value, nrow, ncol, cellsize, xcorner, ycorner, nodatavalue);

        }


    }
}
