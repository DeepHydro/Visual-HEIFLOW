// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core;
using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
namespace Heiflow.Models.Generic
{
    public interface ITimeService
    {
        event EventHandler<int> GridLayerChanged;
        event TimeServiceUpdate Updated;

        List<DateTime> Timeline { get; set; }
        List<DateTime> IOTimeline { get; set; }
        /// <summary>
        /// starting from 0
        /// </summary>
        int CurrentGridLayer { get; set; }
        int CurrentStressPeriod { get; set; }
        int CurrentTimeStep { get; set; }
        int NumTimeStep { get;}
        /// <summary>
        /// in seconds
        /// </summary>
        int TimeInteval { get; }
        /// <summary>
        /// 1 - seconds, 2 - minutes,	3 - hours, 4 - days, 5 - years
        /// </summary>
        int TimeUnit { get; set; }
        int NumTimeStepInDay { get; }
        string Name { get; set; }
        IBasicModel Model { get; set; }
        DateTime Start { get; set; }
        DateTime End { get; set; }
        bool Initialized { get; }
        bool UseStressPeriods { get; set; }
       List<StressPeriod> StressPeriods { get; }
       void UpdateTimeLine();
       void UpdateStressPeriodTimeLine();
       void RaiseUpdated();
       void Clear();
       void InitSP(int[] steps, bool has_steady_state);
       bool ContainsSP(int sp_id);
        int[] GetIntevalSteps(TimeUnits time);
        int[] GetIntevalSteps(TimeUnits time, int maxstep);
        void PopulateTimelineFromSP(DateTime start);
        void PopulateIOTimelineFromSP();
        int GetNumStepsBetween(List<DateTime> list, DateTime start, DateTime end);
    }
}
