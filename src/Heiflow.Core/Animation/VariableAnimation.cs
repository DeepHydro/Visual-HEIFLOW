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

using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Animation
{
     public  abstract class VariableAnimation
    {
         protected int _Speed;
         protected int _Current;
         public event EventHandler<int> CurrentChanged;
         public event EventHandler DataSourceChanged;
         public event EventHandler RequiredUpdated;
         protected MyArray<float> _DataSource;
         protected int _AnimatedDimension1;
         protected int _AnimatedDimension2;

         public VariableAnimation()
         {
             _AnimatedDimension1 = 0;
             _AnimatedDimension2 = 0;
             _Speed = 500;
             _Current = 0;
         }

        public MyArray<float> DataSource
        {
            get
            {
                return _DataSource;
            }
            set
            {
                _DataSource = value;
                OnDataSourceChanged();
            }
        }

        public int VariableIndex
        {
            get;
            set;
        }
         /// <summary>
         /// starting from 0
         /// </summary>
        public int Current
        {
            get
            {
                return _Current;
            }
            set
            {
                _Current = value;
                OnCurrentChanged();
            }
        }

        public int Maximum
        {
            get;
            set;
        }

        public int Minimum
        {
            get;
            set;
        }
         /// <summary>
         /// in mini seconds
         /// </summary>
        public int Speed
        {
            get
            { 
                return _Speed; 
            }
            set
            {
                _Speed = value;
            }
        }
         /// <summary>
        /// Dimension index for the Current property  
         /// </summary>
        public int AnimatedDimension1
        {
            get
            {
                return _AnimatedDimension1;
            }
            set
            {
                _AnimatedDimension1 = value;
            }
        }
         /// <summary>
        /// Dimension index for the VariableIndex property
         /// </summary>
        public int AnimatedDimension2
        {
            get
            {
                return _AnimatedDimension2;
            }
            set
            {
                _AnimatedDimension2 = value;
            }
        }

        public abstract void Play();

        public abstract void Stop();

        public abstract void Go(int time_index);

        public abstract void Pause();

        public abstract void Initialize();

        protected virtual void OnCurrentChanged()
        {
            if (CurrentChanged != null)
                CurrentChanged(this,_Current);
        }

        protected virtual void OnDataSourceChanged()
        {
            if (DataSourceChanged != null)
                DataSourceChanged(this, new EventArgs());
        }

        protected virtual void OnRequiredUpdated()
        {
            if (RequiredUpdated != null)
                RequiredUpdated(this, new EventArgs());
        }



        public string VariableName
        {
            get;
            set;
        }
    }
}
