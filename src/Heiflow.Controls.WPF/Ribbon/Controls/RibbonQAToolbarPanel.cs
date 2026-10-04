using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    public class RibbonQAToolbarPanel:StackPanel
    {
        public RibbonQAToolbarPanel()
            : base()
        {
            //Orientation = Orientation.Horizontal;
        }

        /// <summary>
        /// Overriding this to enable a templated parent to add or remove children to this panel within OnMeasureOverride or somewhere else.
        /// </summary>
        protected override UIElementCollection CreateUIElementCollection(System.Windows.FrameworkElement logicalParent)
        {
            return new UIElementCollection(this, (base.TemplatedParent == null) ? logicalParent : null);
        }
    }
}
