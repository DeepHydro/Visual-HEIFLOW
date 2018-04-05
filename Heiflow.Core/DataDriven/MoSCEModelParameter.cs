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
using System.ComponentModel;

namespace Heiflow.Core.DataDriven
{
    public class MoSCEModelParameter : ModelParameter
    {
        public MoSCEModelParameter()
        {
            ErrorFunction = ErrorIndicator.RMSE;
        }

        int nOptPar = 13;
        int nOptObj = 2;
        int nSamples = 500;
        int nComplex = 10;
        int nMaxIteration = 1000;

        [CategoryAttribute("SCE Model Parameter"), DescriptionAttribute("Number of parameters to be optimized")]
        public int ParametersCount
        {
            get
            {
                return nOptPar;
            }
            set
            {
                nOptPar = value;
            }
        }

        [CategoryAttribute("SCE Model Parameter"), DescriptionAttribute("Number of optimization functions")]
        public int ObjectivesCount
        {
            get
            {
                return nOptObj;
            }
            set
            {
                nOptObj = value;
            }
        }

        [CategoryAttribute("SCE Model Parameter"), DescriptionAttribute("An integar indicating the number of samples. Its allowalbe minimum value is 500.")]
        public int Samples
        {
            get
            {
                return nSamples;
            }
            set
            {
                nSamples = value;
                if (nSamples < 500)
                    nSamples = 500;
            }
        }

        [CategoryAttribute("SCE Model Parameter"), DescriptionAttribute("Number of complex")]
        public int Complex
        {
            get
            {
                return nComplex;
            }
            set
            {
                nComplex = value;
            }
        }

        [CategoryAttribute("SCE Model Parameter"), DescriptionAttribute("Maximum iterations (default 1000)")]
        public int MaximumIteration
        {
            get
            {
                return nMaxIteration;
            }
            set
            {
                nMaxIteration = value;
                if (nMaxIteration < 1000)
                    nMaxIteration = 1000;
                if ((nMaxIteration % 100) != 0)
                    nMaxIteration = ((int)(nMaxIteration / 100)) * 100;
            }
        }

        [CategoryAttribute("SCE Model Parameter"), DescriptionAttribute("Length of simulated values")]
        public int SimulatedLength
        {
            set;
            get;
        }

        public ErrorIndicator ErrorFunction
        {
            get;
            set;
        }

        [CategoryAttribute("SCE Model Parameter"), DescriptionAttribute("Start time")]
        public DateTime Start
        {
            get;
            set;
        }

        [CategoryAttribute("SCE Model Parameter"), DescriptionAttribute("End time")]
        public DateTime Termination
        {
            get;
            set;
        }
    }
}
