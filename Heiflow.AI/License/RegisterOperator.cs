// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
