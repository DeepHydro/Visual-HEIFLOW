//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using DotSpatial.Controls;
using DotSpatial.Symbology;
using Heiflow.Applications.Spatial;
using Heiflow.Applications.ViewModels;
using Heiflow.Core.Data;
using Heiflow.Core.Utility;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Waf.Applications;
using System.Windows.Forms;

namespace Heiflow.Applications.Controllers
{
    [Export(typeof(IProjectController))]
    public class ProjectController : IProjectController
    {
        private readonly IShellService _ShellService;
        private readonly IPackageService _PackageService;
        private readonly StateMonitorViewModel _StateMonitor;
        private readonly DelegateCommand _NewPrjCommand;
        private readonly DelegateCommand _OpenPrjCommand;
        private readonly DelegateCommand _ClosePrjCommand;
        private readonly DelegateCommand _ImportCommand;
        private readonly DelegateCommand _SaveCommand;
        private readonly IProjectService _ProjectService;
        private readonly IActiveDataService _ActiveDataService;
        private readonly IPackageUIService _PackageUIService;
        private MapFunctionManager _MapFunctionManager;
        private RunningMonitorViewModel _RunningMonitorViewModel;
        private ConceputalModelManager _ConceputalModelManager;

        [ImportingConstructor]
        public ProjectController(IShellService shellService, IActiveDataService dataservice, IProjectService projectServ,
     IPackageUIService packageUIService, StateMonitorViewModel state, IPackageService pck_service, RunningMonitorViewModel rm_vm)
        {
            _ShellService = shellService;
            _ActiveDataService = dataservice;
            _ProjectService = projectServ;
            _PackageService = pck_service;
            _StateMonitor = state;
            _PackageUIService = packageUIService;
            _NewPrjCommand = new DelegateCommand(AddNewProject, CanAddNewProject);
            _OpenPrjCommand = new DelegateCommand(p => OpenProject(p), t => CanOpenProject(t, true));  
            _ImportCommand = new DelegateCommand(p => ImportFrom(p), CanImportFrom);
            _SaveCommand = new DelegateCommand(SaveProject, CanSaveProject);
            _ClosePrjCommand = new DelegateCommand(CloseProject, CanCloseProject);
            _MapFunctionManager = new MapFunctionManager();
            _ConceputalModelManager = new ConceputalModelManager();
            ProjectController.Instance = this;
            _RunningMonitorViewModel = rm_vm;
        }

        /// <summary>
        /// Gets the list tools available.
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IModelTool> Tools { get; set; }

        public IProject Project
        {
            get
            {
                return _ProjectService.Project;
            }
        }

        public DelegateCommand New
        {
            get
            {
                return _NewPrjCommand;
            }
        }

        public DelegateCommand Open
        {
            get
            {
                return _OpenPrjCommand;
            }
        }

        public DelegateCommand Import
        {
            get
            {
                return _ImportCommand;
            }
        }
        public DelegateCommand Save
        {
            get
            {
                return _SaveCommand;
            }
        }

        public DelegateCommand Close
        {
            get
            {
                return _ClosePrjCommand;
            }
        }

        /// <summary>
        /// Map App Manager
        /// </summary>
        public AppManager MapAppManager
        {
            get;
            set;
        }

        public IShellService ShellService
        {
            get
            {
                return _ShellService;
            }
        }

        public IProjectService ProjectService
        {
            get
            {
                return _ProjectService;
            }
        }
        public RunningMonitorViewModel RunningMonitorViewModel
        {
            get
            {
                return _RunningMonitorViewModel;
            }
        }
        public IActiveDataService ActiveDataService
        {
            get
            {
                return _ActiveDataService;
            }
        }
        public static IProjectController Instance
        {
            get;
            private set;
        }
        public void Initialize()
        {
            _ShellService.MapAppManager = this.MapAppManager;
            _ProjectService.Serializer.ProjectOpened +=Serializer_ProjectOpened;
            _ProjectService.Serializer.OpenFailed += Serializer_OpenFailed;
            _ProjectService.Serializer.App = MapAppManager;
            _ProjectService.ProjectOpenedOrCreated += _MapFunctionManager.OnProjectOpened;
            _ProjectService.ProjectOpenedOrCreated += _ConceputalModelManager.OnProjectOpened;
            _ProjectService.ProjectOpenedOrCreated += _StateMonitor.OnProjectOpened;

            _ShellService.ProjectExplorer.NodeFactory = _ProjectService.NodeFactory;
            _ShellService.ProjectExplorer.ContextMenuFactory = _ProjectService.ContextMenuFactory;
            _ShellService.ProjectExplorer.Initialize();
            ModflowService.SupportedPackages = _PackageService.SupportedMFPackages;
        }

        #region New
        private void AddNewProject()
        {
            //if (!_Initialized)
            //    Initialize();
            _ShellService.NewProjectWindow.ShowDialog();
        }

        private bool CanAddNewProject()
        {
            return true;
        }
        #endregion

        #region Open
        private bool CanOpenProject(object fn, bool can)
        {
            return true;
        }

        public void OpenProject(object filename)
        {
            if (_ShellService.ProgressWindow != null)
            {
                _ShellService.ProgressWindow.DoWork += ProgressPanel_DoOpenProject;
                _ShellService.ProgressWindow.WorkCompleted += ProgressWindow_DoOpenProjectCompleted;
                _ShellService.ProgressWindow.EnableCancel = false;
                _ShellService.ProgressWindow.Run(filename);
            }
        }

        private void ProgressPanel_DoOpenProject(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
           // _ShellService.ProgressWindow.Reset();
            _ShellService.ProgressWindow.ProgressBarStyle = ProgressBarStyle.Marquee;
            _ProjectService.Serializer.Open(e.Argument.ToString(), _ShellService.ProgressWindow);          
        }
        private void Serializer_ProjectOpened(object sender, bool e)
        {
            _ProjectService.Project = _ProjectService.Serializer.CurrentProject;
            _ShellService.ProjectExplorer.AddProject(_ProjectService.Project);
            ModelService.ProjectDirectory = _ProjectService.Project.AbsolutePathToProjectFile;
            _ShellService.ProgressWindow.Progress("Loading map layers...");
        }

        private void ProgressWindow_DoOpenProjectCompleted(object sender, EventArgs e)
        {
            MapAppManager.SerializationManager.OpenProject(_ProjectService.Project.FullMapFileName);
            _ProjectService.Project.Map = MapAppManager.Map;
            _ProjectService.Project.AttachFeatures();
            BatchBindUI();
            if (_ProjectService.Project.ODMSource != null)
            {
                _ProjectService.Project.ODMSource.WorkDirectory = _ProjectService.Project.FullModelWorkDirectory;
                _ProjectService.Project.ODMSource.Open();
            }
          //  ShellService.ProgressWindow.Progress("Finished.");
            _ProjectService.RaiseProjectOpenedOrCreated(MapAppManager.Map, this.Project);
            _ShellService.ProgressWindow.DoWork -= ProgressPanel_DoOpenProject;
            _ShellService.ProgressWindow.WorkCompleted -= ProgressWindow_DoOpenProjectCompleted;
            _ShellService.ProgressWindow.ProgressBarStyle = ProgressBarStyle.Continuous;
        }
        private void Serializer_OpenFailed(object sender, string e)
        {
            _ShellService.ProgressWindow.Progress("Failed to open project. Error message: " + e);
        }
        #endregion

        #region Import
        public void ImportFrom(object importProperty)
        {
            _ShellService.ProgressWindow.DoWork += ProgressPanel_DoImport;
            _ShellService.ProgressWindow.WorkCompleted += ProgressWindow_DoImportCompleted;
            _ShellService.ProgressWindow.EnableCancel = false;
            _ShellService.ProgressWindow.Run(importProperty);
        }
        private bool CanImportFrom(object importProperty)
        {
            bool can = false;
            var _ImportProperty = importProperty as IImportProperty;
            var _Project = _ProjectService.Serializer.CurrentProject;
            var extent = Path.GetExtension(_ImportProperty.FileName);
            foreach (var mod in _ProjectService.Serializer.SurpportedModelLoaders)
            {
                if (mod.Extension.ToLower() == extent)
                {
                    ModelService.WorkDirectory = _ImportProperty.WorkDirectory;
                    _Project.RelativeModelWorkDirectory = _ImportProperty.WorkDirectory;
                    _Project.RelativeControlFileName = DirectoryHelper.RelativePathTo(_ImportProperty.FileName, ModelService.WorkDirectory);
                    can = mod.CanImport(_Project);
                    break;
                }
            }
            return can;
        }
        private void ProgressPanel_DoImport(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var _ImportProperty = e.Argument as IImportProperty;
            var _Project = _ProjectService.Serializer.CurrentProject;
            //_ShellService.ProgressWindow.Reset();
            _ShellService.ProgressWindow.ProgressBarStyle = ProgressBarStyle.Marquee;
            var extent = Path.GetExtension(_ImportProperty.FileName);
            foreach (var mod in _ProjectService.Serializer.SurpportedModelLoaders)
            {
                if (mod.Extension.ToLower() == extent)
                {
                    _Project.RelativeModelWorkDirectory = _ImportProperty.WorkDirectory;
                    mod.Import(_Project, _ImportProperty, _ShellService.ProgressWindow);
                    break;
                }
            }
            _ShellService.ProgressWindow.Progress("Finished.");
        }
        private void ProgressWindow_DoImportCompleted(object sender, EventArgs e)
        {
            _ProjectService.Project = _ProjectService.Serializer.CurrentProject;
            _ShellService.ProjectExplorer.AddProject(_ProjectService.Project);
            ModelService.ProjectDirectory = _ProjectService.Project.AbsolutePathToProjectFile;
            _ProjectService.Project.Map = MapAppManager.Map;
            _ProjectService.Project.AttachFeatures();
            _ProjectService.RaiseProjectOpenedOrCreated(MapAppManager.Map, _ProjectService.Project);

            BatchBindUI();    
            _ShellService.ProgressWindow.DoWork -= ProgressPanel_DoImport;
            _ShellService.ProgressWindow.WorkCompleted -= ProgressWindow_DoImportCompleted;
            _ShellService.ProgressWindow.ProgressBarStyle = ProgressBarStyle.Continuous;

        }
        #endregion

        #region Save
        private void SaveProject()
        {
            _ShellService.ProgressWindow.DoWork += ProgressPanel_DoSaveProject;
            _ShellService.ProgressWindow.WorkCompleted += ProgressWindow_SaveProjectCompleted;
            _ShellService.ProgressWindow.EnableCancel = false;
            _ShellService.ProgressWindow.Run(null);
        }
        private bool CanSaveProject()
        {
            return Project != null;
        }

        private void ProgressPanel_DoSaveProject(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
      //      _ShellService.ProgressWindow.Reset();
            _ShellService.ProgressWindow.ProgressBarStyle = ProgressBarStyle.Marquee;
            _ProjectService.Serializer.Save(Project.FullProjectFileName, Project);
            Project.Model.Save(_ShellService.ProgressWindow);
            ShellService.ProgressWindow.Progress("Saving map layers...");
            MapAppManager.SerializationManager.SaveProject(_ProjectService.Project.FullMapFileName);
            ShellService.ProgressWindow.Progress("Finshied.");
        }
        private void ProgressWindow_SaveProjectCompleted(object sender, EventArgs e)
        {

            _ShellService.ProgressWindow.DoWork -= ProgressPanel_DoSaveProject;
            _ShellService.ProgressWindow.WorkCompleted -= ProgressWindow_SaveProjectCompleted;
            _ShellService.ProgressWindow.ProgressBarStyle = ProgressBarStyle.Continuous;
        }
        #endregion

        #region Close
        private void CloseProject()
        {

        }
        private bool CanCloseProject()
        {
            return true;
        }
        public void Shutdown()
        {
            if (_ProjectService.Project != null && _ProjectService.Project.ODMSource != null)
            {
                _ProjectService.Project.ODMSource.Close();
            }
        }
        #endregion

        public void Clear()
        {
            //_ProjectService.Serializer.ProjectOpened -= Serializer_ProjectOpened;
            //_ProjectService.Serializer.OpenFailed -= Serializer_OpenFailed;
            //_ProjectService.ProjectOpenedOrCreated -= _MapFunctionManager.OnProjectOpened;
            //_ProjectService.ProjectOpenedOrCreated -= _ConceputalModelManager.OnProjectOpened;
            //_Initialized = false;
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