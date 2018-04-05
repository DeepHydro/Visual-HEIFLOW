// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Presentation.Services
{
    public interface IMessageService
    {
        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="owner">The window that owns this Message Window.</param>
        /// <param name="message">The message.</param>
        void ShowMessage(object owner, string message);

        /// <summary>
        /// Shows the message as warning.
        /// </summary>
        /// <param name="owner">The window that owns this Message Window.</param>
        /// <param name="message">The message.</param>
        void ShowWarning(object owner, string message);

        /// <summary>
        /// Shows the message as error.
        /// </summary>
        /// <param name="owner">The window that owns this Message Window.</param>
        /// <param name="message">The message.</param>
        void ShowError(object owner, string message);

        /// <summary>
        /// Shows the specified question.
        /// </summary>
        /// <param name="owner">The window that owns this Message Window.</param>
        /// <param name="message">The question.</param>
        /// <returns><c>true</c> for yes, <c>false</c> for no and <c>null</c> for cancel.</returns>
        bool? ShowQuestion(object owner, string message);

        /// <summary>
        /// Shows the specified yes/no question.
        /// </summary>
        /// <param name="owner">The window that owns this Message Window.</param>
        /// <param name="message">The question.</param>
        /// <returns><c>true</c> for yes and <c>false</c> for no.</returns>
        bool ShowYesNoQuestion(object owner, string message);
    }
}
