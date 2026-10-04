using HUST.WREIS.Dot3D.Renderable;
using System;
using System.Globalization;

namespace HUST.WREIS.Dot3D
{
	/// <summary>
	/// Formats urls for images stored in NLT-style
	/// </summary>
	public class NltImageStore : ImageStore
	{
		#region Private Members

		string m_dataSetName;
		string m_serverUri;

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.ImageTileService"/> class.
		/// </summary>
		/// <param name="dataSetName"></param>
		/// <param name="serverUri"></param>
		public NltImageStore(
			string dataSetName,
			string serverUri)
		{
            IsDownloadableLayer = true;
			m_serverUri = serverUri;
			m_dataSetName = dataSetName;
		}

		public override string GetDownloadUrl(QuadTile qt)
		{
			return string.Format(CultureInfo.InvariantCulture, 
				"{0}?T={1}&L={2}&X={3}&Y={4}", m_serverUri, 
				m_dataSetName, qt.Level, qt.Col, qt.Row);
		}
	}
}
