using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    public class TwoLineTextConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Splits a string into two lines which have almost the same length if possible.
        /// </summary>
        /// <param name="value">The string to split. If this type is not a string, the value is returned directly.</param>
        /// <param name="parameter">Specifies the line number to return. The value must be either (int)1 or (int)2.</param>
        /// <returns>The first or second line of the string, otherwise value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int line;
            try
            {
                line = int.Parse(parameter as string);
            }
            catch
            {
                throw new ArgumentException("parameter must be either 1 or 2.");
            }
            string s = value as string;
            if (s == null) return line == 1 ? value : null;

            int l = SplitIn2Lines(s);
            if (l == 0) return line == 1 ? s : null;

            switch (line)
            {
                case 1: return s.Substring(0, l).Trim();
                case 2: return s.Substring(l + 1).Trim();
                default: throw new ArgumentException("parameter must be either 1 or 2.");
            }
        }

        private static int SplitIn2Lines(string s)
        {
            int n = s.Length;
            int l = n / 2;
            int r = l + 1;
            while (l > 0)
            {
                char c = s[l];
                if (char.IsSeparator(c)) break;
                if (r < n)
                {
                    c = s[r];
                    if (char.IsSeparator(c))
                    {
                        l = r;
                        break;
                    }
                }
                r++;
                l--;
            }
            return l;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
