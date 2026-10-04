using Heiflow.Applications;
using Heiflow.Controls;
using Heiflow.Core.Animation;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.GHM;
using Heiflow.Models.Tools;
using Heiflow.Models.Visualization;
using Heiflow.Presentation.Animation;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using Heiflow.Visualization.Applications;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// AnimationPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class AnimationPlayer : UserControl
    {
        private IDataCubeAnimation _Animator;
        private DataCubeWorkspace _workspace;
        private BackgroundWorker worker;
       
        public AnimationPlayer()
        {
            InitializeComponent();
            if (PlayButton != null)
                PlayButton.Click += new RoutedEventHandler(PlayButton_Click);
            if (PauseButton != null)
                PauseButton.Click += new RoutedEventHandler(PauseButton_Click);
            if (StopButton != null)
                StopButton.Click += new RoutedEventHandler(StopButton_Click);

            _Animator = new D3DLayerAnimation();
            _Animator.CurrentChanged += _Animator_CurrentChanged;
            _Animator.Stopped += _Animator_Stopped;
            _workspace = new DataCubeWorkspace();
            _workspace.DataSourceCollectionChanged += _workspace_DataSourceCollectionChanged;
            ChangeButtonState(true);
            PlayButton.IsEnabled = false;
            cmbDates.IsEnabled = false;
            sliderPlayPrg.IsEnabled = false;
            cacheProgress.Visibility = System.Windows.Visibility.Hidden;
        }

        public IDataCubeAnimation Animator
        {
            get
            {
                return _Animator;
            }
        }

        public IDataCubeWorkspace DataCubeWorkspace
        {
            get 
            {
                return _workspace;
            }
        }

        private void _workspace_DataSourceCollectionChanged(object sender, EventArgs e)
        {
            if (_workspace.DataSources.Any())
            {
                var mat = _workspace.DataSources.Last();
                _Animator.DataSource = mat;
                var render = (mat.DataOwner as IPackage).Layer3D;
                render.RenderObject.DataSource = mat as DataCube<float>;
                (_Animator as D3DLayerAnimation).Render = render.RenderObject;
                sliderPlayPrg.Maximum = mat.Size[1] - 1;
                cmbDates.ItemsSource = mat.DateTimes;
                tbAniVarName.Text = "Animated Variable: " + mat.Name;
                cmbDates.IsEnabled = true;
                sliderPlayPrg.IsEnabled = true;
                ChangeButtonState(true);
                _Animator.Go(0);
            }
        }
        private void _Animator_CurrentChanged(object sender, int e)
        {
            sliderPlayPrg.ValueChanged -= sliderPlayPrg_ValueChanged;
            if (_Animator.DataSource.DateTimes != null)
                CurrentTime.Text = _Animator.DataSource.DateTimes[_Animator.Current].ToString();
            else
                CurrentTime.Text = _Animator.Current.ToString();
            (_Animator as D3DLayerAnimation).Render.CurrentTimeStep = _Animator.Current;
            sliderPlayPrg.Value = Animator.Current;
            sliderPlayPrg.ValueChanged += sliderPlayPrg_ValueChanged;
        }

        private void _Animator_Stopped(object sender, EventArgs e)
        {
            sliderPlayPrg.ValueChanged -= sliderPlayPrg_ValueChanged;
            sliderPlayPrg.Value = 0;
            ChangeButtonState(true);
            sliderPlayPrg.ValueChanged += sliderPlayPrg_ValueChanged;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonState(false);
            _Animator.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonState(true);
            _Animator.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonState(true);
            _Animator.Stop();
        }

        private void RewindButton_Click(object sender, RoutedEventArgs e)
        {
            if (_Animator.Current > 0)
                _Animator.Go(_Animator.Current - 1);
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (_Animator.Current < _Animator.Maximum)
                _Animator.Go(_Animator.Current + 1);
        }

        private void cobAnimateVelocity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_Animator != null)
            {
                var speed = int.Parse((cobAnimateVelocity.SelectedItem as ComboBoxItem).Tag.ToString());
                _Animator.Speed = speed;
            }
        }

        private void sliderPlayPrg_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ModelService.CurrentTimeStep = (int)sliderPlayPrg.Value;
            _Animator.Go((int)sliderPlayPrg.Value);
        }
        private void cmbDates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDates.SelectedIndex >= 0)
            {
                _Animator.Go(cmbDates.SelectedIndex);
                sliderPlayPrg.ValueChanged -= sliderPlayPrg_ValueChanged;
                sliderPlayPrg.Value = cmbDates.SelectedIndex;
                sliderPlayPrg.ValueChanged += sliderPlayPrg_ValueChanged;
            }
        }
        private void ChangeButtonState(bool enable_play)
        {
            PlayButton.IsEnabled = enable_play;
            cobAnimateVelocity.IsEnabled = enable_play;
            PauseButton.IsEnabled = !enable_play;
            StopButton.IsEnabled = !enable_play;
            ForwardButton.IsEnabled = !enable_play;
            RewindButton.IsEnabled = !enable_play;
        }
        private void ButCacheColor_Click(object sender, RoutedEventArgs e)
        {
            var render = (_Animator as D3DLayerAnimation).Render;
            render.CachingColorProgressChanged += render_CachingColorProgressChanged;
            render.CachingColorFinished += render_CachingColorFinished;
            butCacheColor.IsEnabled = false;
            cacheProgress.Visibility = System.Windows.Visibility.Visible;
            cacheProgress.Value = 0;
            ChangeButtonState(true);
            PlayButton.IsEnabled = false;
            cmbDates.IsEnabled = false;
            sliderPlayPrg.IsEnabled = false;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(render);

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            butCacheColor.IsEnabled = true;
            ChangeButtonState(false);
            PlayButton.IsEnabled = true;
            cmbDates.IsEnabled = true;
            sliderPlayPrg.IsEnabled = true;
            cacheProgress.Visibility = System.Windows.Visibility.Hidden;
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            cacheProgress.Value = e.ProgressPercentage;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var render = e.Argument as I3DLayerRender;
            render.CacheColor();
        }

        private void render_CachingColorFinished(object sender, EventArgs e)
        {
            var obj = sender as I3DLayerRender;
            obj.CachingColorProgressChanged -= render_CachingColorProgressChanged;
            obj.CachingColorFinished -= render_CachingColorFinished;
        }

        private void render_CachingColorProgressChanged(object sender, int e)
        {
            worker.ReportProgress(e);
        }

        private void BtnClearCacheColor(object sender, RoutedEventArgs e)
        {
            var render = (_Animator as D3DLayerAnimation).Render;
            render.ClearCachedColor();
        }

        private void chkUseCache_Checked(object sender, RoutedEventArgs e)
        {
            var render = (_Animator as D3DLayerAnimation).Render;
            render.UseCache = chkUseCache.IsChecked.Value;
        }
    }
}
