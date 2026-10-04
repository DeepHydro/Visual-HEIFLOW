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
using Odyssey.Controls.Classes;
using Odyssey.Controls.Ribbon.Interfaces;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    [ContentProperty("Items")]
    public class RibbonFlowGroup : ItemsControl, IRibbonControl, IRibbonLargeControl
    {
        static RibbonFlowGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonFlowGroup), new FrameworkPropertyMetadata(typeof(RibbonFlowGroup)));
        }
    }
}
