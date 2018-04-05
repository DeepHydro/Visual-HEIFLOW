// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Core.IO;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using HUST.WREIS.Numerics.Local;
using ILNumerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.SpatialAnalyst
{
    public class GradientInverseDistanceWeighting : ModelTool
    {
        private string _ValueField;
        private int _SelectedVarIndex = 0;
        private string _FeatureFileName;

        public GradientInverseDistanceWeighting()
        {
            Name = "Gradient Inverse Distance Weighting";
            Category = "Spatial Analyst";
            Description = "Gradient Inverse Distance Weighting (GIDW) is a quick deterministic interpolator based on IDW plus condsidering elevations";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            OutputMatrix = "gidw_out";
            Neighbors = 8;
            Power = 2;
            Start = DateTime.Now;
            TimeStep = 86400;
            MinmumValue = 0;
            MaximumValue = 9000;

            this.DataFileName = @"E:\Project\HRB\HEIFLOW\HRB1970-2016\pcp.csv";
            this.TargetFeatureFile = @"E:\Project\HRB\HEIFLOW\Geospatial\Climate\UTM47N\XZ_Stations.shp";
            this.SourceFeatureFile = @"E:\Project\HRB\HEIFLOW\Geospatial\Climate\UTM47N\Weather_Stations.shp";
        }


        [Category("Input")]
        [Description("The data file name which contains obsvered time series at selected sites")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DataFileName { get; set; }


        [Category("Input")]
        [Description("The target feature file")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string TargetFeatureFile { get; set; }

        [Category("Input")]
        [Description("The source feature name")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string SourceFeatureFile
        {
            get
            {
                return _FeatureFileName;
            }
            set
            {
                _FeatureFileName = value;
                if (File.Exists(_FeatureFileName))
                {
                    var  _SourceFeatureSet = FeatureSet.Open(_FeatureFileName);
                    var buf = from DataColumn dc in _SourceFeatureSet.DataTable.Columns select dc.ColumnName;
                    Fields = buf.ToArray();
                    _SourceFeatureSet.Close();
                }
            }
        }

        [Category("Parameter")]
        [Description("Name of the elevation")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string ElevationField
        {
            get
            {
                return _ValueField;
            }
            set
            {
                _ValueField = value;
                if (Fields != null)
                {
                    for (int i = 0; i < Fields.Length; i++)
                    {
                        if (_ValueField == Fields[i])
                        {
                            _SelectedVarIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
        }

        [Category("Parameter")]
        [Description("The number of neighbors. If Neighbors<=0, all source sites will be used")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int Neighbors { get; set; }

        [Category("Parameter")]
        [Description("The start date time")]
        public DateTime Start { get; set; }

        [Category("Parameter")]
        [Description("The power used to calcuate weights")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int Power { get; set; }

        [Category("Parameter")]
        public bool AllowNegative { get; set; }
        [Category("Parameter")]
        public float MinmumValue { get; set; }
        [Category("Parameter")]
        public float MaximumValue { get; set; }
        [Category("Parameter")]
        [Description("The time step of the time series. its unit is second")]
        public int TimeStep
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("The name of the output matrix")]
        public string OutputMatrix
        {
            get;
            set;
        }


        public override void Initialize()
        {
            this.Initialized = !TypeConverterEx.IsNull(SourceFeatureFile);
            this.Initialized = !TypeConverterEx.IsNull(TargetFeatureFile);
            this.Initialized = !TypeConverterEx.IsNull(OutputMatrix);
            this.Initialized = File.Exists(DataFileName);
            this.Initialized = TypeConverterEx.IsNotNull(ElevationField);
        }


        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var fs_source = FeatureSet.Open(SourceFeatureFile);
            var fs_target = FeatureSet.Open(TargetFeatureFile);
            CSVFileStream csv = new CSVFileStream(DataFileName);
            var ts_data = csv.Load<double>();
            cancelProgressHandler.Progress("Package_Tool", 1, "Time series at known sites loaded");
            int nsource_sites = fs_source.DataTable.Rows.Count;
            int ntar_sites = fs_target.DataTable.Rows.Count;
            int nstep = ts_data.Size[0];
            int nsite_data = ts_data.Size[1];
            int progress = 0;
            double sumOfDis = 0;
            double sumOfVa = 0;

            if (nsite_data != nsource_sites)
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "the number of sites in the data file dose not match to that in the source feature file");
                return false;
            }
            else
            {
                if (Neighbors > nsource_sites)
                    Neighbors = nsource_sites;
                var known_sites = new Site[nsource_sites];
                My3DMat<float> mat = new My3DMat<float>(1, nstep, ntar_sites);
                mat.DateTimes = new DateTime[nstep];
                mat.Name = OutputMatrix;
                mat.TimeBrowsable = true;
                mat.AllowTableEdit = false;

                double[][] xvalues = new double[nsource_sites][];
                double[] yvalues = new double[nsource_sites];
                for (int i = 0; i < nsource_sites; i++)
                {
                    var cor = fs_source.Features[i].Geometry.Coordinate;
                    var ele = double.Parse(fs_source.Features[i].DataRow[_ValueField].ToString());
                    xvalues[i] = new double[3];
                    known_sites[i] = new Site()
                    {
                        LocalX = cor.X,
                        LocalY = cor.Y,
                        ID = i,
                        Elevation = ele
                    };
                    xvalues[i][0] = cor.X;
                    xvalues[i][1] = cor.Y;
                    xvalues[i][2] = ele;
                }
                  for (int i = 0; i < nsource_sites; i++)
                  {
                      var count = 0;
                      double sum = 0;
                      for (int t = 0; t < nstep; t++)
                      {
                          if (ts_data.GetValue(t, i) < MaximumValue)
                          {
                              sum += ts_data.GetValue(t, i);
                              count++;
                          }
                      }
                      yvalues[i] = sum / count;
                  }
                  MultipleLinearRegression mlineRegrsn = new MultipleLinearRegression(xvalues, yvalues, true);
                  Matrix result = new Matrix();
                  mlineRegrsn.ComputeFactorCoref(result);
                  double[] coeffs = result[0, Matrix.mCol];
                  var cx = coeffs[0];
                  var cy = coeffs[1];
                  var ce = coeffs[2];
                  cancelProgressHandler.Progress("Package_Tool", 2, "Regression coefficients calculated");

                  for (int i = 0; i < ntar_sites; i++)
                  {
                
                      var cor = fs_target.Features[i].Geometry.Coordinate;
                      var site_intep = new Site()
                      {
                          LocalX = cor.X,
                          LocalY = cor.Y,
                          ID = i,
                          Elevation = double.Parse(fs_target.Features[i].DataRow[_ValueField].ToString())
                      };
                      var neighborSites = FindNeareastSites(Neighbors, known_sites, site_intep);
                      for (int j = 0; j < nstep; j++)
                      {
                          sumOfDis = 0;
                          sumOfVa = 0;
                          foreach (var nsite in neighborSites)
                          {
                              var vv = ts_data.GetValue(j, nsite.ID);
                              if (vv < MaximumValue)
                              {
                                  double temp = 1 / System.Math.Pow(nsite.Distance, Power);
                                  double gd = (site_intep.LocalX - nsite.LocalX) * cx + (site_intep.LocalY - nsite.LocalY) * cy + (site_intep.Elevation - nsite.Elevation) * ce;
                                  sumOfVa += vv * temp;
                                  sumOfDis += temp;
                              }
                          }
                          if (sumOfDis != 0)
                          {
                              mat.Value[0][j][i] = (float)(sumOfVa / sumOfDis);
                              if (!AllowNegative && mat.Value[0][j][i] <0)
                              {
                                  mat.Value[0][j][i] = MinmumValue;
                              }
                          }
                          else
                          {
                              mat.Value[0][j][i] = MinmumValue;
                          }
                      }
                      progress = (i + 1) * 100 / ntar_sites;
                      cancelProgressHandler.Progress("Package_Tool", progress, "Caculating Cell: " + (i + 1));
                  }
                  for (int j = 0; j < nstep; j++)
                  {
                      mat.DateTimes[j] = Start.AddSeconds(j * TimeStep);
                  }
                cancelProgressHandler.Progress("Package_Tool",100,"");
                Workspace.Add(mat);
                fs_source.Close();
                fs_target.Close();
                return true;
            }
        }

        private Site[] FindNeareastSites(int count, Site[] knownSites, Site intepolatedSite)
        {
            foreach (var s in knownSites)
            {
                s.Distance = Distance(s, intepolatedSite);
            }

            var sites = knownSites.OrderBy(s => s.Distance);

            Site[] foundSites = new Site[count];

            for (int i = 0; i < count; i++)
            {
                foundSites[i] = sites.ElementAt(i);
            }
            return foundSites;
        }
        private float Distance(Coordinate a, Coordinate b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return (float)System.Math.Sqrt(dx * dx + dy * dy);
        }
        private double Distance(Site a, Site b)
        {
            double dx = a.LocalX - b.LocalX;
            double dy = a.LocalY - b.LocalY;
            return System.Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
