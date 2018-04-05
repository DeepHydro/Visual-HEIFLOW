// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core;
using System;

namespace Heiflow.Core.Plugin
{
 	/// <summary>
	/// Base class to be derived by all plugins (loaded by PluginCompiler)
	/// Keep as light-weight as possible to keep plugin simple to write.
	/// </summary>
	public abstract class Plugin
	{
		/// <summary>
		/// Handle to the  Application object
		/// </summary>
        protected IApplication m_Application;

		/// <summary>
		/// The directory from which this plugin was loaded.
		/// </summary>
		protected string m_PluginDirectory;

		/// <summary>
		/// Plugin running flag (true while running, reset when exiting plugin)
		/// </summary>
		protected bool m_isLoaded;



		/// <summary>
		/// Reference to the main application object.  
		/// Deprecated: Use ParentApplication property instead!  
		/// TODO: Remove this to avoid name collision
		/// </summary>
        public virtual IApplication Application
		{
			get
			{
				return m_Application;
			}
		}

		/// <summary>
		/// Reference to the main application object.
		/// </summary>
        public virtual IApplication ParentApplication
		{
			get
			{
				return m_Application;
			}
		}

		/// <summary>
		/// The location this plugin was loaded from.
		/// </summary>
		public virtual string PluginDirectory
		{
			get
			{
				return m_PluginDirectory;
			}
		}

		/// <summary>
		/// Whether the plugin is currently running.
		/// </summary>
		public virtual bool IsLoaded
		{
			get
			{
				return m_isLoaded;
			}
		}

        public bool Visible 
        { get; set; }

		/// <summary>
		/// Load the plugin.  This is the plugin entry point.
		/// </summary>
		/// <param name="parent">The Application.</param>
		public virtual void Load()
		{
			// Override with plugin initialization code.
		}

		/// <summary>
		/// Unload the plugin. Plugins that modify  or 
		/// runs in background should override this method.
		/// </summary>
		public virtual void Unload()
		{
			// Override with plugin dispose code.
		}

        public virtual void Show()
        {
        }

        public virtual void Hide()
        {
        }
		/// <summary>
		/// Base class load, calls Load. 
		/// </summary>
		/// <param name="parent"></param>
        public virtual void PluginLoad(IApplication parent, string pluginDirectory)
		{
			if(m_isLoaded)
				// Already loaded
				return;
			m_Application = parent;
			m_PluginDirectory = pluginDirectory;
			Load();
			m_isLoaded = true;
		}

		/// <summary>
		/// Base class unload, calls Unload. 
		/// </summary>
		public virtual void PluginUnload()
		{
			Unload();
			m_isLoaded = false;
		}
	}
}
