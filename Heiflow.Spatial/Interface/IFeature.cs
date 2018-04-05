// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
