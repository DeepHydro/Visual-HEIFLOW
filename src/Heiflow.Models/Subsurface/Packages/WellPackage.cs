using GSMS.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace GSMS.External.Modflow
{
    public class WellPackage:MFPackage
    {
        public WellPackage(string name, MFGrid grid)
            : base(name, grid)
        {
            IWELCB = 0;
            Option = "AUXILIARY IFACE NOPRINT";
            MFParameter para = new MFParameter("MXACTW");
            para.Package = this;
            para.ValueType = 1;
            para.VariableType = ParameterType.Parameter;
            para.Dimension = 0;
            para.IntValue = 0;
              para.SPVariable=false;
            AddParameter(para);

            para = new MFParameter("IWELCB");
            para.Package = this;
            para.ValueType = 1;
            para.VariableType = ParameterType.Parameter;
            para.Dimension = 0;
            para.IntValue = 0;
              para.SPVariable=false;
            AddParameter(para);

            para = new MFParameter("OPTION");
            para.Package = this;
            para.ValueType = 4;
            para.VariableType = ParameterType.Parameter;
            para.Dimension = 0;
            para.StringValue = "AUXILIARY IFACE NOPRINT";
            para.SPVariable=false;
            AddParameter(para);

            para = new MFParameter("WELL");
            para.Package = this;
            para.ValueType = 5;
            para.VariableType = ParameterType.Parameter;
            para.Dimension = 2; 
            para.SPVariable=true;
            AddParameter(para);
        }

        public int MXACTW { get; set; }
        public int IWELCB { get; set; }
        /// <summary>
        /// Default value is: AUXILIARY IFACE NOPRINT
        /// </summary>
        public string Option { get; set; }
        public MFWell[][] Wells { get; set; }

        public override void Read(string filename = "")
        {
            if (filename == "")
            {
                var mm = (from f in mfgrid.MFProject.MFFile.mMFModuleFile where f.ModuleName.ToUpper() == "WEL" select f).FirstOrDefault();
                filename = mm.FullFileName;
            }

            if (File.Exists(filename))
            {
                var grid = mfgrid;

                StreamReader sr = new StreamReader(filename);
                var line = ReadCommet(sr);
                var ss = TypeConverterEx.Split<int>(line, 2);
                Parameters["MXACTW"].IntValue = ss[0];
                Parameters["IWELCB"].IntValue = ss[1];

                Wells = new MFWell[grid.MFProject.NPeriod][];
                SPInfo = new int[grid.MFProject.NPeriod, 2];
                for (int n = 0; n < grid.MFProject.NPeriod; n++)
                {
                    ss = TypeConverterEx.Split<int>(sr.ReadLine(), 2);
                    SPInfo[n, 0] = ss[0];
                    SPInfo[n, 1] = ss[1];
                    if (ss[0] > 0)
                    {
                        Wells[n] = new MFWell[ss[0]];
                        for (int i = 0; i < ss[0]; i++)
                        {
                            var w = new MFWell(i, grid);
                            var vv = TypeConverterEx.Split<float>(sr.ReadLine());
                            w.Layer = (int)vv[0];
                            w.Row = (int)vv[1];
                            w.Column = (int)vv[2];
                            w.PumpingRate = vv[3];
                            w.IFace = (int)vv[4];
                            w.Coordinate = grid.LocateNode(w.Column, w.Row);
                            Wells[n][i] = w;
                        }
                    }
                }
                Parameters["WELL"].Value = Wells;
                sr.Close();
            }
        }

        public override void Save()
        {
            string filename = GetInputFile(this.Name);
            SaveAs(filename);
        }
        /// <summary>
        /// load from shp file that has columns with names "Layer", "Row","Column"
        /// </summary>
        /// <param name="para"> first para is DataTable,  sencond para is PumpRate field name, third para is layer count (if this para >1, then pumping rate is equally distributed in each layer) </param>
        public override void Load(object[] para)
        {
            if(para != null)
            {
                var dataTable = para[0] as DataTable;
                string [] prField = (string[]) para[1];
                int nlayer = (int)para[2];
                Wells = new MFWell[mfgrid.MFProject.NPeriod][];
                if(SPInfo == null)
                {
                    SPInfo = new int[2, 2];
                    SPInfo[0, 0] = dataTable.Rows.Count * nlayer;
                    SPInfo[1, 0] = -1;
                }

                for (int n = 0; n < mfgrid.MFProject.NPeriod; n++)
                {
                    if (SPInfo[n, 0] > 0)
                    {
                        Wells[n] = new MFWell[dataTable.Rows.Count * nlayer];
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            for (int l = 0; l < nlayer; l++)
                            {
                                int index=i * nlayer + l;
                                var w = new MFWell(index, mfgrid);
                                var dr = dataTable.Rows[i];
                                w.Layer = int.Parse(dr["Layer"].ToString()) - l;
                                w.Row = int.Parse(dr["Row"].ToString());
                                w.Column = int.Parse(dr["Col"].ToString());
                                w.PumpingRate = float.Parse(dr[prField[n]].ToString()) / nlayer;
                                w.IFace = 0;
                                w.Coordinate = mfgrid.LocateNode(w.Column, w.Row);
                                Wells[n][index] = w;
                            }
                        }
                    }
                }
                Parameters["WELL"].Value = Wells;
                Parameters["MXACTW"].IntValue = dataTable.Rows.Count * nlayer;
            }
        }

        public override void SaveAs(string filename)
        {
            var grid = mfgrid;
            StreamWriter sw = new StreamWriter(filename);
            var wells = (Parameters["WELL"] as MFParameter).Value as MFWell[][];

            WriteDefaultComment(sw, "WEL");
            string line = string.Format("{0}\t{1}\t{2}\t# DataSet 2: MXACTW IWELCB Option", Parameters["MXACTW"].GetValue(),
                Parameters["IWELCB"].GetValue(), Parameters["OPTION"].GetValue());
            sw.WriteLine(line);
            line = "";

            for (int n = 0; n < grid.MFProject.NPeriod; n++)
            {
                line = string.Format("{0}\t{1}\t# Data Set 5: ITMP NP Stress period {2}\n", SPInfo[n, 0], SPInfo[n, 1], n + 1);
                if (SPInfo[n, 0] > 0)
                {
                    for (int i = 0; i < wells[n].Length; i++)
                    {
                        var w = wells[n][i];
                        line += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n", w.Layer, w.Row, w.Column, -w.PumpingRate, w.IFace);
                    }
                    sw.Write(line);
                }
                else
                {
                    sw.Write(line);
                }
            }
       
            sw.Close();
        }

        public void SaveRechargeWell(string filename, string shpfile)
        {
            var fs = DotSpatial.Data.FeatureSet.Open(shpfile);
            var dt = fs.DataTable;
            StreamWriter sw = new StreamWriter(filename);
            var rows = (from dr in dt.AsEnumerable() where dr.Field<int>("IUZFBND") == 0 select dr).ToArray();
            int count = rows.Count();
            string line = string.Format("{0}\t{1}\t# Data Set 5: ITMP NP Stress period {2}", count, 0, 1);
            sw.WriteLine(line);
            for(int i=0;i<count;i++)
            {
                DataRow dr = rows[i];
                var w = new MFWell(i, mfgrid);
                w.Layer = 1;
                w.Row = int.Parse(dr["Row"].ToString());
                w.Column = int.Parse(dr["Column"].ToString());
                var rch = float.Parse(dr["Recharge"].ToString());
                w.PumpingRate = rch * 1000 * 1000 * 2;
                if (w.PumpingRate == 0)
                    w.PumpingRate = 100;
                w.IFace = 0;
                line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", w.Layer, w.Row, w.Column, w.PumpingRate, w.IFace);
                sw.WriteLine(line);
            }
            sw.Close();
            fs.Close();
        }

        public void SaveIrriWellFile(string filename, string shpfile)
        {
            var fs = DotSpatial.Data.FeatureSet.Open(shpfile);
            var dt = fs.DataTable;
            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultComment(sw, "IRW");
            string line = string.Format("{0}\t{1}\t{2}\t#Data Set 1: nsp nwell readoption", mfprj.NPeriod, dt.Rows.Count, 1);
            sw.WriteLine(line);
            line = "1 0 #Data Set 2: NP NWELL Stress period 1";
            sw.WriteLine(line);
            for (int n = 1; n < mfprj.NPeriod; n++)
            {
                line = string.Format("{0}\t{1}\t#Data Set 2: NP NWELL Stress period {2}", n + 1, dt.Rows.Count, n + 1);
                sw.WriteLine(line);
                string prfield = "PR" + (2000 + n - 1);
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    var dr = dt.Rows[r];
                    line = string.Format("{0}\t{1}\t{2}", dr["ID"].ToString(), dr["ACT_ID"].ToString(), dr[prfield].ToString());
                    sw.WriteLine(line);
                }
            }
            sw.Close();
        }

    }
}
