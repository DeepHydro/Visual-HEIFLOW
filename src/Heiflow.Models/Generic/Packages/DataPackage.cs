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

using DotSpatial.Data;
using Heiflow.Core;
using Heiflow.Core.Animation;
using Heiflow.Core.Data;
using Heiflow.Models.IO;
using Heiflow.Models.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic
{
    public abstract class DataPackage : Package, IDataPackage
    {
        protected string[] _Variables;
      
        protected TimeUnits mTimeUnit;
        protected DateTime _StartLoading;
        protected DateTime _EndLoading;
        protected NumericalDataType mNumericalDataType;
        protected int _MaxTimeStep;
        protected int _NumTimeStep;
        protected int _StepsToLoad;
        protected ICancelProgressHandler _ProgressHandler;

        public DataPackage()
        {
            Icon = Properties.Resources.TableFolder16;
            LargeIcon = Properties.Resources.TableFolder16;
            _MaxTimeStep = Settings.Default.MaxTimeStep;
            _StartLoading = DateTime.Now.AddDays(-_MaxTimeStep);
            _EndLoading = DateTime.Now;
            DataViewMode = IO.DataViewMode.DayByDay;
            TimeUnits = TimeUnits.Day;
            NumericalDataType = Core.NumericalDataType.Average;
            ScaleFactor = 1.0;
            Description = "This is a data package";
            UseSpecifiedFile = false;
        }

        [Category("General")]
        public virtual string[] Variables
        {
            get
            {
                return _Variables;
            }
            protected set
            {
                _Variables = value;
                OnPropertyChanged("Variables");      
            }
        }

        [Category("Time")]
        public int NumTimeStep
        {
            get
            {
                return _NumTimeStep;
            }
            protected set
            {
                _NumTimeStep = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// 
         [Category("Time")]
        public int StepsToLoad
        {
            get
            {
                _StepsToLoad = Math.Min(MaxTimeStep, _NumTimeStep);
                return _StepsToLoad;
            }
        }
        [Category("Spatial Behavior")]
        [Browsable(false)]
        public int Layer
        {
            get;
            set;
        }

        [Category("Time")]
        [Browsable(false)]
        public int SkippedSteps
        {
            get;
            set;
        }

        [Category("Time")]
        public int MaxTimeStep
        {
            get
            {
                return _MaxTimeStep;
            }
            set
            {
                 _MaxTimeStep = value;
                 if (_MaxTimeStep > 0 && _MaxTimeStep <= TimeService.IOTimeline.Count)
                     _EndLoading = TimeService.IOTimeline[_MaxTimeStep - 1];
                 else
                 {
                     _EndLoading = TimeService.IOTimeline.Last();
                     _MaxTimeStep = TimeService.IOTimeline.Count;
                 }
            }
        }

        [Category("Time")]
        [Browsable(true)]
        public TimeUnits TimeUnits
        {
            get
            {
                return mTimeUnit;
            }
            set
            {
                mTimeUnit = value;
            }
        }

        [Category("Time")]
        public DateTime StartOfLoading
        {
            get
            {
                return _StartLoading;
            }
            set
            {
                _StartLoading = value;
            }
        }

        [Category("Time")]
        public DateTime EndOfLoading
        {
            get
            {
                return _EndLoading;
            }
            set
            {
                if (TimeService.IOTimeline.Count > 0)
                {
                    if (value > TimeService.IOTimeline.Last())
                        value = TimeService.IOTimeline.Last();
                    _EndLoading = value;
                    _MaxTimeStep = TimeService.GetNumStepsBetween(this.TimeService.IOTimeline, _StartLoading, _EndLoading);
                }
                else
                {
                    _EndLoading = value;
                    _MaxTimeStep = 1;
                }
            }
        }

        [Category("Numerical")]
        [Browsable(false)]
        public DataViewMode DataViewMode { get; set; }

        [Category("Numerical")]
        public NumericalDataType NumericalDataType
        {
            get
            {
                return mNumericalDataType;
            }
            set
            {
                mNumericalDataType = value;
            }
        }

        [Category("Numerical")]
        public double ScaleFactor
        {
            get;
            set;
        }
        [Category("Database")]
        [Description("Specify the variable ID used to retrived observation time series from the ODM database")]
        public int ODMVariableID
        {
            get;
            set;
        }

        [Browsable(false)]
        public DataCube<float> DataCube
        {
            get;
            protected set;
        }

        [Category("File")]
        public bool UseSpecifiedFile
        {
            get;
            set;
        }
           [Category("File")]
        public string  SpecifiedFileName
        {
            get;
            set;
        }

        public abstract  bool Scan();

        public abstract bool Load(int var_index, ICancelProgressHandler progess);

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
             
        }
        public override void OnTimeServiceUpdated(ITimeService time)
        {
            this.StartOfLoading = TimeService.Start;
            this.EndOfLoading = TimeService.End;
            ScaleFactor = 1.0;
            TimeUnits = (TimeUnits)(100 + (int)TimeService.TimeUnit);
            NumericalDataType = Core.NumericalDataType.Average;
            DataViewMode = DataViewMode.Average;
        }
    }
}
