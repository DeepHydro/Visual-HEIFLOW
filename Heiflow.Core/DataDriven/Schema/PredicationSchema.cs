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
using Heiflow.Core.Data;

namespace Heiflow.Core.Schema
{
   public  class PredicationSchema:DiagramSchema,IPredicationSchema
    {
       public PredicationSchema(IVariable[] stimulus, IVariable[] responses)
           : base(stimulus, responses)
       {
           NormalizationMehod = NormalizationModelType.Range;
           SelectionMehod = DataSelectionMethod.Rolling;
       }

       public PredicationSchema(IVariable[] stimulus, IVariable[] responses, IForecastingModel iModel):base(stimulus,responses)
       {
           mIModel = iModel;
           NormalizationMehod = NormalizationModelType.Range;
           SelectionMehod = DataSelectionMethod.Rolling;
       }
      
       private DataSelectionMethod mDataSelectionMethod;
       private IForecastingModel mIModel;
       int mMaximumWindowSize;
       int mTotalWindowSize;
       int mPredicationSize;

       [CategoryAttribute("Variables"), Browsable(false)]
       public IForecastingModel ForecastingModel
        {
            get { return mIModel; }
        }

       [CategoryAttribute("Normalization"), Browsable(false)]
       public INormalizationModel NormalizationModel
       {
           get;
           set;
       }

       [CategoryAttribute("Selection"), Browsable(false)]
       public IForecastingDataSetsSelection ForecastingDataSetsSelectionModel
       {
           get;
           set;
       }


       [CategoryAttribute("Normalization")]
       public NormalizationModelType NormalizationMehod
       {
           get
           {
               return mNormalizationModelType;
           }
           set
           {
               mNormalizationModelType = value;
               if (mNormalizationModelType == NormalizationModelType.Range)
                   NormalizationModel = new RangeNormalization();
               else if (mNormalizationModelType == NormalizationModelType.Sqrt)
                   NormalizationModel = new SqrtNormalization();
               else if (mNormalizationModelType == NormalizationModelType.None)
                   NormalizationModel = null;
               SetNormalizationModel(mNormalizationModelType);
           }
       }

       [CategoryAttribute("Normalization"), Description("Select a method to formulate input-output patterns")]
       public DataSelectionMethod SelectionMehod
       {
           get
           {
               return mDataSelectionMethod;
           }
           set
           {
               mDataSelectionMethod = value;
               if (mDataSelectionMethod == DataSelectionMethod.Rolling)
                   ForecastingDataSetsSelectionModel = new RollingSelection();
               else if (mDataSelectionMethod == DataSelectionMethod.CorrespondingPeriod)
                   ForecastingDataSetsSelectionModel = new CorrespondingPeriodSelection();
           }
       }

       [CategoryAttribute("Schema structure"), ReadOnly(true)]
       public int MaximumWindowSize
       {
           get { return mMaximumWindowSize; }
       }
       [CategoryAttribute("Schema structure"), ReadOnly(true)]
       public int TotalWindowSize
       {
           get { return mTotalWindowSize; }
       }
       [CategoryAttribute("Schema structure"), ReadOnly(true)]
       public int PredicationSize
       {
           get { return mPredicationSize; }
       }

       public override bool CheckSchema()
       {
           if (Stimulus != null && Responses != null)
           {
               mTotalWindowSize = (from v in Stimulus select v.WindowSize).Sum();
               mMaximumWindowSize = (from v in Stimulus select v.WindowSize).Max();
               mPredicationSize = (from v in Responses select v.PredicationSize).Max();
               return true;
           }
           else
           {
               return false;
           }
       }

        #region ICloneable 成员

        public override object Clone()
        {
            //return new PredicationSchema()
            //{
            //    End = End,
            //    IModel = IModel,
            //    InstancesCount = InstancesCount,
            //    IsValid = IsValid,
            //    MaximumWindowSize = MaximumWindowSize,
            //    NormalizationModel = NormalizationModel,
            //    PredicationSize = PredicationSize,
            //    Responses = Responses,
            //    Start = Start,
            //    Stimulus = Stimulus,
            //    TimePeriod = TimePeriod,
            //    TotalWindowSize = TotalWindowSize
            //};
            return null;
        }

        #endregion
    }
}
