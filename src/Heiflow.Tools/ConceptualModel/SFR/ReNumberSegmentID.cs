using Heiflow.Applications;
using Heiflow.Core.Hydrology;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.ConceptualModel.SFR
{
    public class ReNumberSegmentID : MapLayerRequiredTool
    {
        public ReNumberSegmentID()
        {
            Name = "Renumber Segment ID";
            Category = Cat_CMG;
            SubCategory = "SFR";
            Description = "Renumber Segment ID based on distance to outlets";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
        }

        public override void Initialize()
        {
            Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;

            if (sfr != null)
            {
                var rvnet = sfr.RiverNetwork;
                sfr.NetworkToMat();
                Renumber(rvnet);
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 90, "SFR package not loaded.");
                return false;
            }
        }

        private void Renumber(RiverNetwork net)
        {
            foreach (var river in net.Rivers)
            {
                double length = 0;
                FindLengthToOutlet(net, river, ref length);
                river.DistanceToOutlet = length;
            }
            var ordered = net.Rivers.OrderByDescending(x => x.DistanceToOutlet);
            Dictionary<int, int> idmap = new Dictionary<int, int>();
            for (int i = 0; i < ordered.Count(); i++)
            {
                var river = ordered.ElementAt(i);
                river.OrderedID = i + 1;
                idmap.Add(river.ID, river.OrderedID);
            }
            for (int i = 0; i < ordered.Count(); i++)
            {
                var river = ordered.ElementAt(i);
                river.ID = i + 1;
                if (river.OutRiverID > 0)
                    river.OutRiverID = idmap[river.OutRiverID];
                foreach (var rch in river.Reaches)
                {
                    rch.ISEG = river.ID;
                }
            }
        }

        private double FindLengthToOutlet(RiverNetwork net, River river, ref double len)
        {
            if (river.Downstream != null)
            {
                len = len + river.Length;
                return FindLengthToOutlet(net, river.Downstream, ref len);
            }
            else
            {
                len = len + river.Length;
                return len;
            }
        }
    }
}
