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

using DotSpatial.Data;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.IO;
using GeoAPI.Geometries;
using Heiflow.Core.MyMath;
using Heiflow.Spatial.SpatialAnalyst;

namespace Heiflow.Tools.ConceptualModel
{
    public class LPFByZoneID  : MapLayerRequiredTool
    {

        private List<LookupTableRecord> _LookupTable = new List<LookupTableRecord>();
        public LPFByZoneID()
        {
            Name = "Set LPF Parameters By Lookup Table";
            Category = Cat_CMG;
            SubCategory = "LPF";
            Description = "Set LPF parameters by lookup table.";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
        }

        [Category("Input")]
        [Description("A csv file that contains the zone id for each layer. The first column is HRU_ID."+
            " The rest columns are layer1, layer2,..., layerN. Each row contains HURID, zone_id of the HRU1 in  layer1, zone_id of the HRU1 in  layer2, etc.")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ZoneIDFile
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("A csv file that contains the lookup table between raster value and aquifer properties."+ 
        "The first line of the text file is a head line. The column names must be: LAYER,ID,HK,VKA,SY,SS,WETDRY")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string LookupTableFile
        {
            get;
            set;
        }

        public override void Initialize()
        {    
            this.Initialized = TypeConverterEx.IsNotNull(LookupTableFile);
            this.Initialized = TypeConverterEx.IsNotNull(ZoneIDFile);
        }

        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            int progress = 0;
            int count = 1;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf =  model as Modflow;

            if (mf != null)
            {
                int nlayer = mf.Grid.ActualLayerCount;
                var grid = mf.Grid as MFGrid;
                var lpf = mf.FlowPropertyPackage;
                if (!LoadLookupTable(cancelProgressHandler))
                    return false;
                var zoneid = LoadZoneIDTable(cancelProgressHandler, grid.ActualLayerCount, grid.ActiveCellCount);
                for (int i = 0; i < grid.ActiveCellCount; i++)
                {
                    for (int l = 0; l < nlayer; l++)
                    {
                        var record = from rec in _LookupTable where rec.ID == zoneid[i][l] && rec.Layer == (l + 1) select rec;
                        if (record.Any())
                        {
                            var selected = record.First();
                            lpf.HK[l, 0, i] = selected.HK;
                            lpf.VKA[l, 0, i] = selected.VKA;
                            lpf.SS[l, 0, i] = selected.SS;
                            lpf.SY[l, 0, i] = selected.SY;
                            lpf.WETDRY[l, 0, i] = selected.WETDRY;
                        }
                        else
                        {
                            cancelProgressHandler.Progress("Package_Tool", progress, "Warning: no values are found in the lookup table for the cell ID " + (i + 1) + " in the Layer " + (l + 1));
                        }
                    }
                    progress = i * 100 / grid.ActiveCellCount;
                    if (progress > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing cell: " + i);
                        count++;
                    }
                }
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow must be used by this tool.");
                return false;
            }
         
        }
        private bool 
            LoadLookupTable(ICancelProgressHandler cancelProgressHandler)
        {
            StreamReader sr = new StreamReader(LookupTableFile);
            bool result = false;
            try
            {
                string line = "";
                line = sr.ReadLine();
                var fields = TypeConverterEx.Split<string>(line);
                if (fields.Length != 7)
                {
                    cancelProgressHandler.Progress("Package_Tool", 100, "Error message: failed to load lookup table file. The number of column names in the head line is not equal to 7.");
                    result = false;
                }
                else
                {
                    _LookupTable.Clear();
                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();
                        var buf = TypeConverterEx.Split<float>(line);
                        LookupTableRecord table = new LookupTableRecord()
                        {
                            Layer = int.Parse(buf[0].ToString()),
                            ID = int.Parse(buf[1].ToString()),
                            HK = buf[2],
                            VKA = buf[3],
                            SY = buf[4],
                            SS = buf[5],
                            WETDRY = buf[6],
                        };
                        _LookupTable.Add(table);
                    }
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            finally
            {
                sr.Close();
            }
            return result;
        }

        private int[][] LoadZoneIDTable(ICancelProgressHandler cancelProgressHandler, int nlayer, int nhru)
        {
            StreamReader sr = new StreamReader(ZoneIDFile);
            var zoneid = new int[nhru][];
            try
            {
                string line = "";
                line = sr.ReadLine();
                var fields = TypeConverterEx.Split<string>(line);
                for (int i = 0; i < nhru; i++)
                {
                    zoneid[i] = new int[nlayer];
                    line = sr.ReadLine();
                    var buf = TypeConverterEx.Split<int>(line, nlayer + 1);
                    for (int j = 0; j < nlayer; j++)
                    {
                        zoneid[i][j] = buf[j + 1];
                    }
                }
            }
            catch
            {
                zoneid = null;
            }
            finally
            {
                sr.Close();
            }
            return zoneid;
        }
        public class LookupTableRecord
        {
            public LookupTableRecord()
            {

            }
            public int Layer { get; set; }
            public int ID { get; set; }
            public float HK { get; set; }
            public float VKA { get; set; }
            public float SY { get; set; }
            public float SS { get; set; }
            public float WETDRY { get; set; }
        }
    }
}
