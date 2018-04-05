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

namespace Heiflow.Models.GHM
{
    [Serializable]
    public class Member
    {
        public Member()
        {
            Spatial = new List<Item>();
            Spatiotemporal = new List<TimeVariantItem>();
            TimeSeries = new List<TimeVariantItem>();
        }



        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        [XmlArrayItem]
        public List<Item> Spatial
        {
            get;
            set;
        }

        [XmlArrayItem]
        public List<TimeVariantItem> Spatiotemporal
        {
            get;
            set;
        }

        [XmlArrayItem]
        public List<TimeVariantItem> TimeSeries
        {
            get;
            set;
        }

        [XmlIgnore]
        public List<Item>Items
        {
            get
            {
                List<Item> list = new List<Item>();
                list.AddRange(Spatial);
                list.AddRange(Spatiotemporal);
                list.AddRange(TimeSeries);
                return list;
            }
        }

    }
}
 