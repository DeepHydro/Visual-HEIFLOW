using Heiflow.Core;
using Heiflow.Core.Plugin;
using Heiflow.Visualization.Applications;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Display;
using HUST.WREIS.Dot3D.Display.Plugins;
using HUST.WREIS.Dot3D.Renderable;

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Waf.Applications;
using System.Windows;


using SharpDX;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

namespace Heiflow.Visualization.Studio.Views
{
    /// <summary>
    /// VirtrualGlobeView.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export(typeof(IVirtualGlobeView))]
    public partial class VirtrualGlobeView : System.Windows.Controls.UserControl, IVirtualGlobeView
    {
        private readonly Lazy<VirtualGlobeViewModel> viewModel;

        public VirtrualGlobeView()
        {
            InitializeComponent();
            this.Loaded += VirtrualGlobeView_Loaded; 
             viewModel = new Lazy<VirtualGlobeViewModel>(() => ViewHelper.GetViewModel<VirtualGlobeViewModel>(this));
        }

        public HUST.WREIS.Dot3D.SceneWindow VirtualGlobe
        {
            get 
            { 
                return this.sceneWindow; 
            }
        }

        private void VirtrualGlobeView_Loaded(object sender, RoutedEventArgs e)
        {        
            var settings = ConfigurationManager.Engine3DSettings;
            long CacheUpperLimit = (long)settings.CacheSizeMegaBytes * 1024L * 1024L;
            long CacheLowerLimit = (long)settings.CacheSizeMegaBytes * 768L * 1024L;	//75% of upper limit
            sceneWindow.Cache = new Cache(settings.CachePath, CacheLowerLimit, CacheUpperLimit, settings.CacheCleanupInterval, settings.TotalRunTime);
            viewModel.Value.OnViewLoaded();
        }

        protected override  void OnGotFocus(RoutedEventArgs e)
        {
            if (sceneWindow != null)
                sceneWindow.Focus();
            base.OnGotFocus(e);
        }

    }
}
