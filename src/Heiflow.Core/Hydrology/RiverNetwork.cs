//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Heiflow.Core;
using System.ComponentModel;

namespace Heiflow.Core.Hydrology
{
    public class RiverNetwork:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int mRiverCount;
        private int mReachCount;
        public RiverNetwork()
        {
            Rivers = new List<River>();
            Reaches = new List<Reach>();
            RiverJunctions = new List<HydroPoint>();
            RiverConjunctions = new List<HydroPoint>();
            Outfalls = new List<HydroPoint>();
            HydroTrees = new List<HydroTree>();
        }
        public int RiverCount 
        { 
            get
            {
                return mRiverCount;
            }
            set
            {
                mRiverCount = value;
                OnPropertyChanged("RiverCount");
            }
        }

        public int ReachCount
        {
            get
            {
                return mReachCount;
            }
            set
            {
                mReachCount = value;
                OnPropertyChanged("mReachCount");
            }
        }

        public double Offset { get; set; }

        public List<HydroPoint> RiverJunctions { get; set; }
        public List<HydroPoint> RiverConjunctions { get; set; }
        public List<HydroPoint> Outfalls { get; set; }
        public List<HydroTree> HydroTrees { get; set; }

        public List<River> Rivers
        {
            get;
            set;
        }

        public List<Reach> Reaches
        {
            get;
            set;
        }

        public void AddRiver(River river)
        {
            Rivers.Add(river);
        }

        public River GetRiver(int id)
        {
            if (id == 0)
                return null;
             var river =Rivers.Where(r => r.ID == id);
             if (river.Count() == 1)
                 return river.First();
             else
                 return null;
        }
        public Reach GetReach(int iseg, int ireach)
        {
            var river = GetRiver(iseg);
            if (river != null)
            {
                return river.GetReach(iseg, ireach);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        ///  Get reaches based on their locations
        /// </summary>
        /// <param name="rowid">ID starting from 1</param>
        /// <param name="colid">ID starting from 1</param>
        /// <returns></returns>
        public Reach[] GetReachByLocation(int rowid, int colid)
        {
            var rch = from r in Reaches where r.IRCH == rowid && r.JRCH == colid select r;
            if (rch.Any())
                return rch.ToArray();
            else
                return null;
        }
        public List<River> BuildProfile(int startID, int endID)
        {
            List<River> pro = new List<River>();
            var first = (from r in Rivers where r.ID == startID select r).First();
            pro.Add(first);
            FindDownSteam(pro, first, endID);
            var end = (from r in Rivers where r.ID == endID select r).First();
            pro.Add(end);
            return pro;
        }
        public float[] GetProfileProperty(int startID, int endID,string propname)
        {
            var profile = BuildProfile(startID, endID);
            var list = new List<float>();
            foreach(var riv in profile)
            {
                foreach(var rch in riv.Reaches)
                {
                    var vv = rch.GetType().GetProperty(propname).GetValue(rch);
                    list.Add(float.Parse(vv.ToString()));
           //         rch.TopElevation,rch.Width,rch.ROUGHCH
                }
            }
            return list.ToArray();
        }
        private void FindDownSteam(List<River> pro, River up, int endid)
        {
            if (up.Downstream != null && up.Downstream.ID != endid)
            {
                pro.Add(up.Downstream);
                FindDownSteam(pro, up.Downstream, endid);
            }
        }

        public void BuildHydroTrees()
        {
            HydroTrees.Clear();
            GetJunctions();
            int treeid = 0;

            foreach (var outlet in Outfalls)
            {
                HydroTree tree = new HydroTree(treeid);
                int nodeid = 0;
                tree.Outlet = new HydroTreeNode(nodeid)
                {
                    River = outlet.RiverObject
                };
                nodeid++;
                tree.Root = new HydroTreeNode(nodeid)
                {
                    River = outlet.RiverObject,
                    Parent = null
                };
                tree.Nodes.Add(tree.Root);
                BuildHydroTree(tree.Root, ref nodeid, tree.Nodes);
                HydroTrees.Add(tree);
                treeid++;
            }
        }

        private void BuildHydroTree(HydroTreeNode parent, ref int nodeid, List<HydroTreeNode> nodes)
        {
            if(parent.River.Upstreams.Count == 0)
            {
                parent.IsLeaf = true;
                return;
            }
            else
            {
                foreach(var river in parent.River.Upstreams)
                {
                    nodeid++;
                    HydroTreeNode node = new HydroTreeNode(nodeid)
                    {
                        River = river,
                        Parent = parent
                    };
                    parent.AddChild(node);
                    if(!nodes.Contains(node))
                        nodes.Add(node);
                    BuildHydroTree(node, ref nodeid, nodes);
                }
            }
        }

        public List<River> BuildProfile(int startID)
        {
            List<River> pro = new List<River>();
            var first = (from r in Rivers where r.ID == startID select r).First();
            FindDownSteam(pro, first);
            return pro;
        }

        private void FindDownSteam(List<River> pro, River up)
        {
            if (up.Downstream != null)
            {
                pro.Add(up.Downstream);
                FindDownSteam(pro, up.Downstream);
            }
        }

        public void RiversToSWMM(string filename, List<River> rivers)
        {
            StreamWriter sw = new StreamWriter(filename);
        
            List<HydroPoint> outfalls = new List<HydroPoint>();
            List<HydroPoint> junctions = new List<HydroPoint>();

            foreach (var river in rivers)
            {
                if (!junctions.Contains(river.InletNode))
                {
                    junctions.Add(river.InletNode);
                }
                if (!junctions.Contains(river.OutletNode))
                    junctions.Add(river.OutletNode);
            }
            foreach (var river in rivers)
            {
                if (river.Downstream == null)
                {
                    outfalls.Add(river.OutletNode);
                    if (junctions.Contains(river.OutletNode))
                        junctions.Remove(river.OutletNode);
                }
            }

            string line = "[JUNCTIONS]\n;;               Invert     Max.       Init.      Surcharge  Ponded    \n;;Name           Elev.      Depth      Depth      Depth      Area";
            sw.WriteLine(line);
            line = " ;;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);

            foreach (var junc in junctions)
            {
                if (junc != null)
                {
                    line = junc.ID + " " + junc.Coordinate.Z + " 5.0      0 .00        0          0    ";
                    sw.WriteLine(line);
                }
            }

            line = "[OUTFALLS]\n;;               Invert     Outfall    Stage/Table      Tide   \n;;Name           Elev.      Type       Time Series      Gate";
            sw.WriteLine(line);
            line = " ;;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);
            foreach (var junc in outfalls)
            {
                if (junc != null)
                {
                    line = junc.ID + " " + junc.Coordinate.Z + "    FREE                        NO    ";
                    sw.WriteLine(line);
                }
            }

            line = "[CONDUITS]\n;;               Inlet            Outlet                      Manning    Inlet      Outlet     Init.      Max.";
            sw.WriteLine(line);
            line = ";;Name           Node             Node             Length     N          Offset     Offset     Flow       Flow";
            sw.WriteLine(line);
            line = " ;;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);
            foreach (var r in rivers)
            {
                line = r.ID + " " + r.InletNode.ID + " " + r.OutletNode.ID + " " + r.Length + " "+ r.ROUGHCH+"  0      0        0          0    ";
                sw.WriteLine(line);
            }

            line = "[XSECTIONS]\n;;Link           Shape        Geom1            Geom2      Geom3      Geom4      Barrels ";
            sw.WriteLine(line);
            line = ";;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);
            foreach (var r in rivers)
            {
                line = r.ID + " RECT_OPEN  " + r.Width1 +  " 10.0      1        1         1    ";
                sw.WriteLine(line);
            }

            line = "[COORDINATES]";
            sw.WriteLine(line);
            line = ";;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);

            foreach (var junc in junctions)
            {
                line = String.Format("{0}    {1}    {2}", junc.ID, junc.Coordinate.X, junc.Coordinate.Y);
                sw.WriteLine(line);
            }
            foreach (var junc in outfalls)
            {
                line = String.Format("{0}    {1}    {2}", junc.ID, junc.Coordinate.X, junc.Coordinate.Y);
                sw.WriteLine(line);
            }
            sw.Close();

        }

        public void ReachesToSWMM(string filename, List<River> rivers)
        {
            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultSWMMSections(sw);
            List<HydroPoint> outfalls = new List<HydroPoint>();
            List<HydroPoint> junctions = new List<HydroPoint>();

            foreach (var river in rivers)
            {
                foreach (var rch in river.Reaches)
                {
                    if (junctions.Where(t => t.ID == rch.InletNode.ID).Count() == 0)
                    {
                        junctions.Add(rch.InletNode);
                    }
                    if (junctions.Where(t => t.ID == rch.OutletNode.ID).Count() == 0)
                        junctions.Add(rch.OutletNode);
                }
            }

            foreach (var river in rivers)
            {
                if (river.Downstream == null)
                {
                    outfalls.Add(river.OutletNode);
                    if (junctions.Where(t => t.ID == river.OutletNode.ID).Count() == 1)
                    {
                        junctions.Remove(river.OutletNode);
                    }
                }
            }

            string line = "[JUNCTIONS]\n;;               Invert     Max.       Init.      Surcharge  Ponded    \n;;Name           Elev.      Depth      Depth      Depth      Area";
            sw.WriteLine(line);
            line = ";;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);

            foreach (var junc in junctions)
            {
                if (junc != null)
                {
                    line = string.Format("{0}\t{1}\t{2}", junc.ID, junc.Elevation.ToString("0.00"), " 15.0\t0 .00\t0\t0");
                    sw.WriteLine(line);
                }
            }

            line = "[OUTFALLS]\n;;               Invert     Outfall    Stage/Table      Tide   \n;;Name           Elev.      Type       Time Series      Gate";
            sw.WriteLine(line);
            line = ";;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);
            foreach (var junc in outfalls)
            {
                if (junc != null)
                {
                    line = junc.ID + " " + junc.Elevation.ToString("0.00") + "     FREE                        NO    ";
                    sw.WriteLine(line);
                }
            }

            line = "[CONDUITS]\n;;               Inlet            Outlet                      Manning    Inlet      Outlet     Init.      Max. ";
            sw.WriteLine(line);
            line = ";;Name           Node             Node             Length     N          Offset     Offset     Flow       Flow      ";
            sw.WriteLine(line);
            line = ";;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);
            foreach (var r in rivers)
            {
                foreach (var rch in r.Reaches)
                {
                    line = rch.ID + " " + rch.InletNode.ID + " " + rch.OutletNode.ID + " " + rch.Length.ToString("0.00") + " " + rch.ROUGHCH + "  0      0        0          0    ";
                    sw.WriteLine(line);
                }
            }

            line = "[XSECTIONS]\n;;Link           Shape        Geom1            Geom2      Geom3      Geom4      Barrels";
            sw.WriteLine(line);
            line = ";;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);
            foreach (var r in rivers)
            {
                foreach (var rch in r.Reaches)
                {
                    line = rch.ID + " RECT_OPEN  10.0  " + rch.Width + "     1        1          1    ";
                    sw.WriteLine(line);
                }
            }

            line = "[COORDINATES]";
            sw.WriteLine(line);
            line = ";;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);

            foreach (var junc in junctions)
            {
                line = String.Format("{0}\t{1}\t{2}", junc.ID, junc.Coordinate.X, junc.Coordinate.Y);
                sw.WriteLine(line);
            }
            foreach (var junc in outfalls)
            {
                line = String.Format("{0}\t{1}\t{2}", junc.ID, junc.Coordinate.X, junc.Coordinate.Y);
                sw.WriteLine(line);
            }
            sw.Close();
        }

        public void NetworkToSWMM(string filename, List<River> rivers)
        {
            ReachesToSWMM(filename, Rivers);
        }

        public void NetworkToSWMM(string filename)
        {
            ReachesToSWMM(filename, Rivers);
        }

        private void WriteDefaultSWMMSections(StreamWriter sw)
        {
            sw.WriteLine(Resource.SWMMSections);
        }
        public void Export2SFR(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            sw.WriteLine(Resource.SFRHead);
            string newline = "";
            //# Data Set 2: KRCH IRCH JRCH ISEG IREACH RCHLEN STRTOP SLOPE STRTHICK STRHC1 THTS THTI EPS IFACE Defined by object: reach_1
            string format = "";
            for(int i=0; i<14;i++)
            {
                format += "{" + i + "}\t";
            }
            foreach (var river in Rivers)
            {
                foreach (var reach in river.Reaches)
                {
                    reach.Slope = reach.Slope < 0.0001 ? 0.0001 : reach.Slope;
                    newline = string.Format(format, reach.KRCH, reach.IRCH, reach.JRCH, reach.ISEG, reach.IREACH, reach.Length.ToString("0.0"), reach.TopElevation.ToString("0.00"), reach.Slope.ToString("0.0000"),
                        reach.BedThick,reach.STRHC1.ToString("0.00"),reach.THTS,reach.THTI,reach.EPS,reach.IFACE);
                    sw.WriteLine(newline);
                }
            }
            foreach (var river in Rivers)
            {
                //newline = string.Format();
                //sw.WriteLine(newline);
            }
            sw.Write(Resource.SFRSections);
            sw.Close();
        }

        public void GetUpRivers(int outID,  List<River> rivers)
        {
            var riv= GetRiver(outID);
            if(riv.Upstreams.Count >0)
            {
                rivers.Add(riv);
                foreach(var r in riv.Upstreams)
                {
                    GetUpRivers(r.ID, rivers);
                }
            }
            else
            {
                rivers.Add(riv);
            }
        }

        public void GetJunctions()
        {
            RiverJunctions.Clear();
            Outfalls.Clear();
            RiverConjunctions.Clear();

            foreach (var river in Rivers)
            {
                foreach (var rch in river.Reaches)
                {
                    if (RiverJunctions.Where(t => t.ID == rch.InletNode.ID).Count() == 0)
                        RiverJunctions.Add(rch.InletNode);
                    if (RiverJunctions.Where(t => t.ID == rch.OutletNode.ID).Count() == 0)
                        RiverJunctions.Add(rch.OutletNode);
                }
            }

            foreach (var river in Rivers)
            {
                if (river.Downstream == null)
                {
                    Outfalls.Add(river.OutletNode);
                    if (RiverJunctions.Where(t => t.ID == river.OutletNode.ID).Count() == 1)
                    {
                        RiverJunctions.Remove(river.OutletNode);
                        RiverConjunctions.Add(river.OutletNode);
                    }
                }
            }
        }

        public int GetReachCount()
        {
            if (Rivers != null)
                return (from r in Rivers select r.Reaches.Count).Sum();
            else
                return 0;
        }

        public int GetRiverCount()
        {
            if (Rivers != null)
                return Rivers.Count;
            else
                return 0;
        }

        private void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
