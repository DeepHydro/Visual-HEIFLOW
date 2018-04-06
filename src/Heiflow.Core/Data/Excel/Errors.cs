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

namespace Excel
{
	internal static class Errors
	{
		public const string ErrorStreamWorkbookNotFound = "Error: Neither stream 'Workbook' nor 'Book' was found in file.";
		public const string ErrorWorkbookIsNotStream = "Error: Workbook directory entry is not a Stream.";
		public const string ErrorWorkbookGlobalsInvalidData = "Error reading Workbook Globals - Stream has invalid data.";
		public const string ErrorFATBadSector = "Error reading as FAT table : There's no such sector in FAT.";
		public const string ErrorFATRead = "Error reading stream from FAT area.";
		public const string ErrorHeaderSignature = "Error: Invalid file signature.";
		public const string ErrorHeaderOrder = "Error: Invalid byte order specified in header.";
		public const string ErrorBIFFRecordSize = "Error: Buffer size is less than minimum BIFF record size.";
		public const string ErrorBIFFBufferSize = "BIFF Stream error: Buffer size is less than entry length.";
		public const string ErrorBIFFIlegalBefore = "BIFF Stream error: Moving before stream start.";
		public const string ErrorBIFFIlegalAfter = "BIFF Stream error: Moving after stream end.";

		public const string ErrorDirectoryEntryArray = "Directory Entry error: Array is too small.";
	}
}
