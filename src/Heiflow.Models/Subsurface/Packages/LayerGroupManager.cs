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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface
{
    public delegate void LayerGroupChangedHandler (object sender, LayerGroup lg, string ChangedProp);
    public class LayerGroupManager
    {
        private ObservableCollection<LayerGroup> _Layers;
        private string[] _LayerProperties;
        public event LayerGroupChangedHandler LayerGroupChanged;
       
        public LayerGroupManager()
        {
            _Layers = new ObservableCollection<LayerGroup>();
            Initialized = false;
            _LayerProperties = new string[] { "HK", "VKA", "SS", "SY", "WETDRY" };
        }

        public ObservableCollection<LayerGroup> LayerGroups
        {
            get
            {
                return _Layers;
            }
        }

        public bool Initialized
        {
            get;
            private set;
        }

        public void Initialize(int num_layer)
        {
            Clear();
            float hk0 = (float)Math.Round(20.0f / num_layer, 0);
            float ss = 0.0001f;
            float sy = 0.1f;
            float wetdry = 0.1f;

            for (int i = 0; i < num_layer; i++)
            {
                LayerGroup layer = new LayerGroup()
                {
                    Name = "Layer " + (i + 1),
                    CHANI = CHANI.Define,
                    LAYAVG = LAYAVG.Harmonic_Mean,
                    LayerIndex = i,
                    LAYTYP = LAYTYP.Convertable,
                    LAYVKA = LAYVKA.Ratio_of_horizontal_to_vertical_hydraulic_conductivity,
                    LAYWET = LAYWET.Active,
                    HK = 20 - i * hk0,
                    VKA = 100,
                    SS = ss,
                    SY = sy,
                    WETDRY = wetdry
                };
                _Layers.Add(layer);
            }
            Initialized = true;
        }

        public void Add(int pre_index)
        {
            int index = pre_index + 1;
            LayerGroup layer = new LayerGroup()
            {
                Name = "Layer " + (index + 1),
                CHANI = CHANI.Define,
                LAYAVG = LAYAVG.Harmonic_Mean,
                LayerIndex = index,
                LAYTYP = LAYTYP.Convertable,
                LAYVKA = LAYVKA.Ratio_of_horizontal_to_vertical_hydraulic_conductivity,
                LAYWET = LAYWET.Active
            };
            if (index >= 1)
            {
                layer.LAYTYP = LAYTYP.Confined;
                layer.LAYWET = LAYWET.Inactive;
            }
            _Layers.Insert(index, layer);
            Refresh();
        }

        public void Remove(LayerGroup layer)
        {
            _Layers.Remove(layer);
            Refresh();
        }

        public void Refresh()
        {
            for (int i = 0; i < _Layers.Count; i++)
            {
                var layer = _Layers[i];
                layer.Name = "Layer " + (i + 1);
                layer.LayerIndex = i;
            }
        }
        public void OnItemChanged(LayerGroup lg, string prop)
        {
            if (LayerGroupChanged != null)
                LayerGroupChanged(this, lg, prop);
        }
        public void Clear()
        {
            _Layers.Clear();
        }
 

        public void ConvertFrom(int[] values, string enum_name)
        {
            switch (enum_name)
            {
                case "LAYAVG":
                    for (int i = 0; i < _Layers.Count; i++)
                    {
                        _Layers[i].LAYAVG = (LAYAVG)values[i];
                    }
                    break;
                case "LAYTYP":
                    for (int i = 0; i < _Layers.Count; i++)
                    {
                        if (values[i] == 0)
                        {
                            _Layers[i].LAYTYP = LAYTYP.Confined;
                        }
                        else
                        {
                            _Layers[i].LAYTYP = LAYTYP.Convertable;
                        }
                    }
                    break;
                case "LAYVKA":
                    for (int i = 0; i < _Layers.Count; i++)
                    {
                        if (values[i] == 0)
                        {
                            _Layers[i].LAYVKA = LAYVKA.Vertical_hydraulic_conductivity;
                        }
                        else
                        {
                            _Layers[i].LAYVKA = LAYVKA.Ratio_of_horizontal_to_vertical_hydraulic_conductivity;
                        }
                    }
                    break;
                case "LAYWET":
                    for (int i = 0; i < _Layers.Count; i++)
                    {
                        if (values[i] == 0)
                        {
                            _Layers[i].LAYWET = LAYWET.Inactive;
                        }
                        else
                        {
                            _Layers[i].LAYWET = LAYWET.Active;
                        }
                    }
                    break;
            };
        }
        public void ConvertFrom(float[] values, string enum_name)
        {
            switch (enum_name)
            {
                case "CHANI":
                    for (int i = 0; i < _Layers.Count; i++)
                    {
                        if (values[i] <= 0)
                        {
                            _Layers[i].CHANI = CHANI.Define;
                        }
                        else
                        {
                            _Layers[i].CHANI = CHANI.NotDefine;
                        }
                    }
                    break;
            };
        }
        public int[] ConvertToInt(string enum_name)
        {
            int ncount = _Layers.Count;
            int[] values = new int[ncount];

            switch (enum_name)
            {
                case "CHANI":
                    for (int i = 0; i < ncount; i++)
                    {
                        values[i] = (int)_Layers[i].CHANI;
                    }
                    break;
                case "LAYAVG":
                    for (int i = 0; i < ncount; i++)
                    {
                        values[i] = (int)_Layers[i].LAYAVG;
                    }
                    break;
                case "LAYTYP":
                    for (int i = 0; i < ncount; i++)
                    {
                        values[i] = (int)_Layers[i].LAYTYP;
                    }
                    break;
                case "LAYVKA":
                    for (int i = 0; i < ncount; i++)
                    {
                        values[i] = (int)_Layers[i].LAYVKA;
                    }
                    break;
                case "LAYWET":
                    for (int i = 0; i < ncount; i++)
                    {
                        values[i] = (int)_Layers[i].LAYWET;
                    }
                    break; 
            };

            return values;
        }

        public float[] ConvertToFloat(string enum_name)
        {
            int ncount = _Layers.Count;
            float[] values = new float[ncount];

            switch (enum_name)
            {
                case "HK":
                    for (int i = 0; i < ncount; i++)
                    {
                        values[i] = (float)_Layers[i].HK;
                    }
                    break;
                case "VKA":
                    for (int i = 0; i < ncount; i++)
                    {
                        values[i] = (float)_Layers[i].VKA;
                    }
                    break;
                case "SS":
                    for (int i = 0; i < ncount; i++)
                    {
                        values[i] = (float)_Layers[i].SS;
                    }
                    break;
                case "SY":
                    for (int i = 0; i < ncount; i++)
                    {
                        values[i] = (float)_Layers[i].SY;
                    }
                    break;
                case "WETDRY":
                    for (int i = 0; i < ncount; i++)
                    {
                        values[i] = (float)_Layers[i].WETDRY;
                    }
                    break;
                case "CHANI":
                    for (int i = 0; i < ncount; i++)
                    {
                        values[i] = (float)_Layers[i].CHANI;
                    }
                    break;
            };

            return values;
        }
   
    
    }
}
