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
