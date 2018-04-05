// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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

        public DataPackage()
        {
            Icon = Properties.Resources.TableFolder16;
            LargeIcon = Properties.Resources.TableFolder16;
            _MaxTimeStep = Settings.Default.MaxTimeStep;
            _StartLoading = DateTime.Now.AddDays(-_MaxTimeStep);
            _EndLoading = DateTime.Now;
            Start = DateTime.Now;
            End = DateTime.Now;
            DataViewMode = IO.DataViewMode.DayByDay;
            TimeUnits = TimeUnits.Day;
            NumericalDataType = Core.NumericalDataType.Average;
            ScaleFactor = 1.0;
            Description = "This is a data package";
            UseSpecifiedFile = false;
        }

        [Category("Metadata")]
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

        [Category("Metadata")]
        public int NumTimeStep
        {
            get;
            protected set;
        }

        [Category("Metadata")]
        public DateTime Start
        {
            get;
            protected set;
        }

        [Category("Metadata")]
        public DateTime End
        {
            get;
            protected set;
        }
        /// <summary>
        /// 
        /// </summary>
        /// 
         [Category("Metadata")]
        public int StepsToLoad
        {
            get
            {
                int ndays = TimeService.IOTimeline.Count;
                int nstep = 0;
                if (NumTimeStep > 0)
                {
                    if (MaxTimeStep < 1)
                    {
                        nstep = NumTimeStep;
                    }
                    else
                    {
                        nstep = Math.Min(MaxTimeStep, NumTimeStep);
                    }
                }
                else
                {
                    if (MaxTimeStep < 1)
                        nstep = ndays;
                    else
                        nstep = MaxTimeStep;
                }
                var buf = Math.Min(ndays, nstep);
                return buf;
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
                if (value > NumTimeStep)
                    _MaxTimeStep = NumTimeStep;
                else
                    _MaxTimeStep = value;
                if (_MaxTimeStep > 0 && _MaxTimeStep <= TimeService.IOTimeline.Count)
                    _EndLoading = TimeService.IOTimeline[_MaxTimeStep - 1];
                else
                    _EndLoading = TimeService.IOTimeline.Last();
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
                if (value > End)
                    value = End;
                _EndLoading = value;
                _MaxTimeStep = TimeService.GetNumStepsBetween(this.TimeService.IOTimeline, StartOfLoading, EndOfLoading);
                if (_MaxTimeStep > NumTimeStep)
                    _MaxTimeStep = NumTimeStep;
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
        /// <summary>
        /// 3DMat [VarCount, TimeSteps, ActiveCellCount]
        /// </summary>
       // [PackageElement(typeof(MyArray<float>))]
        [Browsable(false)]
        public MyLazy3DMat<float> Values
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

        public abstract bool Load(int var_index);

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
             
        }
        public override void OnTimeServiceUpdated(ITimeService time)
        {
            this.Start = TimeService.Start;
            this.End = TimeService.End;
            this.StartOfLoading = TimeService.Start;
            this.EndOfLoading = TimeService.End;
            ScaleFactor = 1.0;
            TimeUnits = (TimeUnits)(100 + (int)TimeService.TimeUnit);
            NumericalDataType = Core.NumericalDataType.Average;
            DataViewMode = DataViewMode.Average;
        }
    }
}
