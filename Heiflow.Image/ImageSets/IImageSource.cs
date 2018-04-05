// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Drawing;
namespace Heiflow.Image.ImageSets
{
    public interface IImageSource
    {
        bool AddRecoginzingImage(string filename);
        bool AddTrainingImage(string filename);
        bool AddRecoginzingImages(string[] filenames);
        bool AddTrainingImages(string [] filenames);
        void Initialize();
        System.Collections.Generic.List<string> MarkedTrainingImages { get; set; }
        System.Collections.Generic.List<string> RecoginzingImages { get; set; }
        void Save();
        void Clear();
        void SaveAs(string filename);
        System.Collections.Generic.List<string> TrainingImages { get; set; }

        Bitmap[][] ToTrainingImages();
        Bitmap[][] ToRecoginzingImages();
    }
}
