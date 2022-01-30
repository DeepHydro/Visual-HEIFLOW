using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Hydrology;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation.Services;
using Heiflow.Core.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;

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
        bool filesaved = false;

        [Category("Output")]
        [Description("The inp filename")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string SegIDMapFileName
        {
            get;
            set;
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
                filesaved = false;
                var rvnet = sfr.RiverNetwork;
                sfr.NetworkToMat();
                Renumber(rvnet);
                if(filesaved)
                {
                    cancelProgressHandler.Progress("Package_Tool", 100, "The old and sorted segment ID is saved to the file: " + SegIDMapFileName);
                }
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
            filesaved = true;
            if (TypeConverterEx.IsNotNull(SegIDMapFileName))
            {
                StreamWriter sw = new StreamWriter(SegIDMapFileName);
                string line = "Old ID,New ID"; 
                sw.WriteLine(line);
                foreach(var oid in idmap.Keys)
                {
                    line = string.Format("{0},{1}", oid,idmap[oid]);
                    sw.WriteLine(line);
                }
                sw.Close();
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
