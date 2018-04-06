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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Heiflow.Core.Schema
{
    public class WorkSpaceManager
    {
        public WorkSpaceManager()
        {
        }

        public static void SetWorkSpace(WorkSpace ws)
        {
            CurrentWorkSpace = ws;
        }

        public static WorkSpace CurrentWorkSpace
        {
            get;
            set;
        }

        public static WorkSpace LoadFromXML(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WorkSpace));
            XmlTextReader textReader = new XmlTextReader(filename);
            return serializer.Deserialize(textReader) as WorkSpace;
        }
    }

    public class WorkSpace
    {
        public WorkSpace(WorkSpaceSettings settings)
        {
           Settings = settings;
        }

        public WorkSpace()
        {
        }

        private  DiagramApplication mDiagramApplication;

        public WorkSpaceSettings Settings
        {
            get;
            set;
        }

        public  DiagramApplication CurrentApplicaton
        { 
            get { return mDiagramApplication; } 
        }

        public  void SetCurrentApplicaton(DiagramApplication app)
        {
            mDiagramApplication = app;
        }

        public bool SerializeToXML(string filename)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(WorkSpace));
                TextWriter textWriter = new StreamWriter(filename);
                serializer.Serialize(textWriter, this);
                textWriter.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }   
}
