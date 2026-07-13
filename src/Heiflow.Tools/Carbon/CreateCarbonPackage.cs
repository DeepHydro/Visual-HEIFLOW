using DotSpatial.Data;
using Heiflow.Applications;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Surface.WaterQuality;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.WaterQuality
{
    public class CreateCarbonTool : MapLayerRequiredTool
    {
        public CreateCarbonTool()
        {
            Name = "Create Carbon Package";
            Category = "Carbon Module";
            Description = "Create Carbon Pacakge input files";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
        }

        public override void Initialize()
        {
            this.Initialized = true;
        }

        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model as HeiflowModel;

            if (model != null)
            {
                var mf = model.ModflowModel;
                //var sfrpck = mf.GetPackage(SFRPackage.PackageName) as SFRPackage;
                var mfgrid = mf.Grid as RegularGrid;
                //var starttime = model.TimeService.Start;
                //var endtime = model.TimeService.End;
                //var nhru = mfgrid.ActiveCellCount;
                //var nseg = sfrpck.NSS;
                //var nreach = sfrpck.NSTRM;

                var wqinputpath = prj.Project.WQDirectory;
                var configpath = BaseModel.ConfigPath;
                //WQFiles wqfile = new WQFiles();
                //wqfile.New(configpath, wqinputpath, nhru, nseg, nreach, starttime, endtime);
                //cancelProgressHandler.Progress("Package_Tool", 50, "WQ files copied");

                model.MasterPackage.carbon_module = true;

                model.PRMSModel.NewCarbonPackage(null);
                var carbonpck = model.PRMSModel.CarbonPackage;
                carbonpck.Grid = mfgrid;
                carbonpck.OnGridUpdated(mfgrid);
                carbonpck.Save(null);
                cancelProgressHandler.Progress("Package_Tool", 80, "Carbon parameter file created");

                //model.ExtensionManPackage.EnableSFRWQ = true;
                //model.ExtensionManPackage.Save(null);
                //cancelProgressHandler.Progress("Package_Tool", 90, "Extension file modified");

                model.MasterPackage.Save(null);
                cancelProgressHandler.Progress("Package_Tool", 100, "Model control file modified");


                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Failed to run.");
                return false;
            }
        }

        public override void AfterExecution(object args)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model as HeiflowModel;
            shell.ProjectExplorer.ClearContent();
            shell.ProjectExplorer.AddProject(prj.Project);
        }
    }
}