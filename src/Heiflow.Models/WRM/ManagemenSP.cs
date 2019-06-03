using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.WRM
{
    public class ManagemenSP
    {
        private ManagementObject[] _Farms;
        private ManagementObject[] _Industry;
        private ManagementCycle[] _FarmCyles;
        private ManagementCycle[] _IndustryCyles;
        public ManagemenSP( int sp)
        {
            StressPeriod = sp;
        }

        /// <summary>
        /// starting from 1
        /// </summary>
        public int StressPeriod
        {
            get;
            protected set;
        }

        public ManagementObject[] Farms
        {
            get
            {
                return _Farms;
            }
            set
            {
                _Farms = value;
            }
        }
        public ManagementObject[] Industries
        {
            get
            {
                return _Industry;
            }
            set
            {
                _Industry = value;
            }
        }
        public ManagementCycle[] FarmCycles
        {
            get
            {
                return _FarmCyles;
            }
            set
            {
                _FarmCyles = value;
            }
        }
        public ManagementCycle[] IndustryCycles
        {
            get
            {
                return _IndustryCyles;
            }
            set
            {
                _IndustryCyles = value;
            }
        }
        public DataCube<float> Quota
        {
            get;set;
        }
        public int[] QuotaFlag
        {
            get; set;
        }
    }
}
