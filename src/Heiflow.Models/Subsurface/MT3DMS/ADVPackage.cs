﻿//
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
using Heiflow.Models.Properties;
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
    public class ADVPackage : MFPackage
    {
        public enum SolveOptionEnum { Third_Order_TVD = -1, Method_Of_Characteristics = 1, Modified_Method_Of_Characteristics = 2, Hybrid_Method_Of_Characteristics = 3 };
        public enum WeightingSchemeEnum { Upstream = 1, Central_In_Space = 2 };

        public static string PackageName = "ADV";
        public ADVPackage()
        {
            Name = "ADV";
            _FullName = "Advection Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".adv";
            _PackageInfo.ModuleName = "ADV";
            Description = "The ADV Package solves the advection term";
            Version = "ADV";
            IsMandatory = false;
            _Layer3DToken = "RegularGrid";
            
            ResetToDefault();
            Category = Resources.MT3DCategory; 
        }
        [Category("Solve")]
        [Description("The advection solution option")]
        /// <summary>
        /// The advection solution option
        /// </summary>
        public SolveOptionEnum MIXELM
        {
            get;
            set;
        }
         [Category("Solve")]
        [Description("Courant number (i.e., the number of cells, or a fraction of a cell)")]
        /// <summary>
        /// Courant number (i.e., the number of cells, or a fraction of a cell)
        /// </summary>
        public float PERCEL
        {
            get;
            set;
        }
         [Category("Solve")]
         [Description("The maximum total number of moving particles allowed and is used only when MIXELM = 1 or 3)")]
         public int MXPART
         {
             get;
             set;
         }
         [Category("Solve")]
         [Description("An integer flag indicating which weighting scheme should be used; it is needed only when the advection term is solved using the implicit finitedifference method. 1: upstream weighting (default); 2: central-in-space weighting")]
         public WeightingSchemeEnum NADVFD
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
            MIXELM = SolveOptionEnum.Third_Order_TVD;
            PERCEL = 1.0f;
            MXPART = 9999;
            NADVFD = WeightingSchemeEnum.Upstream;
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
                    MIXELM = EnumHelper.FromString<SolveOptionEnum>(bufs[0]);
                    PERCEL = float.Parse(bufs[1]);
                    try
                    {
                        if (bufs.Length > 2)
                            MXPART = int.Parse(bufs[2]);
                        if (bufs.Length > 3)
                            NADVFD = EnumHelper.FromString<WeightingSchemeEnum>(bufs[3]);
                    }
                    catch
                    {

                    }
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
            string line = string.Format("{0}{1}", ((int)MIXELM).ToString().PadLeft(10, ' '), PERCEL.ToString().PadLeft(10, ' '));

            //if(MIXELM == SolveOptionEnum.Method_Of_Characteristics || MIXELM== SolveOptionEnum.Third_Order_TVD)
            //{
            //    line += MXPART.ToString().PadLeft(10, ' ');
            //}
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