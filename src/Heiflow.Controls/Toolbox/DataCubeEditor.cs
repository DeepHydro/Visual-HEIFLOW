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
using System.Linq;
using System.Windows.Forms;
using Heiflow.Models.Tools;
using System.Collections.ObjectModel;
using Heiflow.Core.Data;
using Heiflow.Presentation.Controls;
using System.ComponentModel.Composition;
using Heiflow.Presentation;
using Heiflow.Core.IO;
using Heiflow.Models.UI;

namespace Heiflow.Controls.WinForm.Toolbox
{
    public partial class DataCubeEditor : UserControl, IDataCubeEditor,IChildView
    {
        private DataCubeWorkspace _Workspace;
        private ObservableCollection<DataCubeMeta> _MatMataList;
        private List<DCVarientMeta> _TVMataList;
        private DCVarientMeta _SelectedDCVarientMeta;
        private DataCubeMeta _SelectedDataCubeMeta;
        public DataCubeEditor()
        {
            InitializeComponent();
            _Workspace = new DataCubeWorkspace();
            _Workspace.DataSources.CollectionChanged += DataSources_CollectionChanged;
            _MatMataList = new ObservableCollection<DataCubeMeta>();
            _MatMataList.CollectionChanged += _MatMataList_CollectionChanged;
            _TVMataList = new List<DCVarientMeta>();
            tsSelectionMode.SelectedIndex = 0;
            tsDataViewMode.SelectedIndex = 0;
            arrayGrid.Selection.EnableMultiSelection = true;
            toolStripArray.Enabled = false;
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
        private void olvMatName_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            _SelectedDataCubeMeta = olvMatName.SelectedObject as DataCubeMeta;
            if (_SelectedDataCubeMeta != null && _SelectedDataCubeMeta.Mat != null)
            {
                toolStripArray.Enabled = true;
                tsLabel.Text = string.Format("{0}", _SelectedDataCubeMeta.Name);
                if (_SelectedDataCubeMeta.Mat.Topology != null)
                {
                    tsDataViewMode.Enabled = true;
                }
                else
                {
                    tsDataViewMode.Enabled = false;
                }

                //if (_SelectedDataCubeMeta.Mat.TimeBrowsable)
                //{
                    UpdateVariableView(_SelectedDataCubeMeta.Mat);
                    olvVariableName.SelectedIndex = 0;
                //}
                //else
                //{
                //    olvVariableName.ClearObjects();
                //    _SelectedDCVarientMeta = null;
                //    ShowDataGrid();
                //}
                btnRemove.Enabled = true;
            }
            else
            {
                tsLabel.Text = "Empty";
                olvVariableName.ClearObjects();
                btnRemove.Enabled = false;
                toolStripArray.Enabled = false;
                
            }
        }
        private void olvMatName_MouseUp(object sender, MouseEventArgs e)
        {
            if (olvMatName.SelectedObject != null)
            {
                menu_remove.Enabled = true;
                menu_Clear.Enabled = true;
            }
            else
            {
                menu_remove.Enabled = false;
                menu_Clear.Enabled = true;
            }
        }

        private void olvVariableName_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            _SelectedDCVarientMeta = olvVariableName.SelectedObject as DCVarientMeta;
            if (_SelectedDCVarientMeta != null)
            {
                ShowDataGrid();
            }
        }

        private void olvVariableName_CellEditFinishing(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            var meta = e.RowObject as DCVarientMeta;
            if (meta != null)
            {
                if (e.Column.AspectName == "Behavior")
                {
                    if (meta.TimeIndex == 0)
                    {
                        if (e.NewValue.ToString() == TimeVarientFlag.Repeat.ToString())
                        {
                            MessageBox.Show("The behavior can not be set to Repeat for the first time step");
                            e.Cancel = true;
                        }
                    }
                }
            }
        }
        private void olvVariableName_CellEditFinished(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            var meta = e.RowObject as DCVarientMeta;
            if (meta != null)
            {
                if (e.Column.AspectName == "Behavior")
                {
                    if (e.NewValue.ToString() == TimeVarientFlag.Constant.ToString())
                    {
                        meta.Owner.Flags[meta.TimeIndex, 0] = TimeVarientFlag.Constant;
                        meta.Owner.Constants.SetValue((float)meta.Constant, meta.TimeIndex, 0);       
                    }
                    else if (e.NewValue.ToString() == TimeVarientFlag.Individual.ToString())
                    {
                        meta.Owner.Flags[meta.TimeIndex, 0] = TimeVarientFlag.Individual;
                        meta.Owner.Multipliers[meta.TimeIndex, 0] = (float)meta.Multiplier;
                        //meta.Owner.AllocateSpaceDim(meta.TimeIndex, 0, meta.Owner.Topology.ActiveCellCount);
                    }
                    else if (e.NewValue.ToString() == TimeVarientFlag.Repeat.ToString())
                    {
                        meta.Owner.Flags[meta.TimeIndex, 0] = TimeVarientFlag.Repeat;
                    }
                }
                else if (e.Column.AspectName == "Constant")
                {
                    if (meta.Behavior == TimeVarientFlag.Constant)
                    {
                        meta.Owner.Constants[meta.TimeIndex, 0] = (float) meta.Constant;
                    }
                }
                else if (e.Column.AspectName == "Multiplier")
                {
                    if (meta.Behavior == TimeVarientFlag.Individual)
                    {
                        meta.Multiplier = float.Parse(e.NewValue.ToString());
                        meta.Owner.Multipliers[meta.TimeIndex, 0] = (float)meta.Multiplier;
                    }
                }
            }
        }
        private void menu_remove_Click(object sender, EventArgs e)
        {
            var meta = olvMatName.SelectedObject as DataCubeMeta;
            if (meta != null && meta.Mat != null)
            {
                Workspace.Remove(meta.Name);
                if (Workspace.DataSources.Count == 0)
                {
                    olvVariableName.ClearObjects();
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
            olvVariableName.ClearObjects();
            arrayGrid.DataSource = null;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_SelectedDataCubeMeta == null)
                return;

            if (_SelectedDataCubeMeta.Mat.DataCubeType == DataCubeType.Varient)
            {
                if (_SelectedDCVarientMeta != null && _SelectedDCVarientMeta.Behavior == TimeVarientFlag.Individual)
                {
                    if (tsDataViewMode.SelectedIndex == 0)
                    {
                        _SelectedDCVarientMeta.Owner.FromSerialArray(_SelectedDCVarientMeta.TimeIndex, 0, arrayGrid.DataSource);
                    }
                    else
                    {
                        _SelectedDCVarientMeta.Owner.FromRegularArray(_SelectedDCVarientMeta.TimeIndex, 0, arrayGrid.DataSource);
                    }
                }
            }
            else if (_SelectedDataCubeMeta.Mat.DataCubeType == DataCubeType.Vector)
            {
                if (tsDataViewMode.SelectedIndex == 0)
                {
                    _SelectedDataCubeMeta.Mat.FromSerialArray(0, 0, arrayGrid.DataSource);
                }
                else
                {
                    _SelectedDataCubeMeta.Mat.FromRegularArray(0, 0, arrayGrid.DataSource);
                }
            }
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
                _MatMataList.Add(new DataCubeMeta() { Name = mat.Name, Size = size, Mat = mat, Owner = mat.OwnerName });
            }
        }

        private void _MatMataList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateMatView();
        }

        private void UpdateMatView()
        {
            olvMatName.SetObjects(_MatMataList);
        }

        private void UpdateVariableView(IDataCubeObject mat)
        {
            if (mat == null)
            {
                _TVMataList.Clear();
            }
            else
            {
                _TVMataList.Clear();

                int ntime = mat.Size[0];
                for (int i = 0; i < ntime; i++)
                {
                    _TVMataList.Add(new DCVarientMeta()
                    {
                        TimeIndex = i,
                        Behavior = mat.Flags[i, 0],
                        Multiplier = mat.Multipliers[i, 0],
                        Constant = mat.Constants[i, 0],
                        Owner = mat
                    });
                }

            }
            olvVariableName.SetObjects(_TVMataList);
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
                    array = dc.GetSerialArrayByTime(_SelectedDCVarientMeta.TimeIndex, 0);
                }
                else
                {
                    array = dc.GetRegularlArrayByTime(_SelectedDCVarientMeta.TimeIndex, 0);
                }
            }
            else
            {
                dc = _SelectedDataCubeMeta.Mat;
                if(dc.Topology != null)
                {
                    if (tsDataViewMode.SelectedIndex == 0)
                    {
                        array = dc.GetSerialArrayByTime(0, 0);
                    }
                    else
                    {
                        array = dc.GetRegularlArrayByTime(0, 0);
                    }
                }
                else
                {
                    array = dc.GetSerialArrayByTime(0, 0);
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
                csv.Save(arrayGrid.DataSource, _SelectedDataCubeMeta.Mat.Variables);
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

        public void ClearContent()
        {
            btnClear_Click(null, null);
        }


        public void InitService()
        {
          
        }
    }
}
