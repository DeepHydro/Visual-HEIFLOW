using Heiflow.Core.Utility;
using Heiflow.Models.Generic.Project;
using Heiflow.Presentation.Controls.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Visualization.Project
{
    public class OpenH3DProjectFileProvider : IOpenProjectFileProvider
    {
        public OpenH3DProjectFileProvider()
        {

        }
        public  string FileTypeDescription
        {
            get
            {
                return "HydroEarth Project File";
            }
        }

        public  string Extension
        {
            get
            {
                return ".ihmx";
            }
        }

        public string ProviderName
        {
            get { return "Heiflow3DProject"; }
        }


        public string FileName
        {
            get;
            set;
        }

        public  IProject Open(string fileName)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Heiflow3DProject));
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            var project = (Heiflow3DProject)xs.Deserialize(stream);
            project.AbsolutePathToProjectFile = Path.GetDirectoryName(fileName);
            return project;
        }


        
    }
}
