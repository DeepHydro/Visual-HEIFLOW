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
    public class SaveH3DProjectFileProvider : ISaveProjectFileProvider
    {
        public SaveH3DProjectFileProvider()
        {
          
        }

        public void Save(string fileName, IProject project)
        {
            if (project != null)
            {
                XmlSerializer xs = new XmlSerializer(typeof(Heiflow3DProject));
                Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                xs.Serialize(stream, project);
                stream.Close();
            }
        }

        public  string FileTypeDescription
        {
            get
            {
                return "IHM3D Project File";
            }
        }

        public  string Extension
        {
            get
            {
                return ".ihmx";
            }
        }

    }
}
