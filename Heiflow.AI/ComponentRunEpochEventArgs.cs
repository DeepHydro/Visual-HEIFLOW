// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Models.AI
{
    public class ComponentRunEpochEventArgs : EventArgs
    {
        public ComponentRunEpochEventArgs()
        {
        }

        public ComponentRunEpochEventArgs(int iteration)
        {
            this.trainingIteration = iteration;
        }

        private int trainingIteration;

        /// <summary>
        /// Gets the current training iteration
        /// </summary>
        /// <value>
        /// Current Training Iteration.
        /// </value>
        public int TrainingIteration
        {
            get { return trainingIteration; }
        }
    }

    public class ComponentRunEventArgs : EventArgs
    {
        public ComponentRunEventArgs()
        {
        }

        public ComponentRunEventArgs(IForecastingDataSets datasets)
        {
            mIForecastingDataSets = datasets;
        }

        private IForecastingDataSets mIForecastingDataSets;

        public IForecastingDataSets ForecastingDataSets
        {
            get
            {
                return mIForecastingDataSets;
            }
        }

        public object State { get; set; }
    }
}
