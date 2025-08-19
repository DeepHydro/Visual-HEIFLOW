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

using Heiflow.Models.Surface.PRMS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Climate
{
    public partial class WeatherGeneratorForm : Form
    {
        private ClimateDataPackage _ClimateDataPackage;
        public WeatherGeneratorForm(ClimateDataPackage climate)
        {
            InitializeComponent();
            _ClimateDataPackage = climate;
        }

        private void WeatherGeneratorForm_Load(object sender, EventArgs e)
        {
            if (_ClimateDataPackage.MasterPackage.ClimateInputFormat == Models.Generic.FileFormat.Binary)
                cmbClimateFormat.SelectedIndex = 0;
            else
                cmbClimateFormat.SelectedIndex = 1;

            dateTimePickerStart.Value = _ClimateDataPackage.TimeService.Start;
            dateTimePickerEnd.Value = _ClimateDataPackage.TimeService.End;
            tbPpt.Text = _ClimateDataPackage.MasterPackage.PrecipitationFile;
            tbTMax.Text = _ClimateDataPackage.MasterPackage.TempMaxFile;
            tbTMin.Text = _ClimateDataPackage.MasterPackage.TempMinFile;
            tbWind.Text = _ClimateDataPackage.MasterPackage.WindFile;
            tbHumidity.Text = _ClimateDataPackage.MasterPackage.HumidityFile;
            tbPressure.Text = _ClimateDataPackage.MasterPackage.PressureFile;

            if (_ClimateDataPackage.MasterPackage.PotentialET == Models.Integration.PETModule.climate_hru)
                tbPet.Text = _ClimateDataPackage.MasterPackage.PETFile;
            else
                tbPet.Text = "Disable";

            cmbMethod.SelectedIndex = 0;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            float ppt=0.15f;
            float tmax=15;
            float tmin = 5;
            float pet = 0.1f;
            float wind = 4;
            float hum = 0.7f;
            float press = 101;
            float.TryParse(tbPptvalue.Text, out ppt);
            float.TryParse(tbMaxTval.Text, out tmax);
            float.TryParse(tbMinTval.Text, out tmin);
            float.TryParse(tbPetval.Text, out pet);
            float.TryParse(tbWind.Text, out wind);
            float.TryParse(labelhum.Text, out hum);
            float.TryParse(labelPressure.Text, out press);
            Cursor.Current = Cursors.WaitCursor;
            if (cmbClimateFormat.SelectedIndex == 0)
            {
                _ClimateDataPackage.SaveAsDcxByConstant(ppt, tmax, tmin, pet, wind, hum, press, checkBoxOneGrid.Checked);
            }
            else
            {
                _ClimateDataPackage.SaveAsTxtByConstant(ppt, tmax, tmin, pet, wind, hum, press);
            }
            Cursor.Current = Cursors.Default;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbMethod.SelectedIndex == 0)
            {
                tabControl1.SelectedTab = tabUniform;
            }
            else if (cmbMethod.SelectedIndex == 0)
            {
                tabControl1.SelectedTab = tabRandom;
            }
        }
    }
}
