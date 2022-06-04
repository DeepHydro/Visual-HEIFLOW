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
    public class CreateGwhAssimCoefFile : MapLayerRequiredTool
    {
        public CreateGwhAssimCoefFile()
        {
            Name = "Create GWH Assim Coef File";
            Category = "Preprocessing";
            Description = "";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            Neighbors = 5;
            Power = 2;
            GWHLayer = 1;
            MaxDistance = 40000;
        }

        [Category("Input")]
        [Description("The output filename")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OutputFileName { get; set; }

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
        [Description("The layer number of GWH")]
        public int GWHLayer { get; set; }

        [Category("Parameter")]
        [Description("The number of neighbors. If Neighbors<=0, all source sites will be used")]
        public int Neighbors { get; set; }
        [Category("Parameter")]
        [Description("The power used to calcuate weights")]
        public int Power { get; set; }
        [Category("Parameter")]
        [Description("The maximum distance allowed")]
        public float MaxDistance { get; set; }

        public override void Initialize()
        {
            this.Initialized = true;
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
            int nsource_sites = fs_source.DataTable.Rows.Count;
            int ntar_sites = fs_target.DataTable.Rows.Count;
            int progress = 0;
            int count = 1;
            double sumOfVa = 0;

            StreamWriter sw = new StreamWriter(OutputFileName);
            
            if (Neighbors > nsource_sites)
                Neighbors = nsource_sites;
            var known_sites = new Site[nsource_sites];
            for (int i = 0; i < nsource_sites; i++)
            {
                var cor = fs_source.Features[i].Geometry.Coordinate;
                var dr = fs_source.Features[i].DataRow;
                known_sites[i] = new Site()
                {
                    LocalX = cor.X,
                    LocalY = cor.Y,
                    ID = int.Parse(dr["ID"].ToString())
                };
            }
            for (int i = 0; i < ntar_sites; i++)
            {
                var dr = fs_target.Features[i].DataRow;
                var cor = fs_target.Features[i].Geometry.Coordinate;
                var site_intep = new Site()
                {
                    LocalX = cor.X,
                    LocalY = cor.Y,
                    ID = int.Parse(dr["HRU_ID"].ToString()),
                    Row = int.Parse(dr["ROW"].ToString()),
                    Col = int.Parse(dr["COLUMN"].ToString())
                };
              
                var neighborSites = FindNeareastSites(Neighbors, known_sites, site_intep);
                if(neighborSites != null)
                {
                    var coefs = new double[Neighbors];
                    sumOfVa = (from ss in neighborSites select ss.ParaValue).Sum();
                    for (int j = 0; j < Neighbors; j++)
                    {
                        coefs[j] = System.Math.Round( neighborSites[j].ParaValue / sumOfVa,3);
                    } 
                    string coefsstr = string.Join("\t",coefs);
                    var kownsiteid  = from ss in neighborSites select ss.ID;
                    string kownsiteidstr = string.Join("\t", kownsiteid);
                    string newline = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", site_intep.ID, GWHLayer, site_intep.Row, site_intep.Col, kownsiteidstr, coefsstr);
                    sw.WriteLine(newline);
                }
                progress = (i + 1) * 100 / ntar_sites;
                if (progress > count)
                {
                    cancelProgressHandler.Progress("Package_Tool", progress, "Caculating Cell: " + (i + 1));
                    count++;
                }
            }
            sw.Close();
            fs_source.Close();
            fs_target.Close();
            return true;

        }
        public Site[] FindNeareastSites(int count, Site[] knownSites, Site intepolatedSite)
        {
            foreach (var s in knownSites)
            {
                s.Distance = Distance(s, intepolatedSite);
                s.ParaValue = 1 / System.Math.Pow(s.Distance, Power);
            }

            var sites = knownSites.OrderBy(s => s.Distance);

            Site[] foundSites = new Site[count];

            for (int i = 0; i < count; i++)
            {
                foundSites[i] = sites.ElementAt(i);
            }
            if (foundSites[count - 1].Distance <= MaxDistance)
            {
                return foundSites;
            }
            else
            {
                return null;
            }
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
