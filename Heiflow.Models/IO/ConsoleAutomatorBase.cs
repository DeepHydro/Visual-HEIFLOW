// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.IO
{
    public abstract class ConsoleAutomatorBase : IConsoleAutomator
    {
        protected readonly StringBuilder inputAccumulator = new StringBuilder();

        protected readonly byte[] buffer = new byte[256];

        protected volatile bool stopAutomation;

        public StreamWriter StandardInput { get; protected set; }

        protected StreamReader StandardOutput { get; set; }

        protected StreamReader StandardError { get; set; }

        public event EventHandler<ConsoleInputReadEventArgs> StandardInputRead;

        protected void BeginReadAsync()
        {
            if (!this.stopAutomation)
            {
                this.StandardOutput.BaseStream.BeginRead(this.buffer, 0, this.buffer.Length, this.ReadHappened, null);
            }
        }

        protected virtual void OnAutomationStopped()
        {
            this.stopAutomation = true;
            this.StandardOutput.DiscardBufferedData();
        }

        private void ReadHappened(IAsyncResult asyncResult)
        {
            var bytesRead = this.StandardOutput.BaseStream.EndRead(asyncResult);
            if (bytesRead == 0)
            {
                this.OnAutomationStopped();
                return;
            }

            var input = this.StandardOutput.CurrentEncoding.GetString(this.buffer, 0, bytesRead);
            this.inputAccumulator.Append(input);

            if (bytesRead < this.buffer.Length)
            {
                this.OnInputRead(this.inputAccumulator.ToString());
            }

            this.BeginReadAsync();
        }

        private void OnInputRead(string input)
        {
            var handler = this.StandardInputRead;
            if (handler == null)
            {
                return;
            }

            handler(this, new ConsoleInputReadEventArgs(input));
            this.inputAccumulator.Clear();
        }
    }
}
