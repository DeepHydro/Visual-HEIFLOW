using DotSpatial.Data;
using System;
namespace Heiflow.Models.Subsurface
{
    public interface IFlowPropertyPackage
    {
        bool IsDirty { get; set; }
        float[] CHANI { get; set; }
        Heiflow.Core.Data.DataCube<float> HANI { get; set; }
        float HDRY { get; set; }
        Heiflow.Core.Data.DataCube<float> HK { get; set; }
        int IPHDRY { get; set; }
        int IWETIT { get; set; }
        int[] LAYAVG { get; set; }
        int[] LAYTYP { get; set; }
        int[] LAYVKA { get; set; }
        int[] LAYWET { get; set; }
        Heiflow.Core.Data.DataCube<float> SS { get; set; }
        Heiflow.Core.Data.DataCube<float> SY { get; set; }
        Heiflow.Core.Data.DataCube<float> VKA { get; set; }
        Heiflow.Core.Data.DataCube<float> WETDRY { get; set; }
        void Save(ICancelProgressHandler progress);
    }
}