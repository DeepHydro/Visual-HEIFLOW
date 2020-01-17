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

using DotSpatial.Controls;
using DotSpatial.Modeling.Forms;
using DotSpatial.Symbology;
using Heiflow.Applications;
using Heiflow.Controls.Tree;
using Heiflow.Controls.WinForm.Properties;
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.Tools;
using Heiflow.Models.UI;
using Heiflow.Presentation;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Heiflow.Controls.WinForm.Toolbox
{
    public partial class ModelToolManager : UserControl, IPartImportsSatisfiedNotification, IModelToolManager, IWorkspaceView,IChildView
    {
        private ModelWorkspace _ModelWorkspace;
        private TreeModel _model;
        private ContextMenuStrip _contextMenu;
        private IModelTool toolToExecute;
        private IProjectService _ProjectService;
        private ObservableCollection<MatMeta> _MatMataList;
        private List<VariableMeta> _VariableMataList;
        private IMessageService _MessageService;

        public ModelToolManager()
        {
            InitializeComponent();
            _ModelWorkspace = new ModelWorkspace();
            _ModelWorkspace.DataSources.CollectionChanged += DataSources_CollectionChanged;
            _model = new TreeModel();
            treeView1.Model = _model;
            this._nodeStateIcon.DataPropertyName = "Image";
            this._nodeTextBox.DataPropertyName = "Text";
            _contextMenu = new ContextMenuStrip();
            this.treeView1.BackColor = Color.White; //ColorTranslator.FromHtml("#FF2D2D30");
            this.treeView1.BorderStyle = BorderStyle.None;
            treeView1.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;
            _MatMataList = new ObservableCollection<MatMeta>();
            _MatMataList.CollectionChanged += MatMataList_CollectionChanged;
            _VariableMataList = new List<VariableMeta>();
            this.Load += ModelToolManager_Load;
        }


        /// Gets the list tools available.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ModelWorkspace Workspace
        {
            get
            {
                return _ModelWorkspace;
            }
        }

        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }

        [Browsable(false)]
        public ImageList ImageList
        {
            get
            {
                return imageList1;
            }
        }
           [Browsable(false)]
        public string ChildName
        {
            get { return "ModelToolManager"; }
        }

        public void OnImportsSatisfied()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    RefreshTree();
                }));
            }
            else
            {
                RefreshTree();
            }
        }
        private void ModelToolManager_Load(object sender, EventArgs e)
        {
            _MessageService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IMessageService>();
        }

        public virtual void RefreshTree()
        {
            _ProjectService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            treeView1.BeginUpdate();
            _model.Nodes.Clear();

            var groups = from tool in ProjectManager.Tools group tool by tool.Category into gp select new { cat = gp.Key, items = gp };
            var keys = (from gp in groups select gp.cat).OrderBy(c => c);
            foreach(var key in keys)
            {
                var cat = (from gp in groups where gp.cat == key select gp).First();
                Node node_folder = new Node(key)
                {
                    Image = Resources.toolbox16,
                    Tag = key
                };
                foreach (var tool in cat.items)
                {
                    tool.Workspace = this.Workspace;
                    tool.WorkspaceView = this;
                    tool.BindProjectService(_ProjectService);
                    Node node_tool = new Node(tool.Name)
                    {
                        Image = Resources.hammer16,
                        Tag = tool
                    };
                    node_folder.Nodes.Add(node_tool);
                }
                _model.Nodes.Add(node_folder);

            }
            //foreach (var gg in groups)
            //{
            //    Node node_folder = new Node(gg.cat)
            //    {
            //        Image = Resources.toolbox16,
            //        Tag = gg.cat
            //    };
            //    foreach (var tool in gg.items)
            //    {
            //        tool.Workspace = this.Workspace;
            //        tool.WorkspaceView = this;
            //        tool.BindProjectService(_ProjectService);
            //        Node node_tool = new Node(tool.Name)
            //        {
            //            Image = Resources.hammer16,
            //            Tag = tool
            //        };
            //        node_folder.Nodes.Add(node_tool);
            //    }
            //    _model.Nodes.Add(node_folder);
            //}

            treeView1.EndUpdate();
        }
  
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var node = e.Node.Tag as Node;
            toolToExecute = node.Tag as IModelTool;
            tabControlCmd.SelectedTab = tabPageCommand;
            if (toolToExecute != null)
            {
                propertyGrid1.SelectedObject = toolToExecute;
            }
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            if (toolToExecute == null)
                return;

            toolToExecute.Initialize();
            if (!toolToExecute.Initialized)
                return;

            DotSpatial.Modeling.Forms.ToolProgress progForm = new DotSpatial.Modeling.Forms.ToolProgress(1);
            if (toolToExecute.MultiThreadRequired)
            {
                //We create a background worker thread to execute the tool
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += BwDoWork;
                bw.RunWorkerCompleted += ExecutionComplete;

                object[] threadParameter = new object[2];
                threadParameter[0] = toolToExecute;
                threadParameter[1] = progForm;

                // Show the progress dialog and kick off the Async thread
                progForm.Show(this);
                bw.RunWorkerAsync(threadParameter);
            }
            else
            {
                try
                {
                    System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                    toolToExecute.Execute(progForm);
                    toolToExecute.AfterExecution(null);
                    System.Windows.Forms.Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    _MessageService.ShowError(null, ex.Message);
                }
       
            }
        }

        private static void BwDoWork(object sender, DoWorkEventArgs e)
        {
            object[] threadParameter = e.Argument as object[];
            if (threadParameter == null) return;
            IModelTool toolToExecute = threadParameter[0] as IModelTool;
            ToolProgress progForm = threadParameter[1] as ToolProgress;
            if (progForm == null) return;
            if (toolToExecute == null) return;
            progForm.Progress(String.Empty, 0, "==================");
            progForm.Progress(String.Empty, 0, String.Format("Executing Tool: {0}", toolToExecute.Name));
            progForm.Progress(String.Empty, 0, "==================");

            try
            {
                toolToExecute.Execute(progForm);
            }
            catch (Exception ex)
            {
                progForm.Progress(String.Empty, 100, "Failed to run. Errors:" + ex.Message);
            }
            finally
            {
                progForm.ExecutionComplete();
                progForm.Progress(String.Empty, 100, "==================");
                progForm.Progress(String.Empty, 100, String.Format("Done Executing Tool: {0}", toolToExecute.Name));
                progForm.Progress(String.Empty, 100, "==================");
            }
        }

        private void ExecutionComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (toolToExecute == null)
                return;
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    toolToExecute.AfterExecution(e);
                }));
            }
            else
            {
                toolToExecute.AfterExecution(e);
            }
         
        }

        private void DataSources_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var mat = e.OldItems[0] as DataCube<float>;
                var matmata = from mm in _MatMataList where mm.Name == mat.Name select mm;
                if(matmata.Any())
                    _MatMataList.Remove(matmata.First());
              
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var mat = e.NewItems[0] as DataCube<float>;
                string size = mat.SizeString();         
                _MatMataList.Add(new MatMeta() { Name = mat.Name, Size = size, Mat = mat });
            }    
        }


        private void MatMataList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateMatView();
        }

        private void olvMatName_CellEditFinished(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            if (e.Column.Index == 0)
            {
                var mat = e.ListViewItem.Tag as DataCube<float>;
                if (mat != null)
                {
                    mat.Name = e.NewValue.ToString();
                }
            }
        }

        private void olvMatName_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var meta = olvMatName.SelectedObject as MatMeta;
            if (meta != null && meta.Mat != null)
            {
                UpdateVariableView(meta.Mat);
            }
            else
                olvVariableName.Items.Clear();
        }

        private void olvMatName_MouseUp(object sender, MouseEventArgs e)
        {
            if (olvMatName.SelectedObject != null)
            {
                menu_Open.Enabled = true;
                menu_remove.Enabled = true;
                menu_SaveAs.Enabled = true;
            }
            else
            {
                menu_Open.Enabled = false;
                menu_remove.Enabled = false;
                menu_SaveAs.Enabled = false;
            }
        }

        private void menu_Clear_Click(object sender, EventArgs e)
        {
            Workspace.Clear();
            _MatMataList.Clear();
            _VariableMataList.Clear();
            olvMatName.Items.Clear();
            olvVariableName.Items.Clear();
        }


        private void UpdateVariableView(DataCube<float> mat)
        {
            if (mat == null)
            {
                _VariableMataList.Clear();
            }
            else
            {
                _VariableMataList.Clear();

                int nvar = mat.Size[0];
                string size = "empty";
                for (int i = 0; i < nvar; i++)
                {
                    if (mat[i] != null)
                    {
                        if (mat.Flags[i] == TimeVarientFlag.Individual)
                            size = string.Format("{0}×{1}", mat.Size[1], mat.GetSpaceDimLength(i, 0));
                        else
                            size = string.Format("{0}×1", mat.Size[1]);
                    }
                    else
                    {
                        size = "empty";
                    }
                    _VariableMataList.Add(new VariableMeta() { Index = i.ToString(), Variable = mat.Variables[i], Size = size, Mat = mat, Max = "none", Min = "none" });
                }
             
            }
            olvVariableName.SetObjects(_VariableMataList);
        }

        private void menu_Open_Click(object sender, EventArgs e)
        {
            var meta = olvMatName.SelectedObject as MatMeta;
            if (meta != null && meta.Mat != null)
            {
                var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
                shell.SelectPanel(DockPanelNames.DataGridPanel);
                shell.DataGridView.Bind(meta.Mat);
            }
        }

        private void menu_Remove_Click(object sender, System.EventArgs e)
        {
            var meta = olvMatName.SelectedObject as MatMeta;
            if (meta != null && meta.Mat != null)
            {
                Workspace.Remove(meta.Name);
                UpdateVariableView(null);
            }
        }

        private void menu_SaveAs_Click(object sender, EventArgs e)
        {
            var meta = olvMatName.SelectedObject as MatMeta;
            if (meta != null && meta.Mat != null)
            {
                SaveDcxForm dlg = new SaveDcxForm(meta.Mat);

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var index = dlg.CheckedIndex;
                    if (index.Count() > 0)
                    {
                        System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                        DataCubeStreamWriter asx = new DataCubeStreamWriter(dlg.FileName);
                        asx.WriteAll(meta.Mat, index);
                        System.Windows.Forms.Cursor.Current = Cursors.Default;
                    }
                }
            }
        }

        private void menu_refresh_Click(object sender, EventArgs e)
        {
            UpdateMatView();
        }

        private void btn_open_dcx_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "data cube file|*.dcx";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                DataCubeStreamReader asx = new DataCubeStreamReader(dlg.FileName);
               asx.LoadDataCube();
                var  mat = asx.DataCube;
                mat.Name = Path.GetFileNameWithoutExtension(dlg.FileName);
                Workspace.Add(mat);
                System.Windows.Forms.Cursor.Current = Cursors.Default;
            }
        }

        private void UpdateMatView()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    olvMatName.SetObjects(_MatMataList);
                }));
            }
            else
            {
                olvMatName.SetObjects(_MatMataList);
            } 
        }


        public void OutputTo(DataTable dt)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    this.dataGridEx1.Bind(dt);
                }));
            }
            else
            {
                this.dataGridEx1.Bind(dt);
            }
        }

        public void OutputTo(string msg)
        {
            this.txt_msg.Text = msg;
        }
    
        public void ShowView(IWin32Window pararent)
        {

        }
        public void Plot<T>(T[] xx, T[] yy, string seriesName = "", MySeriesChartType chartType = MySeriesChartType.FastLine)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    this.winChart_timeseries.Plot<T>(xx, yy, seriesName,
            EnumHelper.FromString<SeriesChartType>(chartType.ToString()));
                    this.winChart_timeseries.Refresh();
                }));
            }
            else
            {
                this.winChart_timeseries.Plot<T>(xx, yy, seriesName,
                 EnumHelper.FromString<SeriesChartType>(chartType.ToString()));
                this.winChart_timeseries.Refresh();
            }
        }
        public void Plot<T>(T[] xx, T[] yy, T nodata, string seriesName = "", MySeriesChartType chartType = MySeriesChartType.FastLine)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    this.winChart_timeseries.Plot<T>(xx, yy, nodata, seriesName,
            EnumHelper.FromString<SeriesChartType>(chartType.ToString()));
                    this.winChart_timeseries.Refresh();
                }));
            }
            else
            {
                this.winChart_timeseries.Plot<T>(xx, yy,nodata, seriesName,
                 EnumHelper.FromString<SeriesChartType>(chartType.ToString()));
                this.winChart_timeseries.Refresh();
            }
        }
        public void Plot<T>(T[] yy, string seriesName, MySeriesChartType chartType)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    this.winChart_timeseries.Plot<T>(yy, seriesName,
            EnumHelper.FromString<SeriesChartType>(chartType.ToString()));
                    this.winChart_timeseries.Refresh();
                }));
            }
            else
            {
                this.winChart_timeseries.Plot<T>(yy, seriesName,
                 EnumHelper.FromString<SeriesChartType>(chartType.ToString()));
                this.winChart_timeseries.Refresh();
            }
        }

        public void Plot<T>(DataCube<T> source, MySeriesChartType chartType)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    this.winChart_timeseries.Plot<T>(source, EnumHelper.FromString<SeriesChartType>(chartType.ToString()));
                    this.winChart_timeseries.Refresh();
                }));
            }
            else
            {
                this.winChart_timeseries.Plot<T>(source, EnumHelper.FromString<SeriesChartType>(chartType.ToString()));
                this.winChart_timeseries.Refresh();
            }
        }

        public void RefreshChart()
        {
            this.Refresh();
            this.winChart_timeseries.Refresh();
        }

        public void RefreshLayerBy(object layer, string fieldName)
        {
            var gridLayer = layer as IMapFeatureLayer;
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    ApplyScheme(gridLayer, fieldName);
                }));
            }
            else
            {
                ApplyScheme(gridLayer, fieldName);
            }
        }

        public void ApplyScheme(IMapFeatureLayer gridLayer, string fieldName)
        {
            IFeatureScheme newScheme = gridLayer.Symbology;
            if (newScheme != null)
            {
                newScheme.EditorSettings.NumBreaks = 5;
                newScheme.EditorSettings.UseGradient = true;
                newScheme.EditorSettings.ClassificationType = ClassificationType.Quantities;
                newScheme.EditorSettings.FieldName = fieldName;
                newScheme.CreateCategories(gridLayer.DataSet.DataTable);
                newScheme.ResumeEvents();
                gridLayer.ApplyScheme(newScheme);
            }
        }

        public void ClearContent()
        {
            _MatMataList.Clear();
            _ModelWorkspace.Clear();
            olvMatName.Clear();
            olvVariableName.Clear();
            propertyGrid1.SelectedObject = null;
            richTextBox1.Clear();
        }
        public void InitService()
        {

        }
    }
}
