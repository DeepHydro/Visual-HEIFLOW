using Heiflow.Models.UI;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Applications.Services
{
    [Export(typeof(IShellService)), Export]
    public class ShellService : System.Waf.Foundation.Model, IShellService
    {
        protected object shellView;
        protected IProgressView _IProgressPanel;
        protected IProjectExplorer _IProjectExplorer;
        protected IPropertyView _IPropertyGrid;
        protected IAnimationView _AnimationPanel;
        protected ISurfacePlotView _SurfacePlot;
        protected IModelSymbologyControl _ModelSymbologyControl;
        protected INewProject _NewProjectWindow;
        protected IDataGridView _DataGridPanel;
        protected IWinChartView _WinChart;
        protected IFigureService _FigureService;
        protected IModelToolManager _PackageToolManager;
        protected IDataCubeEditor _TV3DMatEditor;
        protected List<IChildView> _ChildViews = new List<IChildView>();
        public object ShellView
        {
            get { return shellView; }
            set { SetProperty(ref shellView, value); }
        }
        public List<IChildView> ChildViews
        {
            get
            {
                return _ChildViews;
            }
        }
        public IWin32Window MainForm
        {
            get;
            set;
        }
        public IProgressView ProgressWindow
        {
            get { return _IProgressPanel; }
            set { SetProperty(ref _IProgressPanel, value); }
        }
        public IPropertyView PropertyView
        {
            get { return _IPropertyGrid; }
            set { SetProperty(ref _IPropertyGrid, value); }
        }

        public IAnimationView AnimationPlayer
        {
            get { return _AnimationPanel; }
            set { SetProperty(ref _AnimationPanel, value); }
        }

        public ISurfacePlotView SurfacePlot
        {
            get { return _SurfacePlot; }
            set { SetProperty(ref _SurfacePlot, value); }
        }

        public IModelSymbologyControl SymbologyControl
        {
            get { return _ModelSymbologyControl; }
            set { SetProperty(ref _ModelSymbologyControl, value); }
        }

        public IDataGridView DataGridView
        {
            get { return _DataGridPanel; }
            set { SetProperty(ref _DataGridPanel, value); }
        }

        public IWinChartView WinChart
        {
            get { return _WinChart; }
            set { SetProperty(ref _WinChart, value); }
        }
        public IModelToolManager PackageToolManager
        {
            get { return _PackageToolManager; }
            set { SetProperty(ref _PackageToolManager, value); }
        }

        public IDataCubeEditor TV3DMatEditor
        {
            get { return _TV3DMatEditor; }
            set { SetProperty(ref _TV3DMatEditor, value); }
        }

        [Import(typeof(IProjectExplorer))]
        public IProjectExplorer ProjectExplorer
        {
            get { return _IProjectExplorer; }
            set
            {
                SetProperty(ref _IProjectExplorer, value);
            }
        }

        [Import(typeof(INewProject))]
        public INewProject NewProjectWindow
        {
            get { return _NewProjectWindow; }
            set { SetProperty(ref _NewProjectWindow, value); }
        }


        [Import(typeof(IFigureService))]
        public IFigureService FigureService
        {
            get { return _FigureService; }
            set
            {
                SetProperty(ref _FigureService, value);
                _FigureService.ShellService = this;
            }
        }
        [Import(typeof(IMessageService))]
        public IMessageService MessageService
        {
            get;
            set;
        }

        public DotSpatial.Controls.AppManager MapAppManager
        {
            get;
            set;
        }
        public bool ShowAnimationMonitor
        {
            get;
            set;
        }
        public virtual void SelectPanel(string key)
        {
            if(MyAppManager.Instance.AppMode == AppMode.VHF)
                MapAppManager.DockManager.SelectPanel(key);
        }

        public virtual void ShowChildWindow(string childname)
        {
            var view = SelectChild(childname);
            view.ShowView(this.MainForm);
        }

        public virtual void ClearContents()
        {
            MapAppManager.Map.ClearLayers();
            foreach(var ch in ChildViews)
            {
                ch.ClearContent();
            }
        }

        public IChildView SelectChild(string child_name)
        {
            var child = from cc in ChildViews where cc.ChildName == child_name select cc;
            if (child.Any())
                return child.First();
            else
                return null;
        }

        public void AddChild(Models.UI.IChildView child)
        {
            if (!ChildViews.Contains(child))
                ChildViews.Add(child);
        }

        public void AddNativeChildren()
        {
            AddChild(this._IProjectExplorer);
        }

        public void InitChildViewService()
        {
            foreach (var ch in ChildViews)
                ch.InitService();
        }
    }
}
