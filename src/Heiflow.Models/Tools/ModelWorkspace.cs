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
