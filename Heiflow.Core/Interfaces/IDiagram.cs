// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices.ComTypes;

namespace Heiflow.Core
{
    public class ShapeNodeInfo : IDataObject
    {
        public string Caption;
        public string Name;
        public PointF Location;
        public Image Image;
        public object Tag;
        public ModelRole ModelRole;
        public ILinkableObject LinkableObjects;

        #region IDataObject 成员

        public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
        {
            throw new NotImplementedException();
        }

        public void DUnadvise(int connection)
        {
            throw new NotImplementedException();
        }

        public int EnumDAdvise(out IEnumSTATDATA enumAdvise)
        {
            throw new NotImplementedException();
        }

        public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
        {
            throw new NotImplementedException();
        }

        public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
        {
            throw new NotImplementedException();
        }

        public void GetData(ref FORMATETC format, out STGMEDIUM medium)
        {
            throw new NotImplementedException();
        }

        public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
        {
            throw new NotImplementedException();
        }

        public int QueryGetData(ref FORMATETC format)
        {
            throw new NotImplementedException();
        }

        public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public interface IDiagramNode
    {
        ShapeNodeInfo NodeInfo { set; get; }
    }

    public interface IDiagram
    {
        IDiagramNode[] DiagramNodes { get; }
    }

    public interface IDiagramEngine
    {
        IDiagram DiagramObject { get; set; }
        IDiagramSchema Translate();
    }

    public interface IDiagramManager
    {
        ModelCategory ModelCategory { get; set; }
        OperationMode Mode { get; set; }
        IDiagramSchema DiagramSchema { get; }
        void CreateSchema();
    }
}
