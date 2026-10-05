using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
using System.ComponentModel.Composition;
using System.Windows;

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// Host window of the project explorer.
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
        }

        public bool CloseAllowed
        {
            get;
            set;
        }

        public IProjectExplorer ProjectExplorer
        {
            get
            {
                return _ProjectExplorer;
            }
            set
            {
                _ProjectExplorer = value as LayerManagerView;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
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

        private void ProjectExplorerView_Loaded(object sender, RoutedEventArgs e)
        {
            this.InvalidateVisual();
        }

        public void ClearContents()
        {
            if (_ProjectExplorer != null)
                _ProjectExplorer.ClearContent();
        }
    }
}
