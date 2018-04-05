// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
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

namespace Heiflow.Controls.WinForm.Toolbox
{
    public partial class SaveDcxForm : Form
    {
        private My3DMat<float> _Mat;
        public SaveDcxForm(My3DMat<float> mat)
        {
            InitializeComponent();
            _Mat = mat;
            for (int i = 0; i < _Mat.Size[0]; i++)
            {
                checkedListBox1.Items.Add(_Mat.Variables[i], _Mat.Value[i] != null);
            }
        }

        public int[] CheckedIndex { get; protected set; }

        public string FileName { get; protected set; }

        private void SaveDcxForm_Load(object sender, EventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "data cube file|*.dcx";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tb_filename.Text = dlg.FileName;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty( tb_filename.Text))
                return;

            FileName = tb_filename.Text;
            var list = new List<int>();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                    list.Add(i);
            }
            CheckedIndex = list.ToArray();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_Mat.Value[e.Index] == null)
                e.NewValue = CheckState.Unchecked;
        }
    }
}
