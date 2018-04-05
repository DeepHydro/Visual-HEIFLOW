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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.Hydrology
{
    public class HydroLine : HydroFeature
    {
        public HydroLine(int id)
            : base(id)
        {
            //HydroFeatureType = HydroFeatureType.HydroLine;
            //Width = 20;
            //Length = 1000;
            //TopElevation = 500;
            //Slope = 0.001;
            //BedThick = 3;
        }

        public double Slope { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double TopElevation { get; set; }
        public double BedThick { get; set; }

        //public int InletNodeIndex { get; set; }
        //public int OutletNodeIndex { get; set; }

        public HydroPoint InletNode { get; set; }
        public HydroPoint OutletNode { get; set; }

        public double ETSW { get; set; }
        public double PPTSW { get; set; }
        public double ROUGHCH { get; set; }
        public double Width1 { get; set; }
        public double Width2 { get; set; }

        /// <summary>
        /// Row * Column = Variable * Time Step
        /// </summary>
        public double[,] AnimationMatrix { get; set; }
    }

}
