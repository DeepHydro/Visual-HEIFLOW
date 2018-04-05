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
    public class ImportGsflowProperty : IImportProperty
    {
        private string contro_filename;
 

        public ImportGsflowProperty()
        {
            Token = "GSFLOW";
            CopyFiles = false;
        }
        [Browsable(false)]
        public string Token
        {
            get;
            private  set;
        }

        [Category("File")]
        [Description("The master file name for the imported model")]
        [EditorAttribute(typeof(ControlFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
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

        [Category("File")]
        [Description("Copy model files into the project directory when importing the model")]
        public bool CopyFiles
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
