// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
