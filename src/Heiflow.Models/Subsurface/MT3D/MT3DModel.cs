using Heiflow.Models.Generic;
using Heiflow.Models.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface.MT3D
{
    [Export(typeof(IBasicModel))]
    [ModelItem]
    public class MT3DModel : BaseModel
    {
        public MT3DModel()
        {
            Name = "MT3D";
            PackageFileNameProvider = new MT3DPackFileNameProvider(this);
            this.Icon = Resources.mf16;
            this.TimeService = new TimeService("Subsurface Timeline")
            {
                UseStressPeriods = true
            };
            this.TimeService.Updated += this.OnTimeServiceUpdated;

            _IsDirty = false;
            Description = "A Modular Three-Dimensional Multispecies Transport Model for Simulation of Advection, Dispersion, and Chemical Reactions of Contaminants in Groundwater Systems";
        }

        public override LoadingState Load(DotSpatial.Data.ICancelProgressHandler progress)
        {

            throw new NotImplementedException();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override bool LoadGrid(DotSpatial.Data.ICancelProgressHandler progress)
        {
            throw new NotImplementedException();
        }

        public override bool Validate()
        {
            throw new NotImplementedException();
        }

        public override bool New(DotSpatial.Data.ICancelProgressHandler progress)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Save(DotSpatial.Data.ICancelProgressHandler progress)
        {
            throw new NotImplementedException();
        }

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            throw new NotImplementedException();
        }
    }
}
