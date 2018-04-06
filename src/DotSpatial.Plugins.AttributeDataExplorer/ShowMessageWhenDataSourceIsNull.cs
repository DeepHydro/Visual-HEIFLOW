// -----------------------------------------------------------------------
// <copyright file="ShowMessageWhenDataSourceIsNull.cs" company="">
//   (c) 2011; Released under Microsoft Public License (Ms-PL)
// </copyright>
// -----------------------------------------------------------------------

namespace DotSpatial.Plugins.AttributeDataExplorer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using DevExpress.Data.Filtering;
    using DevExpress.Utils;
    using DevExpress.XtraGrid;
    using DevExpress.XtraGrid.Views.Base;
    using DevExpress.XtraGrid.Views.Grid;
    using DevExpress.XtraGrid.Views.Grid.ViewInfo;

    public class ShowMessageWhenDataSourceIsNull
    {
        private const string NoData = "Select a feature layer to examine attribute data.";

        //http://devexpress.com/Support/Center/p/K18292.aspx
        private GridView _ActiveView;

        public GridView ActiveView
        {
            get { return _ActiveView; }
            set { _ActiveView = value; }
        }

        public ShowMessageWhenDataSourceIsNull(GridView view)
        {
            ActiveView = view;
            SubscribeEvents();
        }

        private Font _Font;

        public Font PaintFont
        {
            get { return _Font == null ? AppearanceObject.DefaultFont : _Font; }
            set { _Font = value; }
        }

        public GridControl ActiveGridControl
        {
            get { return ActiveView.GridControl; }
        }

        private void SubscribeEvents()
        {
            ActiveView.CustomDrawEmptyForeground += new CustomDrawEventHandler(ActiveView_CustomDrawEmptyForeground);
        }

        private void ActiveView_CustomDrawEmptyForeground(object sender, CustomDrawEventArgs e)
        {
            DrawNothingToSee(e);
        }

        private Size GetStringSize(string s, Font font)
        {
            Graphics g = Graphics.FromHwnd(ActiveGridControl.Handle);
            return g.MeasureString(s, font).ToSize();
        }

        private Rectangle NothingToSeeBounds(Rectangle bounds)
        {
            Size size = GetStringSize(NoData, PaintFont);
            int x = (bounds.Width - size.Width) / 2;
            int y = bounds.Y + 50;
            return new Rectangle(new Point(x, y), size);
        }

        private Rectangle GetForegroundBounds()
        {
            return (ActiveView.GetViewInfo() as GridViewInfo).ViewRects.Rows;
        }

        private void DrawNothingToSee(CustomDrawEventArgs e)
        {
            if (ActiveGridControl.DataSource == null)
                e.Graphics.DrawString(NoData, PaintFont, Brushes.Gray, NothingToSeeBounds(e.Bounds).Location);
        }
    }
}