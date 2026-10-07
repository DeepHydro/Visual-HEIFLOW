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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Heiflow.Controls;
  using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Animation;
using Heiflow.Presentation.Controls;
using Heiflow.Models.Generic;
using Heiflow.Models.Tools;
using Heiflow.Core.Data;
using Heiflow.Presentation.Animation;
using System.ComponentModel.Composition;
using Heiflow.Presentation;
using Heiflow.Presentation.Services;
using Heiflow.Applications;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;

namespace Heiflow.Controls.WinForm.Controls
{

    public partial class AnimationPlayer : UserControl, IAnimationView,IChildView
    {
        private IDataCubeWorkspace _WorkSpace;
        private bool isPlay = false;
        private IDataCubeObject _selectedDc;
        public delegate void ControlStringConsumer(ComboBox control, List<ITimeService> list);  // defines a delegate type
        private List<IDataCubeAnimation> _Animators = new List<IDataCubeAnimation>();
        private IDataCubeAnimation _SelectedAnimator;
        private IShellService _ShellService;
        private const string ReadyKey = "ready";
        private const string StandbyKey = "standby";

        public AnimationPlayer()
        {
            InitializeComponent();
            _WorkSpace = new DataCubeWorkspace();
            _WorkSpace.DataSourceCollectionChanged += _WorkSpace_DataSourceCollectionChanged;
            this.Load += AnimationPlayer_Load;
        }
        public object DataContext
        {
            get;
            set;
        }

        public IDataCubeWorkspace DataCubeWorkspace
        {
            get
            {
                return _WorkSpace;
            }
        }
        public string ChildName
        {
            get { return "AnimationPlayer"; }
        }

        public void ShowView(IWin32Window pararent)
        {
             
        }
        private void AnimationPlayer_Load(object sender, EventArgs e)
        {
            if (_ShellService == null)
                _ShellService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();

            MapAnimation map = new MapAnimation();
            map.CurrentChanged += map_CurrentChanged;
            _Animators.Add(map);
            SurfaceAnimation surf = new SurfaceAnimation()
            {
                SurfacePlot = _ShellService.SurfacePlot
            };
            surf.CurrentChanged += map_CurrentChanged;
            _Animators.Add(surf);
            cmbAnimators.ComboBox.DisplayMember = "Name";
            cmbAnimators.ComboBox.DataSource = _Animators.ToArray();
            cmbAnimators.ComboBox.SelectedIndex = 0;
        }
        private void _WorkSpace_DataSourceCollectionChanged(object sender, EventArgs e)
        {
            FillCubeTree();
        }

        /// <summary>Rebuilds the cube tree from the workspace: one node per data cube with its
        /// variables below it. Nodes are expanded so a cube just added is visible at once.</summary>
        private void FillCubeTree()
        {
            tvDataCubes.BeginUpdate();
            tvDataCubes.Nodes.Clear();
            foreach (var dc in _WorkSpace.DataSources)
            {
                var root = new TreeNode(dc.Name);
                root.ImageKey = ReadyKey;
                root.SelectedImageKey = ReadyKey;
                root.ToolTipText = string.Format("Size: {0}{1}Owner: {2}", dc.SizeString(),
                    Environment.NewLine, dc.OwnerName);
                root.Tag = new CubeNode(dc, -1);
                tvDataCubes.Nodes.Add(root);

                if (dc.Variables != null)
                {
                    for (int i = 0; i < dc.Variables.Length; i++)
                    {
                        var child = new TreeNode(dc.Variables[i]);
                        var ready = dc.IsAllocated(i);
                        child.ImageKey = ready ? ReadyKey : StandbyKey;
                        child.SelectedImageKey = child.ImageKey;
                        child.ToolTipText = string.Format("[1][{0}][{1}]{2}Owner: {3}", dc.Size[1], dc.Size[2],
                            Environment.NewLine, dc.OwnerName);
                        child.Tag = new CubeNode(dc, i);
                        root.Nodes.Add(child);
                    }
                }
                root.Expand();
            }
            tvDataCubes.EndUpdate();
        }
        private void map_CurrentChanged(object sender, int e)
        {
            if (e <= (listBox_timeline.Items.Count - 1))
                listBox_timeline.SelectedIndex = e;
        }

        private void tvDataCubes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node.Tag as CubeNode;
            if (node != null)
                _selectedDc = node.Cube;

            // A cube node has no time steps of its own, only its variable nodes do.
            if (node == null || node.VariableIndex < 0 || !node.Cube.IsAllocated(node.VariableIndex)
                || node.Cube.DateTimes == null)
            {
                listBox_timeline.DataSource = null;
                listBox_timeline.Items.Clear();
                return;
            }

            _selectedDc.SelectedVariableIndex = node.VariableIndex;
            listBox_timeline.DataSource = _selectedDc.DateTimes;
            if (listBox_timeline.Items.Count > 0)
                listBox_timeline.SelectedIndex = 0;
        }

        private void cmbAnimators_SelectedIndexChanged(object sender, EventArgs e)
        {
            _SelectedAnimator = cmbAnimators.SelectedItem as IDataCubeAnimation;
            if(cmbAnimators.SelectedIndex == 1)
                _ShellService.SelectPanel(DockPanelNames.View3DPanel);
        }
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (isPlay)
            {
                _SelectedAnimator.Stop();
                isPlay = false;
                btnPlay.Image = Resources.UiRun24;
                btnPlay.ToolTipText = "Play";
            }
            else
            {
                btnPlay.Image = Resources.UiPause24;
                btnPlay.ToolTipText = "Stop";
                isPlay = true;
                _SelectedAnimator.Play();
            }
        }


        private void listBoxTimeLine_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            //if the item state is selected them change the back color 
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          e.ForeColor,
                                          Color.Yellow);//Choose the color

            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Draw the current item text
            e.Graphics.DrawString(listBox_timeline.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

        private void listBoxTimeLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_SelectedAnimator != null && _selectedDc != null && !isPlay)
            {
                _SelectedAnimator.DataSource = _selectedDc;
                _SelectedAnimator.Go(listBox_timeline.SelectedIndex);
                if(listBox_timeline.SelectedIndex == (listBox_timeline.Items.Count-1))
                {
                    isPlay = false;
                    btnPlay.Image = Resources.UiRun24;
                    btnPlay.ToolTipText = "Play";
                }
            }
        }

        public void ClearContent()
        {
            // Clearing the control here would also drop the column definitions. They are never
            // rebuilt afterwards (AutoGenerateColumns is off), so the grid would stay empty for
            // every cube added from then on. Routing through the workspace lets the grid rebuild
            // itself empty and keeps its columns.
            _WorkSpace.Clear();
            listBox_timeline.DataSource = null;
            _selectedDc = null;
        }
        public void InitService()
        {

        }

        /// <summary>What a node of the cube tree stands for: the cube itself when VariableIndex is
        /// below zero, otherwise one of the variables of that cube.</summary>
        private class CubeNode
        {
            public CubeNode(IDataCubeObject cube, int variableIndex)
            {
                Cube = cube;
                VariableIndex = variableIndex;
            }

            public IDataCubeObject Cube { get; private set; }

            public int VariableIndex { get; private set; }
        }
    }

}
