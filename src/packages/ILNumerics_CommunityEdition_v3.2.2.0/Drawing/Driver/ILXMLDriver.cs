///
///    This file is part of ILNumerics Community Edition.
///
///    ILNumerics Community Edition - high performance computing for applications.
///    Copyright (C) 2006 - 2013 Haymo Kutschbach, http://ilnumerics.net
///
///    ILNumerics Community Edition is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License version 3 as published by
///    the Free Software Foundation.
///
///    ILNumerics Community Edition is distributed in the hope that it will be useful,
///    but WITHOUT ANY WARRANTY; without even the implied warranty of
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
///    GNU General Public License for more details.
///
///    You should have received a copy of the GNU General Public License
///    along with ILNumerics Community Edition. See the file License.txt in the root
///    of your distribution package. If not, see <http://www.gnu.org/licenses/>.
///
///    In addition this software uses the following components and/or licenses: 
///
///    =================================================================================
///    The Open Toolkit Library License
///    
///    Copyright (c) 2006 - 2009 the Open Toolkit library.
///    
///    Permission is hereby granted, free of charge, to any person obtaining a copy
///    of this software and associated documentation files (the "Software"), to deal
///    in the Software without restriction, including without limitation the rights to 
///    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
///    the Software, and to permit persons to whom the Software is furnished to do
///    so, subject to the following conditions:
///
///    The above copyright notice and this permission notice shall be included in all
///    copies or substantial portions of the Software.
///
///    =================================================================================
///    Intel® Math Kernel Library 11.1 for Windows
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Intel® Math Kernel Library 10.3 for Linux
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Products / Software which is implicitly used by ILNumerics due to the inclusion 
///    of 3rd party components: 
///  
///        BLAS/ LAPACK; see: http://netlib.org
///        FFT Functions; see: http://www.spiral.net, http://fftw.org
///        OpenGL; see: http://opengl.org
///
///    =================================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml; 
using System.Globalization;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Reflection; 

namespace ILNumerics.Drawing {
    /// <summary>
    /// Serializing driver for XML output
    /// </summary>
    public class ILXMLDriver : ILDriver {

        /**
         * Some notes on the functioning of ILXMLDriver. We refused to utilize one of the common .NET serialization methods. 
         * Reasons: They all either force (break) a specific design (XML Serializer: publicity of properties, the 
         * need of XMLIncludeAttributes on base classes, huge hassle to get inheritance working) or do not give enough
         * flexibility by determining the resulting XML layout (DataContract Serializer). Hence, we do it that way: 
         * (1) (removed)
         * (2) Some basic types are known to the serializer, e.g. ILGroup. These types are 'manually' serialized, utilizing the 'knowledge' of ILXMLDriver. 
         * (3) When the driver receives a node which it does not know, it first inspects the type, if it implements a ToXML(XMLWriter) method. If 
         *     such method is found, it will be used to serialize the instance. Otherwise, reflection is used to serialize the object. 
         * (4) Only properties are serialized. Common XMLAttributes on a property are used to indicate special handling: XMLIgnore, XMLAttribute, XMLElement. 
         *     If no attribute exists for a public property it will be serialized as XML element by default.  
         **/

        #region attributes
        XmlWriter m_writer; 
        Size m_size; 
        #endregion

        #region constructors
        public ILXMLDriver(StringBuilder builder, Size size, ILScene scene = null) : base(scene ?? new ILScene()) {
            m_writer = XmlWriter.Create(builder);
            Size = size;
        }
        public ILXMLDriver(XmlWriter writer, Size size, ILScene scene = null)
            : base(scene ?? new ILScene()) {
            m_writer = writer; 
            Size = size; 
        }
        #endregion

        public override bool IsDisposed {
            get { return false; }
        }

        public override System.Drawing.Size Size {
            get {
                return m_size; 
            }
            set {
                m_size = value; 
            }
        }

        public override RendererTypes Driver {
            get { return RendererTypes.XML; }
        }

        protected override void BeginRenderPass(ILRenderParameter renderParams) {
            base.BeginRenderPass(renderParams);
            if (renderParams.CurrentPassCount == 1) {
                var oldCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
                try {
                    System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                    var serializer = new ILXmlSerializer(m_writer); 
                    serializer.Serialize(SceneSyncRoot); 
                    m_writer.Flush();
                    m_writer.Close();
                } finally {
                    System.Threading.Thread.CurrentThread.CurrentCulture = oldCulture; 
                }
            }
        }

    }
}
