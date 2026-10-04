using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Heiflow.Models.IO;
using Heiflow.Core.Data;
using Heiflow.Core.IO;

namespace UnitTest
{
    public class ACWriter
    {
        public static void PreProcess()
        {
            string prefix = @"E:\Heihe\HRB\GeoData\GBHM\Model\section_v1.09_ylx\result\pre_";

            for (int y = 2000; y < 2013; y++)
            {
                for (int m = 1; m < 13; m++)
                {
                    string fn = prefix + y.ToString() + m.ToString("00") + ".asc";
                    string fn_out = fn.Replace("pre", "ppt");
                    StreamReader sr = new StreamReader(fn);
                    StreamWriter sw = new StreamWriter(fn_out);
                    string line = "";

                    for (int i = 0; i < 6; i++)
                    {
                        line = sr.ReadLine();
                        sw.WriteLine(line);
                    }

                    for (int i = 0; i < 255; i++)
                    {
                        line = sr.ReadLine();
                        line += sr.ReadLine();
                        sw.WriteLine(line);
                    }

                    sr.Close();
                    sw.Close();
                }
            }
        }

        public static void Save()
        {
            AscReader asc = new AscReader();
            string ascf = @"E:\Heihe\HRB\GeoData\GBHM\Model\section_v1.09_ylx\result\et_200001.asc";
            var vec = asc.LoadSerial(ascf, null);
            string[] vars = new string[] { "ET" };

            string prefix = @"E:\Heihe\HRB\GeoData\GBHM\Model\section_v1.09_ylx\result\ppt_";
            My3DMat<float> mat = new My3DMat<float>(1, 156, vec.Size[0]);
            int t = 0;
            for (int y = 2000; y < 2013; y++)
            {
                for (int m = 1; m < 13; m++)
                {
                    string fn = prefix + y.ToString() + m.ToString("00") + ".asc";
                    var buf = asc.LoadSerial(fn, null);
                    mat[0,t] = buf.GetSeriesAt(0,0);
                    t++;
                }
            }

      //      string acfile_fn = @"E:\Heihe\HRB\GeoData\GBHM\Model\section_v1.09_ylx\result\ppt.ac";
//            AcFile acfile = new AcFile();
   //         acfile.Save(acfile_fn, mat.Value, vars, mat.Size[2]);
        }


        public static void SaveForcings()
        {
            AscReader asc = new AscReader();
            string ascf = @"E:\Heihe\HRB\GeoData\GBHM\Model\section_v1.09_ylx\dem\elevation.asc";
            var vec = asc.LoadSerial(ascf, null);
            string varb = "wind";
            string[] vars = new string[] { varb };

            string prefix = @"E:\Heihe\HRB\GeoData\GBHM\Model\section_v1.09_ylx\data\forcing\" + varb +"_2009";
            My3DMat<float> mat = new My3DMat<float>(1, 59, vec.Size[0]);
            int t = 0;
            asc.NoDataValue = -999;
            for (int m = 1; m < 3; m++)
            {
                int days = DateTime.DaysInMonth(2009, m);
                for (int d = 1; d <= days; d++)
                {
                    string fn = prefix + m.ToString("00") + d.ToString("00") + ".asc";
                    var buf = asc.LoadSerial(fn, 177, 240);
                //    mat[0, t, MyMath.full, 0] = buf.GetVector();
                    t++;
                }
            }


            //string acfile_fn = @"E:\Heihe\HRB\GeoData\GBHM\Model\section_v1.09_ylx\data\"+ varb +".ac";
            //AcFile acfile = new AcFile();
            //acfile.Save(acfile_fn, mat.Value, vars, mat.Size[2]);
        }


        public static void SavePXD()
        {
            AscReader asc = new AscReader();
            string ascf = @"C:\Users\Administrator\Documents\ZYX\dem\elevation.asc";
            var vec = asc.LoadSerial(ascf, null);
            string varb = "sw";
            string[] vars = new string[] { varb };

            var ibound = asc.Load(ascf);
            string prefix = @"E:\Heihe\HRB\GeoData\GBHM\Model\section_v1.09_ylx\data\forcing\" + varb + "_2009";
            My3DMat<float> mat = new My3DMat<float>(1, 300, vec.Size[0]);
            int t = 0;
            asc.NoDataValue = -999;
 
            //for (int m = 1; m < 3; m++)
            //{
            //    int days = DateTime.DaysInMonth(2009, m);
            for (int d = 1; d <= 300; d++)
            {
                //string fn = "E:\\data\\" + varb + ".txt" + d;
                //var buf = asc.Load(fn, ibound);
                //mat[0, d-1, MyMath.full, 0] = buf.GetVector();
            }
            //}


            //string acfile_fn = @"C:\Users\Administrator\Documents\ZYX\data\" + varb + ".ac";
            //AcFile acfile = new AcFile();
            //acfile.Save(acfile_fn, mat.Value, vars, mat.Size[2]);
        }
    }
}
