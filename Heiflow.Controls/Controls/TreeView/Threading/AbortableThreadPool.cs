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

// Stephen Toub
// stoub@microsoft.com

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Heiflow.Controls.Threading
{
	public class AbortableThreadPool
	{
		private LinkedList<WorkItem> _callbacks = new LinkedList<WorkItem>();
		private Dictionary<WorkItem, Thread> _threads = new Dictionary<WorkItem, Thread>();

		public WorkItem QueueUserWorkItem(WaitCallback callback)
		{
			return QueueUserWorkItem(callback, null);
		}

		public WorkItem QueueUserWorkItem(WaitCallback callback, object state)
		{
			if (callback == null) throw new ArgumentNullException("callback");

			WorkItem item = new WorkItem(callback, state, ExecutionContext.Capture());
			lock (_callbacks)
			{
				_callbacks.AddLast(item);
			}
			ThreadPool.QueueUserWorkItem(new WaitCallback(HandleItem));
			return item;
		}

		private void HandleItem(object ignored)
		{
			WorkItem item = null;
			try
			{
				lock (_callbacks)
				{
					if (_callbacks.Count > 0)
					{
						item = _callbacks.First.Value;
						_callbacks.RemoveFirst();
					}
					if (item == null)
						return;
					_threads.Add(item, Thread.CurrentThread);

				}
				ExecutionContext.Run(item.Context,
					delegate { item.Callback(item.State); }, null);
			}
			finally
			{
				lock (_callbacks)
				{
					if (item != null)
						_threads.Remove(item);
				}
			}
		}

		public bool IsMyThread(Thread thread)
		{
			lock (_callbacks)
			{
				foreach (Thread t in _threads.Values)
				{
					if (t == thread)
						return true;
				}
				return false;
			}
		}

		public WorkItemStatus Cancel(WorkItem item, bool allowAbort)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			lock (_callbacks)
			{
				LinkedListNode<WorkItem> node = _callbacks.Find(item);
				if (node != null)
				{
					_callbacks.Remove(node);
					return WorkItemStatus.Queued;
				}
				else if (_threads.ContainsKey(item))
				{
					if (allowAbort)
					{
						_threads[item].Abort();
						_threads.Remove(item);
						return WorkItemStatus.Aborted;
					}
					else
						return WorkItemStatus.Executing;
				}
				else
					return WorkItemStatus.Completed;
			}
		}

		public void CancelAll(bool allowAbort)
		{
			lock (_callbacks)
			{
				_callbacks.Clear();
				if (allowAbort)
				{
					foreach (Thread t in _threads.Values)
						t.Abort();
					_threads.Clear();
				}
			}
		}
	}
}
