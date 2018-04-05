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
using System.Text;

namespace Excel.Log
{
/// <summary>
/// Custom interface for logging messages
/// </summary>
public partial interface ILog
{
    /// <summary>
    /// Initializes the instance for the logger name
    /// </summary>
    /// <param name="loggerName">Name of the logger</param>
    void InitializeFor(string loggerName);
    
    /// <summary>
    /// Debug level of the specified message. The other method is preferred since the execution is deferred.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="formatting">The formatting.</param>
    void Debug(string message, params object[] formatting);

    /// <summary>
    /// Info level of the specified message. The other method is preferred since the execution is deferred.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="formatting">The formatting.</param>
    void Info(string message, params object[] formatting);

    /// <summary>
    /// Warn level of the specified message. The other method is preferred since the execution is deferred.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="formatting">The formatting.</param>
    void Warn(string message, params object[] formatting);

    /// <summary>
    /// Error level of the specified message. The other method is preferred since the execution is deferred.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="formatting">The formatting.</param>
    void Error(string message, params object[] formatting);

    /// <summary>
    /// Fatal level of the specified message. The other method is preferred since the execution is deferred.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="formatting">The formatting.</param>
    void Fatal(string message, params object[] formatting);

}

/// <summary>
/// Ensures a default constructor for the logger type
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ILog<T> where T : new()
{
}

}

