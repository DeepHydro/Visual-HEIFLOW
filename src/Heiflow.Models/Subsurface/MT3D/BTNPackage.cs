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
    public class BTNPackage : MFPackage
    {
        public static string PackageName = "BTN";
        public BTNPackage()
        {
            Name = "BTN";
            _FullName = "Basic Transport Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".btn";
            _PackageInfo.ModuleName = "BTN";
            Description = "The BTN Package consists of nine primary modules";
            Version = "BTN";
            IsMandatory = false;
            _Layer3DToken = "RegularGrid";
        }
        [Category("Chemical")]
        /// <summary>
        /// NCOMP is the total number of chemical species included in the current simulation. For single-species simulation, set NCOMP = 1;
        /// </summary>
        public int NCOMP
        {
            get;
            set;
        }
        [Category("Chemical")]
        /// <summary>
        /// MCOMP is the total number of “mobile” species. MCOMP must be equal to or less than NCOMP. For single-species simulation, set MCOMP=1.
        /// </summary>
        public int MCOMP
        {
            get;
            set;
        }

        //TUNIT, LUNIT, MUNIT
        [Category("Units")]
        public string TUNIT
        {
            get;
            set;
        }
        [Category("Units")]
        public string LUNIT
        {
            get;
            set;
        }
        [Category("Units")]
        public string MUNIT
        {
            get;
            set;
        }
        [Category("Packages")]
        /// <summary>
        /// TRNOP are logical flags for major transport and solution options. TRNOP(1) to (5) correspond to Advection, Dispersion, Sink & Source Mixing, Chemical Reaction, and Generalized Conjugate Gradient Solver packages, respectively.
        /// </summary>
        public string TRNOP
        {
            get;
            set;
        }
        [Category("Layers")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<int> LAYCON
        {
            get;
            set;
        }
        [Category("Grid")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube2DLayout<float> DELR
        {
            get;
            set;
        }
        [Category("Grid")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<float> DELC
        {
            get;
            set;
        }
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<float> HTOP
        {
            get;
            set;
        }
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<float> DZ
        {
            get;
            set;
        }
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<float> PRSITY
        {
            get;
            set;
        }
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<int> ICBUND
        {
            get;
            set;
        }
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<float> SCONC
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
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    var intbufs = TypeConverterEx.Split<int>(line);
                    NCOMP = intbufs[4];
                    MCOMP = intbufs[5];
                    line = sr.ReadLine();
                    var strbufs = TypeConverterEx.Split<string>(line);
                    TUNIT = strbufs[0];
                    LUNIT = strbufs[1];
                    MUNIT = strbufs[2];
                    line = sr.ReadLine();
                    TRNOP = line;
                    LAYCON = new DataCube<int>(1, 1, grid.ActualLayerCount);
                    DELR = new DataCube2DLayout<float>(1, grid.RowCount,grid.ColumnCount);
                    DELC= new DataCube2DLayout<float>(1, grid.RowCount, grid.ColumnCount);
                    HTOP = new DataCube2DLayout<float>(1, grid.RowCount, grid.ColumnCount);
                    DZ = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount);
                    PRSITY = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount);
                    ICBUND = new DataCube<int>(grid.ActualLayerCount, 1, grid.ActiveCellCount);
                    SCONC = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount);

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