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
