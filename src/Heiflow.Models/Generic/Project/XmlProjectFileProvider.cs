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

using System.IO;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic.Project
{
    /// <summary>
    /// Base class of the xml based project file providers. It keeps the deserialization
    /// code in one place and guarantees that the file stream is always closed.
    /// </summary>
    /// <typeparam name="T">Type of the project stored in the file.</typeparam>
    public abstract class XmlProjectFileProvider<T> : IOpenProjectFileProvider where T : class, IProject
    {
        public virtual string FileTypeDescription
        {
            get { return "Visual HEIFLOW Project File"; }
        }

        public virtual string Extension
        {
            get { return ".vhfx"; }
        }

        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the root element of the project file. It selects the provider.
        /// </summary>
        public abstract string ProviderName
        {
            get;
        }

        public IProject Open(string fileName)
        {
            FileName = fileName;
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                return xs.Deserialize(stream) as T;
            }
        }
    }
}
