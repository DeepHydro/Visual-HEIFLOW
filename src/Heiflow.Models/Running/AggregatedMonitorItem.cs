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

namespace Heiflow.Models.Running
{
    public class AggregatedMonitorItem : MonitorItem
    {
        public AggregatedMonitorItem(string name)
            : base(name)
        {
            Source = new List<MonitorItem>();
            SourceSign = new List<int>();
            Derivable = true;
        }

        public List<MonitorItem> Source
        {
            get;
            set;
        }

        public List<int> SourceSign
        {
            get;
            set;
        }

        public override double[] Derive(ListTimeSeries<double> sourcedata)
        {
            int c = 0;
            if (Source.Count > 0)
            {
                foreach (var item in Source)
                {
                    if (item.Derivable && item.DerivedValues == null)
                    {
                        item.DerivedValues = item.Derive(sourcedata);
                    }
                }

                double[] values = new double[sourcedata.Dates.Count];

                for (int i = 0; i < sourcedata.Dates.Count; i++)
                {
                    c = 0;
                    foreach (var item in Source)
                    {
                        if (item.Derivable)
                        {
                            values[i] += item.DerivedValues[i] * SourceSign[c];
                        }
                        else
                        {
                            values[i] += sourcedata.Values[item.VariableIndex][i] * SourceSign[c];
                        }
                        c++;
                    }
                }
                return values;
            }
            else
            {
                return null;
            }
        }
    }
}
