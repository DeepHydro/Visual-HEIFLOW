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

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;


namespace Excel.Core.OpenXmlFormat
{
    internal class XlsxWorkbook
    {
        private const string N_sheet = "sheet";
        private const string N_t = "t";
        private const string N_si = "si";
        private const string N_cellXfs = "cellXfs";
        private const string N_numFmts = "numFmts";

        private const string A_sheetId = "sheetId";
        private const string A_name = "name";
        private const string A_rid = "r:id";

        private const string N_rel = "Relationship";
        private const string A_id = "Id";
        private const string A_target = "Target";

        private XlsxWorkbook() { }

        public XlsxWorkbook(Stream workbookStream, Stream relsStream, Stream sharedStringsStream, Stream stylesStream)
        {
            if (null == workbookStream) throw new ArgumentNullException();

            ReadWorkbook(workbookStream);

            ReadWorkbookRels(relsStream);

            ReadSharedStrings(sharedStringsStream);

            ReadStyles(stylesStream);
        }

        private List<XlsxWorksheet> sheets;

        public List<XlsxWorksheet> Sheets
        {
            get { return sheets; }
            set { sheets = value; }
        }

        private XlsxSST _SST;

        public XlsxSST SST
        {
            get { return _SST; }
        }

        private XlsxStyles _Styles;

        public XlsxStyles Styles
        {
            get { return _Styles; }
        }


        private void ReadStyles(Stream xmlFileStream)
        {
            if (null == xmlFileStream) return;

            _Styles = new XlsxStyles();

            bool rXlsxNumFmt = false;

            using (XmlReader reader = XmlReader.Create(xmlFileStream))
            {
                while (reader.Read())
                {
                    if (!rXlsxNumFmt && reader.NodeType == XmlNodeType.Element && reader.LocalName == N_numFmts)
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Depth == 1) break;

                            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == XlsxNumFmt.N_numFmt)
                            {
                                _Styles.NumFmts.Add(
                                    new XlsxNumFmt(
                                        int.Parse(reader.GetAttribute(XlsxNumFmt.A_numFmtId)),
                                        reader.GetAttribute(XlsxNumFmt.A_formatCode)
                                        ));
                            }
                        }

                        rXlsxNumFmt = true;
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == N_cellXfs)
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Depth == 1) break;

                            if (reader.NodeType == XmlNodeType.Element && reader.LocalName == XlsxXf.N_xf)
                            {
	                            var xfId = reader.GetAttribute(XlsxXf.A_xfId);
	                            var numFmtId = reader.GetAttribute(XlsxXf.A_numFmtId);
								
								_Styles.CellXfs.Add(
                                    new XlsxXf(
										xfId == null ? -1 : int.Parse(xfId),
										numFmtId == null ? -1 : int.Parse(numFmtId),
                                        reader.GetAttribute(XlsxXf.A_applyNumberFormat)
                                        ));
                            }
                        }

                        break;
                    }
                }

                xmlFileStream.Close();
            }
        }

        private void ReadSharedStrings(Stream xmlFileStream)
        {
            if (null == xmlFileStream) return;

            _SST = new XlsxSST();

            using (XmlReader reader = XmlReader.Create(xmlFileStream))
            {
                // There are multiple <t> in a <si>. Concatenate <t> within an <si>.
                bool bAddStringItem = false;
                string sStringItem = "";

                while (reader.Read())
                {
                    // There are multiple <t> in a <si>. Concatenate <t> within an <si>.
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == N_si)
                    {
                        // Do not add the string item until the next string item is read.
                        if (bAddStringItem)
                        {
                            // Add the string item to XlsxSST.
                            _SST.Add(sStringItem);
                        }
                        else
                        {
                            // Add the string items from here on.
                            bAddStringItem = true;
                        }

                        // Reset the string item.
                        sStringItem = "";
                    }

                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == N_t)
                    {
                        // Append to the string item.
                        sStringItem += reader.ReadElementContentAsString();
                    }
                }
                // Do not add the last string item unless we have read previous string items.
                if (bAddStringItem)
                {
                    // Add the string item to XlsxSST.
                    _SST.Add(sStringItem);
                }

                xmlFileStream.Close();
            }
        }


        private void ReadWorkbook(Stream xmlFileStream)
        {
            sheets = new List<XlsxWorksheet>();

            using (XmlReader reader = XmlReader.Create(xmlFileStream))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == N_sheet)
                    {
                        sheets.Add(new XlsxWorksheet(
                                               reader.GetAttribute(A_name),
                                               int.Parse(reader.GetAttribute(A_sheetId)), reader.GetAttribute(A_rid)));
                    }

                }

                xmlFileStream.Close();
            }

        }

        private void ReadWorkbookRels(Stream xmlFileStream)
        {
            using (XmlReader reader = XmlReader.Create(xmlFileStream))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == N_rel)
                    {
                        string rid = reader.GetAttribute(A_id);

                        for (int i = 0; i < sheets.Count; i++)
                        {
                            XlsxWorksheet tempSheet = sheets[i];

                            if (tempSheet.RID == rid)
                            {
                                tempSheet.Path = reader.GetAttribute(A_target);
                                sheets[i] = tempSheet;
                                break;
                            }
                        }
                    }

                }

                xmlFileStream.Close();
            }
        }

    }
}
