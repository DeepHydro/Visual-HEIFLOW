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

namespace Heiflow.Models.Subsurface.MT3DMS
{
    [PackageItem]
    [PackageCategory("Basic", true)]
    [CoverageItem]
    [Export(typeof(IMFPackage))]
    public class GCGPackage : MFPackage
    {
        public enum PreconditionersEnum { Jacobi = 1, SSOR = 2, Modified_Incomplete_Cholesky = 3 };
        public enum WeightingSchemeEnum { Upstream = 1, Central_In_Space = 2 };

        public static string PackageName = "GCG";
        public GCGPackage()
        {
            Name = GCGPackage.PackageName;
            _FullName = "Generalized Conjugate Gradient Solver Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".gcg";
            _PackageInfo.ModuleName = "GCG";
            Description = "The GCG Package";
            Version = "GCG";
            IsMandatory = false;
            _Layer3DToken = "RegularGrid";
            ResetToDefault();
            Category = Modflow.MT3DCategory;
        }
        [Category("Solver")]
        [Description("The maximum number of outer iterations.")]
        public int MXITER
        {
            get;
            set;
        }
        [Category("Solver")]
        [Description("The maximum number of outer iterations.")]
        public int ITER1
        {
            get;
            set;
        }
        [Category("Solver")]
        [Description("The maximum number of outer iterations.")]
        public PreconditionersEnum Preconditioner
        {
            get;
            set;
        }
        [Category("Solver")]
        [Description("Treatment of dispersion tensor cross terms. False: lump all dispersion cross terms to the righthand-side (approximate but highly efficient). True: include full dispersion tensor (memory intensive")]
        public bool NCRS
        {
            get;
            set;
        }
        [Category("Solver")]
        [Description("The relaxation factor for the SSOR option; a value of 1.0 is generally adequate")]
        public float ACCL
        {
            get;
            set;
        }
        [Category("Solver")]
        [Description("the convergence criterion in terms of relative concentration; a real value between 10-4 and 10-6 is generally adequate.")]
        public float CCLOSE
        {
            get;
            set;
        }
        [Category("Solver")]
        [Description("The interval for printing the maximum concentration changes of each iteration. Set IPRGCG to zero as default for printing at the end of each stress period")]
        public int IPRGCG
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
        public override void ResetToDefault()
        {
            MXITER = 100;
            ITER1 = 10;
            Preconditioner = PreconditionersEnum.Jacobi;
            NCRS = false;
            ACCL = 1;
            CCLOSE = 0.0001f;
            IPRGCG = 0;
        }
        public override void New()
        {
            ResetToDefault();
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
                    MXITER = int.Parse(bufs[0]);
                    ITER1 = int.Parse(bufs[1]);
                    Preconditioner = (PreconditionersEnum)(int.Parse(bufs[2]));
                    NCRS = int.Parse(bufs[1]) > 0;
                    line = sr.ReadLine(); 
                    bufs = TypeConverterEx.Split<string>(line);
                    ACCL = float.Parse(bufs[0]);
                    CCLOSE = float.Parse(bufs[1]);
                    IPRGCG = int.Parse(bufs[2]);
                    result = LoadingState.Normal;
                }
                catch (Exception ex)
                {
                    Message = string.Format("Failed to load {0}. Error message: {1}", Name, ex.Message);
                    ShowWarning(Message, progress);
                    result = LoadingState.Warning;
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
                result = LoadingState.Warning;
            }
            OnLoaded(progress, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            var grid = (Owner.Grid as IRegularGrid);
            StreamWriter sw = new StreamWriter(filename);
            //WriteDefaultComment(sw, this.Name);
            string line = string.Format("{0}\t{1}\t{2}\t{3}\t#MXITER, ITER1, ISOLVE, NCRS", MXITER, ITER1, (int)Preconditioner, NCRS ? 1 : 0);
            sw.WriteLine(line);
            line = string.Format("{0}\t{1}\t{2}\t#ACCL, CCLOSE, IPRGCG", ACCL, CCLOSE, IPRGCG);
            sw.WriteLine(line);
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