using Heiflow.Models.IO;
using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
using Heiflow.Visualization.Studio.Controls;
using HUST.WREIS.Dot3D;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;

namespace Heiflow.Visualization.Studio
{
    /// <summary>
    /// ShellWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export(typeof(IVGSShellView))]
    public partial class ShellWindow :  IVGSShellView
    {
        private readonly Lazy<VGSShellViewModel> viewModel;
       
        public ShellWindow()
        {
            InitializeComponent();
            viewModel = new Lazy<VGSShellViewModel>(() => ViewHelper.GetViewModel<VGSShellViewModel>(this));
            this.Loaded += ShellWindow_Loaded;
            this.Closing += ShellWindow_Closing;
            this.Closed += ShellWindow_Closed;
        }

        private void ShellWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShellWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewModel.Value.ShellService.Timer.Stop();
        }

        private void ShellWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (IChildWPFWindow ch in viewModel.Value.ShellService.ChildWPFWindows)
            {
                ch.Owner = this;
            }
            foreach (var ch in viewModel.Value.PackageUIService.OptionalViews)
            {
                if (ch is IChildWPFWindow)
                    (ch as IChildWPFWindow).Owner = this;
            }
            var man=VGSManager.Instance as VGSManager;
            if(man.CheckLicense())
            {
                MainGrid.Visibility = System.Windows.Visibility.Visible;
                ActivicationPanel.Visibility = System.Windows.Visibility.Hidden;
                mainMenu.IsEnabled = true;
            }
            else
            {
                MainGrid.Visibility = System.Windows.Visibility.Collapsed;
                ActivicationPanel.Visibility = System.Windows.Visibility.Visible;
                mainMenu.IsEnabled = false;
            }
            InitMenuItemState();

        }

        private void InitMenuItemState()
        {
            miCameraHasInertia.IsChecked = World.Settings.CameraHasInertia;
            miCameraHasMoveMo.IsChecked = World.Settings.CameraHasMomentum;
            miSunshading.IsChecked = World.Settings.EnableSunShading;
            miHasSky.IsChecked = World.Settings.EnableSky;
            miHasAtmosphereScattering.IsChecked = World.Settings.EnableAtmosphericScattering;
            miPositionInfo.IsChecked = World.Settings.ShowPosition;
            miHasCrossHairs.IsChecked = World.Settings.ShowCrosshairs;
            miDisplayFog.IsChecked = World.Settings.EnableFog;
            miHasCompass.IsChecked = World.Settings.ShowCompass;
            miShowScaleBar.IsChecked = World.Settings.ShowScaleBar;
            miGridLengend.IsChecked = World.Settings.ShowLengendBar;
            miShowLatLong.IsChecked = World.Settings.ShowLatLonLines;
            miEnableHighPerfomance.IsChecked = World.Settings.EnableHighPerfomance;
            miHasAtmosphere.IsChecked = World.Settings.EnableAtmosphere;
        }

        private void btnVisSetting_Click(object sender, RoutedEventArgs e)
        {
            var index = int.Parse((sender as System.Windows.Controls.Button).Tag.ToString());
            ToggleFlyout(index);
        }

        private void Activication_Activated(object sender, EventArgs e)
        {
            MainGrid.Visibility = System.Windows.Visibility.Visible;
            ActivicationPanel.Visibility = System.Windows.Visibility.Hidden;
            mainMenu.IsEnabled = true;
        }

        private void OpenPrj_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "project file|*.ihmx";
            if (viewModel.Value.ProjectService.Project != null)
            {
                if(MessageBox.Show("A project has been opened, do you want to open another project?", "Open Project",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    viewModel.Value.ClearProject.Execute(null);
                }
            }
            if (dialog.ShowDialog().Value)
            {
                viewModel.Value.OpenProject.Execute(dialog.FileName);
                viewModel.Value.ShellService.SelectPanel(DockPanelNames.ProjectExplorerPanel);
            }

        }

        private void ViewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;
            viewModel.Value.ViewCommand.Execute(item.Name);
        }
        private void miGotoDefault_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Value.ShellService.VirtualGlobeView.VirtualGlobe.GotoLatLonAltitude(World.Settings.DefalutLatitude, World.Settings.DefalutLongitude, 1500000.0f);
        }

        private void miCurrentPosAsDefault_Click(object sender, RoutedEventArgs e)
        {
            var ww = viewModel.Value.ShellService.VirtualGlobeView.VirtualGlobe;
            World.Settings.DefalutLatitude = (float)ww.DrawArgs.WorldCamera.Latitude.Degrees;
            World.Settings.DefalutLongitude = (float)ww.DrawArgs.WorldCamera.Longitude.Degrees;
            World.Settings.Save();
        }

        private void miSaveViewSetting_Click(object sender, RoutedEventArgs e)
        {
            World.Settings.Save();
        }
        private void miLoadShp_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog().Value)
            {
                viewModel.Value.LoadShpFile.Execute(dialog.FileName);
            }
        }
        private void miLoad3DModel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog().Value)
            {
                viewModel.Value.Load3DModel.Execute(dialog.FileName);
            }
        }
        private void miLoadODM_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog().Value)
            {
                viewModel.Value.LoadODM.Execute(dialog.FileName);
            }
        }
        private void miSearchSites_Click(object sender, RoutedEventArgs e)
        {
            SeachSitesWindow sw = new SeachSitesWindow();
            sw.ShowInTaskbar = false;
            sw.Owner = this;
            sw.Show();
        }

        private void miMeasure_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Value.MeasureCommand.Execute(miMeasure.IsChecked);
        }
        private void miScreenShot_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Filter = "bmp file | *.bmp | jpg file | *.jpg| png file(*.png) | *.png";
            if (ofd.ShowDialog().Value)
            {
                viewModel.Value.ScreenShotCommand.Execute(ofd.FileName);
            }
        }
        private void miShowPE_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Value.ShellService.SelectPanel(DockPanelNames.ProjectExplorerPanel);
        }
        private void miShowAnimPlayer_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Value.ShellService.SelectPanel(DockPanelNames.AnimationPlayerPanel);
        }
        private void miShowProperty_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Value.ShellService.SelectPanel(DockPanelNames.PropertyPanel);
        }
        private void miShowChart_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Value.ShellService.SelectPanel(DockPanelNames.WinChartPanel);
        }
        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Value.ShellService.AboutView.Show();
        }
        private void miRunModel_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Value.ShellService.SelectPanel(DockPanelNames.RunModelPanel);
        }
        private void ToggleFlyout(int index)
        {
            var flyout = this.Flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }
            flyout.IsOpen = !flyout.IsOpen;
        }

        private void miShowStatInfo_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Value.ShellService.VirtualGlobeView.VirtualGlobe.ShowStatisticsInfo = !viewModel.Value.ShellService.VirtualGlobeView.VirtualGlobe.ShowStatisticsInfo;
        }

        private void miDis3DRenderInfo_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Value.ShellService.VirtualGlobeView.VirtualGlobe.ShowPerformanceInfo = !viewModel.Value.ShellService.VirtualGlobeView.VirtualGlobe.ShowPerformanceInfo;
        }


    }
}
