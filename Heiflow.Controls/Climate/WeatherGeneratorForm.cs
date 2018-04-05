// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
            dateTimePickerStart.Value = _ClimateDataPackage.TimeService.Start;
            dateTimePickerEnd.Value = _ClimateDataPackage.TimeService.End;
            tbPet.Text = _ClimateDataPackage.MasterPackage.PETFile;
            tbPpt.Text = _ClimateDataPackage.MasterPackage.PrecipitationFile;
            tbTMax.Text = _ClimateDataPackage.MasterPackage.TempMaxFile;
            tbTMin.Text = _ClimateDataPackage.MasterPackage.TempMinFile;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            float ppt=0.15f;
            float tmax=15;
            float tmin = 5;
            float pet = 0.1f;
            float.TryParse(tbPptvalue.Text, out ppt);
            float.TryParse(tbMaxTval.Text, out tmax);
            float.TryParse(tbMinTval.Text, out tmin);
            float.TryParse(tbPetval.Text, out pet);
            Cursor.Current = Cursors.WaitCursor;
            _ClimateDataPackage.Constant(ppt, tmax, tmin, pet);
            Cursor.Current = Cursors.Default;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
