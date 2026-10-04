using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic;
using Heiflow.Models.UI;
using Heiflow.Visualization.Applications;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
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

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// SeachSitesWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SeachSitesWindow : MetroWindow, IWindow, ISeachSitesView
    {
        private readonly Lazy<SeachSitesViewModel> viewModel;

        public SeachSitesWindow()
        {
            InitializeComponent();
            viewModel = new Lazy<SeachSitesViewModel>(() => ViewHelper.GetViewModel<SeachSitesViewModel>(this));
            this.Loaded += SeachSitesWindow_Loaded;
        }

        public bool CloseAllowed
        {
            get;
            set;
        }

        private void SeachSitesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dateStart.SelectedDate = DateTime.Now.AddYears(-15);
            dateEnd.SelectedDate = DateTime.Now;
            if (viewModel.Value.LayerService != null)
            {
                var keywords = viewModel.Value.LayerService.ODMSource.GetKeyWords();
                tbKeyword.ItemsSource = keywords;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var odm = viewModel.Value.LayerService.ODMSource;
            if (odm != null)
            {
                QueryCriteria qc = new QueryCriteria()
                {
                    BBox = new BBox()
                    {
                        East = double.Parse(tbElng.Text),
                        North = double.Parse(tbNLat.Text),
                        South = double.Parse(tbSLat.Text),
                        West = double.Parse(tbWLng.Text)
                    },
                    End = dateEnd.SelectedDate.Value,
                    Start = dateStart.SelectedDate.Value,
                    VariableName = tbKeyword.Text
                };
                var sites = odm.GetSites(qc);
                viewModel.Value.LayerService.SitesLayer.ShowSites(sites);
            }
        }


        public void ClearContents()
        {
            
        }
    }
}
