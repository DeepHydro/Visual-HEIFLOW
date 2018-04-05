// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Heiflow.Core
{
    public interface ITimeSeriesQueryCriteria
    {
        DateTime Start { get; set; }
        DateTime End { get; set; }
        int SiteID { get; set; }
        int VariableID { get; set; }
        HydroFeatureType SiteType { get; set; }
    }

    public interface IUnits
    {
        int ID { get; }
        string Name { get; set; }
        string AliasName { get; set; }
        string UnitType { get; set; }
        string Abbreviation { get; set; }
    }

    public interface IVariable
    {
        int VariableID { get; }
        string Code { get; set; }
        string Name { get; set; }
        int SiteID { get; set; }
        string Specification { get; set; }

        string ValueType { get; set; }
         string DataType { get; set; }
        double NoDataValue { get; set; }
        string SampleMedium { get; set; }
        string GeneralCategory { get; set; }
        double TimeSupport { get; set; }
        TimeUnits TimeUnits { get; set; }
        int TimeUnitsID { get; set; }
        int VariableUnitsID { get; set; }
        string UnitName { get; set; }
        DateTime BeginDate { get; set; }
        DateTime EndDate { get; set; }
        int ValueCount { get; set; }
        bool IncludeIntradayValue { get; set; }
        IUnits Units { get; set; }
        ModelRole ModelRole { get; set; }
        int WindowSize { get; set; }
        int PredicationSize { get; set; }
        INormalizationModel NormalizationModel { get; set; }
        IVectorTimeSeries<double> GetValues(ITimeSeriesQueryCriteria qc, ITimeSeriesTransform provider);
        IVectorTimeSeries<double> GetTransformedValues(ITimeSeriesQueryCriteria qc, ITimeSeriesTransform provider, double multiplier);
    }

    public interface ITimeSpan
    {
        DateTime Start { get; set; }
        DateTime End { get; set; }
    }

    public interface IObservationsSite
    {
        int ID { get; }
        int SpatialIndex { get; set; }
        string Code { get; set; }
        string Name { get; set; }
        string Comments { get; set; }
        double Longitude { get; set; }
        double Latitude { get; set; }
        double LocalX { get; set; }
        double LocalY { get; set; }
        double Distance { get; set; }
        double Elevation { get; set; }
        double Cell_Elevation { get; set; }
        string SiteType { get; set; }
        string Country { get; set; }
        string State { get; set; }
        int MonitorType { get; set; }
        ITimeSeries<double> TimeSeries { get; set; }
        List<IVectorTimeSeries<double>> TimeSeriesCollection { get; set; }
    }

    public interface INumericalSeriesPair
    {
        DateTime DateTime { get; set; }
        double Value { get; set; }
    }

 

   
}
