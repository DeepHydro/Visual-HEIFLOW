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
using Heiflow.Controls.Tree.NodeControls;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Heiflow.Controls.Tree
{
	internal class IncrementalSearch
	{
		private const int SearchTimeout = 300; //end of incremental search timeot in msec

		private TreeViewAdv _tree;
		private TreeNodeAdv _currentNode;
		private string _searchString = "";
		private DateTime _lastKeyPressed = DateTime.Now;

		public IncrementalSearch(TreeViewAdv tree)
		{
			_tree = tree;
		}

		public void Search(Char value)
		{
			if (!Char.IsControl(value))
			{
				Char ch = Char.ToLowerInvariant(value);
				DateTime dt = DateTime.Now;
				TimeSpan ts = dt - _lastKeyPressed;
				_lastKeyPressed = dt;
				if (ts.TotalMilliseconds < SearchTimeout)
				{
					if (_searchString == value.ToString())
						FirstCharSearch(ch);
					else
						ContinuousSearch(ch);
				}
				else
				{
					FirstCharSearch(ch);
				}
			}
		}

		private void ContinuousSearch(Char value)
		{
			if (value == ' ' && String.IsNullOrEmpty(_searchString))
				return; //Ingnore leading space

			_searchString += value;
			DoContinuousSearch();
		}

		private void FirstCharSearch(Char value)
		{
			if (value == ' ')
				return;

			_searchString = value.ToString();
			TreeNodeAdv node = null;
			if (_tree.SelectedNode != null)
				node = _tree.SelectedNode.NextVisibleNode;
			if (node == null)
				node = _tree.Root.NextVisibleNode;

			if (node != null)
				foreach (string label in IterateNodeLabels(node))
				{
					if (label.StartsWith(_searchString))
					{
						_tree.SelectedNode = _currentNode;
						return;
					}
				}
		}

		public virtual void EndSearch()
		{
			_currentNode = null;
			_searchString = "";
		}

		protected IEnumerable<string> IterateNodeLabels(TreeNodeAdv start)
		{
			_currentNode = start;
			while(_currentNode != null)
			{
				foreach (string label in GetNodeLabels(_currentNode))
					yield return label;

				_currentNode = _currentNode.NextVisibleNode;
				if (_currentNode == null)
					_currentNode = _tree.Root;

				if (start == _currentNode)
					break;
			} 
		}

		private IEnumerable<string> GetNodeLabels(TreeNodeAdv node)
		{
			foreach (NodeControl nc in _tree.NodeControls)
			{
				BindableControl bc = nc as BindableControl;
				if (bc != null && bc.IncrementalSearchEnabled)
				{
					object obj = bc.GetValue(node);
					if (obj != null)
						yield return obj.ToString().ToLowerInvariant();
				}
			}
		}

		private bool DoContinuousSearch()
		{
			bool found = false;
			if (!String.IsNullOrEmpty(_searchString))
			{
				TreeNodeAdv node = null;
				if (_tree.SelectedNode != null)
					node = _tree.SelectedNode;
				if (node == null)
					node = _tree.Root.NextVisibleNode;

				if (!String.IsNullOrEmpty(_searchString))
				{
					foreach (string label in IterateNodeLabels(node))
					{
						if (label.StartsWith(_searchString))
						{
							found = true;
							_tree.SelectedNode = _currentNode;
							break;
						}
					}
				}
			}
			return found;
		}

	}
}
