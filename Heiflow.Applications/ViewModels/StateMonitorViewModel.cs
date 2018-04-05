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
using Heiflow.Applications.Views;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Running;
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
    public class StateMonitorViewModel : ViewModel<IStateMonitorView>
    {
        private DelegateCommand _LoadCommand;

        [ImportingConstructor]
        public StateMonitorViewModel(IStateMonitorView view)
            : base(view)
        {

        }

        [ImportMany(typeof(IFileMonitor))]
        public IEnumerable<IFileMonitor> Monitors
        {
            get;
            set;
        }

        public IFileMonitor Monitor
        {
            get;
            set;
        }

        public DelegateCommand LoadCommand
        {
            get
            {
                return _LoadCommand;
            }
            set
            {
                _LoadCommand = value;
            }
        }

        public void Show()
        {
            ViewCore.Show();
        }

        public void Close()
        {
            ViewCore.Close();
        }
        public void OnProjectOpened(IMap map, IProject Project)
        {
            var master = Project.Model.GetPackage(Heiflow.Models.Integration.HeiflowModel.MasterPackageName);
            if (master != null)
            {
                var props = master.GetType().GetProperties();
                foreach (var pr in props)
                {
                    var atrs = pr.GetCustomAttributes(typeof(FileMonitorItem), true);
                    if (atrs.Length == 1)
                    {
                        var atr = atrs[0] as FileMonitorItem;
                        foreach (var monitor in Monitors)
                        {
                            if (monitor.MonitorName == atr.MonitorName)
                            {
                                monitor.FileName = master.GetType().GetProperty(pr.Name).GetValue(master).ToString();
                                break;
                            }

                        }
                    }
                }
                foreach (var monitor in Monitors)
                {
                    monitor.Watcher.FileName = monitor.FileName;
                }
            }
        }
    }
}
