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
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.UI;
using ILNumerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface.MT3D
{
    [PackageItem]
    [PackageCategory("Basic", true)]
    [CoverageItem]
    [Export(typeof(IMFPackage))]
    public class VDFPackage : MFPackage
    {
        public static string PackageName = "VDF";
        public VDFPackage()
        {
            Name = VDFPackage.PackageName;
            _FullName = "Variable-Density Flow Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".vdf";
            _PackageInfo.ModuleName = "VDF";
            Description = "The VDF Package solves the following form of the variable-density ground-water flow equation";
            Version = "VDF";
            IsMandatory = false;
            _Layer3DToken = "RegularGrid";
            //-1         1         0         0  MT3DRHOFLG   MFNADVFD   NSWTCPL   IWTABLE
            MT3DRHOFLG = -1;
            MFNADVFD = 1;
            NSWTCPL = 0;
            IWTABLE = 0;
        }

        [Category("Solve")]
        [Description("")]
        /// <summary>
        /// 
        /// </summary>
        public int MT3DRHOFLG
        {
            get;
            set;
        }

        [Category("Solve")]
        [Description("")]
        /// <summary>
        /// 
        /// </summary>
        public int MFNADVFD
        {
            get;
            set;
        }

        [Category("Solve")]
        [Description("")]
        /// <summary>
        /// 
        /// </summary>
        public int NSWTCPL
        {
            get;
            set;
        }
        [Category("Solve")]
        [Description("")]
        /// <summary>
        /// 
        /// </summary>
        public int IWTABLE
        {
            get;
            set;
        }
              
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.Grid.Updated += this.OnGridUpdated;
            this.TimeService = Owner.TimeService;
            base.Initialize();
        }
        public override void New()
        {
            base.New();
        }
        public override LoadingState Load(ICancelProgressHandler progress)
        {
            var result = LoadingState.Normal;
            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName);
                try
                {
                    var grid = Owner.Grid as MFGrid;
                    var mf = Owner as Modflow;
                    string line = sr.ReadLine();
                    var bufs = TypeConverterEx.Split<string>(line);
                    result = LoadingState.Normal;
                }
                catch (Exception ex)
                {
                    Message = string.Format("Failed to load {0}. Error message: {1}", Name, ex.Message);
                    ShowWarning(Message, progress);
                    result = LoadingState.FatalError;
                }
                finally
                {
                    sr.Close();
                }
            }
            else
            {
                Message = string.Format("Failed to load {0}. The package file does not exist: {1}", Name, FileName);
                ShowWarning(Message, progress);
                result = LoadingState.FatalError;
            }
            OnLoaded(progress, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
        }
        public override void CompositeOutput(MFOutputPackage mfout)
        {
            var mf = Owner as Modflow;
           UCNPackage acn = new UCNPackage()
            {
                Owner = mf,
                Parent = this,
                FileName = this.FileName
            };
            mfout.AddChild(acn);
        }
        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            var grid = (Owner.Grid as IRegularGrid);
            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultComment(sw, this.Name);
            sw.Close();
            OnSaved(progress);
        }
        public override void Clear()
        {
            if (_Initialized)
                this.Grid.Updated -= this.OnGridUpdated;
            base.Clear();
        }

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }
    }
}