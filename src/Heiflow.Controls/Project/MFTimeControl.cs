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
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

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
    public partial class MFTimeControl : Form
    {
        private TimeService _MFTimeService;

        public MFTimeControl(TimeService mf)
        {
            InitializeComponent();
            _MFTimeService = mf;
            this.tabControl1.SelectedTab = this.tabPageMF;
            cmbTimeUnit.SelectedIndex = 3;
        }

        private void HeiflowTimeControl_Load(object sender, EventArgs e)
        {
            olvMF.SetObjects(_MFTimeService.StressPeriods);

            if (_MFTimeService.StressPeriods.Count > 0)
            {
                numericUpDownMF.ValueChanged -= this.numericUpDownMF_ValueChanged;
                numericUpDownMF.Value = _MFTimeService.StressPeriods.Count;
                numericUpDownMF.ValueChanged += this.numericUpDownMF_ValueChanged;
            }
            else
            {
                numericUpDownMF.Value = 2;
            }
        }
        private void btnCreateGlobalTime_Click(object sender, EventArgs e)
        {
            (_MFTimeService.Model as Heiflow.Models.Subsurface.Modflow).TimeUnit = cmbTimeUnit.SelectedIndex + 1;
            _MFTimeService.Start = dateTimePickerStart.Value;
            numericUpDownMF_ValueChanged(null, null);
        }

        private void dateTimePickerStart_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cmbTimeUnit_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #region MF

        private void rbtnMFNum_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownMF.Enabled = rbtnMFNum.Checked;
        }
        private void numericUpDownMF_ValueChanged(object sender, EventArgs e)
        {
            int nsp = (int)numericUpDownMF.Value;
            _MFTimeService.CreateSP(nsp, chbHasSteadystate.Checked, dateTimePickerStart.Value);
            _MFTimeService.PopulateTimelineFromSP(dateTimePickerStart.Value);
            _MFTimeService.PopulateIOTimelineFromSP();
            olvMF.SetObjects(_MFTimeService.StressPeriods);
        }

        #endregion

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            _MFTimeService.PopulateTimelineFromSP(dateTimePickerStart.Value);
            //_MFTimeService.PopulateIOTimelineFromSP();
            //_MFTimeService.UpdateTimeLine();
            _MFTimeService.RaiseUpdated();
        }
    }
}