// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
