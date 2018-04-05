// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
namespace Heiflow.Models.Generic.Project
{
    public interface IProjectSerialization
    {
        event EventHandler<bool> ProjectOpened;
        event EventHandler<string> OpenFailed;

        DotSpatial.Controls.AppManager App { get; set; }    
        IProject CurrentProject { get; }
        bool HasError { get; }
        IEnumerable<IOpenProjectFileProvider> OpenProjectFileProviders { get; }
        IEnumerable<ISaveProjectFileProvider> SaveProjectFileProviders { get; }
        IEnumerable<IProject> SurpportedProjects { get; set; }
        IEnumerable<IModelLoader> SurpportedModelLoaders { get; set; }
        void Open(string fileName,IProgress progress);
        void Save(string fileName, Heiflow.Models.Generic.Project.IProject project);
        void Save(Heiflow.Models.Generic.Project.IProject project);
        bool New(string prjName, string prjDic, Heiflow.Models.Generic.Project.IProject project, IProgress progress, bool from_exit_model);
    }
}
