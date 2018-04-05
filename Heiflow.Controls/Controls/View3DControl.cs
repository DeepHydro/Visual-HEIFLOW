// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;
using ILNumerics;
using Heiflow.Presentation.Controls.Project;
using System.ComponentModel.Composition;
using Heiflow.Presentation.Controls;
using Heiflow.Applications;
using Heiflow.Applications.Services;
using Heiflow.Presentation.Services;
using Heiflow.Core.Data;

namespace Heiflow.Controls.WinForm.Controls
{
 
    public partial class View3DControl : UserControl
    {
        private ILScene _ILScene;
        private ILPlotCube _PlotCube;
        public View3DControl()
        {
            InitializeComponent();
            var cm = new ContextMenu();
            cm.MenuItems.Add(new MenuItem("Clear", Clear_Clicked));
            this.ilPanel1.ContextMenu = cm;
            _ILScene = new ILScene();
            _PlotCube = new ILPlotCube(twoDMode: false);
            _ILScene.Add(_PlotCube);
            ilPanel1.Scene = _ILScene;
            cmbColorMap.SelectedIndex = 9;
            checkedListBox1.Items.Clear();
            ((ListBox)this.checkedListBox1).DisplayMember = "ID";
            cmbColorMap.SelectedIndexChanged += cmbColorMap_SelectedIndexChanged;
        }

        public void PlotSurface(ILArray<float> array)
        {
            //float max=array.Max();
            float scale = 1;
            float.TryParse(cmbZScale.Text, out scale);
      
            if(btnRemoveExisted.Checked)
            {
                _PlotCube.Children.Clear();
            }
         
         //   _PlotCube.Axes.ZAxis.Scale(1, 1, scale);
            var sf = new ILSurface(array)
            {
                Colormap = EnumHelper.FromString<Colormaps>(cmbColorMap.ComboBox.SelectedItem.ToString())
            }; 
             
            sf.Fill.Markable = false;
            sf.Wireframe.Markable = false;
            if (scale != 1)
            {
                var newsf = sf.Scale(1, 1, scale);
                _PlotCube.Add(newsf);
            }
            else
            {
                _PlotCube.Add(sf);
            }
           // _PlotCube.Axes.ZAxis.Max = max;
            PopulateLegend();
            ilPanel1.Refresh();
        }

        public void ClearContent()
        {
            Clear_Clicked(null, null);
        }
        private void Clear_Clicked(object sender, EventArgs e)
        {
            _PlotCube.Children.Clear();
            checkedListBox1.Items.Clear();
            ilPanel1.Refresh();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear_Clicked(null, null);
        }

        private void PopulateLegend()
        {
            checkedListBox1.Items.Clear();
            foreach (var il in _PlotCube.Children)
            {
                if(il is ILSurface)
                    checkedListBox1.Items.Add(il, il.Visible);
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var series = checkedListBox1.Items[e.Index] as ILSurface;
            if (series != null)
                series.Visible = e.NewValue == CheckState.Checked;
            ilPanel1.Refresh();
        }

        private void cmbColorMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var il in _PlotCube.Children)
            {
                if (il is ILSurface)
                    (il as ILSurface).Colormap = EnumHelper.FromString<Colormaps>(cmbColorMap.SelectedItem.ToString());
            }
            ilPanel1.Refresh();
        }

        private void btnShowSeries_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
            btnShowSeries.Checked = splitContainer1.Panel2Collapsed;
        }
    }
}
