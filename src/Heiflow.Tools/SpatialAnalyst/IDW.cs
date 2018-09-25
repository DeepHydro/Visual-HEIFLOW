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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.SpatialAnalyst
{
    public class InverseDistanceWeighting : MapLayerRequiredTool
    {
        public InverseDistanceWeighting()
        {
            Name = "Inverse Distance Weighting";
            Category = "Spatial Analyst";
            Description = "Inverse Distance Weighting (IDW) is a quick deterministic interpolator that is exact. There are very few decisions to make regarding model parameters. It can be a good way to take a first look at an interpolated surface. However, there is no assessment of prediction errors, and IDW can produce bulls eyes around data locations. There are no assumptions required of the data.";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            OutputDataCube = "idw_out";
            Neighbors = 5;
            Power = 2;
            MaximumValue = 100;
            MinmumValue = 0;
            Start = DateTime.Now;
            TimeStep = 86400;
        }


        [Category("Input")]
        [Description("The name of  a data file that  contains obsvered time series at selected sites. The data file must contain a header line.")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DataFileName { get; set; }

        [Category("Input")]
        [Description("Source feature layer, which must be point feature layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor SourceFeatureLayer
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("Target feature layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor TargetFeatureLayer
        {
            get;
            set;
        }

        [Category("Optional")]
        [Description("The source feature file")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string SourceFeatureFile { get; set; }

        [Category("Optional")]
        [Description("The target feature file")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string TargetFeatureFile { get; set; }

        [Category("Parameter")]
        [Description("The number of neighbors. If Neighbors<=0, all source sites will be used")]
        public int Neighbors { get; set; }

        [Category("Parameter")]
        [Description("The start date time")]
        public DateTime Start { get; set; }

        [Category("Parameter")]
        [Description("The power used to calcuate weights")]
        public int Power { get; set; }

        [Category("Parameter")]
        [Description("The time step of the time series. its unit is second")]
        public int TimeStep
        {
            get;
            set;
        }
        [Category("Output")]
        [Description("The name of the output data cube")]
        public string OutputDataCube
        {
            get;
            set;
        }
        [Category("Parameter")]
        public bool AllowNegative { get; set; }
        [Category("Parameter")]
        public float MinmumValue { get; set; }
        [Category("Parameter")]
        public float MaximumValue { get; set; }

        public override void Initialize()
        {
            //this.Initialized = !TypeConverterEx.IsNull(SourceFeatureFile);
            //this.Initialized = !TypeConverterEx.IsNull(TargetFeatureFile);
            this.Initialized = !TypeConverterEx.IsNull(OutputDataCube);
            this.Initialized = File.Exists(DataFileName);
        }
        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            IFeatureSet fs_source = null;
            IFeatureSet fs_target = null;
            if (SourceFeatureLayer != null)
                fs_source = SourceFeatureLayer.DataSet as IFeatureSet;
            else if (TypeConverterEx.IsNotNull(SourceFeatureFile))
                fs_source = FeatureSet.Open(SourceFeatureFile);

            if (TargetFeatureLayer != null)
                fs_target = TargetFeatureLayer.DataSet as IFeatureSet;
            else if (TypeConverterEx.IsNotNull(TargetFeatureFile))
                fs_target = FeatureSet.Open(TargetFeatureFile);

            if (fs_source == null || fs_target == null)
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Failed. The inputs are invalid.");
                return false;
            }

            CSVFileStream csv = new CSVFileStream(DataFileName);
            var ts_data = csv.Load<double>();
            cancelProgressHandler.Progress("Package_Tool", 1, "Time series at known sites loaded");
            int nsource_sites = fs_source.DataTable.Rows.Count;
            int ntar_sites = fs_target.DataTable.Rows.Count;
            int nstep = ts_data.Size[0];
            int nsite_data = ts_data.Size[1];
            int progress = 0;
            int count = 1;
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
                DataCube<float> mat = new DataCube<float>(1, nstep, ntar_sites);
                mat.DateTimes = new DateTime[nstep];
                mat.Name = OutputDataCube;
                mat.TimeBrowsable = true;
                mat.AllowTableEdit = false;
                for (int i = 0; i < nsource_sites; i++)
                {
                    var cor = fs_source.Features[i].Geometry.Coordinate;
                    known_sites[i] = new Site()
                    {
                        LocalX = cor.X,
                        LocalY = cor.Y,
                        ID = i
                    };
                }
                for (int i = 0; i < ntar_sites; i++)
                {
                    var cor = fs_target.Features[i].Geometry.Coordinate;
                    var site_intep = new Site()
                    {
                        LocalX = cor.X,
                        LocalY = cor.Y,
                        ID = i
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
                                sumOfVa += vv * temp;
                                sumOfDis += temp;
                            }
                        }
                        if (sumOfDis != 0)
                        {
                            mat[0,j,i] = (float)(sumOfVa / sumOfDis);
                            if (!AllowNegative && mat[0, j, i] < 0)
                            {
                                mat[0, j, i] = MinmumValue;
                            }
                        }
                        else
                        {
                            mat[0, j, i] = MinmumValue;
                        }
                    }
                    progress = (i + 1) * 100 / ntar_sites;
                    if (progress > count)
                    {        
                        cancelProgressHandler.Progress("Package_Tool", progress, "Caculating Cell: " + (i + 1));
                        count++;
                    }
                }
                for (int j = 0; j < nstep; j++)
                {
                    mat.DateTimes[j] = Start.AddSeconds(j * TimeStep);
                }
                cancelProgressHandler.Progress("Package_Tool", 100, "");
                Workspace.Add(mat);
                fs_source.Close();
                fs_target.Close();
                return true;
            }
        }
        public Site[] FindNeareastSites(int count, Site[] knownSites, Site intepolatedSite)
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
        private double Distance(Site a, Site b)
        {
            double dx = a.LocalX - b.LocalX;
            double dy = a.LocalY - b.LocalY;
            return System.Math.Sqrt(dx * dx + dy * dy);
        }
    
        private float Distance(Coordinate a, Coordinate b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return (float)System.Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
