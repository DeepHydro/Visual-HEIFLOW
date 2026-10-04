using System;
using System.ComponentModel;
using System.Net;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using HUST.WREIS.Dot3D.Configuration;
using HUST.WREIS.Dot3D;

namespace HUST.WREIS.Dot3D
{
	/// <summary>
    /// Dot3D persisted settings.
	/// </summary>
	public class Engine3DSettings : HUST.WREIS.Dot3D.Configuration.SettingsBase
	{

		public Engine3DSettings() : base()
		{
            AutoCleanupCache = false;
		}
	
        #region Proxy

		// Proxy settings
		private bool useWindowsDefaultProxy = true;
		private string proxyUrl = "";
		private bool useDynamicProxy = false;
		private string proxyUsername = "";
		private string proxyPassword = "";

		[Browsable(true),Category("Proxy")]
		[Description("Whether to use Internet Explorer proxy settings (disables url).")]
		public bool UseWindowsDefaultProxy
		{
			get
			{
				return useWindowsDefaultProxy;
			}
			set
			{
				this.useWindowsDefaultProxy = value;
				UpdateProxySettings();
			}
		}

		[Browsable(true),Category("Proxy")]
		[Description("The address of the proxy, or a proxy script if UseDynamicProxy is enabled.")]
        [XmlIgnore]
		public string ProxyUrl
		{
			get
			{
				return proxyUrl;
			}
			set
			{
				this.proxyUrl = value;
				UpdateProxySettings();
			}
		}

		[Browsable(true),Category("Proxy")]
		[Description("Select if your proxy is determined by a script. If you leave ProxyScriptUrl empty, autodiscovery will be attempted")]
        [XmlIgnore]
        public bool UseDynamicProxy
		{
			get 
			{
				return this.useDynamicProxy;
			}
			set 
			{
				this.useDynamicProxy = value;
				UpdateProxySettings();
			}         
		}

		[Browsable(true),Category("Proxy")]
		[Description("The user name to use if your proxy requires authentication")]
        [XmlIgnore]
        public string ProxyUsername
		{
			get 
			{
				return this.proxyUsername;
			}
			set 
			{
				this.proxyUsername = value;
				UpdateProxySettings();
			}         
		}

		[Browsable(true),Category("Proxy")]
		[Description("The password to use if your proxy requires authentication")]
        [XmlIgnore]
        public string ProxyPassword
		{
			get 
			{
				return this.proxyPassword;
			}
			set 
			{
				this.proxyPassword = value;
				UpdateProxySettings();
			}         
		}

		#endregion

		#region Cache

		// Cache settings
		private string cachePath = "Cache";
        private string iconPath = "\\Resources\\Images\\";
		private int cacheSizeMegaBytes = 10000;
		private TimeSpan cacheCleanupInterval = TimeSpan.FromMinutes(60);
		public static readonly DateTime ApplicationStartTime = DateTime.Now;
		private TimeSpan totalRunTime = TimeSpan.Zero;

		[Browsable(true),Category("Cache")]
		[Description("Directory to use for caching Image and Terrain files.")]
        [XmlIgnore]
		public string CachePath
		{
			get
			{
				if (!Path.IsPathRooted(cachePath))
					return Path.Combine( ApplicationDirectory, cachePath);
				return cachePath;
			}
			set
			{
				cachePath = value;
			}
		}

        /// <summary>
        /// "\\Resources\\Images\\"
        /// </summary>
        [Browsable(true), Category("Cache")]
        [Description("Directory to use for caching Image and Terrain files.")]
        [XmlIgnore]       
        public string IconPath
        {
            get
            {
                return  ApplicationDirectory + iconPath;
            }
            set
            {
                iconPath = value;
            }
        }

		[Browsable(true),Category("Cache")]
		[Description("Upper limit for amount of disk space to allow cache to use (MegaBytes).")]
		public int CacheSizeMegaBytes
		{
			get
			{
				return cacheSizeMegaBytes;
			}
			set
			{
				cacheSizeMegaBytes = value;
			}
		}

		[Browsable(true),Category("Cache")]
		[Description("Controls the frequency of cache cleanup.")]
		[XmlIgnore]
		public TimeSpan CacheCleanupInterval
		{
			get
			{
				return cacheCleanupInterval;
			}
			set
			{
				TimeSpan minimum = TimeSpan.FromMinutes(1);
				if(value < minimum)
					value = minimum;
				cacheCleanupInterval = value;
			}
		}

        [Browsable(true), Category("Cache")]
        [Description("Whether cleanuping cache automatically")]
        public bool AutoCleanupCache
        {
            get;
            set;
        }

		/// <summary>
		/// Because Microsoft forgot to implement TimeSpan in their xml serializer.
		/// </summary>
		[Browsable(false)]
		[XmlElement("CacheCleanupInterval", DataType="duration")] 
		public string CacheCleanupIntervalXml 
		{     
			get     
			{         
				if(cacheCleanupInterval < TimeSpan.FromSeconds(1) )
					return null;

				return XmlConvert.ToString(cacheCleanupInterval);         
			}     
			set     
			{
				if(value == null || value == string.Empty)
					return;         

				cacheCleanupInterval = XmlConvert.ToTimeSpan(value);
			} 
		} 

		[Browsable(true),Category("Cache")]
		[Description("Total amount of time the application has been running.")]
		[XmlIgnore]
		public TimeSpan TotalRunTime
		{
			get
			{
				return totalRunTime + DateTime.Now.Subtract(ApplicationStartTime);
			}
			set
			{
				value = totalRunTime;
			}
		}

		/// <summary>
		/// Because Microsoft forgot to implement TimeSpan in their xml serializer.
		/// </summary>
		[Browsable(false)]
		[XmlElement("TotalRunTime", DataType="duration")] 
		public string TotalRunTimeXml 
		{     
			get     
			{         
				if(TotalRunTime < TimeSpan.FromSeconds(1) )
					return null;

				return XmlConvert.ToString(TotalRunTime);         
			}     
			set     
			{         
				if(value == null || value == string.Empty)
					return;         

				totalRunTime = XmlConvert.ToTimeSpan(value);
			} 
		} 

		#endregion

		#region Plugin

        private System.Collections.Generic.List<string> pluginsLoadedOnStartup = new System.Collections.Generic.List<string>();

		[Browsable(true),Category("Plugin")]
		[Description("List of plugins loaded at startup.")]
		public System.Collections.Generic.List<string> PluginsLoadedOnStartup
		{
			get
			{
				return pluginsLoadedOnStartup;
			}
            set
            {
                pluginsLoadedOnStartup = value;
            }
		}

		#endregion

		#region Miscellaneous settings

		// Misc
		private string defaultWorld = "Earth";
		// default is to show the Configuration Wizard at startup
		private bool configurationWizardAtStartup = true;

		[Browsable(true),Category("Miscellaneous")]
		[Description("World to load on startup.")]
		public string DefaultWorld
		{
			get
			{
				return defaultWorld;
			}
			set
			{
				defaultWorld = value;
			}
		}

		[Browsable(true),Category("Miscellaneous")]
		[Description("Show Configuration Wizard on program startup.")]
		public bool ConfigurationWizardAtStartup
		{
			get
			{
				return configurationWizardAtStartup;
			}
			set
			{
				configurationWizardAtStartup = value;
			}
		}

		#endregion

		#region File System Settings

		// File system settings
		private string configPath = "Config";
		private string dataPath = "Data";
        private bool validateXML = true;
		
		[Browsable(true),Category("FileSystem")]
		[Description("Location where configuration files are stored.")]
        [XmlIgnore]
		public string ConfigPath
		{
			get
			{
				if (!Path.IsPathRooted(configPath))
					return Path.Combine( ApplicationDirectory, configPath);
				return configPath;
			}
			set
			{
				configPath = value;
			}
		}

		[Browsable(true),Category("FileSystem")]
		[Description("Location where data files are stored.")]
        [XmlIgnore]
		public string DataPath
		{
			get
			{
				if (!Path.IsPathRooted(dataPath))
					return Path.Combine( ApplicationDirectory, dataPath);
				return dataPath;
			}
			set
			{
				dataPath = value;
			}
		}

        [Browsable(true), Category("FileSystem")]
        [Description("Validate XML Data on load.")]
        public bool ValidateXML
        {
            get
            {
                return validateXML;
            }
            set
            {
                validateXML = value;
            }
        }

		/// <summary>
		/// Main application base directory
		/// </summary>
		public readonly string ApplicationDirectory = Path.GetDirectoryName(Application.ExecutablePath);

		#endregion

        [Browsable(true), Category("UI")]
        [XmlIgnore]
        public Control SceneWindow { get; set; }

		/// <summary>
		/// Propagate proxy-related settings to statics in WebDownload class
		/// </summary>
		void UpdateProxySettings()
		{
			HUST.WREIS.Dot3D.Net.WebDownload.useWindowsDefaultProxy = this.useWindowsDefaultProxy;
			HUST.WREIS.Dot3D.Net.WebDownload.useDynamicProxy        = this.useDynamicProxy;
			HUST.WREIS.Dot3D.Net.WebDownload.proxyUrl               = this.proxyUrl;
			HUST.WREIS.Dot3D.Net.WebDownload.proxyUserName          = this.proxyUsername;
			HUST.WREIS.Dot3D.Net.WebDownload.proxyPassword          = this.proxyPassword;
		}

		// comment out ToString() to have namespace+class name being used as filename
		public override string ToString()
		{
			return "Dot3D";
		}
	}
}
