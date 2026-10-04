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
using Odyssey.Controls.Ribbon.Interfaces;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{

    [ContentProperty("Items")]
    public class RibbonButtonGroup : ItemsControl
    {
        static RibbonButtonGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonButtonGroup), new FrameworkPropertyMetadata(typeof(RibbonButtonGroup)));
        }


        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RibbonButtonGroup), new UIPropertyMetadata(new CornerRadius(3)));



        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            int max = Items.Count-1;
            CornerRadius r = CornerRadius;
            for (int i = 0; i <= max; i++)
            {
                IRibbonButton b = Items[i] as IRibbonButton;
                if (i == 0 && i == max) b.CornerRadius = CornerRadius;
                else if (i == 0) b.CornerRadius = new CornerRadius(r.TopLeft, 0d, 0d, r.BottomLeft);
                else if (i == max) b.CornerRadius = new CornerRadius(0d, r.TopRight, r.BottomRight, 0d);
                else b.CornerRadius = new CornerRadius(0);
            }
        }        


        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonButton();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            IRibbonButton b = element as IRibbonButton;
            if (b != null)
            {
                RibbonBar.SetSize(element, RibbonSize.Small);
            }
        }
    }
}
