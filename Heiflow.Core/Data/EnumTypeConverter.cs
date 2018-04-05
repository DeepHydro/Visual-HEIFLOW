// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;

namespace Heiflow.Core.Data
{
    /// <summary>
    /// EnumConverter supporting System.ComponentModel.DescriptionAttribute
    /// </summary>
    public class EnumDescConverter : System.ComponentModel.EnumConverter
    {
        protected System.Type myVal;

        /// <summary>
        /// Gets Enum Value's Description Attribute
        /// </summary>
        /// <param name="value">The value you want the description attribute for</param>
        /// <returns>The description, if any, else it's .ToString()</returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
              (DescriptionAttribute[])fi.GetCustomAttributes(
              typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Gets the description for certaing named value in an Enumeration
        /// </summary>
        /// <param name="value">The type of the Enumeration</param>
        /// <param name="name">The name of the Enumeration value</param>
        /// <returns>The description, if any, else the passed name</returns>
        public static string GetEnumDescription(System.Type value, string name)
        {
            FieldInfo fi = value.GetField(name);
            DescriptionAttribute[] attributes =
              (DescriptionAttribute[])fi.GetCustomAttributes(
              typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : name;
        }

        /// <summary>
        /// Gets the value of an Enum, based on it's Description Attribute or named value
        /// </summary>
        /// <param name="value">The Enum type</param>
        /// <param name="description">The description or name of the element</param>
        /// <returns>The value, or the passed in description, if it was not found</returns>
        public static object GetEnumValue(System.Type value, string description)
        {
            FieldInfo[] fis = value.GetFields();
            foreach (FieldInfo fi in fis)
            {
                DescriptionAttribute[] attributes =
                  (DescriptionAttribute[])fi.GetCustomAttributes(
                  typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    if (attributes[0].Description == description)
                    {
                        return fi.GetValue(fi.Name);
                    }
                }
                if (fi.Name == description)
                {
                    return fi.GetValue(fi.Name);
                }
            }
            return description;
        }

        public EnumDescConverter(System.Type type)
            : base(type.GetType())
        {
            myVal = type;
        }

      
    }
}