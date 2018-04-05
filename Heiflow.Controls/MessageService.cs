// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
