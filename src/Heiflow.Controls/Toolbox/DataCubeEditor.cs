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

using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.Tools;
using Heiflow.Models.UI;
using Heiflow.Presentation;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.Toolbox
{
    public partial class DataCubeEditor : UserControl, IDataCubeEditor,IChildView
    {
        private DataCubeWorkspace _Workspace;
        private ObservableCollection<DataCubeMeta> _MatMataList;
        private BindingList<DCVarientMeta> _TVMataList;
        private DCVarientMeta _SelectedDCVarientMeta;
        private DataCubeMeta _SelectedDataCubeMeta;
        public DataCubeEditor()
        {
            InitializeComponent();
            _Workspace = new DataCubeWorkspace();
            _Workspace.DataSources.CollectionChanged += DataSources_CollectionChanged;
            _MatMataList = new ObservableCollection<DataCubeMeta>();
            _MatMataList.CollectionChanged += _MatMataList_CollectionChanged;
            _TVMataList = new BindingList<DCVarientMeta>();
            // Binding once is enough: clearing and refilling the list refreshes the grid by itself.
            dgvVariables.DataSource = _TVMataList;
            colBehavior.DataSource = Enum.GetValues(typeof(TimeVarientFlag));
            tsSelectionMode.SelectedIndex = 0;
            tsDataViewMode.SelectedIndex = 0;
            arrayGrid.Selection.EnableMultiSelection = true;
            toolStripArray.Enabled = false;
        }

        /// <summary>The cube selected in the upper list, or null when nothing is selected.</summary>
        private DataCubeMeta SelectedMatMeta
        {
            get
            {
                if (lvMatName.SelectedItems.Count == 0)
                    return null;
                return lvMatName.SelectedItems[0].Tag as DataCubeMeta;
            }
        }

        public IDataCubeWorkspace Workspace
        {
            get
            {
                return _Workspace;
            }
        }
        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }
        public string ChildName
        {
            get { return "DataCubeEditor"; }
        }

        public void ShowView(IWin32Window pararent)
        {
             
        }
        public void ClearContent()
        {
            btnClear_Click(null, null);
        }


        public void InitService()
        {

        }
        private void DataSources_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var mat = e.OldItems[0] as IDataCubeObject;
                var matmata = from mm in _MatMataList where mm.Name == mat.Name select mm;
                if (matmata.Any())
                    _MatMataList.Remove(matmata.First());
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var mat = e.NewItems[0] as IDataCubeObject;
                string size = mat.SizeString();
                _MatMataList.Add(new DataCubeMeta()
                {
                    Name = mat.Name,
                    Size = size,
                    Mat = mat,
                    Owner = mat.OwnerName,
                    RepeatAllowed = (mat.ZeroDimension == DimensionFlag.Time)
                });
            }
        }
        private void _MatMataList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateMatView();
        }
        private void lvMatName_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            _SelectedDataCubeMeta = SelectedMatMeta;
            if (_SelectedDataCubeMeta != null && _SelectedDataCubeMeta.Mat != null)
            {
                toolStripArray.Enabled = true;
                tsLabel.Text = string.Format("{0}", _SelectedDataCubeMeta.Name);
                tsDataViewMode.Enabled = _SelectedDataCubeMeta.Mat.Layout == DataCubeLayout.ThreeD
                    && _SelectedDataCubeMeta.Mat.Topology != null;
                UpdateVariableView(_SelectedDataCubeMeta.Mat);
                SelectFirstVariable();
                btnRemove.Enabled = true;
            }
            else
            {
                tsLabel.Text = "Empty";
                UpdateVariableView(null);
                btnRemove.Enabled = false;
                toolStripArray.Enabled = false;
            }
        }
        private void lvMatName_MouseUp(object sender, MouseEventArgs e)
        {
            menu_remove.Enabled = SelectedMatMeta != null;
            menu_Clear.Enabled = true;
        }
        private void dgvVariables_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVariables.SelectedRows.Count == 0)
                return;
            _SelectedDCVarientMeta = dgvVariables.SelectedRows[0].DataBoundItem as DCVarientMeta;
            if (_SelectedDCVarientMeta != null)
            {
                ShowDataGrid();
            }
        }
        /// <summary>Repeat is only allowed when the zero dimension holds time and never for the
        /// first time step.</summary>
        private void dgvVariables_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dgvVariables.Columns[e.ColumnIndex].Name != colBehavior.Name || e.FormattedValue == null)
                return;

            TimeVarientFlag flag;
            if (!Enum.TryParse(e.FormattedValue.ToString(), out flag) || flag != TimeVarientFlag.Repeat)
                return;

            var meta = dgvVariables.Rows[e.RowIndex].DataBoundItem as DCVarientMeta;
            if (meta != null && meta.Owner != null
                && (meta.Owner.ZeroDimension != DimensionFlag.Time || meta.VariableIndex == 0))
            {
                MessageBox.Show("The behavior can not be set to Repeat for the first time step");
                e.Cancel = true;
            }
        }
        /// <summary>Keeps the data cube in step with what was edited in the grid: the binding has
        /// already written the new value onto the meta object, so it only has to be pushed on.</summary>
        private void dgvVariables_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            var meta = dgvVariables.Rows[e.RowIndex].DataBoundItem as DCVarientMeta;
            if (meta == null || meta.Owner == null)
                return;

            var index = meta.VariableIndex;
            if (meta.Owner.Flags != null && index < meta.Owner.Flags.Length)
                meta.Owner.Flags[index] = meta.Behavior;

            if (meta.Behavior == TimeVarientFlag.Constant && meta.Owner.Constants != null
                && index < meta.Owner.Constants.Length)
            {
                meta.Owner.Constants[index] = (float)meta.Constant;
            }
            else if (meta.Behavior == TimeVarientFlag.Individual && meta.Owner.Multipliers != null
                && index < meta.Owner.Multipliers.Length)
            {
                meta.Owner.Multipliers[index] = (float)meta.Multiplier;
            }
        }
        /// <summary>Shows the first variable of the cube just selected.</summary>
        private void SelectFirstVariable()
        {
            if (_TVMataList.Count == 0)
            {
                _SelectedDCVarientMeta = null;
                return;
            }
            dgvVariables.Rows[0].Selected = true;
            // The selection event only fires once the grid has a handle, so fall back to showing
            // the variable by hand when the grid is not on screen yet.
            if (!ReferenceEquals(_SelectedDCVarientMeta, _TVMataList[0]))
            {
                _SelectedDCVarientMeta = _TVMataList[0];
                ShowDataGrid();
            }
        }
        private void tsSelectionMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tsSelectionMode.SelectedIndex == 0)
                arrayGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            else if (tsSelectionMode.SelectedIndex == 1)
                arrayGrid.SelectionMode = SourceGrid.GridSelectionMode.Row;
            else if (tsSelectionMode.SelectedIndex == 2)
                arrayGrid.SelectionMode = SourceGrid.GridSelectionMode.Column;
        }

        private void tsDataViewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_SelectedDCVarientMeta != null || _SelectedDataCubeMeta != null)
                ShowDataGrid();
        }
        private void menu_remove_Click(object sender, EventArgs e)
        {
            var meta = SelectedMatMeta;
            if (meta != null && meta.Mat != null)
            {
                Workspace.Remove(meta.Name);
                if (Workspace.DataSources.Count == 0)
                {
                    UpdateVariableView(null);
                    arrayGrid.DataSource = null;
                }
                UpdateMatView();
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            Workspace.Clear();
            _MatMataList.Clear();
            UpdateMatView();
            UpdateVariableView(null);
            arrayGrid.DataSource = null;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_SelectedDataCubeMeta == null)
                return;
            if (_SelectedDCVarientMeta != null && _SelectedDCVarientMeta.Behavior == TimeVarientFlag.Individual)
            {
                if (tsDataViewMode.SelectedIndex == 0)
                {
                    _SelectedDCVarientMeta.Owner.FromSpatialSerialArray(_SelectedDCVarientMeta.VariableIndex, 0, arrayGrid.DataSource);
                }
                else
                {
                    _SelectedDCVarientMeta.Owner.FromSpatialRegularArray(_SelectedDCVarientMeta.VariableIndex, 0, arrayGrid.DataSource);
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (_SelectedDataCubeMeta == null)
                return;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "csv file|*.csv";
            dlg.FileName = _SelectedDataCubeMeta.Mat.Name + ".csv";
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                CSVFileStream csv = new CSVFileStream(dlg.FileName);
                if(_SelectedDataCubeMeta.Mat.Layout== DataCubeLayout.ThreeD)
                    csv.Save(arrayGrid.DataSource, _SelectedDataCubeMeta.Mat.Variables);
                else
                    csv.Save(arrayGrid.DataSource, _SelectedDataCubeMeta.Mat.ColumnNames);
            } 
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "csv file|*.csv";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                CSVFileStream csv = new CSVFileStream(dlg.FileName);
                var array = csv.LoadArray();
                arrayGrid.DataSource = array;
            } 
        }


        private void UpdateMatView()
        {
            lvMatName.BeginUpdate();
            lvMatName.Items.Clear();
            foreach (var meta in _MatMataList)
            {
                var item = new ListViewItem(new string[]
                {
                    meta.Name, meta.Size, meta.Owner, meta.RepeatAllowed.ToString()
                });
                item.Tag = meta;
                lvMatName.Items.Add(item);
            }
            lvMatName.EndUpdate();
        }

        private void UpdateVariableView(IDataCubeObject mat)
        {
            _TVMataList.Clear();
            _SelectedDCVarientMeta = null;

            if (mat != null)
            {
                var flags = mat.Flags;
                var multipliers = mat.Multipliers;
                var constants = mat.Constants;
                int nvar = mat.Size[0];
                for (int i = 0; i < nvar; i++)
                {
                    _TVMataList.Add(new DCVarientMeta()
                    {
                        // stress period or layer is used as variable
                        VariableIndex = i,
                        Behavior = flags != null && i < flags.Length ? flags[i] : TimeVarientFlag.Individual,
                        Multiplier = multipliers != null && i < multipliers.Length ? multipliers[i] : 0,
                        Constant = constants != null && i < constants.Length ? constants[i] : 0,
                        Owner = mat
                    });
                }
            }
        }
        private void ShowDataGrid()
        {
            Array array = null;
            IDataCubeObject dc = null;
            if (_SelectedDCVarientMeta != null)
            {
                dc = _SelectedDCVarientMeta.Owner;
                if (tsDataViewMode.SelectedIndex == 0)
                {
                    array = dc.GetSpatialSerialArray(_SelectedDCVarientMeta.VariableIndex, 0);
                }
                else
                {
                    array = dc.GetSpatialRegularArray(_SelectedDCVarientMeta.VariableIndex, 0);
                }
            }

            arrayGrid.DataSource = array;
            var ncol = array.GetLength(1);
            arrayGrid.Columns.Clear();
            for (int i = 0; i < ncol + 1; i++)
            {
                var col = new SourceGrid.ColumnInfo(arrayGrid);
                col.MinimalWidth = 50;
                arrayGrid.Columns.Add(col);
            }
        }

    }
}
