// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
