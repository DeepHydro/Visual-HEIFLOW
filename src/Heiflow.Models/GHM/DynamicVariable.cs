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
    public class DynamicVariable
    {
        public DynamicVariable()
        {
            Name = "Dynamic variable";
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
    }
}
