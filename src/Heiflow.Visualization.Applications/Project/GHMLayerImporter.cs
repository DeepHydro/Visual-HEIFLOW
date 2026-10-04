using DotSpatial.Projections;
using Heiflow.Models.Generic;
using Heiflow.Models.GHM;
using Heiflow.Presentation.Animation;
using Heiflow.Visualization.Project;
using Heiflow.Visualization.Renderable;
using Heiflow.Visualization.Renderable.Grid;
using HUST.WREIS.Dot3D;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace Heiflow.Visualization.Applications
{
    [Export(typeof(IModelLayerImporter))]
    public class GHMLayerImporter :ModelLayerImporter
    {
        public GHMLayerImporter()
        {
            Name = "GHM";
        }

        public override void Import(Models.Generic.IBasicModel model, HUST.WREIS.Dot3D.World world)
        {
            var project = model.Project as Heiflow3DProject;
            string assembly = Path.Combine(ConfigurationManager.ApplicationPath, "Heiflow.Visualization.dll");
            var ghm = model as GHModel;
            this.ProjectService.DX3DLayers = new System.Collections.Generic.List<IDX3DLayer>();
            
            string prjfile = Path.Combine(model.Project.AbsolutePathToProjectFile, project.RelativeModelGeoPrjFile);
            ProjectionInfo srcinfo = KnownCoordinateSystems.Geographic.World.WGS1984;
            if (File.Exists(prjfile))
            {
                StreamReader sr = new StreamReader(prjfile);
                var esristr = sr.ReadLine();
                srcinfo = ProjectionInfo.FromEsriString(esristr);
                sr.Close();
            }
            GCSConverter gcs = new GCSConverter()
            {
                SourceProjection = srcinfo,
                Hemi = GCSConverter.Hemisphere.Northern
            };

            foreach (var layer in ghm.Serializer.Layers)
            {
                var render = Activator.CreateInstanceFrom(assembly, "Heiflow.Visualization.Renderable.Grid." + layer.RenderName).Unwrap() as IDX3DLayerRender;
               
                render.EquatorialRadius = world.EquatorialRadius;
                render.Owner = ghm;
                render.Project = ghm.Project;
                render.GCSConverter = gcs;

                var obj_render_layer = Activator.CreateInstanceFrom(assembly, render.SupportedLayerObject);
                var render_layer = obj_render_layer.Unwrap() as RenderableModelObject;
                render_layer.Name = layer.Name;
                render_layer.DistanceAboveSurface = layer.DistanceAboveSurface;
                render_layer.MaxDisplayAltitude = layer.MaxDisplayAltitude;
                render_layer.MinDisplayAltitude = layer.MinDisplayAltitude;
                render_layer.World = world;
                render_layer.IsOn = layer.LayerIsOn;
                render_layer.RenderObject = render;
                render.VerticalExaggeration = World.Settings.VerticalExaggeration;
                render.RenderableObject = render_layer;
                render.LoadDataSource(layer.FullDataSource);
                render.Initilize();
                _LayerService.ModelLayerGroup.Add(render_layer);
                this.ProjectService.DX3DLayers.Add(render_layer);
            }

            foreach(var pck in ghm.Packages.Values)
            {
                var ghmpck = pck as GHMPackage;
                foreach(var svar in ghmpck.StaticVariables)
                {
                    svar.Layer3D = Select(svar.RenderableModelLayer);
                }
                foreach (var svar in ghmpck.DynamicVariables)
                {
                    svar.Layer3D = Select(svar.RenderableModelLayer);
                }
            }
        }

        private IDX3DLayer Select(string  name)
        {
            var buf= from layer in this.ProjectService.DX3DLayers where layer.Name == name select  layer;
            if(buf.Any())
                return  buf.First();
            else
                return null;
        }
    }
}
