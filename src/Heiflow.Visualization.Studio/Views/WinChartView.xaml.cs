using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// WinChartView.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export(typeof(IWPFWinChartView))]
    public partial class WinChartView : MetroWindow,IWPFWinChartView
    {
        public WinChartView()
        {
            InitializeComponent();
            this.Name = DockPanelNames.WinChartPanel;
            this.Closing += Window_Closing;
            CloseAllowed = false;
        }
        public bool CloseAllowed
        {
            get;
            set;
        }
        public bool IsDisposed
        {
            get { return false; }
        }
        public string ChildName
        {
            get { return "WinChartView"; }
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
        public void Plot<T>(DateTime[] xx, T[] yy, string series_name, System.Windows.Forms.DataVisualization.Charting.SeriesChartType chartType)
        {
            chart.Plot<T>(xx, yy, series_name, chartType);
        }

        public void Plot<T>(T[] yy, string series_name, System.Windows.Forms.DataVisualization.Charting.SeriesChartType chartType)
        {
            chart.Plot<T>( yy, series_name, chartType);
        }

        //public void Plot<T>(TimeSeries<T> source, System.Windows.Forms.DataVisualization.Charting.SeriesChartType chartType)
        //{
        //    chart.Plot<T>(source, chartType);
        //}

        public void ShowView(System.Windows.Forms.IWin32Window parent)
        {
            this.Show();
        }

        public void CloseView()
        {
            this.Hide();
        }

        public void ClearContents()
        {
            chart.Clear();
        }



        public void Plot<T>(Core.Data.DataCube<T> source, System.Windows.Forms.DataVisualization.Charting.SeriesChartType chartType)
        {
            throw new NotImplementedException();
        }


        public void ClearContent()
        {
          
        }

        public void InitService()
        {
        
        }
    }
}
