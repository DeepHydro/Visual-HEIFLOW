using Heiflow.Controls;
using Heiflow.Core.Data;
using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
using ILNumerics;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
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

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export(typeof(IDataGridView))]
    public partial class DataGridWindow : MetroWindow, IDataGridView, IChildWPFWindow
    {
        public DataGridWindow()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            CloseAllowed = false;
            this.Name = DockPanelNames.DataGridPanel;
            this.Loaded += DataGridWindow_Loaded;
        }

        public bool CloseAllowed
        {
            get;
            set;
        }

        public string DataObjectName
        {
            get
            {
                return datagrid.DataObjectName;
            }
            set
            {
                datagrid.DataObjectName = value;
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
        public void Bind<T>(ILArray<T> data)
        {
            datagrid.Bind(data);
        }

        public void Bind<T>(T[] array)
        {
            datagrid.Bind(array);
        }

        public void Bind<T>(T[][] array)
        {
            datagrid.Bind(array);
        }

        public void Bind(DataTable table)
        {
            datagrid.Bind(table);
        }

        public void Bind(IDataCubeObject dc)
        {
            datagrid.Bind(dc);
        }

        public void Bind(Models.Generic.IParameter[] paras)
        {
            datagrid.Bind(paras);
        }

        public void ShowView()
        {
            this.Show();
        }

        public void ClearContents()
        {
            Bind(new DataTable());
        }

        public void DataGridWindow_Loaded(object sender, RoutedEventArgs e)
        {
            datagrid.InitService();
        }
        public void Clear()
        {
           
        }
    }
}
