// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using Heiflow.Models.Generic.Project;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Presentation.Services
{
    public delegate void ProjectOpenedOrCreatedHandler(IMap Map, IProject project);
    public interface IProjectService
    {
        event ProjectOpenedOrCreatedHandler ProjectOpenedOrCreated;
        IProject Project {get;set;}
        IProjectSerialization Serializer { get; set; }
        IExplorerNodeFactory NodeFactory { get; set; }
        IPEContextMenuFactory ContextMenuFactory { get; set; }

        void RaiseProjectOpenedOrCreated(IMap Map, IProject project);
        void Clear();
    }
}
