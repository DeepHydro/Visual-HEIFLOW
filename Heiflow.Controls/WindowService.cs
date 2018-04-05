// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Controls.WinForm.Display;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm
{
    [Export(typeof(IWindowService))]
    public class WindowService : IWindowService
    {
        //public Presentation.Controls.IWinChartView NewWinChart()
        //{
        //    return new WinChartView();
        //}


        public Presentation.Controls.ICoverageSetupView NewCoverageSetupWindow()
        {
            return new CoverageSetup();
        }


        public Presentation.Controls.ILookupTableView NewParameterTableWindow()
        {
            return new LookupTableForm();
        }
    }
}
