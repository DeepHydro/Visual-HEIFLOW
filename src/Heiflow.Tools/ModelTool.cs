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

using DotSpatial.Controls;
using DotSpatial.Symbology;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Heiflow.Tools
{
    [InheritedExport(typeof(IModelTool))]
    public abstract class ModelTool : IModelTool
    {
        /// <summary>
        /// Conceptual Modelling for Groundwater
        /// </summary>
        protected const string Cat_CMG = "Conceptual Modelling for Groundwater";
        public ModelTool()
        {
            Description = "This is a modeling tool";
            MultiThreadRequired = true;
            SubCategory = "None";
        }
        [Browsable(false)]
        public string AssemblyQualifiedName { get; protected set; }
        [Category("Developer Information")]
        public string Author { get; protected set; }
 
        [Browsable(false)]
        public string Category { get; protected set; }

        [Browsable(false)]
        public string SubCategory { get; protected set; }
 
        [Browsable(true)]
        [Category("Basic")]
        public string Description { get; protected set; }
 
        [Browsable(false)]
        public System.Drawing.Bitmap HelpImage { get; protected set; }
 
        [Browsable(false)]
        public string HelpUrl { get; protected set; }
 
        [Browsable(false)]
        public System.Drawing.Bitmap Icon { get; protected set; }
 
        [Browsable(false)]
        public DotSpatial.Modeling.Forms.Parameter[] InputParameters { get; protected set; }
        [Category("Basic")]
        public string Name { get; protected set; }
 
        [Browsable(false)]
        public DotSpatial.Modeling.Forms.Parameter[] OutputParameters { get; protected set; }
 
        [Browsable(false)]
        public string ToolTip { get; protected set; }
      [Category("Developer Information")]
        public string Version { get; protected set; }
         [Browsable(false)]
        public bool Initialized { get; protected set; }
        [Browsable(false)]
        public IPackage Package { get; set; }
        [Browsable(false)]
        public IModelWorkspace Workspace { get; set; }
          [Browsable(false)]
        public IWorkspaceView WorkspaceView { get; set; }
        [Browsable(false)]
        public IProjectService ProjectService  { get; set; }
        [Browsable(false)]
        public bool MultiThreadRequired
        {
            get;
            protected set;
        }

        public virtual void ParameterChanged(DotSpatial.Modeling.Forms.Parameter sender)
        {
        }

        public abstract void Initialize();

        public abstract bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler);

        public bool Validate(string var_fullname)
        {
            var var_name = GetName(var_fullname);
            bool has_var = false;
            bool dim_valid = false;
            bool valid = false;
            has_var = Workspace.Contains(var_name);
            dim_valid = GetDims(var_fullname) != null;
            valid = has_var && dim_valid;
            return valid;
        }

        public bool ValidateVector(string var_fullname)
        {
            var vec = GetVector(var_fullname);
            return vec != null;
        }

        public float[] GetVector(string var_fullname)
        {
            float[] vector = null;
            var var_name = this.GetName(var_fullname);
            var mat = Workspace.Get(var_name);
            if(mat != null)
            {
                var dims = GetDims(var_fullname);
                if (dims != null)
                    vector = mat.GetVector(int.Parse(dims[0]), dims[1], dims[2]);
            }
            return vector;
        }

        public DataCube<float> Get3DMat(string var_fullname)
        {
            if (TypeConverterEx.IsNull(var_fullname))
                return null;
            var true_name = GetName(var_fullname);
            if (true_name != "")
                return Workspace.Get(true_name);
            else
                return null;
        }

        public DataCube<float> Get3DMat(string var_fullname, ref int var_index)
        {
            var dims = GetDims(var_fullname);
            var true_name = GetName(var_fullname);
            if(dims != null && true_name!= "")
            {
                var_index = int.Parse(dims[0]);
                return Workspace.Get(true_name);
            }
           else
            {
                return null;
            }
        }

        protected string GetName(string var_fullname)
        {
            var index = var_fullname.IndexOf("[");
            if (index < 0)
                return "";
            var name_ture = var_fullname.Substring(0, index);
            return name_ture;
        }

        protected string[] GetDims(string var_fullname)
        {
            var pattern = @"\[(.*?)\]";
          //  var int_pattern = @"\d+";
            Regex regex = new Regex(pattern);
           // Regex int_regex = new Regex(int_pattern);
            var matches = regex.Matches(var_fullname);
            if (matches.Count == 3)
            {
                string[] dims = new string[3];
                for (int i = 0; i < 3; i++)
                {
                    var str = matches[i].Value;
                    //if (int_regex.IsMatch(str))
                    //{
                        var str1 = str.Replace("[", "");
                        str1 = str1.Replace("]", "");
                        dims[i] = str1;
                    //}
                }
                return dims;
            }
            else
            {
                return null;
            }
        }
        protected double[] ToDouble(int[] vec)
        {
            var dou_vec = Array.ConvertAll<int, double>(vec, s => s);
            return dou_vec;
        }

        /// <summary>
        /// bind project service
        /// </summary>
        /// <param name="en"></param>
        public virtual void BindProjectService(object en)
        {
            ProjectService = en as IProjectService;
        }

        public virtual void AfterExecution(object args)
        {

        }

        public virtual void Setup()
        {
            
        }
    }
}