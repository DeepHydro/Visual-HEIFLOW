using Heiflow.Applications;
using Heiflow.Core.Hydrology;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.ConceptualModel.SFR
{
    public class ModifyLayerByFHD : MapLayerRequiredTool
    {
        public ModifyLayerByFHD()
        {
            Name = "Modify Layer By FHD";
            Category = Cat_CMG;
            SubCategory = "SFR";
            Description = "Modify Layer Based on FHD";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            NoDataValue = -9999;
            NewAquiferLayer = 2;
        }

        [Category("Input")]
        [Description("The data cube used to set layer of SFR. The Data Cube style should be mat[0][0][:]")]
        public string GWHeadDataCube
        {
            get;
            set;
        }
        [Category("Input")]
        [Description("No DtaValue")]
        public float NoDataValue
        {
            get;
            set;
        }
        [Category("Input")]
        [Description("New Aquifer Layer")]
        public int NewAquiferLayer
        {
            get;
            set;
        }
        public override void Initialize()
        {
            Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
            var vec_src = GetVector(GWHeadDataCube);

            if (sfr != null && vec_src != null)
            {
                var rvnet = sfr.RiverNetwork;
                foreach (var river in rvnet.Rivers)
                {
                    foreach (Reach reach in river.Reaches)
                    {
                        var index = grid.Topology.GetSerialIndex(reach.IRCH - 1, reach.JRCH - 1);
                        if(vec_src[index] == NoDataValue)
                        {
                            reach.KRCH = NewAquiferLayer;
                        }
                    }
                }
                sfr.NetworkToMat();
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
