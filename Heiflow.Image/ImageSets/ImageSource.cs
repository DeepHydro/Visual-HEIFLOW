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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Heiflow.Image.ImageSets
{
    [Serializable]
    public class ImageSource : Heiflow.Image.ImageSets.IImageSource
    {
        private string _FileName;
        public ImageSource()
        {
            Initialize();
        }

        [XmlArrayItem]
        public List<string> TrainingImages
        {
            get;
            set;
        }
          [XmlArrayItem]
        public List<string> MarkedTrainingImages
        {
            get;
            set;
        }
          [XmlArrayItem]
        public List<string> RecoginzingImages
        {
            get;
            set;
        }

        public static ImageSource Open(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ImageSource));
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            var source = (ImageSource)xs.Deserialize(stream);
            source._FileName = filename;
            return source;
        }

        public void Initialize()
        {
            TrainingImages = new List<string>();
            MarkedTrainingImages = new List<string>();
            RecoginzingImages = new List<string>();
        }

        public void Save()
        {
            SaveAs(_FileName);
        }




        public bool AddRecoginzingImage(string filename)
        {
            if(File.Exists(filename))
            {
                if (!RecoginzingImages.Contains(filename))
                {
                    RecoginzingImages.Add(filename);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool AddRecoginzingImages(string[] filenames)
        {
            bool added = false;
            foreach (var fn in filenames)
            {
                added = AddRecoginzingImage(fn);
            }
            return added;
        }

        public bool AddTrainingImage(string filename)
        {
            if (File.Exists(filename))
            {
                if (!TrainingImages.Contains(filename))
                {
                  
                    var fn = Path.GetFileNameWithoutExtension(filename);
                    var extension = Path.GetExtension(filename);
                    var fn_mark = Path.Combine(Path.GetDirectoryName(filename), fn + "_train" + extension);

                    if (File.Exists(fn_mark))
                    {
                        MarkedTrainingImages.Add(fn_mark);
                        TrainingImages.Add(filename);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                  
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool AddTrainingImages(string[] filenames)
        {
            bool added = false;
            foreach (var fn in filenames)
            {
                added = AddTrainingImage(fn);
            }
            return added;
        }
        public void Clear()
        {
            TrainingImages.Clear();
            MarkedTrainingImages.Clear();
            RecoginzingImages.Clear();
        }

        public void SaveAs(string filename)
        {
            XmlSerializer xs = new XmlSerializer(this.GetType());
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            xs.Serialize(stream, this);
            stream.Close();
        }


        public System.Drawing.Bitmap[][] ToTrainingImages()
        {
            int nimg= TrainingImages.Count;
            if (nimg > 0)
            {
                Bitmap [][] images = new Bitmap [2][];
                images[0] = new Bitmap[nimg];
                images[1] = new Bitmap[nimg];
                for (int i = 0; i < nimg;i++ )
                {
                    images[0][i] = new Bitmap(TrainingImages[i]);
                    images[1][i] = new Bitmap(MarkedTrainingImages[i]);
                }
                    return images;
            }
            else
                return null;
        }

        Bitmap[][] IImageSource.ToRecoginzingImages()
        {
            throw new NotImplementedException();
        }
    }
}
