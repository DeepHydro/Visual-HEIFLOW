﻿//
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
