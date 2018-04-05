// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Controls
{
    public  class MyListView : ListView
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x201 || m.Msg == 0x203)
            {  // Trap WM_LBUTTONDOWN + double click
                var pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                var loc = this.HitTest(pos);
                switch (loc.Location)
                {
                    case ListViewHitTestLocations.None:
                    case ListViewHitTestLocations.AboveClientArea:
                    case ListViewHitTestLocations.BelowClientArea:
                    case ListViewHitTestLocations.LeftOfClientArea:
                    case ListViewHitTestLocations.RightOfClientArea:
                        return;  // Don't let the native control see it
                }
            }
            base.WndProc(ref m);
        }
    }
}
