// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
