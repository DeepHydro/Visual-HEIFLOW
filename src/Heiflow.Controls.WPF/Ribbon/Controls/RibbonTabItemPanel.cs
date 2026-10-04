using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    /// <summary>
    /// Uses to layout tab items and paint the separator if necassary.
    /// </summary>
    internal class RibbonTabItemPanel : Panel
    {

        static Size infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        private double scaleX;

        protected override Size MeasureOverride(Size availableSize)
        {
            double width = 0;
            double height = 0;
            foreach (UIElement e in VisibleChildren)
            {
                e.Measure(infiniteSize);
                height = Math.Max(height, e.DesiredSize.Height);
                width += e.DesiredSize.Width;
            }

            width = Math.Min(width, availableSize.Width);
            double scaleX = CalculateScaleX(new Size(width, availableSize.Height));

            if (this.scaleX != scaleX) InvalidateVisual();
            this.scaleX = scaleX;
            if (scaleX < 1)
            {
                width = 0;
                foreach (UIElement e in VisibleChildren)
                {
                    e.Measure(new Size(e.DesiredSize.Width * scaleX, e.DesiredSize.Height));
                    width += e.DesiredSize.Width;
                }
            }
            return new Size(width, height);
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            double scaleX = CalculateScaleX(finalSize);

            double x = 0;
            double height = finalSize.Height;
            foreach (UIElement e in VisibleChildren)
            {
                double w = e.DesiredSize.Width * scaleX;
                e.Arrange(new Rect(x, 0.0, w, height));
                x += w;
            }
            return new Size(x, height);
        }


        IEnumerable<UIElement> VisibleChildren
        {
            get
            {
                foreach (UIElement e in Children)
                {
                    if (e.Visibility == Visibility.Visible) yield return e;
                }
            }
        }

        private double GetMaxWidth()
        {
            double width = 0;
            foreach (UIElement e in VisibleChildren)
            {
                width += e.DesiredSize.Width;
            }
            return width;
        }

        double CalculateScaleX(Size finalSize)
        {
            double MaxWidth = GetMaxWidth();
            double scaleX = MaxWidth > 0 ? Math.Min(1, finalSize.Width / MaxWidth) : 1.0;
            if (double.IsPositiveInfinity(scaleX)) scaleX = 1.0;
            return scaleX;
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
            double scaleX = this.scaleX;
            if (scaleX < 1d)
            {
                Brush brush = SeparatorBrush.Clone();
                brush.Opacity = 1d - scaleX * 0.8d;
                Pen pen = new Pen(brush, 1);

                double x = 0;
                foreach (UIElement e in Children)
                {
                    if (x > 0)
                    {
                        dc.DrawLine(pen, new Point(x, 2), new Point(x, ActualHeight - 8));
                    }
                    x += e.DesiredSize.Width;
                }
            }
        }



        /// <summary>
        /// Gets or sets the brush that is used to paint the separators between each tab item if
        /// the available width is too small.
        /// </summary>
        public Brush SeparatorBrush
        {
            get { return (Brush)GetValue(SeparatorBrushProperty); }
            set { SetValue(SeparatorBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SeparatorBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeparatorBrushProperty =
            DependencyProperty.Register("SeparatorBrush", typeof(Brush), typeof(RibbonTabItemPanel), new UIPropertyMetadata(new SolidColorBrush(Colors.SlateBlue)));

    }
}
