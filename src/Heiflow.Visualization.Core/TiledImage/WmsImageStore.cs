using Heiflow.Core;
using System;
using System.Globalization;
using System.Net;

namespace HUST.WREIS.Dot3D.Net.Wms
{
	/// <summary>
	/// Calculates URLs for WMS layers.
	/// </summary>
	public class WmsImageStore : ImageStore
	{
		/// <summary>
		/// Image servers are https only these days and refuse a handshake below TLS 1.2, while what a
		/// request offers by default on a 4.5 runtime is SSL 3 and TLS 1.0, so every GetMap dies with
		/// "Could not create SSL/TLS secure channel". It is turned on once here rather than per request,
		/// and it is added to whatever is already set so an older protocol another download needs stays
		/// available. See TerrariumDownloadRequest, which has the same switch for the terrain tiles.
		/// </summary>
		static WmsImageStore()
		{
			try
			{
				ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
			}
			catch (Exception caught)
			{
				Log.Write(Log.Levels.Warning, "WMS",
					"TLS 1.2 could not be turned on, https tiles may fail to load: " + caught.Message);
			}
		}

		#region Private Members
		
		string m_serverGetMapUrl;
		string m_wmsLayerName;
		string m_wmsLayerStyle;
		string m_imageFormat;
		string m_version;
		int	m_textureSizePixels = 512;

		#endregion	

		#region Properties
		
		public override bool IsDownloadableLayer
		{
			get
			{
				return true;
			}
		}

		public virtual string ServerGetMapUrl
		{
			get
			{
				return m_serverGetMapUrl;
			}
			set
			{
				m_serverGetMapUrl = value;
			}
		}

		public virtual string WMSLayerName
		{
			get
			{
				return m_wmsLayerName;
			}
			set
			{
				m_wmsLayerName = value;
			}
		}

		public virtual string WMSLayerStyle
		{
			get
			{
				return m_wmsLayerStyle;
			}
			set
			{
				m_wmsLayerStyle = value;
			}
		}

		public virtual string ImageFormat
		{
			get
			{
				return m_imageFormat;
			}
			set
			{
				m_imageFormat = value;
			}
		}

		public virtual string Version
		{
			get
			{
				return m_version;
			}
			set
			{
				m_version = value;
			}
		}

		/// <summary>
		/// Bitmap width/height
		/// </summary>
		public int TextureSizePixels
		{
			get
			{
				return m_textureSizePixels;
			}
			set
			{
				m_textureSizePixels = value;
			}
		}

		#endregion

		#region Public Methods

		public override string GetDownloadUrl(HUST.WREIS.Dot3D.Renderable.QuadTile qt)
		{
			if(m_serverGetMapUrl.IndexOf('?')>=0)
			{
				// Allow custom format string url
				// http://server.net/path?imageformat=png&width={WIDTH}&north={NORTH}...
				string url = m_serverGetMapUrl;
				url = url.Replace("{WIDTH}", m_textureSizePixels.ToString(CultureInfo.InvariantCulture));
				url = url.Replace("{HEIGHT}", m_textureSizePixels.ToString(CultureInfo.InvariantCulture));
				url = url.Replace("{WEST}", qt.West.ToString(CultureInfo.InvariantCulture));
				url = url.Replace("{EAST}", qt.East.ToString(CultureInfo.InvariantCulture));
				url = url.Replace("{NORTH}", qt.North.ToString(CultureInfo.InvariantCulture));
				url = url.Replace("{SOUTH}", qt.South.ToString(CultureInfo.InvariantCulture));

				return url;
			}
			else
			{
				string url = string.Format(CultureInfo.InvariantCulture,
                    "{0}?request=GetMap&layers={1}&crs=CRS:84&width={2}&height={3}&bbox={4},{5},{6},{7}&format={8}&version={9}&styles={10}&",
					m_serverGetMapUrl,
					m_wmsLayerName, 
					m_textureSizePixels, 
					m_textureSizePixels, 
					qt.West, qt.South, qt.East, qt.North,  
					m_imageFormat, 
					m_version,  
					m_wmsLayerStyle );

				return url;
			}
		}

		#endregion
	}
}
