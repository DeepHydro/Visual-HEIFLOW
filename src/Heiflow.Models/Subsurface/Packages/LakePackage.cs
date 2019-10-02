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
using Heiflow.Models.Subsurface.Packages;

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
        /// [1,NLAKES,5] (STAGES,SSMN,SSMX,IUNITTAB,CLAKE)
        /// </summary>
        /// 
       [StaticVariableItem]
        public DataCube2DLayout<float> STAGES { get; set; }

        //*****DataSet4
        /// <summary>
        /// [1,nperiod,3] (ITMP,ITMP1, LWRT )
        /// </summary>
        /// 
       [StaticVariableItem]
       public DataCube<int> ITMP { get; set; }

        //*****DataSet5
        /// <summary>
        ///[nlayer,1,nactcell]   0, the grid cell is not a lake volume cell. > 0, its value is the identification number of the lake occupying the grid cell. 
        /// </summary>
       [StaticVariableItem]
       public DataCube<int> LKARR { get; set; }

        //*****DataSet6
        /// <summary>
       /// [nlake,1,nactcell]. the lakebed leakance that will be assigned to lake/aquifer interfaces that occur in the corresponding grid cell.
        /// </summary>
        /// 
       [StaticVariableItem]
       public DataCube<float> BDLKNC { get; set; }

        /// <summary>
       /// [1,1, nsp] The number of sublake systems if coalescing/dividing lakes are to be simulated (only in transient runs). Enter 0 if no sublake systems are to be simulated.
        /// </summary>
        /// 
        [StaticVariableItem]
       public DataCube<int> NSLMS { get; set; }

        /// <summary>
        /// [np,nlake,6]: PRCPLK EVAPLK RNF WTHDRW [SSMN] [SSMX]
        /// </summary>
        /// 
       [StaticVariableItem]
        public DataCube2DLayout<float> WSOUR { get; set; }
        #endregion

        public override void Initialize()
        {
            Message = "";
            this.Grid = Owner.Grid;
            this.Grid.Updated += this.OnGridUpdated;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            base.Initialize();
        }
        public override bool New()
        {

            return base.New();
        }
        public override bool Load(ICancelProgressHandler progresshandler)
        {
            var mf = (Owner as Modflow);
             var grid = (Owner.Grid as MFGrid);
            int nsp = TimeService.StressPeriods.Count;
            int nlayer= grid.ActualLayerCount;
            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName);
                string line = sr.ReadLine();
                line = sr.ReadLine();
                //7	0	 # DataSet 1b: NLAKES ILKCB
                //-0.5 	100	 0.01 	0.5	 # DataSet 2: THETA NSSITR SSCNCR SURFDEPTH
                int[] intbuf = TypeConverterEx.Split<int>(line, 2);
                NLAKES = intbuf[0];
                ILKCB = intbuf[1];
                line = sr.ReadLine();
                var floatbuf = TypeConverterEx.Split<float>(line, 4);
                THETA = floatbuf[0];
                NSSITR = (int)floatbuf[1];
                SSCNCR = floatbuf[2];
                SURFDEPTH = floatbuf[3];

                STAGES = new DataCube2DLayout<float>(1, NLAKES, 5, true);
                for (int i = 0; i < NLAKES; i++)
                {
                    line = sr.ReadLine();
                    floatbuf = TypeConverterEx.Split<float>(line, 3);
                    STAGES[0][i, ":"] = floatbuf;
                }

                ITMP = new DataCube<int>(1, nsp, 3,true);
                LKARR = new DataCube<int>(nlayer, 1, grid.ActiveCellCount,true)
                {
                    Name = "Lake ID",
                    TimeBrowsable = false,
                    AllowTableEdit = true,
                    Variables = new string[nlayer]
                };
                for (int l = 0; l < nlayer; l++)
                {
                    LKARR.Variables[l] = "Lake ID of " + " Layer " + (l + 1);
                }

                BDLKNC = new DataCube<float>(nlayer, 1, grid.ActiveCellCount, false)
                {
                    Name = "Leakance",
                    TimeBrowsable = false,
                    AllowTableEdit = true,
                    Variables = new string[nlayer]
                };
                for (int l = 0; l < nlayer; l++)
                {
                    BDLKNC.Variables[l] = " Layer " + (l + 1);
                }
                NSLMS = new DataCube<int>(nsp, 1, 1,true)
                {
                    Name = "Num of Sublakes",
                    TimeBrowsable = false,
                    AllowTableEdit = true,
                    Variables = new string[nsp]
                };
                for (int l = 0; l < nsp; l++)
                {
                    NSLMS.Variables[l] = "Stress Period " + (l + 1);
                }
                WSOUR = new  DataCube2DLayout<float>(nsp, NLAKES, 6,true)
                {
                    Name = "Recharge Discharge",
                    TimeBrowsable = false,
                    AllowTableEdit = true,
                    Variables = new string[nsp],
                    Layout = DataCubeLayout.TwoD
                };
                for (int l = 0; l < nsp; l++)
                {
                    WSOUR.Variables[l] = "Stress Period " + (l + 1);
                }


                line = sr.ReadLine();
                intbuf = TypeConverterEx.Split<int>(line, 3);



            }
            return true;
        }
        public override bool SaveAs(string filename, ICancelProgressHandler progress)
        {
            return true;
        }
        public override string CreateFeature(DotSpatial.Projections.ProjectionInfo proj_info, string directory)
        {
            return base.CreateFeature(proj_info, directory);
        }
        public override void Clear()
        {
            if (_Initialized)
            {
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
                this.Grid.Updated -= this.OnGridUpdated;
            }
            base.Clear();
        }
        public override void OnGridUpdated(IGrid sender)
        {
            this.FeatureLayer = this.Grid.FeatureLayer;
            this.Feature = this.Grid.FeatureSet;
            base.OnGridUpdated(sender);
        }
        public override void OnTimeServiceUpdated(ITimeService time)
        {
            var nsp = time.StressPeriods.Count;
        }
    }
}
