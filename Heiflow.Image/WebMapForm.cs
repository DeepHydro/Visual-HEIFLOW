// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using GMap.NET;
using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Image
{
    public partial class WebMapForm : Form
    {
        private GMapProvider _GMapProvider;

        public WebMapForm()
        {
            InitializeComponent();
            this.comboBoxMapType.DataSource = GMapProviders.List;
            comboBoxMapType.ValueMember = "Name";
        }
     
        private void button1_Click(object sender, EventArgs e)
        {
            GPoint gp=new GPoint(397,190);
            var ms= _GMapProvider.GetTileImage(gp, 9);
            //img.Data
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms.Data);
            img.Save("c:\\img.png");
        }

        private void comboBoxMapType_DropDownClosed(object sender, EventArgs e)
        {
          _GMapProvider=  comboBoxMapType.SelectedItem as GMapProvider;
        }
    }
}
