using Heiflow.Core.Data.ODM;
using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Project;
using Heiflow.Visualization.Renderable;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Visualization.Applications
{
    [Export]
    public class LayerController
    {
        private Lazy<VGSShellViewModel> _ShellViewModel;
        private Lazy<VirtualGlobeViewModel> _VirtualGlobeViewModel;
        private LayerService _LayerService;
        private VGSProjectService _VGSProjectService;
        private LayerManagerViewModel _LayerManagerViewModel;
        private IVGSShellService _ShellService;
        private SiteViewModel _SiteViewModel;
        private SFRViewModel _SFRViewModel;

        [ImportingConstructor]
        public LayerController(Lazy<VGSShellViewModel> shellViewModel, Lazy<VirtualGlobeViewModel> vgvm,
            LayerService layerserv, VGSProjectService prj, LayerManagerViewModel layervm, IVGSShellService shell, SiteViewModel sitevm, SFRViewModel sfr)
        {
            _ShellViewModel = shellViewModel;
            _VirtualGlobeViewModel = vgvm;
            _LayerService = layerserv;
            _VGSProjectService = prj;
            _LayerManagerViewModel = layervm;
            _ShellService = shell;
            _SiteViewModel = sitevm;
            _SFRViewModel = sfr;
        }

        public VGSShellViewModel ShellViewModel
        {
            get
            {
                return _ShellViewModel.Value;
            }
        }

        public SiteViewModel SiteViewModel
        {
            get
            {
                return _SiteViewModel;
            }
        }

        public SFRViewModel SFRViewModel
        {
            get
            {
                return _SFRViewModel;
            }
        }

        public void Initialize()
        {
            _ShellService.LayerManager = _LayerManagerViewModel.View;
            ShellViewModel.LoadShpFile = new DelegateCommand(LoadShpFile);
            ShellViewModel.LoadRasterFile = new DelegateCommand(LoadRasterFile);
            ShellViewModel.Load3DModel = new DelegateCommand(Load3DModel);
            ShellViewModel.LoadODM = new DelegateCommand(LoadODM);
            _LayerManagerViewModel.RefreshCommand = new DelegateCommand(Refresh);
            _LayerManagerViewModel.RemoveCommand = new DelegateCommand(Remove);
        }

        public void Run()
        {
            var icons = new ClickableIcons("Observation Sites")
            {
                IsOn = true
            };
            var world = _VirtualGlobeViewModel.Value.VirtualGlobe.CurrentWorld;
            var layer = new RenderableSitesLayer(world, icons);
            layer.ViewSiteDataClicked += layer_SiteClicked;
            layer.SitePropertyClicked += layer_SitePropertyClicked;
            _LayerService.SitesLayer = layer;
            world.DataLayerList.Add(icons);
            _LayerService.RootLayers = world.DataLayerList;
        }

        private void Remove(object obj)
        {
           // var ro = obj as RenderableObject;
            //if (ro != null && ro.ParentList.Name == "Coverage")
            //{

            //    _LayerService.ModelLayerGroup.Remove(ro);
            //    ro.Dispose();
            //}
        }
        private void Refresh(object obj)
        {
            (_ShellService.LayerManager as ILayerManagerView).Refresh();
        }
        private void layer_SiteClicked(object sender, Core.Data.ODM.Site e)
        {
            _ShellService.SelectPanel(DockPanelNames.SitePanel);
            _ShellService.SiteView.Site = e;
        }

        private void layer_SitePropertyClicked(object sender, Site e)
        {
            _ShellService.SelectPanel(DockPanelNames.PropertyPanel);
            _ShellService.PropertyView.SelectedObject = e;
        }

        private void LoadShpFile(object para)
        {
            ShapeFeatureProvider provider = new ShapeFeatureProvider();
            var ro = provider.Load(para.ToString(), _ShellService.VirtualGlobeView.VirtualGlobe.CurrentWorld);
            if(_VGSProjectService.Project != null)
            {
                _LayerService.CoverageLayerGroup.Add(ro);
            }
            else
            {
                _LayerService.RootLayers.Add(ro);
            }
            Refresh(null);
        }

        private void Load3DModel(object para)
        {
            ModelFeatureProvider provider = new ModelFeatureProvider();
            var ro = provider.Load(para.ToString(), _ShellService.VirtualGlobeView.VirtualGlobe.CurrentWorld);
            if (_VGSProjectService.Project != null)
            {
                _LayerService.CoverageLayerGroup.Add(ro);
            }
            else
            {
                _LayerService.RootLayers.Add(ro);
            }
            Refresh(null);
        }
        private void LoadODM(object para)
        {
            ODMSource source = new ODMSource();
            string msg = "";
            if (source.Open(para.ToString(), ref msg))
            {
                _LayerService.ODMSource = source;
                var sites = source.GetSites(new QueryCriteria());
                _LayerService.SitesLayer.ShowSites(sites);
            }
        }
        private void LoadRasterFile(object para)
        {

        }
    }
}
