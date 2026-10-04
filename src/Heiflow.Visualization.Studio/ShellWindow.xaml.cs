using Heiflow.Models.IO;
using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
using Heiflow.Visualization.Studio.Controls;
using HUST.WREIS.Dot3D;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.ComponentModel.Composition;
using System.Linq;
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
                ActivicationPanel.Visibility = System.Windows.Visibility.Collapsed;
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
            var btn = sender as System.Windows.Controls.Button;
            if (btn == null || btn.Tag == null)
                return;
            int index;
            if (!int.TryParse(btn.Tag.ToString(), out index))
                return;
            // 0 opens the scene settings, the other buttons fall back to the layers tab
            ShowRightPanelTab(index == 0 ? 1 : 0);
        }

        private void Activication_Activated(object sender, EventArgs e)
        {
            MainGrid.Visibility = System.Windows.Visibility.Visible;
            ActivicationPanel.Visibility = System.Windows.Visibility.Collapsed;
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
        /// <summary>
        /// Width of the right panel before it was collapsed.
        /// </summary>
        private GridLength rightPanelWidth = new GridLength(300);

        /// <summary>
        /// True while the right panel is collapsed to its header only.
        /// </summary>
        private bool rightPanelCollapsed = false;

        /// <summary>
        /// Index of the layers tab in the right panel.
        /// </summary>
        private const int LayersTabIndex = 0;

        private void btnCollapseRightPanel_Click(object sender, RoutedEventArgs e)
        {
            CollapseRightPanel(!rightPanelCollapsed);
        }

        /// <summary>
        /// Shows the given tab of the right panel and expands it when it is collapsed.
        /// </summary>
        private void ShowRightPanelTab(int tabIndex)
        {
            if (rightPanelCollapsed)
                CollapseRightPanel(false);
            var tabs = FindRightPanelTabs();
            if (tabs != null)
                tabs.SelectedIndex = tabIndex;
        }

        /// <summary>
        /// Collapses the right panel to its header, or restores the width it had before.
        /// </summary>
        private void CollapseRightPanel(bool collapsed)
        {
            // MainGrid holds the scene (0), the splitter (1) and the right panel (2)
            if (MainGrid == null || MainGrid.ColumnDefinitions.Count < 3)
                return;
            var rightColumn = MainGrid.ColumnDefinitions[2];
            var rightPanel = MainGrid.Children.OfType<System.Windows.Controls.Grid>().FirstOrDefault();
            if (rightPanel == null)
                return;
            var splitter = MainGrid.Children.OfType<System.Windows.Controls.GridSplitter>().FirstOrDefault();
            var tabs = rightPanel.Children.OfType<System.Windows.Controls.TabControl>().FirstOrDefault();

            rightPanelCollapsed = collapsed;
            if (collapsed)
            {
                rightPanelWidth = rightColumn.Width;
                rightColumn.MinWidth = 0;
                // only the header stays visible, so the panel can be expanded again
                rightColumn.Width = new GridLength(30);
            }
            else
            {
                rightColumn.MinWidth = 180;
                rightColumn.Width = rightPanelWidth;
            }
            if (splitter != null)
                splitter.Visibility = collapsed ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            if (tabs != null)
                tabs.Visibility = collapsed ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        /// <summary>
        /// Returns the tab control of the right panel. The name based field is not used because the
        /// generated code does not expose elements declared inside the main grid.
        /// </summary>
        private System.Windows.Controls.TabControl FindRightPanelTabs()
        {
            if (MainGrid == null)
                return null;
            var rightPanel = MainGrid.Children.OfType<System.Windows.Controls.Grid>().FirstOrDefault();
            return rightPanel != null ? rightPanel.Children.OfType<System.Windows.Controls.TabControl>().FirstOrDefault() : null;
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
