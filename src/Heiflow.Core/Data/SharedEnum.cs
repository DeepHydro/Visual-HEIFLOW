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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core
{
    public enum TimeUnits
    {
        Millisecond = 100,
        Second = 101,
        Minute = 102,
        Hour = 103, 
        Day = 104, 
        CommanYear = 105,
        Month = 106,
        Week = 107, 
        Season = 200, 
        HalfYear = 201,        
        Unknown = 202,
        FiveDays=203
    }

    public enum ObservationsDataType
    {
        BestEasySystematicEstimator,
        Categorical,
        ConstantOverInterval,
        Continuous,
        Cumulative,
        Incremental,
        Maximum,
        Median,
        Minimum,
        Sporadic,
        StandardDeviation,
        Unknown,
        Variance
    }

    public enum ObservationsValueType
    {
        DerivedValue,
        FieldObservation,
        ModelSimulationResult,
        Sample,
        Unknown
    }

    /// <summary>
    /// Indicate the role of a linkable component
    /// </summary>
    public enum ModelRole { Input, Output, Compute,None,InputOrOutput,Calibrate }
    
    /// <summary>
    /// Indicate the method to normalize time series
    /// </summary>
    public enum NormalizationModelType
    {
        /// <summary>
        /// all the data values are transformed into a range of minimum value (0.1 by default) and maximum value (0.8 by default)
        /// </summary>
        Range,
        /// <summary>
        /// Use sqrt method to normalize the data values
        /// </summary>
        Sqrt,
        /// <summary>
        ///Do not apply any noramlization on the data values
        /// </summary>
        None
    }

    /// <summary>
    /// Indicate the method of selecting forecasting data sets
    /// </summary>
    public enum DataSelectionMethod 
    { 
        /// <summary>
        /// Continuously select input-ouput patterns
        /// </summary>
        Rolling,
        /// <summary>
        /// Select input-ouput patterns in corresponding peroid of each year, then merge all the patterns
        /// </summary>
        CorrespondingPeriod ,
    }
    
    /// <summary>
    /// 
    /// </summary>
    public enum HydroFeatureType { Basin, Watershed, River, Site, HydroPoint, HydroLine, HydroArea, Unknown,IrrigationSystem,Lake }

    public enum NumericalDataType
    {
        Term,
        Average,
        Categorical,
        Continuous,
        Cumulative,
        Incremental,
        Maximum,
        Median,
        Minimum,
        Sporadic,
        StandardDeviation,
        Unknown,
        Variance,
    }

    public enum SummationMethod { ArithmeticAverage,ThiessenPolygon}

    public enum ErrorIndicator
    {
        RMSE,
        STD,
        HMLE,
        NSE,
        NSC,
        BIAS
    }
   //// public enum HydroFeatureType { Gate, FlowStation, WeatherStation, Reservoir, ObservationsSite, River, GroundwaterStation, PricipitatinStation, Unknown }
   // /// <summary>
   // /// 水文观测项目
   // /// </summary>
   // public enum HydrologyItem { 流量, 降雨, 水面蒸发, 水位, 水温, 含沙量, 输沙率, 冰流量, 水面蒸发辅助项目 };
   // /// <summary>
   // /// 气象观测项目
   // /// </summary>
   // public enum MeteorologyItem { 平均风速, 最大风向, 最大风速, 最低气压, 平均气压, 最高气压, 最低气温, 平均气温, 最高气温, 最小相对湿度, 平均相对湿度, 日照时数, 降水量, 蒸发量 };
   // /// <summary>
   // /// 地下水观测项目
   // /// </summary>
   // public enum GroundwaterItem { 埋深 };

    public enum ModelCategory { Forecasting, MultivariateAnalysis, Calibration, Conceptual,CFD,WaterQuality, Unknown }
    public enum OperationMode { Trainning, Validating, Forecasting }
}
