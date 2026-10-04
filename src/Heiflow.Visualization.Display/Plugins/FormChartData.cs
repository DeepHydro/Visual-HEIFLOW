using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HUST.WREIS.Dot3D.Display.Plugins
{
    public partial class FormChartData : Form
    {
        public FormChartData()
        {
            InitializeComponent();
        }

        public void BindData(object data)
        {
            dataGridView1.DataSource = bindingSource1;
            bindingSource1.DataSource = data;
          //  bindingSource1.();
        }

        private void FormChartData_Load(object sender, EventArgs e)
        {
            
        }
    }
}
