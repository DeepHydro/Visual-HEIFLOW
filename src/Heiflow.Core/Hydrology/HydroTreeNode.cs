using GeoAPI.Geometries;
using Heiflow.Core.Hydrology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Hydrology
{
    public  class HydroTreeNode
    {
        public HydroTreeNode(int id)
        {
            ID = id;
            IsLeaf = false;
            Children = new List<HydroTreeNode>();
            ElveCache = new List<double>();
        }
        public int ID
        {
            get;
            private set;
        }
        public Coordinate Coordinate
        {
            get;
            set;
        }
        public double BedElevation
        {
            get;
            set;
        }
        public double SurfaceElevation
        {
            get;
            set;
        }
        public double Depth
        {
            get;
            set;
        }
        public double Slope
        {
            get;
            set;
        }
        public River River
        {
            get;
            set;
        }
        public HydroTreeNode Parent
        {
            get;
            set;
        }
        public List<HydroTreeNode> Children
        {
            get;
            set;
        }
        public int ChildrenCount
        {
            get
            {
                return Children.Count;
            }
        }

        public bool IsLeaf
        {
            get;
            set;
        }

        public object Tag
        {
            get;
            set;
        }
        /// <summary>
        /// 0: normal; 1: lower than downstrea; 2: greater than surface elevation
        /// </summary>
        public int ElevationFlag
        {
            get;
            set;
        }
        public void AddChild(HydroTreeNode node)
        {
            if (!Children.Contains(node))
                Children.Add(node);
        }

        public List<double> ElveCache
        {
            get;
            set;
        }

        public double Length { get; set; }
    }
}
