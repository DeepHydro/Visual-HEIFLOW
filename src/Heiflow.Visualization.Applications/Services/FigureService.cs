// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Applications
{
    [Export(typeof(IFigureService))]
    public class FigureService:IFigureService
    {
        private IShellService _Shell;
        private IWindowService _WindowSerivce;

        [ImportingConstructor]
        public FigureService(IWindowService window)
        {
            _WindowSerivce = window;
        }

        public IShellService ShellService
        {
            get
            {
                return _Shell;
            }
            set
            {
                _Shell = value;
            }
        }

        public void ShowWinChart()
        {
            //if (_Shell.WinChart.IsDisposed)
            //{
            //    _Shell.WinChart = _WindowSerivce.NewWinChart();
            //}
            _Shell.WinChart.ShowView(_Shell.MainForm);
        }
    }
}
