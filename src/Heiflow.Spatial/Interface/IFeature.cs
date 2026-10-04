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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;

namespace Heiflow.Spatial.Geography
{
    public interface IFeature
    {
        IGeometry Geometry { get; set; }

        object this[string key]
        {
            get;
            set;
        }
    }

    public interface IFeatures : IEnumerable<IFeature>
    {
        //todo: This should be an enumerator directly on IFeatures
        void Add(IFeature feature);
        IFeature New();
    }

    public class Features : IFeatures
    {
        private List<IFeature> features = new List<IFeature>();

        public int Count
        {
            get { return features.Count; }
        }

        public IFeature this[int index]
        {
            get { return features[index]; }
        }

        public Features()
        {
            //Perhaps this constructor should get a dictionary parameter
            //to specify the name and type of the columns
        }

        public IFeature New()
        {
            //At this point it is possible to initialize an improved version of
            //Feature with a specifed set of columns.
            return new Feature();
        }

        public void Add(IFeature feature)
        {
            features.Add(feature);
        }

        public IEnumerator<IFeature> GetEnumerator()
        {
            return features.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return features.GetEnumerator();
        }

        private class Feature : IFeature
        {
            private IGeometry _Geometry;
            private Dictionary<string, object> dictionary;

            public Feature()
            {
                dictionary = new Dictionary<string, object>();
            }

            public IGeometry Geometry
            {
                get { return _Geometry; }
                set { _Geometry = value; }
            }

            public object this[string key]
            {
                get { return dictionary[key]; }
                set { dictionary[key] = value; }
            }
        }
    }        
}
