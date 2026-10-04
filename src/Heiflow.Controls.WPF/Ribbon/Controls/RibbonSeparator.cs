using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Odyssey.Controls.Ribbon.Interfaces;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    public class RibbonSeparator:Separator,IRibbonControl
    {
        static RibbonSeparator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonSeparator), new FrameworkPropertyMetadata(typeof(RibbonSeparator)));
        }

    }
}
