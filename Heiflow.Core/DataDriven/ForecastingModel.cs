// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
