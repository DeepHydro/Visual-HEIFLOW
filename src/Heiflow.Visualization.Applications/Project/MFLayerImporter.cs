using DotSpatial.Projections;
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using Heiflow.Presentation.Animation;
using Heiflow.Visualization.Project;
using Heiflow.Visualization.Renderable;
using Heiflow.Visualization.Renderable.Grid;
using HUST.WREIS.Dot3D;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Project
{
    [Export(typeof(IModelLayerImporter))]
    public class MFLayerImporter : ModelLayerImporter
    {

        private List<IDX3DLayer> _supported_layers;
        private double _DistanceAbove = 0;
        public MFLayerImporter()
        {
            Name = "Modflow";
            _supported_layers = new List<IDX3DLayer>();
        }

        public IDX3DLayer this[string layer_name]
        {
            get
            {
                var buf = from ll in _supported_layers where ll.Name == layer_name select ll;
                if (buf.Any())
                    return buf.First();
                else
                    return null;
            }
        }

        public void Initialize(World world)
        {
            _supported_layers.Clear();
            var renderableGridLayer = new RegularGridLayer("Grid", world, 0, 10000, _DistanceAbove)
            {
                Token = "RegularGrid"
            };
            var mfrender = new MFGridRender(world);
            renderableGridLayer.RenderDX = mfrender;
            _supported_layers.Add(renderableGridLayer);
            this.ProjectService.DX3DLayers = _supported_layers;

            var riverGridLayer = new RegularGridLayer("River Network", world, 0, 10000, _DistanceAbove)
            {
                Token = "SFR"
            };
            riverGridLayer.RenderDX = new RiverGridRender(world);
            _supported_layers.Add(riverGridLayer);

            var velGridLayer = new DXVectorLayer("Velocity Field", world, 0, 10000, _DistanceAbove);
            var mfvector = new MFVectorRender(world);
            mfvector.Owner = _VGSProjectService.Project.Model;
            velGridLayer.RenderDX = mfvector;
            _supported_layers.Add(velGridLayer);
            _VGSProjectService.RenderableMFVector = mfvector;

            GridProfileLayer profile = new GridProfileLayer("Profile", world, 0, 10000, _DistanceAbove)
            {
                IsUsed = true,
                Token = "Profile"
            };
            profile.RenderDX = new ProfileRender(world);
            _supported_layers.Add(profile);

            DXMFWellLayer fhb_layer = new DXMFWellLayer("FHB", world, 0, 10000, _DistanceAbove)
            {
                Token = "FHB"
            };
            fhb_layer.RenderDX = new MFFHBRender(world)
            {
                Scale = 0.2f,
                MaxBarHeight = 5000
            };
            _supported_layers.Add(fhb_layer);

            DXMFWellLayer chd_layer = new DXMFWellLayer("CHD", world, 0, 10000, _DistanceAbove)
            {
                Token = "CHD"
            };
            chd_layer.RenderDX = new MFCHDRender(world)
            {
                Scale = 0.2f,
                MaxBarHeight = 5000
            };
            _supported_layers.Add(chd_layer);

            DXMFWellLayer wel_layer = new DXMFWellLayer("WEL", world, 0, 10000, _DistanceAbove)
                        {
                            Token = "Well"
                        };
            wel_layer.RenderDX = new MFWellRender(world)
            {
                EquatorialRadius = world.EquatorialRadius,
                Scale = 0.0f,
                MaxBarHeight = 5000
            };
            _supported_layers.Add(wel_layer);
        }

        public override void Import(Models.Generic.IBasicModel model, World world)
        {
            var project = model.Project as Heiflow3DProject;
            string prjfile = Path.Combine( model.Project.AbsolutePathToProjectFile, project.RelativeModelGeoPrjFile);
            ProjectionInfo srcinfo = KnownCoordinateSystems.Geographic.World.WGS1984;
            if(File.Exists(prjfile))
            {
                StreamReader sr = new StreamReader(prjfile);
                var esristr = sr.ReadLine();
                srcinfo = ProjectionInfo.FromEsriString(esristr);
                sr.Close();
            }
            GCSConverter gcs = new GCSConverter()
            {
                SourceProjection= srcinfo,
                Hemi = GCSConverter.Hemisphere.Northern
            };

            foreach (var pck in model.Packages.Values)
            {
                foreach (var layer in _supported_layers)
                {
                    if (layer.Token == pck.Layer3DToken)
                    {
                        layer.AddPackage(pck);
                        pck.Layer3D = layer;
                        layer.IsUsed = true;
                    }
                }
                foreach (var ch in pck.Children)
                {
                    foreach (var layer in _supported_layers)
                    {
                        if (layer.Token == ch.Layer3DToken)
                        {
                            layer.AddPackage(ch);
                            ch.Layer3D = layer;
                            layer.IsUsed = true;
                        }
                    }
                }
            }

            foreach (IDX3DLayer layer in _supported_layers)
            {
                if (layer.IsUsed)
                {
                    layer.Project = project;
                    layer.RenderDX.GCSConverter = gcs;
                    layer.RenderDX.DistanceAboveSurface = project.DistanceAboveSurface;
                    layer.RenderDX.Initilize();
                }
            }
            foreach (RenderableModelObject layer in _supported_layers)
            {
                if (layer.IsUsed)
                {
                    _LayerService.ModelLayerGroup.Add(layer);
                }
            }
        }

    }
}
