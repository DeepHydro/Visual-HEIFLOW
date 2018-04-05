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
