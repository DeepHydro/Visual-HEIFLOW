using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Surface.PRMS
{
    public class SoilLayerManager
    {
        private ObservableCollection<SoilLayer> _Layers;

        public SoilLayerManager()
        {
            _Layers = new ObservableCollection<SoilLayer>();
        }

        public ObservableCollection<SoilLayer> Layers
        {
            get
            {
                return _Layers;
            }
        }

        public int LayerCount
        {
            get
            {
                return _Layers.Count;
            }
        }

        public void Generate(int nlayer)
        {
            _Layers.Clear();
            for (int i = 0; i < nlayer; i++)
            {
                SoilLayer layer = new SoilLayer()
                {
                    LayerIndex = i,
                    SoilDepth = i
                };
                _Layers.Add(layer);
            }
            _Layers[0].SoilDepth = 0.4f;
        }
    }
}
