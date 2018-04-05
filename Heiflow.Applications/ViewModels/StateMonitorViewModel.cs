// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
