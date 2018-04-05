//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

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
