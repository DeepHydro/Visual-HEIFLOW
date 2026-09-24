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

using Heiflow.Models.Generic;
using Heiflow.Models.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Running
{
    [Export(typeof(IFileMonitor))]
    public class MFMonitor : FileMonitor
    {
        public MFMonitor()
        {
            MonitorName = "MFMonitor";
        
            this.Watcher = new MFListWatcher(this);
        }

        public override Dictionary<string, double> ZonalBudgets()
        {
            return null;
        }

        public override System.Data.DataTable Balance(string itemname, ref string report)
        {
            if (DataSource != null)
            {
                ClampSteps(DataSource.Values[0].Count);

                string ds_term = "";
                var rootitem = (from item in _Roots where item.Name == itemname select item).First();
                if (itemname == "Lake Water Budgets")
                {
                    ds_term = LAK_Storage_Change;
                }
                else if (itemname == "Saturated Zone Water Budgets")
                    ds_term = SAT_DS;

                var dt = CreateBudgetTable();
                double nsteps = EndStep - StartStep + 1;
                double factor = Intevals / nsteps;
                double total_in = 0;
                double total_out = 0;
                double total_ds = 0;
                double total_diff = 0;
                double total_error = 0;
                var total_discrepancy = 0.0;

                var lines = new List<BudgetLine>();

                var items = (from item in rootitem.Children where item.Group == _In_Group select item).ToArray();
                lines.Add(BudgetLine.Section("IN TERMS"));
                total_in = AddTermRows(items, dt, 100, factor, lines);

                items = (from item in rootitem.Children where item.Group == _Out_Group select item).ToArray();
                lines.Add(BudgetLine.Section("OUT TERMS"));
                total_out = AddTermRows(items, dt, 200, factor, lines);

                items = (from item in rootitem.Children where item.Name == ds_term select item).ToArray();
                lines.Add(BudgetLine.Section("STORAGE CHANGE TERMS"));
                total_ds = AddTermRows(items, dt, 300, factor, lines);

                if (itemname == "Lake Water Budgets")
                {
                    total_diff = total_in - total_out;
                    total_error = total_diff - total_ds;
                    total_discrepancy = PercentDiscrepancy(total_in, total_out, total_ds);
                }
                else
                {
                    total_diff = total_in - total_out;
                    total_error = total_diff;
                    total_discrepancy = PercentDiscrepancyModflow(total_in, total_out);
                }

                AddSummaryRows(dt, lines, total_in, total_out, total_ds, total_diff, total_error, total_discrepancy);

                report = BuildReport(itemname, lines);

                return dt;
            }
            else
            {
                return null;
            }
        }

    }
}
