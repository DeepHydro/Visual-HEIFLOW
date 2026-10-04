using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
using Heiflow.Visualization.Renderable.Grid;
using HUST.WREIS.Dot3D.Renderable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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
        private readonly ObservableCollection<LayerItem> _Roots = new ObservableCollection<LayerItem>();
        private readonly DispatcherTimer _SyncTimer;

        public LayerManager()
        {
            InitializeComponent();
            viewModel = new Lazy<LayerManagerViewModel>(() => ViewHelper.GetViewModel<LayerManagerViewModel>(this));
            // The tree is bound to the snapshot below and never to the lists of the 3D engine: those are
            // plain lists and they are changed while the scene is rendered, which makes the item
            // generator of the tree throw "items control is inconsistent with its items source".
            treeView1.ItemsSource = _Roots;
            _SyncTimer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _SyncTimer.Tick += SyncTimer_Tick;
            this.Loaded += LayerManager_Loaded;
            this.Unloaded += LayerManager_Unloaded;
        }

        private void LayerManager_Loaded(object sender, RoutedEventArgs e)
        {
            _SyncTimer.Start();
            SyncTree();
        }

        private void LayerManager_Unloaded(object sender, RoutedEventArgs e)
        {
            _SyncTimer.Stop();
        }

        private void SyncTimer_Tick(object sender, EventArgs e)
        {
            SyncTree();
        }

        public void Refresh()
        {
            SyncTree();
        }

        /// <summary>
        /// Copies the renderable objects of the scene into the tree that is displayed. Only the snapshot
        /// collections are changed and always on the dispatcher thread, so the item generator of the tree
        /// stays consistent.
        /// </summary>
        private void SyncTree()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(SyncTree), DispatcherPriority.Background);
                return;
            }

            LayerManagerViewModel vm;
            try
            {
                vm = viewModel.Value;
            }
            catch
            {
                // the view model is not composed yet, the next tick tries again
                return;
            }
            if (vm == null)
                return;

            var layers = vm.LayerService.Layers ?? vm.LayerService.RootLayers;
            if (layers == null)
                return;

            var snapshot = TryCopy(layers.ChildObjects);
            if (snapshot == null)
                return;
            SyncCollection(_Roots, snapshot);
            foreach (var item in _Roots.ToList())
                SyncItem(item, 1);
        }

        /// <summary>
        /// The scene can carry very deep sub trees, for example the tiles of an image layer. Those are not
        /// part of the layer hierarchy the user works with, so the copy stops after a few levels.
        /// </summary>
        private const int MaxSyncDepth = 8;

        private static void SyncItem(LayerItem item, int depth)
        {
            if (item == null || item.Source == null || depth > MaxSyncDepth)
                return;
            var snapshot = TryCopy(item.Source.ChildObjects);
            if (snapshot == null)
                return;
            SyncCollection(item.Children, snapshot);
            foreach (var child in item.Children.ToList())
                SyncItem(child, depth + 1);
        }

        private static void SyncCollection(IList<LayerItem> target, IEnumerable<RenderableObject> source)
        {
            var sources = source as IList<RenderableObject> ?? source.ToList();

            for (int i = target.Count - 1; i >= 0; i--)
            {
                if (!sources.Any(r => ReferenceEquals(r, target[i].Source)))
                    target.RemoveAt(i);
            }

            int index = 0;
            foreach (var src in sources)
            {
                var known = target.FirstOrDefault(t => ReferenceEquals(t.Source, src));
                if (known == null)
                {
                    known = new LayerItem(src);
                    target.Insert(Math.Min(index, target.Count), known);
                }
                index++;
            }
        }

        /// <summary>
        /// Copies a list of the 3D engine. The engine may be rendering while it is copied, in which case
        /// the enumeration throws; the copy is retried and simply skipped if it keeps failing, the next
        /// tick of the timer tries again.
        /// </summary>
        private static List<RenderableObject> TryCopy(IEnumerable<RenderableObject> source)
        {
            if (source == null)
                return null;
            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    return new List<RenderableObject>(source);
                }
                catch (InvalidOperationException)
                {
                    Thread.Sleep(10);
                }
            }
            return null;
        }

        private void LayerTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = treeView1.SelectedItem as LayerItem;
            _SelectedLayer = item != null ? item.Source : null;
            // the attribute table is only available for layers that carry a data table
            var menu = treeView1.ContextMenu;
            if (menu != null)
            {
                var attrItem = menu.Items.OfType<MenuItem>().FirstOrDefault(m => m.Name == "AttrTableItem");
                if (attrItem != null)
                    attrItem.IsEnabled = _SelectedLayer != null && _SelectedLayer.DataTable != null;
            }
        }

        /// <summary>
        /// Selects the node under the mouse before the menu opens, otherwise the menu would act on the
        /// node that was selected before.
        /// </summary>
        private void LayerTree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (item != null)
            {
                item.IsSelected = true;
                item.Focus();
            }
        }

        private static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = System.Windows.Media.VisualTreeHelper.GetParent(source);
            return source as TreeViewItem;
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
                SyncTree();
            }
        }

        private void AttrTableItem_Click(object sender, RoutedEventArgs e)
        {
            if (_SelectedLayer != null && _SelectedLayer.DataTable != null)
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

    /// <summary>
    /// One row of the layer tree. It wraps a renderable object instead of exposing the object itself, so
    /// that the tree is bound to collections of this control and not to the lists of the 3D engine.
    /// </summary>
    public class LayerItem : INotifyPropertyChanged
    {
        private readonly RenderableObject _Source;

        public LayerItem(RenderableObject source)
        {
            _Source = source;
            Children = new ObservableCollection<LayerItem>();
        }

        public RenderableObject Source
        {
            get { return _Source; }
        }

        public ObservableCollection<LayerItem> Children { get; private set; }

        public string Name
        {
            get { return _Source != null ? _Source.Name : null; }
        }

        public string Description
        {
            get { return _Source != null ? _Source.Description : null; }
        }

        public bool IsOn
        {
            get { return _Source != null && _Source.IsOn; }
            set
            {
                if (_Source == null || _Source.IsOn == value)
                    return;
                _Source.IsOn = value;
                RaisePropertyChanged("IsOn");
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
