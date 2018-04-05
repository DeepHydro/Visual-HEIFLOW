// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data.ODM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.TimeSeriesExplorer
{
    public partial class SQLSelection : Form
    {
        public SQLSelection(string sql)
        {
            InitializeComponent();
            this.tbSQL.Text = sql;
            this.btnOK.Enabled = false;
        }

        [Browsable(false)]
        public ODMSource ODM
        {
            get;
            set;
        }

        public string SQLScript
        {
            get;
            protected set;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var sql = tbSQL.Text;
            if (sql != "" && ODM != null)
            {
                var dt = ODM.Execute(sql);
                if (dt != null)
                {
                    this.bindingSource1.DataSource = dt;
                    this.dataGridView1.DataSource = this.bindingSource1;
                    this.btnOK.Enabled = true;
                    SQLScript = tbSQL.Text;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {            
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void tbSQL_TextChanged(object sender, EventArgs e)
        {
            this.btnOK.Enabled = false;
        }
    }
}
