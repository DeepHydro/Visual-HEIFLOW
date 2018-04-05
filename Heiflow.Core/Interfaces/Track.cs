// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core
{
    /// <summary>
    /// Expresee a sensor used to track message generated in internal procedures
    /// </summary>
    public interface ITrackSensor
    {
        List<string> Message { get; set; }
        void AddMessage(string msg);
        void Clear();
    }

    /// <summary>
    /// Device that displays messages tracked by sensors
    /// </summary>
    public interface ITrackDevice
    {
        /// <summary>
        /// Display message at once
        /// </summary>
        /// <param name="message"></param>
        void Dump(List<string> message);
        /// <summary>
        /// Clear message
        /// </summary>
        void Clear();

    }
    public class TaskSensor : ITrackSensor
    {
        public TaskSensor()
        {
            mMessage = new List<string>();
        }
        private List<string> mMessage;

        public List<string> Message
        {
            get
            {
                return mMessage;
            }
            set
            {
                mMessage = value;
            }
        }

        public void AddMessage(string msg)
        {
            mMessage.Add(msg);
        }

        public void Clear()
        {
            mMessage.Clear();
        }
    }

    public class ErrorSensor : ITrackSensor
    {
        public ErrorSensor()
        {
            mMessage = new List<string>();
        }
        private List<string> mMessage;

        public List<string> Message
        {
            get
            {
                return mMessage;
            }
            set
            {
                mMessage = value;
            }
        }

        public void AddMessage(string msg)
        {
            mMessage.Add(msg);
        }

        public void Clear()
        {
            mMessage.Clear();
        }
    }

    public class ExceptionSensor : ITrackSensor
    {
        public ExceptionSensor()
        {
            mMessage = new List<string>();
        }
        private List<string> mMessage;

        public List<string> Message
        {
            get
            {
                return mMessage;
            }
            set
            {
                mMessage = value;
            }
        }

        public void AddMessage(string msg)
        {
            mMessage.Add(msg);
        }

        public void Clear()
        {
            mMessage.Clear();
        }

    }

    public class OutputSensor : ITrackSensor
    {
        public OutputSensor()
        {
            mMessage = new List<string>();
        }
        private List<string> mMessage;

        public List<string> Message
        {
            get
            {
                return mMessage;
            }
            set
            {
                mMessage = value;
            }
        }

        public void AddMessage(string msg)
        {
            mMessage.Add(msg);
        }

        public void Clear()
        {
            mMessage.Clear();
        }
    }
}
