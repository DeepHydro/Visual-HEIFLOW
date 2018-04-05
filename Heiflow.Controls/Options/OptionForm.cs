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
