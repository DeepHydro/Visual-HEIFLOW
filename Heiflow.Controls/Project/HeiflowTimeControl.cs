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
using Heiflow.Controls.WinForm.Controls;
using Heiflow.Models.Generic;
using Heiflow.Core;

namespace Heiflow.Controls.WinForm.Project
{
    public partial class HeiflowTimeControl : Form
    {
        private TimeService _GlobalTimeService;
        private TimeService _MFTimeService;
        private bool _needrefresh;
        private bool _needCorrectMF;
        private bool _needCorrectLU;

        public HeiflowTimeControl(TimeService global, TimeService mf)
        {
            InitializeComponent();
            _GlobalTimeService = global;
            _MFTimeService = mf;
            this.tabControl1.SelectedTab = this.tabPageMF;
        }

        private void HeiflowTimeControl_Load(object sender, EventArgs e)
        {
            dateTimePickerStart.Value = _GlobalTimeService.Start;
            dateTimePickerEnd.Value = _GlobalTimeService.End;
            cmbTimeUnit.SelectedIndex = _GlobalTimeService.TimeUnit - 3;
            olvLanduse.SetObjects(_GlobalTimeService.StressPeriods);
            olvMF.SetObjects(_MFTimeService.StressPeriods);

            if(_MFTimeService.StressPeriods.Count > 0)
            {
                numericUpDownMF.Value = _MFTimeService.StressPeriods.Count;
            }
            else
            {
                numericUpDownMF.Value = 2;
            }

            chbLanduse.CheckedChanged -= this.chbLanduse_CheckedChanged;
            chbLanduse.Checked = _GlobalTimeService.UseStressPeriods;
            panelLU.Enabled = chbLanduse.Checked;
            if (_GlobalTimeService.UseStressPeriods)
            {
                numericUpDownLU.ValueChanged -= this.numericUpDownLU_ValueChanged;
                numericUpDownLU.Value = _GlobalTimeService.StressPeriods.Count;
                numericUpDownLU.ValueChanged += this.numericUpDownLU_ValueChanged;
            }
            chbLanduse.CheckedChanged += this.chbLanduse_CheckedChanged;

            _needrefresh = false;
            _needCorrectMF = false;
            _needCorrectLU = false;
        }
        private void btnCreateGlobalTime_Click(object sender, EventArgs e)
        {
            _GlobalTimeService.TimeUnit = cmbTimeUnit.SelectedIndex + 3;
            _GlobalTimeService.Start = dateTimePickerStart.Value;
            _GlobalTimeService.End = dateTimePickerEnd.Value;
            _MFTimeService.TimeUnit = cmbTimeUnit.SelectedIndex + 3;
            _MFTimeService.Start = dateTimePickerStart.Value;
            _MFTimeService.End = dateTimePickerEnd.Value;
            _GlobalTimeService.UpdateTimeLine();
            _GlobalTimeService.IOTimeline = _GlobalTimeService.Timeline;

            _MFTimeService.Timeline = _GlobalTimeService.Timeline;
            tbTimeNums.Text = _GlobalTimeService.NumTimeStep.ToString();

            if (rbtnMFNum.Checked)
                numericUpDownMF_ValueChanged(null, null);
            if (rbtnMFTime.Checked)
                cmbMFSPUnit_SelectedIndexChanged(null, null);
            if(chbLanduse.Checked)
            {
                if (rbtnMFNum.Checked)
                    numericUpDownLU_ValueChanged(null, null);
                if (rbtnLUTime.Checked)
                    cmbLUTimeUnit_SelectedIndexChanged(null, null);
            }

            _needrefresh = false;
        }

        private void dateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {
            _needrefresh = true;
        }

        private void cmbTimeUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            _needrefresh = true;
        }

        #region MF

        private void rbtnMFTime_CheckedChanged(object sender, EventArgs e)
        {
            cmbMFSPUnit.Enabled = rbtnMFTime.Checked;
        }

        private void rbtnMFNum_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownMF.Enabled = rbtnMFNum.Checked;
        }
        private void numericUpDownMF_ValueChanged(object sender, EventArgs e)
        {
            if (_MFTimeService.Timeline.Count == 0)
            {
                MessageBox.Show("You should click Generate at first!", "Time", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int nsp = (int)numericUpDownMF.Value - 1;
            _MFTimeService.InitSP(nsp, true);
            olvMF.SetObjects(_MFTimeService.StressPeriods);
        }
        private void cmbMFSPUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_MFTimeService.Timeline.Count == 0)
            {
                MessageBox.Show("You should click Generate at first!", "Time", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var unit = (TimeUnits)(cmbMFSPUnit.SelectedIndex + 104);
            _MFTimeService.InitSP(unit, true);
            olvMF.SetObjects(_MFTimeService.StressPeriods);
        }

        private void btnRefreshMF_Click(object sender, EventArgs e)
        {
            var buf = from ss in _MFTimeService.StressPeriods where ss.State != Models.Subsurface.ModelState.SS select ss.NumTimeSteps;
            if(buf.Sum() != _MFTimeService.NumTimeStep)
            {
                MessageBox.Show("The sum of the Num Time Steps is not equal to the to total time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _needCorrectMF = true;
            }
            else
            {
                _MFTimeService.InitSP(buf.ToArray(), true);
                _MFTimeService.UpdateStressPeriodTimeLine();
                olvMF.SetObjects(_MFTimeService.StressPeriods);
                _needCorrectMF = false;
            }
        }
        #endregion

        #region LU
        private void chbLanduse_CheckedChanged(object sender, EventArgs e)
        {
            panelLU.Enabled = chbLanduse.Checked;
            _GlobalTimeService.UseStressPeriods = chbLanduse.Checked;
        }


        private void rbtnLUNum_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownLU.Enabled = rbtnLUNum.Checked;
        }

        private void rbtnLUTime_CheckedChanged(object sender, EventArgs e)
        {
            cmbLUTimeUnit.Enabled = rbtnLUTime.Checked;
        }

        private void numericUpDownLU_ValueChanged(object sender, EventArgs e)
        {
            int nsp = (int)numericUpDownLU.Value;
            _GlobalTimeService.InitSP(nsp, false);
            olvLanduse.SetObjects(_GlobalTimeService.StressPeriods);
        }

        private void cmbLUTimeUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            var unit = (TimeUnits)(cmbMFSPUnit.SelectedIndex + 104);
            _GlobalTimeService.InitSP(unit, true);
            olvLanduse.SetObjects(_GlobalTimeService.StressPeriods);
        }

        private void btnRefreshLU_Click(object sender, EventArgs e)
        {
            if (_GlobalTimeService.StressPeriods.Count == 0)
            {
                numericUpDownLU_ValueChanged(null, null);
                return;
            }

            var buf = from ss in _GlobalTimeService.StressPeriods where ss.State != Models.Subsurface.ModelState.SS select ss.NumTimeSteps;
            if (buf.Sum() != _GlobalTimeService.NumTimeStep)
            {
                MessageBox.Show("The sum of the Num Time Steps is not equal to the to total time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _needCorrectLU = true;
            }
            else
            {
                _GlobalTimeService.InitSP(buf.ToArray(), true);
                olvLanduse.SetObjects(_GlobalTimeService.StressPeriods);
                _needCorrectLU = false;
            }
        }
        #endregion

        private void btnOk_Click(object sender, EventArgs e)
        {
            if(_GlobalTimeService.Timeline.Count == 0 || _MFTimeService.StressPeriods.Count == 0)
            {
                MessageBox.Show("Need to generate stress periods at first", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_needrefresh)
            {
                MessageBox.Show("Need to refresh global model time", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_needCorrectMF)
            {
                MessageBox.Show("Need to correct Modflow stress periods time", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_needCorrectLU)
            {
                MessageBox.Show("Need to correct Land Use stress periods time", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //if (_needrefresh)
            //{
                _GlobalTimeService.UpdateTimeLine();
                _MFTimeService.UpdateTimeLine();       
                _GlobalTimeService.RaiseUpdated();
                _MFTimeService.RaiseUpdated();
            //}
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
