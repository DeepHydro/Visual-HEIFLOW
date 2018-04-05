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
using Heiflow.Models.Generic.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Heiflow.Models.IO;
using System.ComponentModel.Composition;
using Heiflow.Models.Generic;

namespace Heiflow.Models.Running
{
    [Export(typeof(IFileMonitor))]
    public class BudgetComponentMonitor:FileMonitor
    {

        public BudgetComponentMonitor()
        {

            MonitorName = "BudgetComponentMonitor";

            #region ET BUDGETS
            var root_et = new MonitorItemCollection("ET Budgets");
            _Roots.Add(root_et);

            MonitorItem basinpervet = new MonitorItem("Pervious Areas ET")
            {
                VariableIndex = 1,
                Group = _In_Group
            };

            MonitorItem basinimpervevap = new MonitorItem("Impervious Areas ET")
            {
                VariableIndex = 2,
                Group = _In_Group
            };

            MonitorItem basinintcpevap = new MonitorItem("Intercepted Precipitation ET")
            {
                VariableIndex = 3,
                Group = _In_Group
            };

            MonitorItem basinsnowevap = new MonitorItem("Snowpack Sublimation ET")
            {
                VariableIndex = 4,
                Group = _In_Group
            };

            MonitorItem uzfet = new MonitorItem(UZF_ET)
            {
                VariableIndex = 58,
                Group = _In_Group
            };

            MonitorItem satet = new MonitorItem("Saturated zones ET")
            {
                VariableIndex = 59,
                Group = _In_Group
            };

            var lakes_et = new MonitorItem(LAKET)
            {
                VariableIndex = 51,
                Group = _In_Group
            };

            var sfr_et = new MonitorItem(SFRET)
            {
                VariableIndex = 64,
                Group = _In_Group
            };

            var canal_et = new MonitorItem(CANAL_ET)
            {
                VariableIndex = 70,
                Group = _In_Group
            };

            MonitorItem et_total_in = new MonitorItem("Total ET")
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { 1, 2, 3, 4, 58, 59, 51, 64, 70 }
            };
            root_et.Children.Add(basinpervet);
            root_et.Children.Add(basinimpervevap);
            root_et.Children.Add(basinintcpevap);
            root_et.Children.Add(basinsnowevap);
            root_et.Children.Add(uzfet);
            root_et.Children.Add(satet);
            root_et.Children.Add(lakes_et);
            root_et.Children.Add(sfr_et);
            root_et.Children.Add(canal_et);
            root_et.Children.Add(et_total_in);
            #endregion

            #region Irrigation BUDGETS
            var root_irrigation = new MonitorItemCollection("Irrigation Budgets");
            MonitorItem ir_div = new MonitorItem(IR_DIV)
            {
                VariableIndex = 66,
                Group = _In_Group,
            };
            MonitorItem ir_pump = new MonitorItem(IR_PUMP)
            {
                VariableIndex = 67,
                Group = _In_Group,
            };
            MonitorItem ir_industry = new MonitorItem(IR_Industry)
            {
                VariableIndex = 68,
                Group = _In_Group,
            };

            MonitorItem canal_drain = new MonitorItem(Canal_Drainage)
            {
                VariableIndex = 69,
                Group = _Out_Group,
            };

            MonitorItem canal_et_ir = new MonitorItem(IR_CANAL_ET)
            {
                VariableIndex = 70,
                Group = _Out_Group,
            };

            MonitorItem canal_stor = new MonitorItem(Canal_Storage)
            {
                VariableIndex = 71,
                Group = _Storage_Group,
            };

            MonitorItem canal_ds = new MonitorItem(Canal_DS)
            {
                VariableIndex = 72,
                Group = _Total_Group,
            };

            MonitorItem ir_totalin = new MonitorItem("Total Irrigation In")
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { ir_div.VariableIndex, ir_pump.VariableIndex, ir_industry.VariableIndex }
            };


            root_irrigation.Children.Add(ir_div);
            root_irrigation.Children.Add(ir_pump);
            root_irrigation.Children.Add(ir_industry);
            root_irrigation.Children.Add(canal_drain);
            root_irrigation.Children.Add(canal_et_ir);
            root_irrigation.Children.Add(canal_stor);
            root_irrigation.Children.Add(canal_ds);
            root_irrigation.Children.Add(ir_totalin);

            _Roots.Add(root_irrigation);
            #endregion

            #region HRU BUDGETS
            var root_hru = new MonitorItemCollection("HRU Water Budgets");
            _Roots.Add(root_hru);

            var basinsoilmoist = new MonitorItem("Capillary Reservoirs Storage")
            {
                VariableIndex = 14,
                Group = _Storage_Group
            };
            var basingravstor = new MonitorItem("Gravity Reservoirs Storage")
            {
                VariableIndex = 15,
                Group = _Storage_Group
            };
            var pc_stor = new MonitorItem("Plant Canopy Reservoirs Storage")
            {
                VariableIndex = 17,
                Group = _Storage_Group
            };
            var im_stor = new MonitorItem("Impervious Reservoirs Storage")
            {
                VariableIndex = 18,
                Group = _Storage_Group
            };
            var sn_stor = new MonitorItem("Snowpack")
            {
                VariableIndex = 19,
                Group = _Storage_Group
            };

            MonitorItem hru_stor = new MonitorItem(HRU_STORAGE)
            {
                VariableIndex = -1,
                Group = _Storage_Group,
                Derivable = true,
                DerivedIndex = new int[] { basinsoilmoist.VariableIndex, basingravstor.VariableIndex, pc_stor.VariableIndex, im_stor.VariableIndex, sn_stor.VariableIndex }
            };

            var sfr_inflow = new MonitorItem(SFR_INFLOW)
            {
                VariableIndex = 62,
                Group = _In_Group
            };

            var ppt = new MonitorItem(Daily_PPT)
            {
                VariableIndex = 0,
                Group = _In_Group
            };
            // "Groundwater Discharge from SAT to Soil Zone";
            var basingw2sz_hru = new MonitorItem(BASINGW2SZ_HRU)
            {
                VariableIndex = 7,
                Group = _In_Group
            };
            // "Rejected  Gravity Drainage by UZ/SAT";
            var basinszreject = new MonitorItem(BASINSZREJECT)
            {
                VariableIndex = 25,
                Group = _In_Group
            };
            // "Pervious Areas ET";
            MonitorItem basinpervet_hru = new MonitorItem(BASINPERVET_HRU)
            {
                VariableIndex = 1,
                Group = _Out_Group
            };
            //"Impervious Areas ET";
            MonitorItem basinimpervevap_hru = new MonitorItem(BASINIMPERVEVAP_HRU)
            {
                VariableIndex = 2,
                Group = _Out_Group
            };
            // "Intercepted Precipitation ET";
            MonitorItem basinintcpevap_hru = new MonitorItem(BASININTCPEVAP_HRU)
            {
                VariableIndex = 3,
                Group = _Out_Group
            };
            //"Snowpack Sublimation";
            MonitorItem basinsnowevap_hru = new MonitorItem(BASINSNOWEVAP_HRU)
            {
                VariableIndex = 4,
                Group = _Out_Group
            };
            // "Slow interflow to streams";
            MonitorItem basininterflow = new MonitorItem(BASININTERFLOW)
            {
                VariableIndex = 20,
                Group = _Out_Group
            };
            //"Hortonian and Dunnian surface runoff to streams";
            MonitorItem basinsroff = new MonitorItem(BASINSROFF)
            {
                VariableIndex = 21,
                Group = _Out_Group
            };
            // "Gravity drainage from the soil zone to UZ";
            MonitorItem basinsz2gw = new MonitorItem(BASINSZ2GW)
            {
                VariableIndex = 6,
                Group = _Out_Group
            };
            MonitorItem basinhortonianlakes = new MonitorItem(BASINHORTONIANLAKES)
            {
                VariableIndex = 49,
                Group = _Out_Group
            };
            // "Dunnian runoff and interflow to lakes";
            MonitorItem basinlakeinsz = new MonitorItem(BASINLAKEINSZ)
            {
                VariableIndex = 50,
                Group = _Out_Group
            };

            MonitorItem hru_in = new MonitorItem(HRU_IN)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { 0, 7, 25, ir_div.VariableIndex, ir_pump.VariableIndex }
            };
            MonitorItem hru_out = new MonitorItem(HRU_OUT)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { basinpervet_hru.VariableIndex, basinimpervevap_hru.VariableIndex,
                    basinintcpevap_hru.VariableIndex, basinsnowevap_hru.VariableIndex, basinsz2gw.VariableIndex, basininterflow.VariableIndex,
                    basinsroff.VariableIndex , basinhortonianlakes.VariableIndex, basinlakeinsz.VariableIndex }
            };

            SequenceMonitorItem hru_ds = new SequenceMonitorItem(HRU_DS)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true
            };
            hru_ds.Source = hru_stor;

            AggregatedMonitorItem hru_error = new AggregatedMonitorItem(HRU_ERROR)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true
            };
            hru_error.Source.AddRange(new MonitorItem[] { hru_in, hru_out, hru_ds });
            hru_error.SourceSign.AddRange(new int[] { -1, 1, 1 });

            root_hru.Children.Add(basinsoilmoist);
            root_hru.Children.Add(basingravstor);
            root_hru.Children.Add(pc_stor);
            root_hru.Children.Add(im_stor);
            root_hru.Children.Add(sn_stor);
            root_hru.Children.Add(hru_stor);

            root_hru.Children.Add(sfr_inflow);
            root_hru.Children.Add(ppt);
            root_hru.Children.Add(basingw2sz_hru);
            root_hru.Children.Add(basinszreject);

            root_hru.Children.Add(basinpervet_hru);
            root_hru.Children.Add(basinimpervevap_hru);
            root_hru.Children.Add(basinintcpevap_hru);
            root_hru.Children.Add(basinsnowevap_hru);
            root_hru.Children.Add(basininterflow);
            root_hru.Children.Add(basinsroff);
            root_hru.Children.Add(basinsz2gw);
            root_hru.Children.Add(basinhortonianlakes);

            root_hru.Children.Add(hru_in);
            root_hru.Children.Add(hru_out);
            root_hru.Children.Add(hru_ds);
            root_hru.Children.Add(hru_error);
            #endregion

            #region SOIL ZONE BUDGETS
            var root_soil = new MonitorItemCollection("Soil Zone Water Budgets");
            _Roots.Add(root_soil);

            MonitorItem soil_stor = new MonitorItem("Total Soil Zone Storage")
            {
                VariableIndex = -1,
                Group = _Storage_Group,
                Derivable = true,
                DerivedIndex = new int[] { basinsoilmoist.VariableIndex, basingravstor.VariableIndex }
            };
            root_soil.Children.Add(basinsoilmoist);
            root_soil.Children.Add(basingravstor);
            root_soil.Children.Add(soil_stor);

            // "Groundwater Discharge from SAT to Soil Zone";
            MonitorItem basingw2sz = new MonitorItem(BASINGW2SZ_HRU)
            {
                VariableIndex = 7,
                Group = _In_Group,
            };
            //"Soil infiltration"
            MonitorItem basininfil = new MonitorItem("Soil infiltration")
            {
                VariableIndex = 39,
                Group = _In_Group,
            };
            root_soil.Children.Add(basingw2sz);
            root_soil.Children.Add(basininfil);
            root_soil.Children.Add(basinszreject);
            //"Pervious Areas ET";
            MonitorItem basinpervet_soil = new MonitorItem(BASINPERVET_HRU)
            {
                VariableIndex = 1,
                Group = _Out_Group
            };
            //"Dunnian runoff to streams"
            MonitorItem basindunnian = new MonitorItem("Dunnian runoff to streams")
            {
                VariableIndex = 40,
                Group = _Out_Group
            };
            // "Gravity drainage from the soil zone to UZ";
            MonitorItem basinsoiltogw = new MonitorItem(BASINSZ2GW)
            {
                VariableIndex = 6,
                Group = _Out_Group
            };

            root_soil.Children.Add(basininterflow);
            root_soil.Children.Add(basinsz2gw);
            root_soil.Children.Add(basinpervet_soil);
            root_soil.Children.Add(basindunnian);
            root_soil.Children.Add(basinsoiltogw);
            root_soil.Children.Add(basinlakeinsz);

            SequenceMonitorItem soil_ds = new SequenceMonitorItem("Soil Zone Storage Change")
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true
            };
            soil_ds.Source = soil_stor;

            MonitorItem soil_in = new MonitorItem("Total Soil In")
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { basingw2sz.VariableIndex, basininfil.VariableIndex, basinszreject.VariableIndex }
            };

            MonitorItem soil_out = new MonitorItem("Total Soil Out")
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { basininterflow.VariableIndex, basinsz2gw.VariableIndex,
                basinpervet_soil.VariableIndex, basindunnian.VariableIndex, basinsoiltogw.VariableIndex, basinlakeinsz.VariableIndex}
            };

            AggregatedMonitorItem soil_error = new AggregatedMonitorItem("Total Soil Budget Error")
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true
            };
            soil_error.Source.AddRange(new MonitorItem[] { soil_in, soil_out, soil_ds });
            soil_error.SourceSign.AddRange(new int[] { -1, 1, 1 });

            root_soil.Children.Add(soil_ds);
            root_soil.Children.Add(soil_in);
            root_soil.Children.Add(soil_out);
            root_soil.Children.Add(soil_error);
            #endregion

            #region UZF BUDGETS
            var root_uzf = new MonitorItemCollection("Unsaturated Zone Water Budgets");
            _Roots.Add(root_uzf);

            // "Infiltration to UZ and SZ zones";
            MonitorItem uzf_infil = new MonitorItem(UZF_INFIL)
            {
                VariableIndex = 28,
                Group = _In_Group,
            };
            root_uzf.Children.Add(uzf_infil);

            MonitorItem uzf_et = new MonitorItem(UZF_ET)
            {
                VariableIndex = 58,
                Group = _Out_Group,
            };
            //"Recharge from UZ to SZ";
            MonitorItem uzf_recharge = new MonitorItem(UZF_RECHARGE)
            {
                VariableIndex = 10,
                Group = _Out_Group,
            };
            root_uzf.Children.Add(uzf_et);
            root_uzf.Children.Add(uzf_recharge);

            MonitorItem uzf_del_stor = new MonitorItem(UZF_DS)
            {
                VariableIndex = 29,
                Group = _Total_Group,
            };
            MonitorItem uzf_totalin = new MonitorItem(UZF_IN)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { uzf_infil.VariableIndex }
            };
            MonitorItem uzf_totalout = new MonitorItem(UZF_OUT)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { uzf_et.VariableIndex, uzf_recharge.VariableIndex }
            };
            AggregatedMonitorItem uzf_error = new AggregatedMonitorItem(UZF_ERROR)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true
            };
            uzf_error.Source.AddRange(new MonitorItem[] { uzf_totalin, uzf_totalout, uzf_del_stor });
            uzf_error.SourceSign.AddRange(new int[] { -1, 1, 1 });

            root_uzf.Children.Add(uzf_del_stor);
            root_uzf.Children.Add(uzf_totalin);
            root_uzf.Children.Add(uzf_totalout);
            root_uzf.Children.Add(uzf_error);
            #endregion

            #region SATURATED ZONE BUDGETS
            var root_sat = new MonitorItemCollection("Saturated Zone Water Budgets");
            _Roots.Add(root_sat);

            //"Recharge from UZ to SAT"
            MonitorItem uzf_recharge_sat = new MonitorItem(UZF_RECHARGE)
            {
                VariableIndex = 10,
                Group = _In_Group,
            };
            MonitorItem gw_inout = new MonitorItem(GW_INOUT)
            {
                VariableIndex = 8,
                Group = _In_Group,
            };
            MonitorItem stream_leakage = new MonitorItem(STREAM_LEAKAGE)
            {
                VariableIndex = 9,
                Group = _In_Group,
            };
            root_sat.Children.Add(uzf_recharge_sat);
            root_sat.Children.Add(gw_inout);
            root_sat.Children.Add(stream_leakage);

            //Groundwater Discharge from SAT to Soil Zone
            MonitorItem basingw2sz_sat = new MonitorItem(BASINGW2SZ_HRU)
            {
                VariableIndex = 7,
                Group = _Out_Group,
            };
            MonitorItem sat_et = new MonitorItem(SAT_ET)
            {
                VariableIndex = 59,
                Group = _Out_Group,
            };

            root_sat.Children.Add(basingw2sz_sat);
            root_sat.Children.Add(sat_et);
            //"Total Storage Change in SAT"
            MonitorItem sat_change_stor = new MonitorItem(SAT_CHANGE_STOR)
            {
                VariableIndex = 31,
                Group = _Total_Group,
            };

            MonitorItem sat_totalin = new MonitorItem("Total SAT In")
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { uzf_recharge_sat.VariableIndex, gw_inout.VariableIndex, stream_leakage.VariableIndex }
            };
            MonitorItem sat_totalout = new MonitorItem("Total SAT Out")
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { basingw2sz_sat.VariableIndex }
            };
            AggregatedMonitorItem sat_error = new AggregatedMonitorItem("Total SAT Budget Error")
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true
            };
            sat_error.Source.AddRange(new MonitorItem[] { sat_totalin, sat_totalout, sat_change_stor });
            sat_error.SourceSign.AddRange(new int[] { -1, 1, 1 });

            root_sat.Children.Add(sat_change_stor);
            root_sat.Children.Add(sat_totalin);
            root_sat.Children.Add(sat_totalout);
            root_sat.Children.Add(sat_error);

            #endregion


            foreach (var item in root_et.Children)
            {
                item.SequenceType = SequenceType.StepbyStep;
                item.Monitor = this;
            }
            foreach (var item in root_irrigation.Children)
            {
                item.Monitor = this;
                item.SequenceType = SequenceType.StepbyStep;
            }
            foreach (var item in root_hru.Children)
            {
                item.Monitor = this;
                item.SequenceType = SequenceType.StepbyStep;
            }
            foreach (var item in root_soil.Children)
            {
                item.SequenceType = SequenceType.StepbyStep;
                item.Monitor = this;
            }
            foreach (var item in root_uzf.Children)
            {
                item.Monitor = this;
                item.SequenceType = SequenceType.StepbyStep;
            }
            foreach (var item in root_sat.Children)
            {
                item.SequenceType = SequenceType.StepbyStep;
                item.Monitor = this;
            }
            _Watcher = new CSVWatcher();
        }

        public override System.Data.DataTable Balance(ref string budget)
        {
               //ModelService.WorkDirectory
            return base.Balance(ref budget);
        }

    }

}
