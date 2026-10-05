using Heiflow.Controls;
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.Generic;
using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
using ILNumerics;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// Table view of a data table, an array or a slice of a data cube. The grid is a native WPF
    /// DataGrid, see the note at the top of DataGridWindow.xaml.
    /// </summary>
    /// 
    [Export(typeof(IDataGridView))]
    public partial class DataGridWindow : MetroWindow, IDataGridView, IChildWPFWindow
    {
        /// <summary>Item of the time and cell boxes that stands for the whole dimension.</summary>
        private const string AllString = "All";

        private DataTable _DataTable;
        private IDataCubeObject _DataCubeObject;
        private IParameter[] _Parameters;
        private bool _FillingSlicers;

        public DataGridWindow()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            CloseAllowed = false;
            this.Name = DockPanelNames.DataGridPanel;
        }

        public bool CloseAllowed
        {
            get;
            set;
        }

        public string DataObjectName
        {
            get
            {
                return tbDataName.Text;
            }
            set
            {
                tbDataName.Text = value;
                if (!string.IsNullOrEmpty(value))
                    Title = value;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CloseAllowed)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        #region Binding

        public void Bind<T>(ILArray<T> data)
        {
            ClearSourceObject();
            if (data.IsScalar)
            {
                var dt = CreateTable<T>(1);
                var dr = dt.NewRow();
                dr[0] = data[0].ToArray()[0];
                dt.Rows.Add(dr);
                SetSource(dt);
            }
            else if (data.IsMatrix)
            {
                var dims = data.Size.ToIntArray();
                var dt = CreateTable<T>(dims[1]);
                dt.BeginLoadData();
                for (int r = 0; r < dims[0]; r++)
                {
                    var dr = dt.NewRow();
                    for (int c = 0; c < dims[1]; c++)
                        dr[c] = data[r, c];
                    dt.Rows.Add(dr);
                }
                dt.EndLoadData();
                SetSource(dt);
            }
            else if (data.Size.NumberOfDimensions == 3)
            {
                // Only the first layer of a three dimensional array can be shown, a table has two
                // dimensions. The old grid showed nothing at all in this case.
                var dims = data.Size.ToIntArray();
                var dt = CreateTable<T>(dims[1]);
                dt.BeginLoadData();
                for (int r = 0; r < dims[0]; r++)
                {
                    var dr = dt.NewRow();
                    for (int c = 0; c < dims[1]; c++)
                        dr[c] = data[r, c, 0];
                    dt.Rows.Add(dr);
                }
                dt.EndLoadData();
                SetSource(dt);
            }
            else
            {
                SetSource(new DataTable());
            }
        }

        public void Bind<T>(T[] array)
        {
            ClearSourceObject();
            var dt = CreateTable<T>(1);
            dt.BeginLoadData();
            for (int r = 0; r < array.Length; r++)
            {
                var dr = dt.NewRow();
                dr[0] = array[r];
                dt.Rows.Add(dr);
            }
            dt.EndLoadData();
            SetSource(dt);
        }

        public void Bind<T>(T[][] matrix)
        {
            ClearSourceObject();
            int nrow = matrix.Length;
            int ncol = matrix[0].Length;
            var dt = CreateTable<T>(ncol);
            dt.BeginLoadData();
            for (int r = 0; r < nrow; r++)
            {
                var dr = dt.NewRow();
                for (int c = 0; c < ncol; c++)
                    dr[c] = matrix[r][c];
                dt.Rows.Add(dr);
            }
            dt.EndLoadData();
            SetSource(dt);
        }

        public void Bind(DataTable table)
        {
            ClearSourceObject();
            SetSource(table);
        }

        public void Bind(IDataCubeObject dc)
        {
            if (dc == null)
                return;
            _Parameters = null;
            _DataCubeObject = dc;
            DataObjectName = dc.Name;

            if (dc is IParameter)
            {
                ShowSlicers(false);
                SetSource(dc.ToDataTable());
            }
            else if (dc.SelectedVariableIndex < 0)
            {
                ShowSlicers(false);
                SetSource(dc.ToDataTable(-1, 0, -1));
            }
            else if (dc.Layout == DataCubeLayout.ThreeD)
            {
                FillSlicers(dc);
                ShowSlicers(true);
                ReloadFromCube();
            }
            else
            {
                ShowSlicers(false);
                SetSource(dc.ToDataTable());
            }
        }

        public void Bind(IParameter[] paras)
        {
            if (paras == null)
                return;
            _DataCubeObject = null;
            _Parameters = paras;
            DataObjectName = "Parameters";
            ShowSlicers(false);

            var dt = new DataTable();
            foreach (var pa in paras)
                dt.Columns.Add(new DataColumn(pa.Name, pa.GetVariableType()));

            int nrow = paras[0].ValueCount;
            dt.BeginLoadData();
            for (int i = 0; i < nrow; i++)
            {
                var dr = dt.NewRow();
                for (int j = 0; j < paras.Length; j++)
                    dr[j] = paras[j].GetValue(0, i, 0);
                dt.Rows.Add(dr);
            }
            dt.EndLoadData();
            SetSource(dt);
        }

        public void ShowView()
        {
            this.Show();
        }

        public void ClearContents()
        {
            Bind(new DataTable());
        }

        public void Clear()
        {

        }

        #endregion

        #region Grid

        /// <summary>
        /// Attaches a table. The view is detached first so that the grid does not keep the columns of the
        /// previous table while the new ones are generated.
        /// </summary>
        private void SetSource(DataTable table)
        {
            _DataTable = table;
            datagrid.ItemsSource = null;
            datagrid.Columns.Clear();

            if (table != null)
                datagrid.ItemsSource = table.DefaultView;

            btnSave.IsEnabled = _DataCubeObject != null || _Parameters != null;
            UpdateState();
        }

        private void UpdateState()
        {
            if (_DataTable == null)
            {
                tbState.Text = "";
                return;
            }
            tbState.Text = string.Format("{0} rows x {1} columns", _DataTable.Rows.Count, _DataTable.Columns.Count);
        }

        private void ClearSourceObject()
        {
            _DataCubeObject = null;
            _Parameters = null;
            ShowSlicers(false);
        }

        private static DataTable CreateTable<T>(int ncol)
        {
            var dt = new DataTable();
            for (int c = 0; c < ncol; c++)
                dt.Columns.Add(new DataColumn("C" + c, typeof(T)));
            return dt;
        }

        private void datagrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (_DataTable == null || string.IsNullOrEmpty(e.PropertyName) || !_DataTable.Columns.Contains(e.PropertyName))
                return;

            string format = DisplayFormatOf(_DataTable.Columns[e.PropertyName].DataType);

            // A column of a data cube is often named after a variable or a date, so the name can hold
            // spaces, dashes or colons. Such a name is not a valid binding path and the column would stay
            // empty, therefore the cell is bound through the indexer of the DataRowView instead.
            if (NeedsIndexerBinding(e.PropertyName))
            {
                var binding = new Binding("[" + e.PropertyName + "]");
                binding.StringFormat = format;
                e.Column = new DataGridTextColumn() { Header = e.PropertyName, Binding = binding };
            }
            else if (format != null)
            {
                e.Column = new DataGridTextColumn()
                {
                    Header = e.PropertyName,
                    Binding = new Binding(e.PropertyName) { StringFormat = format }
                };
            }
        }

        private static bool NeedsIndexerBinding(string name)
        {
            if (!char.IsLetter(name[0]) && name[0] != '_')
                return true;
            for (int i = 1; i < name.Length; i++)
            {
                if (!char.IsLetterOrDigit(name[i]) && name[i] != '_')
                    return true;
            }
            return false;
        }

        private static string DisplayFormatOf(Type type)
        {
            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                return "0.###";
            if (type == typeof(DateTime))
                return "yyyy-MM-dd HH:mm";
            return null;
        }

        #endregion

        #region Slicing of a data cube

        private void ShowSlicers(bool visible)
        {
            spSlicers.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void FillSlicers(IDataCubeObject dc)
        {
            // The boxes raise SelectionChanged while they are filled, and a slice is already taken from
            // the cube once the boxes are complete.
            _FillingSlicers = true;

            cobVar.Items.Clear();
            cobTime.Items.Clear();
            cobCell.Items.Clear();

            if (dc.Variables != null)
            {
                foreach (var v in dc.Variables)
                    cobVar.Items.Add(v);
            }
            cobVar.SelectedIndex = dc.SelectedVariableIndex >= 0 && dc.SelectedVariableIndex < cobVar.Items.Count
                ? dc.SelectedVariableIndex : 0;

            int ntime = dc.Size[1];
            int ncell = dc.Size[2];
            // A box with ten thousand entries is of no use to anybody, the cube is not sliced beyond the
            // first thousand steps or cells.
            int maxtime = Math.Min(ntime, 1000);
            int maxcell = Math.Min(ncell, 1000);

            var times = new List<string>();
            if (dc.DateTimes != null && dc.DateTimes.Length > 0)
            {
                for (int i = 0; i < maxtime && i < dc.DateTimes.Length; i++)
                    times.Add(dc.DateTimes[i].ToString());
            }
            else
            {
                for (int i = 0; i < maxtime; i++)
                    times.Add((i + 1).ToString());
            }
            if (ntime > 1)
                times.Add(AllString);

            var cells = new List<string>();
            for (int i = 0; i < maxcell; i++)
                cells.Add((i + 1).ToString());
            if (ncell > 1)
                cells.Add(AllString);

            foreach (var t in times)
                cobTime.Items.Add(t);
            foreach (var c in cells)
                cobCell.Items.Add(c);

            // One time step is known: show every cell of it. One cell is known: show its whole series.
            // Otherwise show the first time step of every cell, which tells more than a single cell does.
            if (ntime <= 1)
            {
                cobTime.SelectedIndex = 0;
                cobCell.SelectedIndex = cells.Count - 1;
            }
            else if (ncell <= 1)
            {
                cobTime.SelectedIndex = times.Count - 1;
                cobCell.SelectedIndex = 0;
            }
            else
            {
                cobTime.SelectedIndex = 0;
                cobCell.SelectedIndex = cells.Count - 1;
            }

            _FillingSlicers = false;
        }

        private void Slicer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadFromCube();
        }

        private void ReloadFromCube()
        {
            if (_DataCubeObject == null || _FillingSlicers)
                return;
            if (cobVar.SelectedIndex < 0 || cobTime.SelectedIndex < 0 || cobCell.SelectedIndex < 0)
                return;

            int timeIndex = IsAllSelected(cobTime) ? -1 : cobTime.SelectedIndex;
            int cellIndex = IsAllSelected(cobCell) ? -1 : cobCell.SelectedIndex;

            if (timeIndex == -1 && cellIndex == -1)
            {
                MessageBox.Show(this, "All time steps and all cells should not be displayed at the same time.",
                    "Table View", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SetSource(_DataCubeObject.ToDataTable(cobVar.SelectedIndex, timeIndex, cellIndex));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to read the selected slice. " + ex.Message,
                    "Table View", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static bool IsAllSelected(ComboBox cob)
        {
            return cob.SelectedItem != null && cob.SelectedItem.ToString() == AllString;
        }

        #endregion

        #region Toolbar

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_DataCubeObject != null)
            {
                if (_DataCubeObject.SelectedVariableIndex == -1)
                {
                    _DataCubeObject.FromDataTable(_DataTable, -1, 0, -1);
                }
                else if (_DataCubeObject.Layout == DataCubeLayout.ThreeD)
                {
                    int timeIndex = IsAllSelected(cobTime) ? -1 : cobTime.SelectedIndex;
                    int cellIndex = IsAllSelected(cobCell) ? -1 : cobCell.SelectedIndex;
                    _DataCubeObject.FromDataTable(_DataTable, cobVar.SelectedIndex, timeIndex, cellIndex);
                }
                else
                {
                    _DataCubeObject.FromDataTable(_DataTable, 0, 0, 0);
                }
            }
            else if (_Parameters != null && _DataTable != null)
            {
                try
                {
                    int nrow = _Parameters[0].ValueCount;
                    if (_DataTable.Rows.Count == nrow && _DataTable.Columns.Count == _Parameters.Length)
                    {
                        for (int i = 0; i < nrow; i++)
                        {
                            var dr = _DataTable.Rows[i];
                            for (int j = 0; j < _Parameters.Length; j++)
                                _Parameters[j].SetValue(0, i, 0, dr[j]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, string.Format("Failed to save. Errors are found in the table: {0}. Please correct it before saving.", ex.Message),
                        "Table View", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (_DataTable == null)
                return;

            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "csv file|*.csv";
            dlg.FileName = "table.csv";
            if (dlg.ShowDialog(this) == true)
            {
                try
                {
                    var csv = new CSVFileStream(dlg.FileName);
                    csv.Save(_DataTable);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Failed to save the table. " + ex.Message,
                        "Table View", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion
    }
}
