using Heiflow.Controls.Project;
using Heiflow.Models.Generic.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Visualization.Project
{
    public class OpenH3DProjectFileProvider : OpenProjectFileProvider
    {
        public OpenH3DProjectFileProvider()
        {

        }

        public override  IProject Open(string fileName)
        {
            XmlSerializer xs = new XmlSerializer(typeof(Heiflow3DProject));
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            var project = (Heiflow3DProject)xs.Deserialize(stream);
            return project;
        }

        public override string FileTypeDescription
        {
            get 
            {
                return "IHM3D Project File";
            }
        }

        public override string Extension
        {
            get 
            {
                return ".ihmx";
            }
        }
    }
}
