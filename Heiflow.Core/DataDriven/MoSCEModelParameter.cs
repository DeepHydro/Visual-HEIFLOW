// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
