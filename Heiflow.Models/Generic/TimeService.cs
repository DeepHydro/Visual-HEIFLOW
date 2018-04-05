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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic
{
    public class TimeService : Heiflow.Models.Generic.ITimeService
    {
        public event EventHandler<int> GridLayerChanged;
        public event TimeServiceUpdate Updated;

        private int _CurrentTimeStep = 0;
        private int _CurrentGridLayer = 0;
        private int _CurrentStressPeriod = 0;
        protected  int[] _timestep = new int[] { 1, 60, 3600, 86400, 31536000 };
        private DateTime _Start;
        private DateTime _End;
        private int _NumTimeStep;
        private int _TimeUnit;

        public TimeService(string name)
        {
            Timeline = new List<DateTime>();
            IOTimeline = new List<DateTime>();
            StressPeriods = new List<StressPeriod>();
            Name = name;
            _TimeUnit = 4;
            UseStressPeriods = false;
            _Start = new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            _End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 0, 0);
            Initialized = false;
        }

        public string Name
        {
            get;
            set;
        }

        public DateTime Start
        {
            get 
            {
                return _Start;
            }
            set
            {
                _Start = value;
            }
        }

        public DateTime End
        {
            get
            {
                return _End;
            }
            set
            {
                _End = value;
            }
        }
        /// <summary>
        /// 1 - seconds, 2 - minutes,	3 - hours, 4 - days, 5 - years
        /// </summary>
        public int TimeUnit
        {
            get
            {
                return _TimeUnit;
            }
            set
            {
                _TimeUnit = value;
            }
        }
        /// <summary>
        /// in seconds
        /// </summary>
        public int TimeInteval
        {
            get
            {
                return _timestep[TimeUnit - 1];
            }
        }

        public int NumTimeStepInDay
        {
            get
            {
                return 86400 / TimeInteval;
            }
        }

        public int NumTimeStep
        {
            get
            {
                _NumTimeStep = Timeline.Count;
                return _NumTimeStep;
            }
            private  set
            {
                _NumTimeStep = value;
            }
        }

        public List<DateTime> Timeline
        {
            get;
            set;
        }
        public List<DateTime> IOTimeline
        {
            get;
            set;
        }
        public List<StressPeriod> StressPeriods
        {
            get;
            protected  set;
        }

        public IBasicModel Model 
        { 
            get; 
            set; 
        }

        public int CurrentTimeStep
        {
            get
            {
                return _CurrentTimeStep;
            }
            set
            {
                _CurrentTimeStep = value;
            }
        }

        public int CurrentGridLayer
        {
            get
            {
                return _CurrentGridLayer;
            }
            set
            {
                _CurrentGridLayer = value;
                OnGridLayerChanged();
            }
        }

        public int CurrentStressPeriod
        {
            get
            {
                return _CurrentStressPeriod;
            }
            set
            {
                _CurrentStressPeriod = value;
            }
        }

        public bool Initialized
        {
            get;
            protected set;
        }

        public bool UseStressPeriods
        {
            get;
            set;
        }

        public int[] GetIntevalSteps(TimeUnits time)
        {
            if (time == TimeUnits.Day)
            {
                var inteval = new int[Timeline.Count];
                MatrixExtension<int>.Set(inteval, 1);
                return inteval;
            }
            else if (time == TimeUnits.Month)
            {
                var derivedValues = Timeline.GroupBy(t => new { t.Year, t.Month }).Select(group =>
                      group.Count());
                var inteval = derivedValues.ToArray();
                return inteval;
            }
            else if (time == TimeUnits.CommanYear)
            {
                var derivedValues = Timeline.GroupBy(t => new { t.Year}).Select(group =>
                      group.Count());
                var inteval = derivedValues.ToArray();
                return inteval;
            }
            return null;
        }

        public int[] GetIntevalSteps(TimeUnits time, int maxstep)
        {
            int count = Timeline.Count;
            if (maxstep < Timeline.Count)
                count = maxstep;
            if (time == TimeUnits.Day)
            {
                var inteval = new int[count];
                MatrixExtension<int>.Set(inteval, 1);
                return inteval;
            }
            else if (time == TimeUnits.Month)
            {
                var list = Timeline;
                if (maxstep < Timeline.Count)
                {
                    var newlist = new List<DateTime>();
                    for (int i = 0; i < maxstep; i++)
                    {
                        newlist.Add(Timeline[i]);
                    }
                    list = newlist;
                }
                var derivedValues = list.GroupBy(t => new { t.Year, t.Month }).Select(group =>
                      group.Count());
                var inteval = derivedValues.ToArray();
                return inteval;
            }
            return null;
        }

        public int GetNumStepsBetween(List<DateTime> list, DateTime start, DateTime end)
        {
            int start_index = 0;
            int end_index = 0;
            int i = 0;
            foreach (var date in list)
            {
                if (date >= start)
                {
                    start_index = i;
                    break;
                }
                i++;
            }
            i = 0;
            foreach (var date in list)
            {
                if (date >= end)
                {
                    end_index = i;
                    break;
                }
                i++;
            }
            int nlen = end_index - start_index + 1;
            nlen = Math.Min(nlen, list.Count);
            return nlen;
        }

        public void UpdateTimeLine()
        {
            Timeline.Clear();
            var current = Start;
            _NumTimeStep = ((End - Start).Days + 1) * NumTimeStepInDay;        
            Timeline.Add(current);
            for (int i = 1; i < _NumTimeStep; i++)
            {
                current = current.AddSeconds((TimeInteval));
                Timeline.Add(current);
            }
            Initialized = true;
        }
        public void UpdateStressPeriodTimeLine()
        {
            IOTimeline.Clear();
            var sp_start = Start;

            for (int i = 0; i < StressPeriods.Count; i++)
            {
                var sp = StressPeriods[i];
                sp.Dates.Clear();
                if (sp.State == ModelState.SS)
                {
                   IOTimeline.Add(Start);
                   sp.Dates.Add(Start);
                }
                else
                {
                    for (int t = 0; t < sp.StepOptions.Count; t++)
                    {
                        var current = sp_start;
                        current = current.AddSeconds((sp.StepOptions[t].Step - 1) * TimeInteval);
                        IOTimeline.Add(current);
                        sp.Dates.Add(current);
                        if (t == sp.StepOptions.Count - 1)
                        {
                            sp_start = current;
                        }
                    }
                }
            }

        }

        public void Clear()
        {
            Timeline.Clear();
            IOTimeline.Clear();
            StressPeriods.Clear();
            Initialized = false;
        }

        public void InitSP(TimeUnits unit, bool has_steady_state)
        {
            var steps = GetIntevalSteps(unit);
            InitSP(steps, has_steady_state);
        }

        public void InitSP(int[] steps, bool has_steady_state)
        {
            StressPeriods.Clear();
            if (has_steady_state)
            {
                StressPeriod sp = new StressPeriod()
                {
                    ID = 1,
                    Length = 1,
                    Multiplier = 1,
                    State =  ModelState.SS
                };
                sp.Dates.Add(this.Start);
                var op = new StepOption(sp.ID, 1);
                sp.StepOptions.Add(op);
                AddSP(sp);
            }

            int start_index = 0;
            for (int i = 0; i < steps.Length; i++)
            {
                StressPeriod sp = new StressPeriod()
                {
                    Length = steps[i],
                    Multiplier = 1,
                    State =  ModelState.TR
                };
                if (has_steady_state)
                    sp.ID = i + 2;
                else
                    sp.ID = i + 1;
                sp.Dates = Timeline.GetRange(start_index, steps[i]);
                for (int n = 0; n < steps[i]; n++)
                {
                    var op = new StepOption(sp.ID, n + 1);
                    sp.StepOptions.Add(op);
                }
                AddSP(sp);
                start_index += steps[i];
            }
        }

        public void InitSP(int nsp, bool has_steady_state)
        {
            int[] steps = new int[nsp];
            int nlen = NumTimeStep / nsp;

            for (int i = 0; i < nsp-1; i++)
            {
                steps[i] = nlen;
            }

            steps[nsp - 1] = NumTimeStep - steps.Sum();
            InitSP(steps, has_steady_state);
        }

        public void UpdateSPDates(int[] steps)
        {
            int start_index = 0;
            for (int i = 0; i < steps.Length; i++)
            {
                StressPeriods[i].Dates = Timeline.GetRange(start_index, steps[i]);
                start_index += steps[i];
            }
        }

        public void AddSP(StressPeriod sp)
        {
            if (!ContainsSP(sp.ID))
                StressPeriods.Add(sp);
        }

        public StressPeriod GetSP(int sp_id)
        {
            var sp = from s in StressPeriods where s.ID == sp_id select s;
            if (sp.Count() == 1)
                return sp.First();
            else
                return null;
        }

        public bool ContainsSP(int sp_id)
        {
            var sp = from s in StressPeriods where s.ID == sp_id select s;
            return sp.Count() == 1;
        }
        private void OnGridLayerChanged()
        {
            if (GridLayerChanged != null)
                GridLayerChanged(Model, CurrentGridLayer);
        }

        public void RaiseUpdated()
        {
            if (Updated != null)
                Updated(this);
        }

        public void PopulateTimelineFromSP(DateTime start)
        {
            Timeline.Clear();
            var current = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second);
            foreach(var sp in StressPeriods)
            {
                if(sp.State ==  ModelState.SS)
                {
                    Timeline.Add(start);
                    
                }
                else
                {
                    var dts = sp.Length * this.TimeInteval / sp.NSTP;
                    for (int i = 0; i < sp.NSTP; i++)
                    {
                        current = current.AddSeconds(dts);
                        sp.Dates.Add(current);
                        Timeline.Add(current);
                    }
                }
            }
        }
        public void PopulateIOTimelineFromSP()
        {
            IOTimeline.Clear();
            foreach (var sp in StressPeriods)
            {
                foreach(var op in sp.StepOptions)
                {
                    IOTimeline.Add(sp.Dates[op.Step - 1]);
                }
            }
        }

    }
}
