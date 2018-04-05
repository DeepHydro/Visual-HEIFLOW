// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
