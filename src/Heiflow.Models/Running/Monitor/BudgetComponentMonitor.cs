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
using System.Data;

namespace Heiflow.Models.Running
{
    [Export(typeof(IFileMonitor))]
    public class BudgetComponentMonitor : FileMonitor
    {

        public BudgetComponentMonitor()
        {

            MonitorName = "BudgetComponentMonitor";

            #region ET BUDGETS
            var root_et = new MonitorItemCollection("ET Budgets");
            _Roots.Add(root_et);

            MonitorItem basinpervet = new MonitorItem(BASINPERVET_HRU)
            {
                VariableIndex = 1,
                Group = _Out_Group
            };

            MonitorItem basinimpervevap = new MonitorItem(BASINIMPERVEVAP_HRU)
            {
                VariableIndex = 2,
                Group = _Out_Group
            };

            MonitorItem basinintcpevap = new MonitorItem(BASININTCPEVAP_HRU)
            {
                VariableIndex = 3,
                Group = _Out_Group
            };

            MonitorItem basinsnowevap = new MonitorItem(BASINSNOWEVAP_HRU)
            {
                VariableIndex = 4,
                Group = _Out_Group
            };

            MonitorItem uzfet = new MonitorItem(UZF_ET)
            {
                VariableIndex = 58,
                Group = _Out_Group
            };

            MonitorItem satet = new MonitorItem(SAT_ET)
            {
                VariableIndex = 59,
                Group = _Out_Group
            };

            var lakes_et = new MonitorItem(LAKET)
            {
                VariableIndex = 51,
                Group = _Out_Group
            };

            var sfr_et = new MonitorItem(SFRET)
            {
                VariableIndex = 64,
                Group = _Out_Group
            };

            var canal_et = new MonitorItem(CANAL_ET)
            {
                VariableIndex = 70,
                Group = _Out_Group
            };

            MonitorItem et_total_in = new MonitorItem("Total ET")
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { 
                    basinpervet.VariableIndex
                    , basinimpervevap.VariableIndex
                    , basinintcpevap.VariableIndex
                    , basinsnowevap.VariableIndex
                    , uzfet.VariableIndex
                    , satet.VariableIndex
                    , lakes_et.VariableIndex
                    , sfr_et.VariableIndex
                    , canal_et.VariableIndex 
                }
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

            MonitorItem ir_totalout = new MonitorItem("Total Irrigation Out")
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { canal_drain.VariableIndex, canal_et.VariableIndex }
            };
            AggregatedMonitorItem ir_error = new AggregatedMonitorItem(HRU_ERROR)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true
            };
            ir_error.Source.AddRange(new MonitorItem[] { ir_totalin, ir_totalout, canal_ds });
            ir_error.SourceSign.AddRange(new int[] { 1, -1, -1 });

            root_irrigation.Children.Add(ir_div);
            root_irrigation.Children.Add(ir_pump);
            root_irrigation.Children.Add(ir_industry);
            root_irrigation.Children.Add(canal_drain);
            root_irrigation.Children.Add(canal_et);
            root_irrigation.Children.Add(canal_stor);
            root_irrigation.Children.Add(ir_totalin);
            root_irrigation.Children.Add(ir_totalout);
            root_irrigation.Children.Add(canal_ds);
            root_irrigation.Children.Add(ir_error);
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
            var sn_stor = new MonitorItem("Snowpack Storage")
            {
                VariableIndex = 19,
                Group = _Storage_Group
            };

            MonitorItem hru_stor = new MonitorItem(HRU_STORAGE)
            {
                VariableIndex = -1,
                Group = _Storage_Group,
                Derivable = true,
                DerivedIndex = new int[] { pc_stor.VariableIndex, im_stor.VariableIndex, sn_stor.VariableIndex }
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
            var basinlakeprecip = new MonitorItem(BasinLakePrecip)
            {
                VariableIndex = 52,
                Group = _Out_Group
            };
            // "Rejected  Gravity Drainage by UZ/SAT";
            var basinszreject = new MonitorItem(BASINSZREJECT)
            {
                VariableIndex = 25,
                Group = _In_Group
            };

            // "Slow interflow to streams";
            MonitorItem basininterflow = new MonitorItem(BASININTERFLOW)
            {
                VariableIndex = 20,
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
            MonitorItem hrubasininfil = new MonitorItem(Soil_infiltration)
            {
                VariableIndex = 39,
                Group = _Out_Group,
            };
            //"Hortonian runoff to streams"
            MonitorItem basinhortonian = new MonitorItem(Hortonian_runoff_to_streams)
            {
                VariableIndex = 41,
                Group = _Out_Group
            };

            MonitorItem hru_in = new MonitorItem(HRU_IN)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { 
                    ppt.VariableIndex
                    , ir_div.VariableIndex
                    , ir_pump.VariableIndex }
            };

            MonitorItem hru_out = new MonitorItem(HRU_OUT)
           {
              VariableIndex = -1,
              Group = _Total_Group,
              Derivable = true,
              DerivedIndex = new int[] {
                  basinimpervevap.VariableIndex
                  , hrubasininfil.VariableIndex
                  , basinintcpevap.VariableIndex
                  , basinsnowevap.VariableIndex
                  , basinhortonian.VariableIndex
                  , basinhortonianlakes.VariableIndex
                  ,basinlakeprecip.VariableIndex}
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
            hru_error.SourceSign.AddRange(new int[] { 1, -1, -1 });

            root_hru.Children.Add(ppt);
            root_hru.Children.Add(basinlakeprecip);
            root_hru.Children.Add(ir_div);
            root_hru.Children.Add(ir_pump);

            root_hru.Children.Add(basinimpervevap);
            root_hru.Children.Add(basinintcpevap);
            root_hru.Children.Add(basinsnowevap);
            root_hru.Children.Add(hrubasininfil);
            root_hru.Children.Add(basinhortonian);
            root_hru.Children.Add(basinhortonianlakes);

            root_hru.Children.Add(pc_stor);
            root_hru.Children.Add(im_stor);
            root_hru.Children.Add(sn_stor);
            root_hru.Children.Add(hru_stor);

            root_hru.Children.Add(hru_in);
            root_hru.Children.Add(hru_out);
            root_hru.Children.Add(hru_ds);
            root_hru.Children.Add(hru_error);
            #endregion

            #region SOIL ZONE BUDGETS
            var root_soil = new MonitorItemCollection("Soil Zone Water Budgets");
            _Roots.Add(root_soil);

            //"Soil infiltration"
            MonitorItem basininfil = new MonitorItem(Soil_infiltration)
            {
                VariableIndex = 39,
                Group = _In_Group,
            };
            //"Dunnian runoff to streams"
            MonitorItem basindunnian = new MonitorItem(Dunnian_runoff_to_streams)
            {
                VariableIndex = 40,
                Group = _Out_Group
            };

            MonitorItem soil_stor = new MonitorItem(Total_Soil_Zone_Storage)
            {
                VariableIndex = -1,
                Group = _Storage_Group,
                Derivable = true,
                DerivedIndex = new int[] { basinsoilmoist.VariableIndex, basingravstor.VariableIndex }
            };

            root_soil.Children.Add(basingw2sz_hru);
            root_soil.Children.Add(basininfil);
            root_soil.Children.Add(basinszreject);

            root_soil.Children.Add(basininterflow);
            root_soil.Children.Add(basinsz2gw);
            root_soil.Children.Add(basinpervet);
            root_soil.Children.Add(basindunnian);
            root_soil.Children.Add(basinlakeinsz);

            root_soil.Children.Add(basinsoilmoist);
            root_soil.Children.Add(basingravstor);
            root_soil.Children.Add(soil_stor);

            SequenceMonitorItem soil_ds = new SequenceMonitorItem(Soil_Storage_Change)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true
            };
            soil_ds.Source = soil_stor;

            MonitorItem soil_in = new MonitorItem(Soil_In)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { basingw2sz_hru.VariableIndex, basininfil.VariableIndex, basinszreject.VariableIndex }
            };

            MonitorItem soil_out = new MonitorItem(Soil_Out)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { basininterflow.VariableIndex, basinsz2gw.VariableIndex,
                basinpervet.VariableIndex, basindunnian.VariableIndex,  basinlakeinsz.VariableIndex}
            };

            AggregatedMonitorItem soil_error = new AggregatedMonitorItem(Soil_Out_Eorror)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true
            };
            soil_error.Source.AddRange(new MonitorItem[] { soil_in, soil_out, soil_ds });
            soil_error.SourceSign.AddRange(new int[] { 1, -1, -1 });

            root_soil.Children.Add(soil_in);
            root_soil.Children.Add(soil_out);
            root_soil.Children.Add(soil_ds);
            root_soil.Children.Add(soil_error);
            #endregion

            #region Streams
            var root_sfr = new MonitorItemCollection("Stream Water Budgets");

            //"Hortonian and Dunnian surface runoff to streams";
            MonitorItem basinsroff = new MonitorItem(BASINSROFF)
            {
                VariableIndex = 21,
                Group = _In_Group
            };
            // "Slow interflow to streams";
            MonitorItem basininterflow_sfr = new MonitorItem(BASININTERFLOW)
            {
                VariableIndex = 20,
                Group = _In_Group
            };

            MonitorItem sfr_ppt = new MonitorItem("Stream Precipitation")
            {
                VariableIndex = 63,
                Group = _In_Group
            };
            MonitorItem gwflow2strms = new MonitorItem(SFR_Gaining)
            {
                VariableIndex = 34,
                Group =_In_Group,
            };

            MonitorItem basinstrmflow = new MonitorItem(SFR_Outflow)
            {
                VariableIndex = 5,
                Group = _Out_Group
            };
            MonitorItem ir_div_sfr = new MonitorItem(IR_DIV)
            {
                VariableIndex = 66,
                Group = _Out_Group,
            };

            MonitorItem ir_industry_sfr = new MonitorItem( IR_Industry)
            {
                VariableIndex = 68,
                Group = _Out_Group,
            };

            MonitorItem streambed_loss = new MonitorItem(SFR_Losing)
            {
                VariableIndex = 32,
                Group = _Out_Group,
            };

            MonitorItem strm_stor = new MonitorItem(SFR_Storage)
            {
                VariableIndex = 22,
                Group = _Storage_Group,
                Derivable =true,
                DerivedIndex = new int[] {22}
            };

            MonitorItem sfr_in = new MonitorItem(SFR_In)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { 
                    basinsroff.VariableIndex
                    , basininterflow_sfr.VariableIndex
                    , sfr_inflow.VariableIndex
                    , sfr_ppt.VariableIndex
                    , gwflow2strms.VariableIndex}
            };


            MonitorItem sfr_out = new MonitorItem(SFR_Out)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true,
                DerivedIndex = new int[] { 
                     basinstrmflow.VariableIndex
                    , sfr_et.VariableIndex 
                    , ir_div_sfr.VariableIndex
                    , ir_industry_sfr.VariableIndex
                   , streambed_loss.VariableIndex}
            };

            SequenceMonitorItem sfr_ds = new SequenceMonitorItem(SFR_Storage_Change)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true
            };
            sfr_ds.Source = strm_stor;

            AggregatedMonitorItem sfr_error = new AggregatedMonitorItem(SFR_Error)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true
            };
            sfr_error.Source.AddRange(new MonitorItem[] { sfr_in, sfr_out, sfr_ds });
            sfr_error.SourceSign.AddRange(new int[] { 1, -1, -1 });

            root_sfr.Children.Add(basinsroff);
            root_sfr.Children.Add(basininterflow_sfr);
            root_sfr.Children.Add(sfr_inflow);
            root_sfr.Children.Add(sfr_ppt);
            root_sfr.Children.Add(gwflow2strms); 

            root_sfr.Children.Add(basinstrmflow);
            root_sfr.Children.Add(sfr_et);
            root_sfr.Children.Add(ir_div_sfr);
            root_sfr.Children.Add(ir_industry_sfr);
            root_sfr.Children.Add(streambed_loss);

            root_sfr.Children.Add(strm_stor);
            root_sfr.Children.Add(sfr_in);
            root_sfr.Children.Add(sfr_out);
            root_sfr.Children.Add(sfr_ds);
            root_sfr.Children.Add(sfr_error);

            _Roots.Add(root_sfr);
            #endregion

            #region Lakes
            // root_lak is added for budget statistics computation
            var root_lak = new MonitorItemCollection("Lake Water Budgets")
            {
                IsDisplay = false
            };
      
            MonitorItem basinhortonianlakes_lak = new MonitorItem("Lake_" + BASINHORTONIANLAKES)
            {
                VariableIndex = 49,
                Group = _In_Group
            };
            // "Dunnian runoff and interflow to lakes";
            MonitorItem basinlakeinsz_lak = new MonitorItem("Lake_" + BASINLAKEINSZ)
            {
                VariableIndex = 50,
                Group = _In_Group
            };
            //MonitorItem lak_gain = new MonitorItem(LAK_Gaining)
            //{
            //    VariableIndex = 38,
            //    Group = _In_Group,
            //};
            //MonitorItem basinlakeprecip_lak = new MonitorItem("Lake_" + BasinLakePrecip)
            //{
            //    VariableIndex = 52,
            //    Group = _In_Group
            //};

            //MonitorItem lak_loss = new MonitorItem(LAK_Losing)
            //{
            //    VariableIndex = 36,
            //    Group = _Out_Group,
            //};

            //MonitorItem lake_stor = new MonitorItem(LAK_Storage)
            //{
            //    VariableIndex = 23,
            //    Group = _Storage_Group,
            //    Derivable = true,
            //    DerivedIndex = new int[] { 23 }
            //};

            //MonitorItem lak_in = new MonitorItem(LAK_In)
            //{
            //    VariableIndex = -1,
            //    Group = _Total_Group,
            //    Derivable = true,
            //    DerivedIndex = new int[] { 
            //        basinhortonianlakes_lak.VariableIndex
            //        , basinlakeinsz_lak.VariableIndex
            //        , lak_gain.VariableIndex
            //        , basinlakeprecip_lak.VariableIndex}
            //};

            //MonitorItem lak_out = new MonitorItem(LAK_Out)
            //{
            //    VariableIndex = -1,
            //    Group = _Total_Group,
            //    Derivable = true,
            //    DerivedIndex = new int[] { 
            //         lak_loss.VariableIndex
            //        , lakes_et.VariableIndex}
            //};

            //SequenceMonitorItem lak_ds = new SequenceMonitorItem(LAK_Storage_Change)
            //{
            //    VariableIndex = -1,
            //    Group = _Total_Group,
            //    Derivable = true
            //};
            //lak_ds.Source = lake_stor;

            //AggregatedMonitorItem lak_error = new AggregatedMonitorItem(LAK_Error)
            //{
            //    VariableIndex = -1,
            //    Group = _Total_Group,
            //    Derivable = true
            //};
            //lak_error.Source.AddRange(new MonitorItem[] { lak_in, lak_out, lak_ds });
            //lak_error.SourceSign.AddRange(new int[] { 1, -1, -1 });

            root_lak.Children.Add(basinhortonianlakes_lak);
            root_lak.Children.Add(basinlakeinsz_lak);
            //root_lak.Children.Add(lak_gain);
            //root_lak.Children.Add(basinlakeprecip_lak);

            //root_lak.Children.Add(lak_loss);
            //root_lak.Children.Add(lakes_et);

            //root_lak.Children.Add(lake_stor);
            //root_lak.Children.Add(lak_in);
            //root_lak.Children.Add(lak_out);
            //root_lak.Children.Add(lak_ds);
            //root_lak.Children.Add(lak_error);

            _Roots.Add(root_lak);
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


            //"Recharge from UZ to SZ";
            MonitorItem uzf_recharge = new MonitorItem(UZF_RECHARGE)
            {
                VariableIndex = 10,
                Group = _Out_Group,
            };
            root_uzf.Children.Add(uzfet);
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
                DerivedIndex = new int[] { uzfet.VariableIndex, uzf_recharge.VariableIndex }
            };
            AggregatedMonitorItem uzf_error = new AggregatedMonitorItem(UZF_ERROR)
            {
                VariableIndex = -1,
                Group = _Total_Group,
                Derivable = true
            };
            uzf_error.Source.AddRange(new MonitorItem[] { uzf_totalin, uzf_totalout, uzf_del_stor });
            uzf_error.SourceSign.AddRange(new int[] { 1, -1, -1 });

            root_uzf.Children.Add(uzf_totalin);
            root_uzf.Children.Add(uzf_totalout);
            root_uzf.Children.Add(uzf_del_stor);
            root_uzf.Children.Add(uzf_error);
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

            foreach (var item in root_sfr.Children)
            {
                item.SequenceType = SequenceType.StepbyStep;
                item.Monitor = this;
            }

            foreach (var item in root_lak.Children)
            {
                item.SequenceType = SequenceType.StepbyStep;
                item.Monitor = this;
            }

            _Watcher = new CSVWatcher();
        }

        public override System.Data.DataTable Balance(string itemname, ref string report)
        {
            ClampSteps(DataSource.Values[0].Count);

            var dt = CreateBudgetTable();
            double nsteps = EndStep - StartStep + 1;
            double factor = Intevals / nsteps;
            double total_in = 0;
            double total_out = 0;
            double total_ds = 0;
            double total_diff = 0;
            double total_error = 0;

            var lines = new List<BudgetLine>();

            string ds_term = "";
            var rootitem = (from item in _Roots where item.Name == itemname select item).First();
            if (itemname == "Irrigation Budgets")
            {
                ds_term = Canal_DS;
            }
            else if (itemname == "HRU Water Budgets")
                ds_term = HRU_DS;
            else if (itemname == "Soil Zone Water Budgets")
                ds_term = Soil_Storage_Change;
            else if (itemname == "Stream Water Budgets")
                ds_term = SFR_Storage_Change;
            else if (itemname == "Unsaturated Zone Water Budgets")
                ds_term = UZF_DS;

            if (itemname == "ET Budgets")
            {
                var items = (from item in rootitem.Children where item.Group == _Out_Group select item).ToArray();
                lines.Add(BudgetLine.Section("OUT TERMS"));
                total_out = AddTermRows(items, dt, 200, factor, lines);

                report = BuildReport(itemname, lines);

                return dt;
            }
            else
            {
                var items = (from item in rootitem.Children where item.Group == _In_Group select item).ToArray();
                lines.Add(BudgetLine.Section("IN TERMS"));
                total_in = AddTermRows(items, dt, 100, factor, lines);

                items = (from item in rootitem.Children where item.Group == _Out_Group select item).ToArray();
                lines.Add(BudgetLine.Section("OUT TERMS"));
                total_out = AddTermRows(items, dt, 200, factor, lines);

                items = (from item in rootitem.Children where item.Name == ds_term select item).ToArray();
                lines.Add(BudgetLine.Section("STORAGE CHANGE TERMS"));

                // 可推导项先 Derive 再取序列
                Func<MonitorItem, IEnumerable<double>> dsSelector = item =>
                {
                    if (item.Derivable)
                        return item.Derive(item.Monitor.DataSource).Skip<double>(StartStep);
                    return item.Monitor.DataSource.Values[item.VariableIndex].Skip<double>(StartStep);
                };
                total_ds = AddTermRows(items, dt, 300, factor, lines, dsSelector);

                total_diff = total_in - total_out;
                total_error = total_diff - total_ds;

                var total_discrepancy = PercentDiscrepancy(total_in, total_out, total_ds);

                AddSummaryRows(dt, lines, total_in, total_out, total_ds, total_diff, total_error, total_discrepancy);

                report = BuildReport(itemname, lines);

                return dt;
            }
        }

    }

}