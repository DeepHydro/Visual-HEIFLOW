// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic.Attributes
{
    [Serializable]
    public class ArealPropertyInfo
    {
        [XmlElement]
        public string PropertyName { get; set; }
        [XmlElement]
        public string TypeName { get; set; }
        [XmlElement]
        public object DefaultValue { get; set; }
        [XmlElement]
        public string ParameterName { get; set; }
        [XmlIgnore]
        public IParameter Parameter { get; set; }
        [XmlIgnore]
        public bool IsParameter
        {
            get
            {
                return ParameterName != "null";
            }
        }
        [XmlIgnore]
        public string AliasName
        {
            get
            {
                if (IsParameter)
                    return ParameterName;
                else
                    return PropertyName;
            }
            set
            {
                if (IsParameter)
                    ParameterName = value;
                else
                    PropertyName = value;
            }
        }
    }

   
    public class ArealProperty : Attribute
    {
        public ArealProperty(Type type, object defaultvalue)
        {
            ElementType = type;
            DefaultValue = defaultvalue;
        }

        public Type ElementType { get; protected set; }

        public object DefaultValue { get; set; }
    }
}
