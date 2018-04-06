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

using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;

namespace Heiflow.Models.Integration
{
    public enum TemperatureModule { climate_hru = 0, temp_1sta_prms = 1, temp_2sta_prms = 2, xyz_dist = 3 }
    public enum PrecipitationModule { climate_hru = 0, precip_prms, precip_laps_prms, xyz_dist }
    public enum SolarRadiationModule { ccsolrad_hru_prms = 0, ddsolrad_hru_prms }
    public enum PETModule { potet_hamon_prms = 0, potet_jh_prms, potet_pm, climate_hru }
    public enum SurfaceRunoffModule { srunoff_carea_casc = 0, srunoff_smidx_casc }
    public enum ModelMode { GSFLOW,HEIFLOW }


    public class MasterPackage : Package
    {
        DateTime _StartTime;
        DateTime _EndTime;
        int _MaxSoilZoneIter = 15;
        int _ReportDays = 1;
        string _DataFile;
        string _ParameterFile;
        string _ModflowFile;
        string _WaterBudgetCsvFile;
        string _PRMSBudgetFile;
        bool _OutputWaterComponent = true;
        string _WaterComponentFile="null";
        string _WaterBudgetFile = "null";
        string _IrrigationComponentFile = "null";
        string _MFListFile = "null";
        TemperatureModule _Temperature = TemperatureModule.climate_hru;
        PrecipitationModule _Precipitation = PrecipitationModule.climate_hru;
        SolarRadiationModule _SolarRadiation = SolarRadiationModule.ddsolrad_hru_prms;
        PETModule _PotentialET = PETModule.potet_pm;
        SurfaceRunoffModule _SurfaceRunoff = SurfaceRunoffModule.srunoff_smidx_casc;
        bool _StatsON = false;
        string _StatVarFile;
        int _NumStatVars = 0;
        string[] _StatVarNames;
        FileFormat _AniOutFileFormat = FileFormat.Binary;
        string _AniOutFileName;
        string[] _AniOutVarNames;
        int _NumAniOutVar;
        bool _InitVarsFromFile = false;
        string _VarInitFile;
        bool _SaveVarsToFile = false;
        string _VarSaveFile;
        bool _SubbasinFlag = false;
        string _PrecipitationFile;
        string _TempMaxFile;
        string _TempMinFile;
        string _PETFile;
        bool _PrintDebug = false;
        bool _dynamic_para = false;
        int[] _dynamic_day;
        string[] _dynamic_param_file;
        ModelMode _ModelMode = ModelMode.GSFLOW;
        int[] _StatVarElement;
        bool _UseGridClimate = false;
        string _GridClimateFile;
        int _hftimeunit;
        FileFormat _ClimateInputFormat = FileFormat.Text;

        public MasterPackage(string name)
            : base(name)
        {
            Construct();
        }

        public MasterPackage()
        {
            Construct();
        }

        private void Construct()
        {
            _WaterComponentFile = "null";
            _WaterBudgetCsvFile = "null";
            TimeService = new TimeService("Base Timeline");
            IsMandatory = true;
            FullName = "Master Package";
        }

        #region Properties
        [Category("Model")]
        [Description("The simulation mode")]
        [Browsable(false)]
        public ModelMode ModelMode
        {
            get
            {
                return _ModelMode;
            }
            set
            {
                _ModelMode = value;
                (Parameters["model_mode"] as ArrayParam<string>).Values = new string[] { value.ToString() };
                OnPropertyChanged("ModelMode");
            }
        }

        [Category("Time")]
        [Description("The starting time of the simulation")]
        public DateTime StartTime
        {
            get
            {
                return _StartTime;
            }
            set
            {
                _StartTime = value;
                (Parameters["start_time"] as ArrayParam<int>).Values = new int[] { value.Year, value.Month, value.Day, 0, 0, 0 };
                OnPropertyChanged("StartTime");
            }
        }

        [Category("Time")]
        [Description("The ending times of the simulation")]
        public DateTime EndTime
        {
            get
            {
                return _EndTime;
            }
            set
            {
                if (value <= StartTime)
                {
                    value = StartTime.AddYears(1);
                }
                _EndTime = value;
                (Parameters["end_time"] as ArrayParam<int>).Values = new int[] { value.Year, value.Month, value.Day, 0, 0, 0 };
                OnPropertyChanged("EndTime");
            }
        }
        [Category("Time")]
        [Description("The time unit of the model. Daily and hourly time units should be specified as 4 and 5, respectively")]
        public int GlobalTimeUnit
        {
            get
            {
                return _hftimeunit;
            }
            set
            {
                _hftimeunit = value;
                (Parameters["hftimeunit"] as ArrayParam<int>).Values = new int[] { _hftimeunit };
                OnPropertyChanged("GlobalTimeUnit");
            }
        }

        [Category("Model")]
        [Description("")]
        [Browsable(false)]
        public bool SubbasinFlag
        {
            get
            {
                return _SubbasinFlag;
            }
            set
            {
                _SubbasinFlag = value;
                var sf = _SubbasinFlag ? 1 : 0;
                (Parameters["subbasin_flag"] as ArrayParam<int>).Values = new int[] { sf };
                OnPropertyChanged("SubbasinFlag");
            }
        }
        /// <summary>
        /// mxsziter
        /// </summary>
        [Category("Model")]
        [Description("Maximum number of iterations soil-zone flow to MODFLOW finite-difference cells are computed each time step")]
        public int MaxSoilZoneIter
        {
            get
            {
                return _MaxSoilZoneIter;
            }
            set
            {
                if (value < 0)
                    value = 10;
                _MaxSoilZoneIter = 10;
                (Parameters["mxsziter"] as ArrayParam<int>).Values = new int[] { value };
                OnPropertyChanged("MaxSoilZoneIter");
            }
        }

        [Category("Input Files")]
        [Description("Pathname for Data File that contains time-series data for input variables")]
        public string DataFile
        {
            get
            {
                return _DataFile;
            }
            set
            {
                _DataFile = value;
                (Parameters["data_file"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("DataFile");
            }
        }

        [Category("Input Files")]
        [Description("Pathname for Parameter File that Specifies dimensions and parameters required for HEIFLOW  modules formatted according to the Modular Modeling System Parameter File format.")]
        public string ParameterFilePath
        {
            get
            {
                return _ParameterFile;
            }
            set
            {
                _ParameterFile = value;
                (Parameters["param_file"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("ParameterFilePath");
            }
        }

        [Category("Input Files")]
        [Description("Pathname for the Name File, which specifies the names of the input and output files for MODFLOW, associates each file name with a unit number and identifies the packages that will be used in the model.")]
        public string ModflowFilePath
        {
            get
            {
                return _ModflowFile;
            }
            set
            {
                _ModflowFile = value;
                (Parameters["modflow_name"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("ModflowFilePath");
            }
        }

        [Category("Input Files")]
        [Description("Flag to determine if a PRMS Initial Conditions File is specified as an input file")]
        public bool InitVarsFromFile
        {
            get
            {
                return _InitVarsFromFile;
            }
            set
            {
                _InitVarsFromFile = value;
                var infs = _InitVarsFromFile ? 1 : 0;
                (Parameters["init_vars_from_file"] as ArrayParam<int>).Values = new int[] { infs };
                OnPropertyChanged("InitVarsFromFile");
            }
        }
        [Category("Input Files")]
        [Description("Pathname for the PRMS Initial Conditions File")]
        public string VarInitFile
        {
            get
            {
                return _VarInitFile;
            }
            set
            {
                _VarInitFile = value;
                (Parameters["var_init_file"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("VarInitFile");
            }
        }

        [Category("Input Files")]
        [Description("Spatially-distributed precipitation file")]
        public string PrecipitationFile
        {
            get
            {
                return _PrecipitationFile;
            }
            set
            {
                _PrecipitationFile = value;
                (Parameters["precip_day"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("PrecipitationFile");
            }
        }

        [Category("Input Files")]
        [Description("Spatially-distributed maximum temperature file")]
        public string TempMaxFile
        {
            get
            {
                return _TempMaxFile;
            }
            set
            {
                _TempMaxFile = value;
                (Parameters["tmax_day"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("TempMaxFile");
            }
        }

        [Category("Input Files")]
        [Description("Spatially-distributed minimum temperature file")]
        public string TempMinFile
        {
            get
            {
                return _TempMinFile;
            }
            set
            {
                _TempMinFile = value;
                (Parameters["tmin_day"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("TempMinFile");
            }
        }
        [Category("Input Files")]
        [Description("Spatially-distributed  potential ET file")]
        public string PETFile
        {
            get
            {
                return _PETFile;
            }
            set
            {
                _PETFile = value;
                (Parameters["potet_day"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("PETFile");
            }
        }
        [Category("Input Files")]
        [Description("Set climate input file format as follows: Text = 1, Binary = 2")]
        public FileFormat ClimateInputFormat
        {
            get
            {
                return _ClimateInputFormat;
            }
            set
            {
                _ClimateInputFormat = value;
                (Parameters["climate_file_format"] as ArrayParam<int>).Values = new int[] { (int)_ClimateInputFormat };
                OnPropertyChanged("ClimateInputFormat");
            }
        }

        [Category("Input Files")]
        [Description("")]
        public bool UseGridClimate
        {
            get
            {
                return _UseGridClimate;
            }
            set
            {
                _UseGridClimate = value;
                OnPropertyChanged("UseGridClimate");
            }
        }

        [Category("Input Files")]
        [Description("")]
        public string GridClimateFile
        {
            get
            {
                return _GridClimateFile;
            }
            set
            {
                _GridClimateFile = value;
                OnPropertyChanged("GridClimateFile");
            }
        }


        [Category("Summary")]
        [Description("Frequency that summary tables are written to Water-Budget File (0=none, >0 frequency in days, e.g., 1=daily, 7=every 7th day)")]
        public int ReportDays
        {
            get
            {
                return _ReportDays;
            }
            set
            {
                if (value < 0)
                    value = 0;
                _ReportDays = value;
                (Parameters["rpt_days"] as ArrayParam<int>).Values = new int[] { value };
                OnPropertyChanged("ReportDays");
            }
        }

        /// <summary>
        /// gsflow_output_file
        /// </summary>
        [Category("Summary")]
        [Description("Pathname for  Water Budget File of summaries of each component of HEIFLOW water budget")]
        public string WaterBudgetFile
        {
            get
            {
                return _WaterBudgetFile;
            }
            set
            {
                _WaterBudgetFile = value;
                (Parameters["gsflow_output_file"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("WaterBudgetFile");
            }
        }

        /// <summary>
        /// model_output_file
        /// </summary>
        [Category("Summary")]
        [Description("Pathname for PRMS  Water Budget File")]
        [Browsable(false)]
        public string PRMSBudgetFile
        {
            get
            {
                return _PRMSBudgetFile;
            }
            set
            {
                _PRMSBudgetFile = value;
                (Parameters["model_output_file"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("PRMSBudgetFile");
            }
        }

        /// <summary>
        /// gsf_rpt (0=no; 1=yes)
        /// </summary>
        [Category("Summary")]
        [Description("Switch to specify whether or not the  CommaSeparated-Values (CSV) File is generated ")]
        public bool OutputWaterComponent
        {
            get
            {
                return _OutputWaterComponent;
            }
            set
            {
                _OutputWaterComponent = value;
                int gsf_rpt = _OutputWaterComponent ? 1 : 0;
                (Parameters["gsf_rpt"] as ArrayParam<int>).Values = new int[] { gsf_rpt };
                OnPropertyChanged("OutputWaterComponent");
            }
        }

        [Category("Summary")]
        [Description("Path name for  Comma-Separated-Values (CSV) File of  water budget and mass balance results for each time step")]
        [FileMonitorItem("BudgetComponentMonitor")]
        public string WaterComponentFile
        {
            get
            {
                return _WaterComponentFile;
            }
            set
            {
                _WaterComponentFile = value;
                (Parameters["csv_output_file"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("WaterComponentFile");
            }
        }

        [Category("Summary")]
        [Description("Path name for  irrigation budget  for each time step")]
       // [FileMonitorItem("IrrigationMonitor")]
        public string IrrigationComponentFile
        {
            get
            {
                return _IrrigationComponentFile;
            }
            set
            {
                _IrrigationComponentFile = value;
                (Parameters["irrigation_output_file"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("IrrigationComponentFile");
            }
        }

        [Category("Summary")]
        [Description("Path name for Modflow List file ")]
        [FileMonitorItem("MFMonitor")]
        public string MFListFile
        {
            get
            {
                return _MFListFile;
            }
            set
            {
                _MFListFile = value;
                (Parameters["mflist_file"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("MFListFile");
            }
        }

        [Category("Summary")]
        [Description("Path name for  Comma-Separated-Values (CSV) File of  water budget")]
        [FileMonitorItem("BasinBudgetMonitor")]
        public string WaterBudgetCsvFile
        {
            get
            {
                return _WaterBudgetCsvFile;
            }
            set
            {
                _WaterBudgetCsvFile = value;
                (Parameters["csv_model_output_file"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("WaterBudgetCsvFile");
            }
        }

        /// <summary>
        /// temp_module
        /// </summary>
        [Category("Modules")]
        [Description("Module name for Temperature distribution method")]
        public TemperatureModule Temperature
        {
            get
            {
                return _Temperature;
            }
            set
            {
                _Temperature = value;
                (Parameters["temp_module"] as ArrayParam<string>).Values = new string[] { value.ToString() };
                OnPropertyChanged("Temperature");
            }
        }

        /// <summary>
        /// temp_module
        /// </summary>
        [Category("Modules")]
        [Description("Module name for Precipitation distribution method")]
        public PrecipitationModule Precipitation
        {
            get
            {
                return _Precipitation;
            }
            set
            {
                _Precipitation = value;
                (Parameters["precip_module"] as ArrayParam<string>).Values = new string[] { value.ToString() };
                OnPropertyChanged("Precipitation");
            }
        }
        /// <summary>
        /// solrad_module
        /// </summary>
        [Category("Modules")]
        [Description("Module name for Solar Radiation distribution method")]
        public SolarRadiationModule SolarRadiation
        {
            get
            {
                return _SolarRadiation;
            }
            set
            {
                _SolarRadiation = value;
                (Parameters["solrad_module"] as ArrayParam<string>).Values = new string[] { value.ToString() };
                OnPropertyChanged("SolarRadiation");
            }
        }

        /// <summary>
        /// et_module
        /// </summary>
        [Category("Modules")]
        [Description("Module name for potential-evapotranspiration computation method ")]
        public PETModule PotentialET
        {
            get
            {
                return _PotentialET;
            }
            set
            {
                _PotentialET = value;
                (Parameters["et_module"] as ArrayParam<string>).Values = new string[] { value.ToString() };
                OnPropertyChanged("PotentialET");
            }
        }
        /// <summary>
        /// srunoff_module
        /// </summary>
        [Category("Modules")]
        [Description("Module name for  surface-runoff/infiltration computation method ")]
        public SurfaceRunoffModule SurfaceRunoff
        {
            get
            {
                return _SurfaceRunoff;
            }
            set
            {
                _SurfaceRunoff = value;
                (Parameters["srunoff_module"] as ArrayParam<string>).Values = new string[] { value.ToString() };
                OnPropertyChanged("SurfaceRunoff");
            }
        }

        [Category("Statistics")]
        [Description("Switch to specify whether or not PRMS Statistic Variables (statvar) File of selected time-series values is generated")]
        public bool StatsON
        {
            get
            {
                return _StatsON;
            }
            set
            {
                _StatsON = value;
                var stat = _StatsON ? 1 : 0;
                (Parameters["statsON_OFF"] as ArrayParam<int>).Values = new int[] { stat };
                OnPropertyChanged("StatsON");
            }
        }


        [Category("Statistics")]
        [Description("Pathname for PRMS Statistic Variables (statvar) File of time-series values; required only when StatsON is true")]
        public string StatVarFile
        {
            get
            {
                return _StatVarFile;
            }
            set
            {
                _StatVarFile = value;
                (Parameters["stat_var_file"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("StatVarFile");
            }
        }
        [Category("Statistics")]
        [Description("Number of variables to include in PRMS Statistic Variables File and names specified in StatVarNames")]
        [Browsable(false)]
        public int NumStatVars
        {
            get
            {
                return _NumStatVars;
            }
            set
            {
                _NumStatVars = value;
                (Parameters["nstatVars"] as ArrayParam<int>).Values = new int[] { value };
                OnPropertyChanged("NumStatVars");
            }
        }
        [Category("Statistics")]
        [Description("List of variable names for which output is written to PRMS Statistic Variables File")]
        public string[] StatVarNames
        {
            get
            {
                return _StatVarNames;
            }
            set
            {
                _StatVarNames = value;
                if (value != null)
                    NumStatVars = _StatVarNames.Length;
                (Parameters["statVar_names"] as ArrayParam<string>).Values = value;
                OnPropertyChanged("StatVarNames");
            }
        }

        [Category("Statistics")]
        [Description("List of identification numbers corresponding to variables specified in statVar_names file ")]
        public int[] StatVarElement
        {
            get
            {
                return _StatVarElement;
            }
            set
            {
                _StatVarElement = value;
                (Parameters["statVar_element"] as ArrayParam<int>).Values = value;
                OnPropertyChanged("StatVarElement");
            }
        }


        [Category("Output Files")]
        [Description("Switch to specify whether or not PRMS Animation Variables File(s) of spatially-distributed values is generated")]
        public FileFormat AniOutFileFormat
        {
            get
            {
                return _AniOutFileFormat;
            }
            set
            {
                _AniOutFileFormat = value;
                (Parameters["aniOutON_OFF"] as ArrayParam<int>).Values = new int[] { (int)value };
                OnPropertyChanged("AniOutFileFormat");
            }
        }
        [Category("Output Files")]
        [Description("pathname for PRMS Animation Variables File(s) to which a filename suffix based on dimension names associated with selected variables is appended;")]
        public string AniOutFileName
        {
            get
            {
                return _AniOutFileName;
            }
            set
            {
                _AniOutFileName = value;
                (Parameters["ani_output_file"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("AniOutFileName");
            }
        }

        [Category("Output Files")]
        [Description("Number of output variables specified in the AniOutVarNames list")]
        [Browsable(false)]
        public int NumAniOutVar
        {
            get
            {
                return _NumAniOutVar;
            }
            set
            {
                _NumAniOutVar = value;
                (Parameters["naniOutVars"] as ArrayParam<int>).Values = new int[] { value };
                OnPropertyChanged("NumAniOutVar");
            }
        }

        [Category("Output Files")]
        [Description("List of variable names for which all values of the variable (that is, the entire dimension size) for each time step are written to PRMS Animation Variables File(s)")]
        public string[] AniOutVarNames
        {
            get
            {
                return _AniOutVarNames;
            }
            set
            {
                _AniOutVarNames = value;
                if (value != null)
                    NumAniOutVar = AniOutVarNames.Length;
                (Parameters["aniOutVar_names"] as ArrayParam<string>).Values = value;
                OnPropertyChanged("AniOutVarNames");
            }
        }
        [Category("Output Files")]
        [Description("Flag to determine if a PRMS Initial Conditions File (var_save_file) will be generated at the end of simulation")]
        public bool SaveVarsToFile
        {
            get
            {
                return _SaveVarsToFile;
            }
            set
            {
                _SaveVarsToFile = value;
                var infs = _SaveVarsToFile ? 1 : 0;
                (Parameters["save_vars_to_file"] as ArrayParam<int>).Values = new int[] { infs };
                OnPropertyChanged("SaveVarsToFile");
            }
        }
        [Category("Output Files")]
        [Description("Pathname for the PRMS Initial Conditions File to be generated at end of simulation")]
        public string VarSaveFile
        {
            get
            {
                return _VarSaveFile;
            }
            set
            {
                _VarSaveFile = value;
                (Parameters["var_save_file"] as ArrayParam<string>).Values = new string[] { value };
                OnPropertyChanged("VarSaveFile");
            }
        }

        [Category("Debug")]
        [Description("Print Debug file")]
        public bool PrintDebug
        {
            get
            {
                return _PrintDebug;
            }
            set
            {
                _PrintDebug = value;
                var infs = _PrintDebug ? 1 : 0;
                (Parameters["print_debug"] as ArrayParam<int>).Values = new int[] { infs };
                OnPropertyChanged("PrintDebug");
            }
        }
        [Category("Time-Varient")]
        [Description("Enable time-varient parameters")]
        public bool DynamicPara
        {
            get
            {
                return _dynamic_para;
            }
            set
            {
                _dynamic_para = value;
                var infs = _dynamic_para ? 1 : 0;
                (Parameters["dynamic_para"] as ArrayParam<int>).Values = new int[] { infs };
                OnPropertyChanged("DynamicPara");
            }
        }

        [Category("Time-Varient")]
        [Description("The days when the parameter file will be changed")]
        public int[] DynamicDays
        {
            get
            {
                return _dynamic_day;
            }
            set
            {
                _dynamic_day = value;
                (Parameters["dynamic_day"] as ArrayParam<int>).Values = value;
                OnPropertyChanged("DynamicDays");
            }
        }

        [Category("Time-Varient")]
        [Description("The list of parameter files")]
        public string[] DynamicParamFiles
        {
            get
            {
                return _dynamic_param_file;
            }
            set
            {
                _dynamic_param_file = value;
                (Parameters["dynamic_param_file"] as ArrayParam<string>).Values = _dynamic_param_file;
                OnPropertyChanged("DynamicParamFiles");
            }
        }
        #endregion

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            base.Initialize();
        }

        /// <summary>
        /// Load from an  exsiting file
        /// </summary>
        /// <returns></returns>
        public override bool Load()
        {
            if (File.Exists(FileName))
            {
                LoadFrom(FileName);
                var start = (Parameters["start_time"] as ArrayParam<int>).Values;
                _StartTime = new DateTime(start[0], start[1], start[2], start[3], start[4], start[5]);
                var end = (Parameters["end_time"] as ArrayParam<int>).Values;
                _EndTime = new DateTime(end[0], end[1], end[2], end[3], end[4], end[5]);
                _DataFile = (Parameters["data_file"] as ArrayParam<string>).Values[0];
                _ParameterFile = (Parameters["param_file"] as ArrayParam<string>).Values[0];
                _ModflowFile = (Parameters["modflow_name"] as ArrayParam<string>).Values[0];
                _ReportDays = (Parameters["rpt_days"] as ArrayParam<int>).Values[0];
                _WaterBudgetFile = (Parameters["gsflow_output_file"] as ArrayParam<string>).Values[0];
                _PRMSBudgetFile = (Parameters["model_output_file"] as ArrayParam<string>).Values[0];
                _OutputWaterComponent = (Parameters["gsf_rpt"] as ArrayParam<int>).Values[0] == 1 ? true : false;
                _WaterComponentFile = (Parameters["csv_output_file"] as ArrayParam<string>).Values[0];
                _Temperature = EnumHelper.FromString<TemperatureModule>((Parameters["temp_module"] as ArrayParam<string>).Values[0]);
                _Precipitation = EnumHelper.FromString<PrecipitationModule>((Parameters["precip_module"] as ArrayParam<string>).Values[0]);
                _SolarRadiation = EnumHelper.FromString<SolarRadiationModule>((Parameters["solrad_module"] as ArrayParam<string>).Values[0]);
                if (Parameters.ContainsKey("et_module"))
                    _PotentialET = EnumHelper.FromString<PETModule>((Parameters["et_module"] as ArrayParam<string>).Values[0]);
                _SurfaceRunoff = EnumHelper.FromString<SurfaceRunoffModule>((Parameters["srunoff_module"] as ArrayParam<string>).Values[0]);
                _StatsON = (Parameters["statsON_OFF"] as ArrayParam<int>).Values[0] == 1 ? true : false;
                _StatVarFile = (Parameters["stat_var_file"] as ArrayParam<string>).Values[0];
                _NumStatVars = (Parameters["nstatVars"] as ArrayParam<int>).Values[0];
                if (Parameters.ContainsKey("_StatVarElement"))
                    _StatVarElement = (Parameters["statVar_element"] as ArrayParam<int>).Values;
                _StatVarNames = (Parameters["statVar_names"] as ArrayParam<string>).Values;
                _AniOutFileFormat = (FileFormat)(Parameters["aniOutON_OFF"] as ArrayParam<int>).Values[0];
                _AniOutFileName = (Parameters["ani_output_file"] as ArrayParam<string>).Values[0] + ".nhru";
                _NumAniOutVar = (Parameters["naniOutVars"] as ArrayParam<int>).Values[0];
                _AniOutVarNames = (Parameters["aniOutVar_names"] as ArrayParam<string>).Values;
                _InitVarsFromFile = (Parameters["init_vars_from_file"] as ArrayParam<int>).Values[0] == 1 ? true : false;

                if ((Parameters.Keys.Contains("csv_model_output_file")))
                    _WaterBudgetCsvFile = (Parameters["csv_model_output_file"] as ArrayParam<string>).Values[0];
                if ((Parameters.Keys.Contains("irrigation_output_file")))
                    _IrrigationComponentFile = (Parameters["irrigation_output_file"] as ArrayParam<string>).Values[0];
                if (Parameters.ContainsKey("mflist_file"))
                    _MFListFile = (Parameters["mflist_file"] as ArrayParam<string>).Values[0];
                if ((Parameters.Keys.Contains("use_gridclimate")))
                    _UseGridClimate = (Parameters["use_gridclimate"] as ArrayParam<int>).Values[0] == 1 ? true : false;
                if ((Parameters.Keys.Contains("gridclimate_file")))
                    _GridClimateFile = (Parameters["gridclimate_file"] as ArrayParam<string>).Values[0];
                if ((Parameters.Keys.Contains("var_init_file")))
                    _VarInitFile = (Parameters["var_init_file"] as ArrayParam<string>).Values[0];
                if ((Parameters.Keys.Contains("save_vars_to_file")))
                    _SaveVarsToFile = (Parameters["save_vars_to_file"] as ArrayParam<int>).Values[0] == 1 ? true : false;
                if ((Parameters.Keys.Contains("var_save_file")))
                    _VarSaveFile = (Parameters["var_save_file"] as ArrayParam<string>).Values[0];
                _PrintDebug = (Parameters["print_debug"] as ArrayParam<int>).Values[0] == 1 ? true : false;
                if ((Parameters.Keys.Contains("precip_day")))
                    _PrecipitationFile = (Parameters["precip_day"] as ArrayParam<string>).Values[0];
                if ((Parameters.Keys.Contains("tmax_day")))
                    _TempMaxFile = (Parameters["tmax_day"] as ArrayParam<string>).Values[0];
                if ((Parameters.Keys.Contains("tmin_day")))
                    _TempMinFile = (Parameters["tmin_day"] as ArrayParam<string>).Values[0];
                if ((Parameters.Keys.Contains("potet_day")))
                    _PETFile = (Parameters["potet_day"] as ArrayParam<string>).Values[0];
                if (Parameters.ContainsKey("subbasin_flag"))
                    _SubbasinFlag = (Parameters["subbasin_flag"] as ArrayParam<int>).Values[0] == 1 ? true : false;
                if (Parameters.ContainsKey("hftimeunit"))
                    _hftimeunit = (Parameters["hftimeunit"] as ArrayParam<int>).Values[0];
                if (Parameters.ContainsKey("climate_file_format"))
                    _ClimateInputFormat = EnumHelper.FromString<FileFormat>((Parameters["climate_file_format"] as ArrayParam<int>).Values[0].ToString());
                if (Parameters.ContainsKey("dynamic_para"))
                    _dynamic_para = (Parameters["dynamic_para"] as ArrayParam<int>).Values[0] == 1 ? true : false;
                else
                    _dynamic_para = false;
                if (Parameters.ContainsKey("dynamic_day"))
                    _dynamic_day = (Parameters["dynamic_day"] as ArrayParam<int>).Values;
                if (Parameters.ContainsKey("dynamic_param_file"))
                    _dynamic_param_file = (Parameters["dynamic_param_file"] as ArrayParam<string>).Values;

                foreach (var pr in Parameters.Values)
                    pr.Owner = this;

                UpdateTimeService();
                OnLoaded("Load successfully");
                return true;
            }
            else
            {
                Message = string.Format("{0} does not exist. Please check the model file.", FileName);
                OnLoadFailed(Message);
                return false;
            }
        }
        public override bool SaveAs(string filename, IProgress prg)
        {
            StreamWriter sw = new StreamWriter(filename);
            Save(sw);
            sw.Close();
            OnSaved(prg);
            return true;
        }
        public override bool New()
        {
            string _Controlfile = Path.Combine(BaseModel.ConfigPath, "heiflow.control");
        
            if (File.Exists(_Controlfile) )
            {
                string xmlcopy = this.FileName.Replace(".control", ".xml");
                File.Copy(_Controlfile, this.FileName, true);
           
                LoadFrom(this.FileName);
                StartTime = new DateTime(DateTime.Now.Year - 1, 1, 1);
                EndTime = new DateTime(DateTime.Now.Year, 1, 1);

                SubbasinFlag = false;
                OutputWaterComponent = true;
                UseGridClimate = false;
                ReportDays = 1;
                SaveVarsToFile = false;
                PrintDebug = false;
                ClimateInputFormat = FileFormat.Text;

                DataFile = string.Format(".\\input\\prms\\{0}.data", Owner.Name);
                ParameterFilePath = string.Format(".\\input\\prms\\{0}.param", Owner.Name);
                ModflowFilePath = string.Format(".\\input\\modflow\\{0}.nam", Owner.Name);
                GridClimateFile = string.Format(".\\input\\prms\\{0}.hru_clm", Owner.Name);
                VarInitFile = string.Format(".\\input\\prms\\{0}_prms_ic.in", Owner.Name);
                PrecipitationFile = string.Format(".\\input\\prms\\{0}_precip.txt", Owner.Name);
                TempMaxFile = string.Format(".\\input\\prms\\{0}_tmax.txt", Owner.Name);
                TempMinFile = string.Format(".\\input\\prms\\{0}_tmin.txt", Owner.Name);
                PETFile = string.Format(".\\input\\prms\\{0}_pet.txt", Owner.Name);

                WaterBudgetFile = string.Format(".\\output\\{0}_total_budget.out", Owner.Name);
                PRMSBudgetFile = string.Format(".\\output\\{0}_prms_budget.out", Owner.Name);
                WaterBudgetCsvFile = string.Format(".\\output\\{0}_accu_budget.csv", Owner.Name);
                WaterComponentFile = string.Format(".\\output\\{0}_budget_component.csv", Owner.Name);
                StatVarFile = string.Format(".\\output\\{0}_statvar.txt", Owner.Name);
                AniOutFileName = string.Format(".\\output\\{0}_animation.out", Owner.Name);
                VarSaveFile = string.Format(".\\output\\{0}_prms_ic.out", Owner.Name);
                IrrigationComponentFile = string.Format(".\\output\\{0}_irrigation.txt", Owner.Name);
                MFListFile = string.Format(".\\output\\{0}.lst", Owner.Name);

                DynamicDays = new int[] { 2 };
                DynamicPara = false;
                DynamicParamFiles = new string[] { _ParameterFile };
                
                foreach (var pr in Parameters.Values)
                    pr.Owner = this;
                UpdateTimeService();
                base.New();
                NewXml();
                State = ModelObjectState.Ready;
                return true;
            }
            else
            {
                Message = string.Format("{0} does not exist. Please repair the software.", _Controlfile);
                IsDirty = false;
                return false;
            }
        }
      

        public override void Clear()
        {
            if(_Initialized)
                TimeService.Updated -= OnTimeServiceUpdated;
            base.Clear();
        }
        public override void UpdateTimeService()
        {
            TimeService.Start = _StartTime;
            TimeService.End = _EndTime;
            TimeService.Timeline.Clear();
            if (Parameters.ContainsKey("hftimeunit"))
                TimeService.TimeUnit = (Parameters["hftimeunit"] as ArrayParam<int>).Values[0];
            else
                TimeService.TimeUnit = 4;
            TimeService.UpdateTimeLine();
            TimeService.IOTimeline = TimeService.Timeline;
            TimeService.UseStressPeriods = _dynamic_para;
            if (TimeService.UseStressPeriods)
            {
                int nsp = _dynamic_day.Length + 1;
                int[] sp_lens = new int[nsp];

                int[] buf = new int[nsp + 1];
                buf[0] = 1;
                buf[nsp] = TimeService.NumTimeStep;
                for (int i = 0; i < nsp - 1; i++)
                {
                    buf[i + 1] = _dynamic_day[i];
                }
                for (int i = 0; i < nsp; i++)
                {
                    sp_lens[i] = buf[i + 1] - buf[i];
                }
                TimeService.InitSP(sp_lens, false);
                TimeService.StressPeriods[0].ParameterFile = _ParameterFile;
                for (int i = 1; i < nsp; i++)
                {
                    TimeService.StressPeriods[i].ParameterFile = _dynamic_param_file[i - 1];
                }
            }
        }

        private void LoadFrom(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            string newline = "";
            bool nextIsVar = false;
            while (!sr.EndOfStream)
            {
                newline = sr.ReadLine().Trim();
                if (newline.StartsWith("#"))
                {
                    nextIsVar = true;
                }
                else
                {
                    nextIsVar = false;
                }
                if (nextIsVar)
                {
                    newline = sr.ReadLine().Trim();
                    string name = newline;
                    newline = sr.ReadLine().Trim();
                    int ValueCount = int.Parse(newline);
                    newline = sr.ReadLine().Trim();
                    int ValueType = int.Parse(newline);
                    var Dimension = 1;

                    var value_str = "";
                    for (int i = 0; i < ValueCount; i++)
                    {
                        value_str += sr.ReadLine() + " ";
                    }

                    if (ValueType == 1)
                    {
                        ArrayParam<int> gv = new ArrayParam<int>(name)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Control,
                            Dimension = Dimension,
                            Owner = this
                        };
                        gv.Values = TypeConverterEx.Split<int>(value_str);
                        if (!Parameters.Keys.Contains(gv.Name))
                            Parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 2)
                    {
                        ArrayParam<float> gv = new ArrayParam<float>(name)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Control,
                            Dimension = Dimension,
                            Owner = this
                        };
                        gv.Values = TypeConverterEx.Split<float>(value_str);
                        if (!Parameters.Keys.Contains(gv.Name))
                            Parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 3)
                    {
                        ArrayParam<double> gv = new ArrayParam<double>(name)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Control,
                            Dimension = Dimension,
                            Owner = this
                        };
                        gv.Values = TypeConverterEx.Split<double>(value_str);
                        if (!Parameters.Keys.Contains(gv.Name))
                            Parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 4)
                    {
                        ArrayParam<string> gv = new ArrayParam<string>(name)
                        {
                            ValueType = ValueType,

                            VariableType = ParameterType.Control,
                            Dimension = Dimension,
                            Owner = this
                        };
                        gv.Values = TypeConverterEx.Split<string>(value_str);
                        if (!Parameters.Keys.Contains(gv.Name))
                            Parameters.Add(gv.Name, gv);
                    }
                }
            }
            sr.Close();
        }

        private void Save(StreamWriter sw)
        {
            sw.WriteLine("Control file generated by Visual HEIFLOW at " + DateTime.Now);
            foreach (var para in Parameters)
            {
                string line = "####";
                sw.WriteLine(line);
                sw.WriteLine(para.Key);
                var pp = para.Value;
                if (pp.ValueType == 1)
                {
                    var vv = (pp as ArrayParam<int>).Values;
                    sw.WriteLine(vv.Length);
                    sw.WriteLine(pp.ValueType);
                    sw.WriteLine(string.Join("\n", vv));
                }
                else if (pp.ValueType == 2)
                {
                    var vv = (pp as ArrayParam<float>).Values;
                    sw.WriteLine(vv.Length);
                    sw.WriteLine(pp.ValueType);
                    sw.WriteLine(string.Join("\n", vv));
                }
                else if (pp.ValueType == 3)
                {
                    var vv = (pp as ArrayParam<double>).Values;
                    sw.WriteLine(vv.Length);
                    sw.WriteLine(pp.ValueType);
                    sw.WriteLine(string.Join("\n", vv));
                }
                else if (pp.ValueType == 4)
                {
                    var vv = (pp as ArrayParam<string>).Values;
                    sw.WriteLine(vv.Length);
                    sw.WriteLine(pp.ValueType);
                    sw.WriteLine(string.Join("\n", vv));
                }
            }
        }

        private void NewXml()
        {
            string xmlfile = Path.Combine(BaseModel.ConfigPath, "heiflow.xml");
            if (File.Exists(xmlfile))
            {
                string xmlcopy = this.FileName.Replace(".control", ".xml");
                File.Copy(xmlfile, xmlcopy, true);
                XmlDocument xml = new XmlDocument();
                xml.Load(xmlcopy);

                XmlElement pet = xml.SelectSingleNode("HeiflowProject/Modules/ModPenmanMonteith") as XmlElement;
                pet.SetAttribute("File", PETFile);
                XmlElement bd = xml.SelectSingleNode("HeiflowProject/Extensions/BudgetsSumEx") as XmlElement;
                bd.SetAttribute("File", WaterBudgetCsvFile);

                xml.Save(xmlcopy);
            }
        }

        public override void OnTimeServiceUpdated(ITimeService time)
        {
            StartTime=TimeService.Start ;
            EndTime = TimeService.End;
        }
    }
}
