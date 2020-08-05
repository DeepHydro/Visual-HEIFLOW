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
using Heiflow.Core.Utility;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Models.Generic.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Heiflow.Models.Surface.PRMS
{
    [MMSPackageItem]
    [CoverageItem]
    public class MMSPackage : Package, IMMSPackage
    {
        private string[] dim_name = new string[] { "Dimension" };
        public string[] _nhru_dim_names = new string[] { "nhru", "nssr", "ngw", "nhrucell", "ngwcell" };
        protected string[] _nlayer_dim_names = new string[] { "nlayer"};
        private Modules[] _IgnoredModules = new Modules[] { Modules.muskingum, Modules.potet_hamon, Modules.potet_hs,
            Modules.potet_jh, Modules.potet_pan, Modules.precip_dist2_prms, Modules.precip_laps_prms, Modules.precip_prms,
            Modules.prms_only, Modules.temp_dist2_prms, Modules.xyz_dist};

        public MMSPackage(string name)
            : base(name)
        {
            IsDirty = false;
            IsMandatory = true;
            _Layer3DToken = "RegularGrid";
        }

        public MMSPackage()
        {
            Name = "Parameter file";
            FullName = "Parameter file";
            IsDirty = false;
            IsMandatory = true;
            _Layer3DToken = "RegularGrid";
        }


        [XmlArrayItem]
        [Browsable(false)]
        public string[] ModuleNames
        {
            get;
            set;
        }


        [XmlIgnore]
        [Browsable(false)]
        [ArealProperty(typeof(IParameter), null)]
        public new Dictionary<string, IParameter> Parameters
        {
            get
            {
                return _Parameters;
            }
            private set
            {
                _Parameters = value;
                OnPropertyChanged("Parameters");
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public Modules[] IgnoredModules
        {
            get
            {
                return _IgnoredModules;
            }
        }

        public IParameter Select(string para_name)
        {
            var par = from pa in Parameters where pa.Key.ToLower() == para_name.ToLower() select pa.Value;
            if (par.Count() > 0)
                return par.First();
            else
                return null;
        }

        public override void New()
        {
            IsDirty = true;
        }

        public override void Initialize()
        {
            Message = "";
            this.Grid = Owner.Grid;
            this.Grid.Updated += this.OnGridUpdated;
            this.TimeService = Owner.TimeService;
            _Initialized = true;
        }
        public LoadingState Load(string filename, ICancelProgressHandler progress)
        {
            var result = LoadingState.Normal;
            if (File.Exists(filename))
            {
                StreamReader sr = new StreamReader(filename);
                try
                {
                    var txt = sr.ReadToEnd().Trim();
                    var lines = txt.Split(new char[] { '\n' });
                    string newline = "";
                    int dimRow = 0;
                    int paraRow = 0;
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].ToLower().Contains("dimensions"))
                        {
                            dimRow = i;
                        }
                        if (lines[i].ToLower().Contains("parameters"))
                        {
                            paraRow = i;
                            break;
                        }
                    }
                    //read dimensions
                    for (int i = dimRow + 1; i < paraRow; )
                    {
                        i++;
                        newline = lines[i].Trim();
                        DataCubeParameter<int> gv = new DataCubeParameter<int>(1, 1, 1, false)
                        {
                            Dimension = 1,
                            VariableType = ParameterType.Dimension,
                            DimensionNames = dim_name,
                            Name = newline,
                            Owner = this
                        };
                        i++;
                        newline = lines[i].Trim();
                        gv.SetValue(0, 0, 0, int.Parse(newline));
                        if (!Parameters.Keys.Contains(gv.Name))
                            Parameters.Add(gv.Name, gv);
                        i++;
                    }
                    // add fixed dimension
                    AddFixDimension();
                    //read parameters
                    for (int i = paraRow + 1; i < lines.Length; )
                    {
                        i++;
                        newline = TypeConverterEx.Split<string>(lines[i], 1)[0];
                        string name = newline.Trim();
                        i++;
                        newline = lines[i];
                        int dimension = int.Parse(newline.Trim());
                        string[] dimensionNames = new string[dimension];
                        for (int d = 0; d < dimension; d++)
                        {
                            i++;
                            dimensionNames[d] = lines[i].Trim();
                        }
                        i++;
                        newline = lines[i];
                        int valueCount = int.Parse(newline.Trim());
                        i++;
                        newline = lines[i];
                        int valueType = int.Parse(newline.Trim());

                        int nrow = int.Parse(Parameters[dimensionNames[0]].GetValue(0, 0, 0).ToString());
                        int ncol = 1;
                        if (dimension > 1)
                            ncol = int.Parse(Parameters[dimensionNames[1]].GetValue(0, 0, 0).ToString());

                        if (nrow == 0 || ncol == 0)
                        {
                            nrow = valueCount;
                            ncol = 1;
                        }

                        if (valueType == 0)
                        {
                            DataCubeParameter<short> gv = new DataCubeParameter<short>(1, nrow, ncol, false)
                            {
                                ValueType = valueType,
                                VariableType = ParameterType.Parameter,
                                Dimension = dimension,
                                DimensionNames = dimensionNames,
                                Owner = this,
                                Name = name
                            };
                            gv.FromStringArrays(lines, i + 1, i + valueCount);
                            if (!Parameters.Keys.Contains(gv.Name))
                                Parameters.Add(gv.Name, gv);
                        }
                        else if (valueType == 1)
                        {
                            DataCubeParameter<int> gv = new DataCubeParameter<int>(1, nrow, ncol, false)
                            {
                                ValueType = valueType,
                                VariableType = ParameterType.Parameter,
                                Dimension = dimension,
                                DimensionNames = dimensionNames,
                                Owner = this,
                                Name = name
                            };
                            gv.FromStringArrays(lines, i + 1, i + valueCount);
                            if (!Parameters.Keys.Contains(gv.Name))
                                Parameters.Add(gv.Name, gv);
                        }
                        else if (valueType == 2)
                        {
                            DataCubeParameter<float> gv = new DataCubeParameter<float>(1, nrow, ncol, false)
                            {
                                ValueType = valueType,
                                VariableType = ParameterType.Parameter,
                                Dimension = dimension,
                                DimensionNames = dimensionNames,
                                Owner = this,
                                Name = name
                            };
                            gv.FromStringArrays(lines, i + 1, i + valueCount);
                            if (!Parameters.Keys.Contains(gv.Name))
                                Parameters.Add(gv.Name, gv);
                        }
                        else if (valueType == 3)
                        {
                            DataCubeParameter<double> gv = new DataCubeParameter<double>(1, nrow, ncol, false)
                            {
                                ValueType = valueType,
                                VariableType = ParameterType.Parameter,
                                Dimension = dimension,
                                DimensionNames = dimensionNames,
                                Owner = this,
                                Name = name
                            };
                            gv.FromStringArrays(lines, i + 1, i + valueCount);
                            if (!Parameters.Keys.Contains(gv.Name))
                                Parameters.Add(gv.Name, gv);
                        }
                        else if (valueType == 4)
                        {
                            DataCubeParameter<string> gv = new DataCubeParameter<string>(1, nrow, ncol, false)
                            {
                                ValueType = valueType,
                                VariableType = ParameterType.Parameter,
                                Dimension = dimension,
                                DimensionNames = dimensionNames,
                                Owner = this,
                                Name = name
                            };
                            gv.FromStringArrays(lines, i + 1, i + valueCount);
                            if (!Parameters.Keys.Contains(gv.Name))
                                Parameters.Add(gv.Name, gv);
                        }

                        i += valueCount + 1;
                    }

                    result = LoadingState.Normal;
                }
                catch (Exception ex)
                {
                    result = LoadingState.FatalError;
                    Message = string.Format("Failed to load {0}. Error message: {1}", Name, ex.Message);
                    ShowWarning(Message, progress);
                }
                finally
                {
                    sr.Close();
                }
            }
            else
            {
                Message = string.Format("\tFailed to load {0}. The package file does not exist: {1}", Name, FileName);
                ShowWarning(Message, progress);
                result = LoadingState.FatalError;
            }
            OnLoaded(progress, new LoadingObjectState() { Message = Message, Object = this, State = result });
            return result;
        }
        public override LoadingState Load(ICancelProgressHandler progress)
        {
            return Load(FileName, progress);
        }
        public void AddFixDimension()
        {
            DataCubeParameter<int> gv = new DataCubeParameter<int>(1, 1, 1, false)
            {
                Dimension = 1,
                VariableType = ParameterType.Dimension,
                DimensionNames = dim_name,
                Name = "ndays",
                Owner = this,
                IsFixed = true
            };
            gv.SetValue(0, 0, 0, 366);
            if (!Parameters.Keys.Contains(gv.Name))
                Parameters.Add(gv.Name, gv);

            gv = new DataCubeParameter<int>(1, 1, 1, false)
            {
                Dimension = 1,
                VariableType = ParameterType.Dimension,
                DimensionNames = dim_name,
                Name = "nmonths",
                Owner = this,
                IsFixed = true
            };
            gv.SetValue(0, 0, 0, 12);
            if (!Parameters.Keys.Contains(gv.Name))
                Parameters.Add(gv.Name, gv);

            gv = new DataCubeParameter<int>(1, 1, 1, false)
            {
                Dimension = 1,
                VariableType = ParameterType.Dimension,
                DimensionNames = dim_name,
                Name = "nhours24",
                Owner = this,
                IsFixed = true
            };
            gv.SetValue(0, 0, 0, 24);
            if (!Parameters.Keys.Contains(gv.Name))
                Parameters.Add(gv.Name, gv);
        }
        public override void Save(ICancelProgressHandler progress)
        {
            if (IsDirty)
            {
                Save(FileName,progress);
            }
            else
            {
                if(progress != null)
                    progress.Progress(this.Name, 1, "Parameter file unchanged.");
            }
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            Save(filename,progress);
        }

        private void Save(string filename,ICancelProgressHandler prg)
        {
            string ext = Path.GetExtension(filename).ToLower();
            try
            {
                if (ext == ".param")
                {
                    Save2Param(filename,prg);
                }
                else if (ext == ".csv")
                {
                    Save2Csv(filename,prg);
                }
                IsDirty = false;
            }
            catch (Exception ex)
            {
                IsDirty = false;
                Message = ex.Message;
            }
        }

        public override void Clear()
        {
            if (_Initialized)
                this.Grid.Updated -= this.OnGridUpdated;
            _Initialized = false;
        }
        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }

        private void Save2Param(string filename,ICancelProgressHandler progress)
        {
            int percent = 0;
            int total = 0;
            int index_dim = 0;
            bool skip = false;
            string line = "";
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            StreamWriter sw = new StreamWriter(filename);
            string buf = string.Format("Parameter file generated by {0}\nVersion: {1} Date:{2} ", fvi.ProductName, fvi.FileVersion, DateTime.Now.ToString());
            sw.WriteLine(buf);
            buf = "** Dimensions **\n";
            MMSPackage mms = this;
            if (this.Owner != null)
                mms = (this.Owner as PRMS).MMSPackage;
            var dim = from p in mms.Parameters where p.Value.VariableType == ParameterType.Dimension select p;
            var zero_value_dim = new List<string>();
            foreach (var dd in dim)
            {
                var gv = dd.Value;
                var dim_value = int.Parse(gv.GetValue(0, 0, 0).ToString());
                if (!gv.IsFixed)
                {
                    line = string.Format("####\n{0}\n{1}\n", gv.Name, dim_value);
                    buf += line;
                }
                if(dim_value == 0)
                {
                    zero_value_dim.Add(gv.Name);
                }
            }
            sw.Write(buf);
            buf = "** Parameters **\n";
            line = "";
            var para = from p in mms.Parameters where p.Value.VariableType == ParameterType.Parameter select p;
            OnSaving(0);
            total = para.Count();
            foreach (var pp in para)
            {
                var gv = pp.Value;
               // if(gv.ModuleName)
                skip = false;
                //skip parameters that have zero value dimension
                foreach(var dimn in gv.DimensionNames)
                {
                    if(zero_value_dim.Contains(dimn))
                    {
                        skip = true;
                        break;
                    }
                }
                if (!skip)
                {
                    //gv.DimensionNames
                    if (gv.ValueType == 1)
                    {
                        line = string.Format("####\n{0}     15\n{1}\n{2}\n{3}\n{4}\n{5}\n", gv.Name, gv.Dimension, string.Join("\n",
                            gv.DimensionNames), gv.ValueCount, gv.ValueType, string.Join("\n", (gv as DataCubeParameter<int>).ToVector()));
                    }
                    else if (gv.ValueType == 2)
                    {
                        line = string.Format("####\n{0}     15\n{1}\n{2}\n{3}\n{4}\n{5}\n", gv.Name, gv.Dimension, string.Join("\n",
                        gv.DimensionNames), gv.ValueCount, gv.ValueType, string.Join("\n", (gv as DataCubeParameter<float>).ToVector()));
                    }
                    else if (gv.ValueType == 3)
                    {
                        line = string.Format("####\n{0}     15\n{1}\n{2}\n{3}\n{4}\n{5}\n", gv.Name, gv.Dimension, string.Join("\n",
                         gv.DimensionNames), gv.ValueCount, gv.ValueType, string.Join("\n", (gv as DataCubeParameter<double>).ToVector()));
                    }
                    else if (gv.ValueType == 4)
                    {
                        line = string.Format("####\n{0}     15\n{1}\n{2}\n{3}\n{4}\n{5}\n", gv.Name, gv.Dimension, string.Join("\n",
                         gv.DimensionNames), gv.ValueCount, gv.ValueType, string.Join("\n", (gv as DataCubeParameter<int>).ToVector()));
                    }
                    buf += line;
                    percent = index_dim * 100 / total;
                    OnSaving(percent);
                    index_dim++;
                }
            }
            sw.Write(buf);
            sw.Close();
            if (percent < 100)
                OnSaving(100);
            OnSaved(progress);
        }

        private void Save2Csv(string filename,ICancelProgressHandler progress)
        {
            OnSaving(0);
            int percent = 0;
            string line = "";
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            StreamWriter sw = new StreamWriter(filename);
            string buf = "";
            var mms = (this.Owner as PRMS).MMSPackage;
            int nhru = (this.Owner as PRMS).NHRU;
            var para = (from p in mms.Parameters
                        where p.Value.ValueCount == nhru
                        select p).ToArray();
            var keys = (from p in para select p.Key);
            int nvar = keys.Count();
            var title = string.Join(",", keys);
            var list = (from p in para select p.Value.ToStringVector()).ToArray();
            buf = title + "\n";

            for (int i = 0; i < nhru; i++)
            {
                var vv = from ll in list select ll[i];
                line = string.Join(",", vv);
                buf += line + "\n";
                percent = i * 100 / nhru;
                if (percent % 10 == 0)
                {
                    OnSaving(percent);
                }
            }
            sw.Write(buf);
            sw.Close();
            if (percent < 100)
                OnSaving(100);
            OnSaved(progress);
        }

        public override void Serialize(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(MMSPackage));
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read);

            List<SerializableParameter> list = new List<SerializableParameter>();
            foreach (var pr in Parameters.Values)
            {
                var p = new SerializableParameter()
                {
                    Description = pr.Description,
                    Name = pr.Name,
                    Dimension = pr.Dimension,
                    ValueType = pr.ValueType,
                    VariableType = pr.VariableType,
                    DefaultValue = pr.DefaultValue,
                    DimensionNames = pr.DimensionNames,
                    ModuleName = pr.ModuleName,
                    Maximum= pr.Maximum,
                    Minimum=pr.Minimum,
                    Units=pr.Units
                };
                list.Add(p);
            }
            _DefaultParameters = list.ToArray();
            xs.Serialize(stream, this);
            stream.Close();
        }

        public override void Deserialize(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(MMSPackage));
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            var mms = (MMSPackage)xs.Deserialize(stream);
            this._DefaultParameters = mms.DefaultParameters;
            this.Name = mms.Name;
            this.Description = mms.Description;
            Parameters.Clear();

            foreach (var pr in DefaultParameters)
            {
                if (pr.VariableType == ParameterType.Dimension)
                {
                    var newpr= new DataCubeParameter<int>()
                    {
                        DefaultValue = pr.DefaultValue,
                        Description = pr.Description,
                        ValueType = pr.ValueType,
                        Name = pr.Name,
                        Dimension = pr.Dimension,
                        Owner = this,
                        VariableType = pr.VariableType,
                        ModuleName = pr.ModuleName,
                        DimensionNames = dim_name,
                        Minimum = pr.Minimum,
                        Maximum = pr.Maximum,
                        Units = pr.Units
                    };
                    Parameters.Add(pr.Name, newpr);
                }
                else if (pr.VariableType == ParameterType.Parameter)
                {
                    if (pr.ValueType == 1)
                    {
                        var newpr = new DataCubeParameter<int>()
                        {
                            DefaultValue = pr.DefaultValue,
                            Description = pr.Description,
                            ValueType = pr.ValueType,
                            Name = pr.Name,
                            Dimension = pr.Dimension,
                            Owner = this,
                            VariableType = pr.VariableType,
                            ModuleName = pr.ModuleName,
                            DimensionNames = pr.DimensionNames,
                            Minimum = pr.Minimum,
                            Maximum = pr.Maximum,
                            Units = pr.Units
                        };
                        Parameters.Add(pr.Name, newpr);
                    }
                    else if (pr.ValueType == 2)
                    {
                        var newpr = new DataCubeParameter<float>()
                        {
                            DefaultValue = pr.DefaultValue,
                            Description = pr.Description,
                            ValueType = pr.ValueType,
                            Name = pr.Name,
                            Dimension = pr.Dimension,
                            Owner = this,
                            VariableType = pr.VariableType,
                            ModuleName = pr.ModuleName,
                            DimensionNames = pr.DimensionNames,
                            Minimum = pr.Minimum,
                            Maximum = pr.Maximum,
                            Units = pr.Units
                        };
                        Parameters.Add(pr.Name, newpr);
                    }
                    else if (pr.ValueType == 3)
                    {
                        var newpr = new DataCubeParameter<double>()
                        {
                            DefaultValue = pr.DefaultValue,
                            Description = pr.Description,
                            ValueType = pr.ValueType,
                            Name = pr.Name,
                            Dimension = pr.Dimension,
                            Owner = this,
                            VariableType = pr.VariableType,
                            ModuleName = pr.ModuleName,
                            DimensionNames = pr.DimensionNames,
                            Minimum = pr.Minimum,
                            Maximum = pr.Maximum,
                            Units = pr.Units
                        };
                        Parameters.Add(pr.Name, newpr);
                    }
                    else if (pr.ValueType == 4)
                    {
                        var newpr = new DataCubeParameter<double>()
                        {
                            DefaultValue = pr.DefaultValue,
                            Description = pr.Description,
                            ValueType = pr.ValueType,
                            Name = pr.Name,
                            Dimension = pr.Dimension,
                            Owner = this,
                            VariableType = pr.VariableType,
                            ModuleName = pr.ModuleName,
                            DimensionNames = pr.DimensionNames,
                            Minimum = pr.Minimum,
                            Maximum = pr.Maximum,
                            Units = pr.Units
                        };
                        Parameters.Add(pr.Name, newpr);
                    }
                }
            }
            stream.Close();
        }
        public void LoadParameterMetaFile(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(MMSPackage));
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            var mms = (MMSPackage)xs.Deserialize(stream);
            this._DefaultParameters = mms.DefaultParameters;
            foreach(var para in _DefaultParameters)
            {
                para.Dimension = para.DimensionNames.Length;
            }
            stream.Close();
        }
        public void AlterLength(string dim_name, int new_length)
        {
            var dim_para = Select(dim_name);
            if(dim_para != null)
            {
                dim_para.SetValue(0, 0, 0, new_length);
                foreach (var pr in this.Parameters.Values)
                {
                    if (pr.DimensionNames.Contains(dim_name))
                    {
                        pr.AlterDimLength(dim_name, new_length);
                    }
                }
            }
            IsDirty = true;
        }
        public void UpdatePamameter(IParameter new_para)
        {
            var para = Select(new_para.Name);
            if (para != null)
            {
                para.UpdateFrom(new_para);
                IsDirty = true;
            }
        }

        public override void OnGridUpdated(IGrid sender)
        {
            var grid = sender as RegularGrid;
            var numhru = (sender as IRegularGrid).ActiveCellCount;
            var prms = Owner as PRMS;
            foreach (var dim in _nhru_dim_names)
            {
                var para = Select(dim);
                if (para != null)
                {
                    para.SetValue(0, 0, 0, numhru);
                    (para as IDataCubeObject).Topology = (this.Grid as RegularGrid).Topology;
                    AlterLength(dim, numhru);
                }
            }
            foreach (var dim in _nlayer_dim_names)
            {
                var para = Select(dim);
                if (para != null)
                {
                    para.SetValue(0, 0, 0, prms.SoilLayerManager.LayerCount);
                    AlterLength(dim, prms.SoilLayerManager.Layers.Count);
                }
            }

            ResetToDefault();

            var hru_elev = Select("hru_elev");
            if(hru_elev != null)
            {
                var para = hru_elev as DataCubeParameter<float>;
                for (int i = 0; i < numhru; i++)
                {
                    para[0, i, 0] = grid.Elevations[0, 0, i];
                }
            }

            var hru_slope = Select("hru_slope");
            if(hru_elev != null && grid.Slope != null)
            {
                var para = hru_slope as DataCubeParameter<float>;
                for (int i = 0; i < numhru; i++)
                {
                    para[0, i, 0] = grid.Slope[0, 0, i];
                }
            }
            var hru_aspect = Select("hru_aspect");
            if (hru_aspect != null && grid.Aspect != null)
            {
                var para = hru_aspect as DataCubeParameter<float>;
                for (int i = 0; i < numhru; i++)
                {
                    para[0, i, 0] = grid.Aspect[0, 0, i];
                }
            }

            var soil_depth = Select("soil_depth");
            if (soil_depth != null)
            {
                var para = soil_depth as DataCubeParameter<float>;
                for (int i = 0; i < prms.SoilLayerManager.LayerCount; i++)
                {
                    para[0][":", i] = prms.SoilLayerManager.Layers[i].SoilDepth;
                }
            }
            var cpr_init = Select("cpr_init");
            if (cpr_init != null)
            {
                var para = cpr_init as DataCubeParameter<float>;
                for (int i = 0; i < prms.SoilLayerManager.LayerCount; i++)
                {
                    para[0][":", i] = prms.SoilLayerManager.Layers[i].InitCPR;
                }
            }
            var gvr_init = Select("gvr_init");
            if (gvr_init != null)
            {
                var para = gvr_init as DataCubeParameter<float>;
                for (int i = 0; i < prms.SoilLayerManager.LayerCount; i++)
                {
                    para[0][":", i] = prms.SoilLayerManager.Layers[i].InitGVR;
                }
            }
            var pfr_init = Select("pfr_init");
            if (pfr_init != null)
            {
                var para = pfr_init as DataCubeParameter<float>;
                for (int i = 0; i < prms.SoilLayerManager.LayerCount; i++)
                {
                    para[0][":", i] = prms.SoilLayerManager.Layers[i].InitPFR;
                }
            }

            ModelService.NHRU = numhru;
            var topo = grid.Topology;
            foreach (var para in Parameters)
            {
                if (para.Value.ValueCount == numhru)
                    (para.Value as IDataCubeObject).Topology = topo;
            }

            var ngwcell_para = Select("ngwcell");
            if (ngwcell_para != null)
            {
                var ngwcell = grid.RowCount * grid.ColumnCount;
                ngwcell_para.SetValue(0, 0, 0, ngwcell);
                AlterLength("ngwcell", ngwcell);
            }

            var hru_area = Select("hru_area");
            hru_area.Constant((float)grid.GetCellArea() * ConstantNumber.SqM2Acre);

            var area = grid.GetTotalArea();
            var basin_area = Select("basin_area");
            basin_area.SetValue(0, 0, 0, area * ConstantNumber.SqM2Acre);
            ModelService.BasinArea = area;

            var master = (this.Owner as PRMS).MasterPackage;
            if (master.UseGridClimate)
            {
                master.WriteDefaultClimateMapFile(numhru);
            }

            base.OnGridUpdated(sender);
            IsDirty = true;
        }

        public override void ResetToDefault()
        {
            var buf = 0.0f;
             foreach(var pr in Parameters.Values)
             {
                if (pr.ValueType < 4)
                {
                    buf = float.Parse(pr.GetValue(0, 0, 0).ToString());
                    if(buf == 0)
                        pr.SetToDefault();
                }   
             }
        }
    }
}