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