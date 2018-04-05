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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

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
