using System;
using System.Collections;
using System.Net;
using System.Threading;

namespace HUST.WREIS.Dot3D.Net
{
	/// <summary>
	/// Download queue with priorities
	/// </summary>
	public class DownloadQueue : IDisposable
	{
		public static int MaxQueueLength = 200;
		public static int MaxConcurrentDownloads = 2;

		/// <summary>
		/// Number of downloads that have to fail in a row before the queue stops asking for data.
		/// </summary>
		public static int FailureLimit = 3;

		/// <summary>
		/// Seconds the queue stays idle after the failure limit was reached.
		/// </summary>
		public static int PauseSeconds = 60;

		private static int s_FailuresInARow;
		private static DateTime s_PausedUntil = DateTime.MinValue;
		private static readonly object s_PauseLock = new object();

		/// <summary>
		/// True while the servers are treated as unreachable. A missing tile is queued again as soon as
		/// its failed request is removed, so without this pause every visible tile would ask for its
		/// image over and over while the network is down: thousands of web exceptions and two download
		/// threads that never stop working, which slows the rendering down to a crawl.
		/// </summary>
		public static bool IsPaused
		{
			get
			{
				lock (s_PauseLock)
				{
					return DateTime.Now < s_PausedUntil;
				}
			}
		}

		/// <summary>
		/// Counts a download that did not get an answer from the server.
		/// </summary>
		public static void RegisterFailure(Exception error)
		{
			// A response means the server was reached (404, 500, ...). Only the failures that never got
			// an answer at all say something about the connection.
			var webError = error as WebException;
			if (webError != null && webError.Response != null)
				return;

			lock (s_PauseLock)
			{
				s_FailuresInARow++;
				if (s_FailuresInARow < FailureLimit)
					return;

				s_FailuresInARow = 0;
				s_PausedUntil = DateTime.Now.AddSeconds(PauseSeconds);
			}
		}

		/// <summary>
		/// Counts a download that was answered, which clears the failures counted so far.
		/// </summary>
		public static void RegisterSuccess()
		{
			lock (s_PauseLock)
			{
				s_FailuresInARow = 0;
				s_PausedUntil = DateTime.MinValue;
			}
		}

		private ArrayList m_requests = new ArrayList();
		private ArrayList m_activeDownloads = new ArrayList();

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:HUST.WREIS.Dot3D.Net.DownloadRequest"/> class 
		/// with default data.
		/// </summary>
		public DownloadQueue()
		{
			DownloadRequest.Queue = this;
		}

		/// <summary>
		/// Request for download queue
		/// </summary>
		public ArrayList Requests
		{
			get
			{
				return m_requests;
			}
		}

		/// <summary>
		/// Currently active downloads
		/// </summary>
		public ArrayList ActiveDownloads
		{
			get
			{
				return m_activeDownloads;
			}
		}

		/// <summary>
		/// Remove all requests with a certain owner.
		/// </summary>
		/// <param name="owner"></param>
		public virtual void Clear(object owner)
		{
			lock(m_requests.SyncRoot)
			{
				for(int i=m_requests.Count-1; i>=0; i--)
				{
					DownloadRequest request = (DownloadRequest)m_requests[i];
					if(request.Owner == owner)
					{
						m_requests.RemoveAt(i);
						request.Dispose();
					}
				}
			}
			ServiceDownloadQueue();
		}

		/// <summary>
		/// Finds the next file to download	
		/// </summary>
		protected virtual DownloadRequest GetNextDownloadRequest()
		{
			DownloadRequest bestRequest = null;
			float highestScore = float.MinValue;

			lock(m_requests.SyncRoot)
			{
				for (int i = m_requests.Count-1; i>=0; i--)
				{
					DownloadRequest request = (DownloadRequest) m_requests[i];
					if(request.IsDownloading)
						continue;

					float score = request.CalculateScore();
					if(float.IsNegativeInfinity(score))
					{
						// Request is of no interest anymore, remove it
						m_requests.RemoveAt(i);
						request.Dispose();
						continue;
					}

					if( score > highestScore )
					{
						highestScore = score;
						bestRequest = request;
					}
				}
			}

			return bestRequest;
		}

		/// <summary>
		/// Add a download request to the queue.
		/// </summary>
		public virtual void Add(DownloadRequest newRequest)
		{
			if(newRequest==null)
				throw new NullReferenceException();

			if(IsPaused)
			{
				// The servers are unreachable for the moment, dropping the request here keeps the queue
				// from filling up with entries that would fail again right away.
				newRequest.Dispose();
				return;
			}

			lock(m_requests.SyncRoot)
			{
				foreach(DownloadRequest request in m_requests)
				{
					if(request.Key == newRequest.Key)
					{
						newRequest.Dispose();
						return;
					}
				}
				
				m_requests.Add(newRequest);

				if(m_requests.Count > MaxQueueLength)
				{
					// Remove lowest scoring queued request
					DownloadRequest leastImportantRequest = null;
					float lowestScore = float.MinValue;

					for (int i = m_requests.Count-1; i>=0; i--)
					{
						DownloadRequest request = (DownloadRequest) m_requests[i];
						if(request.IsDownloading)
							continue;

						float score = request.CalculateScore();
						if(score == float.MinValue)
						{
							// Request is of no interest anymore, remove it
							m_requests.Remove(request);
							request.Dispose();
							return;
						}

						if( score < lowestScore )
						{
							lowestScore = score;
							leastImportantRequest = request;
						}
					}

					if(leastImportantRequest != null)
					{
						m_requests.Remove(leastImportantRequest);
						leastImportantRequest.Dispose();
					}
				}
			}

			ServiceDownloadQueue();
		}

		/// <summary>
		/// Removes a request from the download queue.
		/// </summary>
		public virtual void Remove( DownloadRequest request )
		{
			lock(m_requests.SyncRoot)
			{
				for (int i = m_activeDownloads.Count-1; i>=0; i--)
					if(request == m_activeDownloads[i])
						// Already downloading, let it finish
						return;
					
					m_requests.Remove(request);
			}
			request.Dispose();

			ServiceDownloadQueue();
		}

		/// <summary>
		/// Starts downloads when there are threads available
		/// </summary>
		protected virtual void ServiceDownloadQueue()
		{
			if(IsPaused)
				return;

			lock(m_requests.SyncRoot)
			{
				// Remove finished downloads
				for (int i = m_activeDownloads.Count-1; i>=0; i--)
				{
					DownloadRequest request = (DownloadRequest) m_activeDownloads[i];
					if(!request.IsDownloading)
						m_activeDownloads.RemoveAt(i);
				}

				// Start new downloads
				while(m_activeDownloads.Count < MaxConcurrentDownloads)
				{
					DownloadRequest request = GetNextDownloadRequest();
					if(request == null)
						break;

					m_activeDownloads.Add(request);
					request.Start();
				}
			}
		}

		/// <summary>
		/// Callback when download is complete.
		/// </summary>
		internal void OnComplete( DownloadRequest request )
		{
			lock(m_requests.SyncRoot)
			{
				// Remove the finished item from queue
				m_requests.Remove(request);
				request.Dispose();
			}

			// Start next download
			ServiceDownloadQueue();
		}

		#region IDisposable Members

		public void Dispose()
		{
			lock(m_requests.SyncRoot)
			{
				foreach(DownloadRequest request in m_requests)
					request.Dispose();
				m_requests.Clear();
				m_activeDownloads.Clear();
			}

			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
