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
            Category = Resources.SEAWATCategory;
            ResetToDefault();
        }

        [Category("Option")]
        [Description("")]
        /// <summary>
        /// 
        /// </summary>
        public int MT3DRHOFLG
        {
            get;
            set;
        }

        [Category("Option")]
        [Description("2, internodal conductance values used to conserve fluid mass are calculated using a central-in-space algorithm; not 2, using an upstream-weighted algorithm")]
        /// <summary>
        /// 
        /// </summary>
        public int MFNADVFD
        {
            get;
            set;
        }

        [Category("Option")]
        [Description("A flag used to determine the flow and transport coupling procedure")]
        /// <summary>
        /// A flag used to determine the flow and transport coupling procedure
        /// </summary>
        public int NSWTCPL
        {
            get;
            set;
        }
        [Category("Option")]
        [Description("a flag used to activate the variable-density water-table corrections")]
        /// <summary>
        /// A flag used to activate the variable-density water-table corrections
        /// </summary>
        public int IWTABLE
        {
            get;
            set;
        }

        [Category("Option")]
        [Description("The minimum fluid density. If DENSEMIN = 0, the computed fluid density is not limited by DENSEMIN (this is the option to use for most simulations")]
        /// <summary>
        /// The minimum fluid density. If DENSEMIN = 0, the computed fluid density is not limited by DENSEMIN (this is the option to use for most simulations
        /// </summary>
        public float DENSEMIN
        {
            get;
            set;
        }

        [Category("Option")]
        [Description("The maximum fluid density. If DENSEMAX = 0, the computed fluid density is not limited by DENSEMAX (this is the option to use for most simulations")]
        /// <summary>
        /// 
        /// </summary>
        public float DENSEMAX
        {
            get;
            set;
        }
        [Category("Option")]
        [Description("a user-specified density value.")]
        public float DNSCRIT
        {
            get;
            set;
        }

        [Category("Option")]
        [Description("The fluid density at the reference concentration, temperature, and pressure")]
        public float DENSEREF
        {
            get;
            set;
        }

        [Category("Option")]
        [Description("the slope of the linear equation of state that relates fluid density to solute concentration")]
        public float DRHODC
        {
            get;
            set;
        }

        [Category("Option")]
        [Description("the slope of the linear equation of state that relates fluid density to the height of the pressure head. A value of zero, which is typically used for most problems, inactivates the dependence of fluid density on pressure")]
        public float DRHODPRHD
        {
            get;
            set;
        }

        [Category("Option")]
        [Description("the reference pressure head. This value should normally be set to zero")]
        public float PRHDREF
        {
            get;
            set;
        }

        [Category("Option")]
        [Description("the number of MT3DMS species to be used in the equation of state for fluid density. This value is read only if MT3DRHOFLG = -1.")]
        public int NSRHOEOS
        {
            get;
            set;
        }
        [Category("Option")]
        [Description("MT3DMS species properties")]
        [StaticVariableItem]
        [Browsable(false)]
        public DataCube2DLayout<float> SPEC
        {
            get;
            set;
        }
        [Category("Option")]
        [Description("the length of the first transport timestep used to start the simulation")]
        public float FIRSTDT
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
            ResetToDefault();
            base.New();
        }

        public override void ResetToDefault()
        {
            MT3DRHOFLG = -1;
            MFNADVFD = 1;
            NSWTCPL = 0;
            IWTABLE = 0;

            DENSEMIN = 0;
            DENSEMAX =0;

            DENSEREF = 1000;
            DRHODPRHD = 0;
            PRHDREF =0;


            NSRHOEOS = 7;
            SPEC = new DataCube2DLayout<float>(1, NSRHOEOS, 3);
            SPEC.Name = "MT3DMS Species";
            SPEC.ColumnNames[0] = "MTRHOSPEC";
            SPEC.ColumnNames[1] = "DRHODC";
            SPEC.ColumnNames[2] = "CRHOREF";
            var conc = new float[] { 21.7f, 35.1f, 9.6f, 6f, 116.8f, 54.4f, 12.2f };
            for (int i = 0; i < NSRHOEOS; i++)
            {
                SPEC.ILArrays[0][i, 0] = i+1;
                SPEC.ILArrays[0][i, 1] = conc[i];
                SPEC.ILArrays[0][i, 2] = 0;
            }

            FIRSTDT = 1;
        }

        public override LoadingState Load(ICancelProgressHandler progress)
        {
            var result = LoadingState.Normal;
            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName);
                try
                {
                    var mf = Owner as Modflow;
                    var line = ReadComment(sr);
                    var grid = Owner.Grid as MFGrid;
                    var intbufs = TypeConverterEx.Split<int>(line);
                    MT3DRHOFLG = intbufs[0];
                    MFNADVFD = intbufs[1];
                    NSWTCPL = intbufs[2];
                    IWTABLE = intbufs[3];

                    line = sr.ReadLine();
                    var ffbufs = TypeConverterEx.Split<float>(line);
                    DENSEMIN = ffbufs[0];
                    DENSEMAX = ffbufs[1];

                    line = sr.ReadLine();
                    ffbufs = TypeConverterEx.Split<float>(line);
                    DENSEREF = ffbufs[0];
                    DRHODPRHD = ffbufs[1];
                    PRHDREF = ffbufs[2];

                    line = sr.ReadLine();
                    intbufs = TypeConverterEx.Split<int>(line);
                    NSRHOEOS = intbufs[0];
                    SPEC = new DataCube2DLayout<float>(1, NSRHOEOS, 3);
                    SPEC.Name = "MT3DMS Species";
                    SPEC.ColumnNames[0] = "MTRHOSPEC";
                    SPEC.ColumnNames[1] = "DRHODC";
                    SPEC.ColumnNames[2] = "CRHOREF";

                    for (int i = 0; i < NSRHOEOS; i++)
                    {
                        line = sr.ReadLine();
                        ffbufs = TypeConverterEx.Split<float>(line);
                        SPEC.ILArrays[0][i, ":"] = ffbufs;
                    }

                    line = sr.ReadLine();
                    ffbufs = TypeConverterEx.Split<float>(line);
                    FIRSTDT = ffbufs[0];

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
            WriteDefaultComment(sw, this.Name);
            string line = string.Format("{0}\t{1}\t{2}\t{3}\t#MT3DRHOFLG MFNADVFD NSWTCPL IWTABLE", MT3DRHOFLG, MFNADVFD, NSWTCPL, IWTABLE);
            sw.WriteLine(line);
            line = string.Format("{0}\t{1}\t#DENSEMIN DENSEMAX", DENSEMIN, DENSEMAX);
            sw.WriteLine(line);
            line = string.Format("{0}\t{1}\t{2}\t#DENSEMIN DENSEMAX", DENSEREF, DRHODPRHD, PRHDREF);
            sw.WriteLine(line);
            line = string.Format("{0}\t#NSRHOEOS", NSRHOEOS);
            sw.WriteLine(line);
            for (int i = 0; i < NSRHOEOS; i++)
            {
                line = string.Format("{0}\t{1}\t{2}\t#MTRHOSPEC({3}) DRHODC({3}) CRHOREF({3})", SPEC[0, i, 0], SPEC[0, i, 1], SPEC[0, i, 2], (i + 1));
                sw.WriteLine(line);
            }
            line = string.Format("{0}\t#FIRSTDT", FIRSTDT);
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