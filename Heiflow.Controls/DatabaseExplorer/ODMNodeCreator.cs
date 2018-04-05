// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
