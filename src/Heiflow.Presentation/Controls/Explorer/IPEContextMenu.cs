//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using DotSpatial.Data;
using DotSpatial.Symbology;
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Presentation.Controls
{
    public interface IPEContextMenu : IChangeItem
    {
        #region Methods

        /// <summary>
        /// Tests the specified legend item to determine whether or not
        /// it can be dropped into the current item.
        /// </summary>
        /// <param name="item">Any object that implements ILegendItem</param>
        /// <returns>Boolean that is true if a drag-drop of the specified item will be allowed.</returns>
        bool CanReceiveItem(IPEContextMenu item);

        void Initialize();

        void Enable(string itemName, bool enabled);

        void EneableAll(bool enabled);

        void NodeDoubleClick();
        #endregion

        #region Properties

        /// <summary>
        /// This is a list of menu items that should appear for this layer.
        /// These are in addition to any that are supported by default.
        /// Handlers should be added to this list before it is retrieved.
        /// </summary>
        List<ExplorerMenuItem> ContextMenuItems
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether or not this legend item should be visible.
        /// This will not be altered unless the LegendSymbolMode is set
        /// to CheckBox.
        /// </summary>
        bool Checked
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this legend item is expanded.
        /// </summary>
        bool IsExpanded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this legend item is currently selected (and therefore drawn differently)
        /// </summary>
        bool IsSelected
        {
            get;
            set;
        }

        /// <summary>
        /// Gets whatever the child collection is and returns it as an IEnumerable set of legend items
        /// in order to make it easier to cycle through those values.
        /// </summary>
        List<IPEContextMenu> Child
        {
            get;
        }

        /// <summary>
        /// Gets or sets a boolean, that if false will prevent this item, or any of its child items
        /// from appearing in the legend when the legend is drawn.
        /// </summary>
        bool ProjectItemVisible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the symbol mode for this legend item.
        /// </summary>
        SymbolMode LegendSymbolMode
        {
            get;
        }

        /// <summary>
        /// The text that will appear in the legend
        /// </summary>
        string LegendText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a pre-defined behavior in the legend when referring to drag and drop functionality.
        /// </summary>
        LegendType LegendType
        {
            get;
        }

        Type ItemType
        {
            get;
        }

        IExplorerItem ExplorerItem
        {
            get;
            set;
        }
        #endregion
    }
}
