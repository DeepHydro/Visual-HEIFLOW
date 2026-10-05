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


#pragma warning disable 1591

using System;
using System.Drawing;

namespace ILNumerics.Drawing
{
   /// <summary>
   /// Color helper - change RGB to/ from HLS model
   /// </summary>
    [Serializable]
    public class ILColorProvider
   {
       
        public static readonly float MAXHUEVALUE = 200;     
        private byte red = 0;
        private byte green = 0;
        private byte blue = 0;

        private float hue = 0;
        private float luminance = 0;
        private float saturation = 0;

        public byte Red
        {
         get
         {
            return red;
         }
         set
         {
            red = value;
            ToHLS();
         }      
      }
      public byte Green
      {
         get
         {
            return green;
         }
         set
         {
            green = value;
            ToHLS();
         }
      }
      public byte Blue
      {
         get
         {
            return blue;
         }
         set
         {
            blue = value;
            ToHLS();
         }
      }
      public float Luminance
      {
         get
         {
            return luminance;
         }
         set
         {
            if ((luminance < 0.0f) || (luminance > 1.0f))
            {
               throw new ArgumentOutOfRangeException("Luminance", "Luminance must be between 0.0 and 1.0");
            }
            luminance = value;
            ToRGB();
         }
      }
      public float Hue
      {
         get
         {
            return hue;
         }
         set
         {
            if ((hue < 0.0f) || (hue > 360.0f))
            {
               throw new ArgumentOutOfRangeException("Hue", "Hue must be between 0.0 and 360.0");
            }
            hue = value;
            ToRGB();
         }
      }
      public float Saturation
      {
         get
         {
            return saturation;
         }
         set
         {
            if ((saturation < 0.0f) || (saturation > 1.0f))
            {
               throw new ArgumentOutOfRangeException("Saturation", "Saturation must be between 0.0 and 1.0");
            }
            saturation = value;
            ToRGB();
         }
      }

      public Color Color
      {
         get
         {
            Color c = Color.FromArgb(red, green, blue);
            return c;
         }
         set
         {
            red = value.R;
            green = value.G;
            blue = value.B;
            ToHLS();
         }
      }

      public void LightenColor(float lightenBy)
      {
         luminance *= (1.0f + lightenBy);
         if (luminance > 1.0f)
         {
            luminance = 1.0f;
         }
         ToRGB();
      }

      public void DarkenColor(float darkenBy)
      {
         luminance *= darkenBy;
         ToRGB();
      }
      

      public ILColorProvider(Color c)
      {
         red = c.R;
         green = c.G;
         blue = c.B;
         ToHLS();
         
      }

      public ILColorProvider(float hue, float luminance, float saturation)
      {
         this.hue = hue;
         this.luminance = luminance;
         this.saturation = saturation;
         ToRGB();
      }

      public ILColorProvider(byte red, byte green, byte blue)
      {
         this.red = red;
         this.green = green;
         this.blue = blue;
      }

      public ILColorProvider(ILColorProvider hlsrgb)
      {
         this.red = hlsrgb.Red;
         this.blue = hlsrgb.Blue;
         this.green = hlsrgb.Green;
         this.luminance = hlsrgb.Luminance;
         this.hue = hlsrgb.Hue;
         this.saturation = hlsrgb.Saturation;
      }

      public ILColorProvider()
      {
      }

      private void ToHLS() 
      {         
         byte minval = Math.Min(red, Math.Min(green, blue));
         byte maxval = Math.Max(red, Math.Max(green, blue));

         float mdiff  = (float)(maxval - minval);
         float msum   = (float)(maxval + minval);
   
         luminance = msum / 510.0f;

         if (maxval == minval) 
         {
            saturation = 0.0f;
            hue = 0.0f; 
         }   
         else 
         { 
            float rnorm = (maxval - red) / mdiff;      
            float gnorm = (maxval - green) / mdiff;
            float bnorm = (maxval - blue) / mdiff;   

            saturation = (luminance <= 0.5f) ? (mdiff / msum) : (mdiff /
             (510.0f - msum));

            if (red == maxval) 
            {
               hue = 60.0f * (6.0f + bnorm - gnorm);
            }
            if (green == maxval) 
            {
               hue = 60.0f * (2.0f + rnorm - bnorm);
            }
            if (blue  == maxval) 
            {
               hue = 60.0f * (4.0f + gnorm - rnorm);
            }
            if (hue > 360.0f) 
            {
               hue = hue - 360.0f;
            }
         }
      }
      /// <summary>
      /// convert hue value to RGB value, based on predefined values  
      /// </summary>
      /// <param name="hue">hue: in range 0...MAXHUEVALUE</param>
      /// <returns>int value as RGB color, A=255</returns>
      /// <remarks>the internal values (rgb / luminance,saturation) will not be altered</remarks>
      public int H2RGB(float hueVal)  
      {
          if (saturation == 0.0) // gray, simple case
          {
              byte bval = (byte)(luminance * 255.0F); 
              return (unchecked((byte)255) << 24) + (bval << 16) + (bval << 8) + bval; 

          } else {
              float rm1;
              float rm2;
             
              if (luminance <= 0.5f) 
              {
                 rm2 = luminance + luminance * saturation;
              } else {
                 rm2 = luminance + saturation - luminance * saturation;
              }
              rm1 = 2.0f * luminance - rm2;
              return (unchecked((byte)255) << 24) 
                        + (ToRGB1(rm1, rm2, hueVal + 120.0f) << 16)
                        + (ToRGB1(rm1, rm2, hueVal) << 8)
                        + ToRGB1(rm1, rm2, hueVal - 120.0f);

          }
      }
      /// <summary>
      /// convert hue value to seperate RGB values, based on predefined values  
      /// </summary>
      /// <param name="hue">hue: in range 0...MAXHUEVALUE</param>
      /// <returns>int value as RGB color, A=255</returns>
      /// <remarks>the internal values (rgb / luminance,saturation) will not be altered. 
      /// rgb returned as seperate float values, normalized to 0.0f...1.0f</remarks>
      public void H2RGB(float hueVal, out float r, out float g, out float b)   
      {
          if (saturation == 0.0) // gray, simple case
          {
              byte bval = (byte)(luminance * 255.0F); 
              r = bval/256f; 
              g = bval/256f; 
              b = bval/256f; 
              return; 
          } else {
              float rm1;
              float rm2;
             
              if (luminance <= 0.5f) 
              {
                 rm2 = luminance + luminance * saturation;
              } else {
                 rm2 = luminance + saturation - luminance * saturation;
              }
              rm1 = 2.0f * luminance - rm2;
              r = ToRGB1(rm1, rm2, hueVal + 120.0f)/256f; 
              g = ToRGB1(rm1, rm2, hueVal)/256f; 
              b = ToRGB1(rm1, rm2, hueVal - 120.0f)/256f; 
              return;  
          }
      }
      /// <summary>
      /// convert hue value to seperate RGB values, based on predefined values  
      /// </summary>
      /// <param name="hue">hue: in range 0...MAXHUEVALUE</param>
      /// <param name="offset">offset into vertex array</param>
      /// <param name="va">vertex array, target for the color</param>
      /// <remarks>the internal values (rgb / luminance,saturation) will not be altered. 
      /// rgb returned as seperate float values, normalized to 0.0f...1.0f</remarks>
      public void H2RGB(float hue, float[] va, ref int offset)   
      {
          if (saturation == 0.0) // gray, simple case
          {
              byte bval = (byte)(luminance * 255.0F); 
              va[offset++] = bval/256f; 
              va[offset++] = bval/256f; 
              va[offset++] = bval/256f; 
              return; 
          } else {
              float rm1;
              float rm2;
             
              if (luminance <= 0.5f) 
              {
                 rm2 = luminance + luminance * saturation;
              } else {
                 rm2 = luminance + saturation - luminance * saturation;
              }
              rm1 = 2.0f * luminance - rm2;
              va[offset++] = ToRGB1(rm1, rm2, hue + 120.0f)/256f; 
              va[offset++] = ToRGB1(rm1, rm2, hue)/256f; 
              va[offset++] = ToRGB1(rm1, rm2, hue - 120.0f)/256f; 
              return;  
          }
      }
      
       /// <summary>
      /// convert hue value to seperate RGB values, based on predefined values  
      /// </summary>
      /// <param name="hue">hue: in range 0...MAXHUEVALUE</param>
      /// <returns>int value as RGB color, A=255</returns>
      /// <remarks>the internal values (rgb / luminance,saturation) will not be altered. 
      /// rgb returned as seperate float values, normalized to 0.0f...1.0f</remarks>
      public void H2RGB(float hueVal, out byte r, out byte g, out byte b)   
      {
          if (saturation == 0.0) // gray, simple case
          {
              byte bval = (byte)(luminance * 255.0F); 
              r = bval; 
              g = bval; 
              b = bval; 
              return; 
          } else {
              float rm1;
              float rm2;
             
              if (luminance <= 0.5f) 
              {
                 rm2 = luminance + luminance * saturation;
              } else {
                 rm2 = luminance + saturation - luminance * saturation;
              }
              rm1 = 2.0f * luminance - rm2;
              r = ToRGB1(rm1, rm2, hueVal + 120.0f); 
              g = ToRGB1(rm1, rm2, hueVal); 
              b = ToRGB1(rm1, rm2, hueVal - 120.0f); 
              return;  
          }
      }

      private void ToRGB() 
      {
         if (saturation == 0.0) // Grauton, einfacher Fall
         {
            red = (byte)(luminance * 255.0F);
            green = red;
            blue = red;
         }
         else
         {
            float rm1;
            float rm2;
         
            if (luminance <= 0.5f) 
            {
               rm2 = luminance + luminance * saturation;
            }
            else
            {
               rm2 = luminance + saturation - luminance * saturation;
            }
            rm1 = 2.0f * luminance - rm2;
            red   = ToRGB1(rm1, rm2, hue + 120.0f);   
            green = ToRGB1(rm1, rm2, hue);
            blue  = ToRGB1(rm1, rm2, hue - 120.0f);
         }
      }

      private static byte ToRGB1(float rm1, float rm2, float rh)
      {
         if (rh > 360.0f) 
         {
            rh -= 360.0f;
         }
         else if (rh <   0.0f) 
         {
            rh += 360.0f;
         }
 
         if (rh <  60.0f) 
         {
            rm1 = rm1 + (rm2 - rm1) * rh / 60.0f;   
         }
         else if (rh < 180.0f) 
         {
            rm1 = rm2;
         }
         else if (rh < 240.0f) 
         {
            rm1 = rm1 + (rm2 - rm1) * (240.0f - rh) / 60.0f; 
         }
                   
         return (byte)(rm1 * 255);
      }
      
      //public static void Colorize (List<ILPlot2DGraph> graphs) {
      //    KnownColor[] colors = (KnownColor[])Enum.GetValues(typeof(KnownColor)); 
      //    int curColIdx = 0; 
      //    foreach (ILPlot2DGraph graph in graphs) {
      //          graph.Line.Color = Color.FromKnownColor(colors[curColIdx++ % colors.Length]); 
      //    }
      //}
   }
}