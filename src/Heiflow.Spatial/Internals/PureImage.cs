//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

namespace Heiflow.Spatial
{
   using System;
   using System.IO;

   /// <summary>
   /// image abstraction proxy
   /// </summary>
   public abstract class PureImageProxy
   {
      abstract public PureImage FromStream(Stream stream);
      abstract public bool Save(Stream stream, PureImage image);

      public PureImage FromArray(byte[] data)
      {
         MemoryStream m = new MemoryStream(data, 0, data.Length, false, true);
         var pi = FromStream(m);
         if(pi != null)
         {
            m.Position = 0;
            pi.Data = m;
         }
         else
         {
            m.Dispose();
            m = null;
         }
         return pi;
      }
   }

   /// <summary>
   /// image abstraction
   /// </summary>
   public abstract class PureImage : IDisposable
   {
      public MemoryStream Data;

      #region IDisposable Members

      public abstract void Dispose();

      #endregion
   }
}
