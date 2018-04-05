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
