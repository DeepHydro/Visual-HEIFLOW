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
    public class PCGPackage : MFPackage
    {
        public PCGPackage()
        {
            Name = "PCG";
            _FullName = "Preconditioned Conjugate-Gradient Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".pcg";
            _PackageInfo.ModuleName = "PCG";
            Description = " The Preconditioned Conjugate-Gradient package" +
            " is used to solve the finite difference equations " +
            " in each step of a MODFLOW stress period";
            Version = "PCG7";
            IsMandatory = true;

            MXITER = 200;
            ITER1 = 500;
            NPCOND = 1;
            IHCOFADD = 0;
            HCLOSE = 0.01f;
            RCLOSE = 0.01f;
            RELAX = 0.99f;
            NBPOL = 1;
            IPRPCG = 1;
            MUTPCG = 1;
            DAMPPCG = 0;
            DAMPPCGT = 0.98f;

        }
        /// <summary>
        /// the maximum number of outer iterations
        /// </summary>
        public int MXITER
        {
            get;
            set;
        }
        /// <summary>
        /// the number of inner iterations.
        /// </summary>
        public int ITER1
        {
            get;
            set;
        }
        /// <summary>
        /// the flag used to select the matrix conditioning method:
        /// 1—is for Modified Incomplete Cholesky (for use on scalar computers)
        /// 2—is for Polynomial (for use on vector computers or to conserve computer memory)
        /// </summary>
        public int NPCOND
        {
            get;
            set;
        }
        /// <summary>
        /// a flag that determines what happens to an active cell that is surrounded by dry cells:
        /// 0—cell converts to dry regardless of HCOF value. This is the default, which is the way PCG2 worked prior to the addition of this option.
        /// Not 0—cell converts to dry only if HCOF is 0 (no head-dependent stresses or storage terms) )
        /// </summary>
        public int IHCOFADD
        {
            get;
            set;
        }
        /// <summary>
        /// the head change criterion for convergence, in units of length. When the maximum absolute value of head change 
        /// from all nodes during an iteration is less than or equal to HCLOSE, and the criterion for RCLOSE is also satisfied (see below), iteration stops.
        /// </summary>
        public float HCLOSE
        {
            get;
            set;
        }
        /// <summary>
        ///  residual criterion for convergence, in units of cubic length per time. 
        ///  When the maximum absolute value of the residual at all nodes during an iteration is less than or equal to RCLOSE, 
        ///  and the criterion for HCLOSE is also satisfied (see above), iteration stops.
        ///  For nonlinear problems, convergence is achieved when the convergence criteria are satisfied for the first inner iteration.
        /// </summary>
        public float RCLOSE
        {
            get;
            set;
        }
        /// <summary>
        ///  the relaxation parameter used with NPCOND = 1. Usually, RELAX = 1.0, 
        ///  but for some problems a value of 0.99, 0.98, or 0.97 will reduce the number of iterations required for convergence. 
        ///  RELAX is not used if NPCOND is not 1.
        /// </summary>
        public float RELAX
        {
            get;
            set;
        }
        /// <summary>
        /// is used when NPCOND = 2 to indicate whether the estimate of the upper
        /// bound on the maximum eigenvalue is 2.0, or whether the estimate will be calculated. 
        /// NBPOL = 2 is used to specify the value is 2.0; for any other value of NBPOL, the estimate is calculated.
        /// Convergence is generally insensitive to this parameter. NBPOL is not used if NPCOND is not 2.
        /// </summary>
        public float NBPOL
        {
            get;
            set;
        }
        /// <summary>
        /// the printout interval for PCG. If IPRPCG is equal to zero, it is changed to 999. 
        /// The maximum head change (positive or negative) and residual change are printed for each iteration of a time step 
        /// whenever the time step is an even multiple of IPRPCG. 
        /// This printout also occurs at the end of each stress period regardless of the value of IPRPCG.
        /// </summary>
        public int IPRPCG
        {
            get;
            set;
        }
        /// <summary>
        /// a flag that controls printing of convergence information from the solver:
        /// 0—is for printing tables of maximum head change and residual each iteration
        /// 1—is for printing only the total number of iterations
        /// 2—is for no printing
        /// 3—is for printing only if convergence fails
        /// </summary>
        public int MUTPCG
        {
            get;
            set;
        }
        /// <summary>
        /// the damping factor. It is typically set equal to one, which indicates no damping.  A value less than 1 and greater than 0 causes damping.
        /// >0 -- applies to both steady-state and transient stress periods.
        /// less than 0 -- applies only to steady-state stress periods.  The absolute value is used as the damping factor.
        /// </summary>
        public float DAMPPCG
        {
            get;
            set;
        }
        /// <summary>
        ///  the damping factor for transient stress periods. DAMPPCGT is enclosed in brackets indicating that it is an optional variable that only is read 
        ///  when DAMPPCG is specified as a negative value. 
        ///  If DAMPPCGT is not read, then the single damping factor, 
        ///  DAMPPCG, is used for both transient and steady-state stress periods.
        /// </summary>
        public float DAMPPCGT
        {
            get;
            set;
        }

        public override bool New()
        {
            var pcg_info = new PackageInfo()
            {
                FID = ModflowInstance.NameManager.NextFID(),
                FileExtension = ".fhd",
                FileName = string.Format("{0}{1}{2}", Modflow.OutputDic, ModflowInstance.Project.Name, ".pcg"),
                Format = FileFormat.Text,
                IOState = IOState.REPLACE,
                ModuleName = "PCG",
                WorkDirectory = ModflowInstance.WorkDirectory
            };
            pcg_info.Name = Path.GetFileName(pcg_info.FileName);
            ModflowInstance.NameManager.Add(pcg_info);
            base.New();
            State = ModelObjectState.Ready;
            return true;
        }
        public override bool Load(ICancelProgressHandler progresshandler)
        {
            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName);
                var line = ReadComment(sr);
                var strs = TypeConverterEx.Split<string>(line);
                int buf = 300;
                float ff = 0.1f;

                int.TryParse(strs[0], out buf);
                MXITER = buf;
                int.TryParse(strs[1], out buf);
                ITER1 = buf;
                int.TryParse(strs[2], out buf);
                NPCOND = buf;
                int.TryParse(strs[3], out buf);
                IHCOFADD = buf;

                line = sr.ReadLine();
                strs = TypeConverterEx.Split<string>(line);
                float.TryParse(strs[0], out ff);
                HCLOSE = ff;
                float.TryParse(strs[1], out ff);
                RCLOSE = ff;
                float.TryParse(strs[2], out ff);
                RELAX = ff;
                float.TryParse(strs[3], out ff);
                NBPOL = ff;
                int.TryParse(strs[4], out buf);
                IPRPCG = buf;
                int.TryParse(strs[5], out buf);
                MUTPCG = buf;
                int.TryParse(strs[6], out buf);
                DAMPPCG = buf;
                float.TryParse(strs[7], out ff);
                DAMPPCGT = ff;

                sr.Close();
                OnLoaded(progresshandler);
                return true;
            }
            else
            {
                OnLoadFailed("Failed to load " + this.Name, progresshandler);
                return false;
            }
        }
        public override bool SaveAs(string filename, ICancelProgressHandler prg)
        {
            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultComment(sw, this.Name);

            string line = string.Format("{0}\t{1}\t{2}\t{3}\t# MXITER, ITER1, NPCOND, IHCOFADD", MXITER, ITER1, NPCOND, IHCOFADD);
            sw.WriteLine(line);
            line = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t# HCLOSE, RCLOSE, RELAX, NBPOL, IPRPCG, MUTPCG, DAMPPCG, DAMPPCGT",
                HCLOSE, RCLOSE, RELAX, NBPOL, IPRPCG, MUTPCG, DAMPPCG, DAMPPCGT);
            sw.WriteLine(line);
            sw.Close();
            OnSaved(prg);
            return true;
        }
        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {

        }
    }
}