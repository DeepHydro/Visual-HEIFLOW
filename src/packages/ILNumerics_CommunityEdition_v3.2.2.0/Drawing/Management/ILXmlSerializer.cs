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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace ILNumerics.Drawing {
    
    /// <summary>
    /// Serializer class for serializing ILNumerics scenes 
    /// </summary>
    public class ILXmlSerializer {

        #region attributes
        XmlWriter m_writer; 
        #endregion

        #region constructors
        /// <summary>
        /// Create a new ILXmlSerializer and provide the xml writer which receives the data
        /// </summary>
        /// <param name="writer">xml writer as target for the data</param>
        public ILXmlSerializer(XmlWriter writer) {
            m_writer = writer;
        }
        #endregion

        #region public interface
        /// <summary>
        /// Convert the given object and all its subobjects / subtree into XML representation
        /// </summary>
        /// <param name="obj">ILNumerics object, mostly ILScene</param>
        public void Serialize(object obj) {
            m_writer.WriteStartDocument();

            writeNode(obj);

            m_writer.WriteEndDocument();

        }      
        #endregion

        #region private helper
        [System.Security.SecuritySafeCritical]
        private void writeNode(object node, string rootName = null) {
            if (object.Equals(node, null)) return;
            var type = node.GetType();
            string elementName = type.Name;
            if (!String.IsNullOrEmpty(rootName)) {
                elementName = rootName;
            }
            m_writer.WriteStartElement(elementName);
            // do we need to expose the true type ?
            if (elementName != type.Name && (!type.IsPrimitive && !(node is string))) {
                m_writer.WriteAttributeString("type", type.Name);
            }
            var elements4Write = new List<Tuple<string, object>>();
            var enumerables4Write = new List<Tuple<string, IEnumerable<object>>>();

            var toXMLMethod = type.GetMethod("ToXML", new Type[] { typeof(System.Xml.XmlWriter) });
            if (toXMLMethod != null) {
                toXMLMethod.Invoke(node, new object[] { m_writer });
            } else if (type.IsPrimitive || node is string || type.IsEnum) {
                m_writer.WriteString(node.ToString());
            } else {
                // find all properties, recognize XMLAttributes
                var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(pi => {
                    bool ret = !Attribute.IsDefined(pi, typeof(System.Xml.Serialization.XmlIgnoreAttribute))
                        && pi.Name != "Item" && (pi.GetIndexParameters() == null || pi.GetIndexParameters().Length == 0);
                    // add more constrains here...
                    return ret;
                });
                foreach (var prop in props) {
                    // fetch the value
                    try {
                        var obj = prop.GetValue(node, null);
                        if (!object.Equals(obj, null)) {

                            if (Attribute.IsDefined(prop, typeof(System.Xml.Serialization.XmlTypeAttribute))) {
                                // fetch value by type surrogate (not supported yet)
                            } else if (Attribute.IsDefined(prop, typeof(System.Xml.Serialization.XmlArrayAttribute))) {
                                // write all items
                                var list = obj as IEnumerable<object>;
                                if (list != null && list.Count() > 0) {
                                    enumerables4Write.Add(new Tuple<string, IEnumerable<object>>(prop.Name, list));
                                }
                                continue;
                            } else if (Attribute.IsDefined(prop, typeof(ILXmlSerializeAsAttribute))) {
                                // fetch value by attribute formatting
                                System.Attribute[] attr = System.Attribute.GetCustomAttributes(prop);
                                foreach (var att in attr) {
                                    if (att is ILXmlSerializeAsAttribute) {
                                        ILXmlSerializeAsAttribute myAtt = (ILXmlSerializeAsAttribute)att;
                                        obj = formatNode(obj, myAtt.Format);
                                        break;
                                    }
                                }
                            } else {
                                // use ToXMLAttrString() method of the type
                                var toXMLAttrStringMethod = prop.PropertyType.GetMethod("ToXMLAttrString");
                                if (toXMLAttrStringMethod != null) {
                                    //var val = prop.GetValue(prop.Name,null); 
                                    obj = toXMLAttrStringMethod.Invoke(obj, null).ToString();
                                }
                            }
                            if (Attribute.IsDefined(prop, typeof(System.Xml.Serialization.XmlAttributeAttribute))) {
                                // simple perform ToString()
                                string attrValue = obj.ToString();
                                m_writer.WriteAttributeString(prop.Name, attrValue);
                            } else if (Attribute.IsDefined(prop, typeof(System.Xml.Serialization.XmlElementAttribute))) {
                                elements4Write.Add(new Tuple<string, object>(prop.Name, obj));
                            } else if (Attribute.IsDefined(prop, typeof(System.Xml.Serialization.XmlArrayAttribute))) {
                                elements4Write.Add(new Tuple<string, object>(prop.Name, obj));
                            } else {
                                // no attribute applied: public scope required 
                                if (type.GetProperty(prop.Name, BindingFlags.Public | BindingFlags.Instance) != null) {
                                    elements4Write.Add(new Tuple<string, object>(prop.Name, obj));
                                }
                            }
                        }
                    } catch (System.Security.SecurityException exc) {
                        // simply skipping this property for now ... 
                    }
                }
                // write out lists
                if (enumerables4Write.Count > 0) {
                    foreach (var o in enumerables4Write) {

                        m_writer.WriteStartElement(o.Item1);
                        foreach (var item in o.Item2) {
                            writeNode(item);
                        }
                        m_writer.WriteEndElement();

                    }
                }
                // write out elements 
                foreach (var o in elements4Write) {
                    writeNode(o.Item2, o.Item1);
                }
            }
            if (node is ILGroup) {
                m_writer.WriteStartElement("Children");
                foreach (var n in (node as ILGroup).InternalChildren) {
                    writeNode(n);
                }
                m_writer.WriteEndElement();
            }
            m_writer.WriteEndElement();

        }
        public string formatNode(object node, String format) {
            if (String.IsNullOrEmpty(format)) {
                return String.Empty;
            }
            if (object.Equals(node, null))
                return null;

            Regex regex = new Regex(@"{(.*?)}");
            var match = regex.Match(format);
            var type = node.GetType();

            while (match.Success) {
                if (match.Groups.Count >= 1) {
                    string m = match.Groups[1].Value.ToString();
                    var prop = type.GetProperty(m);
                    if (prop != null && prop.GetIndexParameters().Length == 0) {
                        var value = prop.GetValue(node, null);
                        if (value != null) {
                            format = format.Replace(@"{" + m + "}", value.ToString());
                        }
                    }
                }
                match = match.NextMatch();
            };
            return format;
        }
        #endregion
    }
}
