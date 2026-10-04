using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Collections;
using System.Diagnostics;
using Odyssey.Controls.Ribbon.Interfaces;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    public class RibbonWrapPanel : Panel
    {
        static RibbonWrapPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonWrapPanel), new FrameworkPropertyMetadata(typeof(RibbonWrapPanel)));
        }


        const double smallHeight = 24;


        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement e in Children) e.Measure(infiniteSize);
            return ArrangeOrMeasure(false);
        }


        private static Size infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        protected override Size ArrangeOverride(Size finalSize)
        {
            return ArrangeOrMeasure(true);
        }

        private Size ArrangeOrMeasure(bool arrange)
        {
            double left = 0;
            int rowIndex = 0;
            int maxRows = 3;

            List<UIElement> rowElements = new List<UIElement>(maxRows);

            foreach (UIElement e in Children)
            {
                if (e.Visibility != Visibility.Visible) continue;
                IRibbonControl ribbonControl = e as IRibbonControl;
                Size dsize = e.DesiredSize;
                if (dsize.Height > smallHeight)
                {
                    if (rowIndex > 0)
                    {
                        left += ArrangeRow(rowElements, left, arrange);
                        rowIndex = 0;
                    }
                    if (arrange)
                    {
                        Size size = e.DesiredSize;
                        double h = Math.Max(smallHeight, size.Height);
                        e.Arrange(new Rect(left, 0, size.Width, h));
                    }
                    left += e.DesiredSize.Width;
                }
                else 
                {
                    RibbonSize size =  RibbonBar.GetSize(e);
                    if (size != RibbonSize.Minimized)
                    {
                        rowElements.Add(e);
                        if (++rowIndex == maxRows)
                        {
                            left += ArrangeRow(rowElements, left, arrange);
                            rowIndex = 0;
                        }
                    }
                }
            }
            left += ArrangeRow(rowElements, left, arrange);

            left = Math.Max(32, left);
            return new Size(left, smallHeight * 3);
        }


        protected double ArrangeRow(List<UIElement> rowElements, double left, bool arrange)
        {
            double max = 0;

            double rowHeight = smallHeight + (rowElements.Count == 2 ? (smallHeight / 3) : 0);
            double topOffset = rowElements.Count == 2 ? smallHeight / 3 : 0;

            foreach (UIElement e in rowElements)
            {
                max = Math.Max(e.DesiredSize.Width, max);
            }
            foreach (UIElement e in rowElements)
            {
                if (arrange)
                {
                    double h = Math.Max(smallHeight, e.DesiredSize.Height);
                    double w = e is IRibbonStretch ? max : e.DesiredSize.Width;
                    e.Arrange(new Rect(left, topOffset, w, h));
                }
                topOffset += rowHeight;
            }
            rowElements.Clear();
            return max;
        }

        //protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        //{
        //    //            return new UIElementCollection(this, (base.TemplatedParent == null) ? logicalParent : null);
        //    return new UIElementCollection(this, base.TemplatedParent as FrameworkElement);
        //}

 



    }
}
