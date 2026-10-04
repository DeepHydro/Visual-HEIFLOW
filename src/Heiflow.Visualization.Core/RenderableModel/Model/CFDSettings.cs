using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HUST.WREIS.Dot3D
{
       [Serializable]
    public class CFDSettings : HUST.WREIS.Dot3D.Configuration.SettingsBase
    {
           public CFDSettings()
               : base()
        {
            HistogramLevel = 256;
        }

           private int mHistogramLevel;

        public int HistogramLevel 
        {
            get
            {
                return mHistogramLevel;
            }
            set
            {
                if (value > 256)
                    value = 256;
                if (value < 0)
                    value = 0;
                mHistogramLevel = value;
            }
        }
    }
}
