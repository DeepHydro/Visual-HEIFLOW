// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic.Project;
using Heiflow.Presentation.Controls;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Applications.Services
{
    [Export(typeof(IProjectService))]
    public class ProjectService : IProjectService
    {
        public event ProjectOpenedOrCreatedHandler ProjectOpenedOrCreated;
        private IProject _Project;

        [ImportingConstructor]
        public ProjectService(IProjectSerialization serial, IExplorerNodeFactory nodefac,IPEContextMenuFactory menufac )
        {
            Serializer = serial;
            NodeFactory = nodefac;
            ContextMenuFactory = menufac;
        }

        public IProject Project
        {
            get
            {
                Debug.WriteLine("Project");
                return _Project;
            }
            set
            {
                _Project = value;
            }
        }

        public IProjectSerialization Serializer
        {
            get;
            set;
        }


        public IProjectSerialization Serialization
        {
            get;
            set;
        }


        public IExplorerNodeFactory NodeFactory
        {
            get;
            set;
        }

        public IPEContextMenuFactory ContextMenuFactory
        {
            get;
            set;
        }
        public void RaiseProjectOpenedOrCreated(DotSpatial.Controls.IMap Map, IProject project)
        {
            if (ProjectOpenedOrCreated != null)
                ProjectOpenedOrCreated(Map, project);
        }

        public void Clear()
        {
            if (_Project != null)
                _Project.Clear();
        }
    }
}
