// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Tools
{
    public class ModelWorkspace : Heiflow.Models.Tools.IModelWorkspace
    {
        private int _mat_counter = 0;

        public ModelWorkspace()
        {
            DataSources = new ObservableCollection<My3DMat<float>>(); 
        }

        public ObservableCollection<My3DMat<float>> DataSources { get; protected set; }

        public void Add(My3DMat<float> mat)
        {
            if (mat == null)
                return;
            if (TypeConverterEx.IsNull(mat.Name))
            {
                mat.Name = GetName();
            }
            var buf = from mm in DataSources where mm.Name == mat.Name select mm;
            if(buf.Any())
            {
                Remove(mat.Name);
            }
            DataSources.Add(mat);
        }

        public My3DMat<float> Get(string name)
        {
            var buf = from mm in DataSources where mm.Name == name select mm;
            if (buf.Any())
            {
                var mat_old = buf.First();
                return buf.First();
            }
            else
            {
                return null;
            }
        }

        public void Remove(string name)
        {
            var buf = from mm in DataSources where mm.Name == name select mm;
            if (buf.Any())
            {
                var mat_old = buf.First();
                DataSources.Remove(mat_old);
                mat_old = null;
            }
        }

        public bool Contains(string name)
        {
              var buf = from mm in DataSources where mm.Name == name select mm;
              if (buf.Any())
              {
                  return true;
              }
            else
              {
                  return false;
              }
        }
        
        public void Clear()
        {
            DataSources.Clear();
        }

        private string GetName()
        {
            string name = "Mat" + _mat_counter;
            _mat_counter++;
            return name;
        }
    }
}
