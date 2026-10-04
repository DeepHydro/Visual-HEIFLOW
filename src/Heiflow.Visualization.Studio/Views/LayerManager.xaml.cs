using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
using Heiflow.Visualization.Renderable.Grid;
using HUST.WREIS.Dot3D.Renderable;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// LayerManager.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export(typeof(ILayerManagerView))]
    public partial class LayerManager : UserControl, ILayerManagerView
    {
        private RenderableObject _SelectedLayer;
        private readonly Lazy<LayerManagerViewModel> viewModel;

        public LayerManager()
        {
            InitializeComponent();
            viewModel = new Lazy<LayerManagerViewModel>(() => ViewHelper.GetViewModel<LayerManagerViewModel>(this));
        }

        public void Refresh()
        {
            treeView1.ItemsSource = viewModel.Value.LayerService.Layers.ChildObjects;
            treeView1.Items.Refresh();
        }

        private void LayerTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem tvi = e.OriginalSource as TreeViewItem;
            _SelectedLayer = treeView1.SelectedItem as RenderableObject;
            var menus = treeView1.Resources["SolutionContext"] as System.Windows.Controls.ContextMenu;
            if (_SelectedLayer.DataTable != null)
            {
                var item = (from MenuItem it in menus.Items where it.Name == "AttrTableItem" select it).First();
                item.IsEnabled = true;         
            }
            else
            {
               var item= (from MenuItem it in menus.Items where it.Name =="AttrTableItem" select it).First();
               item.IsEnabled = false;               
            }

            treeView1.ContextMenu = treeView1.Resources["SolutionContext"] as System.Windows.Controls.ContextMenu;
        }

        private void PropertyLayerItem_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Value.ShellService.SelectPanel(DockPanelNames.PropertyPanel);
            viewModel.Value.ShellService.PropertyView.SelectedObject = _SelectedLayer;
        }

        private void RemoveLayerItem_Click(object sender, RoutedEventArgs e)
        {
            if (_SelectedLayer != null)
            {
                viewModel.Value.RemoveCommand.Execute(_SelectedLayer);
            }
        }

        private void AttrTableItem_Click(object sender, RoutedEventArgs e)
        {
            if(_SelectedLayer.DataTable != null )
            {
                viewModel.Value.ShellService.DataGridView.Bind(_SelectedLayer.DataTable);
                viewModel.Value.ShellService.DataGridView.ShowView();
            }
        }

        private void SymbologyItem_Click(object sender, RoutedEventArgs e)
        {
            if (_SelectedLayer != null && _SelectedLayer is IDX3DLayer)
            {
                viewModel.Value.ShellService.SymbologyView.SelectedRender = (_SelectedLayer as IDX3DLayer).RenderDX;
                viewModel.Value.ShellService.SelectPanel(DockPanelNames.SymbologyPanel);
            }
        }
    }
}
