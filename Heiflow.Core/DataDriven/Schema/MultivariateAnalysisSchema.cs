﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.Schema
{
    public class MultivariateAnalysisSchema:PredicationSchema,IMultivariateAnalysisSchema
    {
        public MultivariateAnalysisSchema(IVariable[] stimulus, IVariable[] responses, IMultivariateAnalysis iModel)
            : base(stimulus, responses)
        {
            mModel = iModel;
        }

        private IMultivariateAnalysis mModel;

        #region IMultivariateAnalysisSchema 成员

        public IMultivariateAnalysis AnalysisModel
        {
            get { return mModel; }
        }

        #endregion
    }
}