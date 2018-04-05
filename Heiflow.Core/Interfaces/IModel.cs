// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core
{
   

    //public interface IForecastingModel : IComponent
    //{
    //    void  Train(IForecastingDataSets datasets);
    //    double Forecast(double[] inputVector);
    //}

    public interface IForecastingDataSetsSelection
    {
        IForecastingDataSets Select(IPredicationSchema schema);
    }

    public interface IDiagramSchema : ICloneable
    {
        IVariable [] Stimulus { get; }
        IVariable [] Responses { get; }     
        TimeUnits TimePeriod { get; set; }
        DateTime Start { get; set; }
        DateTime End { get; set; }
        bool IsValid { get; set; }
        int InstancesCount { get; set; }
        IForecastingDataSets ForecastingDataSets { get; set; }
    }

    public interface IPredicationSchema : IDiagramSchema
    {
        INormalizationModel NormalizationModel { get; set; }
        IForecastingDataSetsSelection ForecastingDataSetsSelectionModel { get; set; }
        IForecastingModel ForecastingModel { get; }
        int MaximumWindowSize { get; }
        int TotalWindowSize { get; }
        int PredicationSize { get; }
    }

    public interface IForecastingModel : IComponent
    {
        void Train(IForecastingDataSets datasets);
        double Forecast(double[] inputVector);
    }

    public interface IMultivariateAnalysisSchema : IDiagramSchema
    {
        IMultivariateAnalysis AnalysisModel { get; }
    }
}
