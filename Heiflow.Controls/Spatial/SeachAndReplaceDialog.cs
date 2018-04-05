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