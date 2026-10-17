using Heiflow.Models.Generic;
using Heiflow.Models.Visualization;
using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
using Heiflow.Visualization.Renderable.Grid;
using Heiflow.Visualization.RenderableModel.Model.Renders;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    /// VisSetting.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class Symbology : UserControl
    {
        private  Lazy<SymbologyViewModel> viewModel;
        private IDX3DLayerRender _Render;

        /// <summary>
        /// The ramp id behind every entry of the ramp list, in the order the entries were added. The
        /// entries carry the colours of the ramp rather than its id, so the ids are kept here and
        /// reached through the position that was picked.
        /// </summary>
        private readonly List<int> _RampIds = new List<int>();

        /// <summary>
        /// The ramp a render is created with, the one DataColor starts from, so the panel opens on
        /// the scheme a layer that has not been touched yet is drawn with.
        /// </summary>
        private const int DefaultRampId = 22;

        /// <summary>
        /// True while the layer the panel is on is a vector layer, which is the only kind of layer
        /// the vector group has anything to say about. It is what the group is enabled by in the
        /// markup, see expVelocity, and it is set every time another layer is put on the panel.
        /// </summary>
        public static readonly DependencyProperty IsVectorLayerProperty =
            DependencyProperty.Register("IsVectorLayer", typeof(bool), typeof(Symbology),
                new PropertyMetadata(false));

        public bool IsVectorLayer
        {
            get { return (bool)GetValue(IsVectorLayerProperty); }
            set { SetValue(IsVectorLayerProperty, value); }
        }

        /// <summary>
        /// True while the layer the panel is on is drawn from a MODFLOW grid, which is the only one
        /// that has a shape to set, see expGrid.
        /// </summary>
        public static readonly DependencyProperty IsModelGridProperty =
            DependencyProperty.Register("IsModelGrid", typeof(bool), typeof(Symbology),
                new PropertyMetadata(false));

        public bool IsModelGrid
        {
            get { return (bool)GetValue(IsModelGridProperty); }
            set { SetValue(IsModelGridProperty, value); }
        }

        public Symbology()
        {
            InitializeComponent();
            Loaded += VisSetting_Loaded;
        }

        public IDX3DLayerRender SelectedRender
        {
            get
            {
                return _Render;
            }
            set
            {
                _Render = value;
                if (_Render != null && _Render.LayerObject != null)
                    tbCurLayer.Text = _Render.LayerObject.Name;
                SyncControlsToRender();
            }
        }

        public Lazy<SymbologyViewModel> ViewModel
        {
            get
            {
                return viewModel;
            }
            set
            {
                viewModel = value;
            }
        }

        private void VisSetting_Loaded(object sender, RoutedEventArgs e)
        {
            if (cobColorRamp == null)
                return;

            DetachHandlers();

            cobColorRamp.Items.Clear();
            cobColorRampCount.Items.Clear();
            cobTileCount.Items.Clear();
            _RampIds.Clear();

            // The entry stands for the id that Ramp is built with. The list used to run from one to
            // twenty three and hand over the id minus one, which the constructor clamps back to one,
            // so two entries asked for the same ramp while the ramp at the top of the range stayed
            // out of reach. RampCount is how many ramps are defined, not the last id, the ids run
            // from MinId to the last index of the array that holds them.
            int lastRampId = Core.Drawing.Constants.RampDefinitions.RampStrings.Length - 1;
            for (int id = Core.Drawing.Constants.RampDefinitions.MinId; id <= lastRampId; id++)
            {
                var ramp = new Core.Drawing.Ramp(id);
                if (ramp.RampId != id)
                    continue;
                _RampIds.Add(id);
                cobColorRamp.Items.Add(BuildRampBrush(ramp));
            }
            cobColorRamp.ItemTemplate = BuildRampItemTemplate();

            foreach (var count in new[] { 5, 6, 7, 8, 9, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 })
            {
                cobColorRampCount.Items.Add(new ComboBoxItem() { Content = count.ToString() });
            }

            for (int i = 1; i < 11; i++)
            {
                cobTileCount.Items.Add(new ComboBoxItem() { Content = i.ToString() });
            }
            cobTileCount.SelectedIndex = 2;

            AttachHandlers();

            SelectRampId(DefaultRampId);
            SelectClassCount(DataColor.DefaultClassCount);
            SyncControlsToRender();
        }

        /// <summary>
        /// A ramp used to be offered as a bare number, which said nothing about the colours it stands
        /// for. The entry carries the ramp itself, so the set can be compared in the drop down. It is
        /// a brush and not a control because the entry is shown twice, in the drop down and in the
        /// box of the closed combo, and one control cannot sit in both places at once.
        /// </summary>
        private static LinearGradientBrush BuildRampBrush(Core.Drawing.Ramp ramp)
        {
            var colors = ramp.Colors;
            if (colors == null || colors.Length == 0)
                return new LinearGradientBrush(Colors.Transparent, Colors.Transparent, 0.0);

            const int stops = 16;
            var gradient = new GradientStopCollection();
            for (int s = 0; s < stops; s++)
            {
                var color = colors[s * (colors.Length - 1) / (stops - 1)];
                gradient.Add(new GradientStop(
                    Color.FromRgb(color.R, color.G, color.B),
                    (double)s / (stops - 1)));
            }
            return new LinearGradientBrush(gradient, new Point(0, 0.5), new Point(1, 0.5));
        }

        /// <summary>
        /// Draws the brush of an entry as a bar.
        /// </summary>
        private static DataTemplate BuildRampItemTemplate()
        {
            var template = new DataTemplate(typeof(Brush));
            var bar = new FrameworkElementFactory(typeof(Grid));
            bar.SetValue(Grid.HeightProperty, 14.0);
            bar.SetValue(Grid.WidthProperty, 158.0);
            bar.SetBinding(Grid.BackgroundProperty, new Binding("."));
            template.VisualTree = bar;
            return template;
        }

        /// <summary>
        /// The panel is shared by every layer, so the controls are made to show the scheme of the
        /// layer that was just selected. Without this a layer that was configured differently kept
        /// the values of the one before it on screen, and picking from those values pushed them at
        /// the new layer without the panel ever showing that it had.
        /// </summary>
        private void SyncControlsToRender()
        {
            // A layer that draws no flow has no arrow and no line to set, and one that is not drawn
            // from a grid has no shape to give it, so the groups that set them are greyed out. Done
            // before the controls are filled, they are filled from the same render either way.
            IsVectorLayer = _Render is IVectorRender;
            // The one that draws a flow is a grid render as well, but it draws arrows and lines
            // where the grid would be and no face of it, so the shape does nothing there.
            IsModelGrid = _Render is MFGridRender && !(_Render is MFVectorRender);

            if (_Render == null)
                return;

            DetachHandlers();
            try
            {
                SelectRampId(_Render.ColorRampID);
                SelectClassCount(_Render.ColourRampCount);
                cobClassMethod.SelectedIndex =
                    _Render.ClassificationMethod == Core.Drawing.ClassificationMethod.Equal_Inteval ? 1 : 0;
                sliderOpacity.Value = _Render.Opacity;
                chbUniqueColor.IsChecked = _Render.UniqueColor;
                chbInvertColor.IsChecked = _Render.InvertColor;
                chbContinuousColor.IsChecked = _Render.ContinuousColor;
                chbShadedLighting.IsChecked = _Render.ShadedLighting;

                // How much of the third dimension the grid is drawn with, read back the same way
                // the tile count is, so the panel opens on what the layer is drawn as.
                var grid = _Render as MFGridRender;
                if (grid != null && !(_Render is MFVectorRender))
                    cobGridShape.SelectedIndex = Math.Max(0, Math.Min(
                        cobGridShape.Items.Count - 1, (int)grid.Skirt));

                // The tile count is the one vector property the panel shows, so it is read back as
                // well. Left out of this, the combo kept the count it was opened with while the
                // render drew with its own, and the setting only reached the layer once it was
                // picked again by hand.
                var vector = _Render as IVectorRender;
                if (vector != null)
                {
                    SelectComboBoxValue(cobTileCount, vector.Style.TileCount);
                    chbStreamlines.IsChecked = vector.Style.ShowStreamlines;
                    SelectComboBoxNumber(cobStreamlineWidth, vector.Style.StreamlineWidth);
                    SelectComboBoxNumber(cobFlowSpeed, vector.Style.FlowSpeed);
                    SelectComboBoxNumber(cobFadeTime, vector.Style.FadeTime);
                    cobStreamlineSpacing.SelectedIndex = Math.Max(0, Math.Min(
                        cobStreamlineSpacing.Items.Count - 1, vector.Style.StreamlineSpacing));
                }
            }
            finally
            {
                AttachHandlers();
            }
        }

        private void SelectRampId(int rampId)
        {
            var index = _RampIds.IndexOf(rampId);
            if (index >= 0)
                cobColorRamp.SelectedIndex = index;
        }

        /// <summary>
        /// Puts a combo whose entries are numbers on the entry that reads as the value. The combo is
        /// left where it is when no entry reads as it.
        /// </summary>
        private static void SelectComboBoxValue(ComboBox combo, int value)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                var item = combo.Items[i] as ComboBoxItem;
                int entry;
                if (item != null && item.Content != null
                    && int.TryParse(item.Content.ToString(), out entry) && entry == value)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        /// <summary>
        /// The same for a value that is not a whole number, which is what a fade time is.
        /// </summary>
        private static void SelectComboBoxNumber(ComboBox combo, double value)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                var item = combo.Items[i] as ComboBoxItem;
                double entry;
                if (item != null && item.Content != null
                    && double.TryParse(item.Content.ToString(), out entry) && Math.Abs(entry - value) < 0.001)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        private void SelectClassCount(int classCount)
        {
            SelectComboBoxValue(cobColorRampCount, classCount);
        }

        /// <summary>
        /// The handlers write straight back at the render, so filling the controls from it has to
        /// happen with them taken off, otherwise showing a value counts as picking it.
        /// </summary>
        private void DetachHandlers()
        {
            cobColorRamp.SelectionChanged -= cobColorRamp_SelectionChanged;
            cobColorRampCount.SelectionChanged -= cobColorRampCount_SelectionChanged;
            cobClassMethod.SelectionChanged -= CobClassMethod_SelectionChanged;
            cobTileCount.SelectionChanged -= cobTileCount_SelectionChanged;
            cobGridShape.SelectionChanged -= cobGridShape_SelectionChanged;
            cobStreamlineWidth.SelectionChanged -= cobStreamlineWidth_SelectionChanged;
            cobFlowSpeed.SelectionChanged -= cobFlowSpeed_SelectionChanged;
            cobFadeTime.SelectionChanged -= cobFadeTime_SelectionChanged;
            cobStreamlineSpacing.SelectionChanged -= cobStreamlineSpacing_SelectionChanged;
            sliderOpacity.ValueChanged -= sliderOpacity_ValueChanged;
            chbUniqueColor.Checked -= chbUniqueColor_Checked;
            chbUniqueColor.Unchecked -= chbUniqueColor_Checked;
            chbInvertColor.Checked -= chbInvertColor_Checked;
            chbInvertColor.Unchecked -= chbInvertColor_Checked;
            chbContinuousColor.Checked -= chbContinuousColor_Checked;
            chbContinuousColor.Unchecked -= chbContinuousColor_Checked;
            chbShadedLighting.Checked -= chbShadedLighting_Checked;
            chbShadedLighting.Unchecked -= chbShadedLighting_Checked;
            chbStreamlines.Checked -= chbStreamlines_Checked;
            chbStreamlines.Unchecked -= chbStreamlines_Checked;
        }

        private void AttachHandlers()
        {
            cobColorRamp.SelectionChanged += cobColorRamp_SelectionChanged;
            cobColorRampCount.SelectionChanged += cobColorRampCount_SelectionChanged;
            cobClassMethod.SelectionChanged += CobClassMethod_SelectionChanged;
            cobTileCount.SelectionChanged += cobTileCount_SelectionChanged;
            cobGridShape.SelectionChanged += cobGridShape_SelectionChanged;
            cobStreamlineWidth.SelectionChanged += cobStreamlineWidth_SelectionChanged;
            cobFlowSpeed.SelectionChanged += cobFlowSpeed_SelectionChanged;
            cobFadeTime.SelectionChanged += cobFadeTime_SelectionChanged;
            cobStreamlineSpacing.SelectionChanged += cobStreamlineSpacing_SelectionChanged;
            sliderOpacity.ValueChanged += sliderOpacity_ValueChanged;
            chbUniqueColor.Checked += chbUniqueColor_Checked;
            chbUniqueColor.Unchecked += chbUniqueColor_Checked;
            chbInvertColor.Checked += chbInvertColor_Checked;
            chbInvertColor.Unchecked += chbInvertColor_Checked;
            chbContinuousColor.Checked += chbContinuousColor_Checked;
            chbContinuousColor.Unchecked += chbContinuousColor_Checked;
            chbShadedLighting.Checked += chbShadedLighting_Checked;
            chbShadedLighting.Unchecked += chbShadedLighting_Checked;
            chbStreamlines.Checked += chbStreamlines_Checked;
            chbStreamlines.Unchecked += chbStreamlines_Checked;
        }

        private static int ReadClassCount(ComboBox combo)
        {
            var item = combo.SelectedItem as ComboBoxItem;
            int value;
            if (item != null && item.Content != null && int.TryParse(item.Content.ToString(), out value))
                return value;
            return 0;
        }

        /// <summary>
        /// The shape the grid is drawn with. The entries are written in the order of GridSkirt, so
        /// the one that was picked is the value itself.
        /// </summary>
        private void cobGridShape_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = _Render as MFGridRender;
            if (grid != null && cobGridShape.SelectedIndex >= 0
                && cobGridShape.SelectedIndex <= (int)GridSkirt.Boundary)
            {
                grid.Skirt = (GridSkirt)cobGridShape.SelectedIndex;
            }
        }

        private void chbUniqueColor_Checked(object sender, RoutedEventArgs e)
        {
            if (_Render != null)
                _Render.UniqueColor = chbUniqueColor.IsChecked.Value;
        }

        private void chbInvertColor_Checked(object sender, RoutedEventArgs e)
        {
            if (_Render != null)
                _Render.InvertColor = chbInvertColor.IsChecked.Value; 
        }

        private void chbContinuousColor_Checked(object sender, RoutedEventArgs e)
        {
            if (_Render != null)
                _Render.ContinuousColor = chbContinuousColor.IsChecked.Value;
        }

        private void chbShadedLighting_Checked(object sender, RoutedEventArgs e)
        {
            if (_Render != null)
                _Render.ShadedLighting = chbShadedLighting.IsChecked.Value;
        }

        private void cobColorRamp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_Render == null)
                return;
            if (cobColorRamp.SelectedIndex < 0 || cobColorRamp.SelectedIndex >= _RampIds.Count)
                return;
            _Render.ColorRampID = _RampIds[cobColorRamp.SelectedIndex];
        }

        private void cobColorRampCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_Render == null)
                return;
            var classCount = ReadClassCount(cobColorRampCount);
            // a count of zero would tell the render to keep the classes it has, which is not what
            // an entry that failed to read stands for, so it is not handed over
            if (classCount > 0)
                _Render.ColourRampCount = classCount;
        }

        private void sliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_Render != null)
            {
                _Render.Opacity = (int)sliderOpacity.Value;
            }
        }

        private void CobClassMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_Render == null)
                return;
            _Render.ClassificationMethod = cobClassMethod.SelectedIndex == 1
                ? Core.Drawing.ClassificationMethod.Equal_Inteval
                : Core.Drawing.ClassificationMethod.Natural_Breaks_Jenks;
        }

        private void chbVelocityScalar_Checked(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
      
            }
        }

        private void chbVelocityAutoColor_Checked(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
                var vector = _Render as IVectorRender;
                if (vector != null)
                {
                    vector.Style.AutoColor = chbVelocityAutoColor.IsChecked.Value;
                }
            }
        }
        private void cobVVScale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (viewModel != null)
            {
                var vector = _Render as IVectorRender;
                if (vector != null)
                {
                    if (cobVVScale.SelectedIndex == 0)
                        vector.Style.ScaleMethod = HUST.WREIS.Dot3D.ScaleMethod.None;
                    else if (cobVVScale.SelectedIndex == 1)
                        vector.Style.ScaleMethod = HUST.WREIS.Dot3D.ScaleMethod.Linear;
                    else if (cobVVScale.SelectedIndex == 2)
                        vector.Style.ScaleMethod = HUST.WREIS.Dot3D.ScaleMethod.StandardDeviation;
                }
            }
        }

        private void cobVectorArrow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (viewModel != null)
            {
                var vector = _Render as IVectorRender;
                if (vector != null)
                {
                    var item = cobVectorArrow.SelectedItem as ComboBoxItem;
                    double headSize;
                    if (item != null && item.Tag != null && double.TryParse(item.Tag.ToString(), out headSize))
                        vector.Style.HeadSize = headSize;
                }
            }
        }

        private void chbStreamlines_Checked(object sender, RoutedEventArgs e)
        {
            var vector = _Render as IVectorRender;
            if (vector != null)
                vector.Style.ShowStreamlines = chbStreamlines.IsChecked.Value;
        }

        private void btnVelocityStyleProperty_Click(object sender, RoutedEventArgs e)
        {
            var vector = _Render as IVectorRender;
            if (vector != null)
            {
                viewModel.Value.ShellService.PropertyView.SelectedObject = vector.Style;
                viewModel.Value.ShellService.SelectPanel(DockPanelNames.PropertyPanel);
            }
        }

        private void cobTileCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (viewModel != null)
            {
                var vector = _Render as IVectorRender;
                if (vector != null)
                {
                    var tileCount = ReadClassCount(cobTileCount);
                    if (tileCount > 0)
                        vector.Style.TileCount = tileCount;
                }
            }
        }

        /// <summary>
        /// The number written on the entry of a combo that is picked, or the value handed over when
        /// the entry does not read as one, which is the one the render is drawing with.
        /// </summary>
        private static float ReadSelectedNumber(ComboBox combo, float kept)
        {
            var item = combo.SelectedItem as ComboBoxItem;
            double value;
            if (item != null && item.Content != null && double.TryParse(item.Content.ToString(), out value))
                return (float)value;
            return kept;
        }

        private void cobStreamlineWidth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vector = _Render as IVectorRender;
            if (vector != null)
                vector.Style.StreamlineWidth = ReadSelectedNumber(cobStreamlineWidth, vector.Style.StreamlineWidth);
        }

        private void cobFlowSpeed_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vector = _Render as IVectorRender;
            if (vector != null)
                vector.Style.FlowSpeed = ReadSelectedNumber(cobFlowSpeed, vector.Style.FlowSpeed);
        }

        private void cobFadeTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vector = _Render as IVectorRender;
            if (vector != null)
                vector.Style.FadeTime = ReadSelectedNumber(cobFadeTime, vector.Style.FadeTime);
        }

        /// <summary>
        /// The spacing is written on the combo as a number of cells, apart from the first entry,
        /// which says that no room is kept at all.
        /// </summary>
        private void cobStreamlineSpacing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vector = _Render as IVectorRender;
            if (vector != null && cobStreamlineSpacing.SelectedIndex >= 0)
                vector.Style.StreamlineSpacing = cobStreamlineSpacing.SelectedIndex;
        }

    }
}
