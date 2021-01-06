using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.GHM
{
    [Serializable]
    public class StaticVariable
    {
        public StaticVariable()
        {
            Name = "Static variable";
        }
        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }
        [Browsable(false)]
        [XmlAttribute]
        public string RenderableModelLayerName
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Source
        {
            get;
            set;
        }
        [Browsable(false)]
        [XmlIgnore]
        public GHMPackage Parent
        {
            get;
            set;
        }
        [XmlIgnore]
        [StaticVariableItem("Layer")]
        [Browsable(false)]
        public DataCube<float> DataSource
        {
            get;
            set;
        }
    }
}
