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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace Heiflow.Models.Subsurface
{
    [PackageItem]
    [PackageCategory("Basic", false)]
    [CoverageItem]
    [Export(typeof(IMFPackage))]
    public class UPWPackage : MFPackage, INotifyPropertyChanged, IFlowPropertyPackage
    {
        public static string PackageName = "UPW";
        public UPWPackage()
        {
            Name = "UPW";
            _FullName = "Upstream Weighting Package";
            WETFCT = 0.5f;
            IWETIT = 4;
            IHDWET = 0;
            IUPWCB = 9;
            HDRY = -9999.0f;
            NPUPW = 0;
            IPHDRY = 1;
            Options = "CONSTANTCV ";
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".upw";
            _PackageInfo.ModuleName = "UPW";
            IsMandatory = false;
            Version = "UPW";
            _Layer3DToken = "RegularGrid";
            Category = Modflow.FlowCategory;
        }

        #region Properties
        /// <summary>
        ///IUPWCB  > 0, it is the unit number to which cell-by-cell flow terms will be written;IUPWCB = 0, cell-by-cell flow terms will not be written.
        /// IUPWCB  less than 0, cell-by-cell flow for constant-head cells will be written in the listing file when "SAVE BUDGET"
        /// </summary>
        public int IUPWCB { get; set; }
        /// <summary>
        /// the head that is assigned to cells that are converted to dry during a simulation
        /// </summary>
        public float HDRY { get; set; }
        /// <summary>
        /// the number of UPW parameters. 
        /// </summary>
        public int NPUPW { get; set; }
        /// <summary>
        /// If IPHDRY=0, then head will not be set to HDRY.
        /// If IPHDRY>0, then head will be set to HDRY.
        /// </summary>
        public int IPHDRY { get; set; }
        /// <summary>
        /// contains a flag for each layer that specifies the layer type. 0—confined ; not 0—convertible 
        /// </summary>
        public int[] LAYTYP { get; set; }
        /// <summary>
        /// 0—harmonic mean 1—logarithmic mean 2—arithmetic mean of saturated thickness and logarithmic-mean hydraulic conductivity. 
        /// </summary>
        public int[] LAYAVG { get; set; }
        /// <summary>
        /// contains a value for each layer that is a flag or the horizontal anisotropy. 
        /// If CHANI is less than or equal to 0, then variable HANI defines horizontal anisotropy. 
        /// If CHANI is greater than 0, then CHANI is the horizontal anisotropy for the entire layer, and HANI is not read.
        /// </summary>
        public float[] CHANI { get; set; }
        /// <summary>
        /// Contains a flag for each layer that indicates whether variable VKA is vertical hydraulic conductivity or the ratio of horizontal to vertical hydraulic conductivity. 
        /// 0—indicates VKA is vertical hydraulic conductivity; 
        /// not 0—indicates VKA is the ratio of horizontal to vertical hydraulic conductivity, where the horizontal hydraulic conductivity is specified as HK in item 10.
        /// </summary>
        public int[] LAYVKA { get; set; }
        /// <summary>
        /// contains a flag for each layer that indicates if wetting is active. Use as many records as needed to enter a value for each layer.
        /// 	0—indicates wetting is inactive
        ///	not 0—indicates wetting is active
        /// </summary>
        public int[] LAYWET { get; set; }
        /// <summary>
        ///  a factor that is included in the calculation of the head that is initially established at a cell when it is converted from dry to wet.
        /// </summary>
        public float WETFCT { get; set; }
        /// <summary>
        ///  iteration interval for attempting to wet cells. Wetting is attempted every IWETIT iteration.
        /// </summary>
        public int IWETIT { get; set; }
        /// <summary>
        ///  a flag that determines which equation is used to define the initial head at cells that become wet:0 or not 0
        /// </summary>
        [Description("a flag that determines which equation is used to define the initial head at cells that become wet:0 or not 0")]
        public int IHDWET { get; set; }

        /// <summary>
        ///3DMat[ActualLayerCount,1,ActiveCellCount]: hydraulic conductivity along rows
        /// </summary>
        [StaticVariableItem("Layer")]
        [ArealProperty(typeof(float), 10f)]
        [Browsable(false)]
        public DataCube<float> HK { get; set; }

        /// <summary>
        /// 3DMat[ActualLayerCount,1,ActiveCellCount]: the ratio of hydraulic conductivity along columns to hydraulic conductivity along rows
        /// </summary>
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 1f)]
        public DataCube<float> HANI { get; set; }

        /// <summary>
        /// 3DMat[ActualLayerCount,1,ActiveCellCount]:
        /// </summary>
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 0.001f)]
        public DataCube<float> VKA { get; set; }

        /// <summary>
        /// 3DMat[ActualLayerCount,1,ActiveCellCount]:
        /// </summary>
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 0.0001f)]
        public DataCube<float> SS { get; set; }

        /// <summary>
        /// 3DMat[ActualLayerCount,1,ActiveCellCount]:
        /// </summary>
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 0.1f)]
        public DataCube<float> SY { get; set; }

        /// <summary>
        /// 3DMat[ActualLayerCount,1,ActiveCellCount]:
        /// </summary>
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        [ArealProperty(typeof(float), 0.1f)]
        public DataCube<float> WETDRY { get; set; }
        #endregion

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.Grid.Updated += this.OnGridUpdated;
            this.TimeService = Owner.TimeService;
            var mf = Owner as Modflow;
            mf.LayerGroupManager.LayerGroups.CollectionChanged += LayerGroups_CollectionChanged;
            mf.LayerGroupManager.LayerGroupChanged += LayerGroupManager_ItemChanged;
            base.Initialize();
        }


        public override void New()
        {
            this.IUPWCB = ModflowInstance.NameManager.NextFID();
            var cbc_info = new PackageInfo()
            {
                FID = this.IUPWCB,
                FileExtension = ".cbc",
                FileName = string.Format("{0}{1}{2}", Modflow.OutputDic, ModflowInstance.Project.Name, ".cbc"),
                Format = FileFormat.Binary,
                IOState = IOState.REPLACE,
                ModuleName = "DATA",
                WorkDirectory = ModflowInstance.WorkDirectory,
                Name = CBCPackage.PackageName
            };
            ModflowInstance.NameManager.Add(cbc_info);
            base.New();
        }
        public override LoadingState Load(ICancelProgressHandler progresshandler)
        {
            var result = LoadingState.Normal;
            if (File.Exists(FileName))
            {

                var mf = Owner as Modflow;
                var grid = (Owner.Grid as MFGrid);
                int nlayer = grid.ActualLayerCount;

                mf.LayerGroupManager.Clear();
                StreamReader sr = new StreamReader(FileName);
                try
                {
                    //Data Set 1: # IUPWCB, HDRY, NPLPF
                    string newline = ReadComment(sr);
                    float[] fv = TypeConverterEx.Split<float>(newline, 4);
                    int ncell = grid.ActiveCellCount;
                    IUPWCB = (int)fv[0];
                    HDRY = fv[1];
                    NPUPW = (int)fv[2];
                    IPHDRY = (int)fv[3];

                    //Data Set 2: # 
                    newline = sr.ReadLine();
                    LAYTYP = TypeConverterEx.Split<int>(newline, nlayer);

                    //Data Set 3: # 
                    newline = sr.ReadLine();
                    LAYAVG = TypeConverterEx.Split<int>(newline, nlayer);

                    //Data Set 4: # 
                    newline = sr.ReadLine();
                    CHANI = TypeConverterEx.Split<float>(newline, nlayer);

                    //Data Set 5: # 
                    newline = sr.ReadLine();
                    LAYVKA = TypeConverterEx.Split<int>(newline, nlayer);

                    //Data Set 6: # 
                    newline = sr.ReadLine();
                    LAYWET = TypeConverterEx.Split<int>(newline, nlayer);

                    mf.LayerGroupManager.LayerGroups.CollectionChanged -= this.LayerGroups_CollectionChanged;
                    mf.LayerGroupManager.Initialize(grid.ActualLayerCount);
                    mf.LayerGroupManager.ConvertFrom(LAYTYP, "LAYTYP");
                    mf.LayerGroupManager.ConvertFrom(LAYAVG, "LAYAVG");
                    mf.LayerGroupManager.ConvertFrom(CHANI, "CHANI");
                    mf.LayerGroupManager.ConvertFrom(LAYVKA, "LAYVKA");
                    mf.LayerGroupManager.ConvertFrom(LAYWET, "LAYWET");
                    mf.LayerGroupManager.LayerGroups.CollectionChanged += this.LayerGroups_CollectionChanged;

                    HK = new DataCube<float>(nlayer, 1, grid.ActiveCellCount)
                    {
                        Name = "HK",
                        ZeroDimension = DimensionFlag.Spatial
                    };

                    HANI = new DataCube<float>(nlayer, 1, grid.ActiveCellCount)
                    {
                        Name = "HANI",
                        ZeroDimension = DimensionFlag.Spatial
                    };
                    VKA = new DataCube<float>(nlayer, 1, grid.ActiveCellCount)
                    {
                        Name = "VKA",
                        ZeroDimension = DimensionFlag.Spatial
                    };
                    SS = new DataCube<float>(nlayer, 1, grid.ActiveCellCount)
                    {
                        Name = "SS",
                        ZeroDimension = DimensionFlag.Spatial
                    };
                    SY = new DataCube<float>(nlayer, 1, grid.ActiveCellCount)
                    {
                        Name = "SY",
                        ZeroDimension = DimensionFlag.Spatial
                    };
                    WETDRY = new DataCube<float>(nlayer, 1, grid.ActiveCellCount)
                    {
                        Name = "WETDRY",
                        ZeroDimension = DimensionFlag.Spatial
                    };

                    HK.Topology = grid.Topology;
                    HANI.Topology = grid.Topology;
                    VKA.Topology = grid.Topology;
                    SS.Topology = grid.Topology;
                    SY.Topology = grid.Topology;
                    WETDRY.Topology = grid.Topology;

                    for (int l = 0; l < grid.ActualLayerCount; l++)
                    {
                        ReadSerialArray(sr, HK, l, 0);
                        ReadSerialArray(sr, HANI, l, 0);
                        ReadSerialArray(sr, VKA, l, 0);
                        ReadSerialArray(sr, SS, l, 0);

                        if (LAYTYP[l] != 0)
                        {
                            ReadSerialArray(sr, SY, l, 0);
                        }

                        if (LAYTYP[l] != 0 && LAYWET[l] != 0)
                        {
                            ReadSerialArray(sr, WETDRY, l, 0);
                        }

                        HK.Variables[l] = "HK Layer" + (l + 1);
                        HANI.Variables[l] = "HANI Layer" + (l + 1);
                        VKA.Variables[l] = "VKA Layer" + (l + 1);
                        SS.Variables[l] = "SS Layer" + (l + 1);
                        SY.Variables[l] = "SY Layer" + (l + 1);
                        WETDRY.Variables[l] = "WETDRY Layer" + (l + 1);
                    }
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
        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }
        public override void SaveAs(string filename, ICancelProgressHandler prg)
        {
            var grid = (Owner.Grid as IRegularGrid);
            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultComment(sw, this.Name);
            string line = string.Format("{0}\t{1}\t{2}\t{3}\t# IUPWCB , HDRY, NPLPF, IPHDRY, OPTIONS ", IUPWCB, HDRY, NPUPW, IPHDRY, Options);
            sw.WriteLine(line);

            line = TypeConverterEx.Vector2String<int>(LAYTYP) + "\t# LAYTYP";
            sw.WriteLine(line);

            line = TypeConverterEx.Vector2String<int>(LAYAVG) + "\t# LAYAVG";
            sw.WriteLine(line);

            line = TypeConverterEx.Vector2String<float>(CHANI) + "\t# CHANI";
            sw.WriteLine(line);

            line = TypeConverterEx.Vector2String<int>(LAYVKA) + "\t# LAYVKA";
            sw.WriteLine(line);

            for (int i = 0; i < LAYWET.Length; i++)
            {
                LAYWET[i] = 0;
            }
            line = TypeConverterEx.Vector2String<int>(LAYWET) + "\t# LAYWET";
            sw.WriteLine(line);

            //line = string.Format("{0}\t{1}\t{2}\t# WETFCT, IWETIT, IHDWET", WETFCT, IWETIT, IHDWET);
            //sw.WriteLine(line);

            for (int l = 0; l < grid.ActualLayerCount; l++)
            {
                string cmt = string.Format("#HK Layer {0}", l + 1);

                // WriteSerialFloatInternalMatrix(sw, HK[l, 0], 1, "E5", -1, cmt);
                WriteSerialFloatArray(sw, HK, l, 0, "E6", cmt);
                cmt = string.Format("#HANI Layer {0}", l + 1);
                // WriteSerialFloatInternalMatrix(sw, HANI[l, 0], 1, "E5", -1, cmt);
                WriteSerialFloatArray(sw, HANI, l, 0, "E6", cmt);
                cmt = string.Format("#VKA Layer {0}", l + 1);
                //WriteSerialFloatInternalMatrix(sw, VKA[l, 0], 1, "E5", -1, cmt);
                WriteSerialFloatArray(sw, VKA, l, 0, "E6", cmt);
                cmt = string.Format("#SS Layer {0}", l + 1);
                //WriteSerialFloatInternalMatrix(sw, SS[l, 0], 1, "E5", -1, cmt);
                WriteSerialFloatArray(sw, SS, l, 0, "E6", cmt);
                if (LAYTYP[l] != 0)
                {
                    cmt = string.Format("#SY Layer {0}", l + 1);
                    //WriteSerialFloatInternalMatrix(sw, SY[l, 0], 1, "E5", -1, cmt);
                    WriteSerialFloatArray(sw, SY, l, 0, "E6", cmt);
                }
            }
            sw.Close();
            this.OnSaved(prg);
        }
        public override void Clear()
        {
            if (_Initialized)
            {
                this.Grid.Updated -= this.OnGridUpdated;
                var mf = Owner as Modflow;
                mf.LayerGroupManager.LayerGroups.CollectionChanged -= LayerGroups_CollectionChanged;
                mf.LayerGroupManager.LayerGroupChanged -= LayerGroupManager_ItemChanged;
            }
            base.Clear();
        }
        public override void CompositeOutput(MFOutputPackage mfout)
        {
            if (IUPWCB > 0)
            {
                var mf = Owner as Modflow;
                var cbc_info = (from info in mf.NameManager.MasterList where info.FID == this.IUPWCB select info).First();
                cbc_info.Name = CBCPackage.PackageName;
                var CBC = new CBCPackage()
                {
                    Owner = mf,
                    Parent = mfout,
                    PackageInfo = cbc_info,
                    FileName = cbc_info.FileName
                };
                mfout.AddChild(CBC);
                CBC.Initialize();
                var FlowField = new VelocityPackage()
                {
                    Owner = mf,
                    CBCPackage = CBC,
                    PackageInfo = cbc_info,
                    Parent = mfout
                };
                FlowField.Initialize();
                mfout.AddChild(FlowField);
            }
        }
        public override void OnGridUpdated(IGrid sender)
        {
            var mf = Owner as Modflow;
            var grid = sender as RegularGrid;
            int ncell = grid.ActiveCellCount;
            this.FeatureLayer = this.Grid.FeatureLayer;
            this.Feature = this.Grid.FeatureSet;

            //mf.LayerGroupManager.LayerGroups.CollectionChanged -= this.LayerGroups_CollectionChanged;
            //mf.LayerGroupManager.Initialize(grid.ActualLayerCount);
            //this.LAYTYP = mf.LayerGroupManager.ConvertToInt("LAYTYP");
            //this.LAYAVG = mf.LayerGroupManager.ConvertToInt("LAYAVG");
            //this.CHANI = mf.LayerGroupManager.ConvertToFloat("CHANI");
            //this.LAYVKA = mf.LayerGroupManager.ConvertToInt("LAYVKA");
            //this.LAYWET = mf.LayerGroupManager.ConvertToInt("LAYWET");
            //mf.LayerGroupManager.LayerGroups.CollectionChanged += this.LayerGroups_CollectionChanged;

            HK = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount)
            {
                Name = "HK",
                ZeroDimension = DimensionFlag.Spatial
            };
            HANI = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount)
            {
                Name = "HANI",
                ZeroDimension = DimensionFlag.Spatial
            };
            VKA = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount)
            {
                Name = "VKA",
                ZeroDimension = DimensionFlag.Spatial
            };
            SS = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount)
            {
                Name = "SS",
                ZeroDimension = DimensionFlag.Spatial
            };
            SY = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount)
            {
                Name = "SY",
                ZeroDimension = DimensionFlag.Spatial
            };
            WETDRY = new DataCube<float>(grid.ActualLayerCount, 1, grid.ActiveCellCount)
            {
                Name = "WETDRY",
                ZeroDimension = DimensionFlag.Spatial
            };

            HK.Topology = grid.Topology;
            HANI.Topology = grid.Topology;
            VKA.Topology = grid.Topology;
            SS.Topology = grid.Topology;
            SY.Topology = grid.Topology;
            WETDRY.Topology = grid.Topology;

            var hk = mf.LayerGroupManager.ConvertToFloat("HK");
            var vka = mf.LayerGroupManager.ConvertToFloat("VKA");
            var ss = mf.LayerGroupManager.ConvertToFloat("SS");
            var sy = mf.LayerGroupManager.ConvertToFloat("SY");
            var wetdry = mf.LayerGroupManager.ConvertToFloat("WETDRY");

            for (int l = 0; l < grid.ActualLayerCount; l++)
            {
                //if (LAYTYP[l] != 0)
                //{
                //    SY.Value[l][0] = new float[ncell];
                //    if(LAYWET[l] != 0)
                //        WETDRY.Value[l][0] = new float[ncell];
                //}
                for (int i = 0; i < ncell; i++)
                {
                    HK[l, 0, i] = hk[l];
                    HANI[l, 0, i] = 1;
                    VKA[l, 0, i] = vka[l];
                    SS[l, 0, i] = ss[l];
                    if (LAYTYP[l] != 0)
                    {
                        SY[l, 0, i] = sy[l];
                    }
                    if (LAYTYP[l] != 0 && LAYWET[l] != 0)
                    {
                        WETDRY[l, 0, i] = wetdry[l];
                    }
                }
                HK.Variables[l] = "HK Layer" + (l + 1);
                HANI.Variables[l] = "HANI Layer" + (l + 1);
                VKA.Variables[l] = "VKA Layer" + (l + 1);
                SS.Variables[l] = "SS Layer" + (l + 1);
                SY.Variables[l] = "SY Layer" + (l + 1);
                WETDRY.Variables[l] = "WETDRY Layer" + (l + 1);
            }
            base.OnGridUpdated(sender);
        }
        public string Check()
        {
            string msg = "**********Checking layer property************\n";
            //int r = 0;
            //int c = 0;
            //  var grid = (Owner.Grid as RegularGrid);

            //foreach (var ac in grid.Topology.ActiveCellLocation)
            //{
            //    r = ac.Value[0];
            //    c = ac.Value[1];
            //    for (int l = 0; l < grid.ActualLayerCount; l++)
            //    {
            //        if (HK[r, c, l] <= 0)
            //        {
            //            msg += string.Format("HK\t{0}\t{1} equals to 0\n", r + 1, c + 1);
            //        }
            //        if (VKA[r, c, l] <= 0)
            //        {
            //            msg += string.Format("VKA\t{0}\t{1} equals to 0\n", r + 1, c + 1);
            //        }
            //    }
            //}
            return msg;
        }
        public override IPackage Extract(Modflow newmf)
        {
            UPWPackage lpf = new UPWPackage();
            lpf.Owner = newmf;
            lpf.IUPWCB = this.IUPWCB;
            lpf.HDRY = this.HDRY;
            lpf.NPUPW = this.NPUPW;
            lpf.IPHDRY = this.IPHDRY;
            lpf.LAYTYP = this.LAYTYP;
            lpf.LAYAVG = this.LAYAVG;
            lpf.CHANI = this.CHANI;
            lpf.LAYVKA = this.LAYVKA;
            lpf.LAYWET = this.LAYWET;
            lpf.WETFCT = this.WETFCT;
            lpf.IWETIT = this.IWETIT;
            lpf.IHDWET = this.IHDWET;

            MFGrid newgrid = newmf.Grid as MFGrid;
            MFGrid rawgrid = Owner.Grid as MFGrid;

            lpf.HK = new DataCube<float>(newgrid.ActualLayerCount, 1, newgrid.ActiveCellCount) { ZeroDimension = DimensionFlag.Spatial };
            lpf.HANI = new DataCube<float>(newgrid.ActualLayerCount, 1, newgrid.ActiveCellCount) { ZeroDimension = DimensionFlag.Spatial };
            lpf.VKA = new DataCube<float>(newgrid.ActualLayerCount, 1, newgrid.ActiveCellCount) { ZeroDimension = DimensionFlag.Spatial };
            lpf.SS = new DataCube<float>(newgrid.ActualLayerCount, 1, newgrid.ActiveCellCount) { ZeroDimension = DimensionFlag.Spatial };
            lpf.SY = new DataCube<float>(newgrid.ActualLayerCount, 1, newgrid.ActiveCellCount) { ZeroDimension = DimensionFlag.Spatial };
            lpf.WETDRY = new DataCube<float>(newgrid.ActualLayerCount, 1, newgrid.ActiveCellCount) { ZeroDimension = DimensionFlag.Spatial };
            for (int l = 0; l < newgrid.ActualLayerCount; l++)
            {
                //lpf.HK[l, 0] = rawgrid.GetSubSerialArray<float>(this.HK, newgrid, l).Value;
                //lpf.HANI[l, 0] = rawgrid.GetSubSerialArray<float>(this.HANI, newgrid, l).Value;
                //lpf.VKA[l, 0] = rawgrid.GetSubSerialArray<float>(this.VKA, newgrid, l).Value;
                //lpf.SS[l, 0] = rawgrid.GetSubSerialArray<float>(this.SS, newgrid, l).Value;
                //if (lpf.LAYTYP[l] != 0)
                //    lpf.SY[l, 0] = rawgrid.GetSubSerialArray<float>(this.SY, newgrid, l).Value;
                //if (LAYTYP[l] != 0 && LAYWET[l] != 0)
                //    lpf.WETDRY[l, 0] = rawgrid.GetSubSerialArray<float>(this.WETDRY, newgrid, l).Value;
            }

            return lpf;
        }

        private void LayerGroupManager_ItemChanged(object sender, LayerGroup e, string propname)
        {
            var grid = this.Grid as RegularGrid;
            int layer = e.LayerIndex;
            int ncell = grid.ActiveCellCount;
            switch (propname)
            {
                case "LAYTYP":
                    LAYTYP[layer] = (int)e.LAYTYP;
                    break;
                case "LAYAVG":
                    LAYAVG[layer] = (int)e.LAYAVG;
                    break;
                case "CHANI":
                    CHANI[layer] = (int)e.CHANI;
                    break;
                case "LAYVKA":
                    LAYVKA[layer] = (int)e.LAYVKA;
                    break;
                case "LAYWET":
                    LAYWET[layer] = (int)e.LAYWET;
                    break;
                case "HK":
                    for (int i = 0; i < ncell; i++)
                    {
                        HK[layer, 0, i] = (float)e.HK;
                        HANI[layer, 0, i] = 1;
                    }
                    break;
                case "VKA":
                    for (int i = 0; i < ncell; i++)
                    {
                        VKA[layer, 0, i] = (float)e.VKA;
                    }
                    break;
                case "SS":
                    for (int i = 0; i < ncell; i++)
                    {
                        SS[layer, 0, i] = (float)e.SS;
                    }
                    break;
                case "SY":
                    if (LAYTYP[layer] != 0)
                    {
                        for (int i = 0; i < ncell; i++)
                        {
                            SY[layer, 0, i] = (float)e.SY;
                        }
                    }
                    break;
                case "WETDRY":
                    if (LAYTYP[layer] != 0 && LAYWET[layer] != 0)
                    {
                        for (int i = 0; i < ncell; i++)
                        {
                            WETDRY[layer, 0, i] = (float)e.WETDRY;
                        }
                    }
                    break;
            }
        }

        private void LayerGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var mf = Owner as Modflow;
            LAYTYP = mf.LayerGroupManager.ConvertToInt("LAYTYP");
            LAYAVG = mf.LayerGroupManager.ConvertToInt("LAYAVG");
            CHANI = mf.LayerGroupManager.ConvertToFloat("CHANI");
            LAYVKA = mf.LayerGroupManager.ConvertToInt("LAYVKA");
            LAYWET = mf.LayerGroupManager.ConvertToInt("LAYWET");
        }
    }
}
