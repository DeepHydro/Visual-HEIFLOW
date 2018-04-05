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

using Heiflow.Controls.Tree;
using Heiflow.Core.Data.ODM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls.WinForm.DatabaseExplorer
{
    public class ODMNodeCreator
    {
        public ODMNodeCreator()
        {

        }

        public ImageList ImageList
        {
            get;
            set;
        }

        public List<Node> Creat(List<IDendritiRecord<ObservationSeries>> records)
        {
            List<Node> nodes = new List<Node>();
            foreach (var record in records)
            {
                var node = new Node(record.Name)
                {
                    Image = ImageList.Images[record.Level],
                    Tag = null
                };
                AddNodes(record, node);
                nodes.Add(node);
            }
            return nodes;
        }

        private void AddNodes(IDendritiRecord<ObservationSeries> record, Node parent)
        {
            if (record.Children.Count == 0)
            {
                return;
            }
            else
            {
                foreach (var ch in record.Children)
                {
                    Node node = new Node(ch.Name)
                    {
                        Image = ImageList.Images[ch.Level],
                        Tag = ch
                    };
                    parent.Nodes.Add(node);
                    AddNodes(ch, node);
                }
            }
        }
    }
}
