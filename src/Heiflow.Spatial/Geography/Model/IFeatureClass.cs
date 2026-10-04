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

using Heiflow.Spatial.Geography;
using System;
using System.Net;


namespace Heiflow.Spatial
{

    public interface IObjectClass
    {
        /// <summary>
        /// Adds a field to this object class
        /// </summary>
        /// <param name="Field"></param>
        void AddField(IField Field);
        /// <summary>
        /// Deletes a field from this object class
        /// </summary>
        /// <param name="Field"></param>
        void DeleteField(IField Field);
        /// <summary>
        /// The fields collection for this object class
        /// </summary>
        IFields Fields { get; }
        /// <summary>
        /// The index of the field with the specified name
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        int FindField(string Name);
        /// <summary>
        /// Indicates if the class has an object identity (OID) field
        /// </summary>
        bool HasOID { get; }
    }

    public interface IFeature
    {
        string[] FieldsName { get;}
        string Name { get; set; }
        string Description { get; set; }
        IObjectClass Class { get; set; }
        /// <summary>
        /// A reference to the default shape for the feature
        /// </summary>
        IGeometry Shape { get; set; }
        ModelType Modeltype { get; set; }
        string DisplayField { get; set; }
        IFeatureRender FeatureRender {get; set; }
    }

    public interface IFeatureClass
    {
        IFeature[] Features { get; set; }
        string[] FieldsName { get; set; }
        /// <summary>
        /// Get the feature with the specified object ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        IFeature GetFeature(int ID);
        IFeature[] GetFeatures(IQueryFilter QueryFilter);
        /// <summary>
        /// The number of features selected by the specified query
        /// </summary>
        /// <param name="QueryFilter"></param>
        /// <returns>If Nothing is supplied for the IQueryFilter, then FeatureCount returns the total number of features in the feature class</returns>
        int FeatureCount(IQueryFilter QueryFilter);
        /// <summary>
        ///  The type of the default Shape for the features in this feature class
        /// </summary>
        GeometryType ShapeType { get; set; }
        ModelType Modeltype { get; set; }
    }
}
