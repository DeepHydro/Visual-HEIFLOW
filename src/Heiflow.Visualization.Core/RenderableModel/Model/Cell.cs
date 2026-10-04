using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HUST.WREIS.Dot3D;
using Heiflow.Models.Generic;

namespace Heiflow.Visualization.Renderable.Grid
{
    public abstract class Cell:ICell
    {
        public Cell(int id)
        {
            ID = id;
        }

        public Cell(int i, int j)
        {
            I = i;
            J = j;
      
        }

        protected double _CurrentValue;

        public Point3d[] Points { get; set; }

        public Point3d NorthWest { get; set; }
        public Point3d NorthEast { get; set; }
        public Point3d SouthWest { get; set; }
        public Point3d SouthEast { get; set; }

        /// <summary>
        /// The grid to which this cell belongs
        /// </summary>
        public ModelRenderDX Render { get; set; }

        /// <summary>
        /// Central longitude in degree
        /// </summary>
        public double CentralLongitude { get; set; }
        /// <summary>
        /// Central latitude  in degree
        /// </summary>
        public double CentralLatitude { get; set; }
        /// <summary>
        ///    The order of longitude values in radian are as following: se,sw,nw,ne
        /// </summary>
        public double[] LongitudeRadianRect { get; set; }
        /// <summary>
        ///    The order of latitude values in radian  are as following: se,sw,nw,ne. 
        /// </summary>
        public double[] LatitudeRadianRect { get; set; }

        /// <summary>
        /// The unique ID of the cell
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        ///Row Index starting from 0
        /// </summary>
        public int I { get; set; }
        /// <summary>
        /// Column index starting from 0
        /// </summary>
        public int J { get;  set; }
        public int SerialIndex { get; set; }
 
        public float Elevation { get; set; }

        public double[] Elevations { get; set; }

        public int CurrentTimeStep { get; set; }

        public int CurrentLayerIndex { get; set; }

         
        public double CurrentValue
        {
            get
            {
                return _CurrentValue;
            }
            set
            {
                _CurrentValue = value;
            }
        }
      
        public override string ToString()
        {
            return "(" + I + "," + J + ")";
        }


        internal double GetCellValue(int l)
        {
            return 0;
        }

        public abstract float[] GetTimeSeries();
    }
}
