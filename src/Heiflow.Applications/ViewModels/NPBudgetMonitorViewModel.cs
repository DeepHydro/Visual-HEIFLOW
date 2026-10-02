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

using DotSpatial.Controls;
using Heiflow.Applications.Views;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Running;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Applications.ViewModels
{
    /// <summary>
    /// N / P 质量平衡分析视图模型。
    /// 与 StateMonitorViewModel 的区别：这里的 Monitor 是显式创建的（N、P 两个），
    /// 而不是通过 MEF 收集所有 IFileMonitor，从而避免与水量平衡监视器相互干扰。
    /// </summary>
    [Export]
    public class NPBudgetMonitorViewModel : ViewModel<INPBudgetMonitorView>
    {
        private const string FileNameFormat = "sz_{0}_budget.csv";
        [ImportingConstructor]
        public NPBudgetMonitorViewModel(INPBudgetMonitorView view)
            : base(view)
        {
            var monitors = new List<IFileMonitor>();
            monitors.Add(new NPBudgetMonitor("N"));
            monitors.Add(new NPBudgetMonitor("P"));
            Monitors = monitors;
            Monitor = monitors[0];
        }

        /// <summary>
        /// N 与 P 两个质量平衡监视器
        /// </summary>
        public IEnumerable<IFileMonitor> Monitors
        {
            get;
            private set;
        }

        /// <summary>
        /// 当前选中的监视器
        /// </summary>
        public IFileMonitor Monitor
        {
            get;
            set;
        }

        public DelegateCommand LoadCommand
        {
            get;
            set;
        }

        public void Show()
        {
            ViewCore.Show();
        }

        public void Close()
        {
            ViewCore.Close();
        }

        /// <summary>
        /// 依据元素名（N / P）返回对应的监视器
        /// </summary>
        public NPBudgetMonitor GetMonitor(string element)
        {
            foreach (var monitor in Monitors)
            {
                var np = monitor as NPBudgetMonitor;
                if (np != null && np.Element == element)
                    return np;
            }
            return null;
        }

        /// <summary>
        /// 项目打开后同步文件名（相对路径相对工作目录解析）
        /// </summary>
        public void OnProjectOpened(IMap map, IProject project)
        {
            // 文件名为相对路径，相对工作目录解析
            if (string.IsNullOrEmpty(ModelService.WorkDirectory))
                return;
            foreach (NPBudgetMonitor monitor in Monitors)
            {
                var fn =  Path.Combine(ModelService.WorkDirectory, ".\\output\\" + string.Format(FileNameFormat, monitor.Element));
                monitor.Watcher.FileName = fn;
                monitor.FileName = fn;
            }
        }
    }
}
