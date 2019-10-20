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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Models.Generic.Project;
using System.Xml.Serialization;
using DotSpatial.Controls;
using System.ComponentModel;
using Heiflow.Models.Generic;
using System.Xml.Linq;
using Heiflow.Models.Generic.Parameters;
using System.Collections.ObjectModel;
using Heiflow.Models.Properties;
using Heiflow.Models.UI;
using DotSpatial.Data;

namespace Heiflow.Models.Generic.Project
{
    [Export(typeof(IProjectSerialization))]
    public class ProjectSerialization : IProjectSerialization 
    {
        private IProject _CurrentProject;
        public event EventHandler<bool> ProjectOpened;
        public event EventHandler<string> OpenFailed;
        private IModelLoader _CurrentModelLoader;

        public ProjectSerialization()
        {
            HasError = false;
        }

        /// <summary>
        /// Gets the AppManager that is responsible for activating and deactivating plugins as well as coordinating
        /// all of the other properties.
        /// </summary>
        public AppManager App { get; set; }

        /// <summary>
        /// Gets the save project file providers.
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<ISaveProjectFileProvider> SaveProjectFileProviders { get; private set; }

        /// <summary>
        /// Gets the open project file providers.
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IOpenProjectFileProvider> OpenProjectFileProviders { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IModelLoader> SurpportedModelLoaders { get; set; }

        [ImportMany]
        public IEnumerable<IProject> SurpportedProjects
        {
            get;
            set;
        }
        public IProject CurrentProject
        {
            get
            {
                return _CurrentProject;
            }
            private set
            {
                _CurrentProject = value;
            }
        }

        public bool HasError
        {
            get;
            protected set;
        }

        /// <summary>
        /// Serializes portions of the model to file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Save(string fileName, IProject project)
        {
            bool isProviderPresent = false;
            string extension = Path.GetExtension(fileName);

            foreach (ISaveProjectFileProvider provider in SaveProjectFileProviders)
            {
                if (String.Equals(provider.Extension, extension, StringComparison.OrdinalIgnoreCase))
                {
                    provider.Save(fileName, project);
                    isProviderPresent = true;
                    if(project.FullMapFileName != null && App != null)
                        App.SerializationManager.SaveProject(project.FullMapFileName);
                    break;
                }
            }
            if (!isProviderPresent)
            {
            }
            AddFileToRecentFiles(fileName);
        }

          public void Save(IProject project)
          {
              Save(project.FullProjectFileName, project);
          }

        /// <summary>
        /// Deserializes the model from a file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
          public void Open(string fileName, ICancelProgressHandler progress)
        {
            var dic = Path.GetDirectoryName(fileName);
            var loaded = true;
            string pname = GetProviderName(fileName);
            string errormsg = "";
            SetCurrentProjectDirectory(dic);

            foreach (var provider in OpenProjectFileProviders)
            {
                if (String.Equals(provider.ProviderName, pname, StringComparison.OrdinalIgnoreCase))
                {
                    var op = provider as IOpenProjectFileProvider;
                    CurrentProject = op.Open(fileName);
                    CurrentProject.AbsolutePathToProjectFile = Path.GetDirectoryName(fileName);
                    string prj_dic = Path.GetDirectoryName(fileName);

                    //repaire path
                    if(prj_dic.ToLower() != CurrentProject.AbsolutePathToProjectFile.ToLower())
                    {
                        CurrentProject.AbsolutePathToProjectFile = prj_dic;
                    }

                    CurrentProject.FullProjectFileName = fileName;
                    string controlfile = Path.Combine(CurrentProject.FullModelWorkDirectory, CurrentProject.RelativeControlFileName);
                    string ext = Path.GetExtension(controlfile);

                    if (!File.Exists(controlfile))
                    {
                        errormsg = string.Format("Control file dosen't exsit: {0}", controlfile);
                        OnOpenFailed(this, errormsg);
                        return;
                    }
                    CurrentProject.Initialize();
                    foreach (var mod in this.SurpportedModelLoaders)
                    {
                        if (mod.Extension.ToLower() == ext.ToLower())
                        {
                            _CurrentModelLoader = mod;
                            mod.LoadFailed += OnOpenFailed;
                            try
                            {
                                loaded = mod.Load(CurrentProject, progress);                              
                            }
                            catch(Exception ex)
                            {
                                loaded = false;
                                errormsg = string.Format("Failed to load project file. Error message: {0}", controlfile);
                                OnOpenFailed(this, ex.Message);
                            }
                            finally
                            {                                
                                mod.LoadFailed -= OnOpenFailed;
                            }
                            break;
                        }
                    }
                    break;
                }
            }
            if (loaded)
            {
                AddFileToRecentFiles(fileName);
                OnProjectOpened(loaded);
            }
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        public bool New(string prjName, string prjDir, IProject project, ICancelProgressHandler progress, bool ImportFromExistingModel)
        {
            bool succ = true;
            if (!Directory.Exists(prjDir))
                Directory.CreateDirectory(prjDir);
            SetCurrentProjectDirectory(prjDir);
            project.Name = prjName;
            project.AbsolutePathToProjectFile = prjDir;
            project.RelativeModelWorkDirectory = ".\\";
            project.Clear();
            project.Initialize();
            if (project.New(progress, ImportFromExistingModel))
            {
                App.SerializationManager.New();
                CurrentProject = project;
                CurrentProject.Map = App.Map;
            }
            else
            {
                succ = false;
                if(progress != null)
                    progress.Progress("Project", 100, "Failed to create project.");
            }
            return succ;
        }

        protected virtual void OnProjectOpened(bool succ)
        {
            if (ProjectOpened != null)
                ProjectOpened(this, succ);
        }
        protected virtual void OnOpenFailed(object sender, string msg)
        {
            CurrentProject = null;
            if (OpenFailed != null)
                OpenFailed(this, msg);
            HasError = true;
        }
        private void SetCurrentProjectDirectory(string dic)
        {
            // we set the working directory to the location of the project file. All filenames will be relative to this path.
            if (String.IsNullOrEmpty(dic))
            {
            }
            else
            {
                Directory.SetCurrentDirectory(dic);
                ModelService.ProjectDirectory = dic;
            }
        }

        private static void AddFileToRecentFiles(string fileName)
        {
            if (Settings.Default.RecentFiles.Contains(fileName))
            {
                Settings.Default.RecentFiles.Remove(fileName);
            }

            if (Settings.Default.RecentFiles.Count >= Settings.Default.MaximumNumberOfRecentFiles)
                Settings.Default.RecentFiles.RemoveAt(Settings.Default.RecentFiles.Count - 1);
            // insert value at the top of the list
            Settings.Default.RecentFiles.Insert(0, fileName);
            Settings.Default.Save();
        }
         private string GetProviderName(string project_filename)
         {
             XDocument doc = XDocument.Load(project_filename);
             string rootLocalName = doc.Root.Name.LocalName;
             return rootLocalName;       
         }

        public void Clear()
        {
            if (_CurrentModelLoader != null)
                _CurrentModelLoader.Clear();
            HasError = false;
        }
    }
}
