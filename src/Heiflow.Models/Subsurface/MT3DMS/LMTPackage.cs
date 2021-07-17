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
    public class LMTPackage : MFPackage
    {

        public static string PackageName = "LMT6";
        public LMTPackage()
        {
            Name = "LMT6";
            _FullName = "Link Model Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".lml";
            _PackageInfo.ModuleName = "LMT6";
            Description = "The MF2K-MT3DMS LINKER FILE";
            Version = "LMT6";
            IsMandatory = false;
            _Layer3DToken = "RegularGrid";
            
            ResetToDefault();
            Category = Resources.MT3DCategory; 
        }
        [Category("File")]
        [Description("")]
        /// <summary>
        /// The output file name
        /// </summary>
        public string OUTPUT_FILE_NAME
        {
            get;
            set;
        }
        [Category("File")]
        [Description("")]
        /// <summary>
        /// 
        /// </summary>
        public int OUTPUT_FILE_UNIT
        {
            get;
            set;
        }
        [Category("File")]
        [Description("")]
        /// <summary>
        /// 
        /// </summary>
        public string OUTPUT_FILE_HEADER
        {
            get;
            set;
        }

        [Category("File")]
        [Description("")]
        /// <summary>
        /// 
        /// </summary>
        public string OUTPUT_FILE_FORMAT
        {
            get;
            set;
        }

        public override void ResetToDefault()
        {
            OUTPUT_FILE_NAME = "MT3D.FLO";
            OUTPUT_FILE_UNIT = 333;
            OUTPUT_FILE_HEADER = "Standard";
            OUTPUT_FILE_FORMAT = "Unformatted";
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
        public override LoadingState Load(ICancelProgressHandler progress)
        {
            var result = LoadingState.Normal;
            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName);
                try
                {
                    var line = ReadComment(sr);
                    var bufs = TypeConverterEx.Split<string>(line);
                    if (bufs.Length == 2)
                        OUTPUT_FILE_NAME = bufs[1];
                    line = sr.ReadLine();
                    bufs = TypeConverterEx.Split<string>(line);
                    if (bufs.Length == 2)
                        OUTPUT_FILE_UNIT = int.Parse(bufs[1]);
                    line = sr.ReadLine();
                    bufs = TypeConverterEx.Split<string>(line);
                    if (bufs.Length == 2)
                        OUTPUT_FILE_HEADER = bufs[1];
                    line = sr.ReadLine();
                    bufs = TypeConverterEx.Split<string>(line);
                    if (bufs.Length == 2)
                        OUTPUT_FILE_FORMAT = bufs[1];

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
            WriteDefaultComment(sw, "LMT");
            string line= string.Format("OUTPUT_FILE_NAME {0}", OUTPUT_FILE_NAME);
            sw.WriteLine(line);
            line = string.Format("OUTPUT_FILE_UNIT {0}", OUTPUT_FILE_UNIT);
            sw.WriteLine(line);
            line = string.Format("OUTPUT_FILE_HEADER {0}", OUTPUT_FILE_HEADER);
            sw.WriteLine(line);
            line = string.Format("OUTPUT_FILE_FORMAT {0}", OUTPUT_FILE_FORMAT);
            sw.WriteLine(line);

            sw.Close();
            OnSaved(progress);
        }
        public override void Clear()
        {
            ResetToDefault();
            base.Clear();
        }

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }

    }
}