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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

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
