using Heiflow.Applications.Services;
using Heiflow.Models.UI;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Display;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Heiflow.Visualization.Applications
{
    [Export(typeof(IVGSShellService)), Export(typeof(IShellService))]
    public class VGSShellService : ShellService, IVGSShellService
    {
        protected ISymbologyView _SymbologyView;
        protected IRunModelView _RunModelView;
        protected List<IChildWPFWindow> _ChildWindows;
        protected ISiteView _SiteView;
        public VGSShellService()
        {
            _ChildWindows = new List<IChildWPFWindow>();
        }

        [Import(typeof(IAboutView))]
        public IAboutView AboutView
        {
            get;
            set;
        }
        [Import(typeof(IProjectExplorerView))]
        public IProjectExplorerView ProjectExplorerView
        {
            get;
            set;
        }
        [Import(typeof(IDataGridView))]
        public new IDataGridView DataGridView
        {
            get { return _DataGridPanel; }
            set { SetProperty(ref _DataGridPanel, value); }
        }
        [Import(typeof(IWPFWinChartView))]
        public new IWPFWinChartView WinChart
        {
            get { return _WinChart as IWPFWinChartView; }
            set { SetProperty(ref _WinChart, value); }
        }
        [Import(typeof(IWPFAnimationView))]
        public new IWPFAnimationView AnimationPlayer
        {
            get { return _AnimationPanel as IWPFAnimationView; }
            set { SetProperty(ref _AnimationPanel, value); }
        }
        [Import(typeof(ISymbologyView))] 
        public ISymbologyView SymbologyView
        {
            get { return _SymbologyView; }
            set { SetProperty(ref _SymbologyView, value); }
        }
        [Import(typeof(IRunModelView))]
        public IRunModelView RunModelView
        {
            get { return _RunModelView; }
            set { SetProperty(ref _RunModelView, value); }
        }
          [Import(typeof(IWPFPropertyView))] 
        public new IWPFPropertyView PropertyView
        {
            get { return _IPropertyGrid as IWPFPropertyView; }
            set { SetProperty(ref _IPropertyGrid, value); }
        }
          [Import(typeof(ISiteView))]
          public ISiteView SiteView
          {
              get { return _SiteView; }
              set { SetProperty(ref _SiteView, value); }
          }
        public GridLengendBar Legend
        {
            get;
            set;
        }
        [Import(typeof(IVirtualGlobeView))] 
        public IVirtualGlobeView VirtualGlobeView
        {
            get;
            set;
        }

        public object LayerManager
        {
            get;
            set;
        }
        public DispatcherTimer Timer
        {
            get;
            set;
        }
        public List<IChildWPFWindow> ChildWPFWindows
        {
            get
            {
                return _ChildWindows;
            }
        }

        public void Initialize()
        {
            _ChildWindows.Add(DataGridView as IChildWPFWindow);
            _ChildWindows.Add(AboutView);
            _ChildWindows.Add(ProjectExplorerView);
            _ChildWindows.Add(AnimationPlayer);
            _ChildWindows.Add(SymbologyView);
            _ChildWindows.Add(PropertyView);
            _ChildWindows.Add(WinChart);
            _ChildWindows.Add(SiteView);
            _ChildWindows.Add(RunModelView);
            ShowAnimationMonitor = false;
        }

        public override void SelectPanel(string key)
        {
            var buf = from vv in _ChildWindows where vv.Name == key select vv;
            if (buf.Any())
            {
                buf.First().Show();
            }
        }

        public override void ClearContents()
        {
             foreach(var ch in _ChildWindows)
             {
                 ch.ClearContents();
             }
        }
    }
}
