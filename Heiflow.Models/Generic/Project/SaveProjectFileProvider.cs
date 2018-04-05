// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic.Project
{
    internal class SaveProjectFileProvider : ISaveProjectFileProvider
    {
        public SaveProjectFileProvider()
        {
          
        }

        public void Save(string fileName, IProject project)
        {
            if (project != null)
            {
                FileName = fileName;
                XmlSerializer xs = new XmlSerializer(project.GetType());
                Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                xs.Serialize(stream, project);
                stream.Close();
            }
        }

        public string FileTypeDescription
        {
            get
            {
                return "Visual HEIFLOW Project File";
            }
        }

        public string Extension
        {
            get
            {
                return ".vhfx";
            }
        }



        public string FileName
        {
            get;
            set;
        }
    }
}
