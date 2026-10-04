using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
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

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export(typeof(IAboutView))]
    public partial class AboutWindow : IAboutView
    {
        public AboutWindow()
        {
            InitializeComponent();
            this.Name = DockPanelNames.AboutPanel;
            this.Closing += Window_Closing;
            CloseAllowed = false;
        }

        public bool CloseAllowed
        {
            get;
            set;
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


        public void ClearContents()
        {
          
        }
    }
}
