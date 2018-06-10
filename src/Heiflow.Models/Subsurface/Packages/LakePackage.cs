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
using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface
{
    [PackageItem]
    [PackageCategory("Boundary Conditions,Head Dependent Flux",false)]
    [CoverageItem]
    [Export(typeof(IMFPackage))]
    public class LakePackage : MFPackage
    {
        public static string PackageName = "LAK";

        public LakePackage()
        {
            Name = "LAK";
            _FullName = "Lake Package";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".lak";
            _PackageInfo.ModuleName = "LAK";
            IsMandatory = false;

            ILKCB = 0;
            THETA = -0.5f;
            NSSITR = 100;
            SSCNCR = 0.01f;
            SURFDEPTH = 0.1f;
            Version = "Lak3";
        }

        #region Properties
        public int[] LakeID { get; set; }

        //*****DataSet1b
        /// <summary>
        /// Number of separate lakes
        /// </summary>
        public int NLAKES { get; set; }

        /// <summary>
        /// Number of separate lakes
        /// </summary>
        public int ILKCB { get; set; }

        //*****DataSet2
        /// <summary>
        /// Explicit (THETA = 0.0), semi-implicit (0.0 .GT. THETA .LT. 1.0), or implicit (THETA = 1.0) solution for lake stages.
        /// SURFDEPTH is read only if THETA is assigned a negative value 
        /// </summary>
        public float THETA { get; set; }
        /// <summary>
        /// Maximum number of iterations for Newton’s method of solution for equilibrium lake stages 
        /// </summary>
        public int NSSITR { get; set; }
        /// <summary>
        /// Convergence criterion 
        /// </summary>
        public float SSCNCR { get; set; }
        /// <summary>
        /// The height of small topological variations 
        /// </summary>
        public float SURFDEPTH { get; set; }

        //*****DataSet3
        /// <summary>
        /// [NLAKES,5] (STAGES,SSMN,SSMX,IUNITTAB,CLAKE)
        /// </summary>
        public float[,] STAGES { get; set; }

        //*****DataSet4
        /// <summary>
        /// [nperiod,3] (ITMP,ITMP1, LWRT )
        /// </summary>
        public int[,] NPINFO { get; set; }

        //*****DataSet5
        /// <summary>
        ///  0, the grid cell is not a lake volume cell. > 0, its value is the identification number of the lake occupying the grid cell. 
        /// </summary>
        public MatrixCube<float> LKARR { get; set; }

        //*****DataSet6
        /// <summary>
        ///  the lakebed leakance that will be assigned to lake/aquifer interfaces that occur in the corresponding grid cell.
        /// </summary>
        public MatrixCube<float> BDLKNC { get; set; }

        //*****DataSet7
        public int NSLMS { get; set; }

        //*****DataSet8a
        /// <summary>
        /// [nlake, 2] (IC,ISUB)
        /// </summary>
        public int[,] MultipleLakeInfo { get; set; }
        //*****DataSet8b
        /// <summary>
        ///  Sill elevation that determines whether the center lake is connected with a given sublake
        /// </summary>
        public float[,] SILLVT { get; set; }
        /// <summary>
        /// [np][nlake,6]: PRCPLK EVAPLK RNF WTHDRW [SSMN] [SSMX]
        /// </summary>
        public float[][,] WSOUR { get; set; }
        #endregion

        public override void Initialize()
        {
            base.Initialize();
        }

        public override bool Load(ICancelProgressHandler progresshandler)
        {
            return true;
        }
        public override bool SaveAs(string filename, ICancelProgressHandler progress)
        {
            return true;
        }

    }
}
