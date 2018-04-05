// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Modeling.Forms;
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Tools
{
    public interface IModelTool : ITool
    {
        bool Initialized { get; }
        bool MultiThreadRequired { get; }
        IModelWorkspace Workspace { get; set; }
        IWorkspaceView WorkspaceView { get; set; }

        /// <summary>
        /// do something to setup enviroment
        /// </summary>
        void Setup();
        /// <summary>
        /// bind project service to the tool object
        /// </summary>
        /// <param name="en"></param>
        void BindProjectService(object en);
        /// <summary>
        /// call after execution finished
        /// </summary>
        /// <param name="args"></param>
        void AfterExecution(object args);
    }
}
