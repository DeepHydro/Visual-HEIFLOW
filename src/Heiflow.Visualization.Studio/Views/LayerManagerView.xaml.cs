using Heiflow.Controls.Tree;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.Generic.Project;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// WPF implementation of the project / layer explorer. It replaces the Windows Forms
    /// <c>ProjectExplorerControl</c>, which had to be hosted in a WindowsFormsHost and therefore never
    /// matched the rest of the shell. The nodes are still produced by the node factory of the model and
    /// the menu handlers are the ones of the model packages, so the behaviour is unchanged.
    /// </summary>
    public partial class LayerManagerView : UserControl, IProjectExplorer
    {
        private readonly ObservableCollection<LayerNode> _Roots = new ObservableCollection<LayerNode>();
        private readonly List<Node> _RootSources = new List<Node>();
        private IProject _currentPrj;

        public LayerManagerView()
        {
            InitializeComponent();
            layerTree.ItemsSource = _Roots;
        }

        public IExplorerNodeFactory NodeFactory
        {
            get;
            set;
        }

        public IPEContextMenuFactory ContextMenuFactory
        {
            get;
            set;
        }

        public void Initialize()
        {
            if (ContextMenuFactory != null && ContextMenuFactory.Menus != null)
            {
                foreach (var menu in ContextMenuFactory.Menus)
                    menu.Initialize();
            }
            if (NodeFactory != null && NodeFactory.Creators != null)
            {
                foreach (var creator in NodeFactory.Creators)
                {
                    creator.ContextMenuFactory = this.ContextMenuFactory;
                    creator.NodeFactory = this.NodeFactory;
                }
            }
        }

        public void AddProject(IProject prj)
        {
            if (prj == null || prj.Model == null)
                return;
            DetachModel();
            ClearContent();
            CreateItems(prj.Model);
            _currentPrj = prj;
            prj.Model.PackageAdded += Model_PackageAdded;
            prj.Model.PackageRemoved += Model_PackageRemoved;
            prj.Model.PackageStatechanged += Model_PackageStatechanged;
            SyncFromSource();
        }

        private void CreateItems(IBasicModel model)
        {
            if (model == null)
                return;

            var atr_model = model.GetType().GetCustomAttributes(typeof(ModelItem), true);
            if (atr_model.Length != 0)
            {
                var atr = atr_model[0] as ModelItem;
                var creator = NodeFactory.Select(atr);
                creator.ContextMenuFactory = this.ContextMenuFactory;
                creator.NodeFactory = this.NodeFactory;
                var root = creator.Creat(model, atr) as Node;
                if (root != null)
                    _RootSources.Add(root);
            }

            var mapdata_item = new MapDataItem();
            var map_creator = NodeFactory.Select(mapdata_item);
            map_creator.ContextMenuFactory = this.ContextMenuFactory;
            map_creator.NodeFactory = this.NodeFactory;
            var root_mapdata = map_creator.Creat(model, mapdata_item) as Node;
            if (root_mapdata != null)
                _RootSources.Add(root_mapdata);
        }

        /// <summary>
        /// Unsubscribes from the model of the previously displayed project so that it can be released.
        /// </summary>
        private void DetachModel()
        {
            if (_currentPrj == null || _currentPrj.Model == null)
                return;
            _currentPrj.Model.PackageAdded -= Model_PackageAdded;
            _currentPrj.Model.PackageRemoved -= Model_PackageRemoved;
            _currentPrj.Model.PackageStatechanged -= Model_PackageStatechanged;
            _currentPrj = null;
        }

        private void Model_PackageStatechanged(object sender, IPackage pck)
        {
            var node = SelectNode(pck.Name);
            if (node == null)
                return;
            // The Windows Forms control swapped the icon of the node for a state icon here. Those icons
            // are internal resources of the Windows Forms assembly, so the node keeps the icon its
            // creator gave it and only the tree is synchronised.
            SyncFromSource();
        }

        private void Model_PackageRemoved(object sender, IPackage pck)
        {
            var node = SelectNode(pck.Name);
            if (node != null && node.Parent != null)
            {
                node.Parent.Nodes.Remove(node);
                SyncFromSource();
            }
        }

        private void Model_PackageAdded(object sender, IPackage pck)
        {
            var node = SelectNode(pck.Owner.Name);
            if (node == null)
                return;
            var atr_pck = pck.GetType().GetCustomAttributes(typeof(PackageItem), true);
            if (atr_pck.Length == 0)
                return;
            var atr = atr_pck[0] as PackageItem;
            var creator = NodeFactory.Select(atr);
            creator.ContextMenuFactory = this.ContextMenuFactory;
            creator.NodeFactory = this.NodeFactory;
            var pck_node = creator.Creat(pck, atr) as Node;
            node.Nodes.Add(pck_node);
            SyncFromSource();
        }

        #region tree synchronisation

        /// <summary>
        /// The node tree produced by the factory is the data source, this method copies it into the
        /// observable tree the WPF control is bound to. It runs after anything that changed the nodes,
        /// including the menu handlers of the packages, which work on the source nodes directly.
        /// </summary>
        private void SyncFromSource()
        {
            SyncCollection(_Roots, _RootSources);
            foreach (var root in _Roots.ToList())
                root.SyncFromSource();
        }

        /// <summary>
        /// Level of the nodes of <paramref name="source"/>, the roots start at one.
        /// </summary>
        internal static void SyncCollection(IList<LayerNode> target, IEnumerable<Node> source, int depth = 1)
        {
            var sources = source as IList<Node> ?? source.ToList();

            for (int i = target.Count - 1; i >= 0; i--)
            {
                if (!sources.Any(n => ReferenceEquals(n, target[i].Source)))
                    target.RemoveAt(i);
            }

            int index = 0;
            foreach (var src in sources)
            {
                var known = target.FirstOrDefault(t => ReferenceEquals(t.Source, src));
                if (known == null)
                {
                    known = new LayerNode(src, depth);
                    target.Insert(Math.Min(index, target.Count), known);
                }
                else
                {
                    known.SyncFromSource();
                }
                index++;
            }
        }

        private Node SelectNode(string name)
        {
            foreach (var root in _RootSources)
            {
                var found = SelectChildNode(root, name);
                if (found != null)
                    return found;
            }
            return null;
        }

        private static Node SelectChildNode(Node parent, string name)
        {
            if (parent == null)
                return null;
            if (parent.Text == name)
                return parent;
            foreach (var child in parent.Nodes)
            {
                var found = SelectChildNode(child, name);
                if (found != null)
                    return found;
            }
            return null;
        }

        #endregion

        #region interaction

        private void LayerTree_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var item = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (item != null)
            {
                item.IsSelected = true;
                item.Focus();
            }

            var menu = BuildContextMenu(layerTree.SelectedItem as LayerNode);
            if (menu != null)
            {
                menu.PlacementTarget = layerTree;
                menu.IsOpen = true;
                e.Handled = true;
            }
        }

        private void LayerTree_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var node = layerTree.SelectedItem as LayerNode;
            if (node == null || node.Source == null)
                return;
            var context = node.Source.Tag as IPEContextMenu;
            if (context == null)
                return;
            context.NodeDoubleClick();
            SyncFromSource();
        }

        /// <summary>
        /// Builds the menu of the selected node. The names and the images come from the model packages,
        /// the handlers are invoked with a Windows Forms menu item as the sender, because that is what
        /// they expect when they read the selected node.
        /// </summary>
        private ContextMenu BuildContextMenu(LayerNode node)
        {
            if (node == null || node.Source == null)
                return null;
            var context = node.Source.Tag as IPEContextMenu;
            if (context == null || context.ContextMenuItems == null)
                return null;

            var menu = new ContextMenu();
            foreach (var ci in context.ContextMenuItems)
            {
                var name = ReadName(ci);
                if (name == Heiflow.Controls.WinForm.MenuItems.PEContextMenu.MenuSeparator)
                {
                    menu.Items.Add(new Separator());
                    continue;
                }
                // the property item is added at the bottom, as the Windows Forms control did
                if (name == PropertyMenuName)
                    continue;
                menu.Items.Add(CreateMenuItem(ci, node));
            }

            var property = context.ContextMenuItems.FirstOrDefault(m => ReadName(m) == PropertyMenuName);
            if (property != null)
                menu.Items.Add(CreateMenuItem(property, node));

            return menu.Items.Count == 0 ? null : menu;
        }

        private const string PropertyMenuName = "Property";

        private MenuItem CreateMenuItem(object item, LayerNode node)
        {
            var name = ReadName(item);
            var mi = new MenuItem();
            mi.Header = name;
            mi.IsEnabled = ReadEnabled(item);

            var image = ReadImage(item);
            if (image != null)
            {
                mi.Icon = new System.Windows.Controls.Image
                {
                    Source = ToImageSource(image),
                    Width = 16,
                    Height = 16
                };
            }

            var node_handle = node.Source;
            if (name == "Expand All")
                mi.Click += (s, e) => SetExpanded(node, true);
            else if (name == "Collapse All")
                mi.Click += (s, e) => SetExpanded(node, false);
            else
                mi.Click += (s, e) => InvokeHandler(ReadHandler(item), node_handle);

            var children = ReadMenuItems(item);
            if (children != null)
            {
                foreach (var child in children)
                {
                    if (ReadName(child) == Heiflow.Controls.WinForm.MenuItems.PEContextMenu.MenuSeparator)
                    {
                        mi.Items.Add(new Separator());
                        continue;
                    }
                    mi.Items.Add(CreateMenuItem(child, node));
                }
            }
            return mi;
        }

        /// <summary>
        /// The handlers of the packages read the selected node from the sender, which is a Windows Forms
        /// menu item in the original control. The source tree is synchronised afterwards, because the
        /// handlers add or remove nodes and change the images of the nodes.
        /// </summary>
        private void InvokeHandler(EventHandler handler, Node node)
        {
            if (handler == null)
                return;
            var senderItem = new System.Windows.Forms.ToolStripMenuItem();
            senderItem.Tag = node;
            handler(senderItem, EventArgs.Empty);
            SyncFromSource();
        }

        private static void SetExpanded(LayerNode node, bool expanded)
        {
            if (node == null)
                return;
            node.IsExpanded = expanded;
            foreach (var child in node.Children)
                SetExpanded(child, expanded);
        }

        private static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = System.Windows.Media.VisualTreeHelper.GetParent(source);
            return source as TreeViewItem;
        }

        #endregion

        #region reflection helpers for the menu items

        // The menu items are declared in a DotSpatial base class which is not referenced here, so their
        // members are read by name.
        private static string ReadName(object item)
        {
            return GetValue(item, "Name") as string;
        }

        private static bool ReadEnabled(object item)
        {
            var value = GetValue(item, "Enabled");
            return value is bool ? (bool)value : true;
        }

        private static System.Drawing.Image ReadImage(object item)
        {
            return GetValue(item, "Image") as System.Drawing.Image;
        }

        private static EventHandler ReadHandler(object item)
        {
            return GetValue(item, "ClickHandler") as EventHandler;
        }

        private static System.Collections.IEnumerable ReadMenuItems(object item)
        {
            return GetValue(item, "MenuItems") as System.Collections.IEnumerable;
        }

        private static object GetValue(object item, string propertyName)
        {
            if (item == null)
                return null;
            var property = item.GetType().GetProperty(propertyName);
            return property != null ? property.GetValue(item, null) : null;
        }

        #endregion

        /// <summary>
        /// Converts the Windows Forms icon of a node into a WPF image source.
        /// </summary>
        internal static ImageSource ToImageSource(System.Drawing.Image image)
        {
            if (image == null)
                return null;
            try
            {
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Position = 0;
                    var bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.StreamSource = stream;
                    bmp.EndInit();
                    bmp.Freeze();
                    return bmp;
                }
            }
            catch
            {
                return null;
            }
        }

        public void ClearContent()
        {
            _RootSources.Clear();
            _Roots.Clear();
        }

        public void Clear()
        {
            DetachModel();
            ClearContent();
        }

        public void InitService()
        {
        }

        public void ShowView(System.Windows.Forms.IWin32Window pararent)
        {
        }

        public string ChildName
        {
            get { return "LayerManagerView"; }
        }
    }

    /// <summary>
    /// One row of the explorer. It mirrors a node of the source tree and notifies the view about the
    /// changes the model applied to it.
    /// </summary>
    public class LayerNode : INotifyPropertyChanged
    {
        private string _Text;
        private ImageSource _Icon;
        private bool _IsExpanded;
        private bool _IsSelected;

        /// <summary>
        /// Number of levels the tree opens when a project is loaded. A row is expanded while its level is
        /// below this one, so three levels are visible and the deeper packages stay collapsed until the
        /// user opens them.
        /// </summary>
        public const int ExpandedLevels = 3;

        public LayerNode(Node source, int depth)
        {
            Source = source;
            Depth = depth;
            _IsExpanded = depth < ExpandedLevels;
            Children = new ObservableCollection<LayerNode>();
            SyncFromSource();
        }

        public Node Source { get; private set; }

        /// <summary>
        /// Level of the row, the roots are level one.
        /// </summary>
        public int Depth { get; private set; }

        public ObservableCollection<LayerNode> Children { get; private set; }

        public string Text
        {
            get { return _Text; }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    RaisePropertyChanged("Text");
                }
            }
        }

        public ImageSource Icon
        {
            get { return _Icon; }
            set
            {
                if (!Equals(_Icon, value))
                {
                    _Icon = value;
                    RaisePropertyChanged("Icon");
                }
            }
        }

        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                if (_IsExpanded != value)
                {
                    _IsExpanded = value;
                    RaisePropertyChanged("IsExpanded");
                }
            }
        }

        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// Copies the text, the icon and the children of the source node into this row.
        /// </summary>
        public void SyncFromSource()
        {
            if (Source == null)
                return;
            Text = Source.Text;
            Icon = LayerManagerView.ToImageSource(Source.Image);
            SyncChildren(Children, Source.Nodes, Depth + 1);
        }

        private static void SyncChildren(IList<LayerNode> target, IEnumerable<Node> source, int depth)
        {
            if (source == null)
                return;
            LayerManagerView.SyncCollection(target, source, depth);
            foreach (var child in target.ToList())
                child.SyncFromSource();
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
