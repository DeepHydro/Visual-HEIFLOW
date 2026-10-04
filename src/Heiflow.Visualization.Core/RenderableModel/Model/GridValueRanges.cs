using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable.Grid
{
    public class GridValueRanges
    {
        public GridValueRanges(int timeCycles)
        {
            VelocityRange = new double[timeCycles, 2];
            HorizonalVelocityRange = new double[timeCycles, 2];
            VerticalVelocityRange = new double[timeCycles, 2];
        }
        /// <summary>
        /// A two-dimentional array with size (time cycle) * 2, each row store 
        /// (Maximum,Minimum) pair at corresponding time step
        /// </summary>
        public double[,] VelocityRange { get; set; }

        public double[,] HorizonalVelocityRange { get; set; }

        public double[,] VerticalVelocityRange { get; set; }
    }

}
