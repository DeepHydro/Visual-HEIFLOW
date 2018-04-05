// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Editors
{
    public class SaveFileNameEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null || context.Instance == null || provider == null)
            {
                return base.EditValue(context, provider, value);
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                if (value != null)
                {
                    saveFileDialog.FileName = value.ToString();
                }

                saveFileDialog.Title = context.PropertyDescriptor.DisplayName;
                saveFileDialog.Filter = "All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    value = saveFileDialog.FileName;
                }
            }

            return value;
        }
    }
}
