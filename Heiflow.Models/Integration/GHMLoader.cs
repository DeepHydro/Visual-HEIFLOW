// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.GHM;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Integration
{
    public class GHMLoader : IModelLoader
    {
        public string FileTypeDescription
        {
            get
            {
                return "General Hydrological Model";
            }
        }

        public string Extension
        {
            get
            {
                return ".ghm";
            }
        }

        public bool CanImport(IProject project)
        {
            return true;
        }

        public void Import(IProject project, IImportProperty property, IProgress progress)
        {
            GHModel model = new GHModel();
            model.Project = project;
            model.Load(progress);
        }

        public bool Load(IProject project, IProgress progress)
        {
            var succ = true;
            GHModel model = new GHModel();
            succ= model.Load( progress);
            succ = model.LoadGrid(progress);
            return succ;
        }
        
    }
}