using Heiflow.Models.Generic.Project;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// LayerManager.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectExplorer : System.Windows.Controls.UserControl, IProjectExplorer
    {
        private IProject _currentPrj;
        private List<TreeViewItem> _mfDataItems = new List<TreeViewItem>();
        private List<MyNode> _mfDataNodes = new List<MyNode>();
        private Brush _WhiteBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        public ProjectExplorer()
        {
            InitializeComponent();
        }

        public void AddProject(IProject prj)
        {
            //var ihm = prj.Model as IIntegratedModel;
            //treeView1.ItemsSource = CreateNodes(prj.Model as IIntegratedModel);
            prj.ModelChanged += prj_ModelChanged;
            _currentPrj = prj;
        }

        private void prj_ModelChanged(object sender, EventArgs e)
        {
            //var prj = sender as IProject;
            //treeView1.ItemsSource = CreateNodes(prj.Model as IIntegratedModel);
        }

        void pck_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
         
        }
    
        private void LayerTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // The menu used to be built and shown here. Building it on every change of the selection
            // made it appear where nothing had been right clicked, and it was a Windows Forms strip
            // that had to be placed with coordinates read from the mouse. The right click below shows
            // it now, as a WPF menu that places itself.
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }

            // The line above selects what was clicked, so the tree knows it by the time the menu is
            // built. The item is asked for and not the value: no SelectedValuePath is set, so the
            // value stays empty even where the item is not.
            var menu = BuildContextMenu(treeView1.SelectedItem as MyNode);
            if (menu != null)
            {
                menu.PlacementTarget = treeView1;
                menu.IsOpen = true;
                e.Handled = true;
            }
        }

        private const string PropertyMenuName = "Property";

        /// <summary>
        /// Builds the menu of the node that was clicked. The names and the images come from the model
        /// packages, and their handlers are invoked with a Windows Forms menu item as the sender,
        /// because that is what they read the node they were called for from.
        /// </summary>
        private ContextMenu BuildContextMenu(MyNode node)
        {
            if (node == null || node.Tag == null)
                return null;
            var context = node.Tag as IPEContextMenu;
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

        private MenuItem CreateMenuItem(object item, MyNode node)
        {
            var mi = new MenuItem();
            mi.Header = ReadName(item);
            mi.IsEnabled = ReadEnabled(item);

            var image = ReadImage(item);
            if (image != null)
            {
                mi.Icon = new System.Windows.Controls.Image
                {
                    Source = LayerManagerView.ToImageSource(image),
                    Width = 16,
                    Height = 16
                };
            }

            mi.Click += (s, e) => InvokeHandler(ReadHandler(item), node);

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
        /// The handlers read the node they were called for from the sender, which is a Windows Forms
        /// menu item in the original control, so one is handed to them as it always was.
        /// </summary>
        private static void InvokeHandler(EventHandler handler, MyNode node)
        {
            if (handler == null)
                return;
            var senderItem = new System.Windows.Forms.ToolStripMenuItem();
            senderItem.Tag = node;
            handler(senderItem, EventArgs.Empty);
        }

        private TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        #region the members of the menu items are read by name

        // They are declared in a DotSpatial base class that is not referenced here.
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

        public IExplorerNodeFactory NodeFactory
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public IPEContextMenuFactory ContextMenuFactory
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }


        public void Clear()
        {
            throw new NotImplementedException();
        }

        public string ChildName
        {
            get { return "PE3D"; }
        }

        public void ShowView(System.Windows.Forms.IWin32Window pararent)
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
