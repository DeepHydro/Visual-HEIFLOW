// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.IO
{
    public  class SecurityFile
    {
        private string _FileName;

        public SecurityFile(string filename)
        {
            _FileName = filename;
            ExpiredDate = DateTime.Now.AddMonths(3);
        }

        public DateTime Date { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string AuthenticationCode { get; set; }
        public bool Authenticated { get; set; }
        public string Mac { get; protected set; }

        public void Update(string product_name)
        {
            FileStream fs = new FileStream(_FileName, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(product_name);
            bw.Write(Mac);
            bw.Write(Date.ToString("yyyy-MM-dd hh:mm:ss"));
            bw.Write(AuthenticationCode);
            bw.Write(Authenticated);
            bw.Close();
            fs.Close();
        }

        public void Activate(string product_name)
        {
            Mac = GetFirstMacAddress();
            FileStream fs = new FileStream(_FileName, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(product_name);
            bw.Write(Mac);
            bw.Write(Date.ToString("yyyy-MM-dd hh:mm:ss"));
            bw.Write(AuthenticationCode);
            bw.Write(Authenticated);
            bw.Close();
            fs.Close();
        }

        public void Read()
        {
            FileStream fs = new FileStream(_FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(fs);
            var str = br.ReadString();
            Mac = br.ReadString();
            str = br.ReadString();
            Date = DateTime.Parse(str);
            AuthenticationCode = br.ReadString();
            Authenticated = br.ReadBoolean();
            br.Close();
            fs.Close();
        }

        public void Validate()
        {
            this.Read();
            if (Authenticated)
            {
                if (Date > ExpiredDate)
                {
                    Authenticated = false;
                }
            }
            if (GetFirstMacAddress() != Mac)
                Authenticated = false;
        }

        public string Convert(string code)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(code, "MD5");
        }

        public static void Generate(string filename, string code, string product_name)
        {
            SecurityFile sf = new SecurityFile(filename);
            sf.Mac = GetFirstMacAddress();
            sf.Date = DateTime.Now;
            sf.AuthenticationCode = sf.Convert(code);
            sf.Authenticated = false;
            sf.Update(product_name);
        }

        /// <summary>
        /// Finds the MAC address of the first operation NIC found.
        /// </summary>
        /// <returns>The MAC address.</returns>
        public static  string GetFirstMacAddress()
        {
            string macAddresses = string.Empty;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    Console.WriteLine(macAddresses);
                    break;
                }
            }

            return macAddresses;
        }
    }
}
