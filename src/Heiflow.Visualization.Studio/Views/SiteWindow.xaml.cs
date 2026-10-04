using Heiflow.Core.Data.ODM;
using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
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
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using System.Windows.Forms;

namespace Heiflow.Visualization.Studio.Views
{
    /// <summary>
    /// SitesWindow.xaml 的交互逻辑
    /// </summary>
    /// 
      [Export(typeof(ISiteView))]
    public partial class SiteWindow : MetroWindow, ISiteView
    {
          private Site _site;
          private Lazy<SiteViewModel> viewModel;
          private DataCube<double> _ts;
        public SiteWindow()
        {
            InitializeComponent();
            CloseAllowed = false;
            viewModel = new Lazy<SiteViewModel>(() => ViewHelper.GetViewModel<SiteViewModel>(this));
            this.Closing += Window_Closing;
            this.Name = DockPanelNames.SitePanel;
        }
        public bool CloseAllowed
        {
            get;
            set;
        }

        public  Site Site
        {
            get
            {
                return _site;
            }
            set
            {
                _site = value;
                tbName.Text = _site.Name;
                tbLon.Text = _site.Longitude.ToString();
                tbLat.Text = _site.Latitude.ToString();
                if(viewModel.Value.LayerService.ODMSource != null)
                {
                    var variables= viewModel.Value.LayerService.ODMSource.GetVariables(_site.ID);
                    if (variables != null && variables.Any())
                    {
                        cmbVariables.ItemsSource = variables;
                        cmbVariables.SelectedIndex = 0;
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
            tbName.Text = "";
            tbLon.Text = "";
            tbLat.Text = "";
            cmbVariables.ItemsSource = null;
        }

        private void cmbVariables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vv = cmbVariables.SelectedItem as Variable;
            if (viewModel.Value.LayerService.ODMSource != null && vv!= null)
            {
                QueryCriteria qc = new QueryCriteria()
                {
                    SiteID = _site.ID,
                    VariableID = vv.ID,
                    AllTime = true
                };
                _ts = viewModel.Value.LayerService.ODMSource.GetTimeSeries(qc);
                viewModel.Value.VGSShellService.SelectPanel(DockPanelNames.WinChartPanel);
                string series = string.Format("{0} at {1}", vv.Name, _site.Name);
                var vec = _ts.GetVector(0, ":", "0");
                _ts.Name = series;
                viewModel.Value.VGSShellService.WinChart.Plot<double>(_ts.DateTimes, vec, series, System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine);
            }
        }

        private void btnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (_ts != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.FileName = _ts.Name;
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    CSVFileStream csv = new CSVFileStream(dlg.FileName);
                    csv.Save(_ts);
                }
            }
        }
    }
}
