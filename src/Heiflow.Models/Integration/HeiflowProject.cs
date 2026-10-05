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
using DotSpatial.Data;
using DotSpatial.Symbology;
using Heiflow.Core.Data;
using Heiflow.Core.Utility;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.GeoSpatial;
using Heiflow.Models.Properties;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Heiflow.Models.Integration
{
    [Serializable]
    [Export(typeof(IProject))]
    public class HeiflowProject : BaseProject
    {
        public HeiflowProject()
        {
            this.Name = "HEIFLOW Project";
            this.NameToShown = "HEIFLOW";
            this.Icon = Resources.UiRaster16;
            this.LargeIcon = Resources.UiRaster32;
            Description = "HEIFLOW model version 1.1.0";
            Token = "HEIFLOW";
            SupportedVersions = new string[] { "v1.1.0" };
            SelectedVersion = SupportedVersions[0];
            MODFLOWVersion = Subsurface.MODFLOWVersion.MFNWT;
            ProcessModule = Integration.ProcessModule.Hydrology;
        }
       [Category("Model")]
        public MODFLOWVersion MODFLOWVersion
        {
            get;
            set;
        }

        protected override string[] WorkingDirectories
        {
            get
            {
                return new string[] { GeoSpatialDirectory, ProcessingDirectory, InputDirectory, MFInputDirectory,
                    PRMSInputDirectory, ExtensionInputDirectory, WQDirectory, WRAInputDirectory, OutputDirectory, DatabaseDirectory };
            }
        }

        protected override string ControlFileExtension
        {
            get { return ".control"; }
        }

        protected override IBasicModel CreateModel(string controlFileName)
        {
            return new HeiflowModel()
            {
                Project = this,
                WorkDirectory = FullModelWorkDirectory,
                ControlFileName = controlFileName,
                ProcessModule = Integration.ProcessModule.Hydrology
            };
        }


    }
}
