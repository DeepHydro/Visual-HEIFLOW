using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    //Specifies the size of a RibbonControl within a RibbonGroup
    public enum RibbonSize
    {

        /// <summary>
        /// the control appears with the large image and text.
        /// </summary>
        Large=3,

        /// <summary>
        /// the control appears with the small image and text.
        /// </summary>
        Medium=2,

        /// <summary>
        /// the control appears with the small image only.
        /// </summary>
        Small=1,

        /// <summary>
        /// the control appears collapsed.
        /// </summary>
        Minimized=0
    }
}
