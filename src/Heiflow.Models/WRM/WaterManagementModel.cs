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
        private WRAPackage _WRAPackage;

        public WaterManagementModel()
        {
            Name = "Water Management";
            Icon = Resources.mf16;
            if (!ModelService.SafeMode)
            {
                LargeIcon = Resources.mf32;
                Description = "Water resources management model";
            }
            _WRAPackage = new WRAPackage();
            //_WRAPackage.LoadFailed += this.OnLoadFailed;
        }

        [Browsable(false)]
        public MasterPackage MasterPackage
        {
            get;
            set;
        }

        public override void Attach(IMap map, string directory)
        {
            foreach (var pck in Packages.Values)
            {
                if (pck.State == ModelObjectState.Ready)
                {
                    pck.Attach(map, directory);
                    foreach (var ch in pck.Children)
                    {
                        ch.Attach(map, directory);
                    }
                }
            }
        }

        public override void Initialize()
        {
            Packages.Clear();
        }

        public override LoadingState Load(ICancelProgressHandler progress)
        {
            if (MasterPackage.WRAModule == "auto_wra")
            {
                string msg = "Loading Water Resources Management File...";
                progress.Progress(this.Name, 1, msg);
                _WRAPackage.FileName = MasterPackage.WRAModuleFile;
                _WRAPackage.Owner = this;
                _WRAPackage.Initialize();
                AddInSilence(_WRAPackage);
                return _WRAPackage.Load(progress);
            }
            else
            {
                return LoadingState.Normal;
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
        public override void Clear()
        {
            foreach (var pck in Packages.Values)
                pck.Clear();
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
