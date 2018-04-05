// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic.Project
{
    internal class PackageChangeTracker
    {
        private List<IPackage> _Packages;
        public event EventHandler PackagePropertyChanged;

        public PackageChangeTracker()
        {
            _Packages = new List<IPackage>();
        }

        public List<IPackage> Packages
        {
            get
            {
                return _Packages;
            }
        }

        public void Add(IPackage pck)
        {
            if(!_Packages.Contains(pck))
            {
                _Packages.Add(pck);
                pck.PropertyChanged += pck_PropertyChanged;
            }
        }
        public void Remove(IPackage pck)
        {
            if (_Packages.Contains(pck))
            {
                _Packages.Remove(pck);
                pck.PropertyChanged -= pck_PropertyChanged;
            }
        }

        void pck_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (PackagePropertyChanged != null)
                PackagePropertyChanged(this, new EventArgs());
        }   
    }
}
