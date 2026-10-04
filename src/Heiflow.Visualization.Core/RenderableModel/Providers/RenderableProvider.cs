using Heiflow.Core.IO;
using Heiflow.Core.Utility;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Visualization.Renderable
{
    [Serializable]
    public class RenderableProvider : IRenderableProvider
    {
        private string _RelativeFileName;
        public RenderableProvider()
        {

        }
        [XmlElement]
        public string Name
        {
            get;
            set;
        }
        [XmlElement]
        public byte Opacity
        {
            get;
            set;
        }
        [XmlElement]
        public float DistanceAboveSurface
        {
            get;
            set;
        }
        [XmlElement]
        public float MinimumDisplayAltitude
        {
            get;
            set;
        }
        [XmlElement]
        public float MaximumDisplayAltitude
        {
            get;
            set;
        }
        [XmlElement]
        public HUST.WREIS.Dot3D.Renderable.RenderPriority RenderPriority
        {
            get;
            set;
        }
        [XmlIgnore]
        public Microsoft.DirectX.Direct3D.FillMode FillMode
        {
            get;
            set;
        }
        [XmlElement]
        public bool IsOn
        {
            get;
            set;
        }
        [XmlElement]
        public bool IsSelectable
        {
            get;
            set;
        }
        [XmlIgnore]
        public string BaseDirectory
        {
            get;
            set;
        }
        [XmlIgnore]
        public string FullFileName
        {
            get
            {
                if(DirectoryHelper.IsRelativePath(_RelativeFileName))
                {
                    return Path.GetFullPath(Path.Combine(BaseDirectory, _RelativeFileName));
                }
                else
                {
                    return _RelativeFileName;
                }
            }
        }
        [XmlElement]
        public string RelativeFileName
        {
            get
            {
                return _RelativeFileName;
            }
            set
            {
                _RelativeFileName = value;
            }
        }
        [XmlElement]
        public bool ShowAtStartup
        {
            get;
            set;
        }

        public virtual RenderableObject Load(string filename, World world)
        {
            return null;
        }

        public virtual string GetRelativeFileName(string masterdic, string fullFileName)
        {
            string folder = Path.GetDirectoryName(masterdic);
            var root_ly = Path.GetPathRoot(fullFileName);
            string relativeFileName = "";
            if (Path.GetPathRoot(fullFileName) == Path.GetPathRoot(fullFileName))
            {
                relativeFileName = FileHelper.GetRelativePath(fullFileName, folder);
            }
            else
            {
                relativeFileName = fullFileName;
            }
            return relativeFileName;
        }
    }
}
