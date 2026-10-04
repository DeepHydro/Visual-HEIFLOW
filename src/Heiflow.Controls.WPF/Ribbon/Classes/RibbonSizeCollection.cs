using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Odyssey.Controls.Classes;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    [TypeConverter(typeof(RibbonReductionCollectionConverter))]
    public class RibbonSizeCollection:Collection<RibbonSize>
    {
    }
}
