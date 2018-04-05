// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotSpatial.Plugins.SplashScreenManager
{
    public class SimpleLegendPlugin : Extension
    {
        private Legend legend1;

        public override void Activate()
        {
            ShowLegend();

            var showLengend = new SimpleActionItem("kView", "Lengend",
                delegate(object sender, EventArgs e)
                { App.DockManager.ShowPanel("kLegend"); })
            {
                Key = "kShowLegend",
                ToolTipText = "Show Lengend",
                GroupCaption = "Map",
                LargeImage = Heiflow.Plugins.Default.Properties.Resources.Legend32
            };
            App.HeaderControl.Add(showLengend);

            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.Remove("kShowLegend");
            this.App.DockManager.Remove("kLegend");
            base.Deactivate();
        }

        private void ShowLegend()
        {

            this.legend1 = new DotSpatial.Controls.Legend();
            // 
            // legend1
            // 
            this.legend1.BackColor = System.Drawing.Color.White;
            this.legend1.ControlRectangle = new System.Drawing.Rectangle(0, 0, 176, 128);
            this.legend1.DocumentRectangle = new System.Drawing.Rectangle(0, 0, 34, 114);
            this.legend1.HorizontalScrollEnabled = true;
            this.legend1.Indentation = 30;
            this.legend1.IsInitialized = false;
            this.legend1.Location = new System.Drawing.Point(217, 12);
            this.legend1.MinimumSize = new System.Drawing.Size(5, 5);
            this.legend1.Name = "legend1";
            this.legend1.ProgressHandler = null;
            this.legend1.ResetOnResize = false;
            this.legend1.SelectionFontColor = System.Drawing.Color.Black;
            this.legend1.SelectionHighlight = System.Drawing.Color.FromArgb(((int)(((byte)(215)))), ((int)(((byte)(238)))), ((int)(((byte)(252)))));
            this.legend1.Size = new System.Drawing.Size(176, 128);
            this.legend1.TabIndex = 0;
            this.legend1.Text = "Legend";
            this.legend1.VerticalScrollEnabled = true;

            App.Map.Legend = legend1;
            App.Legend = this.legend1;
            App.DockManager.Add(new DockablePanel("kLegend", "Legend", legend1, DockStyle.Left) { SmallImage = Heiflow.Plugins.Default.Properties.Resources.Legend16 });

        }

    }
}
