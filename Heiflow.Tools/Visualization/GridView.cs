// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using Heiflow.Models.Tools;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.Statisitcs
{
    public class ShowOnMap : ModelTool
    {
        public ShowOnMap()
        {
            Name = "Grid View";
            Category = "Visualization";
            Description = " ";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = false;
        }

        [Category("Input")]
        [Description("The matrix that is to be shown on the map")]
        public string Matrix
        {
            get;
            set;
        }

        public override void Initialize()
        {
            this.Initialized = Validate(Matrix);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var vector = GetVector(Matrix);
            if (vector != null)
            {
                cancelProgressHandler.Progress("Package_Tool", 10, "Calculating...");
                var dt = ProjectService.Project.Model.Grid.FeatureSet.DataTable;
                if (vector.Length == dt.Rows.Count)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i][RegularGrid.ParaValueField] = vector[i];
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void AfterExecution(object args)
        {
            var gridLayer = ProjectService.Project.GridLayer;
            WorkspaceView.RefreshLayerBy(gridLayer, RegularGrid.ParaValueField);
        }
    }
}
