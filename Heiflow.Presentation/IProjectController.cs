// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Waf.Applications;
namespace Heiflow.Presentation
{
    public interface IProjectController
    {
        DotSpatial.Controls.AppManager MapAppManager { get; set; }
        Heiflow.Models.Generic.Project.IProject Project { get; }
        DelegateCommand New { get; }
        DelegateCommand Import { get; }
        DelegateCommand Open { get; }
        DelegateCommand Save { get; }
        DelegateCommand Close { get; }

        IProjectService ProjectService { get; }
        IShellService ShellService { get; }
        IActiveDataService ActiveDataService { get; }
        IEnumerable<IModelTool> Tools { get; set; }
        void OpenProject(object fn);
        void Initialize();
        void Shutdown();

    }
}
