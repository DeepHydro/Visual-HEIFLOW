// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Running
{
      //[Export(typeof(IFileMonitor))]
    public  class IrrigationMonitor : FileMonitor
    {
        public IrrigationMonitor()
        {
            MonitorName = "IrrigationMonitor";
            var root = new MonitorItemCollection("Irrigation Water Budgets");
            _Roots.Add(root);

            MonitorItem div = new MonitorItem("Diversion")
            {
                VariableIndex = 0,
                Group = _In_Group,
                 SequenceType= SequenceType.StepbyStep
            };

            MonitorItem pump = new MonitorItem(IR_PUMP)
            {
                VariableIndex = 1,
                Group = _In_Group,
                SequenceType = SequenceType.StepbyStep
            };

            MonitorItem industry = new MonitorItem("Industry")
            {
                VariableIndex = 2,
                Group = _In_Group,
                SequenceType = SequenceType.StepbyStep
            };

            MonitorItem canal_inflow = new MonitorItem("Canal Inflow")
            {
                VariableIndex = 6,
                Group = _In_Group,
                SequenceType = SequenceType.StepbyStep
            };
            MonitorItem canal_drain = new MonitorItem("Canal Drain")
            {
                VariableIndex = 7,
                Group = _Out_Group,
                SequenceType = SequenceType.Accumulative
            };
            MonitorItem canal_et = new MonitorItem("Canal ET")
            {
                VariableIndex = 8,
                Group = _Out_Group,
                SequenceType = SequenceType.StepbyStep
            };
            MonitorItem canal_stor = new MonitorItem("Canal Storage")
            {
                VariableIndex = 9,
                Group = _Ds_Group,
                SequenceType = SequenceType.StepbyStep
            };
            root.Children.Add(div);
            root.Children.Add(pump);
            root.Children.Add(industry);
            root.Children.Add(canal_inflow);
            root.Children.Add(canal_drain);
            root.Children.Add(canal_et);
            root.Children.Add(canal_stor);

            foreach (var item in root.Children)
            {
                item.Monitor = this;
            }
            this.Watcher = new TxtWatcher();
        }
    }
}
