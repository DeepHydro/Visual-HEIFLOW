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
using System.Windows.Forms;

namespace Heiflow.Core.Animation
{
    public  class DataCubeAnimation:IDataCubeAnimation
    {
        public event EventHandler<int> CurrentChanged;
        public event EventHandler<IDataCubeObject> DataSourceChanged;
        public event EventHandler RequiredUpdated;
        public event EventHandler Stopped;
        protected Timer _Timer;
        protected int _Speed;
        protected int _Current;
        protected int _Maximum;
        protected int _Minimum;
        protected IDataCubeObject _DataSource;
        protected string _Name;
        public DataCubeAnimation()
        {
            _Timer = new Timer();
            _Timer.Tick += _Timer_Tick;
            _Timer.Interval = 500;
             Speed = 500;
             _Current = -1;
             _Name = "Animator";
        }

        public IDataCubeObject DataSource
        {
            get
            {
                return _DataSource;
            }
            set
            {
                _DataSource = value;
                if (_DataSource != null)
                {
                    _Maximum = _DataSource.Size[1] - 1;
                    _Minimum = 0;
                    OnDataSourceChanged();
                }
            }
        }

        public string Name
        {
            get
            {
                return _Name;
            }
        }

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
            get
            {
                return _Maximum;
            }
            set
            {
                _Maximum = value;
            }
        }

        public int Minimum
        {
            get
            {
                return _Minimum;
            }
            set
            {
                _Minimum = value;
            }
        }

        public int Speed
        {
            get
            {
                return _Speed;
            }
            set
            {
                _Speed = value;
                _Timer.Interval = _Speed;
            }
        }

        public virtual void Play()
        {
            _Timer.Start();
        }

        public virtual void Stop()
        { 
            _Timer.Stop();
            Current = 0;
        }

        public virtual void Go(int step)
        {
            Current = step;
            Plot(_Current);
        }

        public virtual void Pause()
        {
            _Timer.Stop();
        }

        protected virtual void _Timer_Tick(object sender, EventArgs e)
        {
            if (Current >= Maximum)
            {
                _Timer.Stop();
                Current = 0;
                OnStopped();
            }
            else
            {
                Plot(Current);
                OnCurrentChanged();
                _Current++;
            }
        }

        public virtual void Initialize()
        {

        }
        protected virtual void Plot(int time_index)
        {

        }

        public virtual void Cache()
        {

        }

        protected void OnDataSourceChanged()
        {
            if (DataSourceChanged != null)
                DataSourceChanged(this, _DataSource);
        }
     
        protected void OnCurrentChanged()
        {
            if (CurrentChanged != null)
                CurrentChanged(this, _Current);
        }

        protected void OnRequiredUpdated()
        {
            if (RequiredUpdated != null)
                RequiredUpdated(this, EventArgs.Empty);
        }

        protected void OnStopped()
        {
            if (Stopped != null)
                Stopped(this, EventArgs.Empty);
        }

    }
}
