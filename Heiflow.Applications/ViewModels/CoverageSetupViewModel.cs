// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Presentation;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Applications.ViewModels
{
  
    [Export]
    public class CoverageSetupViewModel : ViewModel<ICoverageSetupView>
    {
        private IWindowService _WindowService;
        private IShellService _ShellService;
        private PackageCoverage _LayerParameter;
        private IProjectController _ProjectController;

        [ImportingConstructor]
        public CoverageSetupViewModel(ICoverageSetupView view, IWindowService win,  IShellService shell, IProjectController pjc)
            : base(view)
        {
            _WindowService = win;
            _ShellService = shell;
            _ProjectController = pjc;
            CoverageType = CoverageType.Feature;
        }

        public IShellService ShellService
        {
            get
            {
                return _ShellService;
            }
        }

        public IProjectController ProjectController
        {
            get
            {
                return _ProjectController;
            }
        }

        public PackageCoverage Coverage
        {
            get
            {
                return _LayerParameter;
            }
            set
            {
                _LayerParameter = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Coverage"));
                if (_LayerParameter is FeatureCoverage)
                    CoverageType = CoverageType.Feature;
                else
                    CoverageType = CoverageType.Raster;
            }
        }

        public CoverageType CoverageType
        {
            get;
            set;
        }

        public void ShowView()
        {
            if (ViewCore == null)
            {
                var view = _WindowService.NewParameterTableWindow();
                view.ShowView(null);
            }
            else
            {
                ViewCore.ShowView(null);
            }
        }
    }
}
