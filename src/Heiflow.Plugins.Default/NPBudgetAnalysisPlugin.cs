//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
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

using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using Heiflow.Applications;
using Heiflow.Plugins.Default.Properties;
using Heiflow.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Plugins.Default
{
    /// <summary>
    /// 注册 N / P 质量平衡分析面板，读取 sz_N_budget.csv 与 sz_P_budget.csv
    /// </summary>
    public class NPBudgetAnalysisPlugin : Extension
    {
        public static string PanelKey = "kNPBudgetMonitor";
        public const string Caption = "N/P Mass Budget";
        private const string ActionKey = "kShowNPBudgetMonitor";

        private UserControl _NPBudgetMonitor;

        [Import("VHFManager", typeof(VHFAppManager))]
        public VHFAppManager Manager
        {
            get;
            set;
        }

        public override void Activate()
        {
            if (Manager == null || Manager.NPBudgetMonitor == null)
            {
                base.Activate();
                return;
            }

            this._NPBudgetMonitor = Manager.NPBudgetMonitor.ViewModel.View as UserControl;
            if (this._NPBudgetMonitor == null)
            {
                base.Activate();
                return;
            }

            this._NPBudgetMonitor.Name = "npBudgetMonitorView1";
            App.DockManager.Add(new DockablePanel(PanelKey, Caption,
                _NPBudgetMonitor, DockStyle.None) { SmallImage = Resources.UiBalance16 });
            App.DockManager.HidePanel(PanelKey);

            var showPanel = new SimpleActionItem("kModel", Caption, delegate(object sender, EventArgs e)
            {
                var prj = Manager.ProjectController.ProjectService.Project;
                if(prj != null )
                {
                    if (prj.ProcessModule == Models.Integration.ProcessModule.Hydrology)
                    {
                        MessageBox.Show("Current model dose not contains NPS or Carbon module", "Model", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        App.DockManager.ShowPanel(PanelKey);
                    }
                }
                else
                {
                    MessageBox.Show("No project opened", "Model", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
              
            })
            {
                Key = ActionKey,
                ToolTipText = "Analyze nitrogen and phosphorus mass balance (sz_N_budget.csv / sz_P_budget.csv)",
                GroupCaption = Resources.Analysis_group,
                LargeImage = Resources.UiBalance32,
                SmallImage = Resources.UiBalance16
            };
            App.HeaderControl.Add(showPanel);

            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.Remove(ActionKey);
            this.App.DockManager.Remove(PanelKey);
            base.Deactivate();
        }
    }
}
