// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Subsurface;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Models.Hydrodynamics.Susbed
{
    public class ModflowLoader : IModelLoader
    {
        public event EventHandler<string> LoadFailed;

        public ModflowLoader()
        {

        }

        public string FileTypeDescription
        {
            get
            {
                return "Susbed";
            }
        }

        public string Extension
        {
            get
            {
                return ".sus";
            }
        }
        public bool CanImport(IProject project)
        {
            Modflow model = new Modflow();
            return model.Exsit(project.FullProjectFileName);
        }
        public void Import(IProject project, IImportProperty property, ICancelProgressHandler progress)
        {
      
        }

        public LoadingState Load(IProject project, ICancelProgressHandler progress)
        {
            ModelService.WorkDirectory = project.FullModelWorkDirectory;
            Susbed model = new Susbed();
            model.ControlFileName = project.RelativeControlFileName;
            model.WorkDirectory = project.FullModelWorkDirectory;
            model.Project = project;
            project.Model = model;
            model.Initialize();
            var state = LoadingState.FatalError;
            try
            {
                state = model.Load(progress);
            }
            catch (Exception ex)
            {
                OnLoadFailed("Susbed 模型加载失败：" + ex.Message);
            }
            return state;
        }

        public void Clear()
        {
        }

        private void OnLoadFailed(string message)
        {
            if (LoadFailed != null)
                LoadFailed(this, message);
        }
    }
}
