using Heiflow.Core.Data;
using Heiflow.Core.Drawing;
using Heiflow.Core.Hydrology;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using Heiflow.Visualization.Applications;
using HUST.WREIS.Dot3D;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using ZedGraph;

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// SFRWindow.xaml 的交互逻辑
    /// </summary>
    /// 
     [Export(typeof(ISFRView))]
    public partial class SFRWindow : IPackageOptionalView, ISFRView
    {
        private readonly Lazy<SFRViewModel> viewModel;
 

        public SFRWindow()
        {
            InitializeComponent();
            CloseAllowed = false;
            this.Loaded += SFRWindow_Loaded;
            this.Closing += this.SFRWindow_Closing;
            viewModel = new Lazy<SFRViewModel>(() => ViewHelper.GetViewModel<SFRViewModel>(this));
        }

        public bool CloseRequired
        {
            get;
            set;
        }

        public string PackageName
        {
            get { return SFROutputPackage.PackageName; }
        }

        public IPackage Package
        {
            get
            {
                return sfrExplorer.SFROutput;
            }
            set
            {
                sfrExplorer.SFROutput = value as SFROutputPackage;
            }
        }

        public bool CloseAllowed
        {
            get;
            set;
        }
        public string ChildName
        {
            get { return "SFRWindow"; }
        }
        private void SFRWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sfrExplorer.ODM = viewModel.Value.LayerService.ODMSource;
        }
        private void SFRWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CloseAllowed)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                this.Hide();
            }
        }
        public void ShowView(System.Windows.Forms.IWin32Window pararent)
        {
            this.Show();
        }

        public void ClearContents()
        {
             
        }

        public void ClearContent()
        {
        
        }

        public void InitService()
        {
            
        }
    }
}
