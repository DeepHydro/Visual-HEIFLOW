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

using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using System.IO;
using System.Windows.Forms;

namespace Heiflow.Models.Subsurface.VFT3D
{
    /// <summary>
    /// Common behaviour of the reactive transport projects (VFT3D and SEAWAT).
    /// They ship the PHT3D thermodynamic database together with the project and
    /// do not need the legacy IHM project file.
    /// </summary>
    public abstract class PhreaticProjectBase : ModflowProject
    {
        protected override void OnCreated()
        {
            CopyPhreaticDatabase();
            SaveBatchRunFile();
        }

        /// <summary>
        /// Copies the PHT3D database that ships with the application into the project directory.
        /// </summary>
        protected void CopyPhreaticDatabase()
        {
            var phc_dbfile = Path.Combine(Application.StartupPath, "data\\pht3d_datab.dat");
            if (File.Exists(phc_dbfile))
            {
                var dest = Path.Combine(AbsolutePathToProjectFile, "pht3d_datab.dat");
                File.Copy(phc_dbfile, dest, true);
            }
        }
    }
}
