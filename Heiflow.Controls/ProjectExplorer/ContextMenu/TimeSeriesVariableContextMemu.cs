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

using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Packages;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms.DataVisualization.Charting;

namespace Heiflow.Controls.WinForm.MenuItems
{
     [Export(typeof(IPEContextMenu))]
    public class TimeSeriesVariableContextMemu: DynamicVariableContextMemu
    {
         public TimeSeriesVariableContextMemu()
        {

        }

        public override Type ItemType
        {
            get
            {
                return typeof(TimeSeriesVariableItem);
            }
        }

        //public override void AddMenuItems()
        //{
        //    base.AddMenuItems();
        //    this.EneableAll(false);
        //    this.Eneable(_LD, true);
        //}

        //protected override void AttributeTable_Clicked(object sender, EventArgs e)
        //{
        //    if (CheckDataSource())
        //    {
        //        _ShellService.SelectPanel(DockPanelNames.DataGridPanel);
        //        var dp = _Package as ISitesProvider;
        //        var site=  dp.Sites[VariableIndex] ;
        //        var binding = site.TimeSeries as IDataTableBinding;
        //        var dt = binding.ToPointTimeSeries(0);
        //        _ShellService.DataGridView.Bind(dt, binding);
        //    }
        //}
        //protected override void dp_Loaded(object sender, object e)
        //{
        //    var dp = _Package as IDataPackage;
        //    foreach (var item in _sub_menus)
        //    {
        //        item.Enabled = true;
        //    }
        //    this.Eneable(_SOM, false);
        //    this.Eneable(_AN, false);
        //    this.Eneable(_VI3, false);
        //    dp.Loading -= dp_Loading;
        //    dp.Loaded -= dp_Loaded;
        //    _ShellService.ProgressWindow.DoWork -= ProgressPanel_DoWork;
        //    _Site = e as IObservationsSite;
        //}

        //protected override void ProgressWindow_WorkCompleted(object sender, EventArgs e)
        //{
        //    _ShellService.FigureService.ShowWinChart();
        //    _ShellService.WinChart.Plot<double>(_Site.TimeSeries, SeriesChartType.FastLine);
        //    _ShellService.ProgressWindow.WorkCompleted -= ProgressWindow_WorkCompleted;
        //}

        //protected bool CheckDataSource()
        //{
        //    var dp = _Package as ISitesProvider;
        //    if (dp != null && dp.Sites != null)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        _ShellService.MessageService.ShowWarning(null, "You need to load data at first!");
        //        return false;
        //    }
        //}
    }
}
