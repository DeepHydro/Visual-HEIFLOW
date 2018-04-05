// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface
{
    public class StressPeriod
    {
        public StressPeriod()
        {
            StepOptions = new List<StepOption>();
            Dates = new List<DateTime>();
            State = Subsurface.ModelState.TR;
            ParameterFile = "";
        }
        public int ID { get; set; }
        /// <summary>
        /// the length of a stress period.
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// the number of time steps in a stress period.
        /// </summary>
        public int NSTP { get; set; }
        /// <summary>
        ///  the number of time steps in a stress period.
        /// </summary>
        public int NumTimeSteps
        {
            get
            {
                return Dates.Count;
            }
        }
        /// <summary>
        /// the multiplier for the length of successive time steps.
        /// The length of a time step is calculated by multiplying 
        /// the length of the previous time step by TSMULT. 
        /// </summary>
        public int Multiplier { get; set; }
        public ModelState State { get; set; }
        public string ParameterFile { get; set; }
        public bool IsSteadyState
        {
            get
            {
                return State == Subsurface.ModelState.SS;
            }
        }
        public DateTime Start
        {
            get
            {
                return Dates.First();
            }
        }
        public DateTime End
        {
            get
            {
                return Dates.Last();
            }
        }
        public List<StepOption> StepOptions { get; set; }
        public List<DateTime> Dates { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t# PERLEN NSTP TSMULT Ss/tr (Stress period {4})", Length, NumTimeSteps, Multiplier, State, ID);
        }
    }
}
