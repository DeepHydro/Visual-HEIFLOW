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
    public class DSPPackage : MFPackage
    {
        public static string PackageName = "DSP";
        public DSPPackage()
        {
            Name = "DSP";
            _FullName = "Dispersion Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".dsp";
            _PackageInfo.ModuleName = "DSP";
            Description = "The DSP Package";
            Version = "DSP";
            IsMandatory = false;
            _Layer3DToken = "RegularGrid";
            Category = Modflow.MT3DCategory;

        }
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube<float> AL
        {
            get;
            set;
        }
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube2DLayout<float> TRPT
        {
            get;
            set;
        }   
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube2DLayout<float> TRPV
        {
            get;
            set;
        }
        [Category("Layer")]
        [Browsable(false)]
        [StaticVariableItem]
        public DataCube2DLayout<float> DMCOEF
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
                    InitArrays(grid);
                    for (int i = 0; i < grid.ActualLayerCount; i++)
                    {
                        ReadSerialArray<float>(sr, AL, i, 0);
                    }
                    ReadRegularArrayMT3D<float>(sr, TRPT, 0);
                    ReadRegularArrayMT3D<float>(sr, TRPV, 0);
                    ReadRegularArrayMT3D<float>(sr, DMCOEF, 0);

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

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            var grid = (Owner.Grid as IRegularGrid);
            StreamWriter sw = new StreamWriter(filename);
            for (int i = 0; i < grid.ActualLayerCount; i++)
            {
                WriteSerialFloatArrayMT3D(sw, AL, i, 0, "F6", 15,10, "G15.6");
            }
            WriteRegularArrayMT3D(sw, TRPT, 0, "F6", 15, "G15.6");
            WriteRegularArrayMT3D(sw, TRPV, 0, "F6", 15, "G15.6");
            WriteRegularArrayMT3D(sw, DMCOEF, 0, "F6", 15, "G15.6");
            sw.Close();
            OnSaved(progress);
        }

        private void InitArrays(MFGrid grid)
        {
            AL = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount)
            {
                ZeroDimension = DimensionFlag.Spatial
            };
            TRPT = new DataCube2DLayout<float>(1, 1, grid.ActualLayerCount);
            TRPV = new DataCube2DLayout<float>(1, 1, grid.ActualLayerCount);
            DMCOEF = new DataCube2DLayout<float>(1, 1, grid.ActualLayerCount);

            for (int i = 0; i < grid.ActualLayerCount; i++)
            {
                AL.Variables[i] = "Longitudinal Dispersivity of Layer " + (i + 1);
                AL.ILArrays[i][0, ":"] = 0;
            }
            TRPT.Variables[0] = "Ratio of TH to AL";
            TRPV.Variables[0] = "Ratio of TV to AL";
            DMCOEF.Variables[0] = "Effective Molecular Diffusion Coefficient";
            for (int i = 0; i < grid.ActualLayerCount; i++)
            {
                TRPT.ColumnNames[i] = "Layer " + (i + 1);
                TRPV.ColumnNames[i] = "Layer " + (i + 1);
                DMCOEF.ColumnNames[i] = "Layer " + (i + 1);
            }
            TRPT.ILArrays[0][0, ":"] = 0.1f;
            TRPV.ILArrays[0][0, ":"] = 0.01f;
            DMCOEF.ILArrays[0][0, ":"] = 0;
        }
        public override void OnGridUpdated(IGrid sender)
        {
            if (this.TimeService.StressPeriods.Count == 0)
                return;
            var mf = Owner as Modflow;
            var grid = sender as MFGrid;
            InitArrays(grid);
            base.OnGridUpdated(sender);
        }
        public override void Clear()
        {
            if (_Initialized)
                this.Grid.Updated -= this.OnGridUpdated;

            AL = null;
            TRPT = null;
            TRPV = null;
            DMCOEF = null;
            base.Clear();
        }

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }
    }
}