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
                tbCurLayer.Text = _Render.LayerObject.Name;
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
            if (cobColorRamp != null)
            {
                cobColorRamp.SelectionChanged -= cobColorRamp_SelectionChanged;
                cobColorRampCount.SelectionChanged -= cobColorRampCount_SelectionChanged;
                cobTileCount.SelectionChanged -= this.cobTileCount_SelectionChanged;
                cobColorRamp.Items.Clear();
                cobTileCount.Items.Clear();
                for (int i = 1; i < 24; i++)
                {
                    cobColorRamp.Items.Add(new ComboBoxItem() { Content = i.ToString() });
                }
                cobColorRamp.SelectedIndex = 22;
                cobColorRampCount.Items.Clear();
                cobColorRampCount.Items.Add(new ComboBoxItem() { Content = 5 });
                cobColorRampCount.Items.Add(new ComboBoxItem() { Content = 6});
                cobColorRampCount.Items.Add(new ComboBoxItem() { Content = 7 });
                cobColorRampCount.Items.Add(new ComboBoxItem() { Content = 8 });
                cobColorRampCount.Items.Add(new ComboBoxItem() { Content = 9 });
                cobColorRampCount.Items.Add(new ComboBoxItem() { Content = 10 });
                for (int i = 2; i <= 10; i++)
                {
                    cobColorRampCount.Items.Add(new ComboBoxItem() { Content = (i * 10).ToString() });
                }
                cobColorRampCount.SelectedIndex = 0;
                cobColorRamp.SelectionChanged += cobColorRamp_SelectionChanged;
                cobColorRampCount.SelectionChanged += cobColorRampCount_SelectionChanged;

                for (int i = 1; i < 11; i++)
                {
                    cobTileCount.Items.Add(new ComboBoxItem() { Content = i.ToString() });
                }
                cobTileCount.SelectedIndex = 2;
                cobTileCount.SelectionChanged += this.cobTileCount_SelectionChanged;
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

        private void cobColorRamp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_Render != null)
            {
                int rampId = 14;
                int.TryParse((cobColorRamp.SelectedItem as ComboBoxItem).Content.ToString(), out rampId);
                _Render.ColorRampID = rampId - 1;
            }
        }

        private void cobColorRampCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_Render != null)
            {
                int rampNum = 5;
                int.TryParse((cobColorRampCount.SelectedItem as ComboBoxItem).Content.ToString(), out rampNum);
                _Render.ColourRampCount = rampNum;
            }
        }

        private void sliderOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (viewModel != null && _Render != null)
            {
                _Render.Opacity = (int)sliderOpacity.Value;
            }
        }
        private void CobClassMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (viewModel != null && _Render != null)
            {
                if (cobClassMethod.SelectedIndex == 0)
                    _Render.ClassificationMethod = Core.Drawing.ClassificationMethod.Natural_Breaks_Jenks;
                else if (cobClassMethod.SelectedIndex == 1)
                    _Render.ClassificationMethod = Core.Drawing.ClassificationMethod.Equal_Inteval;

            }
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
                    vector.Style.HeadSize = double.Parse((cobVectorArrow.SelectedItem as ComboBoxItem).Tag.ToString());
                }
            }
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

                    vector.Style.TileCount = int.Parse((cobTileCount.SelectedItem as ComboBoxItem).Content.ToString());
                }
            }
        }

    }
}
