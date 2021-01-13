using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.GHM
{
    [Serializable]
    public class StaticVariable : GHMVariable
    {
        public StaticVariable()
        {
        }

    }
}