using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Presentation.Animation;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using Heiflow.Visualization.Project;
using Heiflow.Visualization.Renderable.Grid;
using HUST.WREIS.Dot3D;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Heiflow.Visualization.Applications
{
    [Export(typeof(VGSProjectService)), Export(typeof(IProjectService))]
    public class VGSProjectService : IProjectService
    {
        public event ProjectOpenedOrCreatedHandler ProjectOpenedOrCreated;

        [ImportingConstructor]
        public VGSProjectService(IProjectSerialization serial, IExplorerNodeFactory nodefac, IPEContextMenuFactory menufac)
        {
            Serializer = serial;
            NodeFactory = nodefac;
            ContextMenuFactory = menufac;
        }

        [ImportMany(typeof(IModelLayerImporter))]
        public IEnumerable<IModelLayerImporter> Importers
        {
            get;
            set;
        }

        public IProject Project
        {
            get;
            set;
        }


        public MFVectorRender RenderableMFVector
        {
            get;
            set;
        }

        public List<IDX3DLayer> DX3DLayers
        {
            get;
            set;
        }

        public void Present(IProject project, World world)
        {
            if (project.Model == null)
                return;
            foreach (var im in Importers)
            {
                if (im.Name == project.Model.Name)
                {
                    im.ProjectService = this;
                    im.Import(project.Model, world);
                    break;
                }
            }
        }


        public IProjectSerialization Serialization
        {
            get;
            set;
        }

        public IProjectSerialization Serializer
        {
            get;
            set;
        }


        public IExplorerNodeFactory NodeFactory
        {
            get;
            set;
        }

        public Presentation.Controls.IPEContextMenuFactory ContextMenuFactory
        {
            get;
            set;
        }

        public IDX3DLayer Select(string name)
        {
            var buf = from lay in DX3DLayers where lay.Name == name select lay;
            if (buf.Any())
                return buf.First();
            else
                return null;
        }

        public void RaiseProjectOpenedOrCreated(DotSpatial.Controls.IMap Map, IProject project)
        {
            if (ProjectOpenedOrCreated != null)
                ProjectOpenedOrCreated(Map, project);
        }

        public void Clear()
        {
          
        }
    }
}
