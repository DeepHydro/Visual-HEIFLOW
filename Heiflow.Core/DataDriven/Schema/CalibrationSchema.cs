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

namespace Heiflow.Core.Schema
{
    public class CalibrationSchema : DiagramSchema, ICalibrationSchema
    {
        public CalibrationSchema(IVariable[] stimulus, IVariable[] responses, ICalibrationModel model)
            : base(stimulus, responses)
        {
            mIModel = model;
            CalibrationDataSelectionModel = new CalibrationDataSelection();
        }

        private ICalibrationModel mIModel;

        #region ICalibrationSchema 成员
        [CategoryAttribute("Ignor"), Browsable(false)]
        public ICalibrationModel CalibrationModel
        {
            get { return mIModel; }
        }

        [CategoryAttribute("Ignor"), Browsable(false)]
        public IConceptualModel TargetModel
        {
            get { throw new NotImplementedException(); }
        }

        [CategoryAttribute("Ignor"), Browsable(false)]
        public ICalibrationDataSelection CalibrationDataSelectionModel
        {
            get;
            set;
        }

        public ICalibratonDataSets CalibratonDataSets
        {
            get;
            set;
        }

        #endregion
    }
}
