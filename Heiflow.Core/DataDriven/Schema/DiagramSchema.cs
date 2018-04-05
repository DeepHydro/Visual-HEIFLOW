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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Heiflow.Models.AI;
using Heiflow.Core.Data;

namespace Heiflow.Core.Schema
{
    public class DiagramSchema:IDiagramSchema
    {
        public DiagramSchema(IVariable[] stimulus, IVariable[] responses)
       {
           mStimulus = stimulus;
           mResponses = responses;
           IsValid = CheckSchema();
           Start = new DateTime(1980, 1, 1);
           End = new DateTime(1980, 12, 31);
           //mRefMin = 0.1;
           //mRefMax = 0.8;
           RefMin = 0.0;
           RefMax = 1.0;
           mNormalizationModelType = NormalizationModelType.Range;
       }

       private IVariable[] mStimulus;
       IVariable[] mResponses;
       TimeUnits mTimePeriod;
       DateTime mStart;
       DateTime mEnd;
       protected NormalizationModelType mNormalizationModelType;
       protected double mRefMin;
       protected double mRefMax;

       public double RefMin 
       {
           get
           {
               return mRefMin;         
           }
           set
           {
               mRefMin = value;
               SetNormalizationModel(mNormalizationModelType);
           }
       }

       public double RefMax
       {
           get
           {
               return mRefMax;
           }
           set
           {
               mRefMax = value;
               SetNormalizationModel(mNormalizationModelType);
           }

       }

       #region IDiagramSchema members

       [CategoryAttribute("Variables"),  Browsable(false)]
       public IVariable[] Stimulus
       {
           get {return mStimulus;}
       }
       
        [CategoryAttribute("Variables"), Browsable(false)]
        public IVariable[] Responses
        {
            get { return mResponses; }
        }
      
   
        [CategoryAttribute("Data"), Browsable(false)]
       public IForecastingDataSets ForecastingDataSets { get; set; }

        [CategoryAttribute("Date time"), ReadOnly(true)]
        public TimeUnits TimePeriod
        {
            get { return mTimePeriod; }
            set { mTimePeriod = value; } 
        }

        [CategoryAttribute("Date time")]
        public DateTime Start { get { return mStart; } set { mStart = value; } }

        [CategoryAttribute("Date time")]
        public DateTime End { get { return mEnd; } set { mEnd = value; } }

        [CategoryAttribute("Schema structure"), ReadOnly(true)]
        public bool IsValid
        {
            get;
            set;
        }

       [CategoryAttribute("Schema structure"), ReadOnly(true)]
       public int InstancesCount
        {
            set;
            get;
        }

        #endregion

       public virtual bool CheckSchema()
        {
            return false;
        }

       public void SetNormalizationModel(NormalizationModelType modeltype)
       {
           if (modeltype == NormalizationModelType.Range)
           {
               foreach (IVariable v in Stimulus)
               {
                   v.NormalizationModel = new RangeNormalization()
                   {
                       RefMax = RefMax,
                       RefMin=RefMin
                   };
               }
               foreach (IVariable v in Responses)
               {
                   v.NormalizationModel = new RangeNormalization()
                   {
                       RefMax = RefMax,
                       RefMin = RefMin
                   };
               }
           }
           else if (modeltype == NormalizationModelType.Sqrt)
           {
               foreach (IVariable v in Stimulus)
               {
                   v.NormalizationModel = new SqrtNormalization();
               }
               foreach (IVariable v in Responses)
               {
                   v.NormalizationModel = new SqrtNormalization();
               }
           }
           else if (modeltype == NormalizationModelType.None)
           {
               foreach (IVariable v in Stimulus)
               {
                   v.NormalizationModel = null;
               }
               foreach (IVariable v in Responses)
               {
                   v.NormalizationModel = null;
               }
           }
       }

        #region ICloneable 成员

        public virtual object Clone()
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
