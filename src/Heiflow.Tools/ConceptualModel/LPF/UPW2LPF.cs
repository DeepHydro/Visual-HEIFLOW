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
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.IO;
using GeoAPI.Geometries;
using Heiflow.Core.MyMath;
using Heiflow.Spatial.SpatialAnalyst;

namespace Heiflow.Tools.ConceptualModel
{
    public class UPW2LPF : MapLayerRequiredTool
    {
        public UPW2LPF()
        {
            Name = "Convert UPW To LPF";
            Category = Cat_CMG;
            SubCategory = "LPF/UPW";
            Description = "Convert UPW To LPF";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            WetDry = 0.01f;
            MultiThreadRequired = true;
        }

        [Category("Input")]
        [Description("A text file that contains the lookup table between raster value and aquifer properties. The first line of the text file is a head line." +
        "The column names must be: LAYER,ID,HK,VKA,SY,SS,WETDRY; There are NLayer*NID rows.")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OutputFileName
        {
            get;
            set;
        }

        [Category("Parameters")]
        [Description("")]
        public float WetDry
        {
            get;
            set;
        }


        public override void Initialize()
        {
            this.Initialized = TypeConverterEx.IsNotNull(OutputFileName);
        }

        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;

            if (mf != null)
            {
                int nlayer = mf.Grid.ActualLayerCount;
                var grid = mf.Grid as MFGrid;
                var flowpck = mf.FlowPropertyPackage;
                if(flowpck is UPWPackage)
                {
                    UPWPackage upw = flowpck as UPWPackage;
                    LPFPackage lpf = new LPFPackage();
                    lpf.Owner = upw.Owner;
                    StreamWriter sw = new StreamWriter(OutputFileName);
                    lpf.WriteDefaultComment(sw, this.Name);
                    string line = string.Format("{0}\t{1}\t{2}\t{3}\t# ILPFCB, HDRY, NPLPF", upw.IUPWCB,upw.HDRY, lpf.NPLPF, lpf.Options);
                    sw.WriteLine(line);

                    line = TypeConverterEx.Vector2String<int>(upw.LAYTYP) + "\t# LAYTYP";
                    sw.WriteLine(line);

                    line = TypeConverterEx.Vector2String<int>(upw.LAYAVG) + "\t# LAYAVG";
                    sw.WriteLine(line);

                    line = TypeConverterEx.Vector2String<float>(upw.CHANI) + "\t# CHANI";
                    sw.WriteLine(line);

                    line = TypeConverterEx.Vector2String<int>(upw.LAYVKA) + "\t# LAYVKA";
                    sw.WriteLine(line);

                    for (int i = 0; i < grid.ActualLayerCount; i++)
                    {
                        if (upw.LAYTYP[i] == 1)
                            upw.LAYWET[i] = 1;
                        else
                            upw.LAYWET[i] = 0;
                    }
                        line = TypeConverterEx.Vector2String<int>(upw.LAYWET) + "\t# LAYWET";
                    sw.WriteLine(line);

                    line = string.Format("{0}\t{1}\t{2}\t# WETFCT, IWETIT, IHDWET", lpf.WETFCT, lpf.IWETIT, lpf.IHDWET);
                    sw.WriteLine(line);

                    for (int l = 0; l < grid.ActualLayerCount; l++)
                    {
                        string cmt = string.Format("#HK Layer {0}", l + 1);

                        // WriteSerialFloatInternalMatrix(sw, HK[l, 0], 1, "E5", -1, cmt);
                        lpf.WriteSerialFloatArray(sw, upw.HK, l, 0, "E6", cmt);
                        cmt = string.Format("#HANI Layer {0}", l + 1);
                        // WriteSerialFloatInternalMatrix(sw, HANI[l, 0], 1, "E5", -1, cmt);
                        lpf.WriteSerialFloatArray(sw, upw.HANI, l, 0, "E6", cmt);
                        cmt = string.Format("#VKA Layer {0}", l + 1);
                        //WriteSerialFloatInternalMatrix(sw, VKA[l, 0], 1, "E5", -1, cmt);
                        lpf.WriteSerialFloatArray(sw, upw.VKA, l, 0, "E6", cmt);
                        cmt = string.Format("#SS Layer {0}", l + 1);
                        //WriteSerialFloatInternalMatrix(sw, SS[l, 0], 1, "E5", -1, cmt);
                        lpf.WriteSerialFloatArray(sw, upw.SS, l, 0, "E6", cmt);
                        if (upw.LAYTYP[l] != 0)
                        {
                            cmt = string.Format("#SY Layer {0}", l + 1);
                            //WriteSerialFloatInternalMatrix(sw, SY[l, 0], 1, "E5", -1, cmt);
                            lpf.WriteSerialFloatArray(sw, upw.SY, l, 0, "E6", cmt);
                        }
                        if (upw.LAYTYP[l] != 0 && upw.LAYWET[l] != 0)
                        {
                            cmt = string.Format("#WETDRY Layer {0}", l + 1);
                            //  WriteSerialFloatInternalMatrix(sw, WETDRY[l, 0], 1, "E5", -1, cmt);
                            //lpf.WriteSerialFloatArray(sw, upw.WETDRY, l, 0, "E6", cmt);
                            line = string.Format("CONSTANT	{0}	# WETDRY Layer {1}", WetDry, (l + 1));
                            sw.WriteLine(line);
                        }
                    }
                    sw.Close();
                }

                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow must be used by this tool.");
                return false;
            }

        }
    }
}
