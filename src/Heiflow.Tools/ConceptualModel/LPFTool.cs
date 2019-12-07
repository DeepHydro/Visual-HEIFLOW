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
    public class LPFTool  : MapLayerRequiredTool
    {

        private List<LookupTableRecord> _LookupTable = new List<LookupTableRecord>();
        private string[] _RasterFileList;
        public LPFTool()
        {
            Name = "Layer Property Flow";
            Category = "Conceptual Model";
            Description = "Extract aquifer properties from raster files and assign the properties to LPF";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
        }

        [Category("Input")]
        [Description("A text file that contains the list of raster file names")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string RasterFileList
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("A text file that contains the lookup table between raster value and aquifer properties. The first line of the text file is a head line. The column names must be: LAYER,ID,HK,VKA,SY,SS,WETDRY")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string LookupTableFile
        {
            get;
            set;
        }

        public override void Initialize()
        {    
            this.Initialized = TypeConverterEx.IsNotNull(LookupTableFile);
            this.Initialized = TypeConverterEx.IsNotNull(RasterFileList);
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
                mf = model as Modflow;

            if (mf != null)
            {
                int nlayer = mf.Grid.ActualLayerCount;
                var grid = mf.Grid as MFGrid;
                var lpf = mf.GetPackage(LPFPackage.PackageName) as LPFPackage;
                if (!LoadLookupTable(cancelProgressHandler))
                    return false;
                if (!LoadRasterFileList(cancelProgressHandler, nlayer))
                    return false;

                IRaster[] rasters = new IRaster[nlayer];
                var nullptlist = new List<Coordinate>();
                var cellsize = grid.DELR[0, 0, 0];
                for (int i = 0; i < nlayer; i++)
                {
                    rasters[i] = Raster.Open(_RasterFileList[i]);
                }

                for (int i = 0; i < grid.ActiveCellCount; i++)
                {
                    var loc = grid.Topology.ActiveCellLocation[i];
                    var llpt = grid.LocateLowerLeft(loc[1] + 1, loc[0] + 1);
                    var cellid = grid.Topology.GetID(loc[0], loc[1]);
                    var majorid = (int)ZonalStatastics.GetCellAverage(rasters[0], llpt, cellsize, AveragingMethod.Majority);
                    if (majorid == ZonalStatastics.NoDataValue)
                    {
                        majorid = (int)ZonalStatastics.FindNearestCellValue(rasters[0], llpt, 1, AveragingMethod.Majority);
                    }
                    if (majorid != 0)
                    {
                        for (int l = 0; l < nlayer; l++)
                        {
                            var record = from rec in _LookupTable where rec.ID == majorid && rec.Layer == (l+1) select rec;
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
                                string msg = string.Format("Warning: no values are found in the lookup table at the cell ID {0} in the Layer {1} for the raster value {2}", cellid, (l+1), majorid);
                                cancelProgressHandler.Progress("Package_Tool", progress,msg);
                            }
                        }
                    }
                    else
                    {
                        nullptlist.Add(llpt);
                    }
                    progress = i * 100 / grid.ActiveCellCount;

                    if (progress > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing cell: " + i);
                        count++;
                    }
                }
                if (nullptlist.Count > 0)
                {
                    string msg = string.Format("Warning: no values are mapped for {0} cells", nullptlist.Count);
                    cancelProgressHandler.Progress("Package_Tool", 100, msg);
                }
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow must be used by this tool.");
                return false;
            }

        }
        public  bool Execute1(ICancelProgressHandler cancelProgressHandler)
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
                var lpf = mf.GetPackage(LPFPackage.PackageName) as LPFPackage;
                if (!LoadLookupTable(cancelProgressHandler))
                    return false;
                if (!LoadRasterFileList(cancelProgressHandler, nlayer))
                    return false;

                IRaster[] rasters = new IRaster[nlayer];
                for (int i = 0; i < nlayer; i++)
                {
                    rasters[i] = Raster.Open(_RasterFileList[i]);
                }

                double cellwidth = rasters[0].CellWidth;
                double cellheihgt = rasters[0].CellHeight;
                int ncol = Convert.ToInt32(System.Math.Abs(grid.DELC[0, 0, 0] / rasters[0].CellHeight));
                int nrow = Convert.ToInt32(System.Math.Abs(grid.DELR[0, 0, 0] / rasters[0].CellWidth));
                ncol = ncol < 1 ? 1 : ncol;
                nrow = nrow < 1 ? 1 : nrow;
                var ptlist = new List<RcIndex>();
                var nullptlist = new List<Coordinate>();
                var valuelist = new List<int>();
                for (int i = 0; i < grid.ActiveCellCount; i++)
                {
                    var loc = grid.Topology.ActiveCellLocation[i];
                    var llpt = grid.LocateLowerLeft(loc[1] + 1, loc[0] + 1);
                    ptlist.Clear();
                    for (int j = 0; j < nrow; j++)
                    {
                        for (int k = 0; k < ncol; k++)
                        {
                            var pt = new Coordinate(llpt.X + cellwidth * k, llpt.Y + cellheihgt * j);
                            var cell = rasters[0].ProjToCell(pt);
                            if (cell != null && cell.Row > 0)
                                ptlist.Add(cell);
                        }
                    }
                    if (ptlist.Count > 0)
                    {                    
                        for (int l = 0; l < nlayer; l++)
                        {
                            valuelist.Clear();
                            foreach (var cell in ptlist)
                            {
                                valuelist.Add((int)rasters[l].Value[cell.Row, cell.Column]);
                            }
                            var majorid = ArrayHelper.GetMajorityElement(valuelist.ToArray());
                            var record = from rec in _LookupTable where rec.ID == majorid select rec;
                            if(record.Any())
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
                    }
                    else
                    {
                        nullptlist.Add(llpt);
                    }
                    progress = i * 100 / grid.ActiveCellCount;
                    if (progress > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing cell: " + i);
                        count++;
                    }
                }
                if(nullptlist.Count > 0)
                {
                    string msg = string.Format("Warning: no values are mapped for {0} cells", nullptlist.Count);
                    cancelProgressHandler.Progress("Package_Tool", 100, msg);
                }
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow must be used by this tool.");
                return false;
            }
         
        }
        private bool LoadLookupTable(ICancelProgressHandler cancelProgressHandler)
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

        private bool LoadRasterFileList(ICancelProgressHandler cancelProgressHandler, int nlayer)
        {
            StreamReader sr = new StreamReader(RasterFileList);
            bool result = false;
            string line = "";
            int n = 0;
            _RasterFileList = new string[nlayer];
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (TypeConverterEx.IsNotNull(line))
                    n++;
                else
                    break;
            }
            sr.Close();
            if (n != nlayer)
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: failed to load lookup raster list file. The number of raster filenames is not equal to the MODFLOW layer.");
                result = false;
            }
            else
            {
                result = true;
                sr = new StreamReader(RasterFileList);
                for (int i = 0; i < nlayer; i++)
                {
                    line = sr.ReadLine();
                    _RasterFileList[i] = line.Trim();
                    if (!File.Exists(_RasterFileList[i]))
                    {
                        cancelProgressHandler.Progress("Package_Tool", 100, "Error message: failed to load lookup raster list file. The following raster file does not exist: " + _RasterFileList[i]);
                        result = false;
                        break;
                    }
                }
                sr.Close();
            }
            return result;
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
