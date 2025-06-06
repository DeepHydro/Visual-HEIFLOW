﻿using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation.Controls;
using ILNumerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Controls
{
    [Export(typeof(IVerticalProfileView))]
    public partial class GridProfileAnimator : Form, IVerticalProfileView
    {
        private DataCube<float> _datasource;
        private IRegularGrid _MFGrid;
        private int _curStep = 0;
        private bool _isPlaying = false;
        private ILArray<float> _slice;

        public GridProfileAnimator()
        {
            InitializeComponent();
            this.FormClosing += GridProfileViewer_FormClosing;
        }

        public string ChildName
        {
            get { return ChildWindowNames.GridProfileView; }
        }



        public DataCube<float> DataSource
        {
            get
            {
                return _datasource;
            }
            set
            {
                _datasource = value;
                comboBoxVariable.DataSource = _datasource.Variables;
                if (_datasource.Size[1] > 1)
                {
                    colorSlider1.Maximum = _datasource.Size[1] - 1;
                    tbCurDate.Text = "";
                    radioButtonRow.Checked = true;
                    radioButtonMultiVar.Checked = true;
                    cmbDate.DataSource = _datasource.DateTimes;
                }
            }
        }

        public IRegularGrid Grid
        {
            get
            {
                return _MFGrid;
            }
            set
            {
                _MFGrid = value;
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
                colorSlider1.Enabled = false;
                btnPlay.Enabled = false;
                btnStop.Enabled = false;
                radioButtonRow.Checked = true;

                tbCurDate.Text = "";
            }
        }

        private void GridProfileViewer_FormClosing(object sender, FormClosingEventArgs e)
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
        public void ShowView(IWin32Window pararent)
        {
            if (!this.Visible)
                this.Show(pararent);
        }
        private void radioButtonRow_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRows.Enabled = radioButtonRow.Checked;
            comboBoxCols.Enabled = !radioButtonRow.Checked;
        }

        private void radioButtonCol_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxRows.Enabled = !radioButtonCol.Checked;
            comboBoxCols.Enabled = radioButtonCol.Checked;
        }

        private void comboBoxRows_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioButtonSingleVar.Checked)
            {
                var var_index = comboBoxVariable.SelectedIndex;
                if (!_datasource.IsAllocated(var_index))
                {
                    colorSlider1.Enabled = false;
                    MessageBox.Show("Selected variable is not loaded.", "Variable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    colorSlider1.Enabled = true;
                    btnPlay.Enabled = true;
                    btnStop.Enabled = false;
                    comboBoxVariable.Enabled = true;
                    for (int i = 0; i < DataSource.Size[0]; i++)
                    {
                        if (DataSource.IsAllocated(i))
                        {
                            comboBoxVariable.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void comboBoxCols_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioButtonSingleVar.Checked)
            {
                var var_index = comboBoxVariable.SelectedIndex;
                if (!_datasource.IsAllocated(var_index))
                {
                    colorSlider1.Enabled = false;
                    MessageBox.Show("Selected variable is not loaded.", "Variable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    comboBoxVariable.Enabled = true;
                }
            }

            colorSlider1.Enabled = true;
            btnPlay.Enabled = true;
            btnStop.Enabled = false;

            //for (int i = 0; i < DataSource.Size[0]; i++)
            //{
            //    if (DataSource.IsAllocated(i))
            //    {
            //        comboBoxVariable.SelectedIndex = i;
            //        break;
            //    }
            //}
        }

        private void colorSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            UpdateChart(colorSlider1.Value);
        }

        public void ClearContent()
        {
            view3DControl1.ClearContent();
        }
        public void InitService()
        {

        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            _curStep = 0;
            _isPlaying = true;
            btnPlay.Enabled = false;
            btnStop.Enabled = true;
            colorSlider1.Enabled = false;
            timer1.Start();

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            colorSlider1.Scroll -= this.colorSlider1_Scroll;
            colorSlider1.Value = 0;
            _curStep = 0;
            colorSlider1.Scroll += this.colorSlider1_Scroll;
            _isPlaying = false;
            btnPlay.Enabled = true;
            btnStop.Enabled = false;
            colorSlider1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_curStep < DataSource.Size[1])
            {
                colorSlider1.Value = _curStep;
                _curStep++;
            }
            else
            {
                btnStop_Click(btnStop, e);
            }
        }

        private void cmbSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isPlaying)
            {
                timer1.Interval = (5 - cmbSpeed.SelectedIndex) * 50;
            }
        }

        private void GridProfileViewer_Load(object sender, EventArgs e)
        {
            cmbSpeed.SelectedIndex = 4;
        }

        private void colorSlider1_ValueChanged(object sender, EventArgs e)
        {
            colorSlider1_Scroll(null, null);
        }

        private void radioButtonSingleVar_CheckedChanged(object sender, EventArgs e)
        {
            if (sender.Equals(radioButtonSingleVar))
            {
                comboBoxVariable.Enabled = true;
            }
            else
            {
                colorSlider1.Enabled = true;
                btnPlay.Enabled = true;
                btnStop.Enabled = false;
                comboBoxVariable.Enabled = true;
            }
        }

        private void cmbDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateChart(cmbDate.SelectedIndex);
        }

        private void UpdateChart(int time_index)
        {
            if (radioButtonSingleVar.Checked)
            {
                var var_index = comboBoxVariable.SelectedIndex;
                if (var_index < 0)
                {
                    MessageBox.Show("Selecte an variable please.", "Variable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (radioButtonRow.Checked)
                {
                    _slice = DataSource.ToRowVerticalProfileArray(var_index, time_index, comboBoxRows.SelectedIndex) as ILArray<float>;
                    view3DControl1.PlotSurface(_slice);
                }
                if (radioButtonCol.Checked)
                {
                    _slice = DataSource.ToColumnVerticalProfileArray(var_index, time_index, comboBoxCols.SelectedIndex) as ILArray<float>;
                    view3DControl1.PlotSurface(_slice);
                }
            }

            if (radioButtonMultiVar.Checked)
            {
                if (radioButtonRow.Checked)
                {
                    _slice = DataSource.ToRowVerticalProfileArray(time_index, comboBoxRows.SelectedIndex) as ILArray<float>;
                    view3DControl1.PlotSurface(_slice);
                }
                if (radioButtonCol.Checked)
                {
                    _slice = DataSource.ToColumnVerticalProfileArray(time_index, comboBoxCols.SelectedIndex) as ILArray<float>;
                    view3DControl1.PlotSurface(_slice);
                }
            }
            tbCurDate.Text = DataSource.DateTimes[colorSlider1.Value].ToString();
        }

        private void btnExportData_Click(object sender, EventArgs e)
        {
            if (_slice != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "csv file|*.csv";
                dlg.FileName = "cross_section.csv";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var file = dlg.FileName;
                    CSVFileStream csv = new CSVFileStream(file);
                    csv.Save(_slice);
                }
            }
        }
    }
}
