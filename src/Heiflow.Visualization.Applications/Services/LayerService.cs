using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic.Project;
using Heiflow.Visualization.Project;
using Heiflow.Visualization.Renderable;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Applications
{
    /// <summary>
    /// This class is specially used to handle layers contained in a Project file
    /// </summary>
    [Export]
    public class LayerService
    {

        public LayerService()
        {

        }

        public RenderableSitesLayer SitesLayer
        {
            get;
            set;
        }

        public ODMSource ODMSource
        {
            get;
            set;
        }

        public RenderableObjectList Layers
        {
            get;
            set;
        }

        public RenderableObjectList ModelLayerGroup
        {
            get;
            set;
        }

        public RenderableObjectList CoverageLayerGroup
        {
            get;
            set;
        }

        public RenderableObjectList RootLayers
        {
            get;
            set;
        }

        public void Present(IProject project, World world)
        {
            var prj = project as Heiflow3DProject;
            LoadShpFile(prj, world);
            LoadRasterFile(prj, world);
            Load3DModel(prj, world);
            LoadODM(prj, world);
        }

        public void Clear()
        {
            Layers.Remove(ModelLayerGroup);
            Layers.Remove(CoverageLayerGroup);
            SitesLayer.SiteIcons.RemoveAll();
        }

        private void LoadShpFile(Heiflow3DProject prj, World world)
        {
            foreach (var ly in prj.ShapeFeatureProvider)
            {        
                string filename = "";
                ly.BaseDirectory = prj.AbsolutePathToProjectFile;
                if (File.Exists(ly.FullFileName))
                    filename = ly.FullFileName;
                else
                {
                    string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
                    filename = Path.Combine(folder, ly.RelativeFileName);
                }
                if (File.Exists(filename))
                {
                    //ly.FullFileName = filename;
                    var ro = ly.Load(filename, world);
                    ro.RenderPriority = RenderPriority.SurfaceImages;
                    CoverageLayerGroup.Add(ro);
                }
            }
            foreach (var ly in prj.FlagFeatureProvider)
            {
                string filename = "";
                ly.BaseDirectory = prj.AbsolutePathToProjectFile;
                if (File.Exists(ly.FullFileName))
                    filename = ly.FullFileName;
                else
                {
                    string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
                    filename = Path.Combine(folder, ly.RelativeFileName);
                }
                if (File.Exists(filename))
                {
                //    ly.FullFileName = filename;
                    var ro = ly.Load(filename, world);
                    ro.RenderPriority = RenderPriority.Icons;
                    CoverageLayerGroup.Add(ro);
                }
            }
        }
        private void LoadRasterFile(Heiflow3DProject prj, World world)
        {
            //foreach (var ly in prj.RenderableRasterProvider)
            //{
            //    string filename = "";
            //    if (File.Exists(ly.FullFileName))
            //        filename = ly.FullFileName;
            //    else
            //    {
            //        string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
            //        filename = Path.Combine(folder, ly.RelativeFileName);
            //    }
            //    if (File.Exists(filename))
            //    {
            //        ly.FullFileName = filename;
            //        var ro = ly.Load(filename, world);
            //        world.DataLayerList.Add(ro);
            //    }
            //}
        }
        private void Load3DModel(Heiflow3DProject prj, World world)
        {
            foreach (var ly in prj.ModelFeatureProvider)
            {
                string filename = "";
                ly.BaseDirectory = prj.AbsolutePathToProjectFile;
                if (File.Exists(ly.FullFileName))
                    filename = ly.FullFileName;
                else
                {
                    string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
                    filename = Path.Combine(folder, ly.RelativeFileName);
                }
                if (File.Exists(filename))
                {
                  //  ly.FullFileName = filename;
                    var ro = ly.Load(filename, world);
                    CoverageLayerGroup.Add(ro);
                }
            }
        }

        private void LoadODM(Heiflow3DProject prj, World world)
        {
            foreach (var ly in prj.ODMProvider)
            {
                string filename = "";
                ly.BaseDirectory = prj.AbsolutePathToProjectFile;
                if (File.Exists(ly.FullFileName))
                    filename = ly.FullFileName;
                else
                {
                    string folder = Path.GetDirectoryName(prj.AbsolutePathToProjectFile);
                    filename = Path.Combine(folder, ly.RelativeFileName);
                }
                if (File.Exists(filename))
                {
                    ODMSource source = new ODMSource();
                    string msg="";
                    if (source.Open(filename, ref msg))
                    {
                        this.ODMSource = source;
                        var sites = source.GetSites(new QueryCriteria());
                        this.SitesLayer.ShowSites(sites);
                    }
                }
            }
        }
    }
}
