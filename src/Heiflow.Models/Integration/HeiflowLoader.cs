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
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

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
using DotSpatial.Data;

namespace Heiflow.Models.Integration
{
    public class HeiflowLoader : IModelLoader
    {
        public event EventHandler<string> LoadFailed;
        private HeiflowModel _model;
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

        public void Import(IProject project, IImportProperty property, ICancelProgressHandler progress)
        {
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
            _model = project.Model as HeiflowModel;
            _model.Project = project;
            _model.WorkDirectory = project.FullModelWorkDirectory;
            _model.ControlFileName = project.RelativeControlFileName;
            _model.Initialize();
            _model.Grid.Origin = new GeoAPI.Geometries.Coordinate(property.OriginX, property.OriginY);
            if (_model.Load(progress))
            {
                _model.ModflowModel.Grid.Projection = property.Projection;
            }
        }

        public bool Load(IProject project, ICancelProgressHandler progress)
        {
            ModelService.WorkDirectory = project.FullModelWorkDirectory;
            HeiflowModel model = new HeiflowModel();
            model.ControlFileName = project.RelativeControlFileName;
            model.WorkDirectory = project.FullModelWorkDirectory;
            model.Project = project;
            model.LoadFailed += delegate (object sender, string e) { OnLoadFailed(e); };
            model.Initialize();
            _model = model;
            return model.Load(progress);
        }

        private void OnLoadFailed(string msg)
        {
            if (LoadFailed != null)
                LoadFailed(this, msg);
        }

        public void Clear()
        {
            if (_model != null)
                _model.Clear();
        }
    }
}
