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
    [PackageCategory("Solver", true)]
    [Export(typeof(IMFPackage))]
    public class NWTPackage : MFPackage
    {
        public enum NWTOPTIONS { SPECIFIED, SIMPLE, MODERATE, COMPLEX };
        public NWTPackage()
        {
            Name = "NWT";
            _FullName = "Upstream Weighting Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".nwt";
            _PackageInfo.ModuleName = "NWT";
            Description = @" The UPW Package treats nonlinearities of cell drying and rewetting by use of a continuous function of groundwater head,
            rather than the discrete approach of drying and rewetting that is used by the BCF, LPF, 
                and HUF Packages. This further enables application of the Newton formulation for unconfined groundwater-flow problems
                because conductance derivatives required by the Newton method are smooth over the full range of head for a model cell.";
            Version = "UPW";
            IsMandatory = false;

        }
        /// <summary>
        /// The maximum head change between outer iterations for solution of the nonlinear problem.
        /// </summary>
        /// 
        [Description("The maximum head change between outer iterations for solution of the nonlinear problem.")]
        [Category("Convergence")]
        public float HEADTOL
        {
            get;
            set;
        }
        /// <summary>
        /// The maximum root-mean-squared flux difference between outer iterations for solution of the nonlinear problem.
        /// </summary>
        /// 
        [Description("The maximum root-mean-squared flux difference between outer iterations for solution of the nonlinear problem")]
        [Category("Convergence")]
        public float FLUXTOL
        {
            get;
            set;
        }
        /// <summary>
        /// The maximum number of iterations to be allowed for solution of the outer (nonlinear) problem.
        /// </summary>
        /// 
        [Description("The maximum number of iterations to be allowed for solution of the outer (nonlinear) problem.")]
        [Category("Convergence")]
        public int MAXITEROUT
        {
            get;
            set;
        }
        /// <summary>
        ///the portion of the cell thickness (length) used for smoothly adjusting storage and conductance coefficients to zero 
        /// </summary>
        /// 
        [Description("The portion of the cell thickness (length) used for smoothly adjusting storage and conductance coefficients to zero ")]
        [Category("Solve")]
        public float THICKFACT
        {
            get;
            set;
        }
        /// <summary>
        /// a flag that determines which matrix solver will be used (integer).  A value of 1 indicates GMRES will be used; A value of 2 indicates χMD will be used.
        /// </summary>
        [Description(@"a flag that determines which matrix solver will be used (integer). 
         A value of 1 indicates GMRES will be used; A value of 2 indicates χMD will be used.")]
        [Category("Solve")]
        public int LINMETH
        {
            get;
            set;
        }
        /// <summary>
        ///  A flag that indicates whether additional information about solver convergence will be printed to the main listing file
        /// </summary>
        [Description("A flag that indicates whether additional information about solver convergence will be printed to the main listing file")]
        [Category("Printout")]
        public int IPRNWT
        {
            get;
            set;
        }
        /// <summary>
        ///  a flag that indicates whether corrections will be made to groundwater head relative to the cell-bottom altitude if the cell is surrounded by dewatered cells (integer). A value of 1 indicates that a correction will be made and a value of 0 indicates no correction will be made. This input variable is problem specific and both options (IBOTAV=0 or 1) should be tested.
        /// </summary>
        /// 
        [Description(@"a flag that indicates whether corrections will be made to groundwater head 
                    relative to the cell-bottom altitude if the cell is surrounded by dewatered cells (integer).
                    A value of 1 indicates that a correction will be made and a value of 0 indicates no correction will be made. 
                    This input variable is problem specific and both options (IBOTAV=0 or 1) should be tested.")]
        [Category("Solve")]
        public int IBOTAV
        {
            get;
            set;
        }
        /// <summary>
        /// SPECIFIED indicates that the optional solver input values listed for items 1 and 2 will be specified in the NWT input file by the user; 	
        /// SIMPLE indicates that default solver input values will be defined that work well for nearly linear models; 	
        /// MODERATE indicates that default solver input values will be defined that work well for moderately nonlinear models; 
        /// COMPLEX indicates that default solver input values will be defined that work well for highly nonlinear models. 
        /// </summary>
        /// 
        [Description(@"SPECIFIED indicates that the optional solver input values listed for items 1 and 2 will be specified in the NWT input file by the user;
                    SIMPLE indicates that default solver input values will be defined that work well for nearly linear models; 	
                    MODERATE indicates that default solver input values will be defined that work well for moderately nonlinear models; 
                    COMPLEX indicates that default solver input values will be defined that work well for highly nonlinear models. ")]
        [Category("Solve")]
        public NWTOPTIONS OPTIONS
        {
            get;
            set;
        }
       
        public override void New()
        {
            var pcg_info = new PackageInfo()
            {
                FID = ModflowInstance.NameManager.NextFID(),
                FileExtension = ".nwt",
                FileName = string.Format("{0}{1}{2}", Modflow.OutputDic, ModflowInstance.Project.Name, ".nwt"),
                Format = FileFormat.Text,
                IOState = IOState.REPLACE,
                ModuleName = "NWT",
                WorkDirectory = ModflowInstance.WorkDirectory
            };
            pcg_info.Name = Path.GetFileName(pcg_info.FileName);
            ModflowInstance.NameManager.Add(pcg_info);
            base.New();
            State = ModelObjectState.Ready;
        }
        public override LoadingState Load(ICancelProgressHandler progresshandler)
        {
            var result = LoadingState.Normal;
            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName);
                try
                {
                    var line = ReadComment(sr);
                    var strs = TypeConverterEx.Split<string>(line);
                    int buf = 300;
                    float ff = 0.1f;
                    //0.01 1 400 1E-004 2 1 0 simple CONTINUE   # Data Set 1, HEADTOL FLUXTOL MAXITEROUT THICKFACT LINMETH IPRNWT IBOTAV OPTIONS
                    float.TryParse(strs[0], out ff);
                    HEADTOL = ff;
                    float.TryParse(strs[1], out ff);
                    FLUXTOL = ff;
                    int.TryParse(strs[2], out buf);
                    MAXITEROUT = buf;
                    float.TryParse(strs[3], out ff);
                    THICKFACT = ff;
                    int.TryParse(strs[4], out buf);
                    LINMETH = buf;
                    int.TryParse(strs[5], out buf);
                    IPRNWT = buf;
                    int.TryParse(strs[6], out buf);
                    IBOTAV = buf;
                    OPTIONS = TypeConverterEx.ChangeType<NWTOPTIONS>(strs[7].ToUpper());

                    result = LoadingState.Normal;
                }
                catch (Exception ex)
                {
                    result = LoadingState.Warning;
                    Message = string.Format("Failed to load {0}. Error message: {1}", Name, ex.Message);
                    ShowWarning(Message, progresshandler);
                }
                finally
                {
                    sr.Close();
                }
            }
            else
            {
                ShowWarning("Failed to load " + this.Name, progresshandler);
                result = LoadingState.Warning;
            }
            OnLoaded(progresshandler, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
        }
        public override void SaveAs(string filename, ICancelProgressHandler prg)
        {
            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultComment(sw, this.Name);

            //0.01 1 400 1E-004 2 1 0 simple CONTINUE   # Data Set 1, HEADTOL FLUXTOL MAXITEROUT THICKFACT LINMETH IPRNWT IBOTAV OPTIONS
            string line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\tCONTINUE\t# Data Set 1, HEADTOL FLUXTOL MAXITEROUT THICKFACT LINMETH IPRNWT IBOTAV OPTIONS",
                HEADTOL, FLUXTOL, MAXITEROUT, THICKFACT, LINMETH, IPRNWT, IBOTAV, OPTIONS);
            sw.WriteLine(line);
            sw.Close();
            OnSaved(prg);
        }
        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {

        }
    }
}