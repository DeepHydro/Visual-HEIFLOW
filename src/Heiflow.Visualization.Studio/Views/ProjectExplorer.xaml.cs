using Heiflow.Models.Generic.Project;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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
        private ContextMenuStrip _contextMenu;
        private Point itemPostion;
        System.Drawing.Point _Point;

        public ProjectExplorer()
        {
            InitializeComponent();
            _contextMenu = new ContextMenuStrip();
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
            var node = treeView1.SelectedValue as MyNode;
 
            if(node != null && node.Tag != null)
            {
                _contextMenu.Items.Clear();
                var item = node.Tag as IPEContextMenu;
                if (item != null)
                {
                    foreach (var ci in item.ContextMenuItems)
                    {
                        ToolStripMenuItem drop=null;
                        if(ci.ClickHandler != null)
                             drop = _contextMenu.Items.Add(ci.Name, ci.Image, ci.ClickHandler) as ToolStripMenuItem;
                        else
                            drop = _contextMenu.Items.Add(ci.Name, ci.Image) as ToolStripMenuItem;
                        foreach (var sub in ci.MenuItems)
                        {
                            if(sub.ClickHandler != null)
                                drop.DropDown.Items.Add(sub.Name, sub.Image, sub.ClickHandler);
                            else
                                drop.DropDown.Items.Add(sub.Name, sub.Image);
                        }
                        _contextMenu.Items.Add(drop);
                    }                 
                    // Find the appropriate ContextMenu depending on the selected node.
                    _contextMenu.Show(_Point);
                }
            }
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            itemPostion = e.GetPosition(treeView1);
            _Point = new System.Drawing.Point((int)itemPostion.X, (int)itemPostion.Y);
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        private TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

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

        public void ShowView(IWin32Window pararent)
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
