// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Heiflow.Models.UI;

namespace Heiflow.Models.Integration
{
    public class HeiflowLoader : IModelLoader
    {
        public HeiflowLoader()
        {

        }

        public string FileTypeDescription
        {
            get
            {
                return "Heiflow Model";
            }
        }

        public string Extension
        {
            get
            {
                return ".control";
            }
        }

        public bool CanImport(IProject project)
        {
            HeiflowModel model = new HeiflowModel();
            return model.Exsit(project.RelativeControlFileName);
        }

        public void Import(IProject project, IImportProperty property, IProgress progress)
        {
            var succ = true;
            ModelService.WorkDirectory = project.FullModelWorkDirectory;
            if (project.Model == null)
            {
                project.Model = new HeiflowModel();
                project.Model.Project = project;
            }
            else
            {
                project.Model.Clear();
            }
            var  model = project.Model as HeiflowModel;
            model.Project = project;
            model.WorkDirectory = project.FullModelWorkDirectory;
            model.ControlFileName = project.RelativeControlFileName;
            model.Initialize();
            model.Grid.Origin = new GeoAPI.Geometries.Coordinate(property.OriginX, property.OriginY);
           succ = model.Load(progress);
            if (succ)
            {
                model.ModflowModel.Grid.Projection = property.Projection;
            }
        }

        public bool Load( IProject project, IProgress progress)
        {
            ModelService.WorkDirectory = project.FullModelWorkDirectory;
            HeiflowModel model = new HeiflowModel();
            model.ControlFileName = project.RelativeControlFileName;
            model.WorkDirectory = project.FullModelWorkDirectory;
            model.Project = project;
            model.Initialize();
            return model.Load( progress);
        }
    }
}
