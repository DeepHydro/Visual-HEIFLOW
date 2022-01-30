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
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Core.Data;
using Heiflow.Core.Hydrology;
using Heiflow.Core.IO;
using Heiflow.Core.MyMath;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.ConceptualModel
{
    public class RemoveDuplRch : MapLayerRequiredTool
    {
        public RemoveDuplRch()
        {
            Name = "Remove Duplicated Reaches";
            Category = Cat_CMG;
            SubCategory = "SFR";
            Description = "Remove duplicated reaches";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
        }
  
        public override void Initialize()
        {
            this.Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            if (sfr != null)
            {         
                var net = sfr.RiverNetwork;
                string msg = string.Format("Total number of outfalls is {0}", net.Outfalls.Count);
                int progress = 0;
                int i = 1;
                var list_code = new List<string>();
                var list_empty_river = new List<string>();
                var duplist = new List<Reach>();
                int count = 0;
                
                foreach (var river in net.Rivers)
                {
                    progress = i / net.RiverCount * 100;
                    duplist.Clear();
                    foreach(var rch in river.Reaches)
                    {
                        rch.LocationCode = string.Format("{0}_{1}", rch.IRCH, rch.JRCH);
                        if (list_code.Contains(rch.LocationCode))
                        {
                            duplist.Add(rch);
                        }
                        else
                        {
                            list_code.Add(rch.LocationCode);
                        }
                    }
                    if(duplist.Any())
                    {
                        count += duplist.Count;
                        if(duplist.Count == river.Reaches.Count)
                        {
                            duplist.First().STRHC1 = 0;
                            duplist.RemoveAt(0);
                            msg = string.Format("Warning: all reaches in the river {0} are duplicated. The first reach will not be removed.", river.ID);
                            cancelProgressHandler.Progress("Package_Tool", progress, msg);
                            list_empty_river.Add(river.ID.ToString());
                            count--;
                        }
                        foreach (var rch in duplist)
                            river.Reaches.Remove(rch);
                        msg = string.Format("{0} duplicated reaches are removed for the River {1}", duplist.Count, river.ID);
                        cancelProgressHandler.Progress("Package_Tool", progress, msg);
                    }

                    if(river.Reaches.Count > 0)
                    {
                        for (int j = 0; j < river.Reaches.Count; j++)
                        {
                            river.Reaches[j].ID = j + 1;
                            river.Reaches[j].IREACH = j + 1;
                            river.Reaches[j].SubID = j + 1;
                            river.Reaches[j].SubIndex = j;
                        }
                        msg = string.Format("The river {0} is processed", river.ID);
                    }
                }
                msg = string.Format("In total {0} duplicated reaches have been removed", count);
                cancelProgressHandler.Progress("Package_Tool", 80, msg);
                msg = string.Format("In total {0} rivers have empty reaches. The rivers are listed as follows: {1}", list_empty_river.Count, string.Join(",", list_empty_river));
                cancelProgressHandler.Progress("Package_Tool", 90, msg);
                sfr.RiverNetwork.ReachCount = sfr.RiverNetwork.GetReachCount();
                sfr.RiverNetwork.RiverCount = sfr.RiverNetwork.Rivers.Count;
                sfr.NSTRM = sfr.RiverNetwork.RiverCount;
                sfr.NSS = sfr.RiverNetwork.ReachCount;
                sfr.NetworkToMat();
                sfr.Attach(shell.MapAppManager.Map, prj.Project.GeoSpatialDirectory);

                var model = prj.Project.Model as Heiflow.Models.Integration.HeiflowModel;
                if (model != null)
                {
                    model.PRMSModel.MMSPackage.Parameters["nreach"].SetValue(0, 0, 0, sfr.NSS);
                    model.PRMSModel.MMSPackage.Parameters["nsegment"].SetValue(0, 0, 0, sfr.NSTRM);
                    model.PRMSModel.MMSPackage.IsDirty = true;
                    model.PRMSModel.MMSPackage.Save(null);
                }
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 90, "SFR package not loaded.");
                return false;
            }
        }

    }
}
