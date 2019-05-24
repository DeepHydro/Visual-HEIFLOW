using DotSpatial.Controls;
using DotSpatial.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.WRM
{
    [Export(typeof(IBasicModel))]
    [ModelItem]
    public class WaterManagementModel : BaseModel
    {
        public WaterManagementModel()
        {
            Name = "Water Management";
            Icon = Resources.mf16;
            LargeIcon = Resources.mf32;
            Description = "Water resources management model";
        }
        [Browsable(false)]
        public MasterPackage MasterPackage
        {
            get;
            set;
        }

        public override void Attach(IMap map, string directory)
        {
           
        }

        public override void Clear()
        {
            foreach (var pck in Packages.Values)
                pck.Clear();
        }

        public override void Initialize()
        {
            Packages.Clear();
        }

        public override bool Load(ICancelProgressHandler progress)
        {
            if (MasterPackage.WRAModule == "auto_wra")
            {
                WRAPackage wra = new WRAPackage();
                wra.FileName = MasterPackage.WRAModuleFile;
                wra.Owner = this;
                wra.Initialize();
                Packages.Add(wra.Name, wra);
                return wra.Load(progress);
            }
            else
            {
                return true;
            }
        }

        public override bool LoadGrid(ICancelProgressHandler progress)
        {
            return true;
        }

        public override bool New(ICancelProgressHandler progress)
        {
            return true;
        }

        public override void Save(ICancelProgressHandler progress)
        {
            foreach (var pck in Packages.Values)
                pck.Save(progress);
        }

        public override bool Validate()
        {
            return true;
        }
    }
}
