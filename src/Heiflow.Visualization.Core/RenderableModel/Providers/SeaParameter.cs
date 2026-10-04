using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HUST.WREIS.Dot3D.Renderable
{
    public enum dtDatatypes
    {
        dtBool = 0,
        dtFloat,
        dtInt
    };

    public enum pParameters
    {
        p_fStrength = 0,
        p_fFalloff,
        p_fScale,
        p_bSmooth,
        p_bReflRefrStrength,
        p_iOctaves,
        p_fLODbias,
        p_fAnimspeed,
        p_fTimemulti,
        p_bPaused,
        p_bAsPoints,
        p_fElevation,
        p_bDisplayTargets,
        p_fSunPosTheta,
        p_fSunPosAlpha,
        p_fSunShininess,
        p_fSunStrength,
        p_fWaterColourR,
        p_fWaterColourG,
        p_fWaterColourB,
        p_bDisplace,
        p_bDrawDuckie,
        p_bDrawIsland,
        p_bDiffuseRefl,
        numParameters
    };

   public struct parameter
    {
        public string name;
        public int datatype;

        public int iData;
        public bool bData;
        public float fData;
    };

    public class SeaParameter
    {
        public float f_step = 0.0001f;
        public float f_stepXL = 0.001f;
        private int id;
    //    string text;
        int numParameters = 24;
     public   parameter[] parames;

        public SeaParameter()
        {
            parames = new parameter[numParameters];
            add_parameter((int)pParameters.p_fStrength, "Noise strength", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_bDisplace, "Toggle displacement", (int)dtDatatypes.dtBool);
            add_parameter((int)pParameters.p_bSmooth, "Smooth heightmap", (int)dtDatatypes.dtBool);
            add_parameter((int)pParameters.p_bReflRefrStrength, "Reflection/Refraction strength", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_iOctaves, "Octaves", (int)dtDatatypes.dtInt);
            add_parameter((int)pParameters.p_fScale, "Noise scale", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_fFalloff, "Noise falloff", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_fAnimspeed, "Animation speed", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_fTimemulti, "Animation multi", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_bPaused, "Pause animation", (int)dtDatatypes.dtBool);
            add_parameter((int)pParameters.p_bDisplayTargets, "Display rendertargets(D)", (int)dtDatatypes.dtBool);
            add_parameter((int)pParameters.p_fSunPosAlpha, "Sun location horizontal", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_fSunPosTheta, "Sun location vertical", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_fSunShininess, "Sun shininess", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_fSunStrength, "Sun strength", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_bAsPoints, "Render as points", (int)dtDatatypes.dtBool);
            add_parameter((int)pParameters.p_fLODbias, "Mipmap LOD Bias", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_fElevation, "Projector elevation", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_fWaterColourR, "water colour Red", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_fWaterColourG, "water colour Green", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_fWaterColourB, "water colour Blue", (int)dtDatatypes.dtFloat);
            add_parameter((int)pParameters.p_bDrawDuckie, "Render Duckie", (int)dtDatatypes.dtBool);
            add_parameter((int)pParameters.p_bDrawIsland, "Render Island", (int)dtDatatypes.dtBool);
            add_parameter((int)pParameters.p_bDiffuseRefl, "Diffuse sky reflection", (int)dtDatatypes.dtBool);
        }

        public bool add_parameter(int id, string name, int datatype)
        {
            parames[id].datatype = datatype;
            return true;
        }

        public void next_parameter()
        {
            id = (id + 1) % numParameters;
        }
        void previous_parameter()
        { 
            id = (id - 1 + numParameters) % numParameters; 
        }

       public float get_float(pParameters id){ return parames[(int)id].fData; }
      public  int get_int(pParameters id){ return parames[(int)id].iData; }
     public   bool get_bool(pParameters id){ return parames[(int)id].bData; }
    public    void set_float(pParameters id, float value){ parames[(int)id].fData = value; }
    public    void set_int(pParameters id, int value){ parames[(int)id].iData = value; }
   public     void set_bool(pParameters id, bool value){ parames[(int)id].bData = value; }

        public void value_increase()
        {
            switch ((dtDatatypes)parames[id].datatype)
            {
                case dtDatatypes.dtBool:
                    {
                        parames[id].bData = !parames[id].bData;
                        break;
                    }
                case dtDatatypes.dtInt:
                    {
                        parames[id].iData += 1;
                        break;
                    }
                case dtDatatypes.dtFloat:
                    {
                        parames[id].fData += f_step;
                        break;
                    }
            }
        }

        public void value_decrease()
        {
            switch ((dtDatatypes)parames[id].datatype)
            {
                case dtDatatypes.dtBool:
                    {
                        parames[id].bData = !parames[id].bData;
                        break;
                    }
                case dtDatatypes.dtInt:
                    {
                        parames[id].iData -= 1;
                        break;
                    }
                case dtDatatypes.dtFloat:
                    {
                        parames[id].fData -= f_step;
                        break;
                    }
            };
        }

        void value_increaseXL()
        {
            switch ((dtDatatypes)parames[id].datatype)
            {
                case dtDatatypes.dtBool:
                    {
                        parames[id].bData = !parames[id].bData;
                        break;
                    }
                case dtDatatypes.dtInt:
                    {
                        parames[id].iData += 10;
                        break;
                    }
                case dtDatatypes.dtFloat:
                    {
                        parames[id].fData *= 1.05f;
                        break;
                    }
            };

        }

        void value_decreaseXL()
        {
            switch ((dtDatatypes)parames[id].datatype)
            {
                case dtDatatypes.dtBool:
                    {
                        parames[id].bData = !parames[id].bData;
                        break;
                    }
                case dtDatatypes.dtInt:
                    {
                        parames[id].iData -= 10;
                        break;
                    }
                case dtDatatypes.dtFloat:
                    {
                        parames[id].fData *= 0.95f;
                        break;
                    }
            };
        }

        void value_reset()
        {
            switch ((dtDatatypes)parames[id].datatype)
            {
                case dtDatatypes.dtBool:
                    {
                        //parames[id].bData = !parames[id].bData;
                        break;
                    }
                case dtDatatypes.dtInt:
                    {
                        //parames[id].iData -= 1;
                        break;
                    }
                case dtDatatypes.dtFloat:
                    {
                        parames[id].fData = 0;
                        break;
                    }
            }


        }


    }

}
