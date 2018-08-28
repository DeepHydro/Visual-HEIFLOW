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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Models.Generic;
using System.IO;
using System.ComponentModel.Composition;
using Heiflow.Models.Properties;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Integration;
using Heiflow.Models.Generic.Parameters;
using System.Collections.ObjectModel;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.UI;
using Heiflow.Core.Data;
using System.ComponentModel;
using Heiflow.Core.Utility;
using DotSpatial.Data;

namespace Heiflow.Models.Surface.PRMS
{
    [Export(typeof(IBasicModel))]
    [ModelItem]
    public class PRMS : BaseModel
    {
        public const string InputDic = ".\\Input\\";
        private MMSPackage _mmsPackage;
        private PRMSOutputDataPackage _outputPackage;
        private PRMSInputDataPackage _inputPackage;
        private PRMSDrivingDataPackage _drivingPackage;
        private MasterPackage _master;

        public PRMS()
        {
            Name = "Surface";
            Icon = Resources.mf16;
            LargeIcon = Resources.mf32;
            _mmsPackage = new MMSPackage("PRMS Package")
            {
                Owner = this
            };
            _outputPackage = new PRMSOutputDataPackage()
            {
                Owner = this
            };
            _inputPackage = new PRMSInputDataPackage()
            {
                Owner = this
            };
            _drivingPackage = new  PRMSDrivingDataPackage()
            {
                Owner = this
            };
            _mmsPackage.Owner = this;
            AddInSilence(_inputPackage);
            AddInSilence(_outputPackage);
            AddInSilence(_drivingPackage);
            Description = "A deterministic, distributed-parameter, physical process based surface water model";
        }
        [Browsable(false)]
        public int NHRU
        {
            get;
            set;
        }
          [Browsable(false)]
        public MMSPackage MMSPackage
        {
            get
            {
                return _mmsPackage;
            }
        }
          [Browsable(false)]
        public MasterPackage MasterPackage
        {
            get
            {
                return _master;
            }
            set
            {
                _master = value;
                _inputPackage.MasterPackage = _master;
                _outputPackage.MasterPackage = _master;
                _drivingPackage.MasterPackage = _master;
            }
        }

        public override void Initialize()
        {
            _mmsPackage.FileName = ControlFileName;
            var pcks = new Package[] { _mmsPackage, _inputPackage, _outputPackage, _drivingPackage };
            foreach (var pck in pcks)
            {
                pck.Initialize();
            }
           
        }

        public override bool Load(ICancelProgressHandler progress)
        {
            bool succ = true;
            if (File.Exists(ControlFileName))
            {
                succ=_mmsPackage.Load(progress);
                if (succ)
                {
                    ResolveLoadedParameters(true);
                    ResolveModules();
                    foreach(var pck in Packages.Values)
                    {
                        pck.AfterLoad();
                    }
                    progress.Progress("PRMS",1,"Parameters loaded.");
                }
                else
                {
                    progress.Progress("PRMS", 1, string.Format("\r\n Failed to load parameter file. Error message: {1}", _mmsPackage.Message));
                }
            }
            else
            {
                succ = false;
                progress.Progress("PRMS", 1, string.Format("\r\n Failed to load {0}. The parameter file does not exist: {1}", Name, ControlFileName));
            }
            return succ;
        }

        public override bool LoadGrid(ICancelProgressHandler progress)
        {
            return true;
        }

        public override bool Validate()
        {
            return false;
        }

        public override bool New(ICancelProgressHandler progress)
        {
            string parafile = Path.Combine(BaseModel.ConfigPath, "heiflow.param");
            var succ = true;
            if (File.Exists(parafile))
            {
                File.Copy(parafile, _mmsPackage.FileName, true);
                succ = _mmsPackage.Load(progress);
                if (succ)
                {
                    ResolveLoadedParameters(false);
                    ResolveModules();
                    _mmsPackage.FileName = this.ControlFileName;
                    succ = _inputPackage.New();
                    succ = _outputPackage.New();
                }
                else
                {
                    var msg =string.Format("\r\n Failed to load {0}. Error: {1}", Name, _mmsPackage.Message);
                    progress.Progress(this.Name, 1, msg);
                }
            }
            else
            {
                succ = false;
                 var msg =string.Format("The template parameter file {0} is missing. Please repair the software.", parafile);
                 progress.Progress(this.Name, 1, msg);
            }
            return succ;
        }

        public override void Clear()
        {
            foreach (var pck in Packages.Values)
            {
                pck.Clear();
            }
            Packages.Clear();
        }

        public override void Attach(DotSpatial.Controls.IMap map,  string directory)
        {
            foreach (var pck in Packages.Values)
            {
                pck.Attach(map, directory);
                foreach (var ch in pck.Children)
                {
                    ch.Attach(map, directory);
                }
            }
        }

        public override void Save(ICancelProgressHandler progress)
        {
            _mmsPackage.Save(progress);
            _inputPackage.Save(progress);
            _drivingPackage.Save(progress);
        }

        public override void OnTimeServiceUpdated(ITimeService sender)
        {
            var ndays = _mmsPackage.Select("ndays");
            if(ndays != null)
                ndays.SetValue(sender.NumTimeStep, 0);
        }

        public override void OnGridUpdated(IGrid sender)
        {
    
        }

        public void ResolveModules()
        {
            var para = from par in _mmsPackage.Parameters.Values
                       group par by par.ModuleName into pp
                       select new
                       {
                           Module = pp.Key,
                           Paras = pp.ToArray()
                       };

            foreach (var p in para)
            {
                MMSPackage pk = new MMSPackage(p.Module.ToString());
                pk.Owner = this;
                pk.FileName = _mmsPackage.FileName;
                foreach (var ar in p.Paras)
                {
                    pk.Parameters.Add(ar.Name, ar);
                }
               // pk.Initialize();
                AddInSilence(pk);
            }
        }

        public void ResolveLoadedParameters(bool is_load)
        {
            string _Configfile = Path.Combine(ConfigPath, "mms.config.xml");
            if (File.Exists(_Configfile))
            {
                _mmsPackage.LoadDefaultPara(_Configfile);
                foreach (var para in _mmsPackage.Parameters)
                {
                    var pp = (from pr in _mmsPackage.DefaultParameters where pr.Name == para.Key select pr).FirstOrDefault();
                    if (pp != null)
                    {
                        para.Value.ModuleName = pp.ModuleName;
                        para.Value.DefaultValue = pp.DefaultValue;
                        para.Value.Description = pp.Description;
                        para.Value.Maximum = pp.Maximum;
                        para.Value.Minimum = pp.Minimum;
                        para.Value.Units = pp.Units;
                    }
                }
            }
            if (is_load)
            {
                var par = from pa in _mmsPackage.Parameters where pa.Key.ToLower() == "nhru" select pa;
                NHRU = (int)par.First().Value.ToDouble().First();
                ModelService.NHRU = NHRU;

                var topo = (this.Grid as RegularGrid).Topology;
                foreach (var para in _mmsPackage.Parameters)
                {
                    if (para.Value.ValueCount == NHRU)
                        (para.Value as IDataCubeObject).Topology = topo;
                }
                var basin_area = _mmsPackage.Select("basin_area");
                var area = basin_area.ToDouble().Average() * ConstantNumber.Acre2SqM;
                ModelService.BasinArea = area;
            }
        }
 
    }
}
