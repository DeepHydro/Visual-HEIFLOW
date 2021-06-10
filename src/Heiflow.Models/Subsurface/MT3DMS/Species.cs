using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface.MT3DMS
{
    public class Species
    {
        public Species()
        {
            Selected = false;
            Name = "Species";
            InitialConcentration = 0.001f;
            LonNum = 2;
        }

        public string Name
        {
            get;
            set;
        }

        public bool Selected
        {
            get;
            set;
        }

        public float InitialConcentration
        {
            get;
            set;
        }

        public int LonNum
        {
            get;
            set;
        }
    }
}
