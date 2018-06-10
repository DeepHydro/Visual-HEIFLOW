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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Running
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
