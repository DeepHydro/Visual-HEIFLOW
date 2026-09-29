using DotSpatial.Data;
using Heiflow.Applications;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Surface.NPS;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
// TODO: 
//  (1) 修改extensions.exm 中的SFRWQ = 1
// (2) 修改SFR边界入流，需要加入浓度
namespace Heiflow.Tools.NPS
{
    public class CreateNPSTool : MapLayerRequiredTool
    {
        public CreateNPSTool()
        {
            Name = "Create NPS Package";
            Category = "Non Point Source";
            Description = "Create NPS Pacakge input files";
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
                var sfrpck = mf.GetPackage(SFRPackage.PackageName) as SFRPackage;
                var mfgrid = mf.Grid as RegularGrid;
                var starttime = model.TimeService.Start;
                var endtime = model.TimeService.End;
                var nhru = mfgrid.ActiveCellCount;
                var nseg = sfrpck.NSS;
                var nreach = sfrpck.NSTRM;

                var wqinputpath = prj.Project.WQDirectory;
                var configpath = BaseModel.ConfigPath;
                NPSInputFiles wqfile = new NPSInputFiles();
                wqfile.New(configpath, wqinputpath, nhru, nseg, nreach, starttime, endtime);
                cancelProgressHandler.Progress("Package_Tool", 50, "NPS input files copied");

                model.MasterPackage.nps_module = true;

                model.PRMSModel.NewWQPackage(null);
                var wqpck = model.PRMSModel.WQPackage;
                wqpck.Grid = mfgrid;
                wqpck.OnGridUpdated(mfgrid);
                wqpck.Save(null);
                cancelProgressHandler.Progress("Package_Tool", 80, "NPS parameter file created");

                model.ExtensionManPackage.EnableSFRWQ = true;
                model.ExtensionManPackage.Save(null);
                cancelProgressHandler.Progress("Package_Tool", 90, "Extension file modified");

                model.MasterPackage.Save(null);
                cancelProgressHandler.Progress("Package_Tool", 100, "Model control file modified");

                prj.Project.ProcessModule = ProcessModule.NPS;
                prj.Project.SaveBatchRunFile();
                prj.Serializer.Save(prj.Project);
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