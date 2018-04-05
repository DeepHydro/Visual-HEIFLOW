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
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Heiflow.AI
{
    public class RegisterOperator
    {

        /**/
        /// <summary>
        /// 写入注册表
        /// </summary>
        /// <param name="strName"></param>
        public static void SetRegEditData(string strName, string strValue)
        {
            try
            {
                RegistryKey hklm = Registry.LocalMachine;
                RegistryKey software = hklm.OpenSubKey("SOFTWARE", true);
                RegistryKey aimdir = software.OpenSubKey("MySoftware", true);
                if (aimdir == null)
                    aimdir = software.CreateSubKey("MySoftware");
                aimdir.SetValue(strName, strValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        /**/
        /// <summary>
        /// 修改注册表项
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strValue"></param>
        public static void ModifyRegEditData(string strName, string strValue)
        {
            try
            {
                RegistryKey hklm = Registry.LocalMachine;
                RegistryKey software = hklm.OpenSubKey("SOFTWARE\\MySoftware", true);
                software.SetValue(strName, strValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /**/
        /// <summary>
        /// 判断指定注册表项是否存在
        /// </summary>
        /// <param name="strName"></param>
        /// <returns></returns>
        public static bool IsExist(string strName)
        {
            try
            {
                bool exit = false;
                string[] subkeyNames;
                RegistryKey hkml = Registry.LocalMachine;
                RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
                RegistryKey aimdir = software.OpenSubKey("MySoftware", true);
                if (aimdir == null)
                {
                    aimdir = software.CreateSubKey("MySoftware");
                    return false;
                }
                else
                {
                    subkeyNames = aimdir.GetValueNames();
                    foreach (string keyName in subkeyNames)
                    {
                        if (keyName == strName)
                        {
                            exit = true;
                            return exit;
                        }
                    }
                    return exit;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static string GetRegistData(string name)
        {
            string registData;
            RegistryKey hkml = Registry.LocalMachine;
            RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);
            RegistryKey aimdir = software.OpenSubKey("MySoftware", true);
            registData = aimdir.GetValue(name).ToString();
            return registData;
        }

    }
}
