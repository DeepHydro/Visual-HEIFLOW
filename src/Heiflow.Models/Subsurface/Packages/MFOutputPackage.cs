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
using Heiflow.Models.Generic;
using Heiflow.Models.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface
{
    [DataPackageCollectionItem("Subsurface Output")]
    public class MFOutputPackage : DataPackageCollection
    {
        public static string PackageName="ModFlow Output";
        //public static string PackageName = "VFT3D Output";
        public MFOutputPackage()
        {
            _Name = "ModFlow Output";
            //_Name = "VFT3D Output";
            Category = Resources.OutputCategory; 
        }

        public override void Initialize()
        {
            foreach(var ch in Children)
            {
                ch.Initialize();
            }
            base.Initialize();
            State = ModelObjectState.Ready;
        }
        public override LoadingState Load(ICancelProgressHandler progress)
        {
            OnLoaded(progress, new LoadingObjectState());
            return LoadingState.Normal;
        }

        public override void Clear()
        {
            Children.Clear();
            base.Clear();
        }

        public new void AddChild(IPackage pck)
        {
            if(ContainChild(pck.Name))
            {
               var pck1 = Children.Single(p => p.Name == pck.Name);
               Children.Remove(pck1);
               Children.Add(pck);
            }
            else
            {
                Children.Add(pck);
            }
        }
    }
}
