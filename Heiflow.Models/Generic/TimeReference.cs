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
using System.Xml;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic
{
    [Serializable]
    public class TimeReference
    {
        public TimeReference()
        {
            TimeStep = 86400;
        }
        [XmlElement]
        public DateTime Start
        {
            get;
            set;
        }
        [XmlElement]
        public DateTime End
        {
            get;
            set;
        }
        /// <summary>
        /// Unit is in second
        /// </summary>
        [XmlElement]
        public int TimeStep
        {
            get;
            set;
        }
    }
}
