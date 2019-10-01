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
using Heiflow.Core.Hydrology;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Integration;
using Heiflow.Models.IO;
using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Surface.PRMS
{
    public class Cascade
    {

        //private GsProject mGsPrj;
        private MFGrid mGrid;
        private PRMS _Prms;
        private Modflow _Modflow;
        public CRTProperty Property { get; private set; }
        public int[] IgnoredRiverID { get; set; }
        /// <summary>
        /// a matrix: OUTFLOW_ID ROW COL
        /// </summary>
        public int[,] OutflowID { get; set; }


        public Cascade()
        {
            Property = new CRTProperty();
        }

        public void Initialize(IProject project)
        {
            if (project != null)
            {
                mGrid = project.Model.Grid as MFGrid;
                _Prms = (project.Model as HeiflowModel).PRMSModel;
                _Modflow = (project.Model as HeiflowModel).ModflowModel;
            }
        }

        public DataTable NewOutletTable()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("Cell_ID", Type.GetType("System.Int32"))
            {
                Caption = "Cell ID"
            };
            dt.Columns.Add(dc);
            dc = new DataColumn("Row_ID", Type.GetType("System.Int32"))
            {
                Caption = "Row ID"
            };
            dt.Columns.Add(dc);
            dc = new DataColumn("Column_ID", Type.GetType("System.Int32"))
            {
                Caption = "Column ID"
            };
            dt.Columns.Add(dc);

            return dt;
        }

        public int[,] GetOutlets(DataTable source)
        {
            var nrow=source.Rows.Count;
            if (nrow > 0)
            {
                var outlets = new int[nrow, 3];
                for (int i = 0; i < nrow; i++)
                {
                    outlets[i, 0] = (int)source.Rows[i][0];
                    outlets[i, 1] = (int)source.Rows[i][1];
                    outlets[i, 2] = (int)source.Rows[i][2];
                }
                return outlets;
            }
            else
                return null;
        }

        public void Save(string processing_dir)
        {
            string filename = Path.Combine(processing_dir, "HRU_CASC.DAT");
            SaveCascade(filename);
            filename = Path.Combine(processing_dir, "LAND_ELEV.DAT");
            SaveSurfaceElevation(filename);
            filename = Path.Combine(processing_dir, "STREAM_CELLS.DAT");
            SaveStreamLocation(filename);
            if (Property.HRUFLG == 1)
            {
                filename = Path.Combine(processing_dir, "HRU_ID.DAT");
                SaveHRUIdentifiers(filename);
            }
            if (Property.VISFLG == 1)
            {
                filename = Path.Combine(processing_dir, "XY.DAT");
                SaveVisualization(filename);
            }
            if (OutflowID != null)
            {
                filename = Path.Combine(processing_dir, "OUTFLOW_HRU.DAT");
                SaveOutflowLocation(filename);
            }
        }

        public bool Run(string crtexe_file, string processing_dir)
        { 
            bool is_success = true;
            Process workProcess = null;
            try
            {

                var crt = Path.GetFileName(crtexe_file);
                var local_crt = Path.Combine(processing_dir, crt);
                if (!File.Exists(local_crt))
                {
                    File.Copy(crtexe_file, local_crt);
                }
                ProcessStartInfo info = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    FileName = local_crt,
                    UseShellExecute = false,
                    ErrorDialog = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = processing_dir
                };
                workProcess = Process.Start(info);
                workProcess.WaitForExit();
            }
            catch
            {
                workProcess.Kill();
                is_success = false;
            }
            return is_success;
        }

        public void UpdateParameter( string processing_dir)
        {
            var cascade = Path.Combine(processing_dir, "cascade.param");
            MmsParameterFile mms = new MmsParameterFile(cascade);
            var list = mms.Read(null);
            int ncascade=list["hru_up_id"].ValueCount;
            _Prms.MMSPackage.Parameters["ncascade"].SetValue(0, 0, 0, ncascade);
            _Prms.MMSPackage.AlterLength("ncascade", ncascade);

            MapHruID(list["hru_up_id"]);
            MapHruID(list["hru_down_id"]);

            _Prms.MMSPackage.UpdatePamameter(list["hru_up_id"]);
            _Prms.MMSPackage.UpdatePamameter(list["hru_down_id"]);
            _Prms.MMSPackage.UpdatePamameter(list["hru_strmseg_down_id"]);
            _Prms.MMSPackage.UpdatePamameter(list["hru_pct_up"]);

        }

        public void UpdateParameter()
        {
            int nhru = (int)_Prms.MMSPackage.Parameters["nhru"].GetValue(0,0,0);
           // float area = (float)Math.Round(1000 * 1000 / 4047.0, 2);
            var gvr_cell_id = new int[nhru];
            var gvr_hru_id = new int[nhru];
            var gvr_hru_pct = new float[nhru];
            var gvr_cell_pct = new float[nhru];
            //var hru_area = new float[nhru];
            int index = 0;

            foreach (var k in mGrid.Topology.CellID2CellIndex.Keys)
            {
                gvr_cell_id[index] = k;
                gvr_hru_id[index] = mGrid.Topology.CellID2CellIndex[k] + 1;
                gvr_cell_pct[index] = 1;
                gvr_hru_pct[index] = 1;
               // hru_area[index] = area;
                index++;
            }
            index = 0;

            var gvr_cell_id_para = _Prms.MMSPackage.Parameters["gvr_cell_id"] as DataCubeParameter<int>;
            var gvr_hru_id_para = _Prms.MMSPackage.Parameters["gvr_hru_id"] as DataCubeParameter<int>;
            gvr_cell_id_para[0][":",0] = gvr_cell_id;
            gvr_hru_id_para[0][":", 0] = gvr_hru_id;
        }

        public void MapHruID(IParameter para)
        {
            var ac = mGrid.Topology.CellID2CellIndex;
            var gv = para as DataCubeParameter<int>;
            for (int i = 0; i < gv.ValueCount; i++)
            {
                if (ac.Keys.Contains(gv[0,i,0]))
                    gv[0, i, 0] = ac[gv[0, i, 0]] + 1;
                else
                    gv[0, i, 0] = 0;
            }
        }

        private void SaveCascade(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            int[] item1 = new int[] { Property.HRUFLG, Property.STRMFLG, Property.FLOWFLG, Property.VISFLG, Property.IPRN, Property.IFILL };
            string line = string.Format("{0}\t{1}\t{2}\t#HRUFLG STRMFLG FLOWFLG VISFLG IPRN IFILL DPIT OUTITMAX", string.Join("\t", item1), Property.DPIT, Property.OUTITMAX);
            sw.WriteLine(line);
            var hru_type = (_Prms.MMSPackage.Parameters["hru_type"] as DataCubeParameter<int>).ToVector();
            int aci = 0;
            short[] ht = new short[mGrid.ColumnCount];

            for (uint r = 0; r < mGrid.RowCount; r++)
            {
                line = "";
                MatrixExtension<short>.Set(ht, 0);
                for (uint c = 0; c < mGrid.ColumnCount; c++)
                {
                    if (mGrid.IBound[0, (int)r, (int)c] > 0)
                    {
                        ht[c] = (short)hru_type[aci];
                        aci++;
                    }
                }
                line = string.Join("\t", ht);
                sw.WriteLine(line);
            }

            sw.Close();
        }

        private void SaveSurfaceElevation(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            string line = string.Format("{0}\t{1}\t#NROW NCOL", mGrid.RowCount, mGrid.ColumnCount);
            sw.WriteLine(line);
            var ele = mGrid.Elevations[0,"0",":"];
            var mat = mGrid.ToMatrix<float>(ele, 1000);
            for (int r = 0; r < mGrid.RowCount; r++)
            {
                line = string.Join("\t", mat[r]);
                sw.WriteLine(line);
            }
            sw.Close();
        }

        private void SaveStreamLocation(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            //GsflowRivers river = mGrid.Rivers;
            var network =( _Modflow.Packages[SFRPackage.PackageName] as SFRPackage).RiverNetwork;
            List<River> ignoredRiver = new List<River>();
            if (IgnoredRiverID != null)
            {
                foreach (var id in IgnoredRiverID)
                    network.GetUpRivers(id, ignoredRiver);
            }

            string line = "";
            int on_off = 1;
            int rchnum = 0;
            var igids = from ir in ignoredRiver select ir.ID;
            foreach (var riv in network.Rivers)
            {
                if (!igids.Contains(riv.ID))
                {
                    foreach (var rch in riv.Reaches)
                    {
                        line += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\n", rch.IRCH, rch.JRCH, riv.ID, rch.SubIndex + 1, on_off);
                        rchnum++;
                    }
                }
                else
                {

                }
            }
            sw.WriteLine(rchnum + "\tNREACH");
            sw.Write(line);
            sw.Close();
        }
        private void SaveHRUIdentifiers(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            string line = mGrid.ActiveCellCount.ToString();
            sw.WriteLine(line);
            var keys = mGrid.Topology.CellID2CellIndex.Keys.ToArray();
            int hru_id = 0;
            int cell_id = 0;
            foreach (var k in keys)
            {
                hru_id = mGrid.Topology.CellID2CellIndex[k] + 1;
                cell_id = k;
                line = hru_id + "\t" + cell_id;
                sw.WriteLine(line);
            }
            sw.Close();
        }
        private void SaveVisualization(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            string line = "";
            int i = 1;
            if (Property.HRUFLG == 0)
            {

                for (int r = 0; r < mGrid.RowCount; r++)
                {
                    line = "";
                    for (int c = 0; c < mGrid.ColumnCount; c++)
                    {
                        var cc = mGrid.LocateNode(c + 1, r + 1);
                        line = string.Format("{0}\t{1}\t{2}", i, cc.X, cc.Y);
                        sw.WriteLine(line);
                        i++;
                    }
                }
            }
            else
            {
                var keys = mGrid.Topology.CellID2CellIndex.Keys.ToArray();
                foreach (var k in keys)
                {
                    var index = mGrid.Topology.CellID2CellIndex[k];
                    var loc = mGrid.Topology.ActiveCell[index];
                    var cc = mGrid.LocateNode(loc[1] + 1, loc[0] + 1);
                    line = string.Format("{0}\t{1}\t{2}", i, cc.X, cc.Y);
                    sw.WriteLine(line);
                    i++;
                }
            }
            sw.Close();
        }

        private void SaveOutflowLocation(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            string line = "";

            int num = OutflowID.GetLength(0);
            line = num + "\tNUMOUTFLOWHRUS";
            sw.WriteLine(line);
            for (int i = 0; i < num; i++)
            {
                line = string.Format("{0}\t{1}\t{2}\t#OUTFLOW_ID ROW COL", OutflowID[i, 0], OutflowID[i, 1], OutflowID[i, 2]);
                sw.WriteLine(line);
            }
            sw.Close();
        }
    }
}
