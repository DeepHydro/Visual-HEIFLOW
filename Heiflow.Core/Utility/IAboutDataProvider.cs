// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core
{
    /// <summary>
    /// This interface describes the data that's used to populate the WpfAboutBox.
    /// The properties correspond to fields that are shown in the About dialog. 
    /// Multiple providers can implement this interface to surface this data differently 
    /// to the About dialog.
    /// </summary>
    public interface IAboutDataProvider
    {
        /// <summary>
        /// Gets the title property, which is display in the About dialogs window title.
        /// </summary>
        string Title
        {
            get;
        }

        /// <summary>
        /// Gets the application's version information to show.
        /// </summary>
        string Version
        {
            get;
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        string Product
        {
            get;
        }

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        string Copyright
        {
            get;
        }

        /// <summary>
        /// Gets the product's company name.
        /// </summary>
        string Company
        {
            get;
        }

        /// <summary>
        /// Gets the link text to display in the About dialog.
        /// </summary>
        string LinkText
        {
            get;
        }

        /// <summary>
        /// Gets the link uri that is the navigation target of the link.
        /// </summary>
        string LinkUri
        {
            get;
        }
    }
}
