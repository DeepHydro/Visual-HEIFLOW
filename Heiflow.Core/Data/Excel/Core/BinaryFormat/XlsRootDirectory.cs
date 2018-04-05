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
using System.Collections.ObjectModel;

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents Root Directory in file
	/// </summary>
	internal class XlsRootDirectory
	{
		private readonly List<XlsDirectoryEntry> m_entries;
		private readonly XlsDirectoryEntry m_root;

		/// <summary>
		/// Creates Root Directory catalog from XlsHeader
		/// </summary>
		/// <param name="hdr">XlsHeader object</param>
		public XlsRootDirectory(XlsHeader hdr)
		{
			XlsStream stream = new XlsStream(hdr, hdr.RootDirectoryEntryStart, false, null);
			byte[] array = stream.ReadStream();
			byte[] tmp;
			XlsDirectoryEntry entry;
			List<XlsDirectoryEntry> entries = new List<XlsDirectoryEntry>();
			for (int i = 0; i < array.Length; i += XlsDirectoryEntry.Length)
			{
				tmp = new byte[XlsDirectoryEntry.Length];
				Buffer.BlockCopy(array, i, tmp, 0, tmp.Length);
				entries.Add(new XlsDirectoryEntry(tmp, hdr));
			}
			m_entries = entries;
			for (int i = 0; i < entries.Count; i++)
			{
				entry = entries[i];

				//Console.WriteLine("Directory Entry:{0} type:{1}, firstsector:{2}, streamSize:{3}, isEntryMiniStream:{4}", entry.EntryName, entry.EntryType.ToString(), entry.StreamFirstSector, entry.StreamSize, entry.IsEntryMiniStream);
				if (m_root == null && entry.EntryType == STGTY.STGTY_ROOT)
					m_root = entry;
				if (entry.ChildSid != (uint)FATMARKERS.FAT_FreeSpace)
					entry.Child = entries[(int)entry.ChildSid];
				if (entry.LeftSiblingSid != (uint)FATMARKERS.FAT_FreeSpace)
					entry.LeftSibling = entries[(int)entry.LeftSiblingSid];
				if (entry.RightSiblingSid != (uint)FATMARKERS.FAT_FreeSpace)
					entry.RightSibling = entries[(int)entry.RightSiblingSid];
			}
			stream.CalculateMiniFat(this);
		}

		/// <summary>
		/// Returns all entries in Root Directory
		/// </summary>
		public ReadOnlyCollection<XlsDirectoryEntry> Entries
		{
			get { return m_entries.AsReadOnly(); }
		}

		/// <summary>
		/// Returns the Root Entry
		/// </summary>
		public XlsDirectoryEntry RootEntry
		{
			get { return m_root; }
		}

		/// <summary>
		/// Searches for first matching entry by its name
		/// </summary>
		/// <param name="EntryName">String name of entry</param>
		/// <returns>Entry if found, null otherwise</returns>
		public XlsDirectoryEntry FindEntry(string EntryName)
		{
			foreach (XlsDirectoryEntry e in m_entries)
			{
                if (string.Equals(e.EntryName, EntryName, StringComparison.CurrentCultureIgnoreCase))
					return e;
			}
			return null;
		}
	}
}
