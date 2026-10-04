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
    /// PropertyWindow.xaml 的交互逻辑
    /// </summary>
    /// 
      [Export(typeof(IWPFPropertyView))]
    public partial class PropertyWindow : MetroWindow,IWPFPropertyView
    {
        public PropertyWindow()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            this.Name = DockPanelNames.PropertyPanel;
            CloseAllowed = false;
        }

        public object SelectedObject
        {
            get
            {
                return propertyGrid.SelectedObject;
            }
            set
            {
                propertyGrid.SelectedObject = value;
            }
        }

        public bool CloseAllowed
        {
            get;
            set;
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



        public void ClearContents()
        {
            SelectedObject = null;
        }
    }
}
