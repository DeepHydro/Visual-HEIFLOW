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
