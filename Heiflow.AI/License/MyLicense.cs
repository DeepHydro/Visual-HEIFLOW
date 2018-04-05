// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.AI
{
    public  class MyLicense
    {
        private DateTime _start = new DateTime(2017, 8, 8);
        public MyLicense()
        {

        }
        public  void Init()
        {
            if (!RegisterOperator.IsExist("VHF_COUNT"))
            {
                var str = Crypto.EncryptStringAES(_start.ToString(), GetCPUId());
                RegisterOperator.SetRegEditData("VHF_COUNT", str);
            }
        }
        public  bool Check()
        {
            if (RegisterOperator.IsExist("VHF_COUNT"))
            {
                var str=  RegisterOperator.GetRegistData("VHF_COUNT");
                var buf = Crypto.DecryptStringAES(str, GetCPUId());
                var date = DateTime.Parse(buf);
                var ts = date - _start;
                if (ts.Days > 60)
                    return false;
                else 
                    return true;
            }
            else
            {
                return false;
            }
        }
        public  string GetCPUId()
        {
            string cpuInfo = String.Empty;
            //create an instance of the Managemnet class with the
            //Win32_Processor class
            ManagementClass mgmt = new ManagementClass("Win32_Processor");
            //create a ManagementObjectCollection to loop through
            ManagementObjectCollection objCol = mgmt.GetInstances();
            //start our loop for all processors found
            foreach (ManagementObject obj in objCol)
            {
                if (cpuInfo == String.Empty)
                {
                    // only return cpuInfo from first CPU
                    cpuInfo = obj.Properties["ProcessorId"].Value.ToString();
                }
            }
            return cpuInfo;
        }
       
    }
}
