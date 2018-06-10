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
        string IOTimeFile { get; set; }
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
        int GetIOTimeLength(string work_dic);
    }
}
