using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Odyssey.Controls.RibbonBar;

namespace Odyssey.Controls
{
    public interface IRibbonSize
    {
        RibbonSize Appearance { get; set; }

        RibbonSize PreferedAppearance { get; }

        RibbonSize MinAppearance { get; }
    }
}
