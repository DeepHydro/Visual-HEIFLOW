using Heiflow.Core;
using Heiflow.Models.IO;
using Heiflow.Visualization.Applications;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// Activication.xaml 的交互逻辑
    /// </summary>
    public partial class Activication : UserControl
    {
        public event EventHandler Activated;
       
        public Activication()
        {
            InitializeComponent();
        }

        private void btnActivicate_Click(object sender, RoutedEventArgs e)
        {
            var file = System.IO.Path.Combine(VGSManager.Instance.ApplicationPath, "vgs.dll");
            SecurityFile _SecurityFile = new SecurityFile(file);

            if (File.Exists(file))
            {
                string key = string.Format("{0}-{1}-{2}-{3}", tb1.Text, tb2.Text, tb3.Text, tb4.Text);
                string code = _SecurityFile.Convert(key);
                _SecurityFile.Read();
                if (code == _SecurityFile.AuthenticationCode)
                {
                    _SecurityFile.Date = DateTime.Now;
                    _SecurityFile.Authenticated = true;
                    _SecurityFile.Update("HydroEarth");
                    if (Activated != null)
                    {
                        Activated(this, EventArgs.Empty);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid license key!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Some necessary files are missing!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
