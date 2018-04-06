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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface
{
    public class LayerGroup:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double _VKA;
        public LayerGroup()
        {
            Name = "Aquifer Layer";
            LayerIndex = 1;
            LAYTYP = Subsurface.LAYTYP.Convertable;
            LAYAVG = Subsurface.LAYAVG.Harmonic_Mean;
            LAYVKA = Subsurface.LAYVKA.Ratio_of_horizontal_to_vertical_hydraulic_conductivity;
            LAYWET = Subsurface.LAYWET.Active;
            CHANI = Subsurface.CHANI.Define;
            LayerHeight = 20;
            HK = 10;
            VKA = 100;
            SS = 0.0001;
            SY = 0.1;
            WETDRY = 0.1;
        }
        public int LayerIndex { get; set; }
        public string Name { get; set; }
        public LAYTYP LAYTYP { get; set; }
        public LAYAVG LAYAVG { get; set; }
        public LAYVKA LAYVKA { get; set; }
        public LAYWET LAYWET { get; set; }
        public CHANI CHANI { get; set; }
        public double LayerHeight { get; set; }
        public double HK { get; set; }
        public double VKA
        {
            get
            {
                return _VKA;
            }
            set
            {
                _VKA = value;
                OnPropertyChanged("VKA");
            }
        }
        public double SS { get; set; }
        public double SY { get; set; }
        public double WETDRY { get; set; }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
