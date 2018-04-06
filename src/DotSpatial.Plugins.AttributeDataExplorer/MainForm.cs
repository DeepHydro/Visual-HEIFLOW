// --------------------------------------------------------------------------------------------------------------------
// <copyright file="" company="DotSpatial Team">
//   (c) 2011; Released under Microsoft Public License (Ms-PL)
// </copyright>
// <summary>
//   View Attribute Data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DotSpatial.Plugins.AttributeDataExplorer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using DevExpress.Utils;
    using DevExpress.XtraEditors;
    using DotSpatial.Controls;
    using DotSpatial.Controls.Docking;
    using DotSpatial.Data;
    using DotSpatial.Symbology;

    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private readonly AppManager _App;
        private readonly LayerManager _layerManager;

        private SelectionSynchronization _SelectionSynchronization;
        private FeatureLayerDisplayFilter _Filter;
        private PersistLayoutInMemory _PersistLayout;
        private PreventFirstRowSelection _PreventFirstRowSelection;
        private ShowMessageWhenDataSourceIsNull _ShowNoDataMessage;
        public static bool IsDataBinding = false;
        public static bool IsLayoutRestoring = false;

        /// <summary>
        ///   Initializes a new instance of the MainForm class
        /// </summary>
        /// <param name = "app">The reference to the MapWindow map object.</param>
        public MainForm(AppManager app)
            : this()
        {
            this._App = app;

            _layerManager = app.Map.GetLayerManager();
            _layerManager.ActiveLayerChanged += new EventHandler(LayerManager_ActiveLayerChanged);

            _Filter = new FeatureLayerDisplayFilter(_App.Map, gridView1);
            _PersistLayout = new PersistLayoutInMemory(_App.Map, gridView1);
            _SelectionSynchronization = new SelectionSynchronization(_App.Map, gridView1);
            _PreventFirstRowSelection = new PreventFirstRowSelection(gridView1);
            _ShowNoDataMessage = new ShowMessageWhenDataSourceIsNull(gridView1);

            // When a project is opened the map looses all of its events so we re wireup
            app.SerializationManager.Deserializing += OnDeserializingProject;

            // If we are started after the application has been running, we want to show the selected layer.
            RefreshData(_layerManager.ActiveLayer);
        }

        private void OnDeserializingProject(object sender, SerializingEventArgs e)
        {
            _layerManager.WireUpMapEvents();
            _SelectionSynchronization.WireUpMapEvents();
        }

        public void UILoaded()
        {
            // If we are started after the application has been running, we want to show the selection for the selected layer.
            _SelectionSynchronization.ShowSelectionFromCurrentLayer(_App.Map);
        }

        private void LayerManager_ActiveLayerChanged(object sender, EventArgs e)
        {
            RefreshData(_layerManager.ActiveLayer);
        }

        private void BindData(DataTable table)
        {
            IsDataBinding = true;
            if (table != null)
            {
                // Hack: We use the TextChanged event as a "I'm about to databind" event.
                gridControl1.Text = table.GetHashCode().ToString();
            }
            gridControl1.DataSource = table;
            gridView1.PopulateColumns();
            IsDataBinding = false;

            if (table != null)
            {
               
                if (gridView1.Columns.Count >= 8)
                {
                    gridView1.BestFitMaxRowCount = 16;
                    gridView1.BestFitColumns();
                }
            }

            gridView1.OptionsView.ColumnAutoWidth = AreThereOnlyAFewColumns(table);
        }

        private void RefreshData(DotSpatial.Symbology.ILayer iLayer)
        {    
            DataTable table=null;
            if (iLayer == null)
                return;
            if (iLayer is IFeatureLayer)
            {
                var ifs = iLayer as IFeatureLayer;
                if(ifs.DataSet != null)
                    table = ifs.DataSet.DataTable;
            }
            else
                table = GetDataFromCurrentLayer(iLayer);
            BindData(table);
            if (iLayer != null)
                this.Text = String.Format("ADE - {0}", iLayer.LegendText);
            else
                this.Text = "ADE";
        }

        private bool AreThereOnlyAFewColumns(DataTable table)
        {
            if (table == null) return true;

            return table.Columns.Count <= 8;
        }

        private DataTable GetDataFromCurrentLayer(DotSpatial.Symbology.ILayer iLayer)
        {
            var layer = iLayer as IMapFeatureLayer;

            if (layer == null ||  layer.DataSet==null || layer.DataSet.Filename == null)
                return null;

            return FdoHelper.GetDataFromFile(layer.DataSet.Filename);

            // Using DS 
            //if (layer != null)
            //    return layer.DataSet.DataTable;
        }


        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            this.gridView1.OptionsBehavior.Editable = !(this.gridView1.OptionsBehavior.Editable);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                _layerManager.ActiveLayerChanged -= LayerManager_ActiveLayerChanged;
                _App.SerializationManager.Deserializing -= OnDeserializingProject;
                _SelectionSynchronization = null;
                _Filter.ShowAllFeatures();
                _Filter = null;
                _PersistLayout = null;
                _PreventFirstRowSelection = null;
                LayerManagerExt.ClearCache();
            }

            base.Dispose(disposing);
        }
    }
}