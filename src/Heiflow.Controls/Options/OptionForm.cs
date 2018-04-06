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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using Heiflow.Presentation.Controls.Project;
using Heiflow.Presentation.Controls;
using Heiflow.Applications;

namespace Heiflow.Controls.Options
{
    [Export(typeof(IOptionForm))]
    public partial class OptionForm : Form, IOptionForm
    {
        public OptionForm()
        {
            InitializeComponent();
        }

        private IOptionControl _CurrentOption;

        private void OptionForm_Load(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            var config = MyAppManager.Instance.ConfigManager;
            var categories = from optc in config.OptionControls
                             group optc by optc.Category into cat
                             select new { Category = cat.Key, Items = cat.ToArray() };

            foreach (var cat in categories)
            {
                TreeNode tn = new TreeNode(cat.Category);
                foreach (var item in cat.Items)
                {
                    TreeNode optn = new TreeNode(item.OptionName);
                    optn.Tag = item;
                    tn.Nodes.Add(optn);
                }
                treeView1.Nodes.Add(tn);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Node.Tag != null)
            {
                panel1.Controls.Clear();
                _CurrentOption = (e.Node.Tag as IOptionControl);
                var cont = _CurrentOption.OptionControl;
                cont.Dock = DockStyle.Fill;
                panel1.Controls.Add(cont);
            }
            else
            {
                _CurrentOption = null;
                panel1.Controls.Clear();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_CurrentOption != null)
            {
                _CurrentOption.Save();
            }
        }
    }
}
