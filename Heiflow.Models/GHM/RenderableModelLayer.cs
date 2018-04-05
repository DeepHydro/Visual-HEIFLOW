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

namespace Heiflow.Models.GHM
{
    [Serializable]
    public class RenderableModelLayer
    {
        public RenderableModelLayer()
        {
            Members = new List<Member>();
        }

        [XmlArrayItem]
        public List<Member> Members { get; set; }


        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute]
        public string RenderName
        {
            get;
            set;
        }
        [XmlElement]
        public double DistanceAboveSurface
        {
            get;
            set;
        }
            [XmlElement]
        public double MaxDisplayAltitude
        {
            get;
            set;
        }
            [XmlElement]
        public double MinDisplayAltitude
        {
            get;
            set;
        }
    }
}
