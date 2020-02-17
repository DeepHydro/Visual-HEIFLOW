using DotSpatial.Data;
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation.Services;
using NetTopologySuite.Geometries;
using System.ComponentModel;
using System.Data;

namespace Heiflow.Tools.DataManagement
{
    public class CreateStationLayer : ModelTool
    {
        public CreateStationLayer()
        {
            Name = "Create Station Layer from ODM";
            Category = "Data Management";
            Description = "Create Station Layer from ODM";  
            Version = "1.0.0.0";
            this.Author = "Yong Tian";

            SQLQuery = "select * from sites";
            LayerName = "stations";
            ShpFileName = "stations.shp";
        }

        [Category("Input")]
        [Description("The SQL query string")]
        public string SQLQuery { get; set; }
        [Category("Input")]
        [Description("The number of variables")]
        public string LayerName { get; set; }

        [Category("Output")]
        [Description("The output filename")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ShpFileName
        {
            get;
            set;
        }
        public override void Initialize()
        {
            this.Initialized = (TypeConverterEx.IsNotNull(SQLQuery) || TypeConverterEx.IsNotNull(ShpFileName));
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            if( prj.Project.ODMSource != null )
            {
                var table = prj.Project.ODMSource.GetDataTable(SQLQuery);
                var sites = prj.Project.ODMSource.GetSites(SQLQuery);
                if (sites != null)
                {
                    FeatureSet fs = new FeatureSet(FeatureType.Point);
                    fs.Name = LayerName;
                    //fs.Projection = this.Projection;
                    fs.DataTable.Columns.Add(new DataColumn("SiteID", typeof(int)));
                    fs.DataTable.Columns.Add(new DataColumn("SiteName", typeof(string)));
                    fs.DataTable.Columns.Add(new DataColumn("Longitude", typeof(double)));
                    fs.DataTable.Columns.Add(new DataColumn("Latitude", typeof(double)));
                    fs.DataTable.Columns.Add(new DataColumn("Elevation_m", typeof(double)));
                    foreach (var site in sites)
                    {
                        var vertice = new Coordinate(site.Longitude, site.Latitude);
                        Point geom = new Point(vertice);
                        IFeature feature = fs.AddFeature(geom);
                        feature.DataRow.BeginEdit();
                        feature.DataRow["SiteID"] = site.ID;
                        feature.DataRow["SiteName"] = site.Name;
                        feature.DataRow["Longitude"] = site.Longitude;
                        feature.DataRow["Latitude"] = site.Latitude;
                        feature.DataRow["Elevation_m"] = site.Elevation;
                        feature.DataRow.EndEdit();
                    }
                    WorkspaceView.OutputTo(table);
                    fs.SaveAs(ShpFileName, true);
                }
                else
                {
                    cancelProgressHandler.Progress("Package_Tool", 90, "No sations found");
                }
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 90, "No ODM Database loaded");
            }
            return true;
        }
    }
}
