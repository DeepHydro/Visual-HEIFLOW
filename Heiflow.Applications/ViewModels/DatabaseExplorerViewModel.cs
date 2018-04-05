// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Applications.Views;
using Heiflow.Core.Data.ODM;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Applications.ViewModels
{
    [Export]
    public class DatabaseExplorerViewModel : ViewModel<IDatabaseExplorerView>
    {
        private IShellService _Shell;
        private IProjectService _ProjectService;

        [ImportingConstructor]
        public DatabaseExplorerViewModel(IDatabaseExplorerView view, IShellService shell, IProjectService prj)
            : base(view)
        {
            _Shell = shell;
            _ProjectService = prj;
        }

        public ODMSource ODMSource
        {
            get
            {
                if (_ProjectService.Project != null)
                    return _ProjectService.Project.ODMSource;
                else
                    return null;
            }
            set
            {
                if (_ProjectService.Project != null)
                    _ProjectService.Project.ODMSource = value;
            }
        }

        public IShellService ShellService
        {
            get
            {
                return _Shell;
            }
        }

        public IProjectService ProjectService
        {
            get
            {
                return _ProjectService;
            }
        }
    }
}
