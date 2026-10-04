using DotSpatial.Data;
using HUST.WREIS.Dot3D;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Heiflow.Visualization.Tool
{
    public class ExtractTerrain
    {
        public ExtractTerrain()
        {
            
        }

        public void ExtractTo(string _Filename)
        {
            FileStream ffs = new FileStream(_Filename + ".elv", FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(ffs);

            var _IFeatureSet = FeatureSet.Open(_Filename);
            var dt = _IFeatureSet.DataTable;
            var nshpx = _IFeatureSet.ShapeIndices.Count;
            var vertices = _IFeatureSet.Vertex;
            var world = ConfigurationManager.DrawArgs.CurrentWorld;
            double Y = 0;
            double X = 0;
            byte[] bybes = new byte[1];
            if (!dt.Columns.Contains("Terrain"))
            {
                //DataColumn dc = new DataColumn("Terrain", Type.GetType("System.Byte[]"));
                DataColumn dc = new DataColumn("Terrain", Type.GetType("System.String"));
                dt.Columns.Add(dc);
            }
            bw.Write(nshpx);
            for (int n = 0; n < nshpx; n++)
            {
                var shpx = _IFeatureSet.ShapeIndices[n];
                int start = shpx.Parts[0].StartIndex;
                int end = shpx.Parts[shpx.Parts.Count - 1].EndIndex;
                int npt = end - start + 1;

                bw.Write(npt);
                for (int i = start; i <= end; i++)
                {
                    X = vertices[2 * i];
                    Y = vertices[2 * i + 1];
                    var elev = world.TerrainAccessor.GetElevationAt(Y, X) ;// World.Settings.VerticalExaggeration;
                    bw.Write(elev);
                }
            }
            ffs.Close();
            bw.Close();
            _IFeatureSet.Close();
        }

        public byte[] ToBytes(float[] floatArray1)
        {
            var byteArray = new byte[floatArray1.Length * 4];
            Buffer.BlockCopy(floatArray1, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }
    }
}
