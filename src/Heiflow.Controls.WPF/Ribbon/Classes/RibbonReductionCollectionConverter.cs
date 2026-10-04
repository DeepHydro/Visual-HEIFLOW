using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls.Classes
{
    public class RibbonReductionCollectionConverter:TypeConverter
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

            string[] sizes = str.Split(',');
            RibbonSizeCollection collection = new RibbonSizeCollection();
            foreach (string size in sizes)
            {
                RibbonSize rs;
                switch (size.ToUpper())
                {
                    case "L": rs = RibbonSize.Large; break;
                    case "M": rs = RibbonSize.Medium; break;
                    case "S": rs = RibbonSize.Small; break;
                    case "0": rs = RibbonSize.Minimized; break;


                    default:
                        rs = (RibbonSize)Enum.Parse(typeof(RibbonSize), size, true);
                        break;
                }
                collection.Add(rs);
            }
            return collection;
        }
    }
}
