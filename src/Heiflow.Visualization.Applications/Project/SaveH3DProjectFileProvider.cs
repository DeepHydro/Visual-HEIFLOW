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

        /// <summary>
        /// Writes the project to its file. The project is written to a file of its own beside it
        /// first and copied over afterwards, so that a project that fails on the way out is left as
        /// it was and not half written.
        /// </summary>
        public void Save(string fileName, IProject project)
        {
            var prj = project as Heiflow3DProject;
            if (prj == null)
                return;

            var directory = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var tempFile = fileName + ".tmp";
            XmlSerializer xs = new XmlSerializer(typeof(Heiflow3DProject));
            using (Stream stream = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                xs.Serialize(stream, prj);
            }
            File.Copy(tempFile, fileName, true);
            File.Delete(tempFile);
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



        public string FileName
        {
            get;
            set;
        }
    }
}
