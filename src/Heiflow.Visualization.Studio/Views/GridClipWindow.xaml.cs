using Heiflow.Visualization.Applications;
using Heiflow.Visualization.Renderable.Grid;
using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Heiflow.Visualization.Studio.Views
{
    /// <summary>
    /// 模型网格三维剖切控制窗口：沿模型自身的 X（东）、Y（北）、Z（垂向）三个方向各给一对
    /// 相对位置滑块，设置写进 ModelGridClipper，由 ModelRenderScope 在每个模型图层绘制时
    /// 应用为 D3D 裁剪平面。窗口是非模态的，场景持续重绘，滑块一动网格剖切就跟着变。
    /// </summary>
    public partial class GridClipWindow : MetroWindow
    {
        private readonly VGSProjectService _ProjectService;
        private bool _loading;

        public GridClipWindow(VGSProjectService projectService)
        {
            InitializeComponent();
            _ProjectService = projectService;
            Loaded += GridClipWindow_Loaded;
        }

        void GridClipWindow_Loaded(object sender, RoutedEventArgs e)
        {
            chkEnabled.IsChecked = ModelGridClipper.Enabled;
            if (!ModelGridClipper.HasBounds)
                LoadGridBounds();
            RefreshUI();
        }

        /// <summary>从项目里的 Grid 图层渲染器重新测出三个方向和范围。</summary>
        private void LoadGridBounds()
        {
            GridClipperBounds bounds = FindGridBounds();
            if (!bounds.Available)
            {
                txtStatus.Text = "没有找到已加载的模型网格，请先打开项目再使用剖切。";
                return;
            }

            bool ok = ModelGridClipper.Setup(bounds.Render);
            if (ok)
            {
                SetFraction(ModelGridClipper.XAxis, sldXStart, sldXEnd);
                SetFraction(ModelGridClipper.YAxis, sldYStart, sldYEnd);
                SetFraction(ModelGridClipper.ZAxis, sldZStart, sldZEnd);
                txtStatus.Text = "已载入网格范围，拖动滑块即可剖切。";
            }
            else
            {
                txtStatus.Text = "网格还没有生成几何数据，暂不能确定剖切范围。";
            }
        }

        /// <summary>项目图层列表里名字为 Grid 的图层，取它的渲染器。</summary>
        private GridClipperBounds FindGridBounds()
        {
            if (_ProjectService == null || _ProjectService.DX3DLayers == null)
                return new GridClipperBounds();

            var layer = _ProjectService.DX3DLayers.FirstOrDefault(l => l.Name == "Grid");
            if (layer == null || layer.RenderDX == null)
                return new GridClipperBounds();

            return new GridClipperBounds { Render = layer.RenderDX, Available = true };
        }

        private class GridClipperBounds
        {
            public IDX3DLayerRender Render;
            public bool Available;
        }

        private static void SetFraction(ModelGridClipper.AxisClip axis, Slider start, Slider end)
        {
            start.Value = axis.Start;
            end.Value = axis.End;
        }

        /// <summary>把控件的当前状态写回 ModelGridClipper。</summary>
        private void ApplySettings()
        {
            if (_loading)
                return;

            ModelGridClipper.XAxis.Clip = chkX.IsChecked == true;
            ModelGridClipper.YAxis.Clip = chkY.IsChecked == true;
            ModelGridClipper.ZAxis.Clip = chkZ.IsChecked == true;
            ModelGridClipper.XAxis.Start = sldXStart.Value;
            ModelGridClipper.XAxis.End = sldXEnd.Value;
            ModelGridClipper.YAxis.Start = sldYStart.Value;
            ModelGridClipper.YAxis.End = sldYEnd.Value;
            ModelGridClipper.ZAxis.Start = sldZStart.Value;
            ModelGridClipper.ZAxis.End = sldZEnd.Value;
            ModelGridClipper.Enabled = chkEnabled.IsChecked == true;
        }

        /// <summary>把 ModelGridClipper 的状态刷到控件上并显示实际范围。</summary>
        private void RefreshUI()
        {
            _loading = true;
            try
            {
                chkX.IsChecked = ModelGridClipper.XAxis.Clip;
                chkY.IsChecked = ModelGridClipper.YAxis.Clip;
                chkZ.IsChecked = ModelGridClipper.ZAxis.Clip;
                sldXStart.Value = ModelGridClipper.XAxis.Start;
                sldXEnd.Value = ModelGridClipper.XAxis.End;
                sldYStart.Value = ModelGridClipper.YAxis.Start;
                sldYEnd.Value = ModelGridClipper.YAxis.End;
                sldZStart.Value = ModelGridClipper.ZAxis.Start;
                sldZEnd.Value = ModelGridClipper.ZAxis.End;

                txtXRange.Text = RangeText(ModelGridClipper.XAxis);
                txtYRange.Text = RangeText(ModelGridClipper.YAxis);
                txtZRange.Text = RangeText(ModelGridClipper.ZAxis);

                bool hasBounds = ModelGridClipper.HasBounds;
                bool enabled = chkEnabled.IsChecked == true && hasBounds;
                chkX.IsEnabled = hasBounds;
                chkY.IsEnabled = hasBounds;
                chkZ.IsEnabled = hasBounds;
                sldXStart.IsEnabled = sldXEnd.IsEnabled = enabled && ModelGridClipper.XAxis.Clip;
                sldYStart.IsEnabled = sldYEnd.IsEnabled = enabled && ModelGridClipper.YAxis.Clip;
                sldZStart.IsEnabled = sldZEnd.IsEnabled = enabled && ModelGridClipper.ZAxis.Clip;
            }
            finally
            {
                _loading = false;
            }

            ApplySettings();
            ShowValues();
        }

        private static string RangeText(ModelGridClipper.AxisClip axis)
        {
            if (!axis.IsValid)
                return "";
            return string.Format("范围 {0:F0} m ~ {1:F0} m", axis.Min, axis.Max);
        }

        private void ShowValues()
        {
            txtXStart.Text = PercentText(sldXStart);
            txtXEnd.Text = PercentText(sldXEnd);
            txtYStart.Text = PercentText(sldYStart);
            txtYEnd.Text = PercentText(sldYEnd);
            txtZStart.Text = PercentText(sldZStart);
            txtZEnd.Text = PercentText(sldZEnd);
        }

        private static string PercentText(Slider slider)
        {
            return string.Format("{0:F0}%", slider.Value * 100);
        }

        private void chkEnabled_Changed(object sender, RoutedEventArgs e)
        {
            if (_loading)
                return;
            ApplySettings();
            RefreshUI();
        }

        private void Axis_Changed(object sender, RoutedEventArgs e)
        {
            if (_loading)
                return;
            ApplySettings();
            // 起点/终点不许交叉：把滑块值互相顶住。
            ClampPair(sldXStart, sldXEnd);
            ClampPair(sldYStart, sldYEnd);
            ClampPair(sldZStart, sldZEnd);
            ApplySettings();
            ShowValues();
        }

        private static void ClampPair(Slider start, Slider end)
        {
            if (start.Value > end.Value)
            {
                double mid = (start.Value + end.Value) * 0.5;
                start.Value = mid;
                end.Value = mid;
            }
        }

        private void btnReloadBounds_Click(object sender, RoutedEventArgs e)
        {
            LoadGridBounds();
            RefreshUI();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            chkEnabled.IsChecked = true;
            chkX.IsChecked = true;
            chkY.IsChecked = true;
            chkZ.IsChecked = true;
            sldXStart.Value = 0;
            sldXEnd.Value = 1;
            sldYStart.Value = 0;
            sldYEnd.Value = 1;
            sldZStart.Value = 0;
            sldZEnd.Value = 1;
            RefreshUI();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
