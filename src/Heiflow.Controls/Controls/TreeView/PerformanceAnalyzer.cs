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
using System.Diagnostics;

namespace Heiflow.Controls
{
	/// <summary>
	/// Is used to analyze code performance
	/// </summary>
	public static class PerformanceAnalyzer
	{
		public class PerformanceInfo
		{
			private string _name;
			public string Name
			{
				get { return _name; }
			}

			private int _count = 0;
			public int Count
			{
				get { return _count; }
				set { _count = value; }
			}

			private double _totalTime = 0;
			public double TotalTime
			{
				get { return _totalTime; }
				set { _totalTime = value; }
			}

			private Int64 _start;
			public Int64 Start
			{
				get { return _start; }
				set { _start = value; }
			}

			public PerformanceInfo(string name)
			{
				_name = name;
			}
		}

		private static Dictionary<string, PerformanceInfo> _performances = new Dictionary<string, PerformanceInfo>();

		public static IEnumerable<PerformanceInfo> Performances
		{
			get
			{
				return _performances.Values;
			}
		}

		[Conditional("DEBUG")]
		public static void Start(string pieceOfCode)
		{
			PerformanceInfo info = null;
			lock(_performances)
			{
				if (_performances.ContainsKey(pieceOfCode))
					info = _performances[pieceOfCode];
				else
				{
					info = new PerformanceInfo(pieceOfCode);
					_performances.Add(pieceOfCode, info);
				}

				info.Count++;
				info.Start = TimeCounter.GetStartValue();
			}
		}

		[Conditional("DEBUG")]
		public static void Finish(string pieceOfCode)
		{
			lock (_performances)
			{
				if (_performances.ContainsKey(pieceOfCode))
				{
					PerformanceInfo info = _performances[pieceOfCode];
					info.Count++;
					info.TotalTime += TimeCounter.Finish(info.Start);
				}
			}
		}

		public static void Reset()
		{
			_performances.Clear();
		}

		public static string GenerateReport()
		{
			return GenerateReport(0);
		}

		public static string GenerateReport(string mainPieceOfCode)
		{
			if (_performances.ContainsKey(mainPieceOfCode))
				return GenerateReport(_performances[mainPieceOfCode].TotalTime);
			else
				return GenerateReport(0);
		}

		public static string GenerateReport(double totalTime)
		{
			StringBuilder sb = new StringBuilder();
			int len = 0;
			foreach (PerformanceInfo info in Performances)
				len = Math.Max(info.Name.Length, len);

			sb.AppendLine("Name".PadRight(len) + " Count              Total Time, ms    Avg. Time, ms       Percentage, %");
			sb.AppendLine("----------------------------------------------------------------------------------------------");
			foreach (PerformanceInfo info in Performances)
			{
				sb.Append(info.Name.PadRight(len));
				double p = 0;
				double avgt = 0;
				if (totalTime != 0)
					p = info.TotalTime / totalTime;
				if (info.Count > 0)
					avgt = info.TotalTime * 1000 / info.Count;
				string c = info.Count.ToString("0,0").PadRight(20);
				string tt = (info.TotalTime * 1000).ToString("0,0.00").PadRight(20);
				string t = avgt.ToString("0.0000").PadRight(20);
				string sp = (p * 100).ToString("###").PadRight(20);
				sb.AppendFormat(" " + c + tt + t + sp + "\n");
			}
			return sb.ToString();
		}
	}
}
