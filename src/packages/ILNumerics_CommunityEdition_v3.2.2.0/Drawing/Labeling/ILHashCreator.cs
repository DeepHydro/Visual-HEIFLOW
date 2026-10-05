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
using System.Text;
using System.Drawing; 

namespace ILNumerics.Drawing {
    /// <summary>
    /// Helper class used to create hashes for renderable items (currently text only)
    /// </summary>
    /// <remarks>This class should be used to retrieve an (unique) key for any 
    /// renderable items used in ILNumerics.Drawing renderer classes. This way on can 
    /// ensure not to create hash conflicts between different cached items.
    ///  </remarks>
    public class ILHashCreator {

        /// <summary>
        /// create hash for a text item, used to uniquely identify the item in a collection
        /// </summary>
        /// <param name="text">text content</param>
        /// <param name="font">font used to draw the character</param>
        /// <returns>string as key into collection. The key is: F:[(int)FontStyle][Size (3digits)]&[FontName]&text</returns>
        public static string Hash(string text, Font font) {
            return String.Format("F:{0}&{1:g3}&{2}&{3}",(int)font.Style,font.Size,font.Name,text); 
        }

        internal static void Parse(string p, out Font font) {
            string[] parts = p.Split('&');
            FontStyle style; 
            float size; 
            string name; 
            if (parts.Length >= 3) {
                try {
                    if (!float.TryParse(parts[1], out size)) {
                        size = 11; 
                    }
                    if (String.IsNullOrEmpty(parts[2])) {
                        name = SystemFonts.MessageBoxFont.Name;
                    } else {
                        name = parts[2]; 
                    }
                    style = (FontStyle)Enum.Parse(typeof(FontStyle),parts[0].Substring(2));
                    font = new Font(name, size, style); 
                    return; 
                } catch (Exception) {
                    
                }
            }
            font = SystemFonts.MessageBoxFont; 
        }
    }
}
