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

using Heiflow.Applications;
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.Generic;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Models.UI;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using ILNumerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Heiflow.Controls.WinForm.Controls
{
    public enum BindingVariable { SingleVariable, MultipleVariables }
    public partial class DataGridEx : UserControl, IDataGridView, IChildView
    {
        private DataTable _DataTable;
        private IDataCubeObject _DataCubeObject;
        private int _CurrentColumnIndex;
        private IShellService _ShellService;
        private IWindowService _WindowService;
        private IParameter[] _Parameters;
        private bool _need_refresh_pages = false;
        public static string SaveButtion = "btnSave";
        public static string ImportButtion = "btnImport";
        public static string Save2ExcelButtion = "btnSave2Excel";
        public static string CmbPage = "cmbPage";
        public static string CmbPrimaryKey = "cmb_PrimaryKey";

        public DataGridEx()
        {
            InitializeComponent();
            cmbPage.SelectedIndexChanged += cmbPage_SelectedIndexChanged;
            cmb_PrimaryKey.SelectedIndexChanged += cmb_PrimaryKey_SelectedIndexChanged;
            EnableControls(false);
        }

        public BindingNavigator Navigator
        {
            get
            {
                return this.bindingNavigator1;
            }
        }

        public DataGridView DataGrid
        {
            get
            {
                return this.dataGridView1;
            }
        }

        public ContextMenuStrip HeaderContextMenu
        {
            get
            {
                return this.contextMenuStrip_datagrid;
            }
        }

        public DataTable DataTable
        {
            get
            {
                _DataTable = bindingSource1.DataSource as DataTable;
                return _DataTable;
            }
            set
            {
                _DataTable = value;
                Bind(_DataTable);
            }
        }
        public string DataObjectName
        {
            get
            {
                return toolStripLabel1.Text;
            }
            set
            {
                toolStripLabel1.Text = value;
            }
        }
        public string ChildName
        {
            get { return "DataGridView"; }
        }

        public bool ShowSaveButton
        {
            set
            {
                btnSave.Visible = value;
            }
            get
            {
                return btnSave.Visible;
            }
        }
        public bool ShowSave2Excel
        {
            set
            {
                btnSave2Excel.Visible = value;
            }
            get
            {
                return btnSave2Excel.Visible;
            }
        }
        public bool ShowImport
        {
            set
            {
                btnImport.Visible = value;
            }
            get
            {
                return btnImport.Visible;
            }
        }

        public void ShowView(IWin32Window pararent)
        {
            
        }
        #region Binding
        /// <summary>
        /// display 2d matrix
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="data">2d matrix</param>
        public void BindMatrix<T>(ILArray<T> data)
        {
            var dims = data.Size.ToIntArray();
            var dt = CreateTable<T>(dims[1]);
            for (int r = 0; r < dims[0]; r++)
            {
                var dr = dt.NewRow();
                for (int c = 0; c < dims[1]; c++)
                {
                    dr[c] = data[r, c];
                }
                dt.Rows.Add(dr);
            }


            this.bindingSource1.DataSource = dt;
            this.dataGridView1.DataSource = bindingSource1;

            EnableControls(false);
        }
        public void Bind<T>(ILArray<T> data)
        {
            if (data.IsScalar)
            {
                var dt = CreateTable<T>(1);
                var dr = dt.NewRow();
                dr[0] = data[0].ToArray()[0];
                dt.Rows.Add(dr);
                this.bindingSource1.DataSource = dt;
                this.dataGridView1.DataSource = bindingSource1;
            }
            else if (data.IsMatrix)
            {
                BindMatrix<T>(data);
            }
            else
            {
                if (data.Size.NumberOfDimensions > 2)
                {
                    var dims = data.Size.ToIntArray();
                    cmbPage.Items.Clear();
                    for (int p = 0; p < dims[2]; p++)
                    {
                        cmbPage.Items.Add((p + 1).ToString());
                    }
                }
            }

            EnableControls(false);
        }
        public void Bind<T>(T[] array)
        {
            var dt = CreateTable<T>(1);
            for (int r = 0; r < array.Length; r++)
            {
                var dr = dt.NewRow();
                dr[0] = array[r];
                dt.Rows.Add(dr);
            }
            this.bindingSource1.DataSource = dt;
            this.dataGridView1.DataSource = bindingSource1;
            EnableControls(false);
        }
        public void Bind<T>(T[][] matrix)
        {
            int nrow = matrix.GetLength(0);
            int ncol = matrix[0].GetLength(0);

            var dt = CreateTable<T>(ncol);
            for (int r = 0; r < nrow; r++)
            {
                var dr = dt.NewRow();
                for (int c = 0; c < ncol; c++)
                {
                    dr[c] = matrix[r][c];
                }
                dt.Rows.Add(dr);
            }

            this.bindingSource1.DataSource = dt;
            this.dataGridView1.DataSource = bindingSource1;
            EnableControls(false);
        }
        public void Bind(DataTable table)
        {
            this.bindingSource1.DataSource = table;
            this.dataGridView1.DataSource = bindingSource1;
            EnableControls(false);
        }
        public void Bind(IDataCubeObject dc)
        {
            _need_refresh_pages = true;
            _DataCubeObject = dc;
            if (dc.SelectedVariableIndex < 0)
            {
                DataObjectName = dc.Name;
                EnableControls(true);
                cmb_PrimaryKey.SelectedIndex = 0;
                if (_need_refresh_pages)
                    cmb_PrimaryKey_SelectedIndexChanged(cmb_PrimaryKey, EventArgs.Empty);
                _Parameters = null;
            }
            else
            {
                if (dc.IsAllocated(dc.SelectedVariableIndex))
                {
                    DataObjectName = string.Format("{0}[{1}]", dc.Name, dc.Variables[dc.SelectedVariableIndex]);
                    EnableControls(true);
                    cmb_PrimaryKey.SelectedIndex = 0;
                    if (_need_refresh_pages)
                        cmb_PrimaryKey_SelectedIndexChanged(cmb_PrimaryKey, EventArgs.Empty);
                    _Parameters = null;
                    //_DataTable = dc.ToDataTableByTime(dc.SelectedVariableIndex, 0);
                    //this.bindingSource1.DataSource = _DataTable;
                    //this.dataGridView1.DataSource = bindingSource1;
                   
                }
                else
                {
                    this.bindingSource1.DataSource = null;
                    this.dataGridView1.DataSource = bindingSource1;
                    EnableControls(false);
                    _Parameters = null;
                }
            }
        }

        public void Bind(IParameter[] paras)
        {
            _Parameters = paras;
            _DataTable = new DataTable();
            foreach (var pa in paras)
            {
                DataColumn dc = new DataColumn(pa.Name, pa.GetVariableType());
                _DataTable.Columns.Add(dc);
            }
            int nrow = paras[0].ValueCount;
            for (int i = 0; i < nrow; i++)
            {
                DataRow dr = _DataTable.NewRow();
                for (int j = 0; j < paras.Length; j++)
                {
                    dr[j] = paras[j].GetValue(0,i,0);
                }
                _DataTable.Rows.Add(dr);
            }
            this.bindingSource1.DataSource = _DataTable;
            this.dataGridView1.DataSource = bindingSource1;
            EnableControls(false);
            btnSave.Enabled = true;
            btnImport.Enabled = true;
            _DataCubeObject = null;
        }
        #endregion

        #region Grid Copying
        private DataTable CreateTable<T>(int ncol)
        {
            DataTable dt = new DataTable();
            for (int c = 0; c < ncol; c++)
            {
                DataColumn dc = new DataColumn("C" + c, typeof(T));
                dt.Columns.Add(dc);
            }
            return dt;
        }

        private void CopyToClipboard()
        {
            //Copy to clipboard
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void PasteClipboardValue()
        {
            bool alert = false;
            //Show Error if no cell is selected
            if (dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a cell", "Paste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var type = dataGridView1.Columns[0].ValueType;
            //Get the satring Cell
            DataGridViewCell startCell = GetStartCell(dataGridView1);
            //Get the clipboard value in a dictionary
            Dictionary<int, Dictionary<int, string>> cbValue = ClipBoardValues(Clipboard.GetText());

            int iRowIndex = startCell.RowIndex;
            foreach (int rowKey in cbValue.Keys)
            {
                int iColIndex = startCell.ColumnIndex;
                foreach (int cellKey in cbValue[rowKey].Keys)
                {
                    //Check if the index is with in the limit
                    if (iColIndex <= dataGridView1.Columns.Count - 1 && iRowIndex <= dataGridView1.Rows.Count - 1)
                    {
                        DataGridViewCell cell = dataGridView1[iColIndex, iRowIndex];

                        //Copy to selected cells if 'chkPasteToSelectedCells' is checked
                        //if ((chkPasteToSelectedCells.Checked && cell.Selected) ||
                        //    (!chkPasteToSelectedCells.Checked))
                        try
                        {
                            cell.Value = Convert.ChangeType(cbValue[rowKey][cellKey], type);
                        }
                        catch
                        {
                            alert = true;
                            goto paste_error;
                        }
                    }
                    iColIndex++;
                }
                iRowIndex++;
            }

        paste_error:
            if (alert)
                MessageBox.Show("Failed to Paste", "Paste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private DataGridViewCell GetStartCell(DataGridView dgView)
        {
            //get the smallest row,column index
            if (dgView.SelectedCells.Count == 0)
                return null;

            int rowIndex = dgView.Rows.Count - 1;
            int colIndex = dgView.Columns.Count - 1;

            foreach (DataGridViewCell dgvCell in dgView.SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex)
                    rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex)
                    colIndex = dgvCell.ColumnIndex;
            }

            return dgView[colIndex, rowIndex];
        }

        private Dictionary<int, Dictionary<int, string>> ClipBoardValues(string clipboardValue)
        {
            clipboardValue = clipboardValue.Trim();
            Dictionary<int, Dictionary<int, string>> copyValues = new Dictionary<int, Dictionary<int, string>>();

            String[] lines = clipboardValue.Split('\n');

            for (int i = 0; i <= lines.Length - 1; i++)
            {
                copyValues[i] = new Dictionary<int, string>();
                String[] lineContent = lines[i].Split('\t');

                //if an empty cell value copied, then set the dictionay with an empty string
                //else Set value to dictionary
                if (lineContent.Length == 0)
                    copyValues[i][0] = string.Empty;
                else
                {
                    for (int j = 0; j <= lineContent.Length - 1; j++)
                        copyValues[i][j] = lineContent[j];
                }
            }
            return copyValues;
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteClipboardValue();
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Modifiers == Keys.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.C:
                            CopyToClipboard();
                            break;

                        case Keys.V:
                            PasteClipboardValue();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Copy/paste operation failed. " + ex.Message, "Copy/Paste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void copy_toolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }
        #endregion
        public void ShowView()
        {

        }
        public void ClearContent()
        {
            bindingSource1.DataSource = null;
            dataGridView1.DataSource = bindingSource1;
            EnableControls(false);
        }
        private void EnableControls(bool enable)
        {
            cmb_PrimaryKey.Enabled = enable;
            cmbPage.Enabled = enable;
            btnSave.Enabled = enable;
            btnImport.Enabled = enable;
        }
        #region Toolbar
        private void cmbPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_DataCubeObject == null)
                return;

            if (cmb_PrimaryKey.SelectedIndex == 0)
            {
                _DataCubeObject.SelectedTimeIndex = cmbPage.SelectedIndex;
                _DataCubeObject.SelectedSpaceIndex = -1;
                _DataTable = _DataCubeObject.ToDataTableByTime(_DataCubeObject.SelectedVariableIndex, _DataCubeObject.SelectedTimeIndex);
            }
            else if (cmb_PrimaryKey.SelectedIndex == 1)
            {
                _DataCubeObject.SelectedTimeIndex = -1;
                _DataCubeObject.SelectedSpaceIndex = cmbPage.SelectedIndex;
                _DataTable = _DataCubeObject.ToDataTableBySpace(_DataCubeObject.SelectedVariableIndex, _DataCubeObject.SelectedSpaceIndex);
            }
            this.bindingSource1.DataSource = _DataTable;
            this.dataGridView1.DataSource = bindingSource1;
        }

        private void cmb_PrimaryKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_DataCubeObject == null)
                return;
            _need_refresh_pages = false;
            if (cmb_PrimaryKey.SelectedIndex == 0)//time
            {
                if (_DataCubeObject.DateTimes != null)
                    cmbPage.ComboBox.DataSource = _DataCubeObject.DateTimes;
                else
                {
                    int[] pages = new int[_DataCubeObject.Size[1]];
                    for (int i = 0; i < _DataCubeObject.Size[1]; i++)
                    {
                        pages[i] = i + 1;
                    }
                    cmbPage.ComboBox.DataSource = pages;
                }
            }
            else if (cmb_PrimaryKey.SelectedIndex == 1) // space
            {
                var len = _DataCubeObject.GetSpaceDimLength(_DataCubeObject.SelectedVariableIndex, _DataCubeObject.SelectedTimeIndex);
                int[] pages = new int[len];
                for (int i = 0; i < len; i++)
                {
                    pages[i] = i + 1;
                }
                cmbPage.ComboBox.DataSource = pages;
            }
            //cmbPage.SelectedIndex = 0;
            cmbPage_SelectedIndexChanged(cmbPage, EventArgs.Empty);
        }

        private void sortingAcendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Sort(dataGridView1.Columns[_CurrentColumnIndex], ListSortDirection.Ascending);
        }

        private void sortingDescendingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Sort(dataGridView1.Columns[_CurrentColumnIndex], ListSortDirection.Descending);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_DataTable != null)
            {
                if (_DataCubeObject != null)
                {
                    _DataCubeObject.FromDataTable(_DataTable);
                }
                try
                {
                    if (_Parameters != null)
                    {
                        int nrow = _Parameters[0].ValueCount;
                        if (_DataTable.Rows.Count == nrow && _DataTable.Columns.Count == _Parameters.Length)
                        {
                            for (int i = 0; i < nrow; i++)
                            {
                                var dr = _DataTable.Rows[i];
                                for (int j = 0; j < _Parameters.Length; j++)
                                {
                                    _Parameters[j].SetValue(0, i, 0, dr[j]);
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    string msg = string.Format("Failed to save. Erros are found in the data table:{0}. Please correct it before saving.", ex.Message);
                    _ShellService.MessageService.ShowError(null, msg);
                }
            }
        }
        private void plotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_ShellService == null)
            {
                _ShellService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
                _WindowService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IWindowService>();
            }
            _ShellService.WinChart.ShowView(_ShellService.MainForm);
            if (DataTable != null && _CurrentColumnIndex > -1)
            {
                float buf = 0;
                int nrow = DataTable.Rows.Count;
                float[] vv = new float[nrow];
                for (int i = 0; i < nrow; i++)
                {   
                    float.TryParse(DataTable.Rows[i][_CurrentColumnIndex].ToString(), out buf);
                    vv[i] = buf;
                }
                _ShellService.WinChart.Plot<float>(vv, DataTable.Columns[_CurrentColumnIndex].ColumnName, SeriesChartType.FastLine);
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnImport_Click(sender, e);
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            _CurrentColumnIndex = 0;
            if (e.Button == MouseButtons.Right)
            {
                var ht = dataGridView1.HitTest(e.X, e.Y);
                if (ht.Type == DataGridViewHitTestType.ColumnHeader)
                {
                    if (_DataCubeObject != null && _DataCubeObject is IParameter)
                        toolStripTextBox_constant.Text = (_DataCubeObject as IParameter).DefaultValue.ToString();

                    contextMenuStrip_datagrid.Show(MousePosition);
                    _CurrentColumnIndex = ht.ColumnIndex;
                }
            }
        }

        private void toolStripTextBox_constant_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var data_type = _DataTable.Columns[_CurrentColumnIndex].DataType;
                object constant = null;
                try
                {
                    constant = Convert.ChangeType(toolStripTextBox_constant.Text, data_type);
                }
                catch
                {
                    MessageBox.Show("Invalid number!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                this.bindingSource1.DataSource = null;

                for (int r = 0; r < _DataTable.Rows.Count; r++)
                {
                    _DataTable.Rows[r][_CurrentColumnIndex] = constant;
                }
                this.bindingSource1.DataSource = _DataTable;
                this.dataGridView1.DataSource = bindingSource1;
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_DataCubeObject != null && _DataCubeObject is IParameter)
            {
                var para = _DataCubeObject as IParameter;

                //e.RowIndex
                if (para.VariableType == ParameterType.Dimension)
                {
                    int new_len = int.Parse(_DataTable.Rows[e.RowIndex][e.ColumnIndex].ToString());
                    var mms = para.Owner as IMMSPackage;
                    if (mms != null)
                    {
                        mms.AlterLength(para.Name, new_len);
                    }
                }
            }
        }

        private void btnSave2Excel_Click(object sender, EventArgs e)
        {
            var dt = this.bindingSource1.DataSource as DataTable;
            if (dt != null)
            {
                SaveFileDialog sd = new SaveFileDialog();
                sd.Filter = "csv file|*.csv";
                sd.FileName = "table.csv";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    CSVFileStream csv = new CSVFileStream(sd.FileName);
                    csv.Save(dt);
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var dt = bindingSource1.DataSource as DataTable;
            if (dt != null)
            {
                OpenFileDialog sd = new OpenFileDialog();
                sd.Filter = "csv file|*.csv";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    CSVFileStream csv = new CSVFileStream(sd.FileName);
                    csv.LoadTo(dt);
                    this.bindingSource1.DataSource = dt;
                    this.dataGridView1.DataSource = bindingSource1;
                }
            }
        }
        public void InitService()
        {

        }
        #endregion
    }
}