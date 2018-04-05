// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Spatial
{
    /// <summary>
    /// This form diplay to user to find perticular value or string in DataGridView and replace.
    /// </summary>
    public partial class SeachAndReplaceDialog : Form
    {
        #region Variable

        private string _find;
        private string _replace;

        /// <summary>
        /// get the Find String
        /// </summary>
        public string FindString
        {
            //set { _find = value; }
            get { return _find; }
        }

        /// <summary>
        /// get the ReplaceString
        /// </summary>
        public string ReplaceString
        {
            //set { _replace = value; }
            get { return _replace; }
        }

        #endregion

        /// <summary>
        /// Creates a new instance of the replace form.
        /// </summary>
        public SeachAndReplaceDialog()
        {
            InitializeComponent();
        }

        private void BtnCancelClick(object sender, EventArgs e)
        {
            Hide();
        }

        private void BtnOkClick(object sender, EventArgs e)
        {
            _find = txtFind.Text;
            _replace = txtReplace.Text;
            DialogResult = DialogResult.OK;
        }
    }
}