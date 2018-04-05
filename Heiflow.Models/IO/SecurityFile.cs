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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

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
