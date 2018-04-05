// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Heiflow.Core
{
    /// <summary>
    /// This DataProvider retrieves the application's About information from
    /// the metadata on the assembly / Assembly attributes.
    /// </summary>
    public class AboutAssemblyDataProvider : IAboutDataProvider
    {
        #region Private members
       // private XmlDocument xmlDoc = null;
        private string resourceKey = defaultAboutProviderKey;
        #endregion

        #region Static data
        private const string defaultAboutProviderKey = "aboutProvider";
        private const string propertyNameTitle = "Title";
        private const string propertyNameDescription = "Description";
        private const string propertyNameProduct = "Product";
        private const string propertyNameCopyright = "Copyright";
        private const string propertyNameCompany = "Company";
        private const string xPathRoot = "ApplicationInfo/";
        private const string xPathTitle = xPathRoot + propertyNameTitle;
        private const string xPathVersion = xPathRoot + "Version";
        private const string xPathDescription = xPathRoot + propertyNameDescription;
        private const string xPathProduct = xPathRoot + propertyNameProduct;
        private const string xPathCopyright = xPathRoot + propertyNameCopyright;
        private const string xPathCompany = xPathRoot + propertyNameCompany;
        private const string xPathLink = xPathRoot + "Link";
        private const string xPathLinkUri = xPathRoot + "Link/@Uri";
        #endregion

        #region Public properties
        /// <summary>
        /// Gets and sets the resource key for the XmlDataProvider to retrieve
        /// from Application resources.
        /// </summary>
        public string ResourceKey
        {
            get { return resourceKey; }
            set { resourceKey = value; }
        }
        #endregion

        #region IAboutDataProvider Members
        /// <summary>
        /// Gets the title property, which is display in the About dialogs window title.
        /// </summary>
        public string Title
        {
            get
            {
                string result = CalculatePropertyValue<AssemblyTitleAttribute>("Title", xPathTitle);

                if (string.IsNullOrEmpty(result))
                {
                    // otherwise, just get the name of the assembly itself.
                    result = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the application's version information to show.
        /// </summary>
        public string Version
        {
            get
            {
                string result = "";
                // first, try to get the version string from the assembly.
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                if (version != null)
                {
                    result = version.ToString();
                }
                else
                {
                    // if that fails, try to get the version from a resource in the Application.
                    result = GetLogicalResourceString(xPathVersion);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description
        {
            get
            {
                return CalculatePropertyValue<AssemblyDescriptionAttribute>("Description", xPathDescription);
            }
        }

        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public string Product
        {
            get
            {
                return CalculatePropertyValue<AssemblyProductAttribute>("Product", xPathProduct);
            }
        }

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public string Copyright
        {
            get
            {
                return CalculatePropertyValue<AssemblyCopyrightAttribute>("Copyright", xPathCopyright);
            }
        }

        /// <summary>
        /// Gets the product's company name.
        /// </summary>
        public string Company
        {
            get
            {
                return CalculatePropertyValue<AssemblyCompanyAttribute>("Company", xPathCompany);
            }
        }

        /// <summary>
        /// Gets the link text to display in the About dialog.
        /// </summary>
        public string LinkText
        {
            get
            {
                return GetLogicalResourceString(xPathLink);
            }
        }

        /// <summary>
        /// Gets the link uri that is the navigation target of the link.
        /// </summary>
        public string LinkUri
        {
            get
            {
                return GetLogicalResourceString(xPathLinkUri);
            }
        }
        #endregion

        #region Helper method
        /// <summary>
        /// Gets the specified property value either from a specific attribute,
        /// or from a resource in the Application.
        /// </summary>
        /// <typeparam name="T">Attribute type that we're trying to retrieve.</typeparam>
        /// <param name="propertyName">Property name to use on the attribute.</param>
        /// <param name="xpathQuery">XPath to the element in the XML data resource.</param>
        /// <returns>The resulting string to use for a property.
        /// Returns null if no data could be retrieved.</returns>
        private string CalculatePropertyValue<T>(string propertyName, string xpathQuery)
        {
            string result = "";
            // first, try to get the property value from an attribute.
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            if (attributes.Length > 0)
            {
                T attrib = (T)attributes[0];
                PropertyInfo property = attrib.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    result = property.GetValue(attributes[0], null) as string;
                }
            }
            else
            {
                // if that fails, try to get it from a resource in the Application.
                result = GetLogicalResourceString(xpathQuery);
            }

            return result;
        }
        #endregion

        #region Resource location methods
        ///// <summary>
        ///// Gets the XmlDataProvider's document from an Application resource.
        ///// </summary>
        //protected virtual XmlDocument ResourceXmlDocument
        //{
        //    get
        //    {
        //        if (xmlDoc == null)
        //        {
        //            // if we haven't already found the resource XmlDocument, then
        //            // try to find it.
        //            Debug.Assert(App.Current != null, "The current application should no tbe null at this point.");
        //            XmlDataProvider provider = App.Current.TryFindResource(ResourceKey) as XmlDataProvider;
        //            if (provider != null)
        //            {
        //                // save away the XmlDocument, so we don't have to get it multiple times.
        //                xmlDoc = provider.Document;
        //            }
        //        }

        //        return xmlDoc;
        //    }
        //}

        /// <summary>
        /// Gets the specified data element from the XmlDataProvider in the
        /// Application resources.
        /// </summary>
        /// <param name="xpathQuery">An XPath query to the XML element to retrieve.</param>
        /// <returns>The resulting string value for the specified XML element. 
        /// Returns empty string if resource element couldn't be found.</returns>
        protected virtual string GetLogicalResourceString(string xpathQuery)
        {
            string result = "";

            // get the About xml information from the application's resources.
            //XmlDocument doc = this.ResourceXmlDocument;
            //if (doc != null)
            //{
            //    // if we found the XmlDocument, then look for the specified data. 
            //    XmlNode node = doc.SelectSingleNode(xpathQuery);
            //    if (node != null)
            //    {
            //        if (node is XmlAttribute)
            //        {
            //            // only an XmlAttribute has a Value set.
            //            result = node.Value;
            //        }
            //        else
            //        {
            //            // otherwise, need to just return the inner text.
            //            result = node.InnerText;
            //        }
            //    }
            //}

            return result;
        }
        #endregion
    }
}
