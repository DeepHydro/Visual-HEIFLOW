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
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace Heiflow.Core.Plugin
{
	/// <summary>
	/// Stores information on a plugin.
	/// </summary>
	public class PluginInfo
	{
		Plugin m_plugin;
		string m_fullPath;
		string m_name;
		string m_description;
		string m_developer;
		string m_webSite;
		string m_references;

    //    public List<string> LoadedPluginNames { set; private get; }

		/// <summary>
		/// The plugin instance.
		/// </summary>
		public Plugin Plugin
		{
			get
			{
				return m_plugin;
			}
			set
			{
				m_plugin = value;
			}
		}

		/// <summary>
		/// Directory and filename of the plugin.
		/// </summary>
		public string FullPath
		{
			get
			{
				return m_fullPath;
			}
			set
			{
				m_fullPath = value;
			}
		}

		/// <summary>
		/// The plugin ID
		/// </summary>
		public string ID
		{
			get
			{
				if(m_fullPath!=null)
					return Path.GetFileNameWithoutExtension(m_fullPath);
				return m_name;
			}
		}

		/// <summary>
		/// The plugin name (from plugin comment header "NAME" tag)
		/// </summary>
		public string Name
		{
			get
			{
				if(m_name==null)
					ReadMetaData();

				return m_name;
			}
			set
			{
				m_name=value;
			}
		}

		/// <summary>
		/// The plugin description (from plugin comment header "DESCRIPTION" tag)
		/// </summary>
		public string Description
		{
			get
			{
				if(m_description==null)
					ReadMetaData();

				return m_description;
			}
			set
			{
				m_description=value;
			}
		}

		/// <summary>
		/// The plugin developer's name (from plugin comment header "DEVELOPER" tag)
		/// </summary>
		public string Developer
		{
			get
			{
				if(m_developer==null)
					ReadMetaData();

				return m_developer;
			}
		}

		/// <summary>
		/// The plugin web site url (from plugin comment header "WEBSITE" tag)
		/// </summary>
		public string WebSite
		{
			get
			{
				if(m_webSite==null)
					ReadMetaData();

				return m_webSite;
			}
		}

		/// <summary>
		/// Comma separated list of additional libraries this plugin requires a reference to.
		/// </summary>
		public string References
		{
			get
			{
				if(m_references==null)
					ReadMetaData();

				return m_references;
			}
		}

		/// <summary>
		/// Check whether a plugin is currently loaded.
		/// </summary>
		public bool IsCurrentlyLoaded
		{
			get
			{
				if(m_plugin==null)
					return false;
				return m_plugin.IsLoaded;
			}
		}

		/// <summary>
		/// Set always load on application startup flag for the plugin.
		/// </summary>
        public bool IsLoadedAtStartup
        {
            get;
            set;
        }

		/// <summary>
		/// Reads strings from the source file header tags
		/// </summary>
		private void ReadMetaData()
		{
			try
			{
				if(m_fullPath==null)
					// Source code comments not available
					return;

				// Initialize variables (prevents more than one call here)
				if(m_name==null)
					m_name = "";
				if(m_description==null)
					m_description = "";
				if(m_developer==null)
					m_developer = "";
				if(m_webSite==null)
					m_webSite = "";
				if(m_references==null)
					m_references = "";

				using(TextReader tr = File.OpenText(m_fullPath))
				{
					while(true)
					{
						string line = tr.ReadLine();
						if(line==null)
							break;

						FindTagInLine(line, "NAME", ref m_name);
						FindTagInLine(line, "DESCRIPTION", ref m_description);
						FindTagInLine(line, "DEVELOPER", ref m_developer);
						FindTagInLine(line, "WEBSITE", ref m_webSite);
						FindTagInLine(line, "REFERENCES", ref m_references);
					}
				}
			}
			catch(IOException)
			{
				// Ignore
			}
			finally
			{
				if(m_name.Length==0)
					// If name is not defined, use the filename
					m_name = Path.GetFileNameWithoutExtension(m_fullPath);
			}
		}

		/// <summary>
		/// Extract tag value from input source line.
		/// </summary>
		static void FindTagInLine(string inputLine, string tag, ref string value)
		{
			if(value!=string.Empty)
				// Already found
				return;

			// Pattern: _TAG:_<value>EOL
			tag = " " + tag + ": ";
			int index = inputLine.IndexOf(tag);
			if(index<0)
				return;

			value = inputLine.Substring(index+tag.Length);
		}
	}
}
