using GSMS.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSMS.External.Modflow
{

    public class HFBPackage:MFPackage
    {
        public HFBPackage(string name, MFGrid grid):base(name,grid)
        {
            MFParameter para = MFParameter.Create("NPHFB", this, 1, 0, ParameterType.Parameter);
            para.IntValue = 0;
            AddParameter(para);

            para = MFParameter.Create("MXFB", this, 1, 0, ParameterType.Parameter);
            para.IntValue = 0;
            AddParameter(para);

            para = MFParameter.Create("NHFBNP", this, 1, 0, ParameterType.Parameter);
            para.IntValue = 0;
            AddParameter(para);

            para = MFParameter.Create("NACTHFB", this, 1, 0, ParameterType.Parameter);
            para.IntValue = 0;
            AddParameter(para);
        }

        public int NPHFB { get; set; }
        public int MXFB { get; set; }
        public int NHFBNP { get; set; }
        /// <summary>
        ///  the number of active HFB parameters.
        /// </summary>
        public int NACTHFB { get; set; }
        /// <summary>
        /// Layer IROW1 ICOL1 IROW2  ICOL2 Hydchr
        /// </summary>
        public float[,] CellLocation { get; set; }

        public override void Load(object[] para)
        {
            if (para != null)
            {
                string shpfile = para[0].ToString();
                var fs = DotSpatial.Data.FeatureSet.Open(shpfile);
                var grid = mfgrid;
                var dt = fs.DataTable;

                var rows = (from r in dt.AsEnumerable() where r.Field<float>("Hydchr") > 0 select r).ToArray();
                int rnum = rows.Count(); ;
                Parameters["NHFBNP"].IntValue = rnum;
                CellLocation = new float[rnum, 6];

                for (int i = 0; i < rnum; i++)
                {
                    DataRow dr = rows[i];
                    var hydchr = float.Parse(dr["hydchr"].ToString());
                    int row = int.Parse(dr["Row"].ToString());
                    int col = int.Parse(dr["Column"].ToString());
                    CellLocation[i, 0] = int.Parse(dr["Layer"].ToString());
                    CellLocation[i, 1] = row;
                    CellLocation[i, 2] = col;
                    CellLocation[i, 3] = row;
                    CellLocation[i, 4] = col + 1;
                    CellLocation[i, 5] = hydchr;
                }
            }
        }

        public override void Read(string filename = "")
        {

        }

        public override void Save()
        {
            string filename = GetInputFile(this.Name);
            SaveAs(filename);
        }

        public override void SaveAs(string filename)
        {
          
            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultComment(sw, "HFB");

            string line = string.Format("{0}\t{1}\t{2}\t# Data set 1: NPHFB MXFB NHFBNP", Parameters["NPHFB"].IntValue, Parameters["MXFB"].IntValue, 
                Parameters["NHFBNP"].IntValue);
            sw.WriteLine(line);

            MatrixExtension<float>.Save(CellLocation, sw);

            line = string.Format("{0}\t# Data Set 5: NACTHFB", Parameters["NACTHFB"].IntValue);
            sw.WriteLine(line);
            sw.Close();
        }
    }
}
