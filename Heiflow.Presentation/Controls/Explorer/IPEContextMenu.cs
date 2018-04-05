// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
