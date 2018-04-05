using GSMS.Core;
using HUST.WREIS.ModelBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMS.External.Modflow
{
    public class UPWPackage : LPFPackage
    {
        public UPWPackage(string name, MFGrid grid)
            : base(name, grid)
        {
            
        }

        public override void Load(object[] para)
        {
            if (para == null)
                return;
            var dt_domain = DotSpatial.Data.FeatureSet.Open(para[0].ToString()).DataTable;
            var dt_grid = DotSpatial.Data.FeatureSet.Open(para[1].ToString()).DataTable;
            dt_grid.DefaultView.Sort = para[2].ToString();
            string hkField = para[3].ToString();

            int[] ty = new int[] { 1, 1, 0, 0,0 };

            DataRow dr = dt_domain.Rows[0];
            Parameters["LAYTYP"].Value = TypeConverterEx.Split<int>(dr["LAYTYP"].ToString());
            Parameters["LAYTYP"].Value = ty;
            Parameters["LAYAVG"].Value = TypeConverterEx.Split<int>(dr["LAYAVG"].ToString());
            Parameters["CHANI"].Value = TypeConverterEx.Split<int>(dr["CHANI"].ToString());
            Parameters["LAYVKA"].Value = TypeConverterEx.Split<int>(dr["LAYVKA"].ToString());
            Parameters["LAYWET"].Value = TypeConverterEx.Split<int>(dr["LAYWET"].ToString());
            int layer = int.Parse(dr["NLayer"].ToString());
            var hania = new float[layer];
            MatrixExtension<float>.Set(hania, 1);

            var hk = new MatrixCube<float>(layer);
            var hani = new MatrixCube<float>(hania);
            var vka = new MatrixCube<float>(layer);
            var ss = new MatrixCube<float>(layer);
            var sy = new MatrixCube<float>(layer);
            var wetdry = new MatrixCube<float>(layer);

            Parameters["HK"].Value = hk;
            Parameters["HANI"].Value = hani;
            Parameters["VKA"].Value = vka;
            Parameters["SS"].Value = ss;
            Parameters["SY"].Value = sy;
            Parameters["WETDRY"].Value = wetdry;

            for (int l = 0; l < mfgrid.ActualLayerCount; l++)
            {
                hk.LayeredValues[l] = new float[mfgrid.RowCount, mfgrid.ColumnCount];
                vka.LayeredValues[l] = new float[mfgrid.RowCount, mfgrid.ColumnCount];
                ss.LayeredValues[l] = new float[mfgrid.RowCount, mfgrid.ColumnCount];
                if (Parameters["LAYTYP"].IntValues[l] != 0)
                {
                    sy.LayeredValues[l] = new float[mfgrid.RowCount, mfgrid.ColumnCount];
                }
                if (Parameters["LAYTYP"].IntValues[l] != 0 && Parameters["LAYWET"].IntValues[l] != 0)
                {
                    wetdry.LayeredValues[l] = new float[mfgrid.RowCount, mfgrid.ColumnCount];
                }
            }

            int i = 0;
            foreach (var ac in mfgrid.Topology.ActiveCellLocation.Values)
            {
                hk.LayeredValues[0][ac[0], ac[1]] = (float)dt_grid.Rows[i][hkField];
                i++;
            }
            CreateUnifiedLPF();
        }

        public override void Read(string filename = "")
        {
            if (filename == "")
                filename = GetInputFile(this.Name);

            if (File.Exists(filename))
            {
                var grid = mfgrid;
                int layer = grid.ActualLayerCount;
                var upw = this;

                layer = 3;

                StreamReader sr = new StreamReader(filename);
                //Data Set 1: # ILPFCB, HDRY, NPLPF
                string newline = ReadCommet(sr);
                float[] fv = TypeConverterEx.Split<float>(newline, 4);
                upw.ILPFCB = (short)fv[0];
                upw.HDRY = fv[1];
                upw.NPLPF = (short)fv[2];
                upw.IPHDRY = (short)fv[3];
                //Data Set 2: # 
                newline = sr.ReadLine();
                upw.LAYTYP = TypeConverterEx.Split<int>(newline, layer);

                //Data Set 3: # 
                newline = sr.ReadLine();
                upw.LAYAVG = TypeConverterEx.Split<int>(newline, layer);

                //Data Set 4: # 
                newline = sr.ReadLine();
                upw.CHANI = TypeConverterEx.Split<int>(newline, layer);

                //Data Set 5: # 
                newline = sr.ReadLine();
                upw.LAYVKA = TypeConverterEx.Split<int>(newline, layer);

                //Data Set 6: # 
                newline = sr.ReadLine();
                upw.LAYWET = TypeConverterEx.Split<int>(newline, layer);

                //Data Set 7: # 
                //grid.LPFProperty.HK = new LayeredMatrix<float>(layer);
                //grid.LPFProperty.HANI = new LayeredMatrix<float>(layer);
                //grid.LPFProperty.VKA = new LayeredMatrix<float>(layer);
                //grid.LPFProperty.SS = new LayeredMatrix<float>(layer);
                //grid.LPFProperty.SY = new LayeredMatrix<float>(layer);

                var hania = new float[layer];
                MatrixExtension<float>.Set(hania, 1);
                var hk = new MatrixCube<float>(layer);
                var hani = new MatrixCube<float>(layer);
                var vka = new MatrixCube<float>(layer);
                var ss = new MatrixCube<float>(layer);
                var sy = new MatrixCube<float>(layer);
                var wetdry = new MatrixCube<float>(layer);

                Parameters["HK"].Value = hk;
                Parameters["HANI"].Value = hani;
                Parameters["VKA"].Value = vka;
                Parameters["SS"].Value = ss;
                Parameters["SY"].Value = sy;
                Parameters["WETDRY"].Value = wetdry;

                for (int l = 0; l < layer; l++)
                {
                    ReadInternalMatrix(sr, hk, grid.RowCount, grid.ColumnCount, l);
                    ReadInternalMatrix(sr, hani, grid.RowCount, grid.ColumnCount, l);
                    ReadInternalMatrix(sr, vka, grid.RowCount, grid.ColumnCount, l);
                    ReadInternalMatrix(sr, ss, grid.RowCount, grid.ColumnCount, l);
                    if (upw.LAYTYP[l] != 0)
                        ReadInternalMatrix(sr, sy, grid.RowCount, grid.ColumnCount, l);
                }
                sr.Close();
            }
        }

        public override void Save()
        {
            string filename = GetInputFile(this.Name);
            SaveAs(filename);
        }

        public override void SaveAs(string filename)
        {
            var grid = mfgrid;
            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultComment(sw, "UPW");
            string line = string.Format("{0}\t{1}\t{2}\t {3}\t# ILPFCB, HDRY, NPUPW, IPHDRY", Parameters["ILPFCB"].Value, Parameters["HDRY"].Value,
                 Parameters["NPLPF"].Value, Parameters["IPHDRY"].Value);
            sw.WriteLine(line);

            line = TypeConverterEx.Vector2String<int>(Parameters["LAYTYP"].IntValues) + "\t# LAYTYP";
            sw.WriteLine(line);

            line = TypeConverterEx.Vector2String<int>(Parameters["LAYAVG"].IntValues) + "\t# LAYAVG";
            sw.WriteLine(line);

            line = TypeConverterEx.Vector2String<int>(Parameters["CHANI"].IntValues) + "\t# CHANI";
            sw.WriteLine(line);

            line = TypeConverterEx.Vector2String<int>(Parameters["LAYVKA"].IntValues) + "\t# LAYVKA";
            sw.WriteLine(line);

            var wet = new int[grid.ActualLayerCount];
            line = TypeConverterEx.Vector2String<int>(wet) + "\t# LAYWET";
            sw.WriteLine(line);

            var hk = Parameters["HK"].Value as MatrixCube<float>;
            var hani = Parameters["HANI"].Value as MatrixCube<float>;
            var vka = Parameters["VKA"].Value as MatrixCube<float>;
            var ss = Parameters["SS"].Value as MatrixCube<float>;
            var sy = Parameters["SY"].Value as MatrixCube<float>;
            var wetdry = Parameters["WETDRY"].Value as MatrixCube<float>;

            for (int l = 0; l < grid.ActualLayerCount; l++)
            {
                string cmt = string.Format("#HK Layer {0}", l + 1);
                WriteInternalMatrix(sw, hk, 1, l, -1, cmt);
                cmt = string.Format("#HANI Layer {0}", l + 1);
                WriteInternalMatrix(sw,hani, 1, l, -1, cmt);
                cmt = string.Format("#VKA Layer {0}", l + 1);
                WriteInternalMatrix(sw, vka, 1, l, -1, cmt);
                cmt = string.Format("#SS Layer {0}", l + 1);
                WriteInternalMatrix(sw, ss, 1, l, -1, cmt);
                if (Parameters["LAYTYP"].IntValues[l] != 0)
                {
                    cmt = string.Format("#SY Layer {0}", l + 1);
                    WriteInternalMatrix(sw, sy, 1, l, -1, cmt);
                }
            }
            sw.Close();
        }
    }
}
