using HUST.WREIS.Dot3D;
using System;
using System.Collections.Generic;
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
    /// SceneSetting.xaml 的交互逻辑
    /// </summary>
    public partial class SceneSetting : UserControl
    {
        public SceneSetting()
        {
            InitializeComponent();
            this.Loaded += SceneSetting_Loaded;
        }

        private void SceneSetting_Loaded(object sender, RoutedEventArgs e)
        {
            var index = (int)World.Settings.VerticalExaggeration;
            if (index > 0 && index < cobVerticalEx.Items.Count)
            {
                cobVerticalEx.SelectionChanged -= cobVerticalEx_SelectionChanged;
                cobVerticalEx.SelectedIndex= index-1;
                cobVerticalEx.SelectionChanged += cobVerticalEx_SelectionChanged;
            }
        }

        private void cobVerticalEx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ve = float.Parse((cobVerticalEx.SelectedItem as ComboBoxItem).Content.ToString());
            World.Settings.VerticalExaggeration = ve;
        }
        private void cobFogEffect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int fogDegree = 60;
            var item = cobFogEffect.SelectedItem as ComboBoxItem;
            if (item.Tag != null)
            {
                int.TryParse(item.Tag.ToString(), out fogDegree);
                World.Settings.FogFarFactor = fogDegree;
            }
        }

        private void sliderFogDegree_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            World.Settings.FogFarFactor = (float)sliderFogDegree.Value;
        }

        private void chkSunFixed_Checked(object sender, RoutedEventArgs e)
        {
            World.Settings.SunSynchedWithTime = !chkSunFixed.IsChecked.Value;
        }

        private void sdSunHeading_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            World.Settings.SunHeading = MathEngine.DegreesToRadians(sdSunHeading.Value);
            World.Settings.SunElevation = MathEngine.DegreesToRadians(sdSunElevation.Value);

            lineSunHeading.X2 = 20 * Math.Cos(World.Settings.SunHeading);
            lineSunHeading.Y2 = -20 * Math.Sin(World.Settings.SunHeading);

            lineSunElevation.X2 = 35 * Math.Cos(World.Settings.SunElevation);
            lineSunElevation.Y2 = -35 * Math.Sin(World.Settings.SunElevation);
        }

    }
}
