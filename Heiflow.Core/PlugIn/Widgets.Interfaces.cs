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
using System.Windows.Forms;
using Heiflow.Core.Plugin;


namespace Heiflow.Core.Plugin
{
    /// <summary>
    /// Interface must be implemented in order to recieve user input.  Can be used by IRenderables and IWidgets.
    /// </summary>
    public interface IInteractive
    {
        #region Methods
        bool OnKeyDown(KeyEventArgs e);

        bool OnKeyUp(KeyEventArgs e);

        bool OnKeyPress(KeyPressEventArgs e);

        bool OnMouseDown(MouseEventArgs e);

        bool OnMouseEnter(EventArgs e);

        bool OnMouseLeave(EventArgs e);

        bool OnMouseMove(MouseEventArgs e);

        bool OnMouseUp(MouseEventArgs e);

        bool OnMouseWheel(MouseEventArgs e);
        #endregion
    }

    /// <summary>
    /// Base Interface for DirectX GUI Widgets
    /// </summary>
    public interface IWidget
    {
        #region Methods
        void Render(IDrawArgs drawArgs);
        #endregion

        #region Properties
        IWidgetCollection ChildWidgets { get; set; }
        IWidget ParentWidget { get; set; }
        System.Drawing.Point AbsoluteLocation { get; }
        System.Drawing.Point ClientLocation { get; set; }
        System.Drawing.Size ClientSize { get; set; }
        bool Enabled { get; set; }
        bool Visible { get; set; }
        object Tag { get; set; }
        string Name { get; set; }
        #endregion
    }

    /// <summary>
    /// Collection of IWidgets
    /// </summary>
    public interface IWidgetCollection
    {
        #region Methods
        void BringToFront(int index);
        void BringToFront(IWidget widget);
        void Add(IWidget widget);
        void Clear();
        void Insert(IWidget widget, int index);
        IWidget RemoveAt(int index);
        #endregion

        #region Properties
        int Count { get; }
        #endregion

        #region Indexers
        IWidget this[int index] { get; set; }
        #endregion

    }
}
