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

using DotSpatial.Data;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Subsurface.MT3DMS;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Models.Subsurface
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
                return "Modflow";
            }
        }

        public string Extension
        {
            get
            {
                return ".nam";
            }
        }
        public bool CanImport(IProject project)
        {
            Modflow model = new Modflow();
            return model.Exsit(project.RelativeControlFileName);
        }
        public void Import(IProject project, IImportProperty property, ICancelProgressHandler progress)
        {
            var succ = LoadingState.Normal;
            ModelService.WorkDirectory = project.FullModelWorkDirectory;
            Modflow model = null;
            if (project.Model == null)
            {
                if (project is ModflowProject)
                    model = new Modflow();
                else if (project is MT3DMSProject)
                    model = new MT3DMSModel();
                else if (project is VFT3D.VFT3DProject)
                    model = new VFT3D.VFT3DModel();

                project.Model = model;
                project.Model.Project = project;
            }
            else
            {
                project.Model.Clear();
            }
            model = project.Model as Modflow;
            model.Project = project;
            model.WorkDirectory = project.FullModelWorkDirectory;
            model.ControlFileName = project.RelativeControlFileName;
            model.Initialize();
            model.Grid.Origin = new GeoAPI.Geometries.Coordinate(property.OriginX, property.OriginY);
            project.Model = model;
            succ = model.Load(progress);
            if (succ != LoadingState.FatalError)
            {
                model.TimeService.PopulateTimelineFromSP(property.Start);
                //model.TimeService.PopulateIOTimelineFromSP();
                model.Grid.Projection = property.Projection;
            }
            else
            {
                OnLoadFailed("Fatal error.");
            }
        }

        public LoadingState Load(IProject project, ICancelProgressHandler progress)
        {
            ModelService.WorkDirectory = project.FullModelWorkDirectory;
            Modflow model = null;
            if (project is MT3DMSProject)
                model = new MT3DMSModel();
            else if (project is VFT3D.VFT3DProject)
                model = new VFT3D.VFT3DModel();
            else if (project is ModflowProject)
                model = new Modflow();

            model.ControlFileName = project.RelativeControlFileName;
            model.WorkDirectory = project.FullModelWorkDirectory;
            model.Project = project;
            project.Model = model;
            model.Version = project.SelectedVersion;
            model.Initialize();
            var succ = model.Load(progress);
            if(succ != LoadingState.FatalError)
            {
                var dic =GetExtentSettings(model.ControlFileName+".ext");
                var start=DateTime.Now;
                if(dic.Keys.Contains("Start"))
                {
                    DateTime.TryParse(dic["Start"],out start);
                }
                model.TimeService.PopulateTimelineFromSP(start);
                //model.TimeService.PopulateIOTimelineFromSP();
            }
            return succ;
        }

        private Dictionary<string,string> GetExtentSettings( string filename)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if(File.Exists(filename))
            {
                StreamReader sr = new StreamReader(filename);
                while(!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if(!TypeConverterEx.IsNull(line))
                    {
                        var strs = TypeConverterEx.Split<string>(line, 2);
                        dic.Add(strs[0], strs[1]);
                    }
                }
                sr.Close();
            }
            return dic;
        }
        protected virtual void OnLoadFailed(string msg)
        {
            if (LoadFailed != null)
                LoadFailed(this, msg);
        }

        public void Clear()
        {
           // throw new NotImplementedException();
        }
    }
}
