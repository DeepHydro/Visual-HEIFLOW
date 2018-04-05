// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Data;
using System.IO;
using Heiflow.Models.Generic;
using System.ComponentModel;
using Heiflow.Controls.WinForm.Editors;
using System.Windows.Forms.Design;
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;

namespace Heiflow.Tools.Conversion
{
    public class FromRasterList : ModelTool
    {
        public FromRasterList()
        {
            Name = "From Raster Lists";
            Category = "Conversion";
            Description = "Extract data cube from a set of rasters to a given point shapefile";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
 
            VariableName = "unknown";
        }
       [Category("Input")]
       [Description("A text file that contains the list of raster file names")]
       [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string FilenameList
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The name of exported variable")]
        public string VariableName
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The target feature file")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string TargetFeatureFile { get; set; }

         [Category("Output")]
         [Description("The name of the output matrix")]
        public string OutputMatrix { get; set; }

        public override void Initialize()
        {
            Initialized = File.Exists(TargetFeatureFile);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var fs =  FeatureSet.Open(TargetFeatureFile);
            if (fs != null && File.Exists(FilenameList))
            {
                var npt = fs.NumRows();
                Coordinate[] coors = new Coordinate[npt];
                int progress = 0;
                for (int i = 0; i < npt; i++)
                {
                    var geo_pt = fs.GetFeature(i).Geometry;
                    coors[i] = geo_pt.Coordinate;
                }
                List<string> files = new List<string>();
                StreamReader sr = new StreamReader(FilenameList);
                while(!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (TypeConverterEx.IsNotNull(line))
                        files.Add(line.Trim());
                }
                sr.Close();
                if (files != null)
                {
                    int nstep = files.Count();
                    var mat_out = new My3DMat<float>(1, nstep, npt);
                    mat_out.Name = OutputMatrix;
                    mat_out.Variables = new string[] { VariableName };
                    mat_out.TimeBrowsable = true;
                    mat_out.AllowTableEdit = false;
                    for (int t = 0; t < nstep; t++)
                    {
                        IRaster raster = Raster.Open(files[t]);
                        for (int i = 0; i < npt; i++)
                        {
                            var cell = raster.ProjToCell(coors[i]);
                            if (cell != null && cell.Row > 0)
                                mat_out.Value[0][t][i] = (float)raster.Value[cell.Row, cell.Column];
                            else
                                mat_out.Value[0][t][i] = 0;
                        }
                        progress = t * 100 / nstep;
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing raster:" + files[t]);
                    }
                    Workspace.Add(mat_out);
                    fs.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 50, "Failed to run. The input parameters are incorrect.");
                return false;
            }
        }
    }
}