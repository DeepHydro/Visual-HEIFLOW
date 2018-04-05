// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace  Heiflow.AI
{
    using System;

    /// <summary>
    /// Connection failed exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case if connection to device
    /// has failed.</para>
    /// </remarks>
    /// 
    public class ConnectionFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFailedException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public ConnectionFailedException( string message ) :
            base( message ) { }
    }

    /// <summary>
    /// Connection lost exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case if connection to device
    /// is lost. When the exception is caught, user may need to reconnect to the device.</para>
    /// </remarks>
    /// 
    public class ConnectionLostException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionLostException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public ConnectionLostException( string message ) :
            base( message ) { }
    }

    /// <summary>
    /// Not connected exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case if connection to device
    /// is not established, but user requests for its services.</para>
    /// </remarks>
    /// 
    public class NotConnectedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotConnectedException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public NotConnectedException( string message ) :
            base( message ) { }
    }
}
