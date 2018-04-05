// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

#region Using



#endregion

namespace Heiflow.Spatial.Converters.WellKnownText
{
    /// <summary>
    /// Represents the type of token created by the StreamTokenizer class.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Indicates that the token is a word.
        /// </summary>
        Word,
        /// <summary>
        /// Indicates that the token is a number. 
        /// </summary>
        Number,
        /// <summary>
        /// Indicates that the end of line has been read. The field can only have this value if the eolIsSignificant method has been called with the argument true. 
        /// </summary>
        Eol,
        /// <summary>
        /// Indicates that the end of the input stream has been reached.
        /// </summary>
        Eof,
        /// <summary>
        /// Indictaes that the token is white space (space, tab, newline).
        /// </summary>
        Whitespace,
        /// <summary>
        /// Characters that are not whitespace, numbers, etc...
        /// </summary>
        Symbol
    }
}