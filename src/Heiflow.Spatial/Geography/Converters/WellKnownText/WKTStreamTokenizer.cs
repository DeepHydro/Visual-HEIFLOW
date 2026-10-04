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

using System;
using System.IO;
using Heiflow.Spatial.Converters.WellKnownText.IO;
using System.Globalization;

#endregion

namespace Heiflow.Spatial.Converters.WellKnownText
{
    /// <summary>
    /// Reads a stream of Well Known Text (wkt) string and returns a stream of tokens.
    /// </summary>
    public class WktStreamTokenizer : StreamTokenizer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the WktStreamTokenizer class.
        /// </summary>
        /// <remarks>The WktStreamTokenizer class ais in reading WKT streams.</remarks>
        /// <param name="reader">A TextReader that contains </param>
        public WktStreamTokenizer(TextReader reader) : base(reader, true)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads a token and checks it is what is expected.
        /// </summary>
        /// <param name="expectedToken">The expected token.</param>
        public void ReadToken(string expectedToken)
        {
            NextToken();
            if (GetStringValue() != expectedToken)
            {
                throw new Exception(String.Format(CultureInfo.InvariantCulture,
                                                  "Expecting ('{3}') but got a '{0}' at line {1} column {2}.",
                                                  GetStringValue(), LineNumber, Column, expectedToken));
            }
        }

        /// <summary>
        /// Reads a string inside double quotes.
        /// </summary>
        /// <remarks>
        /// White space inside quotes is preserved.
        /// </remarks>
        /// <returns>The string inside the double quotes.</returns>
        public string ReadDoubleQuotedWord()
        {
            string word = "";
            ReadToken("\"");
            NextToken(false);
            while (GetStringValue() != "\"")
            {
                word = word + GetStringValue();
                NextToken(false);
            }
            return word;
        }

        /// <summary>
        /// Reads the authority and authority code.
        /// </summary>
        /// <param name="authority">String to place the authority in.</param>
        /// <param name="authorityCode">String to place the authority code in.</param>
        public void ReadAuthority(ref string authority, ref long authorityCode)
        {
            //AUTHORITY["EPGS","9102"]]
            if (GetStringValue() != "AUTHORITY")
                ReadToken("AUTHORITY");
            ReadToken("[");
            authority = ReadDoubleQuotedWord();
            ReadToken(",");
            long.TryParse(ReadDoubleQuotedWord(), out authorityCode);
            ReadToken("]");
        }

        #endregion
    }
}