// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Projections;
using Heiflow.Controls.WinForm.Controls;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Models.Generic.Project;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Forms.Design;

namespace Heiflow.Controls.WinForm.Project
{
    [Export(typeof(IImportProperty))]
    public class ImportModflowProperty : IImportProperty
    {
        private string contro_filename;


        public ImportModflowProperty()
        {
            Token = "Modflow2005";
        }
        [Browsable(false)]
        public string Token
        {
            get;
            private  set;
        }

        [Category("File")]
        [Description("The master file name for the imported model")]
        [EditorAttribute(typeof(ModflowControlFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string FileName
        {
            get
            {
                return contro_filename;
            }
            set
            {
                contro_filename = value;
                WorkDirectory = Path.GetDirectoryName(contro_filename);
            }
        }

        [Category("File")]
        [Description("The working directory for the imported model")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string WorkDirectory
        {
            get;
            set;
        }

        [Category("Grid")]
        [Description("The X coordinate of upper left grid origin")]   
        public double OriginX
        {
            get;
            set;
        }
        [Category("Grid")]
        [Description("The Y coordinate of upper left grid origin")]
        public double OriginY
        {
            get;
            set;
        }
        [Category("Spatial Reference")]
        [Description("The spatial reference of the grid")]
        [EditorAttribute(typeof(SRSEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ProjectionInfo Projection
        {
            get;
            set;
        }
        [Category("Time")]
        [Description("The starting time of model")]
        public DateTime Start
        {
            get;
            set;
        }

    }
}
