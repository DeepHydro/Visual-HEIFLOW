// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
