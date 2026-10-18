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
        private DelegateCommand _SaveProjectCommand;

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
              _SaveProjectCommand = new DelegateCommand(SaveProject, CanSaveProject);
              // A command says nothing of its own accord about whether it may be run, so the Save
              // item of the menu is told again when a project is opened, closed or saved.
              _ShellViewModel.SaveProject = _SaveProjectCommand;
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

          /// <summary>
          /// A project can be saved when one is open and the file it came from is known, which is
          /// where saving puts it back.
          /// </summary>
          private bool CanSaveProject(object arg)
          {
              var project = _ProjectService.Project;
              return project != null && !string.IsNullOrEmpty(project.FullProjectFileName);
          }

          /// <summary>
          /// Writes the project back to its own file. The layers that were added to it since it was
          /// opened, a shapefile or a water body among them, go into the file with it, and are
          /// loaded again the next time the project is opened.
          /// </summary>
          private void SaveProject(object arg)
          {
              if (!CanSaveProject(arg))
              {
                  Heiflow.Core.Log.Write(Heiflow.Core.Log.Levels.Warning, "PRJ",
                      "There is no project open to save, or the file it was opened from is not known.");
                  return;
              }
              var project = _ProjectService.Project;
              try
              {
                  _ProjectSerialization.Save(project.FullProjectFileName, project);
                  _SaveProjectCommand.RaiseCanExecuteChanged();
              }
              catch (Exception ex)
              {
                  Heiflow.Core.Log.Write(Heiflow.Core.Log.Levels.Error, "PRJ",
                      "The project could not be saved: " + ex.Message);
                  if (_ShellService.ProgressWindow != null)
                      _ShellService.ProgressWindow.Progress("Failed to save project. Error: " + ex.Message);
              }
          }

          private void Serializer_ProjectOpened(object sender, bool e)
          {        
              _ProjectService.Project = _ProjectSerialization.CurrentProject;
              _SaveProjectCommand.RaiseCanExecuteChanged();
              _ShellService.ProjectExplorerView.ProjectExplorer.AddProject(_ProjectService.Project);
              BatchBindUI();
              ModelService.ProjectDirectory = _ProjectService.Project.AbsolutePathToProjectFile;

              _LayerService.ModelLayerGroup = new HUST.WREIS.Dot3D.Renderable.RenderableObjectList(_ProjectService.Project.Name)
              {
                  RenderPriority = RenderPriority.LinePaths
              };

              // The two groups the project puts its layers into: the layers of the model being
              // simulated under the name of the project, and everything read out of a file belonging
              // to it, a shapefile or a water body among them, under Feature Layers.
              _LayerService.FeatureLayerGroup = new HUST.WREIS.Dot3D.Renderable.RenderableObjectList("Feature Layers")
              {
                  RenderPriority = RenderPriority.LinePaths
              };

              var world = _VirtualGlobeViewModel.Value.VirtualGlobe.CurrentWorld;
              _ProjectService.Present(_ProjectService.Project, world);
              _LayerService.Present(_ProjectService.Project, world);

              _VirtualGlobeViewModel.Value.VirtualGlobe.DX3DLayers = _ProjectService.DX3DLayers;
              world.DataLayerList.Add(_LayerService.ModelLayerGroup);
              world.DataLayerList.Add(_LayerService.FeatureLayerGroup);

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
