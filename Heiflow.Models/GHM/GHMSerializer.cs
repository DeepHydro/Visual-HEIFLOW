// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.GHM
{
    [Serializable]
    public class GHMSerializer
    {
        public GHMSerializer()
        {
 
        }


        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }
        [XmlElement]
        public TimeReference TimeReference
        {
            get;
            set;
        }
        [XmlElement]
        public SpatialReference SpatialReference
        {
            get;
            set;
        }
        [XmlArrayItem]
        public List<RenderableModelLayer> Layers
        {
            get;
            set;
        }

        [XmlIgnore]
        public string FileName
        {
            get;
            set;
        }

        public static GHMSerializer Open(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(GHMSerializer));
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            var ghm = (GHMSerializer)xs.Deserialize(stream);
            return ghm;
        }

        public  void Save()
        {
            XmlSerializer xs = new XmlSerializer(this.GetType());
            Stream stream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            xs.Serialize(stream, this);
            stream.Close();
        }
    }
}
