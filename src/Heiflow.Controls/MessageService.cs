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

using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls
{
    /// <summary>
    /// This is the default implementation of the <see cref="IMessageService"/> interface. It shows messages via the MessageBox 
    /// to the user.
    /// </summary>
    /// <remarks>
    /// If the default implementation of this service doesn't serve your need then you can provide your own implementation.
    /// </remarks>
    [Export(typeof(IMessageService))]
    public class MessageService : IMessageService
    {
        public static DialogResult MessageBoxResult { get; private set; }

        private static MessageBoxOptions MessageBoxOptions
        {
            get
            {             
                return (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft) ? MessageBoxOptions.RtlReading : MessageBoxOptions.DefaultDesktopOnly;
            }
        }

        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="owner">The window that owns this Message Window.</param>
        /// <param name="message">The message.</param>
        public void ShowMessage(object owner, string message)
        {
            Form ownerWindow = owner as Form;
            if (ownerWindow != null)
            {
                MessageBoxResult = MessageBox.Show(ownerWindow, message, "VHF", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            else
            {
                MessageBoxResult = MessageBox.Show(message, "VHF", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }

        /// <summary>
        /// Shows the message as warning.
        /// </summary>
        /// <param name="owner">The window that owns this Message Window.</param>
        /// <param name="message">The message.</param>
        public void ShowWarning(object owner, string message)
        {
            Form ownerWindow = owner as Form;
            if (ownerWindow != null)
            {
                MessageBoxResult = MessageBox.Show(ownerWindow, message, "VHF", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBoxResult = MessageBox.Show(message, "VHF", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Shows the message as error.
        /// </summary>
        /// <param name="owner">The window that owns this Message Window.</param>
        /// <param name="message">The message.</param>
        public void ShowError(object owner, string message)
        {
            Form ownerWindow = owner as Form;
            if (ownerWindow != null)
            {
                MessageBoxResult = MessageBox.Show(ownerWindow, message, "VHF", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBoxResult = MessageBox.Show(message, "VHF", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Shows the specified question.
        /// </summary>
        /// <param name="owner">The window that owns this Message Window.</param>
        /// <param name="message">The question.</param>
        /// <returns><c>true</c> for yes, <c>false</c> for no and <c>null</c> for cancel.</returns>
        public bool? ShowQuestion(object owner, string message)
        {
            Form ownerWindow = owner as Form;
            DialogResult result;
            if (ownerWindow != null)
            {
                result = MessageBox.Show(ownerWindow, message, "VHF", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1
                    , System.Windows.Forms.MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                result = MessageBox.Show(message, "VHF", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }
            MessageBoxResult = result;
            if (result == DialogResult.Yes) { return true; }
            else if (result == DialogResult.No) { return false; }

            return null;
        }

        /// <summary>
        /// Shows the specified yes/no question.
        /// </summary>
        /// <param name="owner">The window that owns this Message Window.</param>
        /// <param name="message">The question.</param>
        /// <returns><c>true</c> for yes and <c>false</c> for no.</returns>
        public bool ShowYesNoQuestion(object owner, string message)
        {
            Form ownerWindow = owner as Form;
            DialogResult result;
            if (ownerWindow != null)
            {
                result = MessageBox.Show(ownerWindow, message, "VHF", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
            }
            else
            {
                result = MessageBox.Show(message, "VHF", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
            }
            MessageBoxResult = result;
            return result == DialogResult.Yes;
        }
    }
}
