using Heiflow.Applications;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Controls
{
    [Export(typeof(IProfileView))]
    public partial class Profile_Viewer : Form, IProfileView
    {
        public Profile_Viewer()
        {
            InitializeComponent();
            cmbPackages.DisplayMember = "Name";
            listBox_Props.DisplayMember = "Name";
            this.FormClosing += Profile_Viewer_FormClosing;
        }
        private IPackage _package;

        private void Profile_Viewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }


        private void Profile_Viewer_Load(object sender, EventArgs e)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var pcks = prj.Project.Model.GetPackages();
            int layer_count = prj.Project.Model.Grid.ActualLayerCount;
            var cov_pcks = new List<IPackage>();
            labelStatus.Text = "Ready";
            foreach (var pck in pcks)
            {
                var atr = pck.GetType().GetCustomAttributes(typeof(CoverageItem), true);
                if (atr.Length == 1)
                {
                    cov_pcks.Add(pck);
                }
            }
            cmbPackages.DataSource = cov_pcks;
            listBox_Props.Items.Clear();

            var _MFGrid = prj.Project.Model.Grid as MFGrid;
            int[] row = new int[_MFGrid.RowCount];
            int[] col = new int[_MFGrid.ColumnCount];
            for (int i = 0; i < _MFGrid.RowCount; i++)
            {
                row[i] = i + 1;
            }
            for (int i = 0; i < _MFGrid.ColumnCount; i++)
            {
                col[i] = i + 1;
            }
            comboBoxRows.DataSource = row;
            comboBoxCols.DataSource = col;
        }

        private void listBox_Props_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            //if the item state is selected them change the back color 
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          e.ForeColor,
                                          Color.Yellow);//Choose the color

            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Draw the current item text
            e.Graphics.DrawString((listBox_Props.Items[e.Index] as DataCubeItem).Name, e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

        private void listBox_Props_SelectedIndexChanged(object sender, EventArgs e)
        {
            float[][] mat = null;
            if (radioButtonRow.Checked)
            {
                if (comboBoxRows.SelectedIndex < 0)
                {
                    MessageBox.Show("Selecte an ROW please.", "ROW selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var item = listBox_Props.SelectedItem as DataCubeItem;
                mat = item.DataCube.GetRowProfile(comboBoxRows.SelectedIndex);
            }
            if (radioButtonCol.Checked) 
            {
                if (comboBoxRows.SelectedIndex < 0)
                {
                    MessageBox.Show("Selecte an COLUMN please.", "ROW selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var item = listBox_Props.SelectedItem as DataCubeItem;
                mat = item.DataCube.GetComunProfile(comboBoxCols.SelectedIndex); 
            }
            if (mat != null)
            {
                winChart1.Clear();
                if(winChart1.ClearExistesSeries)
                    winChart1.ClearExistesSeries = false;
                var nvar = mat.GetLength(0);
                for (int i = 0; i < nvar; i++)
                {
                    winChart1.Plot<float>(mat[i], "Layer " + (i + 1), System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine);
                }
            }
        }

        private void cmbPackages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPackages.SelectedItem == null)
                return;

            listBox_Props.Items.Clear(); 
            _package = cmbPackages.SelectedItem as IPackage;
            var props = _package.GetType().GetProperties();
            foreach (var pr in props)
            {
                var atr = pr.GetCustomAttributes(typeof(StaticVariableItem), false);
                if (atr.Length == 1)
                {
                    var dc = pr.GetValue(_package) as DataCube<float>;
                    if (dc!= null && dc.Size[1] == 1)
                    {
                        listBox_Props.Items.Add(new DataCubeItem() { Name = dc.Name, DataCube = dc });
                    }
                }
            }
        }
        private class DataCubeItem
        {
            public DataCubeItem()
            {

            }

            public string Name
            {
                get;
                set;
            }

            public IDataCubeObject DataCube
            {
                get;
                set;
            }
        }

        private void radioButtonRow_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRows.Enabled = radioButtonRow.Checked;
            comboBoxCols.Enabled = radioButtonCol.Checked; 
        }

        private void radioButtonCol_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRows.Enabled = radioButtonRow.Checked;
            comboBoxCols.Enabled = radioButtonCol.Checked;
        }

        public string ChildName
        {
            get { return ChildWindowNames.ProfileViewer; }
        }


        public void ShowView(IWin32Window pararent)
        {
            if (!this.Visible)
                this.Show(pararent);
        }

        public void ClearContent()
        {
            winChart1.Clear();
            listBox_Props.Items.Clear();
            comboBoxCols.Items.Clear();
            comboBoxRows.Items.Clear();
        }

        public void InitService()
        {
          
        }
    }


}
