// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using Heiflow.Models.UI;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Presentation.Services
{
    public interface IShellService : INotifyPropertyChanged
    {
        object ShellView { get; set; }
        List<IChildView> ChildViews { get; }
        IAnimationView AnimationPlayer { get; set; }
        IDataGridView DataGridView { get; set; }
        IModelSymbologyControl SymbologyControl { get; set; }
        IProgressView ProgressWindow { get; set; }
        IProjectExplorer ProjectExplorer { get; set; }
        IPropertyView PropertyView { get; set; } 
        ISurfacePlotView SurfacePlot { get; set; }     
        INewProject NewProjectWindow { get; set; }
        IModelToolManager PackageToolManager { get; set; }
        IWin32Window MainForm { get; set; }
        IWinChartView WinChart { get; set; }
        IFigureService FigureService { get; set; }
        IMessageService MessageService { get; set; }
        AppManager MapAppManager { get; set; }
        IDataCubeEditor TV3DMatEditor { get; set; }
        bool ShowAnimationMonitor { get; set; }
        void SelectPanel(string key);
        void ClearContents();
        IChildView SelectChild(string child_name);
        void ShowChildWindow(string child_name);
        void AddChild(IChildView child);
        void AddNativeChildren();
        void InitChildViewService();
    }
}
