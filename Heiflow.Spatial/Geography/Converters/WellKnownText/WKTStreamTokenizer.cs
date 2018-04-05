// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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