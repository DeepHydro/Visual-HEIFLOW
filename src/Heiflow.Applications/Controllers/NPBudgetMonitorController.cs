//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
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

using Heiflow.Applications.ViewModels;
using Heiflow.Applications.Views;
using Heiflow.Models.Running;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Applications.Controllers
{
    /// <summary>
    /// N / P 质量平衡分析控制器，负责装配命令并调度数据加载
    /// </summary>
    [Export]
    public class NPBudgetMonitorController
    {
        private NPBudgetMonitorViewModel _ViewModel;

        [ImportingConstructor]
        public NPBudgetMonitorController(NPBudgetMonitorViewModel viewModel)
        {
            _ViewModel = viewModel;
            _ViewModel.LoadCommand = new DelegateCommand(Load, CanLoad);
        }

        public NPBudgetMonitorViewModel ViewModel
        {
            get
            {
                return _ViewModel;
            }
        }

        public void Initialize()
        {
        }

        public void Shutdown()
        {
        }

        private void Load()
        {
            foreach (var monitor in _ViewModel.Monitors)
            {
                monitor.Clear();
                var np = monitor as NPBudgetMonitor;
                if (np == null)
                    continue;

                monitor.Watcher.Load(monitor.FileName, monitor.ConvertToStrepRate, monitor.VarIndexIsConvert);
                np.MapColumns();
            }
        }

        private bool CanLoad()
        {
            bool can = true;
            foreach (var monitor in _ViewModel.Monitors)
            {
                if (monitor.Watcher.State == RunningState.Busy)
                {
                    can = false;
                    break;
                }
            }
            return can;
        }
    }
}
