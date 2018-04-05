// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
