// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
        public ModelTool()
        {
            Description = "This is a modeling tool";
            MultiThreadRequired = true;
        }
        [Browsable(false)]
        public string AssemblyQualifiedName { get; protected set; }
        [Category("Developer Information")]
        public string Author { get; protected set; }
 
        [Browsable(false)]
        public string Category { get; protected set; }
 
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
            if (var_fullname.Contains(":"))
               var_fullname = var_fullname.Replace(":", "-1");
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
            if (var_fullname.Contains(":"))
                var_fullname = var_fullname.Replace(":", "-1");
            var vec = GetVector(var_fullname);
            return vec != null;
        }

        public float[] GetVector(string var_fullname)
        {
            if (var_fullname.Contains(":"))
                var_fullname = var_fullname.Replace(":", "-1");
            float[] vector = null;
            var var_name = this.GetName(var_fullname);
            var mat = Workspace.Get(var_name);
            if(mat != null)
            {
                var dims = GetDims(var_fullname);
                if (dims != null)
                    vector = mat.GetVector(dims[0], dims[1], dims[2]);
            }
            return vector;
        }

        public My3DMat<float> Get3DMat(string var_fullname)
        {
            if (var_fullname.Contains(":"))
                var_fullname = var_fullname.Replace(":", "-1");
            if (TypeConverterEx.IsNull(var_fullname))
                return null;
            var true_name = GetName(var_fullname);
            if (true_name != "")
                return Workspace.Get(true_name);
            else
                return null;
        }

        public My3DMat<float> Get3DMat(string var_fullname, ref int var_index)
        {
            if (var_fullname.Contains(":"))
                var_fullname = var_fullname.Replace(":", "-1");
            var dims = GetDims(var_fullname);
            var true_name = GetName(var_fullname);
            if(dims != null && true_name!= "")
            {
                var_index = dims[0];
                return Workspace.Get(true_name);
            }
           else
            {
                return null;
            }
        }

        protected string GetName(string var_fullname)
        {
            if (var_fullname.Contains(":"))
                var_fullname = var_fullname.Replace(":", "-1");
            var index = var_fullname.IndexOf("[");
            if (index < 0)
                return "";
            var name_ture = var_fullname.Substring(0, index);
            return name_ture;
        }

        protected int[] GetDims(string var_fullname)
        {
            if (var_fullname.Contains(":"))
                var_fullname = var_fullname.Replace(":", "-1");
            var pattern = @"\[(.*?)\]";
            var int_pattern = @"\d+";
            Regex regex = new Regex(pattern);
            Regex int_regex = new Regex(int_pattern);
            var matches= regex.Matches(var_fullname); 
            if(matches.Count == 3)
            {
                int[] dims = new int[3];
                for (int i = 0; i < 3;i++ )
                {
                    int dim = 0;
                    var str=matches[i].Value;
                    if(int_regex.IsMatch(str))
                    {
                        var str1= str.Replace("[","");
                        str1 = str1.Replace("]", "");
                        dim = int.Parse(str1);
                        if (dim < 0)
                            dim = MyMath.full;
                        dims[i] = dim;
                    }
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