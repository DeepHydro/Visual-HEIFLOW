// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
