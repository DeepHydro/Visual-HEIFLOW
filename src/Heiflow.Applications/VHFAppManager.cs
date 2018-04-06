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
