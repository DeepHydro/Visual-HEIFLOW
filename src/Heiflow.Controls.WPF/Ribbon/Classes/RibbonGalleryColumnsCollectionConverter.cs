using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls.Classes
{
    public class RibbonGalleryColumnsCollectionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string str = value as string;
            if (str == null)
            {
                return base.ConvertFrom(context, culture, value);
            }
            str = str.Trim();
            if (str.Length == 0) return null;

            string[] names = str.Split(',');
            RibbonGalleryColumns collection = new RibbonGalleryColumns();
            foreach (string name in names)
            {
                collection.Add(int.Parse(name));
            }
            return collection;
        }
    }
}
