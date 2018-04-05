using Heiflow.Models.IO;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Models.Studio
{
    public partial class LicenseForm : Form
    {
        public LicenseForm()
        {
            InitializeComponent();
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            var file = System.IO.Path.Combine(Application.StartupPath, "vgs.dll");
            SecurityFile _SecurityFile = new SecurityFile(file);

            if (File.Exists(file))
            {
                string key = string.Format("{0}-{1}-{2}-{3}", tb1.Text, tb2.Text, tb3.Text, tb4.Text);
                string code = _SecurityFile.Convert(key);
                _SecurityFile.Read();
                if (code == _SecurityFile.AuthenticationCode)
                {
                    _SecurityFile.Date = DateTime.Now;
                    _SecurityFile.Authenticated = true;
                    _SecurityFile.Activate("VHF");
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("无效的序列号!", "许可",  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("许可文件缺失，程序无法启动!", "许可", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
