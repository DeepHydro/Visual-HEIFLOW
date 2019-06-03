using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.WRM
{
    public class ManagementCycle
    {
        public ManagementCycle(int cycle, ManagemenSP sp)
        {
            Cycle = cycle;
            Owner = sp;
        }
        /// <summary>
        /// starting from 1
        /// </summary>
        public int Cycle
        {
            get;
            protected set;
        }
        public ManagemenSP Owner
        {
            get;
            protected set;
        }
        public int sw_ratio_flag
        {
            get;
            set;
        }
        public int swctrl_factor_flag
        {
            get;
            set;
        }
        public int gwctrl_factor_flag
        {
            get;
            set;
        }
        public int Withdraw_type_flag
        {
            get;
            set;
        }
        public int plantarea_flag
        {
            get;
            set;
        }
        public int max_pump_rate_flag
        {
            get;
            set;
        }
        public int max_total_pump_flag
        {
            get;
            set;
        }
        public float [] sw_ratio_annual
        {
            get;set;
        }
        public float[] swctrl_factor_annual
        {
            get; set;
        }
        public float[] gwctrl_factor_annual
        {
            get; set;
        }

        public DataCube<float> sw_ratio_day
        {
            get; set;
        }
        public DataCube<float> swctrl_factor_day
        {
            get; set;
        }
        public DataCube<float> gwctrl_factor_day
        {
            get; set;
        }
    }
}
