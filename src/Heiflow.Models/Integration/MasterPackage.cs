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
using Heiflow.Models.Generic.Parameters;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace Heiflow.Models.Integration
{
    public enum TemperatureModule { climate_hru = 0, temp_1sta_prms = 1, temp_2sta_prms = 2, xyz_dist = 3 }
    public enum PrecipitationModule { climate_hru = 0, precip_prms, precip_laps_prms, xyz_dist }
    public enum WindModule { climate_hru = 0}
    public enum HumidityModule { climate_hru = 0 }
    public enum PressureModule { climate_hru = 0 }
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
        private string _PrecipitationFile="ppt.txt";
        private string _TempMaxFile="tmax.txt";
        private string _TempMinFile = "tmin.txt";
        private string _PETFile="pet.txt";
        private string _WindFile="wind.txt";
        private string _HumidityFile = "hum.txt";
        private string _PressureFile ="pressure.txt";
        bool _PrintDebug = false;
        bool _dynamic_para = false;
        int[] _dynamic_day;
        string[] _dynamic_param_file;
        ModelMode _ModelMode = ModelMode.GSFLOW;
        int[] _StatVarElement;
        bool _UseGridClimate = false;
        string _GridClimateFile;
        int _GlobalTimeUnit = 4;
        FileFormat _ClimateInputFormat = FileFormat.Text;
        private bool _AnimationOutOC = false;
        private string  _AnimationOutOCFile;
        private string _wra_module = "none";
        private string _wra_module_file;
        private string _hydraulics_engine = "SFR";
        private string _extension_man_file;
        private WindModule _WindModule;
        private HumidityModule _HumidityModule;
        private PressureModule _PressureModule;
        private bool _SaveSoilwaterFile;
        private string _SoilWaterFile;
        private string _SoilWaterBudgetFile;

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
            TimeService.Updated += OnTimeServiceUpdated;
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
                (Parameters["model_mode"] as DataCubeParameter<string>)[0, 0, 0] = _ModelMode.ToString();
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
                (Parameters["start_time"] as DataCubeParameter<int>)[0][":",0] = new int[] { value.Year, value.Month, value.Day, 0, 0, 0 };
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
                (Parameters["end_time"] as DataCubeParameter<int>)[0][":", 0] = new int[] { value.Year, value.Month, value.Day, 0, 0, 0 };
                OnPropertyChanged("EndTime");
            }
        }
        [Category("Time")]
        [Description("The time unit of the model. Daily or hourly time units should be specified as 4 or 5, respectively")]
        public int GlobalTimeUnit
        {
            get
            {
                return _GlobalTimeUnit;
            }
            set
            {
                _GlobalTimeUnit = value;
                (Parameters["global_time_unit"] as DataCubeParameter<int>)[0, 0, 0] = _GlobalTimeUnit;
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
                (Parameters["subbasin_flag"] as DataCubeParameter<int>)[0, 0, 0] = sf;
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
                (Parameters["mxsziter"] as DataCubeParameter<int>)[0, 0, 0] = value;
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
                (Parameters["data_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                (Parameters["param_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                (Parameters["modflow_name"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                (Parameters["init_vars_from_file"] as DataCubeParameter<int>)[0, 0, 0] = infs;
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
                (Parameters["var_init_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                (Parameters["precip_day"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                (Parameters["tmax_day"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                (Parameters["tmin_day"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                if (Parameters.Keys.Contains("potet_day"))
                {
                    (Parameters["potet_day"] as DataCubeParameter<string>)[0, 0, 0] = value;
                    OnPropertyChanged("PETFile");
                }
            }
        }
        [Category("Input Files")]
        [Description("Spatially-distributed wind file")]
        public string WindFile
        {
            get
            {
                return _WindFile;
            }
            set
            {
                _WindFile = value;
                if (Parameters.Keys.Contains("wnd_day"))
                {
                    (Parameters["wnd_day"] as DataCubeParameter<string>)[0, 0, 0] = value;
                    OnPropertyChanged("WindFile");
                }
            }
        }
        [Category("Input Files")]
        [Description("Spatially-distributed humidity file")]
        public string HumidityFile
        {
            get
            {
                return _HumidityFile;
            }
            set
            {
                _HumidityFile = value;
                if (Parameters.Keys.Contains("hum_day"))
                {
                    (Parameters["hum_day"] as DataCubeParameter<string>)[0, 0, 0] = value;
                    OnPropertyChanged("HumidityFile");
                }
            }
        }
        [Category("Input Files")]
        [Description("Spatially-distributed pressure file")]
        public string PressureFile
        {
            get
            {
                return _PressureFile;
            }
            set
            {
                _PressureFile = value;
                if (Parameters.Keys.Contains("press_day"))
                {
                    (Parameters["press_day"] as DataCubeParameter<string>)[0, 0, 0] = value;
                    OnPropertyChanged("PressureFile");
                }
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
                (Parameters["climate_file_format"] as DataCubeParameter<int>)[0, 0, 0] = (int)_ClimateInputFormat;
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
                var infs = _UseGridClimate ? 1 : 0;
                (Parameters["sub_climate_flag"] as DataCubeParameter<int>)[0, 0, 0] = infs;
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
                (Parameters["sub_climate_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                (Parameters["rpt_days"] as DataCubeParameter<int>)[0, 0, 0] = value;
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
                (Parameters["gsflow_output_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                (Parameters["model_output_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                (Parameters["gsf_rpt"] as DataCubeParameter<int>)[0, 0, 0] = gsf_rpt;
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
                (Parameters["csv_output_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
                OnPropertyChanged("WaterComponentFile");
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
                (Parameters["mflist_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                (Parameters["accu_budget_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                (Parameters["temp_module"] as DataCubeParameter<string>)[0, 0, 0] = value.ToString();
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
                (Parameters["precip_module"] as DataCubeParameter<string>)[0, 0, 0] = value.ToString();
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
                (Parameters["solrad_module"] as DataCubeParameter<string>)[0, 0, 0] = value.ToString();
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
                (Parameters["et_module"] as DataCubeParameter<string>)[0, 0, 0] = value.ToString();
                OnPropertyChanged("PotentialET");
            }
        }
        /// <summary>
        /// wind module
        /// </summary>
        [Category("Modules")]
        [Description("Module name for wind speed computation method ")]
        public WindModule Wind
        {
            get
            {
                return _WindModule;
            }
            set
            {
                _WindModule = value;
                (Parameters["wnd_module"] as DataCubeParameter<string>)[0, 0, 0] = value.ToString();
                OnPropertyChanged("Wind");
            }
        }
        /// <summary>
        /// wind module
        /// </summary>
        [Category("Modules")]
        [Description("Module name for humidity computation method ")]
        public HumidityModule Humidity
        {
            get
            {
                return _HumidityModule;
            }
            set
            {
                _HumidityModule = value;
                (Parameters["hum_module"] as DataCubeParameter<string>)[0, 0, 0] = value.ToString();
                OnPropertyChanged("Humidity");
            }
        }
        /// <summary>
        /// wind module
        /// </summary>
        [Category("Modules")]
        [Description("Module name for pressure computation method ")]
        public PressureModule Pressure
        {
            get
            {
                return _PressureModule;
            }
            set
            {
                _PressureModule = value;
                (Parameters["press_module"] as DataCubeParameter<string>)[0, 0, 0] = value.ToString();
                OnPropertyChanged("Pressure");
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
                (Parameters["srunoff_module"] as DataCubeParameter<string>)[0, 0, 0] = value.ToString();
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
                (Parameters["statsON_OFF"] as DataCubeParameter<int>)[0, 0, 0] = stat;
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
                (Parameters["stat_var_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                (Parameters["nstatVars"] as DataCubeParameter<int>)[0, 0, 0] = value;
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
                (Parameters["statVar_names"] as DataCubeParameter<string>)[0][":", 0] = value;
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
                (Parameters["statVar_element"] as DataCubeParameter<int>)[0][":", 0] = value;
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
                (Parameters["aniOutON_OFF"] as DataCubeParameter<int>)[0, 0, 0] = (int)value;
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
                (Parameters["ani_output_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
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
                (Parameters["naniOutVars"] as DataCubeParameter<int>)[0, 0, 0] = value;
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
                    NumAniOutVar = _AniOutVarNames.Length;
                var para = Parameters["aniOutVar_names"] as DataCubeParameter<string>;
                if (para.Size[2] != NumAniOutVar)
                {
                    para.ReSize(1, NumAniOutVar, 1);
                }
                for (int i = 0; i < para.Size[1]; i++)
                {
                    para[0, i, 0] = _AniOutVarNames[i];
                }
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
                (Parameters["save_vars_to_file"] as DataCubeParameter<int>)[0, 0, 0] = infs;
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
                (Parameters["var_save_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
                OnPropertyChanged("VarSaveFile");
            }
        }

        [Category("Output Files")]
        [Description("Flag to determine soil water file saved")]
        public bool SaveSoilWaterFile
        {
            get
            {
                return _SaveSoilwaterFile;
            }
            set
            {
                _SaveSoilwaterFile = value;
                var infs = _SaveSoilwaterFile ? 1 : 0;
                if(Parameters.Keys.Contains("save_soilwater_hru"))
             {
                    (Parameters["save_soilwater_hru"] as DataCubeParameter<int>)[0, 0, 0] = infs;
                    OnPropertyChanged("SaveSoilwaterFile");
                }
            }
        }
        [Category("Output Files")]
        [Description("file name used to save soil water")]
        public string SoilWaterFile
        {
            get
            {
                return _SoilWaterFile;
            }
            set
            {
                _SoilWaterFile = value;
                (Parameters["file_soilwater_hru"] as DataCubeParameter<string>)[0, 0, 0] = _SoilWaterFile;
                OnPropertyChanged("SoilWaterFile");
            }
        }
        [Category("Output Files")]
        [Description("file name used to save soil water budget")]
        public string SoilWaterBudgetFile
        {
            get 
            {
                return _SoilWaterBudgetFile;
            }
            set
            {
                _SoilWaterBudgetFile = value;
                (Parameters["file_soilwater_budget"] as DataCubeParameter<string>)[0, 0, 0] = _SoilWaterBudgetFile;
                OnPropertyChanged("SoilWaterBudgetFile");
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
                (Parameters["print_debug"] as DataCubeParameter<int>)[0, 0, 0] = infs;
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
                (Parameters["dynamic_para"] as DataCubeParameter<int>)[0, 0, 0] = infs;
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
                (Parameters["dynamic_day"] as DataCubeParameter<int>)[0][":", 0] = value;
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
                (Parameters["dynamic_param_file"] as DataCubeParameter<string>)[0][":",0] = _dynamic_param_file;
                OnPropertyChanged("DynamicParamFiles");
            }
        }

        [Category("Output Files")]
        [Description("Enable animation output control")]
        public bool AnimationOutOC
        {
            get
            {
                return _AnimationOutOC;
            }
            set
            {
                _AnimationOutOC = value;
                var infs = _AnimationOutOC ? 1 : 0;
                (Parameters["ani_output_oc"] as DataCubeParameter<int>)[0, 0, 0] = infs;
                OnPropertyChanged("AnimationOutOC");
            }
        }
        [Category("Output Files")]
        [Description("Pathname for the animation output control file")]
        public string AnimationOutOCFile
        {
            get
            {
                return _AnimationOutOCFile;
            }
            set
            {
                _AnimationOutOCFile = value;
                (Parameters["ani_output_oc_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
                OnPropertyChanged("AnimationOutOCFile");
            }
        }
        /// <summary>
        /// Available module: auto_wra, man_wra, none
        /// </summary>
        [Category("Modules")]
        [Description("Water resources allocation module. Available module: auto_wra, man_wra, none")]
        public string WRAModule
        {
            get
            {
                return _wra_module;
            }
            set
            {
                _wra_module = value;
                (Parameters["wra_module"] as DataCubeParameter<string>)[0, 0, 0] = value;
                OnPropertyChanged("WRAModule");
            }
        }
        [Category("Modules")]
        [Description("Input filename for the WRA module")]
        public string WRAModuleFile
        {
            get
            {
                return _wra_module_file;
            }
            set
            {
                _wra_module_file = value;
                (Parameters["wra_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
                OnPropertyChanged("WRAModuleFile");
            }
        }
        [Category("Modules")]
        [Description("Hydraulics Engine: SFR or SWMM")]
        public string HydraulicsEngine
        {
            get
            {
                return _hydraulics_engine;
            }
            set
            {
                _hydraulics_engine = value;
                (Parameters["hydraulics_engine"] as DataCubeParameter<string>)[0, 0, 0] = value;
                OnPropertyChanged("HydraulicsEngine");
            }
        }
        [Category("Modules")]
        [Description("Input filename for extension manager")]
        public string ExtensionManagerFile
        {
            get
            {
                return _extension_man_file;
            }
            set
            {
                _extension_man_file = value;
                (Parameters["extension_man_file"] as DataCubeParameter<string>)[0, 0, 0] = value;
                OnPropertyChanged("ExtensionManagerFile");
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
        public override bool Load(ICancelProgressHandler cancelprogess)
        {
            if (File.Exists(FileName))
            {
                try
                {
                    LoadFrom(FileName);
                    var start = (Parameters["start_time"] as DataCubeParameter<int>).ToVector();
                    _StartTime = new DateTime(start[0], start[1], start[2], start[3], start[4], start[5]);
                    var end = (Parameters["end_time"] as DataCubeParameter<int>).ToVector();
                    _EndTime = new DateTime(end[0], end[1], end[2], end[3], end[4], end[5]);
                    _DataFile = Parameters["data_file"].GetValue(0, 0, 0).ToString();
                    _ParameterFile = Parameters["param_file"].GetValue(0, 0, 0).ToString();
                    _ModflowFile = Parameters["modflow_name"].GetValue(0, 0, 0).ToString();
                    _ReportDays = (int)Parameters["rpt_days"].GetValue(0, 0, 0);
                    _WaterBudgetFile = Parameters["gsflow_output_file"].GetValue(0, 0, 0).ToString();
                    _PRMSBudgetFile = Parameters["model_output_file"].GetValue(0, 0, 0).ToString();
                    _OutputWaterComponent = (Parameters["gsf_rpt"] as DataCubeParameter<int>)[0, 0, 0] == 1 ? true : false;
                    _WaterComponentFile = (Parameters["csv_output_file"] as DataCubeParameter<string>)[0, 0, 0];
                    _Temperature = EnumHelper.FromString<TemperatureModule>((Parameters["temp_module"] as DataCubeParameter<string>)[0, 0, 0]);
                    _Precipitation = EnumHelper.FromString<PrecipitationModule>((Parameters["precip_module"] as DataCubeParameter<string>)[0, 0, 0]);
                    _SolarRadiation = EnumHelper.FromString<SolarRadiationModule>((Parameters["solrad_module"] as DataCubeParameter<string>)[0, 0, 0]);
                    _PotentialET = EnumHelper.FromString<PETModule>((Parameters["et_module"] as DataCubeParameter<string>)[0, 0, 0]);
                    if (Parameters.ContainsKey("wnd_module"))
                        _WindModule = EnumHelper.FromString<WindModule>((Parameters["wnd_module"] as DataCubeParameter<string>)[0, 0, 0]);
                    if (Parameters.ContainsKey("hum_module"))
                        _HumidityModule = EnumHelper.FromString<HumidityModule>((Parameters["hum_module"] as DataCubeParameter<string>)[0, 0, 0]);
                    if (Parameters.ContainsKey("press_module"))
                        _PressureModule = EnumHelper.FromString<PressureModule>((Parameters["press_module"] as DataCubeParameter<string>)[0, 0, 0]);
                    _SurfaceRunoff = EnumHelper.FromString<SurfaceRunoffModule>((Parameters["srunoff_module"] as DataCubeParameter<string>)[0, 0, 0]);
                    _StatsON = (Parameters["statsON_OFF"] as DataCubeParameter<int>)[0, 0, 0] == 1 ? true : false;
                    _StatVarFile = (Parameters["stat_var_file"] as DataCubeParameter<string>)[0, 0, 0];
                    _NumStatVars = (Parameters["nstatVars"] as DataCubeParameter<int>)[0, 0, 0];
                    if (Parameters.ContainsKey("statVar_element"))
                        _StatVarElement = (Parameters["statVar_element"] as DataCubeParameter<int>).ToVector();
                    _StatVarNames = (Parameters["statVar_names"] as DataCubeParameter<string>).ToVector();
                    _AniOutFileFormat = (FileFormat)(Parameters["aniOutON_OFF"] as DataCubeParameter<int>)[0, 0, 0];
                    _AniOutFileName = (Parameters["ani_output_file"] as DataCubeParameter<string>)[0, 0, 0] + ".nhru";
                    _NumAniOutVar = (Parameters["naniOutVars"] as DataCubeParameter<int>)[0, 0, 0];
                    _AniOutVarNames = (Parameters["aniOutVar_names"] as DataCubeParameter<string>).ToVector();
                    _InitVarsFromFile = (Parameters["init_vars_from_file"] as DataCubeParameter<int>)[0, 0, 0] == 1 ? true : false;
                    if ((Parameters.Keys.Contains("accu_budget_file")))
                        _WaterBudgetCsvFile = (Parameters["accu_budget_file"] as DataCubeParameter<string>)[0, 0, 0];
                    if (Parameters.ContainsKey("mflist_file"))
                        _MFListFile = (Parameters["mflist_file"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("use_gridclimate")))
                        _UseGridClimate = (Parameters["use_gridclimate"] as DataCubeParameter<int>)[0, 0, 0] == 1 ? true : false;
                    if ((Parameters.Keys.Contains("sub_climate_file")))
                        _GridClimateFile = (Parameters["sub_climate_file"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("var_init_file")))
                        _VarInitFile = (Parameters["var_init_file"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("save_vars_to_file")))
                        _SaveVarsToFile = (Parameters["save_vars_to_file"] as DataCubeParameter<int>)[0, 0, 0] == 1 ? true : false;
                    if ((Parameters.Keys.Contains("var_save_file")))
                        _VarSaveFile = (Parameters["var_save_file"] as DataCubeParameter<string>)[0, 0, 0];
                    _PrintDebug = (Parameters["print_debug"] as DataCubeParameter<int>)[0, 0, 0] == 1 ? true : false;
                    if ((Parameters.Keys.Contains("precip_day")))
                        _PrecipitationFile = (Parameters["precip_day"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("tmax_day")))
                        _TempMaxFile = (Parameters["tmax_day"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("tmin_day")))
                        _TempMinFile = (Parameters["tmin_day"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("potet_day")))
                        _PETFile = (Parameters["potet_day"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("wnd_day")))
                        _WindFile = (Parameters["wnd_day"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("hum_day")))
                        _HumidityFile = (Parameters["hum_day"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("press_day")))
                        _PressureFile = (Parameters["press_day"] as DataCubeParameter<string>)[0, 0, 0];
                    if (Parameters.ContainsKey("subbasin_flag"))
                        _SubbasinFlag = (Parameters["subbasin_flag"] as DataCubeParameter<int>)[0, 0, 0] == 1 ? true : false;
                    if (Parameters.ContainsKey("global_time_unit"))
                        _GlobalTimeUnit = (Parameters["global_time_unit"] as DataCubeParameter<int>)[0, 0, 0];
                    if (Parameters.ContainsKey("climate_file_format"))
                        _ClimateInputFormat = EnumHelper.FromString<FileFormat>((Parameters["climate_file_format"] as DataCubeParameter<int>)[0, 0, 0].ToString());
                    if (Parameters.ContainsKey("dynamic_para"))
                        _dynamic_para = (Parameters["dynamic_para"] as DataCubeParameter<int>)[0, 0, 0] == 1 ? true : false;
                    else
                        _dynamic_para = false;
                    if (Parameters.ContainsKey("dynamic_day"))
                        _dynamic_day = (Parameters["dynamic_day"] as DataCubeParameter<int>).ToVector();
                    if (Parameters.ContainsKey("dynamic_param_file"))
                        _dynamic_param_file = (Parameters["dynamic_param_file"] as DataCubeParameter<string>).ToVector();
                    if (Parameters.ContainsKey("sub_climate_flag"))
                        _UseGridClimate = (Parameters["sub_climate_flag"] as DataCubeParameter<int>)[0, 0, 0] == 1 ? true : false;
                    else
                        _UseGridClimate = false;
                    if (Parameters.ContainsKey("ani_output_oc"))
                        _AnimationOutOC = (Parameters["ani_output_oc"] as DataCubeParameter<int>)[0, 0, 0] == 1 ? true : false;
                    else
                        _AnimationOutOC = false;
                    if ((Parameters.Keys.Contains("ani_output_oc_file")))
                        _AnimationOutOCFile = (Parameters["ani_output_oc_file"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("wra_module")))
                        _wra_module = (Parameters["wra_module"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("wra_file")))
                        _wra_module_file = (Parameters["wra_file"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("hydraulics_engine")))
                        _hydraulics_engine = (Parameters["hydraulics_engine"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("extension_man_file")))
                        _extension_man_file = (Parameters["extension_man_file"] as DataCubeParameter<string>)[0, 0, 0];
                    if (Parameters.ContainsKey("save_soilwater_hru"))
                        _SaveSoilwaterFile = (Parameters["save_soilwater_hru"] as DataCubeParameter<int>)[0, 0, 0] == 1 ? true : false;
                    else
                        _SaveSoilwaterFile = false;
                    if ((Parameters.Keys.Contains("file_soilwater_hru")))
                        _SoilWaterFile = (Parameters["file_soilwater_hru"] as DataCubeParameter<string>)[0, 0, 0];
                    if ((Parameters.Keys.Contains("file_soilwater_budget")))
                        _SoilWaterBudgetFile = (Parameters["file_soilwater_budget"] as DataCubeParameter<string>)[0, 0, 0];

                    foreach (var pr in Parameters.Values)
                        pr.Owner = this;
                    this.TimeService.IOTimeFile = WaterBudgetCsvFile;
                    UpdateTimeService();
                    OnLoaded(cancelprogess);
                    return true;
                }
                catch(Exception ex)
                {
                    Message = string.Format("Failed to load control file {0}. Error message: {1}", FileName, ex.Message);
                    OnLoadFailed(Message, cancelprogess);
                    return false;
                }       
            }
            else
            {
                Message = string.Format("{0} does not exist. Please check the control file: ", FileName);
                OnLoadFailed(Message, cancelprogess);
                return false;
            }
            
        }

        public override void SaveAs(string filename, ICancelProgressHandler prg)
        {
            StreamWriter sw = new StreamWriter(filename);
            Save(sw);
            sw.Close();
            OnSaved(prg);
        }
        public override void New()
        {
            string _Controlfile = Path.Combine(BaseModel.ConfigPath, "heiflow_" + Owner.Project.SelectedVersion + ".control");

            if (File.Exists(_Controlfile))
            {
                //  string xmlcopy = this.FileName.Replace(".control", ".xml");
                File.Copy(_Controlfile, this.FileName, true);
                LoadFrom(this.FileName);
                ModelMode = Integration.ModelMode.GSFLOW;
                StartTime = new DateTime(DateTime.Now.Year - 1, 1, 1);
                EndTime = new DateTime(DateTime.Now.Year, 1, 1);
                Temperature = TemperatureModule.climate_hru;
                Precipitation = PrecipitationModule.climate_hru;
                SolarRadiation = SolarRadiationModule.ddsolrad_hru_prms;
                if (Owner.Project.SelectedVersion == "v1.0.0")
                {
                    PotentialET = PETModule.climate_hru;
                }
                else
                {
                    PotentialET = PETModule.potet_pm;
                    Wind = WindModule.climate_hru;
                    Humidity = HumidityModule.climate_hru;
                    Pressure = PressureModule.climate_hru;
                }
                SurfaceRunoff = SurfaceRunoffModule.srunoff_carea_casc;
                StatsON = false;
                AniOutFileFormat = FileFormat.Binary;
                InitVarsFromFile = false;
                SaveVarsToFile = false;
                AniOutVarNames = new string[] { "hru_actet", "soil_moist_frac" };
                //NumStatVars = 2;
                //StatVarElement = new int[] { 1,2};
                //StatVarNames = new string[] { "basin_cfs", "basin_reach_latflow" };


                SubbasinFlag = false;
                OutputWaterComponent = true;
                UseGridClimate = true;
                ReportDays = 1;
                PrintDebug = false;
                ClimateInputFormat = FileFormat.Text;
                WRAModule = "none";
                HydraulicsEngine = "SFR";
                AnimationOutOC = false;
                GlobalTimeUnit = 4;
                if (Owner.Project.SelectedVersion != "v1.0.0")
                    SaveSoilWaterFile = true;

                GridClimateFile = string.Format(".\\input\\prms\\{0}_climate.map", Owner.Project.Name);
                DataFile = string.Format(".\\input\\prms\\{0}.data", Owner.Project.Name);
                ParameterFilePath = string.Format(".\\input\\prms\\{0}.param", Owner.Project.Name);
                ModflowFilePath = string.Format(".\\input\\modflow\\{0}.nam", Owner.Project.Name);
                GridClimateFile = string.Format(".\\input\\prms\\{0}.hru_clm", Owner.Project.Name);
                VarInitFile = string.Format(".\\input\\prms\\{0}_prms_ic.in", Owner.Project.Name);
                PrecipitationFile = string.Format(".\\input\\prms\\{0}_precip.txt", Owner.Project.Name);
                TempMaxFile = string.Format(".\\input\\prms\\{0}_tmax.txt", Owner.Project.Name);
                TempMinFile = string.Format(".\\input\\prms\\{0}_tmin.txt", Owner.Project.Name);
                if (Owner.Project.SelectedVersion == "v1.0.0")
                    PETFile = string.Format(".\\input\\prms\\{0}_pet.txt", Owner.Project.Name);
                else
                {
                    WindFile = string.Format(".\\input\\prms\\{0}_wnd.txt", Owner.Project.Name);
                    HumidityFile = string.Format(".\\input\\prms\\{0}_hum.txt", Owner.Project.Name);
                    PressureFile = string.Format(".\\input\\prms\\{0}_pres.txt", Owner.Project.Name);
                }
                AnimationOutOCFile = string.Format(".\\input\\prms\\{0}_aniout.oc", Owner.Project.Name);
                WRAModuleFile = string.Format(".\\input\\prms\\{0}.wra", Owner.Project.Name);
                ExtensionManagerFile = string.Format(".\\input\\extension\\extensions.exm", Owner.Project.Name);

                WaterBudgetFile = string.Format(".\\output\\{0}_total_budget.out", Owner.Project.Name);
                PRMSBudgetFile = string.Format(".\\output\\{0}_prms_budget.out", Owner.Project.Name);
                WaterBudgetCsvFile = string.Format(".\\output\\{0}_accu_budget.csv", Owner.Project.Name);
                WaterComponentFile = string.Format(".\\output\\{0}_budget_component.csv", Owner.Project.Name);
                StatVarFile = string.Format(".\\output\\{0}_statvar.txt", Owner.Project.Name);
                AniOutFileName = string.Format(".\\output\\{0}_animation.out", Owner.Project.Name);
                VarSaveFile = string.Format(".\\output\\{0}_prms_ic.out", Owner.Project.Name);
                MFListFile = string.Format(".\\output\\{0}.lst", Owner.Project.Name);
                if (Owner.Project.SelectedVersion != "v1.0.0")
                {
                    SoilWaterFile = string.Format(".\\output\\{0}_sm.dcx", Owner.Project.Name);
                    SoilWaterBudgetFile = string.Format(".\\output\\{0}_sm_budget.csv", Owner.Project.Name);
                }
                DynamicDays = new int[] { 2 };
                DynamicPara = false;
                DynamicParamFiles = new string[] { _ParameterFile };
                
                foreach (var pr in Parameters.Values)
                    pr.Owner = this;
                UpdateTimeService();
                base.New();
                //NewXml();
                State = ModelObjectState.Ready;
            }
            else
            {
                Message = string.Format("{0} does not exist. Please repair the software.", _Controlfile);
                IsDirty = false;
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
            if (Parameters.ContainsKey("global_time_unit"))
                TimeService.TimeUnit = (Parameters["global_time_unit"] as DataCubeParameter<int>)[0, 0, 0];
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
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs);
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
                    var nDimension = 1;

                    var value_str = "";
                    for (int i = 0; i < ValueCount; i++)
                    {
                        value_str += sr.ReadLine() + " ";
                    }

                    if (ValueType == 1)
                    {
                        var intbuf = TypeConverterEx.Split<int>(value_str);
                        DataCubeParameter<int> gv = new DataCubeParameter<int>(1, intbuf.Count(), 1)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Control,
                            Dimension = nDimension,
                            Name = name,
                            DimensionNames = new string[] {"control"},
                            Owner = this
                        };
                        gv[0][":", 0] = intbuf;
                        if (!Parameters.Keys.Contains(gv.Name))
                            Parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 2)
                    {
                        var floatbuf = TypeConverterEx.Split<float>(value_str);
                        DataCubeParameter<float> gv = new DataCubeParameter<float>(1, floatbuf.Count(), 1)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Control,
                            Dimension = nDimension,
                            DimensionNames = new string[] { "control" },
                            Name = name,
                            Owner = this
                        };
                        gv[0][":", 0] = floatbuf;
                        if (!Parameters.Keys.Contains(gv.Name))
                            Parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 3)
                    {
                        var doublebuf = TypeConverterEx.Split<double>(value_str);
                        DataCubeParameter<double> gv = new DataCubeParameter<double>(1, doublebuf.Count(), 1)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Control,
                            Dimension = nDimension,
                            DimensionNames = new string[] { "control" },
                            Name = name,
                            Owner = this
                        };
                        gv[0][":", 0] = doublebuf;
                        if (!Parameters.Keys.Contains(gv.Name))
                            Parameters.Add(gv.Name, gv);
                    }
                    else if (ValueType == 4)
                    {
                        var strbuf = TypeConverterEx.Split<string>(value_str);
                        DataCubeParameter<string> gv = new DataCubeParameter<string>(1, strbuf.Count(), 1)
                        {
                            ValueType = ValueType,
                            VariableType = ParameterType.Control,
                            Dimension = nDimension,
                            DimensionNames = new string[] { "control" },
                            Name = name,
                            Owner = this
                        };
                        gv[0][":", 0] = strbuf;
                        if (!Parameters.Keys.Contains(gv.Name))
                            Parameters.Add(gv.Name, gv);
                    }
                }
            }
            fs.Close();
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
                    var vv = (pp as DataCubeParameter<int>).ToVector();
                    sw.WriteLine(vv.Length);
                    sw.WriteLine(pp.ValueType);
                    sw.WriteLine(string.Join("\n", vv));
                }
                else if (pp.ValueType == 2)
                {
                    var vv = (pp as DataCubeParameter<float>).ToVector();
                    sw.WriteLine(vv.Length);
                    sw.WriteLine(pp.ValueType);
                    sw.WriteLine(string.Join("\n", vv));
                }
                else if (pp.ValueType == 3)
                {
                    var vv = (pp as DataCubeParameter<double>).ToVector();
                    sw.WriteLine(vv.Length);
                    sw.WriteLine(pp.ValueType);
                    sw.WriteLine(string.Join("\n", vv));
                }
                else if (pp.ValueType == 4)
                {
                    var vv = (pp as DataCubeParameter<string>).ToVector();
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

        public void CheckClimateFilename()
        {
            var files = new string[] { PrecipitationFile, TempMaxFile, TempMinFile, PETFile, WindFile, PressureFile, HumidityFile };
            if (ClimateInputFormat == FileFormat.Binary)
            {
                if (PrecipitationFile.Contains(".txt"))
                {
                    PrecipitationFile = PrecipitationFile.Replace(".txt", ".dcx");
                }
                if (TempMaxFile.Contains(".txt"))
                {
                    TempMaxFile = TempMaxFile.Replace(".txt", ".dcx");
                }
                if (TempMinFile.Contains(".txt"))
                {
                    TempMinFile = TempMinFile.Replace(".txt", ".dcx");
                }
                if (PETFile.Contains(".txt"))
                {
                    PETFile = PETFile.Replace(".txt", ".dcx");
                }
                if (WindFile.Contains(".txt"))
                {
                    WindFile = WindFile.Replace(".txt", ".dcx");
                }
                if (PressureFile.Contains(".txt"))
                {
                    PressureFile = PressureFile.Replace(".txt", ".dcx");
                }
                if (HumidityFile.Contains(".txt"))
                {
                    HumidityFile = HumidityFile.Replace(".txt", ".dcx");
                }
            }
            else
            {
                if (PrecipitationFile.Contains(".dcx"))
                {
                    PrecipitationFile = PrecipitationFile.Replace(".dcx", ".txt");
                }
                if (TempMaxFile.Contains(".dcx"))
                {
                    TempMaxFile = TempMaxFile.Replace(".dcx", ".txt");
                }
                if (TempMinFile.Contains(".dcx"))
                {
                    TempMinFile = TempMinFile.Replace(".dcx", ".txt");
                }
                if (PETFile.Contains(".dcx"))
                {
                    PETFile = PETFile.Replace(".dcx", ".txt");
                }
                if (WindFile.Contains(".dcx"))
                {
                    WindFile = WindFile.Replace(".dcx", ".txt");
                }
                if (PressureFile.Contains(".dcx"))
                {
                    PressureFile = PressureFile.Replace(".dcx", ".txt");
                }
                if (HumidityFile.Contains(".dcx"))
                {
                    HumidityFile = HumidityFile.Replace(".dcx", ".txt");
                }
            }
        }

        public void WriteDefaultClimateMapFile(int nhru)
        {
            string filename = Path.Combine(ModelService.WorkDirectory, _GridClimateFile);
            StreamWriter sw = new StreamWriter(filename);
            string line = string.Format("# Clmate Grid ID, HRU ID");
            sw.WriteLine(line);
            line = string.Format("{0} {1} # subbasin_count hru_count", nhru, nhru);
            sw.WriteLine(line);
            for (int i = 1; i <= nhru; i++)
            {
                line = string.Format("{0}\t{1}", i, i);
                sw.WriteLine(line);
            }
            sw.Close();
        }
    }
}
