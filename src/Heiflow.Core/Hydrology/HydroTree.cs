using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Hydrology
{
    public class HydroTree
    {
        public HydroTree(int id)
        {
            ID = id;
            Nodes = new List<HydroTreeNode>();
            NodeElevChanged = false;
        }
        public int ID
        {
            get;
           private set;
        }
        public HydroTreeNode Root
        {
            get;
            set;
        }

        public HydroTreeNode Outlet
        {
            get;
            set;
        }
        /// <summary>
        /// outlet node is excluded. root node is included.
        /// </summary>
        public List<HydroTreeNode> Nodes
        {
            get;
            set;
        }

        public bool NodeElevChanged
        {
            get;
            set;
        }
    }
}
