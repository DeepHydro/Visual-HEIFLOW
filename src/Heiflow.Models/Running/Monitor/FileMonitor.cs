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
using Heiflow.Models.Generic.Project;
using Heiflow.Models.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Foundation;

namespace Heiflow.Models.Running
{
    public abstract class FileMonitor : Model, Heiflow.Models.Running.IFileMonitor
    {
        protected string _FileName;
        protected List<IMonitorItem> _Roots;
        protected IArrayWatcher _Watcher;
        protected ListTimeSeries<double> _DataSource;
        protected int _CurrentStep;
        protected int _StartStep_Budget = 1;
        protected int _EndStep_Budget = 0;
        protected int _Inteval_Budget = 365;
        protected int _DecimalDigit = 2;

        public const string _In_Group = "In Terms";
        public const string _Out_Group = "Out Terms";
        public const string _Cu_In_Group = "Cumulative In Terms";
        public const string _Cu_Out_Group = "Cumulative Out Terms";
        public const string _Ds_Group = "Storage Change";
        public const string _Total_Group = "Total";
        public const string _Storage_Group = "Storage";

        //Total Budgets Items
        public static string PPT = "Precipitation";
        public static string Streams_Inflow = "Streams Inflow";
        public static string Streams_Outflow = "Streams Outflow";
        public static string Groundwater_Inflow = "Groundwater Inflow";
        public static string Groundwater_Outflow = "Groundwater Outflow";
        public static string Wells_In = "Wells In";
        public static string Lakes_Inflow = "Lakes Inflow";
        public static string Lakes_Outflow = "Lakes Outflow";
        public static string Evapotranspiration = "Evapotranspiration";
        public static string Evaporation = "Evaporation"; 
        public static string LAND_SURFACE_Zone_DS = "Land Surface";
        public static string Soil_Zone_DS = "Soil Zone";
        public static string Unsaturated_Zone_DS = "Unsaturated Zone";
        public static string Saturated_Zone_DS = "Saturated Zone";
        public static string Lakes_Zone_DS = "Lakes";
        public static string Canals_Zone_DS = "Canals";
        public static string SFR_DS = "Streams";
        public static string Total_Storage_Change = "Total Storage Change";

        public static string LAKET = "Lakes ET";

        public static string SFR_PPT = "Stream Precipitation";
        public static string SFR_INFLOW = "Stream Inflow";
        public static string SFRET = "Stream ET";
       

        public static string CANAL_ET = "Canals ET";
        public static string Canal_Drainage = "Canals Drainage";
        public static string Canal_Storage = "Canals Storage";
        public static string Canal_DS = "Canals Storage Change";

        public static string IR_PUMP = "Pumping";
        public static string IR_DIV = "Diversion";
        public static string IR_Industry = "Industry";
        public static string IR_CANAL_ET = "Irrigation Canals ET";

        public static string BASINGW2SZ_HRU = "Groundwater Discharge from SAT to Soil Zone";
        public static string BASINSZREJECT = "Rejected  Gravity Drainage by UZ/SAT";

        public static string BASINPERVET_HRU = "Pervious Areas ET";
        public static string BASINIMPERVEVAP_HRU = "Impervious Areas ET";
        public static string BASININTCPEVAP_HRU = "Intercepted Precipitation ET";
        public static string BASINSNOWEVAP_HRU = "Snowpack Sublimation";

        public static string BASININTERFLOW = "Slow interflow to streams";
        /// <summary>
        ///  Hortonian and Dunnian surface runoff to streams
        /// </summary>
        public static string BASINSROFF = "Hortonian and Dunnian surface runoff to streams";
        /// <summary>
        /// Dunnian runoff and interflow to lakes
        /// </summary>
        public static string BASINLAKEINSZ = "Dunnian runoff and interflow to lakes";
        public static string BASINHORTONIANLAKES = "Hortonian runoff to lakes";
        public static string Daily_PPT = "Daily Precipitation";
        public static string Percolation = "Percolation";
        public static string BASINSZ2GW = "Potential gravity drainage from the soil zone to UZ";
        public static string HRU_IN = "Total HRU In";
        public static string HRU_OUT = "Total HRU Out";
        public static string HRU_STORAGE = "Total HRU Storage";
        public static string HRU_DS = "Total HRU Storage Change";
        public static string HRU_ERROR = "Total HRU Budget Error";
        public static string HRU_DISYP = "Total HRU Budget Percent Discrepancy";

        public static string Soil_ET = "Soil Zone ET";

        public static string UZF_INFIL = "Infiltration to UZ and SZ zones";
        public static string UZF_ET = "ET from UZ and SZ zones";
        public static string UZF_RECHARGE = "Recharge from UZ to SZ";
        public static string UZF_IN = "Total UZ In";
        public static string UZF_OUT = "Total UZ Out";
        public static string UZF_DS = "Total UZ Storage Change";
        public static string UZF_ERROR = "Total UZ Budget Error";
        public static string UZF_DISPY = "Total UZ Budget Percent Discrepancy";

        public static string GW_INOUT = "SAT Boundary Infow";
        public static string STREAM_LEAKAGE = "Stream Leakage to the UZ and SAT";
        public static string SAT_ET = "Saturated ET";
        public static string SAT_CHANGE_STOR = "Total Storage Change in SAT";

        public static string CONSTANT_HEAD_IN = "CONSTANT HEAD IN";
        public static string WELLS_IN = "WELLS IN";
        public static string SPECIFIED_FLOWS_IN = "SPECIFIED FLOWS IN";
        public static string UZF_RECHARGE_IN = "UZF RECHARGE IN";
        public static string GW_ET_IN = "GW ET IN";
        public static string SURFACE_LEAKAGE_IN = "SURFACE LEAKAGE IN";
        public static string STREAM_LEAKAGE_IN = "STREAM LEAKAGE IN";
        public static string LAKE_SEEPAGE_IN = "LAKE SEEPAGE IN";
        public static string STORAGE_IN = "STORAGE IN";

        public static string CONSTANT_HEAD_OUT = "CONSTANT HEAD OUT";
        public static string WELLS_OUT = "WELLS OUT";
        public static string SPECIFIED_FLOWS_OUT = "SPECIFIED FLOWS OUT";
        public static string UZF_RECHARGE_OUT = "UZF RECHARGE OUT";
        public static string GW_ET_OUT = "GW ET OUT";
        public static string SURFACE_LEAKAGE_OUT = "SURFACE LEAKAGE OUT";
        public static string STREAM_LEAKAGE_OUT = "STREAM LEAKAGE OUT";
        public static string LAKE_SEEPAGE_OUT = "LAKE SEEPAGE OUT";
        public static string STORAGE_OUT = "STORAGE OUT";

        public static string SAT_IN = "Total SAT IN";
        public static string SAT_OUT = "Total SAT OUT";
        public static string SAT_ERROR = "Total SAT ERROR";
        /// <summary>
        /// Total SAT Discrepancy in 100%
        /// </summary>
        public static string SAT_DISPY = "Total SAT Percent Discrepancy";
        public static string SAT_DS = "Total SAT DS";
        public static string TOTAL_DISPY = "Total Percent Discrepancy";

        public FileMonitor()
        {
            MonitorName = "FileMonitor";
            _Roots = new List<IMonitorItem>();
            Partners = new List<IFileMonitor>();
            _CurrentStep = 0;
            StartStep = 366;
        }
        [Category("Design")]
        public string MonitorName { get; protected set; }
        [Category("Data")]
        public virtual string FileName
        {
            get
            {
                if (TypeConverterEx.IsNull(_FileName))
                    return _FileName;
                else
                    return Path.Combine(ModelService.WorkDirectory, _FileName);
            }
            set
            {
                SetProperty(ref _FileName, value);
                if (_Watcher != null)
                    _Watcher.FileName = _FileName;
            }
        }
        [Browsable(false)]
        public List<IMonitorItem> Root
        {
            get
            {
                return _Roots;
            }
            set
            {
                _Roots = value;
            }
        }
        [Browsable(false)]
        public IArrayWatcher Watcher
        {
            get
            {
                return _Watcher;
            }
            set
            {
                _Watcher = value;
            }
        }
        [Browsable(false)]
        public ListTimeSeries<double> DataSource
        {
            get
            {
                return Watcher.DataSource;
            }
        }
        [Browsable(false)]
        public int CurrentStep
        {
            get;
            set;
        }
        /// <summary>
        /// starting from 1
        /// </summary>
        [Category("Analysis")]
        public int StartStep
        {
            get
            {
                return _StartStep_Budget;
            }
            set
            {
                if (value < 1)
                    value = 1;
                _StartStep_Budget = value;
            }
        }
        /// <summary>
        /// starting from 1
        /// </summary>
        [Category("Analysis")]
        public int EndStep
        {
            get
            {
                return _EndStep_Budget;
            }
            set
            {
                _EndStep_Budget = value;
            }
        }
        [Category("Analysis")]
        public int Intevals
        {
            get
            {
                return _Inteval_Budget;
            }
            set
            {
                if (value < 1)
                    value = 1;
                _Inteval_Budget = value;
            }
        }
           [Browsable(false)]
        public bool IsStarted
        {
            get;
            protected set;
        }
        [Category("Analysis")]
        public int DecimalDigit
        {
            get
            {
                return _DecimalDigit;
            }
            set
            {
                if (value <= 0)
                    value = 1;
                _DecimalDigit = value;
            }
        }

           [Browsable(false)]
        public List<IFileMonitor> Partners
        {
            get;
            protected set;
        }

        public virtual void Clear()
        {
            _CurrentStep = 0;
            _Watcher.Clear();
        }


        public virtual void Start()
        {     
            _Watcher.Start();
            IsStarted = true;
        }

        public virtual void Stop()
        {
            _Watcher.Stop();
            _CurrentStep = 0;
            IsStarted = false;
        }

        /// <summary>
        /// Create balance table
        /// </summary>
        /// <param name="report">balance report</param>
        /// <returns></returns>
        public virtual System.Data.DataTable Balance(ref string budget)
        {
            return null;
        }

        public MonitorItem Select(string name)
        {
            MonitorItem selected = null;
            foreach (var root in this.Root)
            {
                foreach (var item in root.Children)
                {
                    if (item.Name == name)
                    {
                        selected = item;
                        break;
                    }
                }
            }
            if (selected == null)
            {
                foreach (var par in this.Partners)
                {
                    foreach (var root in par.Root)
                    {
                        foreach (var item in root.Children)
                        {
                            if (item.Name == name)
                            {
                                selected = item;
                                break;
                            }
                        }
                    }
                }
            }
            return selected;
        }


        public virtual Dictionary<string, double> ZonalBudgets()
        {
            return new Dictionary<string, double>();
        }



    }
}