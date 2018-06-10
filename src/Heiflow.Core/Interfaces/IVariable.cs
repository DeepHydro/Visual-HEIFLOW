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
        DataCube<double> GetValues(ITimeSeriesQueryCriteria qc, ITimeSeriesTransform provider);
        DataCube<double> GetTransformedValues(ITimeSeriesQueryCriteria qc, ITimeSeriesTransform provider, double multiplier);
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
        DataCube<double> TimeSeries { get; set; }
        List<DataCube<double>> TimeSeriesCollection { get; set; }
    }

    public interface INumericalSeriesPair
    {
        DateTime DateTime { get; set; }
        double Value { get; set; }
    }

 

   
}
