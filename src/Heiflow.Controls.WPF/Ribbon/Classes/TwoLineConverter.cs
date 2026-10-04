using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;

#region Copyright
// Odyssey.Controls.Ribbonbar
// (c) copyright 2009 Thomas Gerber
// This source code and files, is licensed under The Microsoft Public License (Ms-PL)
#endregion
namespace Odyssey.Controls
{
    /// <summary>
    /// Breaks a string into two lines by adding a LineBreak at the appropriate position.
    /// </summary>
    public class TwoLineConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a string into a TextBlock with one or two lines.
        /// </summary>
        /// <param name="value">Either a string to convert, or any value to return directly.</param>
        /// <param name="targetType">ignored.</param>
        /// <param name="parameter">ignored.</param>
        /// <param name="culture">ignored.</param>
        /// <returns>A TextBlock class if value is of string, otherwise value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = value as string;
            if (s == null) return value;
            {
                int l = SplitIn2Lines(s);
                if (l > 0)
                {
                    s = s.Remove(l,1).Insert(l, "\n");
                }
            }
            TextBlock tb = new TextBlock();
            tb.TextWrapping = System.Windows.TextWrapping.Wrap;
            tb.TextAlignment = System.Windows.TextAlignment.Center;
            tb.LineStackingStrategy = System.Windows.LineStackingStrategy.BlockLineHeight;
            tb.LineHeight = 12.0;
            tb.Text = s;
            return tb;
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
