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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.IO
{
    public abstract class BaseDataCubeStream:IDataCubeStream
    {
        public event EventHandler<DataCube<float>> DataCubeLoaded;
        public event EventHandler<int> Loading;
        public event EventHandler<string> LoadFailed;
        public int StepsToLoad
        {
            get
            {
                return System.Math.Min(NumTimeStep, MaxTimeStep);
            }
        }

        public int NumTimeStep
        {
            get;
            set;
        }
        public int MaxTimeStep
        {
            get;
            set;
        }

        public string[] Variables
        {
            get;
            set;
        }

        public DataCube<float> DataCube
        {
            get;
            set;
        }
        public abstract void Scan();
        public abstract void LoadDataCube();
        public abstract void LoadDataCube(int var_index);
        public void SetDataCube(DataCube<float> source)
        {
            this.DataCube = source;
        }
        protected virtual void OnLoading(int percent)
        {
            if (Loading != null)
                Loading(this, percent);
        }
        protected virtual void OnDataCubedLoaded(DataCube<float> e)
        {
            if (DataCubeLoaded != null)
                DataCubeLoaded(this, e);
        }
        protected virtual void OnLoadFailed(string msg)
        {
            if (LoadFailed != null)
                LoadFailed(this, msg);
        }
    }
}
