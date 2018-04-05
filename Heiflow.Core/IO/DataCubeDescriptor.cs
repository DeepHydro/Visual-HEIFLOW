// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Heiflow.Core.IO
{
    [Serializable]
    public class DataCubeDescriptor
    {
        public DataCubeDescriptor()
        {

        }
        [XmlElement]
        public int NVar { get; set; }
        [XmlElement]
        public int NTimeStep { get; set; }
        [XmlElement]
        public int NCell { get; set; }

        [XmlArrayItem]
        public DateTime[] TimeStamps { get; set; }
        [XmlArrayItem]
        public double[] XCoor { get; set; }
        [XmlArrayItem]
        public double[] YCoor { get; set; }

        public static void Serialize(string filename, DataCubeDescriptor descriptor)
        {
            XmlSerializer xs = new XmlSerializer(typeof(DataCubeDescriptor));
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            xs.Serialize(stream, descriptor);
            stream.Close();
        }

        public static DataCubeDescriptor Deserialize(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(DataCubeDescriptor));
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            var descriptor = (DataCubeDescriptor)xs.Deserialize(stream);
            stream.Close();
            return descriptor;
        }
    }
}
