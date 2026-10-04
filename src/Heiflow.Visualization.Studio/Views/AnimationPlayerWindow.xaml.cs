using Heiflow.Applications;
using Heiflow.Models.Generic;
using Heiflow.Models.GHM;
using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
using Heiflow.Visualization.Renderable.Grid;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Heiflow.Visualization.Studio.Views
{
    /// <summary>
    /// AnimationPlayerWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export(typeof(IWPFAnimationView))]
    public partial class AnimationPlayerWindow : MetroWindow, IWPFAnimationView
    {
        private readonly Lazy<AnimationPlayerViewModel> viewModel;
        public AnimationPlayerWindow()
        {
            InitializeComponent();
            viewModel = new Lazy<AnimationPlayerViewModel>(() => ViewHelper.GetViewModel<AnimationPlayerViewModel>(this));
            animation_player.DataCubeWorkspace.DataSourceCollectionChanged += DataCubeWorkspace_DataSourceCollectionChanged;
            CloseAllowed = false;
            this.Name = DockPanelNames.AnimationPlayerPanel;
            this.Closing += Window_Closing;
            this.Loaded += AnimationPlayerWindow_Loaded;
        }

        public bool CloseAllowed
        {
            get;
            set;
        }

        public Models.Tools.IDataCubeWorkspace DataCubeWorkspace
        {
            get 
            {
                return animation_player.DataCubeWorkspace;
            }
        }

        private void AnimationPlayerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            animation_player.Animator.CurrentChanged += viewModel.Value.ShellService.VirtualGlobeView.VirtualGlobe.Animator_CurrentChanged;
        }
        private void DataCubeWorkspace_DataSourceCollectionChanged(object sender, EventArgs e)
        {
            if (DataCubeWorkspace.DataSources.Any())
            {
                var mat = DataCubeWorkspace.DataSources.Last();
                var render = (mat.DataOwner as IPackage).Layer3D.RenderObject;
                viewModel.Value.ShellService.Legend.RenderObject = render as IDX3DLayerRender;
            }
        }

        public void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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

        public void ClearContents()
        {
            DataCubeWorkspace.Clear();
        }
    }
}
