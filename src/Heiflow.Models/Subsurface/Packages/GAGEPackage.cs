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

using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface
{
    [PackageItem]
    [Export(typeof(IMFPackage))]
    [PackageCategory("Output Control", false)]
    public class GAGEPackage : MFPackage
    {
        public static string GAGEName = "GAGE";
        public GAGEPackage()
        {
            Name = "GAGE";
            _FullName = "Gage Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".gag";
            _PackageInfo.ModuleName = "GAGE";
            IsMandatory = false;
            Description = "Lakes and streams can be designated as having gaging stations located on them. For each such designated lake or stream, the time and a variety of other information will be written to a separate output file after each time step (and each transport time increment)  to facilitate graphical postprocessing of the calculated data.";
            Version = "GAGE";
            _Layer3DToken = "Well";
        }

        [DisplayablePropertyItem]
        public My2DMat<int> GagingInfo { get; set; }

        public override bool Load()
        {
            if (File.Exists(FileName))
            {
                int num = 0;
                StreamReader sr = new StreamReader(FileName);
                string line = sr.ReadLine();
                var strs = TypeConverterEx.Split<string>(line);
                num = int.Parse(strs[0]);
                var mat = new My2DMat<int>(3, num);
                for (int i = 0; i < num; i++)
                {
                    line = sr.ReadLine();
                    var buf = TypeConverterEx.Split<int>(line, 3);
                    mat.Value[0][i] = buf[0];
                    mat.Value[1][i] = buf[1];
                    mat.Value[2][i] = buf[2];
                }
                sr.Close();
                GagingInfo = mat;
                OnLoaded("Sucessfully loaded");
                return true;
            }
            else
            {             
                OnLoadFailed("Failed to load");
                return false;
            }
        }

        public override void CompositeOutput(MFOutputPackage mfout)
        {
            var mf = Owner as Modflow;

            if (mf.Packages.Keys.Contains(LakePackage.PackageName))
            {
                LAKOutputPackage lake_out = new LAKOutputPackage()
                {
                    Owner = mf,
                    Parent = mf.Packages[LakePackage.PackageName]
                };
                lake_out.Initialize();
                var uids = from vv in this.GagingInfo.Value[1] where vv < 0 select vv;
                foreach (var uid in uids)
                {
                    var finfo = (from info in mf.NameManager.MasterList where info.FID == -uid select info).First();
                    lake_out.OutputFilesInfo.Add(finfo);
                }
                mfout.Children.Add(lake_out);
            }
        }

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
             
        }
    }
}