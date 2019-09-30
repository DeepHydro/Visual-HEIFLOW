using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic.Parameters
{
    [Serializable]
    public class SerializableParameter
    {
        public SerializableParameter()
        {

        }
        [XmlElement]
        public  string Name
        {
            get;
            set;
        }
        [XmlElement]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 0 is for short; 1 is for integer;2 is for real; 3 is for double; 4 is for character string; 5 is for object; 6 is for bool
        /// </summary>
        /// 
        [XmlElement]
        public int ValueType
        {
            get;
            set;
        }
        [XmlElement]
        [Browsable(false)]
        public ParameterType VariableType
        {
            get;
            set;
        }

        [XmlElement]
        public int Dimension
        {
            get;
            set;
        }
        [XmlElement]
        [Browsable(false)]
        public string[] DimensionNames
        {
            get;
            set;
        }
        [XmlElement]
        public double DefaultValue
        {
            get;
            set;
        }
        [XmlElement]
        [Browsable(true)]
        public Modules ModuleName
        {
            get;
            set;
        }
        [XmlElement]
        [Browsable(true)]
        public float Maximum
        {
            get;
            set;
        }
        [XmlElement]
        [Browsable(true)]
        public float Minimum
        {
            get;
            set;
        }
        [XmlElement]
        [Browsable(true)]
        public string Units
        {
            get;
            set;
        }
    }
}
