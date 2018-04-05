// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Applications.Controllers;
using Heiflow.Models.Generic;
using Heiflow.Models.IO;
using Heiflow.Models.Running;
using Heiflow.Presentation;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Waf.Applications;
using System.Waf.Applications.Services;

namespace Heiflow.Applications
{
    [Export(typeof(IModuleController))]
    public class VHFAppManager : MyAppManager, IModuleController
    {
        private IProjectController _ProjectController;
        private StateMonitorController _StateMonitorController;
        private RunningMonitorController _RunningMonitorController;
        private DatabaseExplorerController _DatabaseExplorerController;
        private LookupTableController _ParameterTableController;
        private CoverageSetupController _CoverageSetupController;
        private IWindowService _WindowService;
        private IPackageUIService _PackageUIService;

        [ImportingConstructor]
        public VHFAppManager()
        {
            AppMode = Presentation.Controls.AppMode.VHF;
        }

        [Import(typeof(IProjectController))]
        public IProjectController ProjectController
        {
            get
            {
                return _ProjectController;
            }
            set
            {
                _ProjectController = value;
            }
        }

        [Import(typeof(StateMonitorController))]
        public StateMonitorController StateMonitor
        {
            get
            {
                return _StateMonitorController;
            }
            set
            {
                _StateMonitorController = value;
            }
        }

        [Import(typeof(RunningMonitorController))]
        public RunningMonitorController RunningMonitor
        {
            get
            {
                return _RunningMonitorController;
            }
            set
            {
                _RunningMonitorController = value;
            }
        }


        [Import(typeof(DatabaseExplorerController))]
        public DatabaseExplorerController DatabaseExplorerController
        {
            get
            {
                return _DatabaseExplorerController;
            }
            set
            {
                _DatabaseExplorerController = value;
            }
        }

        [Import(typeof(LookupTableController))]
        public LookupTableController ParameterTableController
        {
            get
            {
                return _ParameterTableController;
            }
            set
            {
                _ParameterTableController = value;
            }
        }

        [Import(typeof(CoverageSetupController))]
        public CoverageSetupController CoverageSetupController
        {
            get
            {
                return _CoverageSetupController;
            }
            set
            {
                _CoverageSetupController = value;
            }
        }

        [Import(typeof(IWindowService))]
        public IWindowService WindowService
        {
            get
            {
                return _WindowService;
            }
            set
            {
                _WindowService = value;
            }
        }

           [Import(typeof(IPackageUIService))]
        public IPackageUIService PackageUIService
        {
            get
            {
                return _PackageUIService;
            }
            set
            {
                _PackageUIService = value;
            }
        }


        public IShellService ShellService
        {
            get
            {
                return _ProjectController.ShellService;
            }
        }

        public string AppName
        {
            get;
            set;
        }
        public Icon Icon
        {
            get;
            set;
        }
        public DotSpatial.Controls.AppManager MapAppManager
        {
            get;
            set;
        }

        public void Initialize()
        {
            _ConfigManager.SetPath(ApplicationPath);
            _ProjectController.MapAppManager = MapAppManager;
            BaseModel.ConfigPath = _ConfigManager.ConfigPath;
            _StateMonitorController.Initialize();
            _RunningMonitorController.Initialize();
            _ProjectController.Initialize();
            _ParameterTableController.Initialize();
            _CoverageSetupController.Initialze();
        }


        public void Run()
        {

        }

        public void Shutdown()
        {
            foreach(var view in PackageUIService.OptionalViews)
            {
              //  view.CloseRequired = true;
            }
            _StateMonitorController.Shutdown();
            _RunningMonitorController.Shutdown();
            _ProjectController.Shutdown();
            _ParameterTableController.Shutdown();
            _CoverageSetupController.Shutdown();
        }
        public bool CheckLicense()
        {
            bool ischecked = false;
            string path = Path.Combine(this.ApplicationPath, "vgs.dll");
            if (File.Exists(path))
            {
                SecurityFile _SecurityFile = new SecurityFile(path);
                _SecurityFile.Validate();
                ischecked = _SecurityFile.Authenticated;
                if (ischecked)
                {
                    //update datetime
                    _SecurityFile.Date = System.DateTime.Now;
                    _SecurityFile.Update("VHF");
                }
            }
            return ischecked;
        }
    }
}
