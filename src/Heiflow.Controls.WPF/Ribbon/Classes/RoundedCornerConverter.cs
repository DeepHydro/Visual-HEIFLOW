using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    class RoundedCornerConverter:IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a RoundedCorner property to an appropriate value.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CornerRadius cr = (CornerRadius)value;
            string s = parameter as string;
            switch (s)            
            {
                case "left":
                    return new CornerRadius(cr.TopLeft, 0d, 0d, cr.BottomLeft);

                case "right":
                    return new CornerRadius(0d, cr.TopRight, cr.BottomRight, 0d);

                case "top":
                    return new CornerRadius(cr.TopLeft, cr.TopRight, 0d, 0d);

                case "bottom":
                    return new CornerRadius(0d, 0d, cr.BottomRight, cr.BottomLeft);

                default:
                    return null;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
