using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Surface.PRMS
{
    public class SoilLayer
    {
        public SoilLayer()
        {
            LayerIndex = 0;
            Name = "Soil Layer 1";
            SoilDepth = 0.4f;
            InitGVR = 0;
            InitCPR = 0.1f;
            InitPFR = 0;
        }
        public int LayerIndex
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public float SoilDepth
        {
            get;
            set;
        }
        public float InitGVR
        {
            get;
            set;
        }
        public float InitCPR
        {
            get;
            set;
        }
        public float InitPFR
        {
            get;
            set;
        }
    }
}
