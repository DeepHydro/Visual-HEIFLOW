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
// TODO: 
//  (1) 修改extensions.exm 中的SFRWQ = 1
// (2) 修改SFR边界入流，需要加入浓度
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

        private string _fertFile;

        [Category("Input")]
        [Description("The fertilization filename. The file must contain three lines. The first line is: 5 3 #num_fert_count num_fert_cell; The second line is: 120 150 180 # fert time; The third line is: 1 2 3 4 5 # fert HRU IDs")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string FertilizationFile
        {
            get
            {
                return _fertFile;
            }
            set
            {
                _fertFile = value;
            }
        }

        public override void Initialize()
        {
            this.Initialized = true;
            if (TypeConverterEx.IsNotNull(FertilizationFile))
            {
                if (!File.Exists(FertilizationFile))
                    this.Initialized = false;
            }
            else
            {
                this.Initialized = false;
            }
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
                WQFiles wqfile = new WQFiles();
                wqfile.New(configpath, wqinputpath, nhru, nseg, nreach, starttime, endtime, FertilizationFile);
                cancelProgressHandler.Progress("Package_Tool", 50, "wq files copied");

                model.MasterPackage.nps_module = true;

                model.PRMSModel.NewWQPackage(null);
                var wqpck = model.PRMSModel.WQPackage;
                wqpck.Grid = mfgrid;
                wqpck.OnGridUpdated(mfgrid);
                wqpck.Save(null);
                cancelProgressHandler.Progress("Package_Tool", 90, "NPS parameter file created");

                model.MasterPackage.Save(null);
                cancelProgressHandler.Progress("Package_Tool", 100, "contolr file created");


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