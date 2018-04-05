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

using System.Collections.Generic;

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents Globals section of workbook
	/// </summary>
	internal class XlsWorkbookGlobals
	{
		private readonly List<XlsBiffRecord> m_ExtendedFormats = new List<XlsBiffRecord>();
		private readonly List<XlsBiffRecord> m_Fonts = new List<XlsBiffRecord>();
        private readonly Dictionary<ushort, XlsBiffFormatString> m_Formats = new Dictionary<ushort, XlsBiffFormatString>();
		private readonly List<XlsBiffBoundSheet> m_Sheets = new List<XlsBiffBoundSheet>();
		private readonly List<XlsBiffRecord> m_Styles = new List<XlsBiffRecord>();
		private XlsBiffSimpleValueRecord m_Backup;
		private XlsBiffSimpleValueRecord m_CodePage;
		private XlsBiffRecord m_Country;
		private XlsBiffRecord m_DSF;
		private XlsBiffRecord m_ExtSST;
		private XlsBiffInterfaceHdr m_InterfaceHdr;

		private XlsBiffRecord m_MMS;
		private XlsBiffSST m_SST;

		private XlsBiffRecord m_WriteAccess;

		public XlsBiffInterfaceHdr InterfaceHdr
		{
			get { return m_InterfaceHdr; }
			set { m_InterfaceHdr = value; }
		}

		public XlsBiffRecord MMS
		{
			get { return m_MMS; }
			set { m_MMS = value; }
		}

		public XlsBiffRecord WriteAccess
		{
			get { return m_WriteAccess; }
			set { m_WriteAccess = value; }
		}

		public XlsBiffSimpleValueRecord CodePage
		{
			get { return m_CodePage; }
			set { m_CodePage = value; }
		}

		public XlsBiffRecord DSF
		{
			get { return m_DSF; }
			set { m_DSF = value; }
		}

		public XlsBiffRecord Country
		{
			get { return m_Country; }
			set { m_Country = value; }
		}

		public XlsBiffSimpleValueRecord Backup
		{
			get { return m_Backup; }
			set { m_Backup = value; }
		}

		public List<XlsBiffRecord> Fonts
		{
			get { return m_Fonts; }
		}

        public Dictionary<ushort, XlsBiffFormatString> Formats
		{
			get { return m_Formats; }
		}

        

		public List<XlsBiffRecord> ExtendedFormats
		{
			get { return m_ExtendedFormats; }
		}

		public List<XlsBiffRecord> Styles
		{
			get { return m_Styles; }
		}

		public List<XlsBiffBoundSheet> Sheets
		{
			get { return m_Sheets; }
		}

		/// <summary>
		/// Shared String Table of workbook
		/// </summary>
		public XlsBiffSST SST
		{
			get { return m_SST; }
			set { m_SST = value; }
		}

		public XlsBiffRecord ExtSST
		{
			get { return m_ExtSST; }
			set { m_ExtSST = value; }
		}
	}
}