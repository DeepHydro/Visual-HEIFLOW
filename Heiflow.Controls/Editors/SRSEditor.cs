// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Projections;
using DotSpatial.Projections.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Controls.WinForm.Editors
{
    public class SRSEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));
            if (svc != null)
            {
                ProjectionSelectDialog form = new ProjectionSelectDialog();
                if(form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    value = form.SelectedCoordinateSystem;
                }
                else
                {
                    value = KnownCoordinateSystems.Geographic.World.WGS1984;
                }
            }
            return value;
        }
    }
}
