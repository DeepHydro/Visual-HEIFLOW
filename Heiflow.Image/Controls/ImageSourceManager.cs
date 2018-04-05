// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Heiflow.Controls.Tree;
using Heiflow.Image.ImageSets;
using Heiflow.Image.Properties;

namespace Heiflow.Image.Controls
{
    public partial class ImageSourceManager : UserControl
    {
        private TreeModel _TreeModel;
        private IImageSource _ImageSource;
        public event EventHandler<string> TrainingImageDoubleClicked;
        public event EventHandler<string> RecgonizingImageDoubleClicked;

        public ImageSourceManager()
        {
            InitializeComponent();
            this.nodeStateIcon1.DataPropertyName = "Image";
            this.nodeTextBox1.DataPropertyName = "Text";
            _TreeModel = new TreeModel();
            treeView1.Model = _TreeModel;
            treeView1.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;
            treeView1.MouseUp += treeView1_MouseUp;
        }

        public IImageSource ImageSource
        {
            get
            {
                return _ImageSource;
            }
            set
            {
                _ImageSource = value;
                RefreshTree();
            }
        }

        public void RefreshTree()
        {
            if (ImageSource == null)
                return;

            treeView1.BeginUpdate();
            _TreeModel.Nodes.Clear();
            var root_training_node = new Node("Training")
            {
                Image = Resources.training_24,
                Tag = "Training"
            };

            foreach (var img in ImageSource.TrainingImages)
            {
                if (File.Exists(img))
                {
                    var fn = Path.GetFileNameWithoutExtension(img);
                    var node = new Node(fn)
                    {
                        Image = Resources.image_tr,
                        Tag = img
                    };
                    
                    root_training_node.Nodes.Add(node);
                }
            }
            var root_Recoginzing_node = new Node("Recoginzing")
            {
                Image = Resources.Recognition_24,
                Tag = "Training"
            };
            foreach (var img in ImageSource.RecoginzingImages)
            {
                if (File.Exists(img))
                {
                    var fn = Path.GetFileNameWithoutExtension(img);
                    var node = new Node(fn)
                    {
                        Image = Resources.image_regc,
                        Tag = img 
                    };

                    root_Recoginzing_node.Nodes.Add(node);
                }
            }
            _TreeModel.Nodes.Add(root_training_node);
            _TreeModel.Nodes.Add(root_Recoginzing_node);
            treeView1.EndUpdate();

        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            var node = e.Node.Tag as Node;

            if (node.Parent.Tag != null)
            {
                var tag = node.Parent.Tag.ToString();
                var file = node.Tag.ToString();
                if (tag == "Training")
                {
                    if (TrainingImageDoubleClicked != null)
                        TrainingImageDoubleClicked(this, file);
                }
                else
                {
                    if (RecgonizingImageDoubleClicked != null)
                        RecgonizingImageDoubleClicked(this, file);
                }
            }
        }

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            
        }


    }
}
