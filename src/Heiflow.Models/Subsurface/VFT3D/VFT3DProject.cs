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
using Heiflow.Models.Generic.Project;
using Heiflow.Models.GeoSpatial;
using Heiflow.Models.Properties;
using Heiflow.Models.Subsurface.MT3DMS;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Forms;

namespace Heiflow.Models.Subsurface.VFT3D
{
     [Serializable]
    [Export(typeof(IProject))]
    public class VFT3DProject : ModflowProject
    {
         public VFT3DProject()
        {
            this.Name = "VFT3D Project";
            this.NameToShown = "VFT3D";
            this.Icon = Resources.mf16;
            this.LargeIcon = Resources.vft3dms;
            Description = "Variable Flow Three-Dimensional Transporation and Recation Model";
            Token = "VFT3D";
            SupportedVersions = new string[] { "v1.0.0"};
            SelectedVersion = SupportedVersions[0];
        }
        public override bool New(ICancelProgressHandler progress, bool ImportFromExistingModel)
        {
            var succ = true;
            System.IO.Directory.CreateDirectory(GeoSpatialDirectory);
            System.IO.Directory.CreateDirectory(ProcessingDirectory);
            System.IO.Directory.CreateDirectory(InputDirectory);
            System.IO.Directory.CreateDirectory(MFInputDirectory);
            System.IO.Directory.CreateDirectory(OutputDirectory);

            RelativeMapFileName = Name + ".dspx";
            FullProjectFileName = Path.Combine(AbsolutePathToProjectFile, Name + ".vhfx");

            if (!ImportFromExistingModel)
            {
                RelativeControlFileName = Name + ".nam";
                var model = new VFT3DModel()
                {
                    Project = this,
                    WorkDirectory = FullModelWorkDirectory,
                    ControlFileName = RelativeControlFileName
                };
                model.Initialize();
                succ = model.New(progress);
                model.Version = this.SelectedVersion;
                this.Model = model;
            }
            string phc_dbfile = Path.Combine(Application.StartupPath, "data\\pht3d_datab.dat");
            if(File.Exists(phc_dbfile))
            {
                var dest = Path.Combine(Application.StartupPath, "pht3d_datab.dat");
                File.Copy(phc_dbfile, dest, true);
            }
            SaveBatchRunFile();
            _IsDirty = true;
            return true;
        }

    }
}
