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

using Heiflow.Models.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.DataDriven
{
    public abstract class Component : IComponent
    {
        public Component(IComponentParameter para)
        {
            mParameter = para;
            mParameter.Component = this;
        }
        protected IComponentParameter mParameter;

        #region IComponent 成员

        public string ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Descriptions
        {
            get;
            set;
        }

        public string Orgnization
        {
            get;
            set;
        }

        public IComponentParameter Parameter
        {
            get { return mParameter; }
        }

        public event ComponentRunEpochEventHandler ModelRunningEpoch;

        public event ComponentRunEventHandler ModelStartRunning;

        public event ComponentRunEventHandler ModelFinishRunning;

        #endregion

        protected void OnStartRunning(ComponentRunEventArgs e)
        {
            if (ModelStartRunning != null)
                ModelStartRunning(this, e);
        }

        protected void OnFinishRunning(ComponentRunEventArgs e)
        {
            if (ModelFinishRunning != null)
                ModelFinishRunning(this, e);
        }

        protected void OnRunningEpoch(ComponentRunEpochEventArgs e)
        {
            if (ModelRunningEpoch != null)
                ModelRunningEpoch(this, e);
        }
    }

    public abstract class ForecastingModel: Component, IForecastingModel
    {
        public ForecastingModel(IComponentParameter para):base(para)
        {      
        }

        #region IForecastingModel 成员

        public abstract void Train(IForecastingDataSets datasets);

        public abstract double Forecast(double[] inputVector);

        #endregion

    }
}
