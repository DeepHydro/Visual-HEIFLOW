using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
using System.ComponentModel.Composition;
using System.Windows;

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export(typeof(IProjectExplorerView))]
    public partial class ProjectExplorerView : IProjectExplorerView
    {
        public ProjectExplorerView()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            this.Loaded += ProjectExplorerView_Loaded;
            this.Name = DockPanelNames.ProjectExplorerPanel;
            CloseAllowed = false;
            this.FontSize = 8;
        }
        public bool CloseAllowed
        {
            get;
            set;
        }

        public Heiflow.Presentation.Controls.Project.ProjectExplorerControl ProjectExplorer
        {
            get
            {
                return _ProjectExplorer;
            }
            set
            {
                _ProjectExplorer = value;
            }
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        public void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(CloseAllowed)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void ProjectExplorerView_Loaded(object sender, RoutedEventArgs e)
        {
            this.InvalidateVisual();
        }


        public void ClearContents()
        {
            _ProjectExplorer.ClearContent();
        }
    }
}
