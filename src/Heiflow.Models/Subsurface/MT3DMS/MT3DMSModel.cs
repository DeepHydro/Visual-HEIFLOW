using DotSpatial.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface.MT3DMS
{
    [Export(typeof(IBasicModel))]
    [ModelItem]
    public class MT3DMSModel : Modflow
    {
        public MT3DMSModel()
        {
            Name = "MT3DMS";
            this.MFVersion = MODFLOWVersion.MF2005;
            PackageFileNameProvider = new MT3DPackFileNameProvider(this);
            this.Icon = Resources.mf16;
            Description = "A Modular Three-Dimensional Multispecies Transport Model for Simulation of Advection, Dispersion, and Chemical Reactions of Contaminants in Groundwater Systems";
        }

        public override bool New(ICancelProgressHandler progress)
        {
            SelectVersion();
            bool succ = true;
            MFOutputPackage mfout = new MFOutputPackage()
            {
                Owner = this
            };
            AddInSilence(mfout);

            var list_info = _MFNameManager.GetList(100, Project.Name);
            _MFNameManager.Add(list_info);

            foreach (var pck in ModflowService.SupportedPackages)
            {
                if (pck.IsMandatory && pck is IMFPackage)
                {
                    var pckinfo = pck.PackageInfo;
                    pckinfo.FID = _MFNameManager.NextFID();
                    pckinfo.FileName = string.Format("{0}{1}{2}", InputDic, Project.Name, pckinfo.FileExtension);
                    pckinfo.WorkDirectory = this.WorkDirectory;
                    //must be called here
                    _MFNameManager.Add(pck.PackageInfo);
                    pck.FileName = pckinfo.FileName;
                    pck.Owner = this;
                    pck.Clear();
                    pck.Initialize();
                    pck.New();
                    AddInSilence(pck);
                }
            }
            foreach (var pck in Packages.Values)
            {
                if (pck is IMFPackage)
                {
                    (pck as IMFPackage).CompositeOutput(mfout);
                }
            }
            if (MFVersion == MODFLOWVersion.MFNWT)
                FlowPropertyPackage = Select(UPWPackage.PackageName) as UPWPackage;
            else if (MFVersion == MODFLOWVersion.MF2005)
                FlowPropertyPackage = Select(LPFPackage.PackageName) as LPFPackage;
            mfout.Initialize();
            return succ;
        }

        protected override void SelectVersion()
        {
            ModflowService.SelectedMFVersion = MODFLOWVersion.MF2005;
            var need_change_mfpcks = new string[] { SFRPackage.PackageName, UZFPackage.PackageName};
            var need_change_mt3dpcks = new string[] { BTNPackage.PackageName, ADVPackage.PackageName,  DSPPackage.PackageName,
              GCGPackage.PackageName,SSMPackage.PackageName, RCHPackage.PackageName };
            foreach (var pck in ModflowService.SupportedPackages)
            {                
                if ( need_change_mfpcks.Contains( pck.Name))
                {
                    pck.IsMandatory = false;
                }
                if (need_change_mt3dpcks.Contains(pck.Name))
                {
                    pck.IsMandatory = true;
                }
            }
            base.SelectVersion();
        }
    }
}
