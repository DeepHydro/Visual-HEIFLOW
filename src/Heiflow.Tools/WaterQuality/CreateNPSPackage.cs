using DotSpatial.Data;
using Heiflow.Applications;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.WaterQuality
{
    public class CreateNPSTool : MapLayerRequiredTool
    {
        public CreateNPSTool()
        {
            Name = "Create NPS Package";
            Category = "Water Quality";
            SubCategory = "NPS";
            Description = "Create NPS Pacakge input files";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
        }

        public override void Initialize()
        {    
           
        }

        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model as HeiflowModel;
            int progress = 0;
            int count = 1;

            if (model != null)
            {
                var mf = model.ModflowModel;
                var pck = mf.GetPackage(UZFPackage.PackageName) as UZFPackage;
                var mfgrid = mf.Grid as RegularGrid;
                //prj.Project.WQDirectory
                //Coordinate centroid = null;
                for (int i = 0; i < mfgrid.ActiveCellCount; i++)
                {
                 
                    progress = i * 100 / mfgrid.ActiveCellCount;
                    if (progress > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing cell: " + i);
                        count++;
                    }
                }
                pck.Save(cancelProgressHandler);
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Failed to run.");
                return false;
            }
        }
    }
}
