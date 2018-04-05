// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotNetDBF;
using DotSpatial.Data;
using Heiflow.Core.IO;
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace Heiflow.Models.IO
{
    public class MergeACFile
    {

        public void Runoff_GBHM()
        {
            string dic = @"E:\Heihe\HRB\GeoData\GBHM\IHRB\上游模型集成数据_qy151228\0.stream_runoff\";
            string[] filenames = new string[]
            {
                "1.taolaihe_ws2021.dat",
                    "2.hongshuiba_ws2011.dat",
                        "3.fenglehe_ws2025.dat",
                            "4.mayinghe_ws2015.dat",
                                "5.shuiguanhe_ws1028.dat",
                                    "6.bailanghe_ws1029.dat",
                                    "7.liyuanhe_ws2049.dat",
                                    "8.heihe_ws3017.dat",
                                    "9.suyoukou_ws1069.dat",
                                    "10.daduma_ws1054.dat",
                                    "11.haichaoba_ws1066.dat",
                                    "12.hongshuida_ws1067.dat",
                                         "13.tongziba_ws1073.dat",
                                    "14.dongmaying_ws1082.dat",
            };

            MatrixTextStream mat = new MatrixTextStream();
            var stream = new float[4383];

            foreach(var fn in filenames)
            {
                var buf = mat.Load<float>(dic + fn);

                for(int i=366;i<4749;i++)
                {
                    stream[i - 366] += buf[i, 3, 0];
                }            
            }

            var sum_stream= stream.Sum() * 86400/12;

            var ac_et = @"C:\Users\Administrator\Documents\GBHM\GBHM\result\et_month_2000-2012_gbhm.ac";
            AcFile ac = new AcFile();
            var et_mat = ac.Load(ac_et) as My3DMat<float>;
            float sum_et = 0;
            for (int i = 12; i < 156; i++)
            {
                sum_et += et_mat.Value[0][i].Average();
            }

            sum_et = sum_et / 144 * 365;

            var ac_runoff = @"C:\Users\Administrator\Documents\GBHM\GBHM\result\runoff_month_2000-2012_gbhm.ac";
            var runoff_mat = ac.Load(ac_runoff) as My3DMat<float>;
            float sum_runoff  = 0;
            for (int i = 12; i < 156; i++)
            {
                sum_runoff += runoff_mat.Value[0][i].Average();
            }

            sum_runoff = sum_runoff / 144 * 365;


        }

        public void Determin2()
        {
            string filename = @"E:\Heihe\HRB\Processes\AC\gehmlai.dat";
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            AcFile ac1 = new AcFile();
            string acfile = @"E:\Heihe\HRB\Processes\AC\LAI_IHRB_FWJ_2000-2005.ac";
            string lai_ihrb = @"C:\Users\Administrator\Documents\GBHM\IHRB\result\lai.ac";
            var mat = ac1.Load(acfile) as My3DMat<float>;
            var acmat1 = new My3DMat<float>(1, 195, 90589);

            for (int t = 0; t < 195; t++)
            {
                float[] buf = new float[90589];
                for (int i = 0; i < 8; i++)
                {
                    for (int n = 0; n < 90589; n++)
                    {
                        buf[n] += br.ReadSingle();
                    }
                }
                for (int n = 0; n < 90589; n++)
                {
                    buf[n] /= 8.0f;
                    acmat1.Value[0][t][n] = buf[n];
                }
            }

            string dbffile_hru = @"E:\Heihe\HRB\GeoData\GBHM\IHRB\GIS\Grid_IHRB_active.dbf";
            DBFReader dbf = new DBFReader(dbffile_hru);
            int nfeature = dbf.RecordCount;
            int steps = 195;
            int nvar = mat.Size[0];
            int act_index = dbf.GetFiledNameIndex("act_id");
            int id_index = dbf.GetFiledNameIndex("id");
            Dictionary<int, int> dic = new Dictionary<int, int>();

            int[] ids = new int[nfeature];
            for (int n = 0; n < nfeature; n++)
            {
                var obj = dbf.NextRecord();
                var act_id = int.Parse(obj[act_index].ToString());
                var id = int.Parse(obj[id_index].ToString());
                ids[n] = id;
                dic.Add(id, act_id);
            }

            Array.Sort(ids);
            for (int v = 0; v < nvar; v++)
            {
                for (int t = 0; t < steps; t++)
                {
                    for (int n = 0; n < nfeature; n++)
                    {
                        var actid = dic[ids[n]];
                        if (actid < 100000)
                        {
                            mat[v, t, n] = acmat1[v, t, actid - 1];
                        }
                    }
                }
            }

            ac1.Save(lai_ihrb, mat.Value, ac1.Variables);
            br.Close();
            fs.Close();
        }

        public void Determin1()
        {
            string shp = @"E:\Heihe\HRB\GeoData\GBHM\IHRB\GIS\Grid_GBHM1.shp";
            string ascfile = @"C:\Users\Administrator\Documents\GBHM\GBHM\dem\ext-mask-1.asc";
            IFeatureSet fs = FeatureSet.Open(shp);
            var dt = fs.DataTable.AsEnumerable();
            AscReader asc = new AscReader();
            var mat = asc.Load(ascfile);

            Dictionary<int, int> buf = new Dictionary<int, int>();
            int act = 1;
            for (int i = 0; i < mat.Size[0]; i++)
            {
                for (int j = 0; j < mat.Size[1]; j++)
                {
                    if (mat[i, j, 0] > 0)
                    {
                        int id = i * 388 + j + 1;
                        buf.Add(id,act);
                        act++;
                    }
                }
            }
         
            foreach (DataRow dr in fs.DataTable.Rows)
            {
                var id = dr.Field<int>("ID");
                if (buf.Keys.Contains(id))
                {
                    dr["ACT_ID"] = buf[id];
                }
            }
            fs.Save();

        }

        public void Determin()
        {
            string shp = @"E:\Heihe\HRB\GeoData\GBHM\IHRB\GIS\Grid_HRU1.shp";

            IFeatureSet fs = FeatureSet.Open(shp);
            var dt = fs.DataTable.AsEnumerable();
            Dictionary<int, int> dic = new Dictionary<int, int>();
            int act_id = 100000;
            
            for (int r = 331; r <= 551; r++)
            {
                var rows = from dr in dt where dr.Field<short>("ROW") == r && dr.Field<short>("ZONE") == 1 orderby dr.Field<short>("COL") select dr;
                foreach (var dr in rows)
                {
                    var id = dr.Field<int>("ID");
                    dic.Add(id, act_id);
                    act_id++;
                }
            }

            foreach (DataRow dr in fs.DataTable.Rows)
            {
                var id = dr.Field<int>("ID");
                if (dic.Keys.Contains(id))
                {
                    dr["ACT_ID"] = dic[id];
                }
            }

            fs.Save();
        }

        public void Export(string dbfile, string out_acfile, string acfile1,string acfile2, int maxstep=156 )
        {
            AcFile ac1 = new AcFile();
            AcFile ac2 = new AcFile();
            AcFile ac_out = new AcFile();

            var mat1 = ac1.Load(acfile1);
            var mat2 = ac2.Load(acfile2);

            DBFReader dbf = new DBFReader(dbfile);
            int nfeature = dbf.RecordCount;
            int steps = mat1.Size[1];
            int nvar = mat1.Size[0];
            int act_index = dbf.GetFiledNameIndex("act_id");
            int id_index = dbf.GetFiledNameIndex("id");
            Dictionary<int, int> dic = new Dictionary<int, int>();

            if (steps > maxstep)
                steps = maxstep;

            My3DMat<float> mat = new My3DMat<float>(nvar, steps, nfeature);

            int[] ids = new int[nfeature];
            for (int n = 0; n < nfeature; n++)
            {
                var obj = dbf.NextRecord();
                var act_id = int.Parse(obj[act_index].ToString());
                var id= int.Parse(obj[id_index].ToString());
                ids[n] = id;
                dic.Add(id, act_id);
            }

            Array.Sort(ids);
            int n1 = 0;
            int n2 = 0;
            for (int v = 0; v < nvar; v++)
            {
                for (int t = 0; t < steps; t++)
                {
                    for (int n = 0; n < nfeature; n++)
                    {
                        var actid = dic[ids[n]];
                        if (actid >= 100000)
                        {
                            mat[v, t, n] = mat2[v, t, actid - 100000 - 1 ] * 30;
                            n2++;
                        }
                        else
                        {
                            mat[v, t, n] = mat1[v, t, actid - 1];
                            n1++;
                        }
                    }
                    if (t >= maxstep)
                        break;
                }
            }

            ac_out.Save(out_acfile, mat.Value, ac1.Variables);
        }
    }
}
