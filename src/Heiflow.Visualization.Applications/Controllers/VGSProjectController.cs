using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Models.Generic.Project;
using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Project;
using System.IO;
using Heiflow.Core.Data.ODM;
using HUST.WREIS.Dot3D.Renderable;
using System.Waf.Applications;
using Heiflow.Presentation.Services;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Generic;

namespace Heiflow.Visualization.Applications
{
    [Export(typeof(VGSProjectController))]
    public class VGSProjectController
    {
        private readonly VGSProjectService _ProjectService;
        private readonly IVGSShellService _ShellService;
        private readonly IProjectSerialization _ProjectSerialization;
        private readonly Lazy<VirtualGlobeViewModel> _VirtualGlobeViewModel;
        private readonly LayerService _LayerService;
        private readonly VGSShellViewModel _ShellViewModel;
        private readonly IPackageService _PackageService;
        private readonly IPackageUIService _PackageUIService;

        [ImportingConstructor]
        public VGSProjectController(VGSProjectService prjservice, IVGSShellService shellservice, VGSShellViewModel shellViewModel,
              IProjectSerialization projectSerializer, Lazy<VirtualGlobeViewModel> vgmodel, LayerService layerserv, IPackageService pck_service,
            IPackageUIService packageUIService)
        {
            _ProjectService = prjservice;
            _ShellService = shellservice;
            _ProjectSerialization = projectSerializer;
            _VirtualGlobeViewModel = vgmodel;
            _LayerService = layerserv;
            _ShellViewModel = shellViewModel;
            _PackageService = pck_service;
            _PackageUIService = packageUIService;
        }

          public void Initialize()
          {
              foreach (var im in _ProjectService.Importers)
              {
                  im.LayerService = _LayerService;
                  im.ProjectService = _ProjectService;
                  im.ShellService = _ShellService;
              }
              _ShellViewModel.OpenProject = new DelegateCommand(OpenProject);
              _ShellViewModel.ClearProject = new DelegateCommand(ClearProject);
              _ShellViewModel.PackageUIService = _PackageUIService;
              _ProjectService.Serializer.ProjectOpened += Serializer_ProjectOpened;
              _ProjectService.Serializer.OpenFailed += Serializer_OpenFailed;

              _ShellService.ProjectExplorerView.ProjectExplorer.NodeFactory = _ProjectService.NodeFactory;
              _ShellService.ProjectExplorerView.ProjectExplorer.ContextMenuFactory = _ProjectService.ContextMenuFactory;
              _ShellService.ProjectExplorerView.ProjectExplorer.Initialize();
              ModflowService.SupportedPackages = _PackageService.SupportedMFPackages;
          }

          private void OpenProject(object fn)
          {
              var world = _VirtualGlobeViewModel.Value.VirtualGlobe.CurrentWorld;
              if (_ShellService.ProgressWindow != null)
              {
                  _ShellService.ProgressWindow.ShowView(null);
                  _ShellService.ProgressWindow.Progress("Project",10, "Opening project");
              }
              _ProjectSerialization.Open(fn.ToString(), _ShellService.ProgressWindow);       
           
          }

          private void Serializer_ProjectOpened(object sender, bool e)
          {        
              _ProjectService.Project = _ProjectSerialization.CurrentProject;
              _ShellService.ProjectExplorerView.ProjectExplorer.AddProject(_ProjectService.Project);
              BatchBindUI();
              ModelService.ProjectDirectory = _ProjectService.Project.AbsolutePathToProjectFile;

              _LayerService.ModelLayerGroup = new HUST.WREIS.Dot3D.Renderable.RenderableObjectList(_ProjectService.Project.Name)
              {
                  RenderPriority = RenderPriority.LinePaths
              };

              _LayerService.CoverageLayerGroup = new HUST.WREIS.Dot3D.Renderable.RenderableObjectList("Coverage")
              {
                  RenderPriority = RenderPriority.LinePaths
              };

              var world = _VirtualGlobeViewModel.Value.VirtualGlobe.CurrentWorld;
              _ProjectService.Present(_ProjectService.Project, world);
              _LayerService.Present(_ProjectService.Project, world);

              _VirtualGlobeViewModel.Value.VirtualGlobe.DX3DLayers = _ProjectService.DX3DLayers;
              world.DataLayerList.Add(_LayerService.ModelLayerGroup);
              world.DataLayerList.Add(_LayerService.CoverageLayerGroup);

              (_ShellService.LayerManager as ILayerManagerView).Refresh();

              if (_ShellService.ProgressWindow != null)
              {
                  _ShellService.ProgressWindow.Progress("VGS", 100, "Opening project");
                  _ShellService.ProgressWindow.CloseView();
              }
              var cen = _ProjectService.Project.Model.Grid.BBoxCentroid;
              if (cen != null)
                  _VirtualGlobeViewModel.Value.VirtualGlobe.GotoLatLonAltitude(cen.Y, cen.X, 1500000.0f);
          }

          private void Serializer_OpenFailed(object sender, string e)
          {
              _ShellService.ProgressWindow.Progress("Failed to open project. Error: " + e);
          }

          private void ClearProject(object arg)
          {
                _LayerService.Clear();
              _ShellService.ClearContents();
          }

          public void CloseDB()
          {
              if (_LayerService.ODMSource != null && _LayerService.ODMSource.ODMDB.DbConnection != null)
              {
                  _LayerService.ODMSource.Close();
              }
          }

          private void BatchBindUI()
          {
              foreach (var model in _ProjectService.Project.Model.Children.Values)
              {
                  foreach (var pck in model.Packages.Values)
                  {
                      BindUITo(pck);
                      foreach (var child in pck.Children)
                      {
                          BindUITo(child);
                      }
                  }
              }

              foreach (var pck in _ProjectService.Project.Model.Packages.Values)
              {
                  BindUITo(pck);
                  foreach (var child in pck.Children)
                  {
                      BindUITo(child);
                  }
              }

          }
          private void BindUITo(IPackage pck)
          {
              var props = pck.GetType().GetProperties();
              foreach (var pr in props)
              {
                  var atrs = pr.GetCustomAttributes(typeof(PackageOptionalViewItem), true);
                  if (atrs.Length == 1)
                  {
                      var atr = atrs[0] as PackageOptionalViewItem;
                      var pck_ui = from ui in _PackageUIService.OptionalViews where ui.PackageName == atr.PackageName select ui;
                      if (pck_ui.Count() > 0)
                      {
                          var view = pck_ui.First();
                          pck.OptionalView = view;
                          view.Package = pck;
                      }
                  }
              }
          }
    }
}
