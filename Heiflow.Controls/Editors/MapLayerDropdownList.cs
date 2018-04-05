// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Heiflow.Controls.WinForm.Editors
{
    public class MapLayerDropdownList : UITypeEditor
    {
        private IWindowsFormsEditorService _editorService;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            _editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            // use a list box
            ListBox lb = new ListBox();
            lb.SelectionMode = SelectionMode.One;
            lb.SelectedValueChanged += OnListBoxSelectedValueChanged;

            // use the IBenchmark.Name property for list box display
            lb.DisplayMember = "LegendText";

            // get the analytic object from context
            // this is how we get the list of possible benchmarks
            var tool = context.Instance as IMapLayerRequiredTool;
            tool.Setup();
            // lb.DataSource = tool.MapLayers;

            if (tool.MapLayers != null)
            {
                foreach (var layer in tool.MapLayers)
                {
                    // we store benchmarks objects directly in the listbox
                    int index = lb.Items.Add(layer);
                    if (layer.Equals(value))
                    {
                        lb.SelectedIndex = index;
                    }
                }
            }

            // show this model stuff
            _editorService.DropDownControl(lb);
            if (lb.SelectedItem == null) // no selection, return the passed-in value as is
                return value;

            return lb.SelectedItem;
        }

        private void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            // close the drop down as soon as something is clicked
            _editorService.CloseDropDown();
        }
    }
}
